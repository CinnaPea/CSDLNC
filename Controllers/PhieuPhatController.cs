using CSDLNC.Data;
using CSDLNC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CSDLNC.Controllers
{
    [Authorize(Policy = "CanManageFines")]
    public class PhieuPhatController : Controller
    {
        private readonly ThuVienDbContext _context;

        public PhieuPhatController(ThuVienDbContext context)
        {
            _context = context;
        }

        private void NapDuLieuGoiY(string? soPhieuMuon = null)
        {
            ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
            ViewBag.DanhSachSach = LayDanhSachSachGoiY(soPhieuMuon);
        }

        public IActionResult Index()
        {
            return RedirectToAction("LapHongMat");
        }

        /* =========================
           DANH SÁCH PHIẾU PHẠT HỎNG/MẤT
           Không dùng Entity Model để tránh lỗi cột không tồn tại
           ========================= */
        [HttpGet]
        public IActionResult LapHongMat()
        {
            var dsPhieu = new List<DanhSachPhieuPhatHongMatViewModel>();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        p.sophieuphathongmat,
                        p.ngaylap,
                        p.sophieumuon,
                        p.masinhvien,
                        p.tennguoidung,
                        p.tongphat,
                        COUNT(ct.masach) AS sosachphat
                    FROM phieu_phat_hong_mat p
                    LEFT JOIN ct_phieu_phat_hong_mat ct
                        ON p.sophieuphathongmat = ct.sophieuphathongmat
                    GROUP BY 
                        p.sophieuphathongmat,
                        p.ngaylap,
                        p.sophieumuon,
                        p.masinhvien,
                        p.tennguoidung,
                        p.tongphat
                    ORDER BY p.ngaylap DESC;
                ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsPhieu.Add(new DanhSachPhieuPhatHongMatViewModel
                        {
                            SoPhieuPhatHongMat = reader["sophieuphathongmat"].ToString(),
                            NgayLap = Convert.ToDateTime(reader["ngaylap"]),
                            SoPhieuMuonTra = reader["sophieumuon"].ToString(),
                            MaSinhVien = reader["masinhvien"].ToString(),
                            TenNguoiDung = reader["tennguoidung"].ToString(),
                            TongPhat = Convert.ToDecimal(reader["tongphat"]),
                            SoSachPhat = Convert.ToInt32(reader["sosachphat"])
                        });
                    }
                }
            }

            return View(dsPhieu);
        }

        /* =========================
           MỞ FORM LẬP PHIẾU MỚI
           ========================= */
        [HttpGet]
        public IActionResult TaoHongMat()
        {
            var model = new LapPhieuPhatHongMatViewModel();

            model.DanhSachSach.Add(new ChiTietLapPhieuPhatHongMatViewModel());

            ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
            ViewBag.DanhSachSach = LayDanhSachSachGoiY();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoHongMat(LapPhieuPhatHongMatViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SoPhieuMuonTra))
            {
                model.ThongBao = "Vui lòng nhập số phiếu mượn.";
                model.ThanhCong = false;

                if (model.DanhSachSach == null || model.DanhSachSach.Count == 0)
                    model.DanhSachSach = new List<ChiTietLapPhieuPhatHongMatViewModel> { new ChiTietLapPhieuPhatHongMatViewModel() };
                ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
                ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra);
                return View(model);
            }

            if (model.DanhSachSach == null || model.DanhSachSach.Count == 0)
            {
                model.ThongBao = "Vui lòng nhập ít nhất một sách bị phạt.";
                model.ThanhCong = false;
                model.DanhSachSach = new List<ChiTietLapPhieuPhatHongMatViewModel> { new ChiTietLapPhieuPhatHongMatViewModel() };
                ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
                ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra);
                return View(model);
            }

            model.DanhSachSach = model.DanhSachSach
                .Where(x => !string.IsNullOrWhiteSpace(x.MaSach))
                .ToList();

            if (model.DanhSachSach.Count == 0)
            {
                model.ThongBao = "Vui lòng nhập ít nhất một mã sách.";
                model.ThanhCong = false;
                model.DanhSachSach.Add(new ChiTietLapPhieuPhatHongMatViewModel());
                ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
                ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra);
                return View(model);
            }

            foreach (var item in model.DanhSachSach)
            {
                if (item.TinhTrang != "Hỏng" && item.TinhTrang != "Mất")
                {
                    model.ThongBao = "Tình trạng chỉ được là Hỏng hoặc Mất.";
                    model.ThanhCong = false;
                    ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
                    ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra);
                    return View(model);
                }

                if (item.PhiPhat < 0)
                {
                    model.ThongBao = "Phí phạt không được nhỏ hơn 0.";
                    model.ThanhCong = false;
                    ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
                    ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra);
                    return View(model);
                }
            }

            var dsMaSach = model.DanhSachSach
                .Select(x => x.MaSach)
                .ToList();

            if (dsMaSach.Count != dsMaSach.Distinct().Count())
            {
                model.ThongBao = "Không được nhập trùng mã sách trong cùng một phiếu.";
                model.ThanhCong = false;
                ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
                ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra);
                return View(model);
            }

            try
            {
                DataTable bangChiTiet = new DataTable();
                bangChiTiet.Columns.Add("masach", typeof(string));
                bangChiTiet.Columns.Add("tinhtrang", typeof(string));
                bangChiTiet.Columns.Add("mucdo", typeof(string));
                bangChiTiet.Columns.Add("phiphat", typeof(decimal));

                foreach (var item in model.DanhSachSach)
                {
                    bangChiTiet.Rows.Add(
                        item.MaSach,
                        item.TinhTrang,
                        item.MucDo ?? "",
                        item.PhiPhat
                    );
                }

                var danhSachSachParam = new SqlParameter("@DanhSachSach", bangChiTiet);
                danhSachSachParam.SqlDbType = SqlDbType.Structured;
                danhSachSachParam.TypeName = "dbo.DanhSachSachPhatHongMatType";

                var soPhieuMoiParam = new SqlParameter("@SoPhieuPhatHongMat", SqlDbType.VarChar, 20);
                soPhieuMoiParam.Direction = ParameterDirection.Output;

                var maNguoiDung = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var tenNguoiDung = User.Identity?.Name;

                _context.Database.ExecuteSqlRaw(
                    "EXEC dbo.sp_LapPhieuPhatHongMat " +
                    "@SoPhieuMuonTra, @DanhSachSach, @MaNguoiDung, @TenNguoiDung, @SoPhieuPhatHongMat OUTPUT",

                    new SqlParameter("@SoPhieuMuonTra", model.SoPhieuMuonTra),
                    danhSachSachParam,
                    new SqlParameter("@MaNguoiDung", string.IsNullOrEmpty(maNguoiDung) ? DBNull.Value : (object)maNguoiDung),
                    new SqlParameter("@TenNguoiDung", string.IsNullOrEmpty(tenNguoiDung) ? DBNull.Value : (object)tenNguoiDung),
                    soPhieuMoiParam
                );

                model.SoPhieuPhatMoi = soPhieuMoiParam.Value?.ToString();
                model.ThongBao = "Lập phiếu phạt hỏng/mất thành công. Số phiếu mới: " + model.SoPhieuPhatMoi;
                model.ThanhCong = true;

                model.SoPhieuMuonTra = "";
                model.DanhSachSach = new List<ChiTietLapPhieuPhatHongMatViewModel>
        {
            new ChiTietLapPhieuPhatHongMatViewModel()
        };
            }
            catch (SqlException ex)
            {
                model.ThongBao = ex.Message;
                model.ThanhCong = false;
            }
            catch (Exception ex)
            {
                model.ThongBao = "Có lỗi xảy ra: " + ex.Message;
                model.ThanhCong = false;
            }
            ViewBag.DanhSachPhieuMuon = LayDanhSachSoPhieuMuon();
            ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra);
            return View(model);
        }

        /* =========================
           XEM CHI TIẾT PHIẾU PHẠT HỎNG/MẤT
           ========================= */
        [HttpGet]
        public IActionResult ChiTietHongMat(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var model = new ChiTietPhieuPhatHongMatViewModel();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            /* Lấy thông tin phiếu chính */
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        sophieuphathongmat,
                        ngaylap,
                        sophieumuon,
                        masinhvien,
                        tennguoidung,
                        tongphat
                    FROM phieu_phat_hong_mat
                    WHERE sophieuphathongmat = @id;
                ";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.SoPhieuPhatHongMat = reader["sophieuphathongmat"].ToString();
                        model.NgayLap = Convert.ToDateTime(reader["ngaylap"]);
                        model.SoPhieuMuonTra = reader["sophieumuon"].ToString();
                        model.MaSinhVien = reader["masinhvien"].ToString();
                        model.TenNguoiDung = reader["tennguoidung"].ToString();
                        model.TongPhat = Convert.ToDecimal(reader["tongphat"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            /* Lấy chi tiết sách bị phạt */
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        ct.masach,
                        s.tensach,
                        ct.tinhtrang,
                        ct.mucdo,
                        ct.phiphat
                    FROM ct_phieu_phat_hong_mat ct
                    JOIN sach s
                        ON ct.masach = s.masach
                    WHERE ct.sophieuphathongmat = @id;
                ";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.DanhSachChiTiet.Add(new ChiTietSachPhatHongMatViewModel
                        {
                            MaSach = reader["masach"].ToString(),
                            TenSach = reader["tensach"].ToString(),
                            TinhTrang = reader["tinhtrang"].ToString(),
                            MucDo = reader["mucdo"].ToString(),
                            PhiPhat = Convert.ToDecimal(reader["phiphat"])
                        });
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult SuaHongMat(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var model = new SuaPhieuPhatHongMatViewModel();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 
                sophieuphathongmat,
                ngaylap,
                sophieumuon,
                masinhvien,
                tennguoidung,
                tongphat
            FROM phieu_phat_hong_mat
            WHERE sophieuphathongmat = @id;
        ";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.SoPhieuPhatHongMat = reader["sophieuphathongmat"].ToString();
                        model.NgayLap = Convert.ToDateTime(reader["ngaylap"]);
                        model.SoPhieuMuonTra = reader["sophieumuon"].ToString();
                        model.MaSinhVien = reader["masinhvien"].ToString();
                        model.TenNguoiDung = reader["tennguoidung"].ToString();
                        model.TongPhat = Convert.ToDecimal(reader["tongphat"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 
                masach,
                tinhtrang,
                mucdo,
                phiphat
            FROM ct_phieu_phat_hong_mat
            WHERE sophieuphathongmat = @id;
        ";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.DanhSachSach.Add(new ChiTietLapPhieuPhatHongMatViewModel
                        {
                            MaSach = reader["masach"].ToString(),
                            TinhTrang = reader["tinhtrang"].ToString(),
                            MucDo = reader["mucdo"].ToString(),
                            PhiPhat = Convert.ToDecimal(reader["phiphat"])
                        });
                    }
                }
            }
            ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra, baoGomDaTra: true);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaHongMat(SuaPhieuPhatHongMatViewModel model)
        {
            ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra, baoGomDaTra: true);

            if (string.IsNullOrWhiteSpace(model.SoPhieuPhatHongMat))
            {
                return NotFound();
            }

            if (model.DanhSachSach == null || model.DanhSachSach.Count == 0)
            {
                model.ThongBao = "Phiếu phạt phải có ít nhất một sách.";
                model.ThanhCong = false;
                model.DanhSachSach = new List<ChiTietLapPhieuPhatHongMatViewModel>
        {
            new ChiTietLapPhieuPhatHongMatViewModel()
        };
                return View(model);
            }

            model.DanhSachSach = model.DanhSachSach
                .Where(x => !string.IsNullOrWhiteSpace(x.MaSach))
                .ToList();

            if (model.DanhSachSach.Count == 0)
            {
                model.ThongBao = "Vui lòng nhập ít nhất một mã sách.";
                model.ThanhCong = false;
                model.DanhSachSach.Add(new ChiTietLapPhieuPhatHongMatViewModel());
                return View(model);
            }

            foreach (var item in model.DanhSachSach)
            {
                if (item.TinhTrang != "Hỏng" && item.TinhTrang != "Mất")
                {
                    model.ThongBao = "Tình trạng chỉ được là Hỏng hoặc Mất.";
                    model.ThanhCong = false;
                    return View(model);
                }

                if (item.PhiPhat < 0)
                {
                    model.ThongBao = "Phí phạt không được nhỏ hơn 0.";
                    model.ThanhCong = false;
                    return View(model);
                }
            }

            var dsMaSach = model.DanhSachSach
                .Select(x => x.MaSach!.Trim().ToUpper())
                .ToList();

            if (dsMaSach.Count != dsMaSach.Distinct().Count())
            {
                model.ThongBao = "Không được nhập trùng mã sách trong cùng một phiếu.";
                model.ThanhCong = false;
                return View(model);
            }

            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string soPhieuMuonTra = "";

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        SELECT sophieumuon
                        FROM phieu_phat_hong_mat
                        WHERE sophieuphathongmat = @sophieuphathongmat;
                    ";

                            var p = command.CreateParameter();
                            p.ParameterName = "@sophieuphathongmat";
                            p.Value = model.SoPhieuPhatHongMat;
                            command.Parameters.Add(p);

                            var result = command.ExecuteScalar();

                            if (result == null)
                            {
                                throw new Exception("Phiếu phạt hỏng/mất không tồn tại.");
                            }

                            soPhieuMuonTra = result.ToString()!;
                        }

                        foreach (var item in model.DanhSachSach)
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            SELECT COUNT(*)
                            FROM sach
                            WHERE masach = @masach;
                        ";

                                var p = command.CreateParameter();
                                p.ParameterName = "@masach";
                                p.Value = item.MaSach;
                                command.Parameters.Add(p);

                                int count = Convert.ToInt32(command.ExecuteScalar());

                                if (count == 0)
                                {
                                    throw new Exception("Mã sách " + item.MaSach + " không tồn tại.");
                                }
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            SELECT COUNT(*)
                            FROM ct_phieu_muon
                            WHERE sophieumuon = @sophieumuon
                              AND masach = @masach;
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@sophieumuon";
                                p1.Value = soPhieuMuonTra;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@masach";
                                p2.Value = item.MaSach;
                                command.Parameters.Add(p2);

                                int count = Convert.ToInt32(command.ExecuteScalar());

                                if (count == 0)
                                {
                                    throw new Exception("Sách " + item.MaSach + " không thuộc phiếu mượn này.");
                                }
                            }
                        }

                        var dsSachCu = new List<string>();

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        SELECT masach
                        FROM ct_phieu_phat_hong_mat
                        WHERE sophieuphathongmat = @sophieuphathongmat;
                    ";

                            var p = command.CreateParameter();
                            p.ParameterName = "@sophieuphathongmat";
                            p.Value = model.SoPhieuPhatHongMat;
                            command.Parameters.Add(p);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dsSachCu.Add(reader["masach"].ToString()!);
                                }
                            }
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        DELETE FROM ct_phieu_phat_hong_mat
                        WHERE sophieuphathongmat = @sophieuphathongmat;
                    ";

                            var p = command.CreateParameter();
                            p.ParameterName = "@sophieuphathongmat";
                            p.Value = model.SoPhieuPhatHongMat;
                            command.Parameters.Add(p);

                            command.ExecuteNonQuery();
                        }

                        decimal tongPhat = 0;

                        foreach (var item in model.DanhSachSach)
                        {
                            tongPhat += item.PhiPhat;

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            INSERT INTO ct_phieu_phat_hong_mat
                            (
                                sophieuphathongmat,
                                masach,
                                tinhtrang,
                                mucdo,
                                phiphat,
                                tongphat
                            )
                            VALUES
                            (
                                @sophieuphathongmat,
                                @masach,
                                @tinhtrang,
                                @mucdo,
                                @phiphat,
                                @phiphat
                            );
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@sophieuphathongmat";
                                p1.Value = model.SoPhieuPhatHongMat;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@masach";
                                p2.Value = item.MaSach;
                                command.Parameters.Add(p2);

                                var p3 = command.CreateParameter();
                                p3.ParameterName = "@tinhtrang";
                                p3.Value = item.TinhTrang ?? "Hỏng";
                                command.Parameters.Add(p3);

                                var p4 = command.CreateParameter();
                                p4.ParameterName = "@mucdo";
                                p4.Value = item.MucDo ?? "";
                                command.Parameters.Add(p4);

                                var p5 = command.CreateParameter();
                                p5.ParameterName = "@phiphat";
                                p5.Value = item.PhiPhat;
                                command.Parameters.Add(p5);

                                command.ExecuteNonQuery();
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            UPDATE sach
                            SET tinhtrang = @tinhtrang,
                                trangthai = @tinhtrang,
                                sophieumuonhientai = NULL,
                                ngaycapnhattrangthai = CAST(GETDATE() AS DATE)
                            WHERE masach = @masach;
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@tinhtrang";
                                p1.Value = item.TinhTrang ?? "Hỏng";
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@masach";
                                p2.Value = item.MaSach;
                                command.Parameters.Add(p2);

                                command.ExecuteNonQuery();
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            UPDATE ct_phieu_muon
                            SET ngaytra = ISNULL(ngaytra, CAST(GETDATE() AS DATE)),
                                ghichu = N'Đã cập nhật phiếu phạt hỏng/mất: ' + @sophieuphathongmat
                            WHERE sophieumuon = @sophieumuon
                              AND masach = @masach;
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@sophieuphathongmat";
                                p1.Value = model.SoPhieuPhatHongMat;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@sophieumuon";
                                p2.Value = soPhieuMuonTra;
                                command.Parameters.Add(p2);

                                var p3 = command.CreateParameter();
                                p3.ParameterName = "@masach";
                                p3.Value = item.MaSach;
                                command.Parameters.Add(p3);

                                command.ExecuteNonQuery();
                            }
                        }

                        var dsSachMoi = model.DanhSachSach
                            .Select(x => x.MaSach!.Trim().ToUpper())
                            .ToList();

                        var dsSachBiXoa = dsSachCu
                            .Where(x => !dsSachMoi.Contains(x.Trim().ToUpper()))
                            .ToList();

                        foreach (var maSachCu in dsSachBiXoa)
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            UPDATE s
                            SET tinhtrang = CASE WHEN ct.ngaytra IS NULL THEN N'Đang mượn' ELSE N'Có thể mượn' END,
                                trangthai = CASE WHEN ct.ngaytra IS NULL THEN N'Đang mượn' ELSE N'Trong kho' END,
                                sophieumuonhientai = CASE WHEN ct.ngaytra IS NULL THEN @sophieumuon ELSE NULL END,
                                ngaycapnhattrangthai = CAST(GETDATE() AS DATE)
                            FROM sach s
                            INNER JOIN ct_phieu_muon ct ON ct.masach = s.masach
                            WHERE s.masach = @masach
                              AND ct.sophieumuon = @sophieumuon;
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@masach";
                                p1.Value = maSachCu;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@sophieumuon";
                                p2.Value = soPhieuMuonTra;
                                command.Parameters.Add(p2);

                                command.ExecuteNonQuery();
                            }
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        UPDATE sv
                        SET sosachdangmuon = (
                            SELECT COUNT(*)
                            FROM ct_phieu_muon ct
                            INNER JOIN phieu_muon pm ON pm.sophieumuon = ct.sophieumuon
                            WHERE pm.masinhvien = sv.masinhvien
                              AND ct.ngaytra IS NULL
                        )
                        FROM sinh_vien sv
                        INNER JOIN phieu_muon pm ON pm.masinhvien = sv.masinhvien
                        WHERE pm.sophieumuon = @sophieumuon;

                        UPDATE phieu_muon
                        SET trangthaiphieu = CASE WHEN EXISTS (
                                SELECT 1 FROM ct_phieu_muon
                                WHERE sophieumuon = @sophieumuon AND ngaytra IS NULL
                            ) THEN N'Đang mượn' ELSE N'Đã trả' END,
                            ngayhoantat = CASE WHEN EXISTS (
                                SELECT 1 FROM ct_phieu_muon
                                WHERE sophieumuon = @sophieumuon AND ngaytra IS NULL
                            ) THEN NULL ELSE CAST(GETDATE() AS DATE) END
                        WHERE sophieumuon = @sophieumuon;

                        EXEC dbo.sp_DongBoDauSach;
                    ";

                            var p = command.CreateParameter();
                            p.ParameterName = "@sophieumuon";
                            p.Value = soPhieuMuonTra;
                            command.Parameters.Add(p);

                            command.ExecuteNonQuery();
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        UPDATE phieu_phat_hong_mat
                        SET tongphat = @tongphat
                        WHERE sophieuphathongmat = @sophieuphathongmat;
                    ";

                            var p1 = command.CreateParameter();
                            p1.ParameterName = "@tongphat";
                            p1.Value = tongPhat;
                            command.Parameters.Add(p1);

                            var p2 = command.CreateParameter();
                            p2.ParameterName = "@sophieuphathongmat";
                            p2.Value = model.SoPhieuPhatHongMat;
                            command.Parameters.Add(p2);

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        return RedirectToAction("ChiTietHongMat", new { id = model.SoPhieuPhatHongMat });
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ThongBao = "Có lỗi xảy ra: " + ex.Message;
                model.ThanhCong = false;
                return View(model);
            }
        }




        [HttpGet]
        public IActionResult LayThongTinPhieuMuon(string soPhieuMuon)
        {
            if (string.IsNullOrWhiteSpace(soPhieuMuon))
            {
                return Json(new
                {
                    thanhCong = false,
                    thongBao = "Vui lòng nhập số phiếu mượn."
                });
            }

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT ngaymuon, hantra
            FROM phieu_muon
            WHERE sophieumuon = @sophieumuon;
        ";

                var param = command.CreateParameter();
                param.ParameterName = "@sophieumuon";
                param.Value = soPhieuMuon;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        DateTime ngayMuon = Convert.ToDateTime(reader["ngaymuon"]);
                        DateTime hanTra = Convert.ToDateTime(reader["hantra"]);

                        return Json(new
                        {
                            thanhCong = true,
                            ngayMuon = ngayMuon.ToString("yyyy-MM-dd"),
                            hanTra = hanTra.ToString("yyyy-MM-dd")
                        });
                    }
                }
            }

            return Json(new
            {
                thanhCong = false,
                thongBao = "Không tìm thấy phiếu mượn."
            });
        }



        [HttpGet]
        public IActionResult LapQuaHan()
        {
            var dsPhieu = new List<DanhSachPhieuPhatQuaHanViewModel>();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 
                p.sophieuphatquahan,
                p.ngaylap,
                p.sophieumuon,
                p.masinhvien,
                p.tennguoidung,
                p.tongphat,
                COUNT(ct.masach) AS sosachphat
            FROM phieu_phat_qua_han p
            LEFT JOIN ct_phieu_phat_qua_han ct
                ON p.sophieuphatquahan = ct.sophieuphatquahan
            GROUP BY 
                p.sophieuphatquahan,
                p.ngaylap,
                p.sophieumuon,
                p.masinhvien,
                p.tennguoidung,
                p.tongphat
            ORDER BY p.ngaylap DESC;
        ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsPhieu.Add(new DanhSachPhieuPhatQuaHanViewModel
                        {
                            SoPhieuPhatQuaHan = reader["sophieuphatquahan"].ToString(),
                            NgayLap = Convert.ToDateTime(reader["ngaylap"]),
                            SoPhieuMuonTra = reader["sophieumuon"].ToString(),
                            MaSinhVien = reader["masinhvien"].ToString(),
                            TenNguoiDung = reader["tennguoidung"].ToString(),
                            TongPhat = Convert.ToDecimal(reader["tongphat"]),
                            SoSachPhat = Convert.ToInt32(reader["sosachphat"])
                        });
                    }
                }
            }

            return View(dsPhieu);
        }
        [HttpGet]
        public IActionResult TaoQuaHan()
        {
            var model = new LapPhieuPhatQuaHanViewModel();

            model.DanhSachSach.Add(new ChiTietLapPhieuPhatQuaHanViewModel());

            NapDuLieuGoiY();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoQuaHan(LapPhieuPhatQuaHanViewModel model)
        {
            NapDuLieuGoiY(model.SoPhieuMuonTra);

            if (string.IsNullOrWhiteSpace(model.SoPhieuMuonTra))
            {
                model.ThongBao = "Vui lòng nhập số phiếu mượn.";
                model.ThanhCong = false;

                if (model.DanhSachSach == null || model.DanhSachSach.Count == 0)
                {
                    model.DanhSachSach = new List<ChiTietLapPhieuPhatQuaHanViewModel>
            {
                new ChiTietLapPhieuPhatQuaHanViewModel()
            };
                }

                return View(model);
            }

            if (model.MucPhatMoiNgay < 0)
            {
                model.ThongBao = "Mức phạt mỗi ngày không được nhỏ hơn 0.";
                model.ThanhCong = false;

                if (model.DanhSachSach == null || model.DanhSachSach.Count == 0)
                {
                    model.DanhSachSach = new List<ChiTietLapPhieuPhatQuaHanViewModel>
            {
                new ChiTietLapPhieuPhatQuaHanViewModel()
            };
                }

                return View(model);
            }

            if (model.DanhSachSach == null || model.DanhSachSach.Count == 0)
            {
                model.ThongBao = "Vui lòng nhập ít nhất một sách quá hạn.";
                model.ThanhCong = false;

                model.DanhSachSach = new List<ChiTietLapPhieuPhatQuaHanViewModel>
        {
            new ChiTietLapPhieuPhatQuaHanViewModel()
        };

                return View(model);
            }

            model.DanhSachSach = model.DanhSachSach
                .Where(x => !string.IsNullOrWhiteSpace(x.MaSach))
                .ToList();

            if (model.DanhSachSach.Count == 0)
            {
                model.ThongBao = "Vui lòng nhập ít nhất một mã sách.";
                model.ThanhCong = false;

                model.DanhSachSach.Add(new ChiTietLapPhieuPhatQuaHanViewModel());

                return View(model);
            }

            var dsMaSach = model.DanhSachSach
                .Select(x => x.MaSach!.Trim().ToUpper())
                .ToList();

            if (dsMaSach.Count != dsMaSach.Distinct().Count())
            {
                model.ThongBao = "Không được nhập trùng mã sách trong cùng một phiếu.";
                model.ThanhCong = false;
                return View(model);
            }

            try
            {
                DataTable bangChiTiet = new DataTable();
                bangChiTiet.Columns.Add("masach", typeof(string));

                foreach (var item in model.DanhSachSach)
                {
                    bangChiTiet.Rows.Add(item.MaSach);
                }

                var danhSachSachParam = new SqlParameter("@DanhSachSach", bangChiTiet);
                danhSachSachParam.SqlDbType = SqlDbType.Structured;
                danhSachSachParam.TypeName = "dbo.DanhSachSachPhatQuaHanType";

                var soPhieuMoiParam = new SqlParameter("@SoPhieuPhatQuaHan", SqlDbType.VarChar, 20);
                soPhieuMoiParam.Direction = ParameterDirection.Output;

                var maNguoiDung = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var tenNguoiDung = User.Identity?.Name;

                _context.Database.ExecuteSqlRaw(
                    "EXEC dbo.sp_LapPhieuPhatQuaHan " +
                    "@SoPhieuMuonTra, @DanhSachSach, @NgayTra, @MucPhatMoiNgay, " +
                    "@MaNguoiDung, @TenNguoiDung, @SoPhieuPhatQuaHan OUTPUT",

                    new SqlParameter("@SoPhieuMuonTra", model.SoPhieuMuonTra),
                    danhSachSachParam,
                    new SqlParameter("@NgayTra", model.NgayTra),
                    new SqlParameter("@MucPhatMoiNgay", model.MucPhatMoiNgay),
                    new SqlParameter("@MaNguoiDung", string.IsNullOrEmpty(maNguoiDung) ? DBNull.Value : (object)maNguoiDung),
                    new SqlParameter("@TenNguoiDung", string.IsNullOrEmpty(tenNguoiDung) ? DBNull.Value : (object)tenNguoiDung),
                    soPhieuMoiParam
                );

                var soPhieuMoi = soPhieuMoiParam.Value?.ToString();

                return RedirectToAction("ChiTietQuaHan", new { id = soPhieuMoi });
            }
            catch (SqlException ex)
            {
                model.ThongBao = ex.Message;
                model.ThanhCong = false;
            }
            catch (Exception ex)
            {
                model.ThongBao = "Có lỗi xảy ra: " + ex.Message;
                model.ThanhCong = false;
            }

            if (model.DanhSachSach == null || model.DanhSachSach.Count == 0)
            {
                model.DanhSachSach = new List<ChiTietLapPhieuPhatQuaHanViewModel>
        {
            new ChiTietLapPhieuPhatQuaHanViewModel()
        };
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChiTietQuaHan(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var model = new ChiTietPhieuPhatQuaHanViewModel();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 
                sophieuphatquahan,
                ngaylap,
                sophieumuon,
                masinhvien,
                tennguoidung,
                tongphat
            FROM phieu_phat_qua_han
            WHERE sophieuphatquahan = @id;
        ";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.SoPhieuPhatQuaHan = reader["sophieuphatquahan"].ToString();
                        model.NgayLap = Convert.ToDateTime(reader["ngaylap"]);
                        model.SoPhieuMuonTra = reader["sophieumuon"].ToString();
                        model.MaSinhVien = reader["masinhvien"].ToString();
                        model.TenNguoiDung = reader["tennguoidung"].ToString();
                        model.TongPhat = Convert.ToDecimal(reader["tongphat"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 
                ct.masach,
                s.tensach,
                ct.ngaymuon,
                ct.hantra,
                ct.ngaytra,
                ct.songayquahan,
                ct.phiphat
            FROM ct_phieu_phat_qua_han ct
            JOIN sach s
                ON ct.masach = s.masach
            WHERE ct.sophieuphatquahan = @id;
        ";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.DanhSachChiTiet.Add(new ChiTietSachPhatQuaHanViewModel
                        {
                            MaSach = reader["masach"].ToString(),
                            TenSach = reader["tensach"].ToString(),
                            NgayMuon = reader["ngaymuon"] == DBNull.Value ? null : Convert.ToDateTime(reader["ngaymuon"]),
                            HanTra = reader["hantra"] == DBNull.Value ? null : Convert.ToDateTime(reader["hantra"]),
                            NgayTra = reader["ngaytra"] == DBNull.Value ? null : Convert.ToDateTime(reader["ngaytra"]),
                            SoNgayQuaHan = Convert.ToInt32(reader["songayquahan"]),
                            PhiPhat = Convert.ToDecimal(reader["phiphat"])
                        });
                    }
                }
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult SuaQuaHan(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var model = new SuaPhieuPhatQuaHanViewModel();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 
                p.sophieuphatquahan,
                p.ngaylap,
                p.sophieumuon,
                p.masinhvien,
                p.tennguoidung,
                p.tongphat,
                pm.ngaymuon,
                pm.hantra,
                MAX(ct.ngaytra) AS ngaytra,
                CASE 
                    WHEN MAX(ct.songayquahan) > 0
                    THEN MAX(ct.phiphat) / MAX(ct.songayquahan)
                    ELSE 2000
                END AS mucphatmoingay
            FROM phieu_phat_qua_han p
            JOIN phieu_muon pm
                ON p.sophieumuon = pm.sophieumuon
            JOIN ct_phieu_phat_qua_han ct
                ON p.sophieuphatquahan = ct.sophieuphatquahan
            WHERE p.sophieuphatquahan = @id
            GROUP BY
                p.sophieuphatquahan,
                p.ngaylap,
                p.sophieumuon,
                p.masinhvien,
                p.tennguoidung,
                p.tongphat,
                pm.ngaymuon,
                pm.hantra;
        ";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.SoPhieuPhatQuaHan = reader["sophieuphatquahan"].ToString();
                        model.NgayLap = Convert.ToDateTime(reader["ngaylap"]);
                        model.SoPhieuMuonTra = reader["sophieumuon"].ToString();
                        model.MaSinhVien = reader["masinhvien"].ToString();
                        model.TenNguoiDung = reader["tennguoidung"].ToString();
                        model.TongPhat = Convert.ToDecimal(reader["tongphat"]);
                        model.NgayMuon = Convert.ToDateTime(reader["ngaymuon"]);
                        model.HanTra = Convert.ToDateTime(reader["hantra"]);
                        model.NgayTra = Convert.ToDateTime(reader["ngaytra"]);
                        model.MucPhatMoiNgay = Convert.ToDecimal(reader["mucphatmoingay"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 
                ct.masach,
                s.tensach,
                ct.ngaymuon,
                ct.hantra,
                ct.ngaytra,
                ct.songayquahan,
                ct.phiphat
            FROM ct_phieu_phat_qua_han ct
            JOIN sach s
                ON ct.masach = s.masach
            WHERE ct.sophieuphatquahan = @id;
        ";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.DanhSachChiTiet.Add(new ChiTietSachPhatQuaHanViewModel
                        {
                            MaSach = reader["masach"].ToString(),
                            TenSach = reader["tensach"].ToString(),
                            NgayMuon = reader["ngaymuon"] == DBNull.Value ? null : Convert.ToDateTime(reader["ngaymuon"]),
                            HanTra = reader["hantra"] == DBNull.Value ? null : Convert.ToDateTime(reader["hantra"]),
                            NgayTra = reader["ngaytra"] == DBNull.Value ? null : Convert.ToDateTime(reader["ngaytra"]),
                            SoNgayQuaHan = Convert.ToInt32(reader["songayquahan"]),
                            PhiPhat = Convert.ToDecimal(reader["phiphat"])
                        });
                    }
                }
            }
            ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra, baoGomDaTra: true);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaQuaHan(SuaPhieuPhatQuaHanViewModel model)
        {
            ViewBag.DanhSachSach = LayDanhSachSachGoiY(model.SoPhieuMuonTra, baoGomDaTra: true);

            if (string.IsNullOrWhiteSpace(model.SoPhieuPhatQuaHan))
            {
                return NotFound();
            }

            if (model.NgayTra == default)
            {
                model.ThongBao = "Vui lòng nhập ngày trả.";
                return View(model);
            }

            if (model.MucPhatMoiNgay < 0)
            {
                model.ThongBao = "Mức phạt mỗi ngày không được nhỏ hơn 0.";
                return View(model);
            }

            if (model.DanhSachChiTiet == null || model.DanhSachChiTiet.Count == 0)
            {
                model.ThongBao = "Phiếu phạt quá hạn phải có ít nhất một sách.";
                model.DanhSachChiTiet = new List<ChiTietSachPhatQuaHanViewModel>
        {
            new ChiTietSachPhatQuaHanViewModel()
        };
                return View(model);
            }

            model.DanhSachChiTiet = model.DanhSachChiTiet
                .Where(x => !string.IsNullOrWhiteSpace(x.MaSach))
                .ToList();

            if (model.DanhSachChiTiet.Count == 0)
            {
                model.ThongBao = "Vui lòng nhập ít nhất một mã sách.";
                model.DanhSachChiTiet.Add(new ChiTietSachPhatQuaHanViewModel());
                return View(model);
            }

            var dsMaSach = model.DanhSachChiTiet
                .Select(x => x.MaSach!.Trim().ToUpper())
                .ToList();

            if (dsMaSach.Count != dsMaSach.Distinct().Count())
            {
                model.ThongBao = "Không được nhập trùng mã sách trong cùng một phiếu.";
                return View(model);
            }

            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string soPhieuMuonTra = "";
                        DateTime ngayMuon;
                        DateTime hanTra;

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        SELECT 
                            p.sophieumuon,
                            pm.ngaymuon,
                            pm.hantra
                        FROM phieu_phat_qua_han p
                        JOIN phieu_muon pm
                            ON p.sophieumuon = pm.sophieumuon
                        WHERE p.sophieuphatquahan = @sophieuphatquahan;
                    ";

                            var p = command.CreateParameter();
                            p.ParameterName = "@sophieuphatquahan";
                            p.Value = model.SoPhieuPhatQuaHan;
                            command.Parameters.Add(p);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    soPhieuMuonTra = reader["sophieumuon"].ToString()!;
                                    ngayMuon = Convert.ToDateTime(reader["ngaymuon"]);
                                    hanTra = Convert.ToDateTime(reader["hantra"]);
                                }
                                else
                                {
                                    throw new Exception("Phiếu phạt quá hạn không tồn tại.");
                                }
                            }
                        }

                        int soNgayQuaHan = (model.NgayTra.Date - hanTra.Date).Days;

                        if (soNgayQuaHan <= 0)
                        {
                            throw new Exception("Sách chưa quá hạn, không thể lập/sửa phiếu phạt quá hạn.");
                        }

                        foreach (var item in model.DanhSachChiTiet)
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            SELECT COUNT(*)
                            FROM sach
                            WHERE masach = @masach;
                        ";

                                var p = command.CreateParameter();
                                p.ParameterName = "@masach";
                                p.Value = item.MaSach;
                                command.Parameters.Add(p);

                                int count = Convert.ToInt32(command.ExecuteScalar());

                                if (count == 0)
                                {
                                    throw new Exception("Mã sách " + item.MaSach + " không tồn tại.");
                                }
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            SELECT COUNT(*)
                            FROM ct_phieu_muon
                            WHERE sophieumuon = @sophieumuon
                              AND masach = @masach;
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@sophieumuon";
                                p1.Value = soPhieuMuonTra;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@masach";
                                p2.Value = item.MaSach;
                                command.Parameters.Add(p2);

                                int count = Convert.ToInt32(command.ExecuteScalar());

                                if (count == 0)
                                {
                                    throw new Exception("Sách " + item.MaSach + " không thuộc phiếu mượn này.");
                                }
                            }
                        }

                        var dsSachCu = new List<string>();

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        SELECT masach
                        FROM ct_phieu_phat_qua_han
                        WHERE sophieuphatquahan = @sophieuphatquahan;
                    ";

                            var p = command.CreateParameter();
                            p.ParameterName = "@sophieuphatquahan";
                            p.Value = model.SoPhieuPhatQuaHan;
                            command.Parameters.Add(p);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dsSachCu.Add(reader["masach"].ToString()!);
                                }
                            }
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        DELETE FROM ct_phieu_phat_qua_han
                        WHERE sophieuphatquahan = @sophieuphatquahan;
                    ";

                            var p = command.CreateParameter();
                            p.ParameterName = "@sophieuphatquahan";
                            p.Value = model.SoPhieuPhatQuaHan;
                            command.Parameters.Add(p);

                            command.ExecuteNonQuery();
                        }

                        decimal phiPhatMoiSach = soNgayQuaHan * model.MucPhatMoiNgay;
                        decimal tongPhat = 0;

                        foreach (var item in model.DanhSachChiTiet)
                        {
                            tongPhat += phiPhatMoiSach;

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            INSERT INTO ct_phieu_phat_qua_han
                            (
                                sophieuphatquahan,
                                masach,
                                ngaymuon,
                                hantra,
                                ngaytra,
                                songayquahan,
                                phiphat,
                                tongphat
                            )
                            VALUES
                            (
                                @sophieuphatquahan,
                                @masach,
                                @ngaymuon,
                                @hantra,
                                @ngaytra,
                                @songayquahan,
                                @phiphat,
                                @phiphat
                            );
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@sophieuphatquahan";
                                p1.Value = model.SoPhieuPhatQuaHan;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@masach";
                                p2.Value = item.MaSach;
                                command.Parameters.Add(p2);

                                var p3 = command.CreateParameter();
                                p3.ParameterName = "@ngaymuon";
                                p3.Value = ngayMuon;
                                command.Parameters.Add(p3);

                                var p4 = command.CreateParameter();
                                p4.ParameterName = "@hantra";
                                p4.Value = hanTra;
                                command.Parameters.Add(p4);

                                var p5 = command.CreateParameter();
                                p5.ParameterName = "@ngaytra";
                                p5.Value = model.NgayTra;
                                command.Parameters.Add(p5);

                                var p6 = command.CreateParameter();
                                p6.ParameterName = "@songayquahan";
                                p6.Value = soNgayQuaHan;
                                command.Parameters.Add(p6);

                                var p7 = command.CreateParameter();
                                p7.ParameterName = "@phiphat";
                                p7.Value = phiPhatMoiSach;
                                command.Parameters.Add(p7);

                                command.ExecuteNonQuery();
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            UPDATE ct_phieu_muon
                            SET 
                                ngaytra = @ngaytra,
                                ghichu = N'Đã cập nhật phiếu phạt quá hạn: ' + @sophieuphatquahan
                            WHERE sophieumuon = @sophieumuon
                              AND masach = @masach;
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@ngaytra";
                                p1.Value = model.NgayTra;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@sophieuphatquahan";
                                p2.Value = model.SoPhieuPhatQuaHan;
                                command.Parameters.Add(p2);

                                var p3 = command.CreateParameter();
                                p3.ParameterName = "@sophieumuon";
                                p3.Value = soPhieuMuonTra;
                                command.Parameters.Add(p3);

                                var p4 = command.CreateParameter();
                                p4.ParameterName = "@masach";
                                p4.Value = item.MaSach;
                                command.Parameters.Add(p4);

                                command.ExecuteNonQuery();
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            UPDATE sach
                            SET tinhtrang = N'Có thể mượn',
                                trangthai = N'Trong kho',
                                sophieumuonhientai = NULL,
                                ngaycapnhattrangthai = CAST(GETDATE() AS DATE)
                            WHERE masach = @masach;
                        ";

                                var p = command.CreateParameter();
                                p.ParameterName = "@masach";
                                p.Value = item.MaSach;
                                command.Parameters.Add(p);

                                command.ExecuteNonQuery();
                            }
                        }

                        var dsSachMoi = model.DanhSachChiTiet
                            .Select(x => x.MaSach!.Trim().ToUpper())
                            .ToList();

                        var dsSachBiXoa = dsSachCu
                            .Where(x => !dsSachMoi.Contains(x.Trim().ToUpper()))
                            .ToList();

                        foreach (var maSachCu in dsSachBiXoa)
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            UPDATE sach
                            SET tinhtrang = N'Đang mượn',
                                trangthai = N'Đang mượn',
                                sophieumuonhientai = @sophieumuon,
                                ngaycapnhattrangthai = CAST(GETDATE() AS DATE)
                            WHERE masach = @masach;
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@masach";
                                p1.Value = maSachCu;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@sophieumuon";
                                p2.Value = soPhieuMuonTra;
                                command.Parameters.Add(p2);

                                command.ExecuteNonQuery();
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = @"
                            UPDATE ct_phieu_muon
                            SET 
                                ngaytra = NULL,
                                ghichu = N'Đã xoá khỏi phiếu phạt quá hạn: ' + @sophieuphatquahan
                            WHERE sophieumuon = @sophieumuon
                              AND masach = @masach;
                        ";

                                var p1 = command.CreateParameter();
                                p1.ParameterName = "@sophieuphatquahan";
                                p1.Value = model.SoPhieuPhatQuaHan;
                                command.Parameters.Add(p1);

                                var p2 = command.CreateParameter();
                                p2.ParameterName = "@sophieumuon";
                                p2.Value = soPhieuMuonTra;
                                command.Parameters.Add(p2);

                                var p3 = command.CreateParameter();
                                p3.ParameterName = "@masach";
                                p3.Value = maSachCu;
                                command.Parameters.Add(p3);

                                command.ExecuteNonQuery();
                            }
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        UPDATE sv
                        SET sosachdangmuon = (
                            SELECT COUNT(*)
                            FROM ct_phieu_muon ct
                            INNER JOIN phieu_muon pm ON pm.sophieumuon = ct.sophieumuon
                            WHERE pm.masinhvien = sv.masinhvien
                              AND ct.ngaytra IS NULL
                        )
                        FROM sinh_vien sv
                        INNER JOIN phieu_muon pm ON pm.masinhvien = sv.masinhvien
                        WHERE pm.sophieumuon = @sophieumuon;

                        UPDATE phieu_muon
                        SET trangthaiphieu = CASE WHEN EXISTS (
                                SELECT 1 FROM ct_phieu_muon
                                WHERE sophieumuon = @sophieumuon AND ngaytra IS NULL
                            ) THEN N'Đang mượn' ELSE N'Đã trả' END,
                            ngayhoantat = CASE WHEN EXISTS (
                                SELECT 1 FROM ct_phieu_muon
                                WHERE sophieumuon = @sophieumuon AND ngaytra IS NULL
                            ) THEN NULL ELSE @ngaytra END
                        WHERE sophieumuon = @sophieumuon;

                        EXEC dbo.sp_DongBoDauSach;
                    ";

                            var p1 = command.CreateParameter();
                            p1.ParameterName = "@sophieumuon";
                            p1.Value = soPhieuMuonTra;
                            command.Parameters.Add(p1);

                            var p2 = command.CreateParameter();
                            p2.ParameterName = "@ngaytra";
                            p2.Value = model.NgayTra;
                            command.Parameters.Add(p2);

                            command.ExecuteNonQuery();
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
                        UPDATE phieu_phat_qua_han
                        SET tongphat = @tongphat
                        WHERE sophieuphatquahan = @sophieuphatquahan;
                    ";

                            var p1 = command.CreateParameter();
                            p1.ParameterName = "@tongphat";
                            p1.Value = tongPhat;
                            command.Parameters.Add(p1);

                            var p2 = command.CreateParameter();
                            p2.ParameterName = "@sophieuphatquahan";
                            p2.Value = model.SoPhieuPhatQuaHan;
                            command.Parameters.Add(p2);

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        return RedirectToAction("ChiTietQuaHan", new { id = model.SoPhieuPhatQuaHan });
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ThongBao = "Có lỗi xảy ra: " + ex.Message;
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult ThongKeHongMat()
        {
            return View(new ThongKeSachHongMatViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThongKeHongMat(ThongKeSachHongMatViewModel model)
        {
            if (model.Thang < 1 || model.Thang > 12)
            {
                model.ThongBao = "Tháng không hợp lệ.";
                return View(model);
            }

            if (model.Nam < 2000)
            {
                model.ThongBao = "Năm không hợp lệ.";
                return View(model);
            }

            if (model.TinhTrang != "Tất cả" &&
                model.TinhTrang != "Hỏng" &&
                model.TinhTrang != "Mất")
            {
                model.ThongBao = "Tình trạng chỉ được là Tất cả, Hỏng hoặc Mất.";
                return View(model);
            }

            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.sp_ThongKeSachHongMatTrongThang";
                    command.CommandType = CommandType.StoredProcedure;

                    var pThang = command.CreateParameter();
                    pThang.ParameterName = "@Thang";
                    pThang.Value = model.Thang;
                    command.Parameters.Add(pThang);

                    var pNam = command.CreateParameter();
                    pNam.ParameterName = "@Nam";
                    pNam.Value = model.Nam;
                    command.Parameters.Add(pNam);

                    var pTinhTrang = command.CreateParameter();
                    pTinhTrang.ParameterName = "@TinhTrang";
                    pTinhTrang.Value = model.TinhTrang ?? "Tất cả";
                    command.Parameters.Add(pTinhTrang);

                    using (var reader = command.ExecuteReader())
                    {
                        model.DanhSach = new List<ThongKeSachHongMatChiTietViewModel>();

                        while (reader.Read())
                        {
                            model.DanhSach.Add(new ThongKeSachHongMatChiTietViewModel
                            {
                                MaSach = reader["MaSach"].ToString(),
                                TenSach = reader["TenSach"].ToString(),
                                TheLoai = reader["TheLoai"].ToString(),
                                TacGia = reader["TacGia"].ToString(),
                                NXB = reader["NXB"].ToString(),
                                TinhTrang = reader["TinhTrang"].ToString()
                            });
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                model.TongSoSachHongMat =
                                    reader["TongSoSachHongMat"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TongSoSachHongMat"]);

                                model.SoSachHong =
                                    reader["SoSachHong"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SoSachHong"]);

                                model.SoSachMat =
                                    reader["SoSachMat"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SoSachMat"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ThongBao = "Có lỗi xảy ra: " + ex.Message;
            }

            return View(model);
        }
        private List<string> LayDanhSachSoPhieuMuon()
        {
            var dsPhieuMuon = new List<string>();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT sophieumuon
            FROM phieu_muon
            WHERE EXISTS (
                SELECT 1
                FROM ct_phieu_muon ct
                WHERE ct.sophieumuon = phieu_muon.sophieumuon
                  AND ct.ngaytra IS NULL
            )
            ORDER BY sophieumuon;
        ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsPhieuMuon.Add(reader["sophieumuon"].ToString()!);
                    }
                }
            }

            return dsPhieuMuon;
        }
        private List<SachGoiYViewModel> LayDanhSachSachGoiY(string? soPhieuMuon = null, bool baoGomDaTra = false)
        {
            var dsSach = new List<SachGoiYViewModel>();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT DISTINCT s.masach, s.tensach, ct.sophieumuon
            FROM sach s
            INNER JOIN ct_phieu_muon ct
                ON ct.masach = s.masach
            WHERE (@sophieumuon IS NULL AND ct.ngaytra IS NULL)
               OR (
                    @sophieumuon IS NOT NULL
                    AND ct.sophieumuon = @sophieumuon
                    AND (@baogomdatra = 1 OR ct.ngaytra IS NULL)
               )
            ORDER BY ct.sophieumuon, s.masach;
        ";

                var p = command.CreateParameter();
                p.ParameterName = "@sophieumuon";
                p.Value = string.IsNullOrWhiteSpace(soPhieuMuon) ? DBNull.Value : soPhieuMuon;
                command.Parameters.Add(p);

                var includeReturnedParam = command.CreateParameter();
                includeReturnedParam.ParameterName = "@baogomdatra";
                includeReturnedParam.Value = baoGomDaTra ? 1 : 0;
                command.Parameters.Add(includeReturnedParam);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsSach.Add(new SachGoiYViewModel
                        {
                            MaSach = reader["masach"].ToString(),
                            TenSach = reader["tensach"].ToString(),
                            SoPhieuMuon = reader["sophieumuon"].ToString()
                        });
                    }
                }
            }

            return dsSach;
        }
    }

}
