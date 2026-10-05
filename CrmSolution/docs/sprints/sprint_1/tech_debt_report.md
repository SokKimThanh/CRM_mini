| \# | Vị trí Mã nguồn | Lý do Tạm thời | Giải pháp Lâu dài | Thời hạn Xử lý |
| :---: | :--- | :--- | :--- | :---: |
| 1 | `HttpTenantProvider.TenantId` | Hardcode GUID cho Sprint 1 đơn tenant | Đọc động từ Subdomain và JWT Claim | Sprint 3 |
| 2 | `CreateCustomerCommandHandler` | Lưu 2 lần (2-phase save) để lấy Id sinh Code | Chuyển sang PostgreSQL Sequence `NEXTVAL` trước khi insert | Sprint 2 |
| 3 | `ModelConfigurationTests` | Dùng chuỗi kết nối Npgsql giả lập để trích xuất metadata | Tích hợp Testcontainers với PostgreSQL thật | Sprint 2 |
