using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class BienBanNhanBanGiao
{
    public string Sobienban { get; set; } = null!;

    public DateOnly Ngaylap { get; set; }

    public string? Daidienbenbangiao { get; set; }

    public string? Chucvubenbangiao { get; set; }

    public string? Daidienbennhan { get; set; }

    public string? Chucvubennhan { get; set; }

    public int Tongsodausach { get; set; }

    public int Tongsoluongsach { get; set; }

    public string? Manhacungcap { get; set; }

    public string? Tennguoidung { get; set; }

    public string? Manguoidung { get; set; }

    public virtual ICollection<CtBienBanNhanBanGiao> CtBienBanNhanBanGiaos { get; set; } = new List<CtBienBanNhanBanGiao>();

    public virtual NguoiDung? ManguoidungNavigation { get; set; }

    public virtual NhaCungCap? ManhacungcapNavigation { get; set; }
}
