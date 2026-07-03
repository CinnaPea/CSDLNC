using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class NhomNguoiDung
{
    public string Manhom { get; set; } = null!;

    public string Tennhom { get; set; } = null!;

    public virtual ICollection<NguoiDung> Manguoidungs { get; set; } = new List<NguoiDung>();

    public virtual ICollection<Quyen> Maquyens { get; set; } = new List<Quyen>();
}
