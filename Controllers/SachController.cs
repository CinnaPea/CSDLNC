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
            var dauSach = await _db.DauSaches
                .Include(x => x.Saches)
                .FirstOrDefaultAsync(x => x.Madausach == madausach);

            if (dauSach != null)
            {
                model.Madausach = dauSach.Madausach;
                model.Tendausach = dauSach.Tendausach;
                model.Theloai = dauSach.Theloai;
                model.Tacgia = dauSach.Tacgia;
                model.Namxuatban = dauSach.Namxuatban;
                model.Manhaxuatban = dauSach.Manhaxuatban;
                model.SoLuongBan = dauSach.Saches.Count;
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

        if (model.SoLuongBan <= 0)
            ModelState.AddModelError(nameof(model.SoLuongBan), "Số lượng bản sách phải lớn hơn 0.");

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
            Soluong = model.SoLuongBan,
            Soluonghienco = model.SoLuongBan,
            Soluongdangmuon = 0,
            Soluonghongmat = 0
        };

        _db.DauSaches.Add(dauSach);
        await AddPhysicalCopiesAsync(dauSach, model.SoLuongBan);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Thêm đầu sách thành công và tạo {model.SoLuongBan} bản sách có thể mượn.";
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

        if (model.SoLuongBan < 0)
        {
            TempData["Error"] = "Số lượng bản sách không được nhỏ hơn 0.";
            return RedirectToAction(nameof(Index), new { madausach = model.Madausach });
        }

        dauSach.Tendausach = model.Tendausach ?? "";
        dauSach.Theloai = model.Theloai;
        dauSach.Tacgia = model.Tacgia;
        dauSach.Namxuatban = model.Namxuatban;
        dauSach.Manhaxuatban = model.Manhaxuatban;

        try
        {
            await SyncPhysicalCopiesAsync(dauSach, model.SoLuongBan);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index), new { madausach = model.Madausach });
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Cập nhật đầu sách thành công. Tổng số bản hiện là {model.SoLuongBan}.";
        return RedirectToAction(nameof(Index), new { madausach = model.Madausach });
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
        rebuilt.SoLuongBan = model.SoLuongBan;

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

    private async Task<int> GenerateNextMaSachNumberAsync()
    {
        var existingCodes = await _db.Saches
            .Where(x => x.Masach.StartsWith("S"))
            .Select(x => x.Masach)
            .ToListAsync();

        return existingCodes
            .Select(x =>
            {
                var numberPart = x.Length > 1 ? x.Substring(1) : "0";
                return int.TryParse(numberPart, out var n) ? n : 0;
            })
            .DefaultIfEmpty(0)
            .Max() + 1;
    }

    private async Task AddPhysicalCopiesAsync(DauSach dauSach, int quantity)
    {
        if (quantity <= 0)
            return;

        var nextNumber = await GenerateNextMaSachNumberAsync();
        var nhaXuatBan = string.IsNullOrWhiteSpace(dauSach.Manhaxuatban)
            ? null
            : await _db.NhaXuatBans
                .Where(x => x.Manhaxuatban == dauSach.Manhaxuatban)
                .Select(x => x.Tennhaxuatban)
                .FirstOrDefaultAsync();

        for (var i = 0; i < quantity; i++)
        {
            _db.Saches.Add(new Sach
            {
                Masach = "S" + (nextNumber + i).ToString("000"),
                Madausach = dauSach.Madausach,
                Tensach = dauSach.Tendausach,
                Theloai = dauSach.Theloai,
                Tacgia = dauSach.Tacgia,
                Nhaxuatban = nhaXuatBan ?? dauSach.Manhaxuatban,
                Namxuatban = dauSach.Namxuatban,
                Tinhtrang = "Có thể mượn",
                Trangthai = "Trong kho",
                Ngaynhap = DateOnly.FromDateTime(DateTime.Today),
                Ngaycapnhattrangthai = DateOnly.FromDateTime(DateTime.Today)
            });
        }
    }

    private async Task SyncPhysicalCopiesAsync(DauSach dauSach, int targetQuantity)
    {
        var copies = await _db.Saches
            .Where(x => x.Madausach == dauSach.Madausach)
            .ToListAsync();

        foreach (var copy in copies)
        {
            copy.Tensach = dauSach.Tendausach;
            copy.Theloai = dauSach.Theloai;
            copy.Tacgia = dauSach.Tacgia;
            copy.Namxuatban = dauSach.Namxuatban;
            copy.Nhaxuatban = dauSach.Manhaxuatban;
        }

        var currentQuantity = copies.Count;
        if (targetQuantity > currentQuantity)
        {
            await AddPhysicalCopiesAsync(dauSach, targetQuantity - currentQuantity);
        }
        else if (targetQuantity < currentQuantity)
        {
            var quantityToRemove = currentQuantity - targetQuantity;
            var removableCopies = await _db.Saches
                .Where(x => x.Madausach == dauSach.Madausach
                    && x.Tinhtrang == "Có thể mượn"
                    && !x.CtPhieuMuons.Any()
                    && !x.CtPhieuKiemKes.Any()
                    && !x.CtPhieuPhatQuaHans.Any()
                    && !x.CtPhieuPhatHongMats.Any()
                    && !x.CtPhieuHuySaches.Any()
                    && !x.CtPhieuThanhLies.Any())
                .OrderByDescending(x => x.Masach)
                .Take(quantityToRemove)
                .ToListAsync();

            if (removableCopies.Count < quantityToRemove)
                throw new InvalidOperationException("Không thể giảm xuống số lượng này vì không đủ bản có thể xóa an toàn. Chỉ các bản đang có thể mượn và chưa phát sinh lịch sử mới được xóa.");

            _db.Saches.RemoveRange(removableCopies);
        }

        var finalCopies = await _db.Saches
            .Where(x => x.Madausach == dauSach.Madausach)
            .ToListAsync();

        var pendingAdded = _db.ChangeTracker.Entries<Sach>()
            .Where(x => x.State == EntityState.Added && x.Entity.Madausach == dauSach.Madausach)
            .Select(x => x.Entity)
            .ToList();

        var pendingRemovedIds = _db.ChangeTracker.Entries<Sach>()
            .Where(x => x.State == EntityState.Deleted && x.Entity.Madausach == dauSach.Madausach)
            .Select(x => x.Entity.Masach)
            .ToHashSet();

        var effectiveCopies = finalCopies
            .Where(x => !pendingRemovedIds.Contains(x.Masach))
            .Concat(pendingAdded)
            .ToList();

        dauSach.Soluong = effectiveCopies.Count;
        dauSach.Soluonghienco = effectiveCopies.Count(x => x.Tinhtrang == "Có thể mượn");
        dauSach.Soluongdangmuon = effectiveCopies.Count(x => x.Tinhtrang == "Đang mượn");
        dauSach.Soluonghongmat = effectiveCopies.Count(x => x.Tinhtrang == "Hỏng" || x.Tinhtrang == "Mất");
    }

    private async Task<DanhMucSachViewModel> BuildModelAsync()
    {
        return new DanhMucSachViewModel
        {
            Madausach = await GenerateMaDauSachAsync(),
            SoLuongBan = 1,
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
                    Soluong = x.Saches.Count,
                    Soluonghienco = x.Saches.Count(s => s.Tinhtrang == "Có thể mượn"),
                    Soluongdangmuon = x.Saches.Count(s => s.Tinhtrang == "Đang mượn"),
                    Soluonghongmat = x.Saches.Count(s => s.Tinhtrang == "Hỏng" || s.Tinhtrang == "Mất")
                }).ToListAsync()
        };
    }
}
