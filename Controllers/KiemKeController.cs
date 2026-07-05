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

        var tinhTrang = model.Tinhtrang!;
        sach!.Tinhtrang = tinhTrang;
        sach.Trangthai = tinhTrang == "Có thể mượn" ? "Trong kho" : tinhTrang;
        sach.Sophieumuonhientai = tinhTrang == "Đang mượn" ? sach.Sophieumuonhientai : null;
        sach.Ngaycapnhattrangthai = DateOnly.FromDateTime(DateTime.Today);

        if (tinhTrang != "Đang mượn")
        {
            var openLoanDetails = await _context.CtPhieuMuons
                .Include(x => x.SophieumuonNavigation)
                .ThenInclude(x => x.MasinhvienNavigation)
                .Where(x => x.Masach == model.Masach && x.Ngaytra == null)
                .ToListAsync();

            foreach (var detail in openLoanDetails)
            {
                detail.Ngaytra = DateOnly.FromDateTime(DateTime.Today);
                detail.Ghichu = "Đóng bởi kiểm kê: " + tinhTrang;

                var loan = detail.SophieumuonNavigation;
                if (loan.MasinhvienNavigation != null && loan.MasinhvienNavigation.Sosachdangmuon > 0)
                    loan.MasinhvienNavigation.Sosachdangmuon--;

                await _context.Entry(loan).Collection(x => x.CtPhieuMuons).LoadAsync();

                if (loan.CtPhieuMuons.All(x => x.Ngaytra != null))
                {
                    loan.Trangthaiphieu = "Đã trả";
                    loan.Ngayhoantat = DateOnly.FromDateTime(DateTime.Today);
                }
            }
        }

        _context.PhieuKiemKes.Add(phieu);
        _context.CtPhieuKiemKes.Add(chiTiet);

        await _context.SaveChangesAsync();
        await ResyncDauSachAsync(sach.Madausach);

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

    private async Task ResyncDauSachAsync(string? maDauSach)
    {
        if (string.IsNullOrWhiteSpace(maDauSach))
            return;

        var dauSach = await _context.DauSaches.FirstOrDefaultAsync(x => x.Madausach == maDauSach);
        if (dauSach == null)
            return;

        var saches = await _context.Saches.Where(x => x.Madausach == maDauSach).ToListAsync();
        dauSach.Soluong = saches.Count;
        dauSach.Soluonghienco = saches.Count(x => x.Tinhtrang == "Có thể mượn");
        dauSach.Soluongdangmuon = saches.Count(x => x.Tinhtrang == "Đang mượn");
        dauSach.Soluonghongmat = saches.Count(x => x.Tinhtrang == "Hỏng" || x.Tinhtrang == "Mất");
        dauSach.Lanmuongannhat = saches.Max(x => x.Ngaymuongannhat);

        await _context.SaveChangesAsync();
    }
}
