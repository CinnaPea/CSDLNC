# CSDLNC - Hệ thống quản lý thư viện

## Giới thiệu dự án

Đây là hệ thống quản lý thư viện sử dụng ASP.NET Core MVC, Entity Framework Core và SQL Server.

Hệ thống tập trung vào việc khai thác cơ sở dữ liệu thông qua các chức năng nghiệp vụ như quản lý sách, lập phiếu mượn/trả, lập phiếu phạt, kiểm kê kho sách và thống kê báo cáo. Dữ liệu được thiết kế theo mô hình quan hệ, có phân quyền người dùng và các nhóm chức năng tương ứng với từng vai trò trong hệ thống.

## Công nghệ sử dụng

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap
- Razor View
- Cookie Authentication
- Git/GitHub

## Tài khoản demo

| Mã người dùng | Tên đăng nhập | Vai trò |
| ND001 | admin | Quản trị hệ thống |
| ND002 | thuthu01 | Thủ thư nghiệp vụ |
| ND003 | kho01 | Kho và kiểm kê |
| ND004 | ketoan01 | Phiếu phạt và báo cáo |

Mật khẩu mặc định cho các tài khoản demo:

123456

## Phân công thực hiện

### ND001 - Admin

Phụ trách phân quản trị tổng hợp và kiểm tra cuối hệ thống.

Chức năng chính:

- Đăng nhập / đăng xuất
- Phân quyền hiển thị chức năng theo người dùng
- Quản lý người dùng
- Đồng bộ giao diện sidebar
- Kiểm tra và tích hợp các nhánh chức năng của nhóm

### ND002 - Phuc: Thủ thư nghiệp vụ

Phụ trách các chức năng liên quan đến nghiệp vụ mượn/trả sách.

Chức năng thực hiện:

- Quản lý danh mục sách
- Lập phiếu mượn sách
- Cập nhật trả sách
- Thống kê vi phạm mượn trả sách

### ND003 - Quang: Kho và kiểm kê

Phụ trách các chức năng liên quan đến kho sách và kiểm kê.

Chức năng thực hiện:

- Lập biên bản nhận bàn giao sách
- Lập phiếu kiểm kê sách
- Thống kê số lượng sách theo đầu sách và thể loại

### ND004 - Hieu: Phiếu phạt và báo cáo

Phụ trách các chức năng xử lý vi phạm và báo cáo phakt.

Chức năng thực hiện:

- Lập phiếu phạt hỏng/mất sách
- Lập phiếu phạt quá hạn
- Thống kê sách hỏng/mất trong tháng

## Các chức năng chính của hệ thống

- Đăng nhập và phân quyền người dùng
- Hiển thị sidebar theo quyền của từng tài khoản
- Quản lý danh mục sách
- Lập phiếu mượn sách
- Cập nhật trả sách
- Lập phiếu phạt quá hạn
- Lập phiếu phạt hỏng/mất
- Lập biên bản nhận bàn giao sách
- Lập phiếu kiểm kê sách
- Thống kê số lượng sách theo đầu sách/thể loại
- Thống kê vi phạm mượn trả
- Thống kê sách hỏng/mất

## Cấu trúc dự án

CSDLNC/
├── Controllers/
├── Data/
├── Models/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
├── appsettings.json
└── Program.cs

## Ghi chú triển khai

- CSDL sử dụng SQL Server với database chính: QL_ThuVien_CSDLNC.
- Hệ thống sử dụng cookie authentication để kiểm soát đăng nhập.
- Các chức năng được phân chia theo nhóm quyền để phù hợp với từng tài khoản demo.
- Sidebar được tổ chức theo từng nhóm người dùng để dễ trình bày khi demo.
- Admin là tài khoản tổng hợp, có quyền truy cập và kiểm tra toàn bộ hệ thống.

## Mục tiêu đồ án

Mục tiêu của đồ án là xây dựng một hệ thống quản lý thư viện có khả năng:

- Thể hiện thiết kế cơ sở dữ liệu quan hệ rõ ràng.
- Khai thác dữ liệu thông qua các chức năng nghiệp vụ thực tế.
- Có cập nhật dữ liệu, thống kê và phân quyền người dùng.
- Có giao diện MVC trực quan, dễ sử dụng.
