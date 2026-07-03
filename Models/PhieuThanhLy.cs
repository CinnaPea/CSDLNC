using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class PhieuThanhLy
{
    public string Mathanhly { get; set; } = null!;

    public DateOnly Ngaythanhly { get; set; }

    public string? Nguoilapphieu { get; set; }

    public string? Manguoidung { get; set; }

    public string? Tennguoidung { get; set; }

    public virtual ICollection<CtPhieuThanhLy> CtPhieuThanhLies { get; set; } = new List<CtPhieuThanhLy>();

    public virtual NguoiDung? ManguoidungNavigation { get; set; }
}
