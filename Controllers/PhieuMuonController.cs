using System.Data;
using CSDLNC.Data;
using CSDLNC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CSDLNC.Controllers;

[Authorize]
public class PhieuMuonController : Controller
{
    private readonly ThuVienDbContext _db;

    public PhieuMuonController(ThuVienDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? tuKhoa)
    {
        if (!HasPermission("Q003"))
            return RedirectToAction("AccessDenied", "Account");

        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = _db.PhieuMuons
            .AsNoTracking()
            .Include(x => x.MasinhvienNavigation)
            .Include(x => x.CtPhieuMuons)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(x =>
                x.Sophieumuon.Contains(keyword)
                || x.Masinhvien.Contains(keyword)
                || x.MasinhvienNavigation.Hoten.Contains(keyword));
        }

        var allQuery = _db.PhieuMuons.AsNoTracking();
        var model = new PhieuMuonIndexViewModel
        {
            TuKhoa = tuKhoa,
            TongPhieu = await allQuery.CountAsync(),
            DangMuon = await allQuery.CountAsync(x => x.CtPhieuMuons.Any(ct => ct.Ngaytra == null)),
            DaTra = await allQuery.CountAsync(x => x.Ngayhoantat != null),
            QuaHan = await allQuery.CountAsync(x => x.Hantra < today && x.CtPhieuMuons.Any(ct => ct.Ngaytra == null)),
            DanhSachPhieu = await query
                .OrderByDescending(x => x.Ngaymuon)
                .ThenByDescending(x => x.Sophieumuon)
                .Take(100)
                .Select(x => new PhieuMuonListItemViewModel
                {
                    SoPhieuMuon = x.Sophieumuon,
                    MaSinhVien = x.Masinhvien,
                    TenSinhVien = x.MasinhvienNavigation.Hoten,
                    NgayMuon = x.Ngaymuon,
                    HanTra = x.Hantra,
                    NgayTra = x.Ngayhoantat,
                    TrangThai = x.Trangthaiphieu,
                    TongSoLuong = x.Tongsoluong,
                    SoSachChuaTra = x.CtPhieuMuons.Count(ct => ct.Ngaytra == null)
                })
                .ToListAsync()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!HasPermission("Q003"))
            return RedirectToAction("AccessDenied", "Account");

        var model = new PhieuMuonCreateViewModel
        {
            SoPhieuMuon = await TaoSoPhieuMuonGoiYAsync(),
            MaSinhVienGoiY = await TaoMaSinhVienGoiYAsync()
        };

        await PopulateCreateOptionsAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (!HasAnyPermission("Q003", "Q004"))
            return RedirectToAction("AccessDenied", "Account");

        if (string.IsNullOrWhiteSpace(id))
            return RedirectToAction(nameof(Index));

        var model = new PhieuMuonDetailsViewModel
        {
            SoPhieuMuon = id.Trim(),
            ChiTiet = await LayChiTietPhieuMuonAsync(id.Trim())
        };

        if (!model.ChiTiet.Any())
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (!HasPermission("Q003"))
            return RedirectToAction("AccessDenied", "Account");

        if (string.IsNullOrWhiteSpace(id))
            return RedirectToAction(nameof(Index));

        try
        {
            var deleted = await XoaPhieuMuonAsync(new[] { id });
            TempData[deleted > 0 ? "SuccessMessage" : "ErrorMessage"] =
                deleted > 0 ? $"Đã xóa phiếu mượn {id}." : "Không tìm thấy phiếu mượn cần xóa.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Không thể xóa phiếu này vì đã phát sinh dữ liệu liên quan.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSelected(List<string> selectedIds)
    {
        if (!HasPermission("Q003"))
            return RedirectToAction("AccessDenied", "Account");

        if (selectedIds.Count == 0)
        {
            TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một phiếu cần xóa.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var deleted = await XoaPhieuMuonAsync(selectedIds);
            TempData["SuccessMessage"] = $"Đã xóa {deleted} phiếu mượn được chọn.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Có phiếu đã phát sinh dữ liệu liên quan nên không thể xóa hàng loạt.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PhieuMuonCreateViewModel model)
    {
        if (!HasPermission("Q003"))
            return RedirectToAction("AccessDenied", "Account");

        model.DanhSachSach = model.DanhSachSach
            .Where(x => !string.IsNullOrWhiteSpace(x.MaSach))
            .Select(x => new PhieuMuonSachInputModel
            {
                MaSach = x.MaSach!.Trim(),
                GhiChu = string.IsNullOrWhiteSpace(x.GhiChu) ? null : x.GhiChu.Trim()
            })
            .ToList();

        if (!model.DanhSachSach.Any())
            ModelState.AddModelError("", "Vui lòng nhập ít nhất một mã sách.");

        if (model.HanTra.Date < model.NgayMuon.Date)
            ModelState.AddModelError(nameof(model.HanTra), "Hạn trả phải sau hoặc bằng ngày mượn.");

        var duplicateBook = model.DanhSachSach
            .GroupBy(x => x.MaSach, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(x => x.Count() > 1);

        if (duplicateBook != null)
            ModelState.AddModelError("", $"Mã sách {duplicateBook.Key} bị nhập trùng.");

        if (!ModelState.IsValid)
        {
            if (!model.DanhSachSach.Any())
                model.DanhSachSach.Add(new PhieuMuonSachInputModel());

            await PopulateCreateOptionsAsync(model);
            return View(model);
        }

        try
        {
            await LapPhieuMuonAsync(model);
            TempData["SuccessMessage"] = $"Đã lập phiếu mượn {model.SoPhieuMuon}.";
            return RedirectToAction(nameof(Details), new { id = model.SoPhieuMuon.Trim() });
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateCreateOptionsAsync(model);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStudentQuick(PhieuMuonSinhVienCreateViewModel model)
    {
        if (!HasPermission("Q003"))
            return Forbid();

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            return BadRequest(new { message = string.Join(" ", errors) });
        }

        var maSinhVien = model.MaSinhVien.Trim();

        if (await _db.SinhViens.AnyAsync(x => x.Masinhvien == maSinhVien))
            return BadRequest(new { message = "Mã sinh viên đã tồn tại." });

        var sinhVien = new SinhVien
        {
            Masinhvien = maSinhVien,
            Hoten = model.HoTen.Trim(),
            Gioitinh = string.IsNullOrWhiteSpace(model.GioiTinh) ? null : model.GioiTinh.Trim(),
            Lop = string.IsNullOrWhiteSpace(model.Lop) ? null : model.Lop.Trim(),
            Khoa = string.IsNullOrWhiteSpace(model.Khoa) ? null : model.Khoa.Trim(),
            Sodienthoai = string.IsNullOrWhiteSpace(model.SoDienThoai) ? null : model.SoDienThoai.Trim(),
            Sosachdangmuon = 0,
            Solanvipham = 0,
            Sotienphatchuatra = 0,
            Trangthai = "Đang học"
        };

        _db.SinhViens.Add(sinhVien);
        await _db.SaveChangesAsync();

        return Json(new
        {
            code = sinhVien.Masinhvien,
            name = sinhVien.Hoten,
            className = sinhVien.Lop,
            course = sinhVien.Khoa,
            phone = sinhVien.Sodienthoai,
            status = sinhVien.Trangthai,
            nextCode = await TaoMaSinhVienGoiYAsync()
        });
    }

    private async Task PopulateCreateOptionsAsync(PhieuMuonCreateViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.MaSinhVienGoiY))
            model.MaSinhVienGoiY = await TaoMaSinhVienGoiYAsync();

        model.SinhVienGoiY = await _db.SinhViens
            .OrderBy(x => x.Masinhvien)
            .Take(10)
            .Select(x => new SinhVienMuonSachOptionViewModel
            {
                MaSinhVien = x.Masinhvien,
                HoTen = x.Hoten,
                GioiTinh = x.Gioitinh,
                Lop = x.Lop,
                Khoa = x.Khoa,
                SoDienThoai = x.Sodienthoai,
                SoSachDangMuon = x.Sosachdangmuon,
                TrangThai = x.Trangthai
            })
            .ToListAsync();

        model.SachCoTheMuon = await _db.Saches
            .Where(x => x.Tinhtrang == "Có thể mượn")
            .OrderBy(x => x.Masach)
            .Take(12)
            .Select(x => new SachMuonOptionViewModel
            {
                MaSach = x.Masach,
                MaDauSach = x.Madausach,
                TenSach = x.Tensach,
                TheLoai = x.Theloai,
                TacGia = x.Tacgia,
                NhaXuatBan = x.Nhaxuatban,
                TinhTrang = x.Tinhtrang
            })
            .ToListAsync();
    }

    private async Task<string> TaoSoPhieuMuonGoiYAsync()
    {
        var maPhieu = await _db.PhieuMuons
            .Select(x => x.Sophieumuon)
            .ToListAsync();

        var nextNumber = maPhieu
            .Select(x => x.StartsWith("PM", StringComparison.OrdinalIgnoreCase) && int.TryParse(x[2..], out var number) ? number : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return "PM" + nextNumber.ToString("000");
    }

    private async Task<string> TaoMaSinhVienGoiYAsync()
    {
        var maSinhVien = await _db.SinhViens
            .Select(x => x.Masinhvien)
            .ToListAsync();

        var nextNumber = maSinhVien
            .Select(x => x.StartsWith("SV", StringComparison.OrdinalIgnoreCase) && int.TryParse(x[2..], out var number) ? number : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return "SV" + nextNumber.ToString("000");
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
            command.CommandText = """
                SELECT 
                    pm.sophieumuon,
                    pm.ngaymuon,
                    pm.hantra,
                    pm.trangthaiphieu,
                    sv.masinhvien,
                    sv.hoten,
                    sv.gioitinh,
                    sv.lop,
                    sv.khoa,
                    sv.sodienthoai,
                    s.masach,
                    s.tensach,
                    s.theloai,
                    s.tacgia,
                    s.nhaxuatban,
                    s.namxuatban,
                    ct.ngaytra,
                    s.tinhtrang,
                    ct.ghichu
                FROM phieu_muon pm
                INNER JOIN sinh_vien sv ON pm.masinhvien = sv.masinhvien
                INNER JOIN ct_phieu_muon ct ON pm.sophieumuon = ct.sophieumuon
                INNER JOIN sach s ON ct.masach = s.masach
                WHERE pm.sophieumuon = @SoPhieuMuon
                ORDER BY s.masach;
                """;
            command.CommandType = CommandType.Text;
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
                    GioiTinh = ReadNullableString(reader, "gioitinh"),
                    Lop = ReadNullableString(reader, "lop"),
                    Khoa = ReadNullableString(reader, "khoa"),
                    SoDienThoai = ReadNullableString(reader, "sodienthoai"),
                    MaSach = ReadString(reader, "masach"),
                    TenSach = ReadString(reader, "tensach"),
                    TheLoai = ReadNullableString(reader, "theloai"),
                    TacGia = ReadNullableString(reader, "tacgia"),
                    NhaXuatBan = ReadNullableString(reader, "nhaxuatban"),
                    NamXuatBan = ReadNullableInt(reader, "namxuatban"),
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

    private static int? ReadNullableInt(IDataRecord reader, string name)
    {
        return reader[name] == DBNull.Value ? null : Convert.ToInt32(reader[name]);
    }

    private async Task LapPhieuMuonAsync(PhieuMuonCreateViewModel model)
    {
        var danhSachSach = new DataTable();
        danhSachSach.Columns.Add("MaSach", typeof(string));
        danhSachSach.Columns.Add("GhiChu", typeof(string));

        foreach (var item in model.DanhSachSach)
            danhSachSach.Rows.Add(item.MaSach, item.GhiChu ?? (object)DBNull.Value);

        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_LapPhieuMuonTraSach";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@SoPhieuMuon", SqlDbType.VarChar, 20) { Value = model.SoPhieuMuon.Trim() });
            command.Parameters.Add(new SqlParameter("@MaSinhVien", SqlDbType.VarChar, 20) { Value = model.MaSinhVien.Trim() });
            command.Parameters.Add(new SqlParameter("@NgayMuon", SqlDbType.Date) { Value = model.NgayMuon.Date });
            command.Parameters.Add(new SqlParameter("@HanTra", SqlDbType.Date) { Value = model.HanTra.Date });
            command.Parameters.Add(new SqlParameter("@DanhSachSach", SqlDbType.Structured)
            {
                TypeName = "dbo.DanhSachSachMuon",
                Value = danhSachSach
            });

            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }

    private async Task<int> XoaPhieuMuonAsync(IEnumerable<string> soPhieuMuons)
    {
        var ids = soPhieuMuons
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (ids.Count == 0)
            return 0;

        await using var transaction = await _db.Database.BeginTransactionAsync();

        var phieuMuons = await _db.PhieuMuons
            .Include(x => x.CtPhieuMuons)
            .Include(x => x.MasinhvienNavigation)
            .Where(x => ids.Contains(x.Sophieumuon))
            .ToListAsync();

        foreach (var phieuMuon in phieuMuons)
        {
            var soSachChuaTra = phieuMuon.CtPhieuMuons.Count(x => x.Ngaytra == null);
            var maSaches = phieuMuon.CtPhieuMuons
                .Where(x => x.Ngaytra == null)
                .Select(x => x.Masach)
                .ToList();

            var saches = await _db.Saches
                .Where(x => maSaches.Contains(x.Masach))
                .ToListAsync();

            foreach (var sach in saches)
            {
                if (!string.Equals(sach.Sophieumuonhientai, phieuMuon.Sophieumuon, StringComparison.OrdinalIgnoreCase))
                    continue;

                sach.Tinhtrang = "Có thể mượn";
                sach.Trangthai = "Trong kho";
                sach.Sophieumuonhientai = null;
                sach.Ngaycapnhattrangthai = DateOnly.FromDateTime(DateTime.Today);
            }

            if (phieuMuon.MasinhvienNavigation != null && soSachChuaTra > 0)
            {
                phieuMuon.MasinhvienNavigation.Sosachdangmuon =
                    Math.Max(0, phieuMuon.MasinhvienNavigation.Sosachdangmuon - soSachChuaTra);
            }

            _db.CtPhieuMuons.RemoveRange(phieuMuon.CtPhieuMuons);
            _db.PhieuMuons.Remove(phieuMuon);
        }

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return phieuMuons.Count;
    }

    private bool HasPermission(string permission)
    {
        return User.HasClaim("Permission", permission)
            || User.HasClaim("Permission", "Q001")
            || User.HasClaim("Permission", "Q010");
    }

    private bool HasAnyPermission(params string[] permissions)
    {
        return permissions.Any(HasPermission);
    }
}
