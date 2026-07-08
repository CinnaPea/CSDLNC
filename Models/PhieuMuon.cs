using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class PhieuMuon
{
    public string Sophieumuon { get; set; } = null!;

    public string Masinhvien { get; set; } = null!;

    public DateOnly Ngaymuon { get; set; }

    public DateOnly Hantra { get; set; }

    public string? Nguoilapphieu { get; set; }

    public string? Manguoidung { get; set; }

    public string? Tennguoidung { get; set; }

    public string Trangthaiphieu { get; set; } = null!;

    public int Songayquahan { get; set; }


    public DateOnly? Ngayhoantat { get; set; }

    public int Tongsoluong { get; set; }

    public virtual ICollection<CtPhieuMuon> CtPhieuMuons { get; set; } = new List<CtPhieuMuon>();

    public virtual NguoiDung? ManguoidungNavigation { get; set; }

    public virtual SinhVien MasinhvienNavigation { get; set; } = null!;

    public virtual ICollection<PhieuPhatHongMat> PhieuPhatHongMats { get; set; } = new List<PhieuPhatHongMat>();

    public virtual ICollection<PhieuPhatQuaHan> PhieuPhatQuaHans { get; set; } = new List<PhieuPhatQuaHan>();
}
