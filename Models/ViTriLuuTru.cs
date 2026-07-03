using System;
using System.Collections.Generic;

namespace CSDLNC.Models;

public partial class ViTriLuuTru
{
    public string Mavitri { get; set; } = null!;

    public string? Khu { get; set; }

    public string? Day { get; set; }

    public string? Ke { get; set; }

    public string? Ngan { get; set; }

    public string? Mota { get; set; }

    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
