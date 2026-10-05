# Bảng Từ Điển Dữ Liệu (Data Dictionary)

Tài liệu này định nghĩa cấu trúc cơ sở dữ liệu vật lý của hệ thống CRM. Các thông tin tại đây phải khớp 100% với các file script DDL SQL trong thư mục `db/`.

## 1. Bảng `leads` (Đầu mối tiềm năng)
- `id` (UUID): Khóa chính.
- `tenant_id` (UUID): ID định danh khách thuê (Hỗ trợ Multi-tenancy).
- `owner_id` (UUID): ID của nhân viên phụ trách.
- `team_id` (UUID): ID của đội ngũ phụ trách.
- `company_name` (VARCHAR): Tên doanh nghiệp.
- `status` (VARCHAR): Trạng thái của Lead.
- `is_deleted` (BOOLEAN): Cờ xóa mềm.

## 2. Bảng `accounts` (Khách hàng doanh nghiệp)
*(Sẽ bổ sung cấu trúc chi tiết...)*

---
*Lưu ý: Mọi thay đổi về DDL SQL bắt buộc phải cập nhật đồng bộ vào file này theo chuẩn [K15].*