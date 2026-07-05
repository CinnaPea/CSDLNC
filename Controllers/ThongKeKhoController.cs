using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CSDLNC.Models;
using CSDLNC.Data;

namespace CSDLNC.Controllers
{
    [Authorize]
    public class ThongKeKhoController : Controller
    {
        private readonly ThuVienDbContext _db;
        public ThongKeKhoController(ThuVienDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> SoLuongSach(string? keyword, string? theLoai)
        {
            //if (!User.HasClaim("Permission", "Q009") && !User.HasClaim("Permission", "Q001"))
            //    return Forbid();
            if(!User.HasClaim("Permission", "Q009") && !User.HasClaim("Permission", "Q001") && !User.HasClaim("Permission", "Q006"))
            {
                return Forbid();
            }

            var query = _db.DauSaches
                .Include(x => x.Saches)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.Tendausach!.Contains(keyword) ||
                    x.Tacgia!.Contains(keyword) ||
                    x.Madausach.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(theLoai))
            {
                query = query.Where(x => x.Theloai == theLoai);
            }

            var rows = await query.OrderBy(x => x.Theloai).ThenBy(x => x.Tendausach).Select(x => new ThongKeSoLuongSachRow
                {
                    MaDauSach = x.Madausach,
                    TenDauSach = x.Tendausach,
                    TheLoai = x.Theloai,
                    TacGia = x.Tacgia,
                    TongSoLuong = x.Saches.Count,
                    SoLuongHienCo = x.Saches.Count(s => s.Tinhtrang == "Có thể mượn"),
                    SoLuongDangMuon = x.Saches.Count(s => s.Tinhtrang == "Đang mượn"),
                    SoLuongHongMat = x.Saches.Count(s => s.Tinhtrang == "Hỏng" || s.Tinhtrang == "Mất")
                }).ToListAsync();

            var danhSachTheLoai = await _db.DauSaches.Where(x => x.Theloai != null && x.Theloai != "").Select(x => x.Theloai!).Distinct().OrderBy(x => x).ToListAsync();

            var model = new ThongKeSoLuongSachViewModel
            {
                TuKhoa = keyword,
                TheLoai = theLoai,
                DanhSachTheLoai = danhSachTheLoai,
                Rows = rows,
                TongDauSach = rows.Count,
                TongBan = rows.Sum(x => x.TongSoLuong),
                TongHienCo = rows.Sum(x => x.SoLuongHienCo),
                TongDangMuon = rows.Sum(x => x.SoLuongDangMuon),
                TongHongMat = rows.Sum(x => x.SoLuongHongMat),
                TheLoaiRows = rows.GroupBy(x => x.TheLoai ?? "Chưa phân loại").Select(g => new ThongKeTheLoaiRow
                    {
                        TheLoai = g.Key,
                        TongDauSach = g.Count(),
                        TongBan = g.Sum(x => x.TongSoLuong)
                    }).OrderByDescending(x => x.TongBan).ToList()
            };

            return View(model);
        }
    }
}
