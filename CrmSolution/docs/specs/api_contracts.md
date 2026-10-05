# Đặc tả Giao tiếp API (API Contracts)

Tài liệu này định nghĩa cấu trúc Request/Response (DTO) của các API trong hệ thống. Các thông tin tại đây phải khớp 100% với các record/class Command và Query trong C# (CQRS).

## 1. Module Lead
### Tạo mới Lead (CreateLeadCommand)
- **Endpoint**: Tương đương với việc gọi `Mediator.Send(new CreateLeadCommand(...))`
- **Request Body**:
  - `CompanyName` (string): Tên doanh nghiệp (Bắt buộc).
  - `ContactName` (string): Tên người đại diện (Bắt buộc).
  - `Phone` (string): Số điện thoại (Bắt buộc).
- **Response**: Trả về `LeadDto` kèm `Id` (UUID) của Lead vừa tạo.

---
*Lưu ý: Mọi thay đổi về DTO trong C# bắt buộc phải cập nhật đồng bộ vào file này theo chuẩn [K15].*