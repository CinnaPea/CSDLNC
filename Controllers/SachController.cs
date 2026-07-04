using CSDLNC.Data;
using CSDLNC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CSDLNC.Controllers;

[Authorize]
public class SachController : Controller
{
    private readonly ThuVienDbContext _db;

    public SachController(ThuVienDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? madausach)
    {
        if (!CanUseDanhMucSach())
            return Forbid();

        var model = await BuildModelAsync();

        if (!string.IsNullOrWhiteSpace(madausach))
        {
            var dauSach = await _db.DauSaches.FirstOrDefaultAsync(x => x.Madausach == madausach);

            if (dauSach != null)
            {
                model.Madausach = dauSach.Madausach;
                model.Tendausach = dauSach.Tendausach;
                model.Theloai = dauSach.Theloai;
                model.Tacgia = dauSach.Tacgia;
                model.Namxuatban = dauSach.Namxuatban;
                model.Manhaxuatban = dauSach.Manhaxuatban;
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Them(DanhMucSachViewModel model)
    {
        if (!CanUseDanhMucSach())
            return Forbid();

        if (string.IsNullOrWhiteSpace(model.Madausach))
            ModelState.AddModelError(nameof(model.Madausach), "Vui lòng nhập mã đầu sách.");

        if (string.IsNullOrWhiteSpace(model.Tendausach))
            ModelState.AddModelError(nameof(model.Tendausach), "Vui lòng nhập tên đầu sách.");

        var exists = await _db.DauSaches.AnyAsync(x => x.Madausach == model.Madausach);

        if (exists)
            ModelState.AddModelError(nameof(model.Madausach), "Mã đầu sách đã tồn tại.");

        if (!ModelState.IsValid)
            return View("Index", await RebuildModelAsync(model));

        var dauSach = new DauSach
        {
            Madausach = model.Madausach!,
            Tendausach = model.Tendausach ?? "",
            Theloai = model.Theloai,
            Tacgia = model.Tacgia,
            Namxuatban = model.Namxuatban,
            Manhaxuatban = model.Manhaxuatban,
            Soluong = 0,
            Soluonghienco = 0,
            Soluongdangmuon = 0,
            Soluonghongmat = 0
        };

        _db.DauSaches.Add(dauSach);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Thêm đầu sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhat(DanhMucSachViewModel model)
    {
        if (!CanUseDanhMucSach())
            return Forbid();

        var dauSach = await _db.DauSaches
            .FirstOrDefaultAsync(x => x.Madausach == model.Madausach);

        if (dauSach == null)
        {
            TempData["Error"] = "Không tìm thấy đầu sách cần cập nhật.";
            return RedirectToAction(nameof(Index));
        }

        dauSach.Tendausach = model.Tendausach ?? "";
        dauSach.Theloai = model.Theloai;
        dauSach.Tacgia = model.Tacgia;
        dauSach.Namxuatban = model.Namxuatban;
        dauSach.Manhaxuatban = model.Manhaxuatban;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Cập nhật đầu sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(DanhMucSachViewModel model)
    {
        if (!CanUseDanhMucSach())
            return Forbid();

        var dauSach = await _db.DauSaches
            .FirstOrDefaultAsync(x => x.Madausach == model.Madausach);

        if (dauSach == null)
        {
            TempData["Error"] = "Không tìm thấy đầu sách cần xóa.";
            return RedirectToAction(nameof(Index));
        }

        var hasSach = await _db.Saches.AnyAsync(x => x.Madausach == model.Madausach);

        if (hasSach)
        {
            TempData["Error"] = "Không thể xóa vì đầu sách đã có sách chi tiết.";
            return RedirectToAction(nameof(Index), new { madausach = model.Madausach });
        }

        _db.DauSaches.Remove(dauSach);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Xóa đầu sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    private bool CanUseDanhMucSach()
    {
        return User.HasClaim("Permission", "Q002") ||
               User.HasClaim("Permission", "Q001");
    }

    private async Task<DanhMucSachViewModel> RebuildModelAsync(DanhMucSachViewModel model)
    {
        var rebuilt = await BuildModelAsync();

        rebuilt.Madausach = model.Madausach;
        rebuilt.Tendausach = model.Tendausach;
        rebuilt.Theloai = model.Theloai;
        rebuilt.Tacgia = model.Tacgia;
        rebuilt.Namxuatban = model.Namxuatban;
        rebuilt.Manhaxuatban = model.Manhaxuatban;

        return rebuilt;
    }

    private async Task<string> GenerateMaDauSachAsync()
    {
        var existingCodes = await _db.DauSaches
            .Where(x => x.Madausach.StartsWith("DS"))
            .Select(x => x.Madausach)
            .ToListAsync();

        var maxNumber = existingCodes
            .Select(x =>
            {
                var numberPart = x.Length > 2 ? x.Substring(2) : "0";
                return int.TryParse(numberPart, out var n) ? n : 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        return "DS" + (maxNumber + 1).ToString("000");
    }

    private async Task<DanhMucSachViewModel> BuildModelAsync()
    {
        return new DanhMucSachViewModel
        {
            Madausach = await GenerateMaDauSachAsync(),
            TheLoaiOptions = new List<SelectListItem>
                {
                    new("Công nghệ thông tin", "Công nghệ thông tin"),
                    new("Lập trình", "Lập trình"),
                    new("Mạng máy tính", "Mạng máy tính"),
                    new("An toàn thông tin", "An toàn thông tin"),
                    new("Trí tuệ nhân tạo", "Trí tuệ nhân tạo"),
                    new("Toán học", "Toán học"),
                    new("Vật lý", "Vật lý"),
                    new("Quân sự", "Quân sự"),
                    new("Ngoại ngữ", "Ngoại ngữ")
                },
            NhaXuatBanOptions = await _db.NhaXuatBans.OrderBy(x => x.Manhaxuatban).Select(x => new SelectListItem
                {
                    Value = x.Manhaxuatban,
                    Text = x.Manhaxuatban + " - " + x.Tennhaxuatban
                }).ToListAsync(),

            Rows = await _db.DauSaches.OrderBy(x => x.Madausach).Select(x => new DanhMucSachRow
                {
                    Madausach = x.Madausach,
                    Tendausach = x.Tendausach,
                    Theloai = x.Theloai,
                    Tacgia = x.Tacgia,
                    Namxuatban = x.Namxuatban,
                    Manhaxuatban = x.Manhaxuatban,
                    Soluong = x.Soluong,
                    Soluonghienco = x.Soluonghienco,
                    Soluongdangmuon = x.Soluongdangmuon,
                    Soluonghongmat = x.Soluonghongmat
                }).ToListAsync()
        };
    }
}