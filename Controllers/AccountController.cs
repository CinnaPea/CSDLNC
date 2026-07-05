using Microsoft.AspNetCore.Mvc;
using CSDLNC.Models;
using CSDLNC.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CSDLNC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ThuVienDbContext _db;
        public AccountController(ThuVienDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.NguoiDungs.Include(x => x.Maquyens).Include(x => x.Manhoms).ThenInclude(x => x.Maquyens).FirstOrDefaultAsync(x =>
                    x.Tendangnhap == model.TenDangNhap &&
                    x.Matkhau == model.MatKhau);

            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Manguoidung),
            new Claim("MaNguoiDung", user.Manguoidung),
            new Claim(ClaimTypes.Name, user.Tennguoidung ?? user.Tendangnhap),
            new Claim("TenDangNhap", user.Tendangnhap)
        };

            foreach (var role in user.Manhoms)
                claims.Add(new Claim(ClaimTypes.Role, role.Tennhom));

            var directPermissions = user.Maquyens.Select(q => q.Maquyen);
            var groupPermissions = user.Manhoms.SelectMany(n => n.Maquyens).Select(q => q.Maquyen);

            foreach (var permission in directPermissions.Concat(groupPermissions).Distinct())
                claims.Add(new Claim("Permission", permission));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var exists = await _db.NguoiDungs
                .AnyAsync(x => x.Tendangnhap == model.TenDangNhap);

            if (exists)
            {
                ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            var lastNumber = await _db.NguoiDungs.Where(x => x.Manguoidung.StartsWith("ND")).Select(x => x.Manguoidung).ToListAsync();

            var nextNumber = lastNumber.Select(x => int.TryParse(x.Substring(2), out var n) ? n : 0).DefaultIfEmpty(0).Max() + 1;

            var user = new NguoiDung
            {
                Manguoidung = "ND" + nextNumber.ToString("000"),
                Tennguoidung = model.TenNguoiDung,
                Tendangnhap = model.TenDangNhap,
                Matkhau = model.MatKhau
            };

            var defaultGroup = await _db.NhomNguoiDungs.FirstOrDefaultAsync(x => x.Manhom == "N010");

            if (defaultGroup != null)
                user.Manhoms.Add(defaultGroup);

            _db.NguoiDungs.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
