using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class CtPhieuThanhLy
{
    public string Mathanhly { get; set; } = null!;

    public string Masach { get; set; } = null!;

    public string? Lydothanhly { get; set; }

    public string? Ghichu { get; set; }

    public virtual Sach MasachNavigation { get; set; } = null!;

    public virtual PhieuThanhLy MathanhlyNavigation { get; set; } = null!;
}
