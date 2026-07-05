using CSDLNC.Data;
using CSDLNC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CSDLNC.Controllers;

[Authorize]
public class BanGiaoController : Controller
{
    private readonly ThuVienDbContext _db;

    public BanGiaoController(ThuVienDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!CanUseBanGiao())
            return Forbid();

        var model = await BuildModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BanGiaoCreateViewModel model)
    {
        if (!CanUseBanGiao())
            return Forbid();

        if (string.IsNullOrWhiteSpace(model.Manhacungcap))
            ModelState.AddModelError(nameof(model.Manhacungcap), "Vui lòng chọn nhà cung cấp.");

        if (string.IsNullOrWhiteSpace(model.Madausach))
            ModelState.AddModelError(nameof(model.Madausach), "Vui lòng chọn đầu sách.");

        if (model.Soluongsachmoidausach <= 0)
            ModelState.AddModelError(nameof(model.Soluongsachmoidausach), "Số lượng phải lớn hơn 0.");

        var dauSach = await _db.DauSaches
            .FirstOrDefaultAsync(x => x.Madausach == model.Madausach);

        if (dauSach == null)
            ModelState.AddModelError(nameof(model.Madausach), "Mã đầu sách không tồn tại.");

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildModelAsync();
            invalidModel.Sobienban = model.Sobienban;
            invalidModel.Manhacungcap = model.Manhacungcap;
            invalidModel.Madausach = model.Madausach;
            invalidModel.Soluongsachmoidausach = model.Soluongsachmoidausach;
            invalidModel.Ghichu = model.Ghichu;
            invalidModel.DaidienbenbangiaoId = model.DaidienbenbangiaoId;
            invalidModel.DaidienbennhanId = model.DaidienbennhanId;
            invalidModel.Daidienbenbangiao = model.Daidienbenbangiao;
            invalidModel.Chucvubenbangiao = model.Chucvubenbangiao;
            invalidModel.Daidienbennhan = model.Daidienbennhan;
            invalidModel.Chucvubennhan = model.Chucvubennhan;
            return View(invalidModel);
        }

        var benBanGiao = await _db.NguoiDungs.Include(x => x.Manhoms).FirstOrDefaultAsync(x => x.Manguoidung == model.DaidienbenbangiaoId);
        var benNhan = await _db.NguoiDungs.Include(x => x.Manhoms).FirstOrDefaultAsync(x => x.Manguoidung == model.DaidienbennhanId);

        model.Daidienbenbangiao = benBanGiao?.Tennguoidung;
        model.Chucvubenbangiao = benBanGiao?.Manhoms.FirstOrDefault()?.Tennhom;

        model.Daidienbennhan = benNhan?.Tennguoidung;
        model.Chucvubennhan = benNhan?.Manhoms.FirstOrDefault()?.Tennhom;

        var maNguoiDung = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tenNguoiDung = User.Identity?.Name;

        var bienBan = new BienBanNhanBanGiao
        {
            Sobienban = model.Sobienban,
            Ngaylap = DateOnly.FromDateTime(DateTime.Today),
            Manhacungcap = model.Manhacungcap,
            Manguoidung = maNguoiDung,
            Tennguoidung = tenNguoiDung,
            Daidienbenbangiao = model.Daidienbenbangiao,
            Chucvubenbangiao = model.Chucvubenbangiao,
            Daidienbennhan = model.Daidienbennhan,
            Chucvubennhan = model.Chucvubennhan,
            Tongsodausach = 1,
            Tongsoluongsach = model.Soluongsachmoidausach
        };

        var chiTiet = new CtBienBanNhanBanGiao
        {
            Sobienban = model.Sobienban,
            Madausach = model.Madausach!,
            Soluongsachmoidausach = model.Soluongsachmoidausach,
            Ghichu = model.Ghichu
        };

        var nextSachNumber = await GetNextSachNumberAsync();
        for (var i = 0; i < model.Soluongsachmoidausach; i++)
        {
            _db.Saches.Add(new Sach
            {
                Masach = "S" + (nextSachNumber + i).ToString("000"),
                Madausach = dauSach!.Madausach,
                Tensach = dauSach.Tendausach,
                Theloai = dauSach.Theloai,
                Tacgia = dauSach.Tacgia,
                Nhaxuatban = dauSach.Manhaxuatban,
                Namxuatban = dauSach.Namxuatban,
                Tinhtrang = "Có thể mượn",
                Trangthai = "Trong kho",
                Ngaynhap = DateOnly.FromDateTime(DateTime.Today),
                Ngaycapnhattrangthai = DateOnly.FromDateTime(DateTime.Today)
            });
        }

        dauSach!.Soluong += model.Soluongsachmoidausach;
        dauSach.Soluonghienco += model.Soluongsachmoidausach;

        _db.BienBanNhanBanGiaos.Add(bienBan);
        _db.CtBienBanNhanBanGiaos.Add(chiTiet);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Lập biên bản nhận bàn giao sách thành công.";
        return RedirectToAction(nameof(Create));
    }

    private bool CanUseBanGiao()
    {
        return User.HasClaim("Permission", "Q006") ||
               User.HasClaim("Permission", "Q001");
    }

    private async Task<BanGiaoCreateViewModel> BuildModelAsync()
    {
        var nguoiDungs = await _db.NguoiDungs.Include(x => x.Manhoms).OrderBy(x => x.Manguoidung).ToListAsync();

        var nhanSuOptions = nguoiDungs.Select(x => new NhanSuOption
        {
            MaNguoiDung = x.Manguoidung,
            TenNguoiDung = x.Tennguoidung ?? x.Tendangnhap,
            ChucVu = x.Manhoms.FirstOrDefault()?.Tennhom ?? "Nhân sự"
        }).ToList();

        return new BanGiaoCreateViewModel
        {
            NhanSuOptions = nhanSuOptions,
            Sobienban = await GenerateSoBienBanAsync(),
            NhaCungCapOptions = await _db.NhaCungCaps
                .OrderBy(x => x.Manhacungcap)
                .Select(x => new SelectListItem
                {
                    Value = x.Manhacungcap,
                    Text = x.Manhacungcap + " - " + x.Tennhacungcap
                })
                .ToListAsync(),
            DauSachOptions = await _db.DauSaches
                .OrderBy(x => x.Madausach)
                .Select(x => new SelectListItem
                {
                    Value = x.Madausach,
                    Text = x.Madausach + " - " + x.Tendausach
                })
                .ToListAsync(),
            BienBans = await _db.BienBanNhanBanGiaos
                .Include(x => x.ManhacungcapNavigation)
                .OrderByDescending(x => x.Ngaylap)
                .ThenByDescending(x => x.Sobienban)
                .Select(x => new BanGiaoRow
                {
                    Sobienban = x.Sobienban,
                    Ngaylap = x.Ngaylap,
                    Manhacungcap = x.Manhacungcap,
                    Tennhacungcap = x.ManhacungcapNavigation != null ? x.ManhacungcapNavigation.Tennhacungcap : "",
                    Tongsoluongsach = x.Tongsoluongsach,
                    Tongsodausach = x.Tongsodausach
                })
                .ToListAsync()
        };
    }

    private async Task<string> GenerateSoBienBanAsync()
    {
        var prefix = "BB" + DateTime.Now.ToString("yyyyMMdd");

        var countToday = await _db.BienBanNhanBanGiaos
            .CountAsync(x => x.Sobienban.StartsWith(prefix));

        return prefix + (countToday + 1).ToString("00");
    }

    private async Task<int> GetNextSachNumberAsync()
    {
        var maSaches = await _db.Saches.Select(x => x.Masach).ToListAsync();

        return maSaches
            .Select(x => x.StartsWith("S", StringComparison.OrdinalIgnoreCase) && int.TryParse(x[1..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;
    }
}
