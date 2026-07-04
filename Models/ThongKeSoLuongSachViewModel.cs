namespace CSDLNC.Models;

public class ThongKeSoLuongSachViewModel
{
    public string? TuKhoa { get; set; }
    public string? TheLoai { get; set; }

    public int TongDauSach { get; set; }
    public int TongBan { get; set; }
    public int TongHienCo { get; set; }
    public int TongDangMuon { get; set; }
    public int TongHongMat { get; set; }

    public List<string> DanhSachTheLoai { get; set; } = new();
    public List<ThongKeSoLuongSachRow> Rows { get; set; } = new();
    public List<ThongKeTheLoaiRow> TheLoaiRows { get; set; } = new();
}

public class ThongKeSoLuongSachRow
{
    public string? MaDauSach { get; set; }
    public string? TenDauSach { get; set; }
    public string? TheLoai { get; set; }
    public string? TacGia { get; set; }

    public int TongSoLuong { get; set; }
    public int SoLuongHienCo { get; set; }
    public int SoLuongDangMuon { get; set; }
    public int SoLuongHongMat { get; set; }
}

public class ThongKeTheLoaiRow
{
    public string? TheLoai { get; set; }
    public int TongDauSach { get; set; }
    public int TongBan { get; set; }
}