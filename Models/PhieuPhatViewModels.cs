using System;
using System.Collections.Generic;

namespace CSDLNC.Models
{
    public class LapPhieuPhatHongMatViewModel
    {
        public string? SoPhieuMuonTra { get; set; }

        public List<ChiTietLapPhieuPhatHongMatViewModel> DanhSachSach { get; set; } = new();

        public string? SoPhieuPhatMoi { get; set; }

        public string? ThongBao { get; set; }

        public bool ThanhCong { get; set; }
    }
    public class ChiTietLapPhieuPhatHongMatViewModel
    {
        public string? MaSach { get; set; }

        public string? TinhTrang { get; set; } = "Hỏng";

        public string? MucDo { get; set; }

        public decimal PhiPhat { get; set; }
    }

    public class LapPhieuPhatQuaHanViewModel
    {
        public string? SoPhieuMuonTra { get; set; }
        public DateTime? NgayMuon { get; set; }

        public DateTime? HanTra { get; set; }

        public List<ChiTietLapPhieuPhatQuaHanViewModel> DanhSachSach { get; set; } = new();

        public DateTime NgayTra { get; set; } = DateTime.Today;

        public decimal MucPhatMoiNgay { get; set; } = 2000;

        public string? SoPhieuPhatMoi { get; set; }

        public string? ThongBao { get; set; }

        public bool ThanhCong { get; set; }
    }

    public class ChiTietLapPhieuPhatQuaHanViewModel
    {
        public string? MaSach { get; set; }
    }

   
    public class ThongKeSachHongMatTongHop
    {
        public int TongSoSachHongMat { get; set; }
        public int SoSachHong { get; set; }
        public int SoSachMat { get; set; }
    }

    public class DanhSachPhieuPhatHongMatViewModel
    {
        public string? SoPhieuPhatHongMat { get; set; }
        public DateTime NgayLap { get; set; }
        public string? SoPhieuMuonTra { get; set; }
        public string? MaSinhVien { get; set; }
        public string? TenNguoiDung { get; set; }
        public decimal TongPhat { get; set; }
        public int SoSachPhat { get; set; }
    }

    public class ChiTietPhieuPhatHongMatViewModel
    {
        public string? SoPhieuPhatHongMat { get; set; }
        public DateTime NgayLap { get; set; }
        public string? SoPhieuMuonTra { get; set; }
        public string? MaSinhVien { get; set; }
        public string? TenNguoiDung { get; set; }
        public decimal TongPhat { get; set; }

        public List<ChiTietSachPhatHongMatViewModel> DanhSachChiTiet { get; set; } = new();
    }

    public class ChiTietSachPhatHongMatViewModel
    {
        public string? MaSach { get; set; }
        public string? TenSach { get; set; }
        public string? TinhTrang { get; set; }
        public string? MucDo { get; set; }
        public decimal PhiPhat { get; set; }
    }

    public class SuaPhieuPhatHongMatViewModel
    {
        public string? SoPhieuPhatHongMat { get; set; }

        public DateTime NgayLap { get; set; }

        public string? SoPhieuMuonTra { get; set; }

        public string? MaSinhVien { get; set; }

        public string? TenNguoiDung { get; set; }

        public decimal TongPhat { get; set; }

        public List<ChiTietLapPhieuPhatHongMatViewModel> DanhSachSach { get; set; } = new();

        public string? ThongBao { get; set; }

        public bool ThanhCong { get; set; }
    }


    public class DanhSachPhieuPhatQuaHanViewModel
    {
        public string? SoPhieuPhatQuaHan { get; set; }
        public DateTime NgayLap { get; set; }
        public string? SoPhieuMuonTra { get; set; }
        public string? MaSinhVien { get; set; }
        public string? TenNguoiDung { get; set; }
        public decimal TongPhat { get; set; }
        public int SoSachPhat { get; set; }
    }

    public class ChiTietPhieuPhatQuaHanViewModel
    {
        public string? SoPhieuPhatQuaHan { get; set; }
        public DateTime NgayLap { get; set; }
        public string? SoPhieuMuonTra { get; set; }
        public string? MaSinhVien { get; set; }
        public string? TenNguoiDung { get; set; }
        public decimal TongPhat { get; set; }

        public List<ChiTietSachPhatQuaHanViewModel> DanhSachChiTiet { get; set; } = new();
    }

    public class ChiTietSachPhatQuaHanViewModel
    {
        public string? MaSach { get; set; }
        public string? TenSach { get; set; }
        public DateTime? NgayMuon { get; set; }
        public DateTime? HanTra { get; set; }
        public DateTime? NgayTra { get; set; }
        public int SoNgayQuaHan { get; set; }
        public decimal PhiPhat { get; set; }
    }

    public class SuaPhieuPhatQuaHanViewModel
    {
        public string? SoPhieuPhatQuaHan { get; set; }

        public DateTime NgayLap { get; set; }

        public string? SoPhieuMuonTra { get; set; }

        public string? MaSinhVien { get; set; }

        public string? TenNguoiDung { get; set; }

        public DateTime? NgayMuon { get; set; }

        public DateTime? HanTra { get; set; }

        public DateTime NgayTra { get; set; } = DateTime.Today;

        public decimal MucPhatMoiNgay { get; set; } = 2000;

        public decimal TongPhat { get; set; }

        public List<ChiTietSachPhatQuaHanViewModel> DanhSachChiTiet { get; set; } = new();

        public string? ThongBao { get; set; }

        public bool ThanhCong { get; set; }
    }


    public class ThongKeSachHongMatViewModel
    {
        public int Thang { get; set; } = DateTime.Today.Month;

        public int Nam { get; set; } = DateTime.Today.Year;

        public string? TinhTrang { get; set; } = "Tất cả";

        public List<ThongKeSachHongMatChiTietViewModel> DanhSach { get; set; } = new();

        public int TongSoSachHongMat { get; set; }

        public int SoSachHong { get; set; }

        public int SoSachMat { get; set; }

        public string? ThongBao { get; set; }
    }

    public class ThongKeSachHongMatChiTietViewModel
    {
        public string? MaSach { get; set; }

        public string? TenSach { get; set; }

        public string? TheLoai { get; set; }

        public string? TacGia { get; set; }

        public string? NXB { get; set; }

        public string? TinhTrang { get; set; }
    }
    public class SachGoiYViewModel
    {
        public string? MaSach { get; set; }

        public string? TenSach { get; set; }

        public string? SoPhieuMuon { get; set; }
    }
}
