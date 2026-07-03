QUY TẮC LÀM VIỆC NHÓM

1. Trước khi code

Luôn chạy lệnh sau để lấy phiên bản mới nhất:

git pull

2. Chia việc chính

Mỗi người phụ trách theo tài khoản demo và nhóm quyền:

- ND002 - thuthu01: Thu thu nghiep vu
  Phụ trách: danh mục sách, lập phiếu mượn, cập nhật trả sách, phiếu phạt.

- ND003 - kho01: Kho va kiem ke
  Phụ trách: nhận bản giao sách, kiểm kê sách, thanh lý sách, hủy sách.

- ND004 - ketoan01: Quan ly va bao cao
  Phụ trách: thống kê vi phạm, thống kê sách hỏng/mất, tổng hợp phiếu phạt.

- ND001 - admin:
  Chỉ dùng để kiểm tra tổng thể, chỉ động vào sau khi hoàn thành cả 3.

3. Khong tu y sua phan chung

Không tự ý sửa các phần sau nếu chưa thống nhất:

- Program.cs
- _Layout.cshtml
- Auth/Login/Signup
- Sidebar/phan quyen
- File SQL cuoi
- Connection string

4. Quy tắc commit

Sau khi làm xong phần chạy được:

git add .
git commit -m "ghi rõ đã làm gì"
git push

Vi du:

git commit -m "Add phieu muon create screen"
git commit -m "Add thong ke vi pham report"

5. Không push file rác như:

node_modules/
bin/
obj/
.env
.vs/
.idea/
*.user

6. Khi gặp lỗi

Nếu bị lỗi Git/API/SQL/EF/MVC thì báo ngay cho nhóm.

Không tự ý sửa lung tung các file chung, nhất là SQL và layout, để tránh lỗi dây chuyền.
