using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class CtPhieuPhatQuaHan
{
    public string Sophieuphatquahan { get; set; } = null!;

    public string Masach { get; set; } = null!;

    public int Songayquahan { get; set; }

    public decimal Phiphat { get; set; }

    public DateOnly? Ngaymuon { get; set; }

    public DateOnly? Hantra { get; set; }

    public DateOnly? Ngaytra { get; set; }

    public decimal Tongphat { get; set; }

    public virtual Sach MasachNavigation { get; set; } = null!;

    public virtual PhieuPhatQuaHan SophieuphatquahanNavigation { get; set; } = null!;
}
