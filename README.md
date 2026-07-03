QUY TẮC LÀM VIỆC NHÓM

1. Trước khi code

Luôn chạy lệnh sau để lấy phiên bản mới nhất:

git pull

2. Chia việc chính

Mỗi người phụ trách theo tài khoản demo và nhóm quyền:

ND004 - ketoan01 (Hiếu):
- Lập phiếu phạt hỏng/mất
- Lập phiếu phạt quá hạn
- Thống kê sách hỏng/mất trong tháng

ND002 - thuthu01 (Phúc):
- Lập phiếu mượn trả sách
- Cập nhật trả sách
- Thống kê vi phạm mượn trả sách trong tháng

ND003 - kho01 (Quang):
- Lập biên bản nhận bàn giao sách
- Lập phiếu kiểm kê sách
- Thống kê số lượng sách theo đầu sách, thể loại

ND001 - admin:
  Chỉ dùng để kiểm tra tổng thể, chỉ động vào sau khi hoàn thành cả 3.

3. Không tự ý sửa n chung

Không tự ý sửa các phần sau nếu chưa thống nhất:
- Program.cs
- _Layout.cshtml
- Auth/Login/Signup
- Sidebar/phan quyen
- File SQL cuoi
- Connection string

Không ai sửa layout/CSS chung, mỗi người làm controller/view phần mình; nếu cần CSS thì tạo file riêng theo module rồi gắn bằng @section Styles, conflict thì báo nhóm xử lý chung!

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
