using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSDLNC.Models.Admin;

public class NguoiDungIndexViewModel
{
    public List<NguoiDung> Users { get; set; } = new();
    public List<SelectListItem> AvailableGroups { get; set; } = new();
    public string? Keyword { get; set; }
    public string? Group { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public int TotalUsers { get; set; }
    public int TotalPages => TotalUsers == 0 ? 1 : (int)Math.Ceiling((double)TotalUsers / PageSize);
}
