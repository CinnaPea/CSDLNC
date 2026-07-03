using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class SinhVien
{
    public string Masinhvien { get; set; } = null!;

    public string Hoten { get; set; } = null!;

    public string? Gioitinh { get; set; }

    public string? Lop { get; set; }

    public string? Khoa { get; set; }

    public string? Sodienthoai { get; set; }

    public int Sosachdangmuon { get; set; }

    public int Solanvipham { get; set; }

    public decimal Sotienphatchuatra { get; set; }

    public DateOnly? Ngayviphamgannhat { get; set; }

    public string Trangthai { get; set; } = null!;

    public virtual ICollection<PhieuMuon> PhieuMuons { get; set; } = new List<PhieuMuon>();

    public virtual ICollection<PhieuPhatHongMat> PhieuPhatHongMats { get; set; } = new List<PhieuPhatHongMat>();

    public virtual ICollection<PhieuPhatQuaHan> PhieuPhatQuaHans { get; set; } = new List<PhieuPhatQuaHan>();
}
