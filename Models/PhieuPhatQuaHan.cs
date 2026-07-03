using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class PhieuPhatQuaHan
{
    public string Sophieuphatquahan { get; set; } = null!;

    public DateOnly Ngaylap { get; set; }

    public string Sophieumuon { get; set; } = null!;

    public string Masinhvien { get; set; } = null!;

    public string? Manguoidung { get; set; }

    public string? Tennguoidung { get; set; }

    public decimal Tongphat { get; set; }

    public string Trangthaithanhtoan { get; set; } = null!;

    public DateOnly? Ngaythanhtoan { get; set; }

    public string? Nguoithutien { get; set; }

    public decimal Sotienconlai { get; set; }

    public virtual ICollection<CtPhieuPhatQuaHan> CtPhieuPhatQuaHans { get; set; } = new List<CtPhieuPhatQuaHan>();

    public virtual NguoiDung? ManguoidungNavigation { get; set; }

    public virtual SinhVien MasinhvienNavigation { get; set; } = null!;

    public virtual PhieuMuon SophieumuonNavigation { get; set; } = null!;
}
