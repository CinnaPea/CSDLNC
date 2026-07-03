using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class NguoiDung
{
    public string Manguoidung { get; set; } = null!;

    public string? Tennguoidung { get; set; }

    public string Tendangnhap { get; set; } = null!;

    public string Matkhau { get; set; } = null!;

    public virtual ICollection<BienBanNhanBanGiao> BienBanNhanBanGiaos { get; set; } = new List<BienBanNhanBanGiao>();

    public virtual ICollection<LichSuDangNhap> LichSuDangNhaps { get; set; } = new List<LichSuDangNhap>();

    public virtual ICollection<PhieuHuySach> PhieuHuySaches { get; set; } = new List<PhieuHuySach>();

    public virtual ICollection<PhieuKiemKe> PhieuKiemKes { get; set; } = new List<PhieuKiemKe>();

    public virtual ICollection<PhieuMuon> PhieuMuons { get; set; } = new List<PhieuMuon>();

    public virtual ICollection<PhieuPhatHongMat> PhieuPhatHongMats { get; set; } = new List<PhieuPhatHongMat>();

    public virtual ICollection<PhieuPhatQuaHan> PhieuPhatQuaHans { get; set; } = new List<PhieuPhatQuaHan>();

    public virtual ICollection<PhieuThanhLy> PhieuThanhLies { get; set; } = new List<PhieuThanhLy>();

    public virtual ICollection<NhomNguoiDung> Manhoms { get; set; } = new List<NhomNguoiDung>();

    public virtual ICollection<Quyen> Maquyens { get; set; } = new List<Quyen>();
}
