using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class CtPhieuMuon
{
    public string Sophieumuon { get; set; } = null!;

    public string Masach { get; set; } = null!;

    public DateOnly? Ngaytra { get; set; }

    public string? Ghichu { get; set; }

    public string? Kytra { get; set; }

    public int Tongsoluong { get; set; }

    public int Soluong { get; set; }

    public string? Masinhvien { get; set; }

    public string? Madausach { get; set; }

    public string? Theloai { get; set; }

    public virtual Sach MasachNavigation { get; set; } = null!;

    public virtual PhieuMuon SophieumuonNavigation { get; set; } = null!;
}
