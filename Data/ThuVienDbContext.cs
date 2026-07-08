using System;
using System.Collections.Generic;
using CSDLNC.Models;
using Microsoft.EntityFrameworkCore;

namespace CSDLNC.Data;

public partial class ThuVienDbContext : DbContext
{
    public ThuVienDbContext()
    {
    }

    public ThuVienDbContext(DbContextOptions<ThuVienDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BienBanNhanBanGiao> BienBanNhanBanGiaos { get; set; }

    public virtual DbSet<CtBienBanNhanBanGiao> CtBienBanNhanBanGiaos { get; set; }

    public virtual DbSet<CtPhieuHuySach> CtPhieuHuySaches { get; set; }

    public virtual DbSet<CtPhieuKiemKe> CtPhieuKiemKes { get; set; }

    public virtual DbSet<CtPhieuMuon> CtPhieuMuons { get; set; }

    public virtual DbSet<CtPhieuPhatHongMat> CtPhieuPhatHongMats { get; set; }

    public virtual DbSet<CtPhieuPhatQuaHan> CtPhieuPhatQuaHans { get; set; }

    public virtual DbSet<CtPhieuThanhLy> CtPhieuThanhLies { get; set; }

    public virtual DbSet<DauSach> DauSaches { get; set; }

    public virtual DbSet<LichSuDangNhap> LichSuDangNhaps { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<NhaXuatBan> NhaXuatBans { get; set; }

    public virtual DbSet<NhatKyThayDoi> NhatKyThayDois { get; set; }

    public virtual DbSet<NhomNguoiDung> NhomNguoiDungs { get; set; }

    public virtual DbSet<PhieuHuySach> PhieuHuySaches { get; set; }

    public virtual DbSet<PhieuKiemKe> PhieuKiemKes { get; set; }

    public virtual DbSet<PhieuMuon> PhieuMuons { get; set; }

    public virtual DbSet<PhieuPhatHongMat> PhieuPhatHongMats { get; set; }

    public virtual DbSet<PhieuPhatQuaHan> PhieuPhatQuaHans { get; set; }

    public virtual DbSet<PhieuThanhLy> PhieuThanhLies { get; set; }

    public virtual DbSet<Quyen> Quyens { get; set; }

    public virtual DbSet<Sach> Saches { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<ViTriLuuTru> ViTriLuuTrus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BienBanNhanBanGiao>(entity =>
        {
            entity.HasKey(e => e.Sobienban);

            entity.ToTable("bien_ban_nhan_ban_giao");

            entity.HasIndex(e => new { e.Manhacungcap, e.Ngaylap }, "IX_bbbg_ncc");

            entity.Property(e => e.Sobienban)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sobienban");
            entity.Property(e => e.Chucvubenbangiao)
                .HasMaxLength(100)
                .HasColumnName("chucvubenbangiao");
            entity.Property(e => e.Chucvubennhan)
                .HasMaxLength(100)
                .HasColumnName("chucvubennhan");
            entity.Property(e => e.Daidienbenbangiao)
                .HasMaxLength(100)
                .HasColumnName("daidienbenbangiao");
            entity.Property(e => e.Daidienbennhan)
                .HasMaxLength(100)
                .HasColumnName("daidienbennhan");
            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Manhacungcap)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manhacungcap");
            entity.Property(e => e.Ngaylap).HasColumnName("ngaylap");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(100)
                .HasColumnName("tennguoidung");
            entity.Property(e => e.Tongsodausach).HasColumnName("tongsodausach");
            entity.Property(e => e.Tongsoluongsach).HasColumnName("tongsoluongsach");

            entity.HasOne(d => d.ManguoidungNavigation).WithMany(p => p.BienBanNhanBanGiaos)
                .HasForeignKey(d => d.Manguoidung)
                .HasConstraintName("FK_bbbg_nd");

            entity.HasOne(d => d.ManhacungcapNavigation).WithMany(p => p.BienBanNhanBanGiaos)
                .HasForeignKey(d => d.Manhacungcap)
                .HasConstraintName("FK_bbbg_ncc");
        });

        modelBuilder.Entity<CtBienBanNhanBanGiao>(entity =>
        {
            entity.HasKey(e => new { e.Sobienban, e.Madausach });

            entity.ToTable("ct_bien_ban_nhan_ban_giao");

            entity.Property(e => e.Sobienban)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sobienban");
            entity.Property(e => e.Madausach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("madausach");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(255)
                .HasColumnName("ghichu");
            entity.Property(e => e.Soluongsachmoidausach).HasColumnName("soluongsachmoidausach");

            entity.HasOne(d => d.MadausachNavigation).WithMany(p => p.CtBienBanNhanBanGiaos)
                .HasForeignKey(d => d.Madausach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctbbbg_ds");

            entity.HasOne(d => d.SobienbanNavigation).WithMany(p => p.CtBienBanNhanBanGiaos)
                .HasForeignKey(d => d.Sobienban)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctbbbg_bbbg");
        });

        modelBuilder.Entity<CtPhieuHuySach>(entity =>
        {
            entity.HasKey(e => new { e.Mahuy, e.Masach });

            entity.ToTable("ct_phieu_huy_sach");

            entity.Property(e => e.Mahuy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("mahuy");
            entity.Property(e => e.Masach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masach");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(255)
                .HasColumnName("ghichu");
            entity.Property(e => e.Lydohuy)
                .HasMaxLength(255)
                .HasColumnName("lydohuy");

            entity.HasOne(d => d.MahuyNavigation).WithMany(p => p.CtPhieuHuySaches)
                .HasForeignKey(d => d.Mahuy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cths_phs");

            entity.HasOne(d => d.MasachNavigation).WithMany(p => p.CtPhieuHuySaches)
                .HasForeignKey(d => d.Masach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cths_sach");
        });

        modelBuilder.Entity<CtPhieuKiemKe>(entity =>
        {
            entity.HasKey(e => new { e.Makiemke, e.Masach });

            entity.ToTable("ct_phieu_kiem_ke");

            entity.Property(e => e.Makiemke)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("makiemke");
            entity.Property(e => e.Masach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masach");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(255)
                .HasColumnName("ghichu");

            entity.HasOne(d => d.MakiemkeNavigation).WithMany(p => p.CtPhieuKiemKes)
                .HasForeignKey(d => d.Makiemke)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctkk_pkk");

            entity.HasOne(d => d.MasachNavigation).WithMany(p => p.CtPhieuKiemKes)
                .HasForeignKey(d => d.Masach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctkk_sach");
        });

        modelBuilder.Entity<CtPhieuMuon>(entity =>
        {
            entity.HasKey(e => new { e.Sophieumuon, e.Masach });

            entity.ToTable("ct_phieu_muon");

            entity.HasIndex(e => e.Masach, "IX_ct_phieu_muon_masach");

            entity.Property(e => e.Sophieumuon)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sophieumuon");
            entity.Property(e => e.Masach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masach");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(255)
                .HasColumnName("ghichu");
            entity.Property(e => e.Madausach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("madausach");
            entity.Property(e => e.Masinhvien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masinhvien");
            entity.Property(e => e.Ngaytra).HasColumnName("ngaytra");
            entity.Property(e => e.Soluong)
                .HasDefaultValue(1)
                .HasColumnName("soluong");
            entity.Property(e => e.Theloai)
                .HasMaxLength(100)
                .HasColumnName("theloai");
            entity.Property(e => e.Tongsoluong)
                .HasDefaultValue(1)
                .HasColumnName("tongsoluong");

            entity.HasOne(d => d.MasachNavigation).WithMany(p => p.CtPhieuMuons)
                .HasForeignKey(d => d.Masach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctpm_sach");

            entity.HasOne(d => d.SophieumuonNavigation).WithMany(p => p.CtPhieuMuons)
                .HasForeignKey(d => d.Sophieumuon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctpm_pm");
        });

        modelBuilder.Entity<CtPhieuPhatHongMat>(entity =>
        {
            entity.HasKey(e => new { e.Sophieuphathongmat, e.Masach }).HasName("PK_ct_pphm");

            entity.ToTable("ct_phieu_phat_hong_mat");

            entity.Property(e => e.Sophieuphathongmat)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("sophieuphathongmat");
            entity.Property(e => e.Masach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masach");
            entity.Property(e => e.Mucdo)
                .HasMaxLength(255)
                .HasColumnName("mucdo");
            entity.Property(e => e.Phiphat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phiphat");
            entity.Property(e => e.Tinhtrang)
                .HasMaxLength(50)
                .HasColumnName("tinhtrang");
            entity.Property(e => e.Tongphat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tongphat");

            entity.HasOne(d => d.MasachNavigation).WithMany(p => p.CtPhieuPhatHongMats)
                .HasForeignKey(d => d.Masach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctpphm_sach");

            entity.HasOne(d => d.SophieuphathongmatNavigation).WithMany(p => p.CtPhieuPhatHongMats)
                .HasForeignKey(d => d.Sophieuphathongmat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctpphm_pphm");
        });

        modelBuilder.Entity<CtPhieuPhatQuaHan>(entity =>
        {
            entity.HasKey(e => new { e.Sophieuphatquahan, e.Masach }).HasName("PK_ct_ppqh");

            entity.ToTable("ct_phieu_phat_qua_han");

            entity.Property(e => e.Sophieuphatquahan)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("sophieuphatquahan");
            entity.Property(e => e.Masach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masach");
            entity.Property(e => e.Hantra).HasColumnName("hantra");
            entity.Property(e => e.Ngaymuon).HasColumnName("ngaymuon");
            entity.Property(e => e.Ngaytra).HasColumnName("ngaytra");
            entity.Property(e => e.Phiphat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("phiphat");
            entity.Property(e => e.Songayquahan).HasColumnName("songayquahan");
            entity.Property(e => e.Tongphat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tongphat");

            entity.HasOne(d => d.MasachNavigation).WithMany(p => p.CtPhieuPhatQuaHans)
                .HasForeignKey(d => d.Masach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctppqh_sach");

            entity.HasOne(d => d.SophieuphatquahanNavigation).WithMany(p => p.CtPhieuPhatQuaHans)
                .HasForeignKey(d => d.Sophieuphatquahan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ctppqh_ppqh");
        });

        modelBuilder.Entity<CtPhieuThanhLy>(entity =>
        {
            entity.HasKey(e => new { e.Mathanhly, e.Masach });

            entity.ToTable("ct_phieu_thanh_ly");

            entity.Property(e => e.Mathanhly)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("mathanhly");
            entity.Property(e => e.Masach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masach");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(255)
                .HasColumnName("ghichu");
            entity.Property(e => e.Lydothanhly)
                .HasMaxLength(255)
                .HasColumnName("lydothanhly");

            entity.HasOne(d => d.MasachNavigation).WithMany(p => p.CtPhieuThanhLies)
                .HasForeignKey(d => d.Masach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cttl_sach");

            entity.HasOne(d => d.MathanhlyNavigation).WithMany(p => p.CtPhieuThanhLies)
                .HasForeignKey(d => d.Mathanhly)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cttl_ptl");
        });

        modelBuilder.Entity<DauSach>(entity =>
        {
            entity.HasKey(e => e.Madausach);

            entity.ToTable("dau_sach");

            entity.Property(e => e.Madausach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("madausach");
            entity.Property(e => e.Chuoitimkiem)
                .HasMaxLength(500)
                .HasColumnName("chuoitimkiem");
            entity.Property(e => e.Lanmuongannhat).HasColumnName("lanmuongannhat");
            entity.Property(e => e.Manhaxuatban)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manhaxuatban");
            entity.Property(e => e.Namxuatban).HasColumnName("namxuatban");
            entity.Property(e => e.Soluong).HasColumnName("soluong");
            entity.Property(e => e.Soluongdangmuon).HasColumnName("soluongdangmuon");
            entity.Property(e => e.Soluonghienco).HasColumnName("soluonghienco");
            entity.Property(e => e.Soluonghongmat).HasColumnName("soluonghongmat");
            entity.Property(e => e.Tacgia)
                .HasMaxLength(150)
                .HasColumnName("tacgia");
            entity.Property(e => e.Tendausach)
                .HasMaxLength(255)
                .HasColumnName("tendausach");
            entity.Property(e => e.Theloai)
                .HasMaxLength(100)
                .HasColumnName("theloai");

            entity.HasOne(d => d.ManhaxuatbanNavigation).WithMany(p => p.DauSaches)
                .HasForeignKey(d => d.Manhaxuatban)
                .HasConstraintName("FK_dau_sach_nxb");
        });

        modelBuilder.Entity<LichSuDangNhap>(entity =>
        {
            entity.HasKey(e => e.Maphien);

            entity.ToTable("lich_su_dang_nhap");

            entity.HasIndex(e => new { e.Manguoidung, e.Thoidiemdangnhap }, "IX_lsdn_manguoidung");

            entity.Property(e => e.Maphien)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("maphien");
            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Thoidiemdangnhap).HasColumnName("thoidiemdangnhap");
            entity.Property(e => e.Thoidiemdangxuat).HasColumnName("thoidiemdangxuat");

            entity.HasOne(d => d.ManguoidungNavigation).WithMany(p => p.LichSuDangNhaps)
                .HasForeignKey(d => d.Manguoidung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_lsdn_nd");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Manguoidung);

            entity.ToTable("nguoi_dung");

            entity.HasIndex(e => e.Tendangnhap, "UQ__nguoi_du__CE900A1E4790BAE0").IsUnique();

            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Matkhau)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("matkhau");
            entity.Property(e => e.Tendangnhap)
                .HasMaxLength(50)
                .HasColumnName("tendangnhap");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(100)
                .HasColumnName("tennguoidung");

            entity.HasMany(d => d.Maquyens).WithMany(p => p.Manguoidungs)
                .UsingEntity<Dictionary<string, object>>(
                    "NguoiDungQuyen",
                    r => r.HasOne<Quyen>().WithMany()
                        .HasForeignKey("Maquyen")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ndq_quyen"),
                    l => l.HasOne<NguoiDung>().WithMany()
                        .HasForeignKey("Manguoidung")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ndq_nd"),
                    j =>
                    {
                        j.HasKey("Manguoidung", "Maquyen");
                        j.ToTable("nguoi_dung_quyen");
                        j.IndexerProperty<string>("Manguoidung")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("manguoidung");
                        j.IndexerProperty<string>("Maquyen")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("maquyen");
                    });
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.Manhacungcap);

            entity.ToTable("nha_cung_cap");

            entity.Property(e => e.Manhacungcap)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manhacungcap");
            entity.Property(e => e.Diachi)
                .HasMaxLength(255)
                .HasColumnName("diachi");
            entity.Property(e => e.Sodienthoai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sodienthoai");
            entity.Property(e => e.Tennhacungcap)
                .HasMaxLength(100)
                .HasColumnName("tennhacungcap");
        });

        modelBuilder.Entity<NhaXuatBan>(entity =>
        {
            entity.HasKey(e => e.Manhaxuatban);

            entity.ToTable("nha_xuat_ban");

            entity.Property(e => e.Manhaxuatban)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manhaxuatban");
            entity.Property(e => e.Diachi)
                .HasMaxLength(255)
                .HasColumnName("diachi");
            entity.Property(e => e.Sodienthoai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sodienthoai");
            entity.Property(e => e.Tennhaxuatban)
                .HasMaxLength(100)
                .HasColumnName("tennhaxuatban");
        });

        modelBuilder.Entity<NhatKyThayDoi>(entity =>
        {
            entity.HasKey(e => e.Manhatky);

            entity.ToTable("nhat_ky_thay_doi");

            entity.Property(e => e.Manhatky)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("manhatky");
            entity.Property(e => e.Maphien)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("maphien");
            entity.Property(e => e.Noidungthaydoi)
                .HasMaxLength(500)
                .HasColumnName("noidungthaydoi");
            entity.Property(e => e.Thoigianthaydoi)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("thoigianthaydoi");
            entity.Property(e => e.Thongtincu).HasColumnName("thongtincu");
            entity.Property(e => e.Thongtinmoi).HasColumnName("thongtinmoi");

            entity.HasOne(d => d.MaphienNavigation).WithMany(p => p.NhatKyThayDois)
                .HasForeignKey(d => d.Maphien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nktd_lsdn");
        });

        modelBuilder.Entity<NhomNguoiDung>(entity =>
        {
            entity.HasKey(e => e.Manhom);

            entity.ToTable("nhom_nguoi_dung");

            entity.Property(e => e.Manhom)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manhom");
            entity.Property(e => e.Tennhom)
                .HasMaxLength(100)
                .HasColumnName("tennhom");

            entity.HasMany(d => d.Manguoidungs).WithMany(p => p.Manhoms)
                .UsingEntity<Dictionary<string, object>>(
                    "NhomNguoiDungCt",
                    r => r.HasOne<NguoiDung>().WithMany()
                        .HasForeignKey("Manguoidung")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_nndct_nd"),
                    l => l.HasOne<NhomNguoiDung>().WithMany()
                        .HasForeignKey("Manhom")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_nndct_nhom"),
                    j =>
                    {
                        j.HasKey("Manhom", "Manguoidung");
                        j.ToTable("nhom_nguoi_dung_ct");
                        j.IndexerProperty<string>("Manhom")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("manhom");
                        j.IndexerProperty<string>("Manguoidung")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("manguoidung");
                    });

            entity.HasMany(d => d.Maquyens).WithMany(p => p.Manhoms)
                .UsingEntity<Dictionary<string, object>>(
                    "NhomQuyen",
                    r => r.HasOne<Quyen>().WithMany()
                        .HasForeignKey("Maquyen")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_nq_quyen"),
                    l => l.HasOne<NhomNguoiDung>().WithMany()
                        .HasForeignKey("Manhom")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_nq_nhom"),
                    j =>
                    {
                        j.HasKey("Manhom", "Maquyen");
                        j.ToTable("nhom_quyen");
                        j.IndexerProperty<string>("Manhom")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("manhom");
                        j.IndexerProperty<string>("Maquyen")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("maquyen");
                    });
        });

        modelBuilder.Entity<PhieuHuySach>(entity =>
        {
            entity.HasKey(e => e.Mahuy);

            entity.ToTable("phieu_huy_sach");

            entity.Property(e => e.Mahuy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("mahuy");
            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Ngaylapphieu).HasColumnName("ngaylapphieu");
            entity.Property(e => e.Nguoilapphieu)
                .HasMaxLength(100)
                .HasColumnName("nguoilapphieu");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(100)
                .HasColumnName("tennguoidung");

            entity.HasOne(d => d.ManguoidungNavigation).WithMany(p => p.PhieuHuySaches)
                .HasForeignKey(d => d.Manguoidung)
                .HasConstraintName("FK_phs_nd");
        });

        modelBuilder.Entity<PhieuKiemKe>(entity =>
        {
            entity.HasKey(e => e.Makiemke);

            entity.ToTable("phieu_kiem_ke");

            entity.Property(e => e.Makiemke)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("makiemke");
            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Ngaykiemke).HasColumnName("ngaykiemke");
            entity.Property(e => e.Nguoilapphieu)
                .HasMaxLength(100)
                .HasColumnName("nguoilapphieu");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(100)
                .HasColumnName("tennguoidung");

            entity.HasOne(d => d.ManguoidungNavigation).WithMany(p => p.PhieuKiemKes)
                .HasForeignKey(d => d.Manguoidung)
                .HasConstraintName("FK_pkk_nd");
        });

        modelBuilder.Entity<PhieuMuon>(entity =>
        {
            entity.HasKey(e => e.Sophieumuon);

            entity.ToTable("phieu_muon");

            entity.HasIndex(e => new { e.Masinhvien, e.Ngaymuon }, "IX_phieu_muon_sv");

            entity.Property(e => e.Sophieumuon)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sophieumuon");
            entity.Property(e => e.Hantra).HasColumnName("hantra");
            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Masinhvien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masinhvien");
            entity.Property(e => e.Ngayhoantat).HasColumnName("ngayhoantat");
            entity.Property(e => e.Ngaymuon).HasColumnName("ngaymuon");
            entity.Property(e => e.Nguoilapphieu)
                .HasMaxLength(100)
                .HasColumnName("nguoilapphieu");
            entity.Property(e => e.Songayquahan).HasColumnName("songayquahan");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(100)
                .HasColumnName("tennguoidung");
            entity.Property(e => e.Tongsoluong).HasColumnName("tongsoluong");
            entity.Property(e => e.Trangthaiphieu)
                .HasMaxLength(50)
                .HasDefaultValue("Đang mượn")
                .HasColumnName("trangthaiphieu");

            entity.HasOne(d => d.ManguoidungNavigation).WithMany(p => p.PhieuMuons)
                .HasForeignKey(d => d.Manguoidung)
                .HasConstraintName("FK_pm_nguoi_dung");

            entity.HasOne(d => d.MasinhvienNavigation).WithMany(p => p.PhieuMuons)
                .HasForeignKey(d => d.Masinhvien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pm_sv");
        });

        modelBuilder.Entity<PhieuPhatHongMat>(entity =>
        {
            entity.HasKey(e => e.Sophieuphathongmat).HasName("PK_pphm");

            entity.ToTable("phieu_phat_hong_mat");

            entity.HasIndex(e => e.Ngaylap, "IX_pphm_ngaylap");

            entity.Property(e => e.Sophieuphathongmat)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("sophieuphathongmat");
            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Masinhvien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masinhvien");
            entity.Property(e => e.Ngaylap).HasColumnName("ngaylap");
            entity.Property(e => e.Ngaythanhtoan).HasColumnName("ngaythanhtoan");
            entity.Property(e => e.Nguoithutien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nguoithutien");
            entity.Property(e => e.Sophieumuon)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sophieumuon");
            entity.Property(e => e.Sotienconlai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sotienconlai");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(100)
                .HasColumnName("tennguoidung");
            entity.Property(e => e.Tongphat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tongphat");
            entity.Property(e => e.Trangthaithanhtoan)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa thanh toán")
                .HasColumnName("trangthaithanhtoan");

            entity.HasOne(d => d.ManguoidungNavigation).WithMany(p => p.PhieuPhatHongMats)
                .HasForeignKey(d => d.Manguoidung)
                .HasConstraintName("FK_pphm_nguoi_dung");

            entity.HasOne(d => d.MasinhvienNavigation).WithMany(p => p.PhieuPhatHongMats)
                .HasForeignKey(d => d.Masinhvien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pphm_sv");

            entity.HasOne(d => d.SophieumuonNavigation).WithMany(p => p.PhieuPhatHongMats)
                .HasForeignKey(d => d.Sophieumuon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pphm_pm");
        });

        modelBuilder.Entity<PhieuPhatQuaHan>(entity =>
        {
            entity.HasKey(e => e.Sophieuphatquahan).HasName("PK_ppqh");

            entity.ToTable("phieu_phat_qua_han");

            entity.HasIndex(e => e.Ngaylap, "IX_ppqh_ngaylap");

            entity.Property(e => e.Sophieuphatquahan)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("sophieuphatquahan");
            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Masinhvien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masinhvien");
            entity.Property(e => e.Ngaylap).HasColumnName("ngaylap");
            entity.Property(e => e.Ngaythanhtoan).HasColumnName("ngaythanhtoan");
            entity.Property(e => e.Nguoithutien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nguoithutien");
            entity.Property(e => e.Sophieumuon)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sophieumuon");
            entity.Property(e => e.Sotienconlai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sotienconlai");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(100)
                .HasColumnName("tennguoidung");
            entity.Property(e => e.Tongphat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tongphat");
            entity.Property(e => e.Trangthaithanhtoan)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa thanh toán")
                .HasColumnName("trangthaithanhtoan");

            entity.HasOne(d => d.ManguoidungNavigation).WithMany(p => p.PhieuPhatQuaHans)
                .HasForeignKey(d => d.Manguoidung)
                .HasConstraintName("FK_ppqh_nguoi_dung");

            entity.HasOne(d => d.MasinhvienNavigation).WithMany(p => p.PhieuPhatQuaHans)
                .HasForeignKey(d => d.Masinhvien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ppqh_sv");

            entity.HasOne(d => d.SophieumuonNavigation).WithMany(p => p.PhieuPhatQuaHans)
                .HasForeignKey(d => d.Sophieumuon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ppqh_pm");
        });

        modelBuilder.Entity<PhieuThanhLy>(entity =>
        {
            entity.HasKey(e => e.Mathanhly);

            entity.ToTable("phieu_thanh_ly");

            entity.Property(e => e.Mathanhly)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("mathanhly");
            entity.Property(e => e.Manguoidung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manguoidung");
            entity.Property(e => e.Ngaythanhly).HasColumnName("ngaythanhly");
            entity.Property(e => e.Nguoilapphieu)
                .HasMaxLength(100)
                .HasColumnName("nguoilapphieu");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(100)
                .HasColumnName("tennguoidung");

            entity.HasOne(d => d.ManguoidungNavigation).WithMany(p => p.PhieuThanhLies)
                .HasForeignKey(d => d.Manguoidung)
                .HasConstraintName("FK_ptl_nd");
        });

        modelBuilder.Entity<Quyen>(entity =>
        {
            entity.HasKey(e => e.Maquyen);

            entity.ToTable("quyen");

            entity.Property(e => e.Maquyen)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("maquyen");
            entity.Property(e => e.Tenquyen)
                .HasMaxLength(100)
                .HasColumnName("tenquyen");
        });

        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.Masach);

            entity.ToTable("sach");

            entity.HasIndex(e => new { e.Tinhtrang, e.Trangthai }, "IX_sach_tinhtrang");

            entity.Property(e => e.Masach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masach");
            entity.Property(e => e.Madausach)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("madausach");
            entity.Property(e => e.Mavitri)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("mavitri");
            entity.Property(e => e.Namxuatban).HasColumnName("namxuatban");
            entity.Property(e => e.Ngaycapnhattrangthai).HasColumnName("ngaycapnhattrangthai");
            entity.Property(e => e.Ngaymuongannhat).HasColumnName("ngaymuongannhat");
            entity.Property(e => e.Ngaynhap).HasColumnName("ngaynhap");
            entity.Property(e => e.Nhaxuatban)
                .HasMaxLength(150)
                .HasColumnName("nhaxuatban");
            entity.Property(e => e.Solanmuon).HasColumnName("solanmuon");
            entity.Property(e => e.Sophieumuonhientai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sophieumuonhientai");
            entity.Property(e => e.Tacgia)
                .HasMaxLength(150)
                .HasColumnName("tacgia");
            entity.Property(e => e.Tensach)
                .HasMaxLength(255)
                .HasColumnName("tensach");
            entity.Property(e => e.Theloai)
                .HasMaxLength(100)
                .HasColumnName("theloai");
            entity.Property(e => e.Tinhtrang)
                .HasMaxLength(50)
                .HasDefaultValue("Có thể mượn")
                .HasColumnName("tinhtrang");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(50)
                .HasDefaultValue("Trong kho")
                .HasColumnName("trangthai");

            entity.HasOne(d => d.MadausachNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.Madausach)
                .HasConstraintName("FK_sach_dau_sach");

            entity.HasOne(d => d.MavitriNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.Mavitri)
                .HasConstraintName("FK_sach_vitri");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.Masinhvien);

            entity.ToTable("sinh_vien");

            entity.Property(e => e.Masinhvien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("masinhvien");
            entity.Property(e => e.Gioitinh)
                .HasMaxLength(10)
                .HasColumnName("gioitinh");
            entity.Property(e => e.Hoten)
                .HasMaxLength(100)
                .HasColumnName("hoten");
            entity.Property(e => e.Khoa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("khoa");
            entity.Property(e => e.Lop)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("lop");
            entity.Property(e => e.Ngayviphamgannhat).HasColumnName("ngayviphamgannhat");
            entity.Property(e => e.Sodienthoai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sodienthoai");
            entity.Property(e => e.Solanvipham).HasColumnName("solanvipham");
            entity.Property(e => e.Sosachdangmuon).HasColumnName("sosachdangmuon");
            entity.Property(e => e.Sotienphatchuatra)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sotienphatchuatra");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(50)
                .HasDefaultValue("Được mượn")
                .HasColumnName("trangthai");
        });

        modelBuilder.Entity<ViTriLuuTru>(entity =>
        {
            entity.HasKey(e => e.Mavitri);

            entity.ToTable("vi_tri_luu_tru");

            entity.Property(e => e.Mavitri)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("mavitri");
            entity.Property(e => e.Day)
                .HasMaxLength(50)
                .HasColumnName("day");
            entity.Property(e => e.Ke)
                .HasMaxLength(50)
                .HasColumnName("ke");
            entity.Property(e => e.Khu)
                .HasMaxLength(50)
                .HasColumnName("khu");
            entity.Property(e => e.Mota)
                .HasMaxLength(255)
                .HasColumnName("mota");
            entity.Property(e => e.Ngan)
                .HasMaxLength(50)
                .HasColumnName("ngan");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
