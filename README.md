# Dự án CRM Mini

Dự án CRM (Quản trị Quan hệ Khách hàng) dành cho mô hình B2B.

## 1. Hệ thống Nhánh (Git Flow)
Dự án áp dụng quy trình quản lý mã nguồn tự động, bao gồm 4 nhánh chính:
- `main`: Chứa mã nguồn Production (chỉ dành cho bản chính thức).
- `staging`: Môi trường kiểm thử (Testing/QA).
- `develop`: Nhánh phát triển chính (tích hợp các tính năng mới).
- `docs`: Chuyên lưu trữ tài liệu, quy trình, cấu trúc Sprint.

**Quy tắc tạo nhánh phụ (Làm việc hàng ngày):**
- Tính năng mới: `feature/ten-tinh-nang`
- Sửa lỗi thường: `bugfix/ten-loi`
- Sửa lỗi khẩn cấp (trên main): `hotfix/ten-loi`
- Đóng gói phát hành: `release/vX.X.X`

## 2. Tài liệu Dự án
Để giữ cho file README này ngắn gọn, toàn bộ tài liệu chi tiết (SRS, Kế hoạch) đã được tách nhỏ và chuyển vào thư mục `docs/`. Bạn có thể tham khảo theo cấu trúc sau:
- Mọi tài liệu đặc tả, tiến trình công việc, thiết kế hệ thống xem tại: [Thư mục Documents](./Documents)

*(Ghi chú: Theo Kỹ năng K9, README.md luôn được giữ ngắn gọn dưới 1 trang. Mọi thông tin dài hơn sẽ được cắt vào thư mục `docs/`)*
