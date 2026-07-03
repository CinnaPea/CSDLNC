using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class CtPhieuKiemKe
{
    public string Makiemke { get; set; } = null!;

    public string Masach { get; set; } = null!;

    public string? Ghichu { get; set; }

    public virtual PhieuKiemKe MakiemkeNavigation { get; set; } = null!;

    public virtual Sach MasachNavigation { get; set; } = null!;
}
