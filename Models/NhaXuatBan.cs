using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class NhaXuatBan
{
    public string Manhaxuatban { get; set; } = null!;

    public string Tennhaxuatban { get; set; } = null!;

    public string? Diachi { get; set; }

    public string? Sodienthoai { get; set; }

    public virtual ICollection<DauSach> DauSaches { get; set; } = new List<DauSach>();
}
