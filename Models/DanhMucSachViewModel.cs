using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSDLNC.Models;

public class DanhMucSachViewModel
{
    public string? Madausach { get; set; }
    public string? Tendausach { get; set; }
    public string? Theloai { get; set; }
    public string? Tacgia { get; set; }
    public int? Namxuatban { get; set; }
    public string? Manhaxuatban { get; set; }
    public List<SelectListItem> TheLoaiOptions { get; set; } = new();
    public List<SelectListItem> NhaXuatBanOptions { get; set; } = new();
    public List<DanhMucSachRow> Rows { get; set; } = new();
}

public class DanhMucSachRow
{
    public string Madausach { get; set; } = "";
    public string? Tendausach { get; set; }
    public string? Theloai { get; set; }
    public string? Tacgia { get; set; }
    public int? Namxuatban { get; set; }
    public string? Manhaxuatban { get; set; }

    public int Soluong { get; set; }
    public int Soluonghienco { get; set; }
    public int Soluongdangmuon { get; set; }
    public int Soluonghongmat { get; set; }
}