using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class DauSach
{
    public string Madausach { get; set; } = null!;

    public string Tendausach { get; set; } = null!;

    public string? Theloai { get; set; }

    public string? Tacgia { get; set; }

    public int? Namxuatban { get; set; }

    public string? Manhaxuatban { get; set; }

    public int Soluong { get; set; }

    public string? Chuoitimkiem { get; set; }

    public int Soluonghienco { get; set; }

    public int Soluongdangmuon { get; set; }

    public int Soluonghongmat { get; set; }

    public DateOnly? Lanmuongannhat { get; set; }

    public virtual ICollection<CtBienBanNhanBanGiao> CtBienBanNhanBanGiaos { get; set; } = new List<CtBienBanNhanBanGiao>();

    public virtual NhaXuatBan? ManhaxuatbanNavigation { get; set; }

    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
