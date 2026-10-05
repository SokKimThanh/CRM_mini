## Khối nghiệp vụ Customer CQRS — Day 6

### Queries
- `GetCustomerListQuery(CustomerFilterDto Filter, Guid CurrentUserId, Guid? CurrentTeamId, string CurrentRole)`
  - **Output:** `PagedResult<CustomerListDto>`
  - **Quy tắc bảo mật IDOR Lớp 2:** SALES (chỉ thấy khách của mình), MANAGER (toàn team), ADMIN (toàn tenant), Role khác $\rightarrow$ Deny by default (rỗng).

### Commands
- `CreateCustomerCommand(string Name, string? Phone, string? Email, string? TaxCode, string? Address, string? Industry, int HealthStatus)`
  - **Output:** `long` (Customer Id)
  - **Mã khách hàng:** Sinh tự động 2-phase format `KH-{Id:D4}`.
- `UpdateCustomerCommand(long Id, string Name, string? Phone, string? Email, string? TaxCode, string? Address, string? Industry, int HealthStatus)`
  - **Output:** `Unit`
  - **Kiểm soát IDOR Lớp 2:** Ném `UnauthorizedAccessException` nếu SALES sửa khách không do mình quản lý.

### Ngoại lệ chuẩn hóa
- `ArgumentException`: Vi phạm quy tắc xác thực dữ liệu đầu vào.
- `UnauthorizedAccessException`: Vi phạm IDOR Lớp 2.
- `KeyNotFoundException`: Không tìm thấy bản ghi khách hàng.
- `DbUpdateConcurrencyException`: Xung đột ghi đồng thời qua Concurrency Token `xmin`.
