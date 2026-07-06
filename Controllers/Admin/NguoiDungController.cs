using CSDLNC.Data;
using CSDLNC.Models;
using CSDLNC.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CSDLNC.Controllers.Admin;

public class NguoiDungController : Controller
{
    private readonly ThuVienDbContext _db;

    public NguoiDungController(ThuVienDbContext db)
    {
        _db = db;
    }

    // =========================
    // LIST
    // =========================
    public async Task<IActionResult> Index(string? keyword, string? group)
    {
        var query = _db.NguoiDungs
            .Include(x => x.Manhoms)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x =>
                x.Tendangnhap.Contains(keyword) ||
                x.Tennguoidung.Contains(keyword));
        }

        if (!string.IsNullOrEmpty(group))
        {
            query = query.Where(x =>
                x.Manhoms.Any(g => g.Manhom == group));
        }

        var users = await query.ToListAsync();

        return View(users);
    }

    // =========================
    // CREATE GET
    // =========================
    public async Task<IActionResult> Create()
    {
        var vm = new UserCreateEditViewModel
        {
            AvailableGroups = await _db.NhomNguoiDungs
                .Select(g => new SelectListItem
                {
                    Value = g.Manhom,
                    Text = g.Tennhom
                }).ToListAsync()
        };

        return View(vm);
    }

    // =========================
    // CREATE POST
    // =========================
    [HttpPost]
    public async Task<IActionResult> Create(UserCreateEditViewModel vm)
    {
        var user = new NguoiDung
        {
            Manguoidung = Guid.NewGuid().ToString("N")[..10],
            Tendangnhap = vm.Tendangnhap,
            Tennguoidung = vm.Tennguoidung,
            Matkhau = vm.Matkhau // (later: hash this)
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
            Tennguoidung = user.Tennguoidung,

            SelectedGroups = user.Manhoms.Select(x => x.Manhom).ToList(),

            AvailableGroups = await _db.NhomNguoiDungs
                .Select(g => new SelectListItem
                {
                    Value = g.Manhom,
                    Text = g.Tennhom
                }).ToListAsync()
        };

        return View(vm);
    }

    // =========================
    // EDIT POST
    // =========================
    [HttpPost]
    public async Task<IActionResult> Edit(UserCreateEditViewModel vm)
    {
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
}