# TÀI LIỆU ĐẶC TẢ KỸ THUẬT HỆ THỐNG B2B CRM (PHÂN HỆ CHĂM SÓC KHÁCH HÀNG & QUẢN LÝ CƠ HỘI BÁN HÀNG)

* **Tên dự án:** Hệ thống Quản trị Quan hệ Khách hàng B2B (B2B CRM System)

* **Phiên bản:** 1.0.0 (Final Architecture & Specs)

* **Mục tiêu:** Quản lý quy trình bán hàng B2B, theo dõi sức khỏe khách hàng, tăng trưởng doanh thu và tối ưu hóa quy trình chăm sóc khách hàng.

* **Quy mô triển khai:** Ban đầu 10 người dùng đồng thời, khả năng mở rộng lên 100 người dùng.

* **Nguyên tắc tích hợp:**

  * **KiotViet:** Đóng vai trò là SSOT (Single Source of Truth) cho dữ liệu bán hàng, tồn kho, công nợ và thông tin khách hàng cơ bản.

  * **CRM System:** Quản lý toàn bộ vòng đời tương tác B2B, phân tích rủi ro/sức khỏe khách hàng, cơ hội bán hàng (Opportunity Pipeline), báo giá (Quotes) và các nhiệm vụ bán hàng (Sales Tasks).

## 1. KIẾN TRÚC & CÔNG NGHỆ (TECH STACK)

### 1.1 Chi tiết Công nghệ

| 

| **Lớp (Layer)** | **Công nghệ / Thuật ngữ** | **Mô tả chi tiết** | 
| **Runtime** | .NET 10 | Framework nền tảng hiệu năng cao, hỗ trợ Cross-platform. |
| **Web Presentation** | Blazor Server (Interactive Server) | Rendering phía Server, kết hợp UI động nhẹ nhàng. |
| **User Interface** | Blazor Server + MudBlazor + Chart.js | UI phản hồi nhanh qua Blazor Server, giao diện MudBlazor, hiển thị biểu đồ bằng Chart.js. |
| **ORM** | Entity Framework Core 8 (DB First) | Truy vấn và thao tác dữ liệu qua Scaffold Entity, đảm bảo tối ưu hóa SQL nguyên bản. | 
| **Database** | SQLite (Development) / PostgreSQL 16 (Production) | SQLite linh hoạt khi dev local, PostgreSQL đáp ứng tính toàn vẹn và chịu tải tốt trên Prod. | 
| **Background Jobs** | Hangfire | Quản lý tác vụ chạy ngầm, lập lịch tự động (Cron job) đồng bộ KiotViet và tính toán snapshot. | 
| **Export/Reports** | QuestPDF / ClosedXML | QuestPDF dùng tạo Báo giá/Hóa đơn PDF chuẩn vector, ClosedXML dùng xuất/nhập dữ liệu Excel. | 
| **Authentication & AuthZ** | ASP.NET Core Identity | Quản lý tài khoản, phân quyền dựa trên Role (RBAC) & Claim. | 
| **Deployment Platform** | VPS Ubuntu Server + Nginx Reverse Proxy + Kestrel | Tối ưu chi phí vận hành (300k - 500k VND/tháng). | 

### 1.2 Mô hình Kiến trúc 3 Lớp (3-Tier Architecture)

```
[ CrmApp.Web ] (Blazor Server, MudBlazor, CQRS Handlers, Identity)
       │
       ▼
[ CrmApp.Business ] (Services, Business Rules, DTOs, Validators, KiotViet API Client)
       │
       ▼
[ CrmApp.Data ] (AppDbContext, Entities Scaffolded, Repositories, Enums)
       ▲
       │
[ CrmApp.Jobs ] (Hangfire Tasks & Background Workers)

```

## 2. CẤU TRÚC THƯ MỤC DỰ ÁN

```
CrmApp/
├── CrmApp.Web/
│   ├── Pages/
│   │   ├── Dashboard/
│   │   │   ├── Index.cshtml
│   │   │   └── Index.cshtml.cs
│   │   ├── Customers/
│   │   │   ├── Index.cshtml (Danh sách + Filter)
│   │   │   ├── Detail.cshtml (Account 360 View)
│   │   │   ├── Create.cshtml
│   │   │   └── Edit.cshtml
│   │   ├── Contacts/
│   │   │   └── Index.cshtml
│   │   ├── Opportunities/
│   │   │   ├── Kanban.razor (Kéo thả Blazor)
│   │   │   ├── List.cshtml
│   │   │   ├── Detail.cshtml
│   │   │   └── Create.cshtml
│   │   ├── Quotes/
│   │   │   ├── Index.cshtml
│   │   │   ├── Create.cshtml
│   │   │   └── Print.cshtml (QuestPDF Template View)
│   │   ├── Tasks/
│   │   │   ├── Index.cshtml (Danh sách công việc daily)
│   │   │   └── Calendar.cshtml (Lịch hiển thị)
│   │   ├── Products/
│   │   │   └── Index.cshtml
│   │   └── Account/
│   │       ├── Login.cshtml
│   │       └── AccessDenied.cshtml
│   ├── ViewModels/
│   ├── wwwroot/
│   │   ├── js/ (crm-kanban.js, crm-charts.js)
│   │   ├── css/
│   │   └── lib/
│   ├── Features/ (CQRS Commands & Queries)
│   └── Program.cs
├── CrmApp.Business/
│   ├── Services/
│   │   ├── CustomerService.cs
│   │   ├── OpportunityService.cs
│   │   ├── QuoteService.cs
│   │   ├── TaskService.cs
│   │   ├── DashboardService.cs
│   │   └── KiotVietSyncService.cs
│   ├── DTOs/
│   └── Validators/ (FluentValidation)
├── CrmApp.Data/
│   ├── Entities/ (Sinh ra từ DB First Scaffold)
│   ├── Repositories/
│   ├── Enums/
│   └── AppDbContext.cs
└── CrmApp.Jobs/
    ├── GenerateRemindersJob.cs
    ├── SyncKiotVietJob.cs
    ├── UpdateCustomerHealthJob.cs
    ├── SnapshotDashboardJob.cs
    └── ConsistencyCheckJob.cs

```

## 3. THIẾT KẾ CƠ SỞ DỮ LIỆU (DATABASE DESIGN)

### 3.1 Nguyên tắc Quản lý Dữ liệu

1. **DB-First Workflow:**

   * Tất cả các chỉnh sửa cấu trúc bảng phải thực hiện trực tiếp qua Script SQL/Migration DDL trên PostgreSQL/SQLite.

   * Chạy lệnh Scaffold để cập nhật Entity:

     ```
     dotnet ef dbcontext scaffold "Server=...;Database=crm_db;..." Npgsql.EntityFrameworkCore.PostgreSQL -o Entities --force
     
     ```

   * Không sử dụng EF Core Migrations trong runtime dự án. Sử dụng phương pháp Database-First, tự thiết kế cấu trúc DDL SQL.
   * Khi scaffold EF Core, bắt buộc phải loại trừ các bảng Identity (như `AspNetUsers`, `AspNetRoles`) thông qua tham số `--table` để không ghi đè dữ liệu cấu hình hệ thống.

2. **Thời gian & Múi giờ:**

   * Mọi trường ngày tháng (`created_at`, `updated_at`, `due_date`,...) lưu trữ theo **UTC ISO 8601**.

   * Khi render trên giao diện UI hoặc xuất PDF/Excel sẽ convert trực tiếp về múi giờ Việt Nam (`ICT - GMT+7`).

3. **Tiền tệ & Số học:**

   * Tất cả dữ liệu tiền tệ lưu dưới dạng `DECIMAL(18,2)` trong DB.

   * Khi hiển thị phía UI/Báo cáo: Format tròn không số thập phân (Ví dụ: `1.250.000.000 VNĐ`).

### 3.2 Sơ đồ Các Bảng Dữ liệu (15 Bảng Chi Tiết)

#### 1. Bảng `customers` (Khách hàng B2B / Nhà máy)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Ràng buộc / Mô tả** | 
| `id` | `BIGINT / INT` | PK | Auto increment | 
| `code` | `VARCHAR(50)` | UNIQUE | Mã khách hàng (Sync từ KiotViet) | 
| `name` | `VARCHAR(255)` | NOT NULL | Tên công ty / nhà máy | 
| `industry` | `VARCHAR(100)` | NULL | Ngành nghề kinh doanh | 
| `tax_code` | `VARCHAR(50)` | NULL | Mã số thuế | 
| `address` | `TEXT` | NULL | Địa chỉ trụ sở / nhà máy | 
| `phone` | `VARCHAR(20)` | NULL | Số điện thoại chính | 
| `email` | `VARCHAR(100)` | NULL | Email liên hệ | 
| `assigned_to_user_id` | `VARCHAR(450)` | FK | Nhận diện Sales phụ trách (User Profile ID) | 
| `health_status` | `INT` | DEFAULT 0 | Enum `CustomerHealth` (0: New, 1: Healthy,...) | 
| `average_cycle_days` | `INT` | DEFAULT 0 | Chu kỳ mua hàng trung bình (ngày) | 
| `last_order_date` | `TIMESTAMP` | NULL | Ngày phát sinh đơn hàng gần nhất (KiotViet) | 
| `last_contact_date` | `TIMESTAMP` | NULL | Ngày tương tác gần nhất (Sales CRM) | 
| `next_contact_due` | `TIMESTAMP` | NULL | Hạn tương tác tiếp theo | 
| `revenue_90d` | `DECIMAL(18,2)` | DEFAULT 0 | Doanh thu tích lũy 90 ngày | 
| `order_count_90d` | `INT` | DEFAULT 0 | Số lượng đơn hàng 90 ngày | 
| `note` | `TEXT` | NULL | Ghi chú chung | 
| `is_active` | `BOOLEAN` | DEFAULT TRUE | Trạng thái hoạt động | 
| `deleted_at` | `TIMESTAMP` | NULL | Soft delete flag | 
| `created_at` | `TIMESTAMP` | NOT NULL | Thời gian tạo (UTC) | 
| `updated_at` | `TIMESTAMP` | NULL | Thời gian cập nhật (UTC) | 
| `row_version` | `BYTEA / BYTE[]` | RowVersion | Kiểm soát Optimistic Concurrency | 

#### 2. Bảng `contacts` (Người liên hệ thuộc Khách hàng)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `customer_id` | `BIGINT` | FK | Nối với `customers.id` | 
| `name` | `VARCHAR(100)` | NOT NULL | Họ tên người liên hệ | 
| `position` | `VARCHAR(100)` | NULL | Chức danh (Trưởng phòng vật tư, Kế toán,...) | 
| `phone` | `VARCHAR(20)` | NULL | Số điện thoại di động | 
| `email` | `VARCHAR(100)` | NULL | Email cá nhân | 
| `is_primary` | `BOOLEAN` | DEFAULT FALSE | Là người liên hệ chính | 
| `created_at` | `TIMESTAMP` | NOT NULL | UTC | 

#### 3. Bảng `customer_assignments` (Lịch sử điều chuyển Sales)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `customer_id` | `BIGINT` | FK | Nối với `customers.id` | 
| `from_user_id` | `VARCHAR(450)` | FK | Sales cũ | 
| `to_user_id` | `VARCHAR(450)` | FK | Sales mới | 
| `assigned_by` | `VARCHAR(450)` | FK | Người thực hiện gán (Manager) | 
| `reason` | `TEXT` | NULL | Lý do chuyển bàn giao | 
| `assigned_at` | `TIMESTAMP` | NOT NULL | UTC | 

#### 4. Bảng `opportunity_stages` (Các giai đoạn bán hàng)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `INT` | PK | 1 đến 6 | 
| `name` | `VARCHAR(100)` | NOT NULL | Tên giai đoạn (Mới, Lên Báo Giá, Negotiation, Won, Lost,...) | 
| `display_order` | `INT` | NOT NULL | Thứ tự hiển thị trên Kanban | 
| `default_probability` | `INT` | NOT NULL | Tỷ lệ thành công mặc định (%) | 

#### 5. Bảng `opportunities` (Cơ hội bán hàng)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `code` | `VARCHAR(50)` | UNIQUE | Mã cơ hội (VD: OPP-2026-001) | 
| `customer_id` | `BIGINT` | FK | Khách hàng thuộc về | 
| `contact_id` | `BIGINT` | FK | Người liên hệ đại diện | 
| `stage_id` | `INT` | FK | Nối với `opportunity_stages.id` | 
| `title` | `VARCHAR(255)` | NOT NULL | Tiêu đề cơ hội | 
| `estimated_value` | `DECIMAL(18,2)` | DEFAULT 0 | Giá trị dự kiến | 
| `probability` | `INT` | DEFAULT 0 | Tỷ lệ chốt thành công (%) | 
| `assigned_to_user_id` | `VARCHAR(450)` | FK | Sales chịu trách nhiệm | 
| `expected_close_date` | `TIMESTAMP` | NULL | Ngày dự kiến đóng deal | 
| `source` | `VARCHAR(100)` | NULL | Nguồn cơ hội (Triển lãm, Gọi điện, KiotViet, Web) | 
| `note` | `TEXT` | NULL | Ghi chú chi tiết | 
| `deleted_at` | `TIMESTAMP` | NULL | Soft delete | 
| `created_at` | `TIMESTAMP` | NOT NULL | UTC | 
| `updated_at` | `TIMESTAMP` | NULL | UTC | 
| `row_version` | `BYTEA` | RowVersion | Concurrency control | 

#### 6. Bảng `stage_histories` (Lịch sử dịch chuyển phễu bán hàng)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `opportunity_id` | `BIGINT` | FK | Cơ hội liên quan | 
| `from_stage_id` | `INT` | FK | Giai đoạn trước | 
| `to_stage_id` | `INT` | FK | Giai đoạn mới | 
| `changed_by_user_id` | `VARCHAR(450)` | FK | Người kéo/chuyển giai đoạn | 
| `changed_at` | `TIMESTAMP` | NOT NULL | UTC | 

#### 7. Bảng `categories` (Danh mục sản phẩm)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `kiotviet_id` | `BIGINT` | UNIQUE | ID tương ứng trên KiotViet | 
| `name` | `VARCHAR(255)` | NOT NULL | Tên nhóm hàng | 

#### 8. Bảng `products` (Sản phẩm - Đồng bộ KiotViet)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `kiotviet_id` | `BIGINT` | UNIQUE | ID tương ứng KiotViet | 
| `code` | `VARCHAR(50)` | NOT NULL | Mã SKU sản phẩm | 
| `name` | `VARCHAR(255)` | NOT NULL | Tên sản phẩm | 
| `category_id` | `BIGINT` | FK | Thuộc danh mục nào | 
| `base_price` | `DECIMAL(18,2)` | DEFAULT 0 | Giá niêm yết | 
| `unit` | `VARCHAR(50)` | NULL | Đơn vị tính (Bộ, Cái, Tấn, Kg) | 
| `is_active` | `BOOLEAN` | DEFAULT TRUE | Trạng thái kinh doanh | 
| `updated_at` | `TIMESTAMP` | NOT NULL | UTC | 

#### 9. Bảng `quotes` (Báo giá B2B)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `code` | `VARCHAR(50)` | UNIQUE | Mã báo giá (VD: QUO-2026-089) | 
| `opportunity_id` | `BIGINT` | FK | Cơ hội sinh ra báo giá này | 
| `customer_id` | `BIGINT` | FK | Khách hàng nhận báo giá | 
| `created_by_user_id` | `VARCHAR(450)` | FK | Sales lập báo giá | 
| `status` | `INT` | DEFAULT 0 | Enum `QuoteStatus` (Draft=0, Sent=1, Accepted=2, Rejected=3, Expired=4) | 
| `sub_total` | `DECIMAL(18,2)` | DEFAULT 0 | Tiền hàng trước thuế | 
| `vat_rate` | `DECIMAL(5,2)` | DEFAULT 10 | Phần trăm thuế VAT (%) | 
| `vat_amount` | `DECIMAL(18,2)` | DEFAULT 0 | Tiền thuế VAT | 
| `total_amount` | `DECIMAL(18,2)` | DEFAULT 0 | Tổng thanh toán | 
| `valid_until` | `TIMESTAMP` | NOT NULL | Hạn hiệu lực của báo giá | 
| `note` | `TEXT` | NULL | Điều khoản thanh toán & giao hàng | 
| `created_at` | `TIMESTAMP` | NOT NULL | UTC | 

#### 10. Bảng `quote_items` (Chi tiết báo giá)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `quote_id` | `BIGINT` | FK | Thuộc báo giá | 
| `product_id` | `BIGINT` | FK | Sản phẩm chọn | 
| `quantity` | `DECIMAL(12,2)` | NOT NULL | Số lượng | 
| `unit_price` | `DECIMAL(18,2)` | NOT NULL | Đơn giá bán thực tế | 
| `discount_amount` | `DECIMAL(18,2)` | DEFAULT 0 | Chiết khấu trên dòng | 
| `total_line` | `DECIMAL(18,2)` | NOT NULL | Thành tiền (`quantity * unit_price - discount`) | 

#### 11. Bảng `sales_tasks` (Nhiệm vụ & Kế hoạch hành động)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `title` | `VARCHAR(255)` | NOT NULL | Tiêu đề công việc | 
| `description` | `TEXT` | NULL | Chi tiết nhiệm vụ | 
| `type` | `INT` | NOT NULL | Enum `TaskType` | 
| `priority` | `INT` | DEFAULT 1 | 0: Low, 1: Normal, 2: High, 3: Urgent | 
| `status` | `INT` | DEFAULT 0 | 0: Pending, 1: Completed, 2: Canceled | 
| `customer_id` | `BIGINT` | FK | Liên quan đến Khách hàng nào | 
| `contact_id` | `BIGINT` | FK (NULL) | Người liên hệ cụ thể | 
| `opportunity_id` | `BIGINT` | FK (NULL) | Cơ hội cụ thể | 
| `quote_id` | `BIGINT` | FK (NULL) | Báo giá cụ thể | 
| `due_date` | `TIMESTAMP` | NOT NULL | Mốc thời gian hạn chót (UTC) | 
| `completed_at` | `TIMESTAMP` | NULL | Thời gian thực tế hoàn thành | 
| `assigned_to_user_id` | `VARCHAR(450)` | FK | Sales thực hiện | 
| `completed_by` | `VARCHAR(450)` | FK (NULL) | Người bấm hoàn thành | 
| `outcome` | `INT` | NULL | Enum `TaskOutcome` (Kết quả cuộc gọi/gặp) | 
| `result_note` | `TEXT` | NULL | Lời nhắn / Ghi chú kết quả thu được | 
| `deleted_at` | `TIMESTAMP` | NULL | Soft delete | 
| `created_at` | `TIMESTAMP` | NOT NULL | UTC | 

#### 12. Bảng `interactions` (Lịch sử tương tác)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `customer_id` | `BIGINT` | FK | Khách hàng tương tác | 
| `contact_id` | `BIGINT` | FK (NULL) | Đối tượng tương tác | 
| `user_id` | `VARCHAR(450)` | FK | Sales thực hiện | 
| `type` | `INT` | NOT NULL | Enum `InteractionType` (Call, Meeting, Email, Zalo, Visit) | 
| `content` | `TEXT` | NOT NULL | Nội dung chi tiết cuộc trao đổi | 
| `interacted_at` | `TIMESTAMP` | NOT NULL | Thời điểm diễn ra (UTC) | 

#### 13. Bảng `notifications` (Thông báo / Cảnh báo Hệ thống)

| **Tên cột** | **Kiểu dữ liệu** | **Khóa** | **Mô tả** | 
| `id` | `BIGINT` | PK | Auto increment | 
| `user_id` | `VARCHAR(450)` | FK | User nhận thông báo | 
| `title` | `VARCHAR(255)` | NOT NULL | Tiêu đề thông báo | 
| `message` | `TEXT` | NOT NULL | Nội dung chi tiết | 
| `link_url` | `VARCHAR(500)` | NULL | Đường dẫn điều hướng khi click | 
| `is_read` | `BOOLEAN` | DEFAULT FALSE | Đã đọc hay chưa | 
| `created_at` | `TIMESTAMP` | NOT NULL | UTC | 

#### 14. Bảng `user_profiles` & `teams` (Mở rộng ASP.NET Identity)

* `teams`: `id`, `name`, `leader_user_id`, `created_at`.

* `user_profiles`: `user_id` (PK/FK `AspNetUsers`), `full_name`, `phone`, `team_id` (FK `teams`), `avatar_url`, `is_active`.

#### 15. Bảng `dashboard_snapshots` & `audit_logs`

* `dashboard_snapshots`: Lược đồ lưu trữ dữ liệu thống kê hằng ngày.

  * `id` (PK), `snapshot_date` (DATE), `period_from`, `period_to`, `total_revenue`, `new_customers`, `old_customers`, `total_opp_value`, `won_opportunities`, `total_opportunities`, `win_rate`, `customer_ratio_json`, `funnel_json`, `monthly_revenue_json`, `top_staff_json`, `alerts_json`, `created_at`.

* `audit_logs`: `id` (PK), `user_id`, `action` (INSERT/UPDATE/DELETE), `entity_name`, `entity_id`, `old_values_json`, `new_values_json`, `created_at`.

### 3.3 Danh mục Enums Hệ thống

```
namespace CrmApp.Data.Enums
{
    public enum CustomerHealth
    {
        New = 0,          // Khách hàng mới tạo/mới sync
        Healthy = 1,      // Đang mua hàng đều đặn đúng chu kỳ
        NeedAttention = 2,// Sắp đến hạn mua lại nhưng chưa phát sinh đơn
        AtRisk = 3,       // Quá chu kỳ mua hàng > 30 ngày
        Lost = 4,         // Mất khách (quá chu kỳ > 90 ngày)
        Dormant = 5       // Ngừng hoạt động / Không còn nhu cầu
    }

    public enum TaskType
    {
        AutoReminder = 1,   // Nhắc việc tự động do hệ thống sinh
        Appointment = 2,    // Lịch hẹn gặp trực tiếp
        FollowUp = 3,       // Gọi điện chăm sóc / hỏi thăm
        PaymentReminder = 5,// Nhắc thu hồi công nợ
        Manual = 6          // Công việc tự tạo
    }

    public enum TaskOutcome
    {
        CustomerWillBuy = 1, // Khách đồng ý mua hàng -> Tạo Opportunity
        NeedQuote = 2,       // Khách yêu cầu gửi báo giá -> Tạo Opp + Task "Gửi báo giá"
        AgreeMeeting = 3,    // Đồng ý gặp mặt -> Tạo Opp + Appointment
        CustomerBusy = 4,    // Khách bận -> Tạo Task gọi lại sau 2 ngày
        NoAnswer = 5,        // Không nghe máy -> Tạo Task gọi lại sau 1 ngày
        AlreadyBought = 6,   // Đã mua đối thủ -> Đánh dấu khách rủi ro
        Complaint = 7,       // Phàn nàn chất lượng -> Tạo Ticket + Notify Manager
        CareOnly = 8         // Chỉ hỏi thăm / chưa có nhu cầu ngay
    }

    public enum QuoteStatus
    {
        Draft = 0,    // Bản nháp
        Sent = 1,     // Đã gửi cho khách
        Accepted = 2, // Khách hàng chấp nhận
        Rejected = 3, // Bị từ chối
        Expired = 4   // Hết hạn hiệu lực
    }

    public enum InteractionType
    {
        Call = 1,    // Cuộc gọi
        Meeting = 2, // Gặp trực tiếp
        Email = 3,   // Email
        Zalo = 4,    // Tin nhắn Zalo
        Visit = 5    // Thăm nhà máy/kho
    }
}

```

## 4. CHI TIẾT CÁC MODULE VÀ LOGIC NGHIỆP VỤ

### 4.1 Module Khách hàng (Customers & Account 360)

* **View 360 Độ:** Khi chọn một Khách hàng tại `/Customers/Detail?id={id}`, giao diện hiển thị tổng thể:

  1. Thông tin chung & Mã MST/Địa chỉ.

  2. Chỉ số sức khỏe (`health_status`), Chu kỳ mua hàng trung bình (`average_cycle_days`).

  3. Lịch sử đơn hàng đồng bộ từ KiotViet (Mã đơn, tổng tiền, ngày mua).

  4. Lịch sử các cuộc tương tác (`interactions`).

  5. Các cơ hội bán hàng (`opportunities`) và Báo giá (`quotes`) đang xử lý.

  6. Lịch sử Sales phụ trách (`customer_assignments`).

* **Logic cập nhật tự động:**

  * Mỗi khi thêm mới một `interaction` thành công, tự động gán `customers.last_contact_date = UtcNow`.

  * Cron Job `UpdateCustomerHealthJob` sẽ chạy hàng đêm để tính toán lại `health_status` dựa trên `last_order_date` và `average_cycle_days`.

### 4.2 Module Cơ hội (Opportunities Pipeline)

* **Giao diện Kanban:** Đặt tại `/Opportunities/Kanban`. Hiển thị các cột tương ứng với các giai đoạn (`opportunity_stages`).

* **Kéo thả qua Blazor:**

  * Khi người dùng kéo thả card, Blazor kích hoạt sự kiện và gửi Command qua MediatR để xử lý đổi giai đoạn (Stage).

  * API xử lý:

    1. Cập nhật `stage_id` trong bảng `opportunities`.

    2. Thêm bản ghi mới vào `stage_histories`.

    3. Nếu chuyển sang stage "Thắng deal" (Won) -> Tự động gợi ý mở Modal tạo Báo giá (`Quote`).

    4. Nếu chuyển sang stage "Thua deal" (Lost) -> Yêu cầu nhập lý do bắt buộc và lưu vào `note`.

### 4.3 Module Báo giá (Quotes) & Xuất PDF

* **Tính toán tự động:**

  * `sub_total = SUM(quote_items.total_line)`

  * `vat_amount = sub_total * (vat_rate / 100)`

  * `total_amount = sub_total + vat_amount`

* **Tạo PDF xuất bản:** Sử dụng thư viện **QuestPDF** để biên dịch trực tiếp mã C# ra PDF vector chất lượng cao (gồm Logo công ty, bảng kê hàng hóa, phần ký tên và điều khoản thanh toán). Xuất qua endpoint `POST /api/quote/export-pdf/{id}`.

### 4.4 Module Kế hoạch hành động (Sales Tasks) & Luồng xử lý tự động

Giao diện mặc định tại `/Tasks/Index` ưu tiên hiển thị công việc cần hoàn thành trong ngày **Hôm nay**. Khi Sales mở Modal bấm **"Hoàn thành Task"**, họ chọn `TaskOutcome`. Hệ thống tự động kích hoạt luồng tự động (Workflow) dựa trên kết quả:

| **TaskOutcome** | **Luồng tự động sinh ra** | 
| **CustomerWillBuy** | Tạo Cơ hội bán hàng mới (`Opportunity`) ở Stage 1 với số tiền ước tính mặc định. | 
| **NeedQuote** | 1\. Tạo `Opportunity` ở Stage "Lên báo giá".  2\. Tạo Task mới: "Gửi báo giá cho khách" (Hạn chót: UtcNow + 24h). | 
| **AgreeMeeting** | 1\. Tạo `Opportunity`.  2\. Tạo Task loại `Appointment` với hạn chót do Sales chọn trên UI. | 
| **CustomerBusy** | Tự động tạo Task loại `FollowUp`: "Gọi lại cho khách" (Hạn chót: UtcNow + 2 ngày). | 
| **NoAnswer** | Tự động tạo Task loại `FollowUp`: "Gọi lại - Khách chưa nghe máy" (Hạn chót: UtcNow + 1 ngày). | 
| **AlreadyBought** | 1\. Đánh dấu `CustomerHealth` thành `AtRisk`.  2\. Ghi log tương tác và cập nhật lý do khách đã chọn đối thủ. | 
| **Complaint** | 1\. Tạo Ticket khiếu nại.  2\. Thêm bản ghi vào `notifications` tới tài khoản Trưởng phòng Sales (Manager). | 
| **CareOnly** | Chỉ lưu lại nội dung tương tác, không phát sinh Task hay Cơ hội mới. | 

### 4.5 Module Tích hợp KiotViet (KiotViet Sync Engine)

Hệ thống kết nối trực tiếp với API OAuth2 của KiotViet để duy trì nguồn sự thật dữ liệu:

| **Tác vụ Job (Hangfire)** | **Tần suất** | **Cơ chế đồng bộ** | **Dữ liệu chính xử lý** | 
| **SyncKhachHang** | 15 phút / lần | Delta Sync (`lastModifiedFrom`) | Đồng bộ Mã KH, Tên, SĐT, Địa chỉ, MST, Công nợ hiện tại. | 
| **SyncDonHang** | 15 phút / lần | Delta Sync (`createdDate`) | Đồng bộ Đơn bán hàng, Ngày bán gần nhất, Doanh thu phát sinh. | 
| **SyncSanPham** | 01 lần / ngày (00:30) | Full Sync | Cập nhật Mã SKU, Tên sản phẩm, Giá bán niêm yết, Danh mục. | 
| **SyncCongNo** | 01 lần / ngày (01:00) | Full Sync | Cập nhật tổng dư nợ và số ngày nợ quá hạn từ KiotViet. | 
| **TraStockRealtime** | On-Demand (API) | Direct Call | Tra cứu tồn kho chính xác theo từng kho hàng KiotViet khi tạo Báo giá. | 

**Quy tắc giải quyết xung đột (Conflict Resolution Rules):**

* **KiotViet** nắm quyền ưu tiên tuyệt đối (SSOT) đối với: Tên pháp nhân, MST, Số điện thoại chính, Lịch sử đơn hàng, Giá niêm yết và Tồn kho.

* **CRM System** nắm quyền ưu tiên cho: Trạng thái sức khỏe (`health_status`), Lịch sử chăm sóc, Cơ hội bán hàng, Ghi chú chi tiết, Sales phụ trách (`assigned_to_user_id`).

## 5. DASHBOARD & RÀNG BUỘC THỐNG NHẤT DỮ LIỆU

### 5.1 Kiến trúc Thống nhất Dữ liệu (Single Source of Truth Dashboard)

Dữ liệu Dashboard **KHÔNG KHUYẾN KHÍCH** truy vấn trực tiếp từ các bảng giao dịch realtime nhằm tránh hiện tượng lệch chỉ số giữa các widget và giảm tải DB. Hàng đêm hoặc khi Trigger chạy, `SnapshotDashboardJob` tính toán dữ liệu và kết xuất duy nhất vào một dòng trong bảng `dashboard_snapshots`. Tất cả các widget trên giao diện `/Dashboard/Index` đều truy xuất cùng một dòng snapshot này.

### 5.2 Công thức Định nghĩa 16 Chỉ số Chuẩn

| **#** | **Tên chỉ số** | **Công thức kỹ thuật** | **Nguồn dữ liệu** | **Mặc định kỳ** | 
| 1 | **Tổng doanh thu** | `SUM(kiotviet_orders.total_amount)` | KiotViet | 6 tháng gần nhất | 
| 2 | **Khách hàng mới** | `COUNT(customers WHERE min(order_date) IN period)` | KiotViet | 6 tháng gần nhất | 
| 3 | **Khách hàng cũ** | `COUNT(customers WHERE first_order < period.from AND order_date IN period)` | KiotViet | 6 tháng gần nhất | 
| 4 | **Tổng giá trị cơ hội** | `SUM(opportunities.estimated_value WHERE deleted_at IS NULL)` | CRM System | 6 tháng gần nhất | 
| 5 | **Số cơ hội mới** | `COUNT(opportunities WHERE created_at IN period)` | CRM System | 6 tháng gần nhất | 
| 6 | **Cơ hội thành công** | `COUNT(opportunities WHERE stage_id = WON_ID AND updated_at IN period)` | CRM System | 6 tháng gần nhất | 
| 7 | **Tỷ lệ thành công** | `(Cơ hội thành công / Tổng số cơ hội phát sinh) * 100` | CRM System | 6 tháng gần nhất | 
| 8 | **Tỷ lệ thắng deal** | `(Thắng / (Thắng + Thua)) * 100` | CRM System | 6 tháng gần nhất | 
| 9 | **Phễu tầng N** | `COUNT(DISTINCT opp_id WHERE reached_stage_id >= N)` | CRM (Cohort) | 6 tháng gần nhất | 
| 10 | **Tỷ lệ chuyển đổi N** | `(Số lượng Tầng N / Số lượng Tầng N-1) * 100` | CRM System | 6 tháng gần nhất | 
| 11 | **KH Mục tiêu** | `COUNT(customers WHERE order_count = 0 OR days_since_last_order < 30)` | CRM System | Hiện tại | 
| 12 | **KH Tiềm năng** | `COUNT(customers WHERE days_since_last_order BETWEEN 30 AND 90)` | CRM System | Hiện tại | 
| 13 | **KH Đang bán** | `COUNT(customers WHERE days_since_last_order < 30 AND order_count > 0)` | CRM System | Hiện tại | 
| 14 | **KH Lâu chưa mua** | `COUNT(customers WHERE days_since_last_order > 90)` | CRM System | Hiện tại | 
| 15 | **Doanh thu tháng** | `SUM(orders.total) GROUP BY DATE_TRUNC('month', order_date)` | KiotViet | 6 tháng gần nhất | 
| 16 | **Top 5 Nhân viên** | `SUM(orders.total) GROUP BY sales_user_id ORDER BY SUM DESC LIMIT 5` | KiotViet + CRM | 6 tháng gần nhất | 

### 5.3 Bộ 6 Ràng buộc Nhất quán (Consistency Constraints)

Mọi dữ liệu Snapshot khi xuất ra bắt buộc phải vượt qua bài kiểm tra của `ConsistencyCheckJob`. Nếu vi phạm bất kỳ ràng buộc nào, hệ thống sẽ log cảnh báo lỗi `CRITICAL` và từ chối cập nhật snapshot lên Dashboard:

* **Ràng buộc C1 (Tỷ lệ phân nhóm khách = 100%):** 

  $$
  \%Target + \%Potential + \%Active + \%Lost = 100.0\%
  $$

* **Ràng buộc C2 (Tính đơn điệu của phễu bán hàng):** 

  $$
  Count(Stage_N) \le Count(Stage_{N-1})
  $$

* **Ràng buộc C3 (Đồng nhất số liệu chốt deal):** 

  $$
  funnel["Chốt\_Thành\_Công"] == won\_opportunities
  $$

* **Ràng buộc C4 (Tổng doanh thu các tháng bằng tổng doanh thu đợt):** 

  $$
  \sum_{m=1}^{6} monthly\_revenue[m] == total\_revenue
  $$

* **Ràng buộc C5 (Doanh thu Top nhân viên không vượt tổng):** 

  $$
  \sum top\_5\_sales\_revenue \le total\_revenue
  $$

* **Ràng buộc C6 (Đầu vào phễu bằng số lượng cơ hội tạo mới):** 

  $$
  funnel["Tầng\_1\_Mới"] == total\_opportunities
  $$

### 5.4 Quy tắc Khung thời gian (Period Rules)

* **Hôm nay:** Từ `00:00:00` đến `23:59:59` theo múi giờ GMT+7.

* **Tuần này:** Từ `00:00:00` Thứ Hai đến `23:59:59` Chủ Nhật của tuần hiện tại.

* **Tháng này:** Từ `00:00:00` ngày 1 của tháng đến ngày cuối cùng trong tháng.

* **Quý này:** Ngày đầu tiên của tháng đầu Quý đến ngày cuối cùng của tháng cuối Quý.

* **6 tháng gần đây:** 6 tháng nguyên vẹn gần nhất tính đến hết tháng trước (hoặc tính tròn 180 ngày gần nhất).

### 5.5 Quy tắc Làm tròn & Trạng thái Rỗng (Empty States)

* **Quy tắc hiển thị:**

  * Tiền mặt: `Math.Round(val, 0)` -> Định dạng: `1.250.000.000` (UI) hoặc `1,25 tỷ` (Card tóm tắt).

  * Tỷ lệ phần trăm (%): `Math.Round(val, 1)` -> Định dạng: `68.5%`.

  * Tỷ lệ chuyển đổi (Ratio): `Math.Round(val, 2)` -> Định dạng: `1.45`.

* **Trạng thái rỗng (Empty State):**

  * Khi chỉ số tiền = 0: Hiển thị `"0 đ"` (màu xám), không để trống.

  * Khi chưa có dữ liệu biểu đồ/phễu: Hiển thị văn bản trung tâm `"Chưa có dữ liệu trong kỳ báo cáo"`.

## 6. DANH MỤC RESTFUL API & ENDPOINTS

Tất cả các API trả về kết quả dưới định dạng JSON chuẩn:

```
{
  "success": true,
  "data": { ... },
  "message": "Thao tác thành công",
  "errors": null
}

```

### Chi tiết API Endpoints

| **Method** | **Endpoint** | **Mục đích / Xử lý** | 
| `GET` | `/api/dashboard/kpi` | Trả về 4 KPI chính trên trang chủ. | 
| `GET` | `/api/dashboard/funnel` | Dữ liệu vẽ biểu đồ phễu bán hàng. | 
| `GET` | `/api/dashboard/customer-ratio` | Dữ liệu vẽ biểu đồ tròn cấu trúc khách hàng. | 
| `GET` | `/api/dashboard/revenue-monthly` | Biểu đồ cột doanh thu 6 tháng. | 
| `GET` | `/api/dashboard/top-staff` | Danh sách Top 5 sales xuất sắc. | 
| `GET` | `/api/dashboard/alerts` | Lấy danh sách thông báo cảnh báo công việc/khách quá hạn. | 
| `POST` | `/api/opportunity/move` | Kéo thả chuyển Stage Kanban (Body: `{ oppId, targetStageId }`). | 
| `POST` | `/api/task/complete` | Đánh dấu hoàn thành task & kích hoạt workflow tự động. | 
| `GET` | `/api/customer/search?q={query}` | Tìm kiếm nhanh khách hàng cho Auto-complete. | 
| `GET` | `/api/product/search?q={query}` | Tìm kiếm sản phẩm khi làm Báo giá. | 
| `POST` | `/api/quote/export-pdf/{id}` | Biên dịch và tải về file PDF Báo giá. | 
| `GET` | `/api/kiotviet/stock/{productId}` | Tra cứu tồn kho realtime qua API KiotViet. | 

## 7. MA TRẬN PHÂN QUYỀN (AUTHORIZATION MATRIX)

Hệ thống áp dụng Phân quyền dựa trên Tác vụ và Vai trò (RBAC):

| **Chức năng / Quyền hạn** | **Sales Staff** | **Sales Manager** | **Accountant** | **System Admin** | 
| Xem danh sách & Chi tiết Khách hàng của mình | ✅ | ✅ | ✅ | ✅ | 
| Xem Khách hàng của Sales khác | ❌ | ✅ | ✅ | ✅ | 
| Thêm mới / Cập nhật Khách hàng | ✅ (Thuộc về mình) | ✅ | ❌ | ✅ | 
| Điều chuyển Khách hàng cho Sales khác | ❌ | ✅ | ❌ | ✅ | 
| Tạo & Quản lý Cơ hội bán hàng | ✅ | ✅ | ❌ | ✅ | 
| Duyệt Báo giá (Approve Quote) | ❌ | ✅ | ❌ | ✅ | 
| Xem Báo cáo Dashboard Toàn công ty | ❌ | ✅ | ✅ | ✅ | 
| Xem Báo cáo Dashboard Cá nhân | ✅ | ✅ | ❌ | ✅ | 
| Đồng bộ dữ liệu thủ công từ KiotViet | ❌ | ❌ | ❌ | ✅ | 
| Quản trị Tài khoản & Phân quyền User | ❌ | ❌ | ❌ | ✅ | 

## 8. QUY CHUẨN MÃ NGUỒN & PHÁT TRIỂN (CODING CONVENTIONS)

1. **Kiến trúc Tách biệt:**

   * **Razor Pages (`CrmApp.Web`):** Tuyệt đối không chứa logic truy vấn DB direct hay logic tính toán tiền tệ. Chỉ injection `Services` và xử lý HTTP Context.

   * **CQRS & MediatR (`CrmApp.Business`):** Đóng gói toàn bộ Business Logic vào các Handlers. Các trang tương tác qua việc gửi Commands/Queries.

   * **Repository Layer (`CrmApp.Data`):** Chỉ làm nhiệm vụ truy xuất dữ liệu CRUD căn bản qua `AppDbContext`.

2. **Xử lý Bất đồng bộ (Async/Await):**

   * 100% các thao tác I/O (Database, Call API KiotViet, Export File) bắt buộc dùng async/await (`ToListAsync`, `SaveChangesAsync`,...).

3. **Audit Trail (Nhật ký Hệ thống):**

   * Override phương thức `SaveChangesAsync` trong `AppDbContext` để tự động ghi log thông tin người dùng, thời gian và các trường bị thay đổi vào bảng `audit_logs`.

4. **Xử lý An toàn Dữ liệu:**

   * **Soft Delete:** Các bảng quan trọng (`customers`, `opportunities`, `sales_tasks`) không dùng lệnh `DELETE` cứng. Dùng thuộc tính `deleted_at`, toàn bộ truy vấn mặc định áp dụng Global Query Filter: `.HasQueryFilter(x => x.DeletedAt == null)`.

   * **Xung đột ghi đồng thời (Optimistic Concurrency):** Sử dụng cột `row_version` dạng `byte[]` trong EF Core để ngăn ngừa ghi đè dữ liệu khi 2 người cùng sửa một bản ghi.

5. **Ghi Log Lỗi (Logging & Monitoring):**

   * Tích hợp **Serilog**. Toàn bộ Unhandled Exception được catch tại Middleware và ghi ra cả Console lẫn Log File định dạng JSON theo ngày tại folder `App_Data/Logs/`.

## 9. LỘ TRÌNH PHÁT TRIỂN & TIẾN ĐỘ THỰC HIỆN (8 TUẦN)

Tài nguyên: **01 Lập trình viên Full-stack (.NET)** | Ngân sách hạ tầng: **300.000 - 500.000 VNĐ / tháng (VPS Ubuntu)**

```
[Tuần 1] ──► [Tuần 2] ──► [Tuần 3] ──► [Tuần 4] ──► [Tuần 5] ──► [Tuần 6] ──► [Tuần 7] ──► [Tuần 8]
 Setup DB      Customer      Product       Kanban        Quotes        Tasks &       Dashboard     KiotViet &
 & Frame work   & Contact    & Opp List    Blazor UI       & PDF         Hangfire      Snapshots     Deploy Prod

```

### Chi tiết Công việc Theo Tuần

* **Tuần 1: Setup Khung Dự án & Database**

  * Tạo Solution 3 lớp `.NET 10`, cấu hình Dependency Injection.

  * Viết script SQL tạo 15 bảng DB PostgreSQL, chạy `dbcontext scaffold` tạo Entity.

  * Cấu hình Identity Authentication & Authorization Base.

* **Tuần 2: Module Khách hàng & Người liên hệ**

  * Xây dựng giao diện danh sách, tìm kiếm, lọc Khách hàng.

  * Xây dựng trang Account 360 View (`/Customers/Detail`).

  * Chức năng CRUD Khách hàng & Thêm nhanh Liên hệ (Contact).

* **Tuần 3: Module Sản phẩm & Danh sách Cơ hội**

  * Import danh mục sản phẩm mẫu.

  * Xây dựng CRUD Cơ hội bán hàng (`Opportunities/List`, `Create`, `Detail`).

  * Viết Service tính toán giá trị và tỷ lệ thành công của deal.

* **Tuần 4: Kanban Board & Blazor Integration**

  * Xây dựng giao diện Kanban với MudBlazor.

  * Đấu nối Blazor UI cho thao tác kéo thả đổi Stage.

  * Viết API `POST /api/opportunity/move` xử lý ghi lịch sử chuyển giai đoạn.

* **Tuần 5: Module Báo giá (Quotes) & Xuất File PDF**

  * Tạo giao diện lập Báo giá từ Cơ hội bán hàng.

  * Tự động tính toán VAT, Chiết khấu và Tổng tiền.

  * Thiết kế Template và tích hợp **QuestPDF** xuất file PDF tải về.

* **Tuần 6: Nhiệm vụ Bán hàng (Tasks) & Hangfire Jobs**

  * Tích hợp Hangfire Dashboard (`/hangfire`).

  * Xây dựng giao diện quản lý Task hàng ngày (`/Tasks/Index`).

  * Cấu hình luồng tự động sinh Task mới dựa theo `TaskOutcome`.

* **Tuần 7: Dashboard Thống nhất & Snapshot Engine**

  * Xây dựng màn hình Dashboard 2 khu vực.

  * Tạo Job `SnapshotDashboardJob` tính toán dữ liệu hằng đêm.

  * Viết `ConsistencyCheckJob` kiểm tra 6 ràng buộc dữ liệu.

* **Tuần 8: Tích hợp KiotViet API, Kiểm thử & Triển khai (Production)**

  * Tích hợp KiotViet Sync Service (Đồng bộ KH, Đơn hàng, Tồn kho).

  * Kiểm thử toàn bộ hệ thống (End-to-End Testing) & Phân quyền RBAC.

  * Triển khai VPS Ubuntu với Nginx Reverse Proxy, Kestrel service & Docker PostgreSQL.

**KẾT LUẬN:**
Tài liệu đặc tả kỹ thuật này là căn cứ chuẩn mực để triển khai dự án **CRM B2B**. Mọi thành phần từ Kiến trúc, Cơ sở dữ liệu, Logic nghiệp vụ tự động đến Quy tắc thống nhất dữ liệu trên Dashboard đã được thiết kế hoàn chỉnh, chặt chẽ và sẵn sàng cho việc lập trình.