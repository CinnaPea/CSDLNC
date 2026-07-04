namespace CSDLNC.Models;

public class PhieuMuonDetailsViewModel
{
    public string SoPhieuMuon { get; set; } = "";

    public List<ChiTietPhieuMuonViewModel> ChiTiet { get; set; } = new();

    public int TongSoLuong => ChiTiet.Count;
}
