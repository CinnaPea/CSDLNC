using CSDLNC.Data;
using CSDLNC.Models;
using CSDLNC.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CSDLNC.Controllers.Admin;

[Authorize(Policy = "CanManageUsers")]
public class NguoiDungController : Controller
{
    private readonly ThuVienDbContext _db;

    public NguoiDungController(ThuVienDbContext db)
    {
        _db = db;
    }

    private async Task<List<string>> GetEffectivePermissions(string userId)
    {
        var user = await _db.NguoiDungs
            .Include(x => x.Manhoms)
                .ThenInclude(g => g.Maquyens)
            .FirstOrDefaultAsync(x => x.Manguoidung == userId);

        if (user == null) return new();

        return user.Manhoms
            .SelectMany(g => g.Maquyens)
            .Select(p => p.Tenquyen)
            .Distinct()
            .ToList();
    }

    // =========================
    // LIST
    // =========================
    public async Task<IActionResult> Index(string? keyword, string? group, int page = 1)
    {
        const int pageSize = 8;
        page = Math.Max(page, 1);

        var query = _db.NguoiDungs
            .Include(x => x.Manhoms)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x =>
                x.Tendangnhap.Contains(keyword) ||
                (x.Tennguoidung != null && x.Tennguoidung.Contains(keyword)));
        }

        if (!string.IsNullOrEmpty(group))
        {
            query = query.Where(x =>
                x.Manhoms.Any(g => g.Manhom == group));
        }

        var totalUsers = await query.CountAsync();
        var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);
        page = Math.Min(page, totalPages);

        var users = await query
            .OrderBy(x => x.Manguoidung)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var vm = new NguoiDungIndexViewModel
        {
            Users = users,
            Keyword = keyword,
            Group = group,
            PageNumber = page,
            PageSize = pageSize,
            TotalUsers = totalUsers,
            AvailableGroups = await GetAvailableGroups()
        };

        return View(vm);
    }

    // =========================
    // CREATE GET
    // =========================
    public async Task<IActionResult> Create()
    {
        var vm = new UserCreateEditViewModel
        {
            AvailableGroups = await GetAvailableGroups()
        };

        return View(vm);
    }

    // =========================
    // CREATE POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableGroups = await GetAvailableGroups();
            return View(vm);
        }

        var user = new NguoiDung
        {
            Manguoidung = Guid.NewGuid().ToString("N")[..10],
            Tendangnhap = vm.Tendangnhap,
            Tennguoidung = vm.Tennguoidung,
            Matkhau = vm.Matkhau ?? string.Empty // (later: hash this)
        };

        _db.NguoiDungs.Add(user);

        // GROUP assignment (ONLY SOURCE OF PERMISSION)
        var groups = await _db.NhomNguoiDungs
            .Where(g => vm.SelectedGroups.Contains(g.Manhom))
            .ToListAsync();

        user.Manhoms = groups;

        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // =========================
    // EDIT GET
    // =========================
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _db.NguoiDungs
            .Include(x => x.Manhoms)
            .FirstOrDefaultAsync(x => x.Manguoidung == id);

        if (user == null) return NotFound();

        var vm = new UserCreateEditViewModel
        {
            Manguoidung = user.Manguoidung,
            Tendangnhap = user.Tendangnhap,
            Tennguoidung = user.Tennguoidung ?? string.Empty,

            SelectedGroups = user.Manhoms.Select(x => x.Manhom).ToList(),

            AvailableGroups = await GetAvailableGroups(),
            EffectivePermissions = await GetEffectivePermissions(user.Manguoidung),
            GroupNames = user.Manhoms.Select(x => x.Tennhom).ToList()
        };

        return View(vm);
    }

    // =========================
    // EDIT POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserCreateEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableGroups = await GetAvailableGroups();
            return View(vm);
        }

        var user = await _db.NguoiDungs
            .Include(x => x.Manhoms)
            .FirstOrDefaultAsync(x => x.Manguoidung == vm.Manguoidung);

        if (user == null) return NotFound();

        user.Tendangnhap = vm.Tendangnhap;
        user.Tennguoidung = vm.Tennguoidung;

        // reset groups
        user.Manhoms.Clear();

        var groups = await _db.NhomNguoiDungs
            .Where(g => vm.SelectedGroups.Contains(g.Manhom))
            .ToListAsync();

        user.Manhoms = groups;

        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == currentUserId)
        {
            TempData["ErrorMessage"] = "Không thể xóa tài khoản đang đăng nhập.";
            return RedirectToAction("Index");
        }

        var user = await _db.NguoiDungs
            .Include(x => x.Manhoms)
            .Include(x => x.Maquyens)
            .FirstOrDefaultAsync(x => x.Manguoidung == id);

        if (user == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy người dùng cần xóa.";
            return RedirectToAction("Index");
        }

        user.Manhoms.Clear();
        user.Maquyens.Clear();
        _db.NguoiDungs.Remove(user);

        try
        {
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa người dùng.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Không thể xóa người dùng vì đã phát sinh dữ liệu nghiệp vụ.";
        }

        return RedirectToAction("Index");
    }

    private Task<List<SelectListItem>> GetAvailableGroups()
    {
        return _db.NhomNguoiDungs
            .OrderBy(g => g.Manhom)
            .Select(g => new SelectListItem
            {
                Value = g.Manhom,
                Text = g.Tennhom
            })
            .ToListAsync();
    }
}
