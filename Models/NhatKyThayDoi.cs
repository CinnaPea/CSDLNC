using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class NhatKyThayDoi
{
    public string Manhatky { get; set; } = null!;

    public string Maphien { get; set; } = null!;

    public DateTime Thoigianthaydoi { get; set; }

    public string? Noidungthaydoi { get; set; }

    public string? Thongtincu { get; set; }

    public string? Thongtinmoi { get; set; }

    public virtual LichSuDangNhap MaphienNavigation { get; set; } = null!;
}
