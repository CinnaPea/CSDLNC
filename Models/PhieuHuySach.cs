using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class PhieuHuySach
{
    public string Mahuy { get; set; } = null!;

    public DateOnly Ngaylapphieu { get; set; }

    public string? Nguoilapphieu { get; set; }

    public string? Manguoidung { get; set; }

    public string? Tennguoidung { get; set; }

    public virtual ICollection<CtPhieuHuySach> CtPhieuHuySaches { get; set; } = new List<CtPhieuHuySach>();

    public virtual NguoiDung? ManguoidungNavigation { get; set; }
}
