using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSDLNC.Models.Admin;

public class UserCreateEditViewModel
{
    public List<string> EffectivePermissions { get; set; } = new();
    public List<string> GroupNames { get; set; } = new();
    public string? Manguoidung { get; set; }

    public string Tendangnhap { get; set; } = null!;
    public string Tennguoidung { get; set; } = null!;
    public string? Matkhau { get; set; }

    // Group-based RBAC (IMPORTANT)
    public List<string> SelectedGroups { get; set; } = new();

    // Dropdown source
    public List<SelectListItem> AvailableGroups { get; set; } = new();
}