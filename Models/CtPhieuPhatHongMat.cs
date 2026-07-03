using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class CtPhieuPhatHongMat
{
    public string Sophieuphathongmat { get; set; } = null!;

    public string Masach { get; set; } = null!;

    public string? Tinhtrang { get; set; }

    public string? Mucdo { get; set; }

    public decimal Phiphat { get; set; }

    public decimal Tongphat { get; set; }

    public virtual Sach MasachNavigation { get; set; } = null!;

    public virtual PhieuPhatHongMat SophieuphathongmatNavigation { get; set; } = null!;
}
