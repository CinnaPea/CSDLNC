using System.Data;
using CSDLNC.Data;
using CSDLNC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CSDLNC.Controllers;

[Authorize]
public class TraSachController : Controller
{
    private readonly ThuVienDbContext _db;

    public TraSachController(ThuVienDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(TraSachIndexViewModel model)
    {
        if (!HasPermission("Q004"))
            return RedirectToAction("AccessDenied", "Account");

        model.Page = Math.Max(1, model.Page);
        model.PageSize = model.PageSize is 8 or 15 or 30 ? model.PageSize : 8;

        var filteredQuery = BuildPhieuMuonCanTraQuery(model);
        model.TotalItems = await filteredQuery.CountAsync();

        if (model.Page > model.TotalPages)
            model.Page = model.TotalPages;

        model.PhieuMuonCanTra = await filteredQuery
            .OrderBy(x => x.Hantra)
            .ThenBy(x => x.Sophieumuon)
            .Skip((model.Page - 1) * model.PageSize)
            .Take(model.PageSize)
            .Select(x => new PhieuMuonCanTraViewModel
            {
                SoPhieuMuon = x.Sophieumuon,
                MaSinhVien = x.Masinhvien,
                HoTen = x.MasinhvienNavigation.Hoten,
                NgayMuon = x.Ngaymuon,
                HanTra = x.Hantra,
                SoSachChuaTra = x.CtPhieuMuons.Count(ct => ct.Ngaytra == null)
            })
            .ToListAsync();

        var optionItems = await BuildPhieuMuonCanTraQuery(new TraSachIndexViewModel
            {
                TuKhoa = model.TuKhoa,
                TrangThaiHan = model.TrangThaiHan,
                TuNgay = model.TuNgay,
                DenNgay = model.DenNgay
            })
            .OrderBy(x => x.Hantra)
            .ThenBy(x => x.Sophieumuon)
            .Take(200)
            .Select(x => new PhieuMuonCanTraViewModel
            {
                SoPhieuMuon = x.Sophieumuon,
                MaSinhVien = x.Masinhvien,
                HoTen = x.MasinhvienNavigation.Hoten,
                NgayMuon = x.Ngaymuon,
                HanTra = x.Hantra,
                SoSachChuaTra = x.CtPhieuMuons.Count(ct => ct.Ngaytra == null)
            })
            .ToListAsync();

        model.PhieuMuonOptions = BuildPhieuMuonOptions(optionItems, model.SoPhieuMuon);

        if (!string.IsNullOrWhiteSpace(model.SoPhieuMuon))
        {
            model.ChiTiet = await LayChiTietPhieuMuonAsync(model.SoPhieuMuon.Trim());

            if (!model.ChiTiet.Any())
                TempData["InfoMessage"] = $"Không tìm thấy phiếu mượn {model.SoPhieuMuon}.";
        }

        return View(model);
    }

    private IQueryable<PhieuMuon> BuildPhieuMuonCanTraQuery(TraSachIndexViewModel model)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = _db.PhieuMuons
            .AsNoTracking()
            .Where(x => x.CtPhieuMuons.Any(ct => ct.Ngaytra == null))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(model.TuKhoa))
        {
            var keyword = model.TuKhoa.Trim();
            query = query.Where(x =>
                x.Sophieumuon.Contains(keyword)
                || x.Masinhvien.Contains(keyword)
                || x.MasinhvienNavigation.Hoten.Contains(keyword));
        }

        if (model.TuNgay.HasValue)
        {
            var fromDate = DateOnly.FromDateTime(model.TuNgay.Value.Date);
            query = query.Where(x => x.Hantra >= fromDate);
        }

        if (model.DenNgay.HasValue)
        {
            var toDate = DateOnly.FromDateTime(model.DenNgay.Value.Date);
            query = query.Where(x => x.Hantra <= toDate);
        }

        query = model.TrangThaiHan switch
        {
            "QuaHan" => query.Where(x => x.Hantra < today),
            "ConHan" => query.Where(x => x.Hantra >= today),
            _ => query
        };

        return query;
    }

    private static List<SelectListItem> BuildPhieuMuonOptions(
        IEnumerable<PhieuMuonCanTraViewModel> phieuMuons,
        string? selectedSoPhieuMuon)
    {
        return phieuMuons
            .Select(x => new SelectListItem
            {
                Value = x.SoPhieuMuon,
                Text = $"{x.SoPhieuMuon} - {x.MaSinhVien} - {x.HoTen} ({x.SoSachChuaTra} chưa trả)",
                Selected = x.SoPhieuMuon == selectedSoPhieuMuon
            })
            .ToList();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhat(CapNhatTraSachViewModel model)
    {
        if (!HasPermission("Q004"))
            return RedirectToAction("AccessDenied", "Account");

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Thông tin trả sách chưa hợp lệ.";
            return RedirectToAction(nameof(Index), new { soPhieuMuon = model.SoPhieuMuon });
        }

        if (model.TinhTrangSauTra != "Tốt")
        {
            TempData["ErrorMessage"] = "Sách hỏng, mất hoặc quá hạn phải lập phiếu phạt tại màn hình PhieuPhat; cập nhật trả sách chỉ dùng cho trả bình thường.";
            return RedirectToAction(nameof(Index), new { soPhieuMuon = model.SoPhieuMuon });
        }

        var hanTra = await _db.PhieuMuons
            .Where(x => x.Sophieumuon == model.SoPhieuMuon)
            .Select(x => x.Hantra)
            .FirstOrDefaultAsync();

        if (hanTra != default && DateOnly.FromDateTime(model.NgayTra.Date) > hanTra)
        {
            TempData["ErrorMessage"] = "Sách quá hạn phải lập phiếu phạt quá hạn; cập nhật trả sách chỉ dùng cho trả đúng hạn.";
            return RedirectToAction(nameof(Index), new { soPhieuMuon = model.SoPhieuMuon });
        }

        try
        {
            await CapNhatTraSachAsync(model);
            TempData["SuccessMessage"] = $"Đã cập nhật trả sách {model.MaSach}.";
        }
        catch (SqlException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        var stillHasOpenBooks = await _db.CtPhieuMuons
            .AnyAsync(x => x.Sophieumuon == model.SoPhieuMuon && x.Ngaytra == null);

        return RedirectToAction(nameof(Index), stillHasOpenBooks ? new { soPhieuMuon = model.SoPhieuMuon } : null);
    }

    private async Task<List<ChiTietPhieuMuonViewModel>> LayChiTietPhieuMuonAsync(string soPhieuMuon)
    {
        var results = new List<ChiTietPhieuMuonViewModel>();
        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_LayChiTietPhieuMuon";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@SoPhieuMuon", SqlDbType.VarChar, 20) { Value = soPhieuMuon });

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new ChiTietPhieuMuonViewModel
                {
                    SoPhieuMuon = ReadString(reader, "sophieumuon"),
                    NgayMuon = ReadDate(reader, "ngaymuon"),
                    HanTra = ReadDate(reader, "hantra"),
                    TrangThaiPhieu = ReadString(reader, "trangthaiphieu"),
                    MaSinhVien = ReadString(reader, "masinhvien"),
                    HoTen = ReadString(reader, "hoten"),
                    Lop = ReadNullableString(reader, "lop"),
                    Khoa = ReadNullableString(reader, "khoa"),
                    SoDienThoai = ReadNullableString(reader, "sodienthoai"),
                    MaSach = ReadString(reader, "masach"),
                    TenSach = ReadString(reader, "tensach"),
                    TheLoai = ReadNullableString(reader, "theloai"),
                    TacGia = ReadNullableString(reader, "tacgia"),
                    NgayTra = ReadNullableDate(reader, "ngaytra"),
                    TinhTrang = ReadString(reader, "tinhtrang"),
                    GhiChu = ReadNullableString(reader, "ghichu")
                });
            }
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }

        return results;
    }

    private async Task CapNhatTraSachAsync(CapNhatTraSachViewModel model)
    {
        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_CapNhatTraSach";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@SoPhieuMuon", SqlDbType.VarChar, 20) { Value = model.SoPhieuMuon.Trim() });
            command.Parameters.Add(new SqlParameter("@MaSach", SqlDbType.VarChar, 20) { Value = model.MaSach.Trim() });
            command.Parameters.Add(new SqlParameter("@NgayTra", SqlDbType.Date) { Value = model.NgayTra.Date });
            command.Parameters.Add(new SqlParameter("@TinhTrangSauTra", SqlDbType.NVarChar, 50) { Value = model.TinhTrangSauTra });
            command.Parameters.Add(new SqlParameter("@GhiChu", SqlDbType.NVarChar, 255)
            {
                Value = string.IsNullOrWhiteSpace(model.GhiChu) ? DBNull.Value : model.GhiChu.Trim()
            });

            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }

    private static string ReadString(IDataRecord reader, string name)
    {
        return reader[name]?.ToString() ?? "";
    }

    private static string? ReadNullableString(IDataRecord reader, string name)
    {
        return reader[name] == DBNull.Value ? null : reader[name].ToString();
    }

    private static DateTime ReadDate(IDataRecord reader, string name)
    {
        return Convert.ToDateTime(reader[name]);
    }

    private static DateTime? ReadNullableDate(IDataRecord reader, string name)
    {
        return reader[name] == DBNull.Value ? null : Convert.ToDateTime(reader[name]);
    }

    private bool HasPermission(string permission)
    {
        return User.HasClaim("Permission", permission)
            || User.HasClaim("Permission", "Q001")
            || User.HasClaim("Permission", "Q010");
    }
}
