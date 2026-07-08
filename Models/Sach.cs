using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class Sach
{
    public string Masach { get; set; } = null!;

    public string? Madausach { get; set; }

    public string? Mavitri { get; set; }

    public string Tensach { get; set; } = null!;

    public string? Theloai { get; set; }

    public string? Tacgia { get; set; }

    public string? Nhaxuatban { get; set; }

    public int? Namxuatban { get; set; }

    public string Tinhtrang { get; set; } = null!;

    public string Trangthai { get; set; } = null!;

    public DateOnly? Ngaynhap { get; set; }

    public DateOnly? Ngaycapnhattrangthai { get; set; }

    public string? Sophieumuonhientai { get; set; }

    public int Solanmuon { get; set; }

    public DateOnly? Ngaymuongannhat { get; set; }

    public virtual ICollection<CtPhieuHuySach> CtPhieuHuySaches { get; set; } = new List<CtPhieuHuySach>();

    public virtual ICollection<CtPhieuKiemKe> CtPhieuKiemKes { get; set; } = new List<CtPhieuKiemKe>();

    public virtual ICollection<CtPhieuMuon> CtPhieuMuons { get; set; } = new List<CtPhieuMuon>();

    public virtual ICollection<CtPhieuPhatHongMat> CtPhieuPhatHongMats { get; set; } = new List<CtPhieuPhatHongMat>();

    public virtual ICollection<CtPhieuPhatQuaHan> CtPhieuPhatQuaHans { get; set; } = new List<CtPhieuPhatQuaHan>();

    public virtual ICollection<CtPhieuThanhLy> CtPhieuThanhLies { get; set; } = new List<CtPhieuThanhLy>();

    public virtual DauSach? MadausachNavigation { get; set; }

    public virtual ViTriLuuTru? MavitriNavigation { get; set; }
}
