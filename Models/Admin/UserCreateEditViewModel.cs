using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CSDLNC.Models.Admin;

public class UserCreateEditViewModel
{
    public List<string> EffectivePermissions { get; set; } = new();
    public List<string> GroupNames { get; set; } = new();
    public string? Manguoidung { get; set; }

    [Required(ErrorMessage = "Ten dang nhap la bat buoc.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Ten dang nhap phai tu 3 den 50 ky tu.")]
    [RegularExpression(@"^[A-Za-z0-9_.-]+$", ErrorMessage = "Ten dang nhap chi duoc chua chu, so, dau cham, gach duoi hoac gach ngang.")]
    public string Tendangnhap { get; set; } = null!;

    [Required(ErrorMessage = "Ho ten la bat buoc.")]
    [StringLength(100, ErrorMessage = "Ho ten toi da 100 ky tu.")]
    public string Tennguoidung { get; set; } = null!;

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mat khau phai tu 6 den 100 ky tu.")]
    public string? Matkhau { get; set; }

    // Group-based RBAC (IMPORTANT)
    public List<string> SelectedGroups { get; set; } = new();

    // Dropdown source
    public List<SelectListItem> AvailableGroups { get; set; } = new();
}
