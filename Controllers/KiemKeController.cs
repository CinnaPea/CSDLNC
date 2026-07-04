using CSDLNC.Data;
using CSDLNC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CSDLNC.Controllers;

[Authorize]
public class KiemKeController : Controller
{
    private readonly ThuVienDbContext _context;

    public KiemKeController(ThuVienDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Create(string? masach)
    {
        if (!CanUseKiemKe())
            return Forbid();

        var model = await BuildModelAsync();

        if (!string.IsNullOrWhiteSpace(masach))
        {
            var sach = await _context.Saches.FirstOrDefaultAsync(x => x.Masach == masach);

            if (sach != null)
            {
                model.Masach = sach.Masach;
                model.Tensach = sach.Tensach;
                model.Tinhtrang = sach.Tinhtrang;
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KiemKeCreateViewModel model)
    {
        if (!CanUseKiemKe())
            return Forbid();

        if (string.IsNullOrWhiteSpace(model.Masach))
            ModelState.AddModelError(nameof(model.Masach), "Vui lòng chọn mã sách.");

        if (string.IsNullOrWhiteSpace(model.Tinhtrang))
            ModelState.AddModelError(nameof(model.Tinhtrang), "Vui lòng chọn tình trạng.");

        var sach = await _context.Saches.FirstOrDefaultAsync(x => x.Masach == model.Masach);

        if (sach == null)
            ModelState.AddModelError(nameof(model.Masach), "Mã sách không tồn tại.");

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildModelAsync();
            invalidModel.Makiemke = model.Makiemke;
            invalidModel.Masach = model.Masach;
            invalidModel.Tensach = model.Tensach;
            invalidModel.Tinhtrang = model.Tinhtrang;
            invalidModel.Ghichu = model.Ghichu;
            return View(invalidModel);
        }

        var maNguoiDung = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tenNguoiDung = User.Identity?.Name;

        var phieu = new PhieuKiemKe
        {
            Makiemke = model.Makiemke,
            Ngaykiemke = DateOnly.FromDateTime(DateTime.Today),
            Manguoidung = maNguoiDung,
            Tennguoidung = tenNguoiDung,
            Nguoilapphieu = tenNguoiDung
        };

        var chiTiet = new CtPhieuKiemKe
        {
            Makiemke = model.Makiemke,
            Masach = model.Masach!,
            Ghichu = model.Ghichu
        };

        sach!.Tinhtrang = model.Tinhtrang;
        sach.Ngaycapnhattrangthai = DateOnly.FromDateTime(DateTime.Today);

        _context.PhieuKiemKes.Add(phieu);
        _context.CtPhieuKiemKes.Add(chiTiet);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Ghi kiểm kê sách thành công.";
        return RedirectToAction(nameof(Create));
    }

    private bool CanUseKiemKe()
    {
        return User.HasClaim("Permission", "Q006") ||
               User.HasClaim("Permission", "Q001");
    }

    private async Task<KiemKeCreateViewModel> BuildModelAsync()
    {
        return new KiemKeCreateViewModel
        {
            Makiemke = await GenerateMaKiemKeAsync(),
            TinhTrangOptions = new List<SelectListItem>
            {
                new("Có thể mượn", "Có thể mượn"),
                new("Đang mượn", "Đang mượn"),
                new("Hỏng", "Hỏng"),
                new("Mất", "Mất")
            },
            Saches = await _context.Saches
                .OrderBy(x => x.Masach)
                .Select(x => new KiemKeSachRow
                {
                    Masach = x.Masach,
                    Tensach = x.Tensach,
                    Theloai = x.Theloai,
                    Tacgia = x.Tacgia,
                    Namxuatban = x.Namxuatban,
                    Tinhtrang = x.Tinhtrang,
                    Trangthai = x.Trangthai,
                    Madausach = x.Madausach,
                    Mavitri = x.Mavitri
                })
                .ToListAsync()
        };
    }

    private async Task<string> GenerateMaKiemKeAsync()
    {
        var prefix = "KK" + DateTime.Now.ToString("yyyyMMdd");

        var countToday = await _context.PhieuKiemKes
            .CountAsync(x => x.Makiemke.StartsWith(prefix));

        return prefix + (countToday + 1).ToString("00");
    }
}