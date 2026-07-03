using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class PhieuKiemKe
{
    public string Makiemke { get; set; } = null!;

    public DateOnly Ngaykiemke { get; set; }

    public string? Nguoilapphieu { get; set; }

    public string? Tennguoidung { get; set; }

    public string? Manguoidung { get; set; }

    public virtual ICollection<CtPhieuKiemKe> CtPhieuKiemKes { get; set; } = new List<CtPhieuKiemKe>();

    public virtual NguoiDung? ManguoidungNavigation { get; set; }
}
