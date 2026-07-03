using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class NhaCungCap
{
    public string Manhacungcap { get; set; } = null!;

    public string Tennhacungcap { get; set; } = null!;

    public string? Diachi { get; set; }

    public string? Sodienthoai { get; set; }

    public virtual ICollection<BienBanNhanBanGiao> BienBanNhanBanGiaos { get; set; } = new List<BienBanNhanBanGiao>();
}
