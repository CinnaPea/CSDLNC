using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class LichSuDangNhap
{
    public string Maphien { get; set; } = null!;

    public DateTime Thoidiemdangnhap { get; set; }

    public DateTime? Thoidiemdangxuat { get; set; }

    public string Manguoidung { get; set; } = null!;

    public virtual NguoiDung ManguoidungNavigation { get; set; } = null!;

    public virtual ICollection<NhatKyThayDoi> NhatKyThayDois { get; set; } = new List<NhatKyThayDoi>();
}
