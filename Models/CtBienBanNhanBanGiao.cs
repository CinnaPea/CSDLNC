using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class CtBienBanNhanBanGiao
{
    public string Sobienban { get; set; } = null!;

    public string Madausach { get; set; } = null!;

    public int Soluongsachmoidausach { get; set; }

    public string? Ghichu { get; set; }

    public virtual DauSach MadausachNavigation { get; set; } = null!;

    public virtual BienBanNhanBanGiao SobienbanNavigation { get; set; } = null!;
}
