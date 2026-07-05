USE master;
GO

IF DB_ID(N'QL_ThuVien_CSDLNC') IS NOT NULL
BEGIN
    ALTER DATABASE QL_ThuVien_CSDLNC SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QL_ThuVien_CSDLNC;
END
GO

CREATE DATABASE QL_ThuVien_CSDLNC;
GO

USE QL_ThuVien_CSDLNC;
GO

CREATE TABLE sinh_vien
(
    masinhvien VARCHAR(20) NOT NULL CONSTRAINT PK_sinh_vien PRIMARY KEY,
    hoten NVARCHAR(100) NOT NULL,
    gioitinh NVARCHAR(10) NULL,
    lop VARCHAR(20) NULL,
    khoa VARCHAR(20) NULL,
    sodienthoai VARCHAR(20) NULL,
    sosachdangmuon INT NOT NULL CONSTRAINT DF_sv_sosach DEFAULT 0,
    solanvipham INT NOT NULL CONSTRAINT DF_sv_vipham DEFAULT 0,
    sotienphatchuatra DECIMAL(18,2) NOT NULL CONSTRAINT DF_sv_tienphat DEFAULT 0,
    ngayviphamgannhat DATE NULL,
    trangthai NVARCHAR(50) NOT NULL CONSTRAINT DF_sv_trangthai DEFAULT N'Được mượn'
);
GO

CREATE TABLE nha_xuat_ban
(
    manhaxuatban VARCHAR(20) NOT NULL CONSTRAINT PK_nha_xuat_ban PRIMARY KEY,
    tennhaxuatban NVARCHAR(100) NOT NULL,
    diachi NVARCHAR(255) NULL,
    sodienthoai VARCHAR(20) NULL
);
GO

CREATE TABLE nha_cung_cap
(
    manhacungcap VARCHAR(20) NOT NULL CONSTRAINT PK_nha_cung_cap PRIMARY KEY,
    tennhacungcap NVARCHAR(100) NOT NULL,
    diachi NVARCHAR(255) NULL,
    sodienthoai VARCHAR(20) NULL
);
GO

CREATE TABLE vi_tri_luu_tru
(
    mavitri VARCHAR(20) NOT NULL CONSTRAINT PK_vi_tri_luu_tru PRIMARY KEY,
    khu NVARCHAR(50) NULL,
    day NVARCHAR(50) NULL,
    ke NVARCHAR(50) NULL,
    ngan NVARCHAR(50) NULL,
    mota NVARCHAR(255) NULL
);
GO

CREATE TABLE dau_sach
(
    madausach VARCHAR(20) NOT NULL CONSTRAINT PK_dau_sach PRIMARY KEY,
    tendausach NVARCHAR(255) NOT NULL,
    theloai NVARCHAR(100) NULL,
    tacgia NVARCHAR(150) NULL,
    namxuatban INT NULL,
    manhaxuatban VARCHAR(20) NULL,
    soluong INT NOT NULL CONSTRAINT DF_ds_soluong DEFAULT 0,
    chuoitimkiem NVARCHAR(500) NULL,
    soluonghienco INT NOT NULL CONSTRAINT DF_ds_hienco DEFAULT 0,
    soluongdangmuon INT NOT NULL CONSTRAINT DF_ds_dangmuon DEFAULT 0,
    soluonghongmat INT NOT NULL CONSTRAINT DF_ds_hongmat DEFAULT 0,
    lanmuongannhat DATE NULL,
    CONSTRAINT FK_dau_sach_nxb FOREIGN KEY (manhaxuatban) REFERENCES nha_xuat_ban(manhaxuatban)
);
GO

CREATE TABLE sach
(
    masach VARCHAR(20) NOT NULL CONSTRAINT PK_sach PRIMARY KEY,
    madausach VARCHAR(20) NULL,
    mavitri VARCHAR(20) NULL,
    tensach NVARCHAR(255) NOT NULL,
    theloai NVARCHAR(100) NULL,
    tacgia NVARCHAR(150) NULL,
    nhaxuatban NVARCHAR(150) NULL,
    namxuatban INT NULL,
    tinhtrang NVARCHAR(50) NOT NULL CONSTRAINT DF_sach_tinhtrang DEFAULT N'Có thể mượn',
    trangthai NVARCHAR(50) NOT NULL CONSTRAINT DF_sach_trangthai DEFAULT N'Trong kho',
    manhan VARCHAR(50) NULL,
    sodangkycabiet VARCHAR(50) NULL,
    mavach VARCHAR(50) NULL,
    ngaynhap DATE NULL,
    ngaycapnhattrangthai DATE NULL,
    sophieumuonhientai VARCHAR(20) NULL,
    solanmuon INT NOT NULL CONSTRAINT DF_sach_solan DEFAULT 0,
    ngaymuongannhat DATE NULL,
    CONSTRAINT FK_sach_dau_sach FOREIGN KEY (madausach) REFERENCES dau_sach(madausach),
    CONSTRAINT FK_sach_vitri FOREIGN KEY (mavitri) REFERENCES vi_tri_luu_tru(mavitri)
);
GO

CREATE TABLE phieu_muon
(
    sophieumuon VARCHAR(20) NOT NULL CONSTRAINT PK_phieu_muon PRIMARY KEY,
    masinhvien VARCHAR(20) NOT NULL,
    ngaymuon DATE NOT NULL,
    hantra DATE NOT NULL,
    nguoilapphieu NVARCHAR(100) NULL,
    manguoidung VARCHAR(20) NULL,
    tennguoidung NVARCHAR(100) NULL,
    trangthaiphieu NVARCHAR(50) NOT NULL CONSTRAINT DF_pm_trangthai DEFAULT N'Đang mượn',
    songayquahan INT NOT NULL CONSTRAINT DF_pm_songay DEFAULT 0,
    ngaytradutinh DATE NULL,
    ngayhoantat DATE NULL,
    tongsoluong INT NOT NULL CONSTRAINT DF_pm_tongsl DEFAULT 0,
    CONSTRAINT FK_pm_sv FOREIGN KEY (masinhvien) REFERENCES sinh_vien(masinhvien)
);
GO

CREATE TABLE ct_phieu_muon
(
    sophieumuon VARCHAR(20) NOT NULL,
    masach VARCHAR(20) NOT NULL,
    ngaytra DATE NULL,
    ghichu NVARCHAR(255) NULL,
    kytra NVARCHAR(100) NULL,
    tongsoluong INT NOT NULL CONSTRAINT DF_ctpm_tongsl DEFAULT 1,
    soluong INT NOT NULL CONSTRAINT DF_ctpm_sl DEFAULT 1,
    masinhvien VARCHAR(20) NULL,
    madausach VARCHAR(20) NULL,
    theloai NVARCHAR(100) NULL,
    CONSTRAINT PK_ct_phieu_muon PRIMARY KEY (sophieumuon, masach),
    CONSTRAINT FK_ctpm_pm FOREIGN KEY (sophieumuon) REFERENCES phieu_muon(sophieumuon),
    CONSTRAINT FK_ctpm_sach FOREIGN KEY (masach) REFERENCES sach(masach)
);
GO

CREATE TABLE phieu_phat_qua_han
(
    sophieuphatquahan VARCHAR(30) NOT NULL CONSTRAINT PK_ppqh PRIMARY KEY,
    ngaylap DATE NOT NULL,
    sophieumuon VARCHAR(20) NOT NULL,
    masinhvien VARCHAR(20) NOT NULL,
    manguoidung VARCHAR(20) NULL,
    tennguoidung NVARCHAR(100) NULL,
    tongphat DECIMAL(18,2) NOT NULL CONSTRAINT DF_ppqh_tong DEFAULT 0,
    trangthaithanhtoan NVARCHAR(50) NOT NULL CONSTRAINT DF_ppqh_tt DEFAULT N'Chưa thanh toán',
    ngaythanhtoan DATE NULL,
    nguoithutien VARCHAR(20) NULL,
    sotienconlai DECIMAL(18,2) NOT NULL CONSTRAINT DF_ppqh_conlai DEFAULT 0,
    CONSTRAINT FK_ppqh_pm FOREIGN KEY (sophieumuon) REFERENCES phieu_muon(sophieumuon),
    CONSTRAINT FK_ppqh_sv FOREIGN KEY (masinhvien) REFERENCES sinh_vien(masinhvien)
);
GO

CREATE TABLE ct_phieu_phat_qua_han
(
    sophieuphatquahan VARCHAR(30) NOT NULL,
    masach VARCHAR(20) NOT NULL,
    songayquahan INT NOT NULL,
    phiphat DECIMAL(18,2) NOT NULL,
    ngaymuon DATE NULL,
    hantra DATE NULL,
    ngaytra DATE NULL,
    tongphat DECIMAL(18,2) NOT NULL CONSTRAINT DF_ctppqh_tong DEFAULT 0,
    CONSTRAINT PK_ct_ppqh PRIMARY KEY (sophieuphatquahan, masach),
    CONSTRAINT FK_ctppqh_ppqh FOREIGN KEY (sophieuphatquahan) REFERENCES phieu_phat_qua_han(sophieuphatquahan),
    CONSTRAINT FK_ctppqh_sach FOREIGN KEY (masach) REFERENCES sach(masach)
);
GO

CREATE TABLE phieu_phat_hong_mat
(
    sophieuphathongmat VARCHAR(30) NOT NULL CONSTRAINT PK_pphm PRIMARY KEY,
    ngaylap DATE NOT NULL,
    sophieumuon VARCHAR(20) NOT NULL,
    masinhvien VARCHAR(20) NOT NULL,
    manguoidung VARCHAR(20) NULL,
    tennguoidung NVARCHAR(100) NULL,
    tongphat DECIMAL(18,2) NOT NULL CONSTRAINT DF_pphm_tong DEFAULT 0,
    trangthaithanhtoan NVARCHAR(50) NOT NULL CONSTRAINT DF_pphm_tt DEFAULT N'Chưa thanh toán',
    ngaythanhtoan DATE NULL,
    nguoithutien VARCHAR(20) NULL,
    sotienconlai DECIMAL(18,2) NOT NULL CONSTRAINT DF_pphm_conlai DEFAULT 0,
    CONSTRAINT FK_pphm_pm FOREIGN KEY (sophieumuon) REFERENCES phieu_muon(sophieumuon),
    CONSTRAINT FK_pphm_sv FOREIGN KEY (masinhvien) REFERENCES sinh_vien(masinhvien)
);
GO

CREATE TABLE ct_phieu_phat_hong_mat
(
    sophieuphathongmat VARCHAR(30) NOT NULL,
    masach VARCHAR(20) NOT NULL,
    tinhtrang NVARCHAR(50) NULL,
    mucdo NVARCHAR(255) NULL,
    phiphat DECIMAL(18,2) NOT NULL,
    tongphat DECIMAL(18,2) NOT NULL CONSTRAINT DF_ctpphm_tong DEFAULT 0,
    CONSTRAINT PK_ct_pphm PRIMARY KEY (sophieuphathongmat, masach),
    CONSTRAINT FK_ctpphm_pphm FOREIGN KEY (sophieuphathongmat) REFERENCES phieu_phat_hong_mat(sophieuphathongmat),
    CONSTRAINT FK_ctpphm_sach FOREIGN KEY (masach) REFERENCES sach(masach)
);
GO

CREATE TABLE nguoi_dung
(
    manguoidung VARCHAR(20) NOT NULL CONSTRAINT PK_nguoi_dung PRIMARY KEY,
    tennguoidung NVARCHAR(100) NULL,
    tendangnhap NVARCHAR(50) NOT NULL UNIQUE,
    matkhau VARCHAR(255) NOT NULL
);
GO


ALTER TABLE phieu_muon
ADD CONSTRAINT FK_pm_nguoi_dung FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung);
GO

ALTER TABLE phieu_phat_qua_han
ADD CONSTRAINT FK_ppqh_nguoi_dung FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung);
GO

ALTER TABLE phieu_phat_hong_mat
ADD CONSTRAINT FK_pphm_nguoi_dung FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung);
GO

CREATE TABLE quyen
(
    maquyen VARCHAR(20) NOT NULL CONSTRAINT PK_quyen PRIMARY KEY,
    tenquyen NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE nguoi_dung_quyen
(
    manguoidung VARCHAR(20) NOT NULL,
    maquyen VARCHAR(20) NOT NULL,
    CONSTRAINT PK_nguoi_dung_quyen PRIMARY KEY (manguoidung, maquyen),
    CONSTRAINT FK_ndq_nd FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung),
    CONSTRAINT FK_ndq_quyen FOREIGN KEY (maquyen) REFERENCES quyen(maquyen)
);
GO

CREATE TABLE nhom_nguoi_dung
(
    manhom VARCHAR(20) NOT NULL CONSTRAINT PK_nhom_nguoi_dung PRIMARY KEY,
    tennhom NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE nhom_quyen
(
    manhom VARCHAR(20) NOT NULL,
    maquyen VARCHAR(20) NOT NULL,
    CONSTRAINT PK_nhom_quyen PRIMARY KEY (manhom, maquyen),
    CONSTRAINT FK_nq_nhom FOREIGN KEY (manhom) REFERENCES nhom_nguoi_dung(manhom),
    CONSTRAINT FK_nq_quyen FOREIGN KEY (maquyen) REFERENCES quyen(maquyen)
);
GO

CREATE TABLE nhom_nguoi_dung_ct
(
    manhom VARCHAR(20) NOT NULL,
    manguoidung VARCHAR(20) NOT NULL,
    CONSTRAINT PK_nhom_nguoi_dung_ct PRIMARY KEY (manhom, manguoidung),
    CONSTRAINT FK_nndct_nhom FOREIGN KEY (manhom) REFERENCES nhom_nguoi_dung(manhom),
    CONSTRAINT FK_nndct_nd FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung)
);
GO

CREATE TABLE lich_su_dang_nhap
(
    maphien VARCHAR(30) NOT NULL CONSTRAINT PK_lich_su_dang_nhap PRIMARY KEY,
    thoidiemdangnhap DATETIME2 NOT NULL,
    thoidiemdangxuat DATETIME2 NULL,
    manguoidung VARCHAR(20) NOT NULL,
    CONSTRAINT FK_lsdn_nd FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung)
);
GO

CREATE TABLE nhat_ky_thay_doi
(
    manhatky VARCHAR(30) NOT NULL CONSTRAINT PK_nhat_ky_thay_doi PRIMARY KEY,
    maphien VARCHAR(30) NOT NULL,
    thoigianthaydoi DATETIME2 NOT NULL CONSTRAINT DF_nktd_thoigian DEFAULT SYSDATETIME(),
    noidungthaydoi NVARCHAR(500) NULL,
    thongtincu NVARCHAR(MAX) NULL,
    thongtinmoi NVARCHAR(MAX) NULL,
    CONSTRAINT FK_nktd_lsdn FOREIGN KEY (maphien) REFERENCES lich_su_dang_nhap(maphien)
);
GO

CREATE TABLE phieu_kiem_ke
(
    makiemke VARCHAR(20) NOT NULL CONSTRAINT PK_phieu_kiem_ke PRIMARY KEY,
    ngaykiemke DATE NOT NULL,
    nguoilapphieu NVARCHAR(100) NULL,
    tennguoidung NVARCHAR(100) NULL,
    manguoidung VARCHAR(20) NULL,
    CONSTRAINT FK_pkk_nd FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung)
);
GO

CREATE TABLE ct_phieu_kiem_ke
(
    makiemke VARCHAR(20) NOT NULL,
    masach VARCHAR(20) NOT NULL,
    ghichu NVARCHAR(255) NULL,
    CONSTRAINT PK_ct_phieu_kiem_ke PRIMARY KEY (makiemke, masach),
    CONSTRAINT FK_ctkk_pkk FOREIGN KEY (makiemke) REFERENCES phieu_kiem_ke(makiemke),
    CONSTRAINT FK_ctkk_sach FOREIGN KEY (masach) REFERENCES sach(masach)
);
GO

CREATE TABLE phieu_huy_sach
(
    mahuy VARCHAR(20) NOT NULL CONSTRAINT PK_phieu_huy_sach PRIMARY KEY,
    ngaylapphieu DATE NOT NULL,
    nguoilapphieu NVARCHAR(100) NULL,
    manguoidung VARCHAR(20) NULL,
    tennguoidung NVARCHAR(100) NULL,
    CONSTRAINT FK_phs_nd FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung)
);
GO

CREATE TABLE ct_phieu_huy_sach
(
    mahuy VARCHAR(20) NOT NULL,
    masach VARCHAR(20) NOT NULL,
    lydohuy NVARCHAR(255) NULL,
    ghichu NVARCHAR(255) NULL,
    CONSTRAINT PK_ct_phieu_huy_sach PRIMARY KEY (mahuy, masach),
    CONSTRAINT FK_cths_phs FOREIGN KEY (mahuy) REFERENCES phieu_huy_sach(mahuy),
    CONSTRAINT FK_cths_sach FOREIGN KEY (masach) REFERENCES sach(masach)
);
GO

CREATE TABLE phieu_thanh_ly
(
    mathanhly VARCHAR(20) NOT NULL CONSTRAINT PK_phieu_thanh_ly PRIMARY KEY,
    ngaythanhly DATE NOT NULL,
    nguoilapphieu NVARCHAR(100) NULL,
    manguoidung VARCHAR(20) NULL,
    tennguoidung NVARCHAR(100) NULL,
    CONSTRAINT FK_ptl_nd FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung)
);
GO

CREATE TABLE ct_phieu_thanh_ly
(
    mathanhly VARCHAR(20) NOT NULL,
    masach VARCHAR(20) NOT NULL,
    lydothanhly NVARCHAR(255) NULL,
    ghichu NVARCHAR(255) NULL,
    CONSTRAINT PK_ct_phieu_thanh_ly PRIMARY KEY (mathanhly, masach),
    CONSTRAINT FK_cttl_ptl FOREIGN KEY (mathanhly) REFERENCES phieu_thanh_ly(mathanhly),
    CONSTRAINT FK_cttl_sach FOREIGN KEY (masach) REFERENCES sach(masach)
);
GO

CREATE TABLE bien_ban_nhan_ban_giao
(
    sobienban VARCHAR(20) NOT NULL CONSTRAINT PK_bien_ban_nhan_ban_giao PRIMARY KEY,
    ngaylap DATE NOT NULL,
    daidienbenbangiao NVARCHAR(100) NULL,
    chucvubenbangiao NVARCHAR(100) NULL,
    daidienbennhan NVARCHAR(100) NULL,
    chucvubennhan NVARCHAR(100) NULL,
    tongsodausach INT NOT NULL CONSTRAINT DF_bbbg_tongsodausach DEFAULT 0,
    tongsoluongsach INT NOT NULL CONSTRAINT DF_bbbg_tongsoluongsach DEFAULT 0,
    manhacungcap VARCHAR(20) NULL,
    tennguoidung NVARCHAR(100) NULL,
    manguoidung VARCHAR(20) NULL,
    CONSTRAINT FK_bbbg_ncc FOREIGN KEY (manhacungcap) REFERENCES nha_cung_cap(manhacungcap),
    CONSTRAINT FK_bbbg_nd FOREIGN KEY (manguoidung) REFERENCES nguoi_dung(manguoidung)
);
GO

CREATE TABLE ct_bien_ban_nhan_ban_giao
(
    sobienban VARCHAR(20) NOT NULL,
    madausach VARCHAR(20) NOT NULL,
    soluongsachmoidausach INT NOT NULL CONSTRAINT DF_ctbbbg_sl DEFAULT 0,
    ghichu NVARCHAR(255) NULL,
    CONSTRAINT PK_ct_bien_ban_nhan_ban_giao PRIMARY KEY (sobienban, madausach),
    CONSTRAINT FK_ctbbbg_bbbg FOREIGN KEY (sobienban) REFERENCES bien_ban_nhan_ban_giao(sobienban),
    CONSTRAINT FK_ctbbbg_ds FOREIGN KEY (madausach) REFERENCES dau_sach(madausach)
);
GO

CREATE INDEX IX_sach_tinhtrang ON sach(tinhtrang, trangthai);
CREATE INDEX IX_phieu_muon_sv ON phieu_muon(masinhvien, ngaymuon);
CREATE INDEX IX_ppqh_ngaylap ON phieu_phat_qua_han(ngaylap);
CREATE INDEX IX_pphm_ngaylap ON phieu_phat_hong_mat(ngaylap);
CREATE INDEX IX_ct_phieu_muon_masach ON ct_phieu_muon(masach);
CREATE INDEX IX_lsdn_manguoidung ON lich_su_dang_nhap(manguoidung, thoidiemdangnhap);
CREATE INDEX IX_bbbg_ncc ON bien_ban_nhan_ban_giao(manhacungcap, ngaylap);
GO

/* =========================
   1. NGƯỜI DÙNG
   ========================= */
INSERT INTO nguoi_dung (manguoidung, tennguoidung, tendangnhap, matkhau) VALUES
('ND001', N'Nguyễn Văn Quản', 'admin', '123456'),
('ND002', N'Trần Thị Thủ', 'thuthu01', '123456'),
('ND003', N'Lê Văn Kho', 'kho01', '123456'),
('ND004', N'Phạm Minh Kế', 'ketoan01', '123456'),
('ND005', N'Hoàng Thị Lan', 'thuthu02', '123456'),
('ND006', N'Vũ Đức Nam', 'kho02', '123456'),
('ND007', N'Đỗ Thanh Hà', 'baocao01', '123456'),
('ND008', N'Bùi Quang Huy', 'quanly01', '123456'),
('ND009', N'Ngô Thu Trang', 'kiemke01', '123456'),
('ND010', N'Phan Anh Tuấn', 'user01', '123456');
GO

/* =========================
   2. QUYỀN
   ========================= */
INSERT INTO quyen (maquyen, tenquyen) VALUES
('Q001', N'Quản trị hệ thống'),
('Q002', N'Quản lý danh mục sách'),
('Q003', N'Lập phiếu mượn sách'),
('Q004', N'Cập nhật trả sách'),
('Q005', N'Lập phiếu phạt'),
('Q006', N'Kiểm kê sách'),
('Q007', N'Thanh lý sách'),
('Q008', N'Hủy sách'),
('Q009', N'Thống kê báo cáo'),
('Q010', N'Quản lý người dùng');
GO

/* =========================
   3. NHÓM NGƯỜI DÙNG
   ========================= */
INSERT INTO nhom_nguoi_dung (manhom, tennhom) VALUES
('N001', N'Quản trị viên'),
('N002', N'Thủ thư'),
('N003', N'Nhân viên kho'),
('N004', N'Kế toán thư viện'),
('N005', N'Quản lý thư viện'),
('N006', N'Nhân viên kiểm kê'),
('N007', N'Nhân viên báo cáo'),
('N008', N'Nhân viên nhập sách'),
('N009', N'Nhân viên thanh lý'),
('N010', N'Tài khoản xem dữ liệu');
GO

/* =========================
   4. NGƯỜI DÙNG - QUYỀN
   ========================= */
INSERT INTO nguoi_dung_quyen (manguoidung, maquyen) VALUES
('ND001', 'Q001'),
('ND001', 'Q010'),
('ND002', 'Q003'),
('ND002', 'Q004'),
('ND003', 'Q006'),
('ND004', 'Q005'),
('ND005', 'Q003'),
('ND006', 'Q002'),
('ND007', 'Q009'),
('ND008', 'Q007');
GO

/* =========================
   5. NHÓM - QUYỀN
   ========================= */
INSERT INTO nhom_quyen (manhom, maquyen) VALUES
('N001', 'Q001'),
('N001', 'Q010'),
('N002', 'Q003'),
('N002', 'Q004'),
('N003', 'Q006'),
('N004', 'Q005'),
('N005', 'Q009'),
('N006', 'Q006'),
('N008', 'Q002'),
('N009', 'Q007');
GO

/* =========================
   6. NHÓM - NGƯỜI DÙNG
   ========================= */
INSERT INTO nhom_nguoi_dung_ct (manhom, manguoidung) VALUES
('N001', 'ND001'),
('N002', 'ND002'),
('N003', 'ND003'),
('N004', 'ND004'),
('N002', 'ND005'),
('N008', 'ND006'),
('N007', 'ND007'),
('N005', 'ND008'),
('N006', 'ND009'),
('N010', 'ND010');
GO

/* =========================
   7. SINH VIÊN
   ========================= */
INSERT INTO sinh_vien (masinhvien, hoten, gioitinh, lop, khoa, sodienthoai) VALUES
('SV001', N'Nguyễn Văn An', N'Nam', 'CNTT1', 'K58', '0911111111'),
('SV002', N'Trần Thị Bình', N'Nữ', 'CNTT2', 'K58', '0922222222'),
('SV003', N'Lê Minh Cường', N'Nam', 'ATTT1', 'K59', '0933333333'),
('SV004', N'Phạm Thu Dung', N'Nữ', 'HTTT1', 'K59', '0944444444'),
('SV005', N'Hoàng Văn Đức', N'Nam', 'CNTT3', 'K60', '0955555555'),
('SV006', N'Vũ Thị Hạnh', N'Nữ', 'KHMT1', 'K60', '0966666666'),
('SV007', N'Đỗ Quang Huy', N'Nam', 'CNTT4', 'K61', '0977777777'),
('SV008', N'Bùi Lan Anh', N'Nữ', 'ATTT2', 'K61', '0988888888'),
('SV009', N'Ngô Minh Khang', N'Nam', 'HTTT2', 'K62', '0999999999'),
('SV010', N'Phan Thảo Linh', N'Nữ', 'KHMT2', 'K62', '0900000000');
GO

/* =========================
   8. NHÀ CUNG CẤP
   ========================= */
INSERT INTO nha_cung_cap (manhacungcap, tennhacungcap, diachi, sodienthoai) VALUES
('NCC001', N'Công ty Sách Giáo dục Hà Nội', N'Hà Nội', '0241111111'),
('NCC002', N'Công ty Văn hóa Phương Nam', N'TP. Hồ Chí Minh', '0282222222'),
('NCC003', N'Nhà sách Fahasa', N'TP. Hồ Chí Minh', '0283333333'),
('NCC004', N'Công ty Alpha Books', N'Hà Nội', '0244444444'),
('NCC005', N'Công ty Thái Hà Books', N'Hà Nội', '0245555555'),
('NCC006', N'Công ty Trí Việt', N'Hà Nội', '0246666666'),
('NCC007', N'Công ty Kim Đồng', N'Hà Nội', '0247777777'),
('NCC008', N'Công ty Tổng hợp Sách Việt', N'Đà Nẵng', '0236888888'),
('NCC009', N'Công ty Sách Kỹ thuật', N'Hà Nội', '0249999999'),
('NCC010', N'Công ty Minh Long Book', N'Hà Nội', '0240000000');
GO

/* =========================
   9. NHÀ XUẤT BẢN
   ========================= */
INSERT INTO nha_xuat_ban (manhaxuatban, tennhaxuatban, diachi, sodienthoai) VALUES
('NXB001', N'NXB Giáo dục Việt Nam', N'Hà Nội', '0241000001'),
('NXB002', N'NXB Bách Khoa Hà Nội', N'Hà Nội', '0241000002'),
('NXB003', N'NXB Khoa học và Kỹ thuật', N'Hà Nội', '0241000003'),
('NXB004', N'NXB Thông tin và Truyền thông', N'Hà Nội', '0241000004'),
('NXB005', N'NXB Quân đội nhân dân', N'Hà Nội', '0241000005'),
('NXB006', N'NXB Đại học Quốc gia Hà Nội', N'Hà Nội', '0241000006'),
('NXB007', N'NXB Trẻ', N'TP. Hồ Chí Minh', '0281000007'),
('NXB008', N'NXB Kim Đồng', N'Hà Nội', '0241000008'),
('NXB009', N'NXB Lao động', N'Hà Nội', '0241000009'),
('NXB010', N'NXB Tổng hợp TP.HCM', N'TP. Hồ Chí Minh', '0281000010');
GO

/* =========================
   10. VỊ TRÍ LƯU TRỮ
   ========================= */
INSERT INTO vi_tri_luu_tru (mavitri, khu, day, ke, ngan, mota) VALUES
('VT001', N'Khu A', N'Dãy 1', N'Kệ 1', N'Ngăn 1', N'Sách cơ sở dữ liệu'),
('VT002', N'Khu A', N'Dãy 1', N'Kệ 1', N'Ngăn 2', N'Sách lập trình'),
('VT003', N'Khu A', N'Dãy 2', N'Kệ 1', N'Ngăn 1', N'Sách mạng máy tính'),
('VT004', N'Khu B', N'Dãy 1', N'Kệ 2', N'Ngăn 1', N'Sách an toàn thông tin'),
('VT005', N'Khu B', N'Dãy 1', N'Kệ 2', N'Ngăn 2', N'Sách trí tuệ nhân tạo'),
('VT006', N'Khu C', N'Dãy 3', N'Kệ 1', N'Ngăn 1', N'Sách toán học'),
('VT007', N'Khu C', N'Dãy 3', N'Kệ 1', N'Ngăn 2', N'Sách vật lý'),
('VT008', N'Khu D', N'Dãy 1', N'Kệ 3', N'Ngăn 1', N'Sách quân sự'),
('VT009', N'Khu D', N'Dãy 2', N'Kệ 3', N'Ngăn 2', N'Sách ngoại ngữ'),
('VT010', N'Khu E', N'Dãy 1', N'Kệ 4', N'Ngăn 1', N'Sách tham khảo');
GO

/* =========================
   11. ĐẦU SÁCH
   ========================= */
INSERT INTO dau_sach 
(madausach, tendausach, theloai, tacgia, manhaxuatban, namxuatban, soluong) VALUES
('DS001', N'Cơ sở dữ liệu nâng cao', N'Công nghệ thông tin', N'Nguyễn Mậu Uyên', 'NXB002', 2022, 3),
('DS002', N'Lập trình C# căn bản', N'Lập trình', N'Lê Văn Nam', 'NXB003', 2021, 4),
('DS003', N'Mạng máy tính', N'Mạng máy tính', N'Nguyễn Hồng Sơn', 'NXB004', 2020, 5),
('DS004', N'An toàn thông tin', N'An toàn thông tin', N'Trần Quốc Bảo', 'NXB004', 2023, 2),
('DS005', N'Trí tuệ nhân tạo nhập môn', N'Trí tuệ nhân tạo', N'Phạm Minh Hoàng', 'NXB006', 2024, 3),
('DS006', N'Toán rời rạc', N'Toán học', N'Đặng Văn Đức', 'NXB001', 2019, 6),
('DS007', N'Vật lý đại cương', N'Vật lý', N'Hoàng Xuân Bình', 'NXB001', 2018, 4),
('DS008', N'Lịch sử quân sự Việt Nam', N'Quân sự', N'Vũ Quang Minh', 'NXB005', 2020, 3),
('DS009', N'Tiếng Anh chuyên ngành CNTT', N'Ngoại ngữ', N'Ngô Thị Mai', 'NXB010', 2021, 5),
('DS010', N'Phân tích thiết kế hệ thống', N'Công nghệ thông tin', N'Đỗ Thị Mai Hường', 'NXB002', 2023, 4);
GO

/* =========================
   12. SÁCH
   ========================= */
INSERT INTO sach 
(masach, madausach, mavitri, tensach, theloai, tacgia, nhaxuatban, namxuatban, tinhtrang) VALUES
('S001', 'DS001', 'VT001', N'Cơ sở dữ liệu nâng cao', N'Công nghệ thông tin', N'Nguyễn Mậu Uyên', N'NXB Bách Khoa Hà Nội', 2022, N'Đang mượn'),
('S002', 'DS002', 'VT002', N'Lập trình C# căn bản', N'Lập trình', N'Lê Văn Nam', N'NXB Khoa học và Kỹ thuật', 2021, N'Đang mượn'),
('S003', 'DS003', 'VT003', N'Mạng máy tính', N'Mạng máy tính', N'Nguyễn Hồng Sơn', N'NXB Thông tin và Truyền thông', 2020, N'Hỏng'),
('S004', 'DS004', 'VT004', N'An toàn thông tin', N'An toàn thông tin', N'Trần Quốc Bảo', N'NXB Thông tin và Truyền thông', 2023, N'Mất'),
('S005', 'DS005', 'VT005', N'Trí tuệ nhân tạo nhập môn', N'Trí tuệ nhân tạo', N'Phạm Minh Hoàng', N'NXB Đại học Quốc gia Hà Nội', 2024, N'Có thể mượn'),
('S006', 'DS006', 'VT006', N'Toán rời rạc', N'Toán học', N'Đặng Văn Đức', N'NXB Giáo dục Việt Nam', 2019, N'Có thể mượn'),
('S007', 'DS007', 'VT007', N'Vật lý đại cương', N'Vật lý', N'Hoàng Xuân Bình', N'NXB Giáo dục Việt Nam', 2018, N'Có thể mượn'),
('S008', 'DS008', 'VT008', N'Lịch sử quân sự Việt Nam', N'Quân sự', N'Vũ Quang Minh', N'NXB Quân đội nhân dân', 2020, N'Hỏng'),
('S009', 'DS009', 'VT009', N'Tiếng Anh chuyên ngành CNTT', N'Ngoại ngữ', N'Ngô Thị Mai', N'NXB Tổng hợp TP.HCM', 2021, N'Đã thanh lý'),
('S010', 'DS010', 'VT010', N'Phân tích thiết kế hệ thống', N'Công nghệ thông tin', N'Đỗ Thị Mai Hường', N'NXB Bách Khoa Hà Nội', 2023, N'Đã hủy');
GO

/* =========================
   13. PHIẾU MƯỢN
   ========================= */
INSERT INTO phieu_muon 
(sophieumuon, ngaymuon, hantra, nguoilapphieu, masinhvien, manguoidung, tennguoidung) VALUES
('PM001', '2026-05-01', '2026-05-15', N'Trần Thị Thủ', 'SV001', 'ND002', N'Trần Thị Thủ'),
('PM002', '2026-05-02', '2026-05-16', N'Trần Thị Thủ', 'SV002', 'ND002', N'Trần Thị Thủ'),
('PM003', '2026-05-03', '2026-05-17', N'Hoàng Thị Lan', 'SV003', 'ND005', N'Hoàng Thị Lan'),
('PM004', '2026-05-04', '2026-05-18', N'Hoàng Thị Lan', 'SV004', 'ND005', N'Hoàng Thị Lan'),
('PM005', '2026-05-05', '2026-05-19', N'Trần Thị Thủ', 'SV005', 'ND002', N'Trần Thị Thủ'),
('PM006', '2026-05-06', '2026-05-20', N'Hoàng Thị Lan', 'SV006', 'ND005', N'Hoàng Thị Lan'),
('PM007', '2026-05-07', '2026-05-21', N'Trần Thị Thủ', 'SV007', 'ND002', N'Trần Thị Thủ'),
('PM008', '2026-05-08', '2026-05-22', N'Hoàng Thị Lan', 'SV008', 'ND005', N'Hoàng Thị Lan'),
('PM009', '2026-05-09', '2026-05-23', N'Trần Thị Thủ', 'SV009', 'ND002', N'Trần Thị Thủ'),
('PM010', '2026-05-10', '2026-05-24', N'Hoàng Thị Lan', 'SV010', 'ND005', N'Hoàng Thị Lan');
GO

/* =========================
   14. CHI TIẾT PHIẾU MƯỢN
   ========================= */
INSERT INTO ct_phieu_muon 
(sophieumuon, masach, soluong, ngaytra, ghichu, tongsoluong) VALUES
('PM001', 'S001', 1, NULL, N'Đang mượn', 1),
('PM002', 'S002', 1, NULL, N'Đang mượn', 1),
('PM003', 'S003', 1, '2026-05-25', N'Trả sách bị hỏng', 1),
('PM004', 'S004', 1, '2026-05-26', N'Báo mất sách', 1),
('PM005', 'S005', 1, '2026-05-18', N'Trả đúng hạn', 1),
('PM006', 'S006', 1, '2026-05-29', N'Trả quá hạn', 1),
('PM007', 'S007', 1, '2026-05-21', N'Trả đúng hạn', 1),
('PM008', 'S008', 1, '2026-05-30', N'Trả sách bị hỏng', 1),
('PM009', 'S009', 1, '2026-05-28', N'Trả quá hạn', 1),
('PM010', 'S010', 1, '2026-06-01', N'Trả quá hạn', 1);
GO

/* =========================
   15. PHIẾU PHẠT QUÁ HẠN
   ========================= */
INSERT INTO phieu_phat_qua_han 
(sophieuphatquahan, ngaylap, masinhvien, sophieumuon, manguoidung, tennguoidung) VALUES
('PQH001', '2026-05-20', 'SV001', 'PM001', 'ND004', N'Phạm Minh Kế'),
('PQH002', '2026-05-21', 'SV002', 'PM002', 'ND004', N'Phạm Minh Kế'),
('PQH003', '2026-05-25', 'SV003', 'PM003', 'ND004', N'Phạm Minh Kế'),
('PQH004', '2026-05-26', 'SV004', 'PM004', 'ND004', N'Phạm Minh Kế'),
('PQH005', '2026-05-20', 'SV005', 'PM005', 'ND004', N'Phạm Minh Kế'),
('PQH006', '2026-05-29', 'SV006', 'PM006', 'ND004', N'Phạm Minh Kế'),
('PQH007', '2026-05-22', 'SV007', 'PM007', 'ND004', N'Phạm Minh Kế'),
('PQH008', '2026-05-30', 'SV008', 'PM008', 'ND004', N'Phạm Minh Kế'),
('PQH009', '2026-05-28', 'SV009', 'PM009', 'ND004', N'Phạm Minh Kế'),
('PQH010', '2026-06-01', 'SV010', 'PM010', 'ND004', N'Phạm Minh Kế');
GO

/* =========================
   16. CHI TIẾT PHIẾU PHẠT QUÁ HẠN
   ========================= */
INSERT INTO ct_phieu_phat_qua_han 
(sophieuphatquahan, masach, ngaymuon, hantra, ngaytra, songayquahan, phiphat, tongphat) VALUES
('PQH001', 'S001', '2026-05-01', '2026-05-15', '2026-05-20', 5, 10000, 10000),
('PQH002', 'S002', '2026-05-02', '2026-05-16', '2026-05-21', 5, 10000, 10000),
('PQH003', 'S003', '2026-05-03', '2026-05-17', '2026-05-25', 8, 16000, 16000),
('PQH004', 'S004', '2026-05-04', '2026-05-18', '2026-05-26', 8, 16000, 16000),
('PQH005', 'S005', '2026-05-05', '2026-05-19', '2026-05-20', 1, 2000, 2000),
('PQH006', 'S006', '2026-05-06', '2026-05-20', '2026-05-29', 9, 18000, 18000),
('PQH007', 'S007', '2026-05-07', '2026-05-21', '2026-05-22', 1, 2000, 2000),
('PQH008', 'S008', '2026-05-08', '2026-05-22', '2026-05-30', 8, 16000, 16000),
('PQH009', 'S009', '2026-05-09', '2026-05-23', '2026-05-28', 5, 10000, 10000),
('PQH010', 'S010', '2026-05-10', '2026-05-24', '2026-06-01', 8, 16000, 16000);
GO

/* =========================
   17. PHIẾU PHẠT HỎNG MẤT
   ========================= */
INSERT INTO phieu_phat_hong_mat 
(sophieuphathongmat, ngaylap, sophieumuon, masinhvien, manguoidung, tennguoidung) VALUES
('PHM001', '2026-05-25', 'PM003', 'SV003', 'ND004', N'Phạm Minh Kế'),
('PHM002', '2026-05-26', 'PM004', 'SV004', 'ND004', N'Phạm Minh Kế'),
('PHM003', '2026-05-30', 'PM008', 'SV008', 'ND004', N'Phạm Minh Kế'),
('PHM004', '2026-06-01', 'PM010', 'SV010', 'ND004', N'Phạm Minh Kế'),
('PHM005', '2026-05-20', 'PM001', 'SV001', 'ND004', N'Phạm Minh Kế'),
('PHM006', '2026-05-21', 'PM002', 'SV002', 'ND004', N'Phạm Minh Kế'),
('PHM007', '2026-05-22', 'PM005', 'SV005', 'ND004', N'Phạm Minh Kế'),
('PHM008', '2026-05-23', 'PM006', 'SV006', 'ND004', N'Phạm Minh Kế'),
('PHM009', '2026-05-24', 'PM007', 'SV007', 'ND004', N'Phạm Minh Kế'),
('PHM010', '2026-05-28', 'PM009', 'SV009', 'ND004', N'Phạm Minh Kế');
GO

/* =========================
   18. CHI TIẾT PHIẾU PHẠT HỎNG MẤT
   ========================= */
INSERT INTO ct_phieu_phat_hong_mat 
(sophieuphathongmat, masach, mucdo, phiphat, tongphat) VALUES
('PHM001', 'S003', N'Hỏng nhẹ', 50000, 50000),
('PHM002', 'S004', N'Mất sách', 200000, 200000),
('PHM003', 'S008', N'Hỏng nặng', 120000, 120000),
('PHM004', 'S010', N'Mất sách', 220000, 220000),
('PHM005', 'S001', N'Bìa rách', 30000, 30000),
('PHM006', 'S002', N'Ghi bẩn', 20000, 20000),
('PHM007', 'S005', N'Cong mép', 15000, 15000),
('PHM008', 'S006', N'Rách trang', 80000, 80000),
('PHM009', 'S007', N'Bong gáy', 60000, 60000),
('PHM010', 'S009', N'Hỏng nặng', 100000, 100000);
GO

/* =========================
   19. PHIẾU KIỂM KÊ
   ========================= */
INSERT INTO phieu_kiem_ke 
(makiemke, ngaykiemke, nguoilapphieu, tennguoidung, manguoidung) VALUES
('KK001', '2026-06-01', N'Ngô Thu Trang', N'Ngô Thu Trang', 'ND009'),
('KK002', '2026-06-02', N'Ngô Thu Trang', N'Ngô Thu Trang', 'ND009'),
('KK003', '2026-06-03', N'Lê Văn Kho', N'Lê Văn Kho', 'ND003'),
('KK004', '2026-06-04', N'Vũ Đức Nam', N'Vũ Đức Nam', 'ND006'),
('KK005', '2026-06-05', N'Ngô Thu Trang', N'Ngô Thu Trang', 'ND009'),
('KK006', '2026-06-06', N'Lê Văn Kho', N'Lê Văn Kho', 'ND003'),
('KK007', '2026-06-07', N'Vũ Đức Nam', N'Vũ Đức Nam', 'ND006'),
('KK008', '2026-06-08', N'Ngô Thu Trang', N'Ngô Thu Trang', 'ND009'),
('KK009', '2026-06-09', N'Lê Văn Kho', N'Lê Văn Kho', 'ND003'),
('KK010', '2026-06-10', N'Vũ Đức Nam', N'Vũ Đức Nam', 'ND006');
GO

/* =========================
   20. CHI TIẾT PHIẾU KIỂM KÊ
   ========================= */
INSERT INTO ct_phieu_kiem_ke (makiemke, masach, ghichu) VALUES
('KK001', 'S001', N'Sách đang được mượn'),
('KK002', 'S002', N'Sách đang được mượn'),
('KK003', 'S003', N'Sách hỏng cần phục chế'),
('KK004', 'S004', N'Sách bị mất'),
('KK005', 'S005', N'Sách trong kho'),
('KK006', 'S006', N'Sách trong kho'),
('KK007', 'S007', N'Sách trong kho'),
('KK008', 'S008', N'Sách hỏng'),
('KK009', 'S009', N'Sách đã thanh lý'),
('KK010', 'S010', N'Sách đã hủy');
GO

/* =========================
   21. PHIẾU THANH LÝ
   ========================= */
INSERT INTO phieu_thanh_ly 
(mathanhly, ngaythanhly, nguoilapphieu, manguoidung, tennguoidung) VALUES
('TL001', '2026-06-11', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL002', '2026-06-12', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL003', '2026-06-13', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL004', '2026-06-14', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL005', '2026-06-15', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL006', '2026-06-16', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL007', '2026-06-17', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL008', '2026-06-18', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL009', '2026-06-19', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy'),
('TL010', '2026-06-20', N'Bùi Quang Huy', 'ND008', N'Bùi Quang Huy');
GO

/* =========================
   22. CHI TIẾT PHIẾU THANH LÝ
   ========================= */
INSERT INTO ct_phieu_thanh_ly 
(mathanhly, masach, lydothanhly, ghichu) VALUES
('TL001', 'S009', N'Sách cũ, không còn nhu cầu sử dụng', N'Đã lập hồ sơ thanh lý'),
('TL002', 'S001', N'Sách rách nhiều trang', N'Chờ phê duyệt'),
('TL003', 'S002', N'Sách lỗi thời', N'Đưa vào danh sách thanh lý'),
('TL004', 'S003', N'Sách hỏng không phục chế được', N'Cần thay thế'),
('TL005', 'S004', N'Sách mất bìa và thiếu trang', N'Đã kiểm tra'),
('TL006', 'S005', N'Sách trùng quá nhiều bản', N'Thanh lý bớt'),
('TL007', 'S006', N'Sách cũ', N'Không còn khai thác'),
('TL008', 'S007', N'Sách hư gáy', N'Đề nghị thanh lý'),
('TL009', 'S008', N'Sách hỏng nặng', N'Không phục hồi'),
('TL010', 'S010', N'Sách không còn giá trị sử dụng', N'Đã xử lý');
GO

/* =========================
   23. PHIẾU HỦY SÁCH
   ========================= */
INSERT INTO phieu_huy_sach 
(mahuy, ngaylapphieu, nguoilapphieu, manguoidung, tennguoidung) VALUES
('H001', '2026-06-21', N'Lê Văn Kho', 'ND003', N'Lê Văn Kho'),
('H002', '2026-06-22', N'Lê Văn Kho', 'ND003', N'Lê Văn Kho'),
('H003', '2026-06-23', N'Vũ Đức Nam', 'ND006', N'Vũ Đức Nam'),
('H004', '2026-06-24', N'Vũ Đức Nam', 'ND006', N'Vũ Đức Nam'),
('H005', '2026-06-25', N'Lê Văn Kho', 'ND003', N'Lê Văn Kho'),
('H006', '2026-06-26', N'Lê Văn Kho', 'ND003', N'Lê Văn Kho'),
('H007', '2026-06-27', N'Vũ Đức Nam', 'ND006', N'Vũ Đức Nam'),
('H008', '2026-06-28', N'Vũ Đức Nam', 'ND006', N'Vũ Đức Nam'),
('H009', '2026-06-29', N'Lê Văn Kho', 'ND003', N'Lê Văn Kho'),
('H010', '2026-06-30', N'Vũ Đức Nam', 'ND006', N'Vũ Đức Nam');
GO

/* =========================
   24. CHI TIẾT PHIẾU HỦY SÁCH
   ========================= */
INSERT INTO ct_phieu_huy_sach 
(mahuy, masach, lydohuy, ghichu) VALUES
('H001', 'S010', N'Sách hỏng hoàn toàn', N'Đã hủy theo quy định'),
('H002', 'S001', N'Sách bị mốc', N'Không thể phục chế'),
('H003', 'S002', N'Sách rách nhiều trang', N'Đã kiểm tra'),
('H004', 'S003', N'Sách hỏng nặng', N'Không sử dụng được'),
('H005', 'S004', N'Sách bị mất nội dung chính', N'Đề nghị hủy'),
('H006', 'S005', N'Sách bị thấm nước', N'Đã lập biên bản'),
('H007', 'S006', N'Sách hư hỏng vật lý', N'Chờ xác nhận'),
('H008', 'S007', N'Sách lỗi in ấn nghiêm trọng', N'Không lưu hành'),
('H009', 'S008', N'Sách không còn nguyên vẹn', N'Hủy khỏi kho'),
('H010', 'S009', N'Sách đã thanh lý tồn đọng', N'Hủy sau thanh lý');
GO

/* =========================
   25. BIÊN BẢN NHẬN BÀN GIAO
   ========================= */
INSERT INTO bien_ban_nhan_ban_giao 
(sobienban, ngaylap, daidienbenbangiao, chucvubenbangiao, daidienbennhan, chucvubennhan,
 tongsodausach, tongsoluongsach, manhacungcap, tennguoidung, manguoidung) VALUES
('BB001', '2026-04-01', N'Nguyễn Văn Giao', N'Nhân viên giao hàng', N'Lê Văn Kho', N'Nhân viên kho', 1, 10, 'NCC001', N'Vũ Đức Nam', 'ND006'),
('BB002', '2026-04-02', N'Trần Minh Hải', N'Nhân viên giao hàng', N'Lê Văn Kho', N'Nhân viên kho', 1, 12, 'NCC002', N'Vũ Đức Nam', 'ND006'),
('BB003', '2026-04-03', N'Phạm Quốc Nam', N'Nhân viên giao hàng', N'Vũ Đức Nam', N'Nhân viên kho', 1, 8, 'NCC003', N'Vũ Đức Nam', 'ND006'),
('BB004', '2026-04-04', N'Hoàng Thanh Sơn', N'Nhân viên giao hàng', N'Lê Văn Kho', N'Nhân viên kho', 1, 15, 'NCC004', N'Vũ Đức Nam', 'ND006'),
('BB005', '2026-04-05', N'Đỗ Mạnh Cường', N'Nhân viên giao hàng', N'Vũ Đức Nam', N'Nhân viên kho', 1, 9, 'NCC005', N'Vũ Đức Nam', 'ND006'),
('BB006', '2026-04-06', N'Bùi Văn Hòa', N'Nhân viên giao hàng', N'Lê Văn Kho', N'Nhân viên kho', 1, 11, 'NCC006', N'Vũ Đức Nam', 'ND006'),
('BB007', '2026-04-07', N'Ngô Đức Anh', N'Nhân viên giao hàng', N'Vũ Đức Nam', N'Nhân viên kho', 1, 7, 'NCC007', N'Vũ Đức Nam', 'ND006'),
('BB008', '2026-04-08', N'Phan Văn Lâm', N'Nhân viên giao hàng', N'Lê Văn Kho', N'Nhân viên kho', 1, 14, 'NCC008', N'Vũ Đức Nam', 'ND006'),
('BB009', '2026-04-09', N'Võ Minh Tân', N'Nhân viên giao hàng', N'Vũ Đức Nam', N'Nhân viên kho', 1, 6, 'NCC009', N'Vũ Đức Nam', 'ND006'),
('BB010', '2026-04-10', N'Đặng Văn Phúc', N'Nhân viên giao hàng', N'Lê Văn Kho', N'Nhân viên kho', 1, 13, 'NCC010', N'Vũ Đức Nam', 'ND006');
GO

/* =========================
   26. CHI TIẾT BIÊN BẢN NHẬN BÀN GIAO
   ========================= */
INSERT INTO ct_bien_ban_nhan_ban_giao 
(sobienban, madausach, soluongsachmoidausach, ghichu) VALUES
('BB001', 'DS001', 10, N'Nhập bổ sung đầu sách cơ sở dữ liệu'),
('BB002', 'DS002', 12, N'Nhập sách lập trình'),
('BB003', 'DS003', 8, N'Nhập sách mạng máy tính'),
('BB004', 'DS004', 15, N'Nhập sách an toàn thông tin'),
('BB005', 'DS005', 9, N'Nhập sách trí tuệ nhân tạo'),
('BB006', 'DS006', 11, N'Nhập sách toán học'),
('BB007', 'DS007', 7, N'Nhập sách vật lý'),
('BB008', 'DS008', 14, N'Nhập sách quân sự'),
('BB009', 'DS009', 6, N'Nhập sách ngoại ngữ'),
('BB010', 'DS010', 13, N'Nhập sách phân tích thiết kế hệ thống');
GO

/* =========================
   27. LỊCH SỬ ĐĂNG NHẬP
   ========================= */
INSERT INTO lich_su_dang_nhap 
(maphien, thoidiemdangnhap, thoidiemdangxuat, manguoidung) VALUES
('P001', '2026-06-01T07:30:00', '2026-06-01T10:30:00', 'ND001'),
('P002', '2026-06-02T08:00:00', '2026-06-02T11:10:00', 'ND002'),
('P003', '2026-06-03T08:15:00', '2026-06-03T09:45:00', 'ND003'),
('P004', '2026-06-04T13:00:00', '2026-06-04T15:30:00', 'ND004'),
('P005', '2026-06-05T07:45:00', '2026-06-05T10:20:00', 'ND005'),
('P006', '2026-06-06T08:30:00', '2026-06-06T11:00:00', 'ND006'),
('P007', '2026-06-07T09:00:00', '2026-06-07T10:00:00', 'ND007'),
('P008', '2026-06-08T14:00:00', '2026-06-08T16:10:00', 'ND008'),
('P009', '2026-06-09T08:05:00', '2026-06-09T12:00:00', 'ND009'),
('P010', '2026-06-10T15:00:00', '2026-06-10T16:00:00', 'ND010');
GO

/* =========================
   28. NHẬT KÝ THAY ĐỔI
   ========================= */
INSERT INTO nhat_ky_thay_doi 
(manhatky, maphien, thoigianthaydoi, noidungthaydoi, thongtincu, thongtinmoi) VALUES
('NK001', 'P001', '2026-06-01T08:00:00', N'Thêm người dùng', NULL, N'ND010'),
('NK002', 'P002', '2026-06-02T08:30:00', N'Lập phiếu mượn', NULL, N'PM001'),
('NK003', 'P003', '2026-06-03T08:45:00', N'Cập nhật vị trí sách', N'VT001', N'VT003'),
('NK004', 'P004', '2026-06-04T13:30:00', N'Lập phiếu phạt quá hạn', NULL, N'PQH001'),
('NK005', 'P005', '2026-06-05T08:15:00', N'Cập nhật trả sách', N'Đang mượn', N'Có thể mượn'),
('NK006', 'P006', '2026-06-06T09:00:00', N'Lập biên bản nhận bàn giao', NULL, N'BB001'),
('NK007', 'P007', '2026-06-07T09:30:00', N'Xuất báo cáo thống kê', NULL, N'MB05'),
('NK008', 'P008', '2026-06-08T14:30:00', N'Sửa đầu sách', N'Số lượng 5', N'Số lượng 6'),
('NK009', 'P009', '2026-06-09T09:10:00', N'Lập phiếu kiểm kê', NULL, N'KK001'),
('NK010', 'P010', '2026-06-10T15:20:00', N'Đăng nhập xem dữ liệu', NULL, N'Xem danh mục sách');
GO


/* =========================================================
   29. ĐỒNG BỘ DỮ LIỆU PHÁT SINH CHO CÁC CỘT BỔ SUNG
   - Giữ nguyên dữ liệu test của bạn cùng lớp.
   - Chỉ cập nhật các cột vận hành/thống kê có trong bản SQL mở rộng.
   ========================================================= */
UPDATE s
SET trangthai = CASE 
        WHEN s.tinhtrang = N'Có thể mượn' THEN N'Trong kho'
        ELSE s.tinhtrang
    END,
    ngaycapnhattrangthai = CAST(GETDATE() AS DATE)
FROM sach s;
GO

UPDATE pm
SET tongsoluong = x.tongsoluong,
    trangthaiphieu = CASE WHEN x.sochuatra > 0 THEN N'Đang mượn' ELSE N'Đã trả' END,
    ngayhoantat = CASE WHEN x.sochuatra = 0 THEN x.ngaytracuoi ELSE NULL END,
    songayquahan = CASE 
        WHEN x.ngaytracuoi IS NOT NULL AND x.ngaytracuoi > pm.hantra THEN DATEDIFF(DAY, pm.hantra, x.ngaytracuoi)
        ELSE 0
    END
FROM phieu_muon pm
INNER JOIN (
    SELECT sophieumuon,
           COUNT(*) AS tongsoluong,
           SUM(CASE WHEN ngaytra IS NULL THEN 1 ELSE 0 END) AS sochuatra,
           MAX(ngaytra) AS ngaytracuoi
    FROM ct_phieu_muon
    GROUP BY sophieumuon
) x ON pm.sophieumuon = x.sophieumuon;
GO

UPDATE sv
SET sosachdangmuon = ISNULL(x.sosachdangmuon, 0)
FROM sinh_vien sv
LEFT JOIN (
    SELECT pm.masinhvien, COUNT(*) AS sosachdangmuon
    FROM phieu_muon pm
    INNER JOIN ct_phieu_muon ct ON pm.sophieumuon = ct.sophieumuon
    WHERE ct.ngaytra IS NULL
    GROUP BY pm.masinhvien
) x ON sv.masinhvien = x.masinhvien;
GO

UPDATE p
SET tongphat = x.tongphat,
    sotienconlai = x.tongphat
FROM phieu_phat_qua_han p
INNER JOIN (
    SELECT sophieuphatquahan, SUM(tongphat) AS tongphat
    FROM ct_phieu_phat_qua_han
    GROUP BY sophieuphatquahan
) x ON p.sophieuphatquahan = x.sophieuphatquahan;
GO

UPDATE p
SET tongphat = x.tongphat,
    sotienconlai = x.tongphat
FROM phieu_phat_hong_mat p
INNER JOIN (
    SELECT sophieuphathongmat, SUM(tongphat) AS tongphat
    FROM ct_phieu_phat_hong_mat
    GROUP BY sophieuphathongmat
) x ON p.sophieuphathongmat = x.sophieuphathongmat;
GO

UPDATE ct
SET tinhtrang = CASE
        WHEN ct.mucdo LIKE N'%Mất%' THEN N'Mất'
        WHEN ct.mucdo LIKE N'%Hỏng%' OR ct.mucdo LIKE N'%rách%' OR ct.mucdo LIKE N'%bẩn%' OR ct.mucdo LIKE N'%Cong%' OR ct.mucdo LIKE N'%Bong%' THEN N'Hỏng'
        ELSE ct.mucdo
    END
FROM ct_phieu_phat_hong_mat ct
WHERE ct.tinhtrang IS NULL;
GO

UPDATE sv
SET solanvipham = ISNULL(v.solanvipham, 0),
    sotienphatchuatra = ISNULL(v.tongtienphat, 0),
    ngayviphamgannhat = v.ngayviphamgannhat,
    trangthai = CASE WHEN ISNULL(v.tongtienphat, 0) > 0 THEN N'Đang nợ phạt' ELSE N'Được mượn' END
FROM sinh_vien sv
LEFT JOIN (
    SELECT masinhvien,
           COUNT(*) AS solanvipham,
           SUM(tongphat) AS tongtienphat,
           MAX(ngaylap) AS ngayviphamgannhat
    FROM (
        SELECT masinhvien, tongphat, ngaylap FROM phieu_phat_qua_han
        UNION ALL
        SELECT masinhvien, tongphat, ngaylap FROM phieu_phat_hong_mat
    ) f
    GROUP BY masinhvien
) v ON sv.masinhvien = v.masinhvien;
GO

UPDATE ds
SET soluong = ISNULL(x.tongban, 0),
    soluonghienco = ISNULL(x.hienco, 0),
    soluongdangmuon = ISNULL(x.dangmuon, 0),
    soluonghongmat = ISNULL(x.hongmat, 0),
    lanmuongannhat = x.lanmuongannhat,
    chuoitimkiem = CONCAT(ds.tendausach, N' ', ISNULL(ds.theloai, N''), N' ', ISNULL(ds.tacgia, N''))
FROM dau_sach ds
LEFT JOIN (
    SELECT s.madausach,
           COUNT(*) AS tongban,
           SUM(CASE WHEN s.tinhtrang = N'Có thể mượn' THEN 1 ELSE 0 END) AS hienco,
           SUM(CASE WHEN s.tinhtrang = N'Đang mượn' THEN 1 ELSE 0 END) AS dangmuon,
           SUM(CASE WHEN s.tinhtrang IN (N'Hỏng', N'Mất') THEN 1 ELSE 0 END) AS hongmat,
           MAX(s.ngaymuongannhat) AS lanmuongannhat
    FROM sach s
    GROUP BY s.madausach
) x ON ds.madausach = x.madausach;
GO

CREATE TYPE dbo.DanhSachSachMuon AS TABLE
(
    MaSach VARCHAR(20) PRIMARY KEY,
    GhiChu NVARCHAR(255) NULL
);
GO

CREATE PROCEDURE sp_TimSinhVien
    @MaSinhVien VARCHAR(20)
AS
BEGIN
    SELECT masinhvien, hoten, gioitinh, lop, khoa, sodienthoai, sosachdangmuon, solanvipham, trangthai
    FROM sinh_vien
    WHERE masinhvien = @MaSinhVien;
END;
GO

CREATE PROCEDURE sp_TimSach
    @MaSach VARCHAR(20)
AS
BEGIN
    SELECT masach, tensach, theloai, tacgia, nhaxuatban, namxuatban, tinhtrang, trangthai
    FROM sach
    WHERE masach = @MaSach;
END;
GO

CREATE PROCEDURE sp_LapPhieuMuonTraSach
    @SoPhieuMuon VARCHAR(20),
    @MaSinhVien VARCHAR(20),
    @NgayMuon DATE,
    @HanTra DATE,
    @DanhSachSach dbo.DanhSachSachMuon READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM phieu_muon WHERE sophieumuon = @SoPhieuMuon)
            THROW 50000, N'Số phiếu mượn đã tồn tại.', 1;

        IF NOT EXISTS (SELECT 1 FROM sinh_vien WHERE masinhvien = @MaSinhVien)
            THROW 50001, N'Sinh viên không tồn tại.', 1;

        IF NOT EXISTS (SELECT 1 FROM @DanhSachSach)
            THROW 50002, N'Chưa có sách mượn.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DanhSachSach ds
            LEFT JOIN sach s ON ds.MaSach = s.masach
            WHERE s.masach IS NULL
        )
            THROW 50003, N'Có sách không tồn tại.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DanhSachSach ds
            INNER JOIN sach s ON ds.MaSach = s.masach
            WHERE s.tinhtrang <> N'Có thể mượn'
        )
            THROW 50004, N'Có sách không đủ điều kiện cho mượn.', 1;

        INSERT INTO phieu_muon(sophieumuon, masinhvien, ngaymuon, hantra, trangthaiphieu, tongsoluong)
        VALUES (@SoPhieuMuon, @MaSinhVien, @NgayMuon, @HanTra, N'Đang mượn', (SELECT COUNT(*) FROM @DanhSachSach));

        INSERT INTO ct_phieu_muon(sophieumuon, masach, ngaytra, ghichu, tongsoluong, soluong, masinhvien, madausach, theloai)
        SELECT @SoPhieuMuon, ds.MaSach, NULL, ds.GhiChu,
               (SELECT COUNT(*) FROM @DanhSachSach), 1, @MaSinhVien, s.madausach, s.theloai
        FROM @DanhSachSach ds
        INNER JOIN sach s ON ds.MaSach = s.masach;

        UPDATE s
        SET tinhtrang = N'Đang mượn',
            trangthai = N'Đang mượn',
            sophieumuonhientai = @SoPhieuMuon,
            solanmuon = solanmuon + 1,
            ngaymuongannhat = @NgayMuon,
            ngaycapnhattrangthai = CAST(GETDATE() AS DATE)
        FROM sach s
        INNER JOIN @DanhSachSach ds ON s.masach = ds.MaSach;

        UPDATE sinh_vien
        SET sosachdangmuon = sosachdangmuon + (SELECT COUNT(*) FROM @DanhSachSach)
        WHERE masinhvien = @MaSinhVien;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_LayChiTietPhieuMuon
    @SoPhieuMuon VARCHAR(20)
AS
BEGIN
    SELECT 
        pm.sophieumuon,
        pm.ngaymuon,
        pm.hantra,
        pm.trangthaiphieu,
        sv.masinhvien,
        sv.hoten,
        sv.lop,
        sv.khoa,
        sv.sodienthoai,
        s.masach,
        s.tensach,
        s.theloai,
        s.tacgia,
        ct.ngaytra,
        s.tinhtrang,
        ct.ghichu
    FROM phieu_muon pm
    INNER JOIN sinh_vien sv ON pm.masinhvien = sv.masinhvien
    INNER JOIN ct_phieu_muon ct ON pm.sophieumuon = ct.sophieumuon
    INNER JOIN sach s ON ct.masach = s.masach
    WHERE pm.sophieumuon = @SoPhieuMuon
    ORDER BY s.masach;
END;
GO

CREATE PROCEDURE sp_CapNhatTraSach
    @SoPhieuMuon VARCHAR(20),
    @MaSach VARCHAR(20),
    @NgayTra DATE,
    @TinhTrangSauTra NVARCHAR(50),
    @GhiChu NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaSinhVien VARCHAR(20);
    DECLARE @NgayMuon DATE;
    DECLARE @HanTra DATE;
    DECLARE @SoNgayQuaHan INT;
    DECLARE @PhiPhat DECIMAL(18,2);
    DECLARE @SoPhieuPhat VARCHAR(30);

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM ct_phieu_muon WHERE sophieumuon = @SoPhieuMuon AND masach = @MaSach)
            THROW 51001, N'Sách không thuộc phiếu mượn này.', 1;

        SELECT @MaSinhVien = masinhvien, @NgayMuon = ngaymuon, @HanTra = hantra
        FROM phieu_muon
        WHERE sophieumuon = @SoPhieuMuon;

        UPDATE ct_phieu_muon
        SET ngaytra = @NgayTra, ghichu = @GhiChu
        WHERE sophieumuon = @SoPhieuMuon AND masach = @MaSach;

        UPDATE sach
        SET tinhtrang = CASE 
                WHEN @TinhTrangSauTra = N'Tốt' THEN N'Có thể mượn'
                WHEN @TinhTrangSauTra = N'Hỏng' THEN N'Hỏng'
                WHEN @TinhTrangSauTra = N'Mất' THEN N'Mất'
                ELSE @TinhTrangSauTra
            END,
            trangthai = CASE 
                WHEN @TinhTrangSauTra = N'Tốt' THEN N'Trong kho'
                ELSE @TinhTrangSauTra
            END,
            sophieumuonhientai = NULL,
            ngaycapnhattrangthai = CAST(GETDATE() AS DATE)
        WHERE masach = @MaSach;

        UPDATE sinh_vien
        SET sosachdangmuon = CASE WHEN sosachdangmuon > 0 THEN sosachdangmuon - 1 ELSE 0 END
        WHERE masinhvien = @MaSinhVien;

        IF @NgayTra > @HanTra
        BEGIN
            SET @SoNgayQuaHan = DATEDIFF(DAY, @HanTra, @NgayTra);
            SET @PhiPhat = @SoNgayQuaHan * 2000;
            SET @SoPhieuPhat = CONCAT('PQH', RIGHT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 12));

            INSERT INTO phieu_phat_qua_han(sophieuphatquahan, ngaylap, sophieumuon, masinhvien, tongphat, sotienconlai)
            VALUES (@SoPhieuPhat, CAST(GETDATE() AS DATE), @SoPhieuMuon, @MaSinhVien, @PhiPhat, @PhiPhat);

            INSERT INTO ct_phieu_phat_qua_han(sophieuphatquahan, masach, songayquahan, phiphat, ngaymuon, hantra, ngaytra, tongphat)
            VALUES (@SoPhieuPhat, @MaSach, @SoNgayQuaHan, @PhiPhat, @NgayMuon, @HanTra, @NgayTra, @PhiPhat);

            UPDATE sinh_vien
            SET solanvipham = solanvipham + 1,
                sotienphatchuatra = sotienphatchuatra + @PhiPhat,
                ngayviphamgannhat = CAST(GETDATE() AS DATE)
            WHERE masinhvien = @MaSinhVien;
        END

        IF @TinhTrangSauTra IN (N'Hỏng', N'Mất')
        BEGIN
            SET @PhiPhat = CASE WHEN @TinhTrangSauTra = N'Hỏng' THEN 50000 ELSE 100000 END;
            SET @SoPhieuPhat = CONCAT('PHM', RIGHT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 12));

            INSERT INTO phieu_phat_hong_mat(sophieuphathongmat, ngaylap, sophieumuon, masinhvien, tongphat, sotienconlai)
            VALUES (@SoPhieuPhat, CAST(GETDATE() AS DATE), @SoPhieuMuon, @MaSinhVien, @PhiPhat, @PhiPhat);

            INSERT INTO ct_phieu_phat_hong_mat(sophieuphathongmat, masach, tinhtrang, mucdo, phiphat, tongphat)
            VALUES (@SoPhieuPhat, @MaSach, @TinhTrangSauTra, @TinhTrangSauTra, @PhiPhat, @PhiPhat);

            UPDATE sinh_vien
            SET solanvipham = solanvipham + 1,
                sotienphatchuatra = sotienphatchuatra + @PhiPhat,
                ngayviphamgannhat = CAST(GETDATE() AS DATE)
            WHERE masinhvien = @MaSinhVien;
        END

        IF NOT EXISTS (SELECT 1 FROM ct_phieu_muon WHERE sophieumuon = @SoPhieuMuon AND ngaytra IS NULL)
        BEGIN
            UPDATE phieu_muon
            SET trangthaiphieu = N'Đã trả', ngayhoantat = @NgayTra
            WHERE sophieumuon = @SoPhieuMuon;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_ThongKeViPhamMuonTraTheoThang
    @Thang INT,
    @Nam INT,
    @LoaiViPham NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TuNgay DATE = DATEFROMPARTS(@Nam, @Thang, 1);
    DECLARE @DenNgay DATE = DATEADD(MONTH, 1, @TuNgay);

    SELECT 
        sv.masinhvien AS MaSinhVien,
        sv.hoten AS HoTen,
        sv.lop AS Lop,
        sv.khoa AS Khoa,
        N'Mượn sách quá hạn' AS LoaiViPham,
        CONCAT(N'Quá hạn ', ct.songayquahan, N' ngày') AS MucDoViPham,
        CONCAT(N'Phạt tiền: ', CONVERT(VARCHAR(30), ct.phiphat), N' đồng') AS HinhThucXuLy,
        p.ngaylap AS NgayLap,
        p.sophieuphatquahan AS GhiChu
    FROM phieu_phat_qua_han p
    INNER JOIN sinh_vien sv ON p.masinhvien = sv.masinhvien
    INNER JOIN ct_phieu_phat_qua_han ct ON p.sophieuphatquahan = ct.sophieuphatquahan
    WHERE p.ngaylap >= @TuNgay
      AND p.ngaylap < @DenNgay
      AND (@LoaiViPham IS NULL OR @LoaiViPham = N'Tất cả' OR @LoaiViPham = N'Quá hạn')

    UNION ALL

    SELECT 
        sv.masinhvien AS MaSinhVien,
        sv.hoten AS HoTen,
        sv.lop AS Lop,
        sv.khoa AS Khoa,
        N'Hỏng/Mất sách' AS LoaiViPham,
        ISNULL(ct.mucdo, N'') AS MucDoViPham,
        CONCAT(N'Phạt tiền: ', CONVERT(VARCHAR(30), ct.phiphat), N' đồng') AS HinhThucXuLy,
        p.ngaylap AS NgayLap,
        p.sophieuphathongmat AS GhiChu
    FROM phieu_phat_hong_mat p
    INNER JOIN sinh_vien sv ON p.masinhvien = sv.masinhvien
    INNER JOIN ct_phieu_phat_hong_mat ct ON p.sophieuphathongmat = ct.sophieuphathongmat
    WHERE p.ngaylap >= @TuNgay
      AND p.ngaylap < @DenNgay
      AND (@LoaiViPham IS NULL OR @LoaiViPham = N'Tất cả' OR @LoaiViPham = N'Hỏng/Mất')

    ORDER BY NgayLap DESC, MaSinhVien;
END;
GO

PRINT N'DA TAO XONG DATABASE QL_ThuVien_CSDLNC';
GO

CREATE TYPE dbo.DanhSachSachPhatHongMatType AS TABLE
(
    masach VARCHAR(20),
    tinhtrang NVARCHAR(50),
    mucdo NVARCHAR(100),
    phiphat DECIMAL(18,2)
);
GO

CREATE TYPE dbo.DanhSachSachPhatQuaHanType AS TABLE
(
    masach VARCHAR(20)
);
GO

CREATE OR ALTER PROCEDURE dbo.sp_ThongKeSachHongMatTrongThang
    @Thang INT,
    @Nam INT,
    @TinhTrang NVARCHAR(50) = N'Tất cả'
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.masach AS MaSach,
        s.tensach AS TenSach,
        s.theloai AS TheLoai,
        s.tacgia AS TacGia,
        s.nhaxuatban AS NXB,
        s.tinhtrang AS TinhTrang
    FROM sach s
    WHERE MONTH(s.ngaycapnhattrangthai) = @Thang
      AND YEAR(s.ngaycapnhattrangthai) = @Nam
      AND s.tinhtrang IN (N'Hỏng', N'Mất')
      AND (@TinhTrang = N'Tất cả' OR s.tinhtrang = @TinhTrang);

    SELECT
        COUNT(*) AS TongSoSachHongMat,
        SUM(CASE WHEN s.tinhtrang = N'Hỏng' THEN 1 ELSE 0 END) AS SoSachHong,
        SUM(CASE WHEN s.tinhtrang = N'Mất' THEN 1 ELSE 0 END) AS SoSachMat
    FROM sach s
    WHERE MONTH(s.ngaycapnhattrangthai) = @Thang
      AND YEAR(s.ngaycapnhattrangthai) = @Nam
      AND s.tinhtrang IN (N'Hỏng', N'Mất')
      AND (@TinhTrang = N'Tất cả' OR s.tinhtrang = @TinhTrang);
END;
GO

IF TYPE_ID(N'dbo.DanhSachSachPhatHongMatType') IS NULL
BEGIN
    EXEC(N'
        CREATE TYPE dbo.DanhSachSachPhatHongMatType AS TABLE
        (
            masach VARCHAR(20),
            tinhtrang NVARCHAR(50),
            mucdo NVARCHAR(255),
            phiphat DECIMAL(18,2)
        );
    ');
END;
GO

IF TYPE_ID(N'dbo.DanhSachSachPhatQuaHanType') IS NULL
BEGIN
    EXEC(N'
        CREATE TYPE dbo.DanhSachSachPhatQuaHanType AS TABLE
        (
            masach VARCHAR(20)
        );
    ');
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LapPhieuPhatHongMat
    @SoPhieuMuonTra VARCHAR(20),
    @DanhSachSach dbo.DanhSachSachPhatHongMatType READONLY,
    @MaNguoiDung VARCHAR(20) = NULL,
    @TenNguoiDung NVARCHAR(100) = NULL,
    @SoPhieuPhatHongMat VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaSinhVien VARCHAR(20);

    SELECT @MaSinhVien = masinhvien
    FROM phieu_muon
    WHERE sophieumuon = @SoPhieuMuonTra;

    IF @MaSinhVien IS NULL
        THROW 52001, N'Phiếu mượn không tồn tại.', 1;

    IF EXISTS (
        SELECT 1
        FROM @DanhSachSach ds
        WHERE NOT EXISTS (
            SELECT 1
            FROM ct_phieu_muon ct
            WHERE ct.sophieumuon = @SoPhieuMuonTra
              AND ct.masach = ds.masach
        )
    )
        THROW 52002, N'Có sách không thuộc phiếu mượn này.', 1;

    SET @SoPhieuPhatHongMat = CONCAT('PHM', RIGHT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 12));

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO phieu_phat_hong_mat
        (
            sophieuphathongmat, ngaylap, sophieumuon, masinhvien,
            manguoidung, tennguoidung, tongphat, sotienconlai
        )
        SELECT
            @SoPhieuPhatHongMat, CAST(GETDATE() AS DATE), @SoPhieuMuonTra, @MaSinhVien,
            @MaNguoiDung, @TenNguoiDung, SUM(phiphat), SUM(phiphat)
        FROM @DanhSachSach;

        INSERT INTO ct_phieu_phat_hong_mat
        (
            sophieuphathongmat, masach, tinhtrang, mucdo, phiphat, tongphat
        )
        SELECT
            @SoPhieuPhatHongMat, masach, tinhtrang, mucdo, phiphat, phiphat
        FROM @DanhSachSach;

        UPDATE s
        SET tinhtrang = ds.tinhtrang,
            trangthai = ds.tinhtrang,
            ngaycapnhattrangthai = CAST(GETDATE() AS DATE)
        FROM sach s
        INNER JOIN @DanhSachSach ds ON s.masach = ds.masach;

        UPDATE sinh_vien
        SET solanvipham = solanvipham + 1,
            sotienphatchuatra = sotienphatchuatra + (SELECT SUM(phiphat) FROM @DanhSachSach),
            ngayviphamgannhat = CAST(GETDATE() AS DATE)
        WHERE masinhvien = @MaSinhVien;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LapPhieuPhatQuaHan
    @SoPhieuMuonTra VARCHAR(20),
    @DanhSachSach dbo.DanhSachSachPhatQuaHanType READONLY,
    @NgayTra DATE,
    @MucPhatMoiNgay DECIMAL(18,2),
    @MaNguoiDung VARCHAR(20) = NULL,
    @TenNguoiDung NVARCHAR(100) = NULL,
    @SoPhieuPhatQuaHan VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaSinhVien VARCHAR(20);
    DECLARE @NgayMuon DATE;
    DECLARE @HanTra DATE;
    DECLARE @SoNgayQuaHan INT;
    DECLARE @PhiMoiSach DECIMAL(18,2);
    DECLARE @TongPhat DECIMAL(18,2);

    SELECT
        @MaSinhVien = masinhvien,
        @NgayMuon = ngaymuon,
        @HanTra = hantra
    FROM phieu_muon
    WHERE sophieumuon = @SoPhieuMuonTra;

    IF @MaSinhVien IS NULL
        THROW 53001, N'Phiếu mượn không tồn tại.', 1;

    SET @SoNgayQuaHan = DATEDIFF(DAY, @HanTra, @NgayTra);

    IF @SoNgayQuaHan <= 0
        THROW 53002, N'Phiếu mượn chưa quá hạn.', 1;

    IF EXISTS (
        SELECT 1
        FROM @DanhSachSach ds
        WHERE NOT EXISTS (
            SELECT 1
            FROM ct_phieu_muon ct
            WHERE ct.sophieumuon = @SoPhieuMuonTra
              AND ct.masach = ds.masach
        )
    )
        THROW 53003, N'Có sách không thuộc phiếu mượn này.', 1;

    SET @PhiMoiSach = @SoNgayQuaHan * @MucPhatMoiNgay;
    SET @TongPhat = @PhiMoiSach * (SELECT COUNT(*) FROM @DanhSachSach);
    SET @SoPhieuPhatQuaHan = CONCAT('PQH', RIGHT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 12));

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO phieu_phat_qua_han
        (
            sophieuphatquahan, ngaylap, sophieumuon, masinhvien,
            manguoidung, tennguoidung, tongphat, sotienconlai
        )
        VALUES
        (
            @SoPhieuPhatQuaHan, CAST(GETDATE() AS DATE), @SoPhieuMuonTra, @MaSinhVien,
            @MaNguoiDung, @TenNguoiDung, @TongPhat, @TongPhat
        );

        INSERT INTO ct_phieu_phat_qua_han
        (
            sophieuphatquahan, masach, songayquahan,
            phiphat, ngaymuon, hantra, ngaytra, tongphat
        )
        SELECT
            @SoPhieuPhatQuaHan, masach, @SoNgayQuaHan,
            @PhiMoiSach, @NgayMuon, @HanTra, @NgayTra, @PhiMoiSach
        FROM @DanhSachSach;

        UPDATE ct
        SET ngaytra = @NgayTra,
            ghichu = N'Đã lập phiếu phạt quá hạn: ' + @SoPhieuPhatQuaHan
        FROM ct_phieu_muon ct
        INNER JOIN @DanhSachSach ds ON ct.masach = ds.masach
        WHERE ct.sophieumuon = @SoPhieuMuonTra;

        UPDATE sinh_vien
        SET solanvipham = solanvipham + 1,
            sotienphatchuatra = sotienphatchuatra + @TongPhat,
            ngayviphamgannhat = CAST(GETDATE() AS DATE)
        WHERE masinhvien = @MaSinhVien;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO


-- Reset role package mappings
DELETE FROM nhom_nguoi_dung_ct;
DELETE FROM nhom_quyen;

-- Reset finalized groups
DELETE FROM nhom_nguoi_dung;

INSERT INTO nhom_nguoi_dung (manhom, tennhom) VALUES
('N001', N'Quản trị viên'),
('N002', N'Tủ thư nghiệp vụ'),
('N003', N'Kho và kiểm kê'),
('N004', N'Quản lý và báo cáo');

-- N001: Quản trị viên - full permissions
INSERT INTO nhom_quyen (manhom, maquyen) VALUES
('N001', 'Q001'),
('N001', 'Q002'),
('N001', 'Q003'),
('N001', 'Q004'),
('N001', 'Q005'),
('N001', 'Q006'),
('N001', 'Q007'),
('N001', 'Q008'),
('N001', 'Q009'),
('N001', 'Q010');

-- N002: Thủ thư nghiệp vụ
INSERT INTO nhom_quyen (manhom, maquyen) VALUES
('N002', 'Q002'), -- Quản lý danh mục sách
('N002', 'Q003'), -- Lập phiếu mượn sách
('N002', 'Q004'), -- Cập nhật trả sách
('N002', 'Q005'); -- Lập phiếu phạt

-- N003: Kho và kiểm kê
INSERT INTO nhom_quyen (manhom, maquyen) VALUES
('N003', 'Q002'), -- Quản lý danh mục sách
('N003', 'Q006'), -- Kiểm kê sách
('N003', 'Q007'), -- Thanh lý sách
('N003', 'Q008'); -- Hủy sách

-- N004: Quản lý và báo cáo
INSERT INTO nhom_quyen (manhom, maquyen) VALUES
('N004', 'Q005'), -- Xem/kiểm tra phiếu phạt
('N004', 'Q009'); -- Thống kê báo cáo

-- Assign users to finalized packages
INSERT INTO nhom_nguoi_dung_ct (manhom, manguoidung) VALUES
('N001', 'ND001'), -- admin

('N002', 'ND002'), -- thuthu01
('N002', 'ND005'), -- thuthu02

('N003', 'ND003'), -- kho01
('N003', 'ND006'), -- kho02
('N003', 'ND009'), -- kiemke01

('N004', 'ND004'), -- ketoan01
('N004', 'ND007'), -- baocao01
('N004', 'ND008'), -- quanly01
('N004', 'ND010'); -- user01