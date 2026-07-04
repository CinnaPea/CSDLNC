using System.ComponentModel.DataAnnotations;

namespace CSDLNC.Models;

public class ThongKeViPhamMuonTraViewModel
{
    [Range(1, 12, ErrorMessage = "Tháng phải từ 1 đến 12")]
    public int Thang { get; set; } = DateTime.Today.Month;

    [Range(2000, 2100, ErrorMessage = "Năm không hợp lệ")]
    public int Nam { get; set; } = DateTime.Today.Year;

    public string LoaiViPham { get; set; } = "Tất cả";

    public List<ThongKeViPhamMuonTraItemViewModel> KetQua { get; set; } = new();
}

public class ThongKeViPhamMuonTraItemViewModel
{
    public string MaSinhVien { get; set; } = "";

    public string HoTen { get; set; } = "";

    public string? Lop { get; set; }

    public string? Khoa { get; set; }

    public string LoaiViPham { get; set; } = "";

    public string MucDoViPham { get; set; } = "";

    public string HinhThucXuLy { get; set; } = "";

    public DateTime NgayLap { get; set; }

    public string GhiChu { get; set; } = "";
}
