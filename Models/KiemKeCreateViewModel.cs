using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSDLNC.Models;

public class KiemKeCreateViewModel
{
    public string Makiemke { get; set; } = "";
    public string? Masach { get; set; }
    public string? Tensach { get; set; }
    public string? Tinhtrang { get; set; }
    public string? Ghichu { get; set; }

    public List<SelectListItem> TinhTrangOptions { get; set; } = new();
    public List<KiemKeSachRow> Saches { get; set; } = new();
}

public class KiemKeSachRow
{
    public string Masach { get; set; } = "";
    public string? Tensach { get; set; }
    public string? Theloai { get; set; }
    public string? Tacgia { get; set; }
    public int? Namxuatban { get; set; }
    public string? Tinhtrang { get; set; }
    public string? Trangthai { get; set; }
    public string? Madausach { get; set; }
    public string? Mavitri { get; set; }
}