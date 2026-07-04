using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSDLNC.Models;

public class BanGiaoCreateViewModel
{
    public string? DaidienbenbangiaoId { get; set; }
    public string? DaidienbennhanId { get; set; }

    public List<NhanSuOption> NhanSuOptions { get; set; } = new();

    public string Sobienban { get; set; } = "";
    public string? Manhacungcap { get; set; }
    public string? Madausach { get; set; }
    public int Soluongsachmoidausach { get; set; } = 1;
    public string? Ghichu { get; set; }

    public string? Daidienbenbangiao { get; set; }
    public string? Chucvubenbangiao { get; set; }
    public string? Daidienbennhan { get; set; }
    public string? Chucvubennhan { get; set; }

    public List<SelectListItem> NhaCungCapOptions { get; set; } = new();
    public List<SelectListItem> DauSachOptions { get; set; } = new();
    public List<BanGiaoRow> BienBans { get; set; } = new();
}

public class BanGiaoRow
{
    public string Sobienban { get; set; } = "";
    public DateOnly? Ngaylap { get; set; }
    public string? Manhacungcap { get; set; }
    public string? Tennhacungcap { get; set; }
    public int Tongsoluongsach { get; set; }
    public int Tongsodausach { get; set; }
}

public class NhanSuOption
{
    public string MaNguoiDung { get; set; } = "";
    public string TenNguoiDung { get; set; } = "";
    public string ChucVu { get; set; } = "";
}