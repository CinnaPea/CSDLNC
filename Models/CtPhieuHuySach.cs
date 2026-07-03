using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class CtPhieuHuySach
{
    public string Mahuy { get; set; } = null!;

    public string Masach { get; set; } = null!;

    public string? Lydohuy { get; set; }

    public string? Ghichu { get; set; }

    public virtual PhieuHuySach MahuyNavigation { get; set; } = null!;

    public virtual Sach MasachNavigation { get; set; } = null!;
}
