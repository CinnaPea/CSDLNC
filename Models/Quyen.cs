using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class Quyen
{
    public string Maquyen { get; set; } = null!;

    public string Tenquyen { get; set; } = null!;

    public virtual ICollection<NguoiDung> Manguoidungs { get; set; } = new List<NguoiDung>();

    public virtual ICollection<NhomNguoiDung> Manhoms { get; set; } = new List<NhomNguoiDung>();
}
