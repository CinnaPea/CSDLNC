using System.ComponentModel.DataAnnotations;

namespace CSDLNC.Models;

public class PhieuMuonCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập số phiếu mượn")]
    [StringLength(20, ErrorMessage = "Số phiếu mượn không quá 20 ký tự")]
    public string SoPhieuMuon { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
    [StringLength(20, ErrorMessage = "Mã sinh viên không quá 20 ký tự")]
    public string MaSinhVien { get; set; } = "";

    public string MaSinhVienGoiY { get; set; } = "";

    [DataType(DataType.Date)]
    public DateTime NgayMuon { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime HanTra { get; set; } = DateTime.Today.AddDays(14);

    public List<PhieuMuonSachInputModel> DanhSachSach { get; set; } =
        new() { new PhieuMuonSachInputModel() };

    public List<SinhVienMuonSachOptionViewModel> SinhVienGoiY { get; set; } = new();

    public List<SachMuonOptionViewModel> SachCoTheMuon { get; set; } = new();
}

public class PhieuMuonIndexViewModel
{
    public string? TuKhoa { get; set; }

    public int TongPhieu { get; set; }

    public int DangMuon { get; set; }

    public int DaTra { get; set; }

    public int QuaHan { get; set; }

    public List<PhieuMuonListItemViewModel> DanhSachPhieu { get; set; } = new();
}

public class PhieuMuonListItemViewModel
{
    public string SoPhieuMuon { get; set; } = "";

    public string MaSinhVien { get; set; } = "";

    public string TenSinhVien { get; set; } = "";

    public DateOnly NgayMuon { get; set; }

    public DateOnly HanTra { get; set; }

    public DateOnly? NgayTra { get; set; }

    public string TrangThai { get; set; } = "";

    public int TongSoLuong { get; set; }

    public int SoSachChuaTra { get; set; }
}

public class PhieuMuonSachInputModel
{
    [StringLength(20, ErrorMessage = "Mã sách không quá 20 ký tự")]
    public string? MaSach { get; set; }

    [StringLength(255, ErrorMessage = "Ghi chú không quá 255 ký tự")]
    public string? GhiChu { get; set; }
}

public class PhieuMuonSinhVienCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
    [StringLength(20, ErrorMessage = "Mã sinh viên không quá 20 ký tự")]
    public string MaSinhVien { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ tên sinh viên")]
    [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
    public string HoTen { get; set; } = "";

    [StringLength(10, ErrorMessage = "Giới tính không quá 10 ký tự")]
    public string? GioiTinh { get; set; }

    [StringLength(50, ErrorMessage = "Lớp không quá 50 ký tự")]
    public string? Lop { get; set; }

    [StringLength(50, ErrorMessage = "Khóa không quá 50 ký tự")]
    public string? Khoa { get; set; }

    [StringLength(20, ErrorMessage = "Số điện thoại không quá 20 ký tự")]
    public string? SoDienThoai { get; set; }
}

public class SinhVienMuonSachOptionViewModel
{
    public string MaSinhVien { get; set; } = "";

    public string HoTen { get; set; } = "";

    public string? GioiTinh { get; set; }

    public string? Lop { get; set; }

    public string? Khoa { get; set; }

    public string? SoDienThoai { get; set; }

    public int SoSachDangMuon { get; set; }

    public string TrangThai { get; set; } = "";
}

public class SachMuonOptionViewModel
{
    public string MaSach { get; set; } = "";

    public string? MaDauSach { get; set; }

    public string TenSach { get; set; } = "";

    public string? TheLoai { get; set; }

    public string? TacGia { get; set; }

    public string? NhaXuatBan { get; set; }

    public string TinhTrang { get; set; } = "";
}
