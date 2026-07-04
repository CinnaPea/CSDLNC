using System.ComponentModel.DataAnnotations;

namespace CSDLNC.Models;

public class TraSachIndexViewModel
{
    public string? SoPhieuMuon { get; set; }

    public List<ChiTietPhieuMuonViewModel> ChiTiet { get; set; } = new();

    public List<PhieuMuonCanTraViewModel> PhieuMuonCanTra { get; set; } = new();
}

public class PhieuMuonCanTraViewModel
{
    public string SoPhieuMuon { get; set; } = "";

    public string MaSinhVien { get; set; } = "";

    public string HoTen { get; set; } = "";

    public DateOnly NgayMuon { get; set; }

    public DateOnly HanTra { get; set; }

    public int SoSachChuaTra { get; set; }
}

public class ChiTietPhieuMuonViewModel
{
    public string SoPhieuMuon { get; set; } = "";

    public DateTime NgayMuon { get; set; }

    public DateTime HanTra { get; set; }

    public string TrangThaiPhieu { get; set; } = "";

    public string MaSinhVien { get; set; } = "";

    public string HoTen { get; set; } = "";

    public string? GioiTinh { get; set; }

    public string? Lop { get; set; }

    public string? Khoa { get; set; }

    public string? SoDienThoai { get; set; }

    public string MaSach { get; set; } = "";

    public string TenSach { get; set; } = "";

    public string? TheLoai { get; set; }

    public string? TacGia { get; set; }

    public string? NhaXuatBan { get; set; }

    public int? NamXuatBan { get; set; }

    public DateTime? NgayTra { get; set; }

    public string TinhTrang { get; set; } = "";

    public string? GhiChu { get; set; }
}

public class CapNhatTraSachViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập số phiếu mượn")]
    public string SoPhieuMuon { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mã sách")]
    public string MaSach { get; set; } = "";

    [DataType(DataType.Date)]
    public DateTime NgayTra { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Vui lòng chọn tình trạng sau trả")]
    public string TinhTrangSauTra { get; set; } = "Tốt";

    [StringLength(255, ErrorMessage = "Ghi chú không quá 255 ký tự")]
    public string? GhiChu { get; set; }
}
