# TÀI LIỆU KỸ THUẬT THỰC THI DỰ ÁN CRM (AGILE/SCRUM SPRINT MASTER PLAN)

* **Phiên bản:** 2.0 — Agile Sprint Execution Standard
* **Quy mô:** 7 Sprints · 2 tuần/Sprint · 5 ngày/tuần · 2 giờ/ngày = **140 giờ chuẩn**
* **Mô hình nhân sự:** 1 Solo Full-stack .NET Developer (đảm nhiệm vai trò Product Owner kỹ thuật & Scrum Dev)
* **Mục tiêu:** Hệ thống CRM vận hành ổn định trên Production, đồng bộ KiotViet hai chiều, tự động hóa luồng bán hàng và không phát sinh nợ kỹ thuật (Zero-Technical-Debt).

---

## PHẦN 1: BẢNG TỔNG QUAN LỘ TRÌNH 7 SPRINTS (AGILE ROADMAP)

| Sprint | Thời lượng | Trọng tâm & Mục tiêu chính (Sprint Goal) | Output bàn giao (Shippable Increment) |
| :---: | :---: | :--- | :--- |
| **Sprint 1** | Tuần 1 - 2 (20h) | **Foundation & Customer 360 Core:** Dựng khung kiến trúc 4-tier .NET 8, PostgreSQL, Auth Identity, hoàn thiện Customer 360 (CRUD, Danh bạ, Timeline). | Solution chạy mượt; module Khách hàng hoạt động end-to-end với HTMX. |
| **Sprint 2** | Tuần 3 - 4 (20h) | **Pipeline Engine & Drag-and-Drop Kanban:** Import/Export Excel và hoàn thiện bảng Kanban kéo thả thời gian thực (SortableJS) có cơ chế Rollback. | Kéo thả chuyển giai đoạn cơ hội mượt mà; Import/Export dữ liệu Excel ổn định. |
| **Sprint 3** | Tuần 5 - 6 (20h) | **Quotation Engine (PDF/Email) & Sales Tasks:** Lập báo giá động, render QuestPDF tiếng Việt, gửi email đính kèm và máy trạng thái quản lý kế hoạch hành động. | Xuất PDF báo giá chuẩn in ấn; gửi email tự động; phân loại đóng task thông minh. |
| **Sprint 4** | Tuần 7 - 8 (20h) | **Hangfire Automation & Reporting Engine:** Tác vụ nền đánh giá sức khỏe khách hàng, thuật toán phễu Cohort và chốt chặn kiểm toán nhất quán số liệu. | 4 Background Jobs chạy tự động hàng đêm; Snapshot số liệu báo cáo tải $< 10ms$. |
| **Sprint 5** | Tuần 9 - 10 (20h) | **Executive Dashboard & KiotViet Core Sync:** Trực quan hóa Chart.js đa chiều và kết nối OAuth 2.0 đồng bộ 2 chiều Khách hàng, Đơn hàng, Sản phẩm KiotViet. | Dashboard tải $< 300ms$; Job đồng bộ KiotViet định kỳ 15 phút/lần chống nghẽn. |
| **Sprint 6** | Tuần 11 - 12 (20h) | **Advanced Sync, Audit Trail & Security Hardening:** Đồng bộ công nợ, cache tra tồn kho, EF Core Audit Log, bảo mật OWASP, backup tự động và Telegram Alert. | Audit trail 100% thay đổi; bảo mật HTTPS/Rate Limiting; cảnh báo tức thì qua Bot. |
| **Sprint 7** | Tuần 13 - 14 (20h) | **Production Deployment, E2E Trial & Handover:** Triển khai VPS Ubuntu (Nginx, SSL), nạp dữ liệu thực tế, đào tạo Sales/Ban giám đốc và nghiệm thu bàn giao. | Hệ thống Production chạy thật ổn định; đầy đủ tài liệu HDSD và biên bản nghiệm thu. |

---

## PHẦN 2: KIẾN TRÚC HỆ THỐNG & NGUYÊN TẮC VẬN HÀNH SPRINT

### 1. Technology Stack
* **Backend:** .NET 8 LTS (ASP.NET Core Razor Pages / MVC + Web API)
* **Frontend tương tác:** HTMX (AJAX động không cần SPA framework cồng kềnh) + SortableJS (Kanban kéo thả) + Bootstrap 5.3 & Bootstrap Icons
* **Database & ORM:** PostgreSQL 16 + Entity Framework Core 8 (Npgsql)
* **Xử lý nền (Background Worker):** Hangfire với PostgreSQL Storage
* **Thư viện chuyên dụng:**
  * Xuất PDF: `QuestPDF`
  * Xử lý bảng tính: `ClosedXML`
  * Email engine: `MailKit` & `MimeKit`
  * Ghi vết & Logging: `Serilog` (Sinks: File rolling daily, PostgreSQL Sink cho Fatal)
* **Tích hợp bên ngoài:** KiotViet API (OAuth 2.0 Client Credentials Flow, Polling Worker)

### 2. Cấu trúc Solution (4-Tier Architecture)
```text
CrmSolution/
├── src/
│   ├── Crm.Domain/              # POCO Entities, Enums, Value Objects, Domain Exceptions
│   ├── Crm.Data/                # AppDbContext, Configurations, Migrations, SeedData
│   ├── Crm.Business/            # DTOs, Services, Interfaces, FluentValidation, Business Logic
│   ├── Crm.Jobs/                # Hangfire Recurring Jobs, Background Sync Workers
│   └── Crm.Web/                 # Razor Pages, Controllers, ViewModels, Middlewares, wwwroot
├── tests/
│   └── Crm.Tests/               # xUnit, Moq, FluentAssertions, TestContainers
├── docs/
│   ├── db/schema.sql            # Script khởi tạo DDL chuẩn
│   └── postman/                 # Bộ API Collections
├── scripts/
│   ├── backup.sh                # Backup pg_dump tự động
│   └── deploy.sh                # Automation script publish & reload systemd
├── .gitignore
├── README.md
└── CrmSolution.sln
```

### 3. Quy tắc "Khung giờ vàng 120 phút" trong Sprint
Mỗi ngày làm việc trong Sprint là một Time-boxed 120 phút nghiêm ngặt:
* **00:00 - 00:15 (15p — Daily Standup & Setup):** Rà soát mục tiêu ngày, kiểm tra migration/database, chuẩn bị test case.
* **00:15 - 01:45 (90p — Focused Sprint Execution):** Lập trình tập trung cao độ, tuân thủ SOLID, không hard-code, không copy-paste không kiểm soát.
* **01:45 - 02:00 (15p — Test, Log & Push):** Chạy Unit Test, format code, ghi log Serilog, commit Git (Conventional Commits) và đẩy lên remote branch.

---

## PHẦN 3: KẾ HOẠCH THỰC THI CHI TIẾT THEO 7 SPRINTS (RUNBOOK 70 NGÀY)

---

### SPRINT 1: NỀN TẢNG KỸ THUẬT & KHÁCH HÀNG 360 (TUẦN 1 - 2)
* 🎯 **Sprint Goal:** Xây dựng hoàn chỉnh bộ khung kỹ thuật 4-tier .NET 8, cơ chế HTMX, Authentication phân quyền và quản lý toàn diện thông tin Khách hàng (Account 360).
* ⏱ **Thời lượng:** 10 ngày làm việc (Ngày 1 $\rightarrow$ 10) = 20 giờ.

#### Ngày 1 (Thứ 2) — Setup Solution 4-Tier & Proof-of-Concept HTMX
* 🎯 **Mục tiêu:** Dựng skeleton project chuẩn, thiết lập package, chạy thử cơ chế Razor Partial qua HTMX.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (15p) Kiểm tra môi trường: .NET 8 SDK, PostgreSQL 16 local, pgAdmin/DBeaver.
  * (30p) Khởi tạo Solution bằng .NET CLI:
    ```bash
    dotnet new sln -n CrmSolution
    dotnet new classlib -o src/Crm.Domain
    dotnet new classlib -o src/Crm.Data
    dotnet new classlib -o src/Crm.Business
    dotnet new classlib -o src/Crm.Jobs
    dotnet new webapp -o src/Crm.Web
    dotnet new xunit -o tests/Crm.Tests
    # Reference setup
    dotnet sln add src/*/*.csproj tests/*/*.csproj
    dotnet add src/Crm.Data reference src/Crm.Domain
    dotnet add src/Crm.Business reference src/Crm.Data src/Crm.Domain
    dotnet add src/Crm.Jobs reference src/Crm.Business
    dotnet add src/Crm.Web reference src/Crm.Business src/Crm.Jobs
    dotnet add tests/Crm.Tests reference src/Crm.Business
    ```
  * (25p) Cài đặt NuGet Packages chủ chốt (`Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `Serilog.AspNetCore`).
  * (30p) Thêm thư viện `htmx.org@1.9.10` vào `_Layout.cshtml`. Tạo route `/demo-htmx` và Action trả về `PartialView("_DemoMessage.cshtml")` bằng thuộc tính `hx-get="/demo-htmx" hx-target="#result"`.
  * (20p) Test chạy `dotnet run`, click nút không reload trang, commit git init.
* 📦 **Output:** Solution build thành công; demo HTMX phản hồi HTTP 200 kèm partial HTML.
* ✅ **DoD:** `localhost:5000/demo-htmx` hoạt động mượt mà; Git remote repo đã khởi tạo.

#### Ngày 2 (Thứ 3) — Khởi tạo Schema DDL & Thiết kế PostgreSQL
* 🎯 **Mục tiêu:** Soạn thảo hoàn chỉnh file `schema.sql` với toàn bộ khóa ngoại, ràng buộc và seed danh mục cơ sở.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (15p) Tạo file `docs/db/schema.sql`.
  * (80p) Viết DDL chuẩn Postgres:
    * Tạo schema, UUID extension (`uuid-ossp`).
    * 15 bảng theo thiết kế, ép kiểu dữ liệu chuẩn: `timestamptz` cho toàn bộ timestamp, `numeric(18,2)` cho tiền tệ, `jsonb` cho payload.
    * Tạo chỉ mục: `idx_customers_health`, `idx_customers_code`, `idx_opportunities_stage`, `idx_tasks_due_date`.
    * Cấu hình trigger cập nhật tự động trường `updated_at`.
  * (25p) Thực thi `psql -U postgres -d crm_db -f docs/db/schema.sql`, verify quan hệ ERD.
* 📦 **Output:** File `docs/db/schema.sql` hoàn thiện, chạy thực tế trên DB không lỗi syntax.
* ✅ **DoD:** Toàn bộ 15 bảng hiển thị đúng quan hệ 1-N, N-N trên công cụ quản trị database.

#### Ngày 3 (Thứ 4) — Cấu hình Authentication & Phân quyền Identity
* 🎯 **Mục tiêu:** Cài đặt Identity sử dụng PostgreSQL, hoàn thiện trang Login/Logout, Seed Role & Phân quyền Global.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Tích hợp `Microsoft.AspNetCore.Identity.EntityFrameworkCore`. Kế thừa `IdentityUser<Guid>` thành `ApplicationUser` bổ sung trường `FullName`, `IsActive`.
  * (30p) Đăng ký Identity trong `Program.cs`, thiết lập Password Policy, Cookie Authentication với `ExpireTimeSpan` là 8 tiếng.
  * (30p) Thiết kế Razor Page `Pages/Account/Login.cshtml` và `Logout.cshtml` giao diện Bootstrap tối giản.
  * (20p) Viết hàm seed tự động: 4 Roles (`ADMIN`, `MANAGER`, `SALES`, `ACCOUNTANT`) và 1 user `admin@crm.local` mặc định.
  * (10p) Gán AuthorizeFilter toàn cục: bắt buộc đăng nhập trừ các trang Account.
* 📦 **Output:** Module Auth hoàn chỉnh; cơ chế session cookie hoạt động an toàn.
* ✅ **DoD:** Chưa login bị redirect về `/Account/Login`. Đăng nhập sai báo lỗi; đăng nhập đúng vào màn hình chính.

#### Ngày 4 (Thứ 5) — Scaffold Entity, Cấu hình DbContext & Seed Data Mẫu
* 🎯 **Mục tiêu:** Đồng bộ Entity Framework Core với Schema database và tạo bộ dữ liệu seed chuẩn phục vụ test.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (20p) Cấu hình Fluent API trong `AppDbContext` (thư mục `Crm.Data`), kiểm tra quan hệ `OnModelCreating`.
  * (30p) Thiết lập Converter toàn cục cho Entity Framework để lưu `DateTime` thành UTC chuẩn hóa.
  * (50p) Viết class `DbInitializer.cs`:
    * Seed 20 Khách hàng (đa dạng trạng thái Health: Good, Fair, AtRisk, Churned).
    * Seed 30 Liên hệ (gắn với các công ty).
    * Seed 20 Sản phẩm có đơn giá khác nhau.
    * Seed 15 Cơ hội bán hàng phân bổ đều 6 Stages.
    * Seed 30 Tasks với trạng thái Pending, Completed.
  * (20p) Chạy Migration hoặc Database Update, khởi chạy app để kích hoạt `DbInitializer`.
* 📦 **Output:** Bộ Entity trong `Crm.Domain` sạch đẹp; Database chứa dữ liệu demo phong phú.
* ✅ **DoD:** Truy vấn `_context.Customers.Count()` trả về $\ge 20$, không lỗi timezone.

#### Ngày 5 (Thứ 6) — Enums, Helpers, Master Layout & Serilog
* 🎯 **Mục tiêu:** Xây dựng base layout Bootstrap chuẩn responsive, định nghĩa toàn bộ Value Objects/Enums và Logging.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (20p) Khai báo các Enum: `CustomerHealth`, `TaskType`, `TaskOutcome`, `QuoteStatus`, `InteractionType`.
  * (25p) Viết `CurrencyHelper` (`1250000` $\rightarrow$ "1,25 triệu"), `DateHelper` tính khoảng cách ngày.
  * (45p) Hoàn thiện Master Layout `_Layout.cshtml`: Tích hợp Sidebar (`_Sidebar.cshtml`), Header hiển thị user info, nhúng Bootstrap 5.3, Bootstrap Icons, HTMX library, NProgress bar.
  * (30p) Cấu hình Serilog trong `Program.cs`: Rolling log ra `logs/crm-.log` (10MB/file), bật `UseSerilogRequestLogging()`.
* 📦 **Output:** Giao diện Dashboard khung hoàn chỉnh, ghi log hệ thống tự động theo ngày.
* ✅ **DoD:** App hiển thị responsive mobile/desktop; kiểm tra file `logs/crm-yyyyMMdd.log` có ghi nhận log đăng nhập.

#### Ngày 6 (Thứ 2) — Repository & CustomerService Core
* 🎯 **Mục tiêu:** Xây dựng tầng truy vấn dữ liệu và logic nghiệp vụ lõi cho Quản lý khách hàng.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Thiết kế `ICustomerRepository` và `CustomerRepository` sử dụng IQueryable hỗ trợ phân trang và filter động.
  * (55p) Viết `CustomerService`:
    * `GetListAsync(CustomerFilterDto filter)`: Lọc theo sức khỏe, sales phụ trách, tìm kiếm từ khóa.
    * `CreateAsync(CustomerCreateDto dto)`: Tự động phát sinh mã khách theo quy tắc `KH-0001` (dùng Sequence hoặc Lock tránh trùng).
    * `UpdateAsync(CustomerUpdateDto dto)`: Check Concurrency qua trường `RowVersion` (xmin trong PostgreSQL).
  * (25p) Viết 3 Unit Tests trong `Crm.Tests`: Test sinh mã tự tăng, test validate số điện thoại, test filter.
  * (15p) Đăng ký DI trong `Program.cs` và kiểm tra pass test.
* 📦 **Output:** `ICustomerService` và `CustomerService` hoàn chỉnh, sẵn sàng phục vụ Presentation.
* ✅ **DoD:** Unit tests màu xanh (100% pass), không phát sinh cảnh báo compile.

#### Ngày 7 (Thứ 3) — Màn hình Danh sách Khách hàng (`/Customers/Index`)
* 🎯 **Mục tiêu:** Xây dựng giao diện danh sách kết hợp Server-side Filtering, Badges trạng thái sức khỏe.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (20p) Thiết kế `CustomerListViewModel` hỗ trợ thông tin phân trang (PageIndex, PageSize, TotalCount).
  * (60p) Xây dựng `Pages/Customers/Index.cshtml`:
    * Thanh công cụ lọc: Dropdown chọn Health Status, Dropdown chọn Sales, ô Search hỗ trợ HTMX debounce 300ms.
    * Bảng danh sách: Mã, Tên công ty, Người liên hệ chính, Badge Health (Xanh: Tốt, Vàng: Cảnh báo, Đỏ: Nguy cơ), Doanh thu 90 ngày, Thao tác.
  * (30p) Viết Code-behind `Index.cshtml.cs` kết nối với `CustomerService`.
  * (10p) Kiểm thử hiển thị danh sách với 20 record mẫu từ Tuần 1.
* 📦 **Output:** Trang danh sách khách hàng tải nhanh, phân trang và tìm kiếm realtime mượt mà.
* ✅ **DoD:** Gõ từ khóa tìm kiếm trên giao diện, dữ liệu lọc chính xác mà không reload toàn trang.

#### Ngày 8 (Thứ 4) — Trang Chi tiết Khách hàng 360 (Tab 1: Tổng quan & Tab 2: Danh bạ)
* 🎯 **Mục tiêu:** Xây dựng màn hình Customer 360 độ gồm thông tin tổng quan công ty và danh sách liên hệ.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Thiết kế `CustomerDetailViewModel` bao gồm thông tin tài chính, công nợ, chu kỳ tiêu thụ.
  * (45p) Tạo `Pages/Customers/Detail.cshtml` chia cấu trúc Tab (Nav-tabs Bootstrap):
    * Tab 1: Form hiển thị thông tin pháp nhân, mã số thuế, địa chỉ, người quản lý phụ trách.
    * Tab 2: Danh sách đầu mối liên hệ (Họ tên, Chức vụ, Số điện thoại, Email, Là liên hệ chính).
  * (35p) Dùng HTMX mở Modal thêm mới liên hệ trực tiếp tại Tab 2 (`hx-target="#modal-container"`).
  * (15p) Kiểm tra lưu thêm mới contact thành công và tự động append vào bảng danh bạ.
* 📦 **Output:** Giao diện Customer 360 tải thông tin 2 tabs đầu, thêm contact không reload.
* ✅ **DoD:** Thêm một liên hệ mới trong modal, bảng contact bên dưới tự reload dữ liệu mới.

#### Ngày 9 (Thứ 5) — Hoàn thiện Customer 360 (Tab 3: Cơ hội & Tab 4: Tương tác)
* 🎯 **Mục tiêu:** Hoàn thiện 2 tabs còn lại: Lịch sử deal bán lẻ/sỉ và dòng thời gian tương tác (Timeline).
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Tab 3 (Cơ hội): Hiển thị bảng các pipeline deal của khách hàng (Tên deal, Giá trị dự kiến, Giai đoạn, Ngày dự kiến đóng).
  * (45p) Tab 4 (Dòng thời gian tương tác): Hiển thị timeline xếp theo thời gian mới nhất; form ghi nhanh tương tác (`InteractionType`, `Notes`, `NextActionDate`) qua HTMX post.
  * (20p) Logic nghiệp vụ: Ghi nhận tương tác mới $\rightarrow$ tự động cập nhật trường `LastContactDate` của khách hàng.
  * (10p) Test tổng thể toàn bộ 4 tabs màn hình Detail.
* 📦 **Output:** Màn hình 360 độ hoàn chỉnh, cung cấp góc nhìn toàn diện về khách hàng.
* ✅ **DoD:** Bấm ghi tương tác mới $\rightarrow$ Timeline cập nhật $\rightarrow$ Ngày liên hệ cuối thay đổi ngay lập tức.

#### Ngày 10 (Thứ 6) — CRUD Khách hàng, Soft Delete & Sprint 1 Review
* 🎯 **Mục tiêu:** Xây dựng Form tạo mới, chỉnh sửa thông tin, cơ chế xóa mềm an toàn và tổng kết Sprint 1.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Tạo `CustomerFormViewModel` kết hợp validation attributes (MST, Email, SĐT).
  * (35p) Tạo Partial View `_CustomerForm.cshtml` tái sử dụng cho `Create.cshtml` và `Edit.cshtml`.
  * (30p) Cấu hình Soft Delete: Thêm `IsDeleted`, `DeletedAt`; cấu hình Global Query Filter EF Core `.HasQueryFilter(e => !e.IsDeleted)`.
  * (30p) **Sprint 1 Review & Refactor:** Rà soát toàn bộ code tuần 1 & 2, chạy test suite, kiểm tra tính toàn vẹn của dữ liệu khách hàng.
* 📦 **Output:** Bộ chức năng Thêm - Sửa - Xóa mềm khách hàng an toàn; bản build Sprint 1 sạch sẽ.
* ✅ **DoD:** Xóa 1 khách hàng thành công, DB chuyển `is_deleted = true`, màn hình Index ẩn đi; Unit tests pass 100%.

---

### SPRINT 2: IMPORT/EXPORT DỮ LIỆU & PIPELINE KANBAN (TUẦN 3 - 4)
* 🎯 **Sprint Goal:** Hoàn thiện công cụ nạp/xuất dữ liệu Excel chuyên nghiệp và xây dựng bảng Kanban kéo thả cơ hội kinh doanh thời gian thực với cơ chế Rollback chống lỗi.
* ⏱ **Thời lượng:** 10 ngày làm việc (Ngày 11 $\rightarrow$ 20) = 20 giờ.

#### Ngày 11 (Thứ 2) — Import Khách hàng Hàng loạt từ Excel
* 🎯 **Mục tiêu:** Xử lý đọc file Excel dung lượng lớn, validate từng dòng dữ liệu và nạp vào DB.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (20p) Thêm thư viện `ClosedXML` vào project `Crm.Business`.
  * (65p) Xây dựng `ExcelImportService`: Đọc WorkSheet đầu tiên, bỏ qua header; validate từng hàng (bắt buộc tên công ty, định dạng SĐT); gom lỗi (Error Collection); bulk insert tối ưu hiệu năng.
  * (25p) Thiết kế modal Upload file tại `/Customers/Index` hiển thị tiến trình và bảng danh sách lỗi.
  * (10p) Kiểm thử với file Excel 50 khách hàng chuẩn bị sẵn.
* 📦 **Output:** Tính năng Import Excel ổn định, có báo cáo chi tiết các bản ghi không đạt chuẩn.
* ✅ **DoD:** Upload file mẫu 50 dòng: 48 dòng đúng được nạp vào DB, 2 dòng sai báo lỗi chi tiết ra UI.

#### Ngày 12 (Thứ 3) — Export Dữ liệu Excel & Migration Dữ liệu Thực tế
* 🎯 **Mục tiêu:** Xuất danh sách khách hàng ra Excel định dạng đẹp và làm sạch dữ liệu cũ của doanh nghiệp.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (35p) Viết hàm `ExportCustomersAsync()` trong `ExcelExportService`: Header màu nền, in đậm, auto fit cột, format tiền tệ và ngày tháng VN.
  * (55p) Làm sạch (Data Cleansing) dữ liệu cũ từ Excel doanh nghiệp: script console chuẩn hóa SĐT 10 số, lọc trùng MST.
  * (30p) Nạp dữ liệu thực tế vào DB môi trường Dev, kiểm tra số lượng khớp 100%.
* 📦 **Output:** Nút Export trả về file `.xlsx` chuyên nghiệp; dữ liệu thực tế nạp an toàn.
* ✅ **DoD:** File Excel tải về mở trên MS Excel chuẩn form; dữ liệu công ty hiển thị đúng trên app.

#### Ngày 13 (Thứ 4) — Repository & OpportunityService Core
* 🎯 **Mục tiêu:** Xây dựng logic quản trị Pipeline bán hàng, chuyển đổi trạng thái và ghi nhận lịch sử.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Thiết kế `IOpportunityRepository` với các query chuyên biệt cho Pipeline và Kanban.
  * (55p) Xây dựng `OpportunityService`:
    * `CreateAsync`: Sinh mã `OPP-0001`, thiết lập stage ban đầu, ghi record vào `opportunity_stage_histories`.
    * `MoveStageAsync(Guid oppId, int newStageId)`: Cập nhật stage, tính thời gian ở stage cũ, ghi log vết lịch sử.
    * `MarkWonAsync` / `MarkLostAsync`: Đóng deal, cập nhật lý do hoặc chuyển trạng thái sức khỏe khách hàng.
  * (25p) Viết 4 Unit Tests: Kiểm tra chuyển stage hợp lệ, kiểm tra logic khóa chuyển stage khi deal đã đóng.
  * (10p) Verify kết quả chạy test.
* 📦 **Output:** Service quản lý cơ hội với đầy đủ ràng buộc toàn vẹn lịch sử.
* ✅ **DoD:** Gọi `MoveStageAsync` $\rightarrow$ Bảng `opportunity_stage_histories` sinh bản ghi lưu vết chính xác thời điểm.

#### Ngày 14 (Thứ 5) — Màn hình Danh sách Cơ hội Dạng Bảng (`/Opportunities/List`)
* 🎯 **Mục tiêu:** Xây dựng bảng hiển thị cơ hội bán hàng với bộ lọc đa chiều và sắp xếp theo xác suất chốt.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Xây dựng `OpportunityListViewModel` (Mã, Tên deal, Khách hàng, Giá trị dự tính, Giai đoạn, Sales phụ trách, Ngày đóng dự kiến).
  * (55p) Viết giao diện `Pages/Opportunities/List.cshtml`: Badge màu sắc tương ứng 6 Stages; bộ lọc theo Stage, Nhân viên, Khoảng ngày dự kiến chốt.
  * (30p) Viết Code-behind `List.cshtml.cs`, tích hợp Query parameters phục vụ bookmarkable URL.
  * (10p) Kiểm tra hiển thị với 15 cơ hội đã seed.
* 📦 **Output:** Trang danh sách cơ hội hiển thị rõ ràng, dễ phân loại và tra cứu.
* ✅ **DoD:** Lọc theo stage "Đàm phán" trả về chính xác danh sách deal và hiển thị tổng giá trị pipeline ở chân trang.

#### Ngày 15 (Thứ 6) — Form Tạo mới Cơ hội Tích hợp Tìm kiếm HTMX
* 🎯 **Mục tiêu:** Xây dựng form nhập liệu deal thông minh, tìm kiếm nhanh khách hàng không cần tải lại trang.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Thiết kế `OpportunityFormViewModel` với validation chi tiết.
  * (50p) Xây dựng trang `Create.cshtml`:
    * Ô tìm kiếm Khách hàng thông minh: Sử dụng `hx-get="/api/customers/search"` với thuộc tính `hx-trigger="keyup changed delay:300ms"` gợi ý dropdown.
    * Khi chọn khách hàng: Dropdown "Người liên hệ" tự động kích hoạt HTMX để load danh sách contacts tương ứng.
    * Nhập giá trị tiền tệ có định dạng phân cách hàng nghìn realtime bằng JS đơn giản.
  * (35p) Viết xử lý POST submit, lưu thông tin và điều hướng về trang chi tiết cơ hội.
  * (10p) Test kịch bản tạo cơ hội mới từ đầu.
* 📦 **Output:** Form tạo cơ hội mượt mà, trải nghiệm người dùng cao cấp nhờ HTMX.
* ✅ **DoD:** Gõ tên khách hàng hiển thị gợi ý $\rightarrow$ Chọn khách $\rightarrow$ Dropdown liên hệ tự động nạp danh sách chuẩn.

#### Ngày 16 (Thứ 2) — Chi tiết Cơ hội & Dòng thời gian Pipeline (`/Opportunities/Detail`)
* 🎯 **Mục tiêu:** Xây dựng màn hình chi tiết deal với Timeline trực quan hóa quá trình dịch chuyển giữa các giai đoạn.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Xây dựng `OpportunityDetailViewModel` kèm dữ liệu lịch sử chuyển stage.
  * (55p) Viết `Detail.cshtml`: Step-wizard nằm ngang thể hiện giai đoạn hiện tại; khối thông tin trung tâm (Khách hàng, Giá trị, Xác suất win, Doanh số kỳ vọng); nút hành động nhanh (Won, Lost, Tạo báo giá).
  * (30p) Xử lý logic đóng deal (Won/Lost) với Modal ghi nhận lý do nếu thất bại.
  * (10p) Kiểm tra cập nhật dữ liệu.
* 📦 **Output:** Màn hình chi tiết cơ hội bán hàng chuyên nghiệp, hỗ trợ ra quyết định nhanh.
* ✅ **DoD:** Bấm nút "Won" $\rightarrow$ Deal chuyển sang xanh lá cây, các nút hành động khóa lại, hệ thống ghi log hoàn tất.

#### Ngày 17 (Thứ 3) — Khung Màn hình Kanban Cơ hội (`/Opportunities/Kanban`)
* 🎯 **Mục tiêu:** Xây dựng cấu trúc hiển thị 6 cột trạng thái tương ứng với 6 giai đoạn bán hàng.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Viết hàm `GetKanbanDataAsync()` trong `OpportunityService` trả về Group theo Stage.
  * (60p) Xây dựng `Kanban.cshtml`:
    * Bố cục 6 cột dạng ngang có thanh cuộn (Horizontal Scrollable).
    * Header mỗi cột: Tên giai đoạn, Số lượng deal hiện có, Tổng giá trị tiền tệ lũy kế của cột.
    * Component Card cơ hội: Mã, Tiêu đề, Tên khách hàng, Badge số tiền, Avatar nhân viên, Tag cảnh báo quá hạn.
  * (30p) Viết CSS tùy biến tối ưu hiển thị cho Kanban board.
* 📦 **Output:** Khung Kanban chuẩn thẩm mỹ, phản ánh đầy đủ dữ liệu thời gian thực.
* ✅ **DoD:** Dữ liệu cơ hội chia đúng vào 6 cột; tổng giá trị hiển thị trên đỉnh mỗi cột khớp chính xác với tổng các card con.

#### Ngày 18 (Thứ 4) — Tích hợp Thư viện SortableJS & Kéo thả Cơ bản
* 🎯 **Mục tiêu:** Kích hoạt tính năng Drag-and-Drop giữa các cột và bắt sự kiện gọi API ngầm.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Nhúng thư viện `Sortable.min.js` vào Kanban view.
  * (45p) Viết đoạn mã JavaScript khởi tạo Sortable cho cả 6 danh sách cột:
    ```javascript
    document.querySelectorAll('.kanban-column-body').forEach(col => {
        new Sortable(col, {
            group: 'opportunities-kanban',
            animation: 150,
            ghostClass: 'kanban-ghost-card',
            onEnd: function (evt) {
                const oppId = evt.item.dataset.id;
                const newStageId = evt.to.dataset.stageId;
                handleMoveCard(oppId, newStageId, evt);
            }
        });
    });
    ```
  * (35p) Tạo Minimal API Endpoint: `POST /api/opportunities/{id}/move` nhận `{ newStageId }` và gọi `OpportunityService`.
  * (15p) Kiểm thử kéo card từ cột 1 sang cột 2, kiểm tra bản ghi DB đã cập nhật `stage_id` mới.
* 📦 **Output:** Kéo thả card mượt mà, backend nhận request và cập nhật trạng thái chuẩn xác.
* ✅ **DoD:** Kéo thả thẻ cơ hội giữa 2 cột bất kỳ không reload trang; refresh trang lại thẻ vẫn nằm ở cột mới.

#### Ngày 19 (Thứ 5) — Cập nhật Realtime Header Cột & Cơ chế Rollback khi Lỗi
* 🎯 **Mục tiêu:** Tự động tính toán lại số lượng và tổng tiền của 2 cột bị ảnh hưởng; hoàn tác card nếu API lỗi.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (40p) Viết logic JavaScript cập nhật DOM sau khi kéo thả thành công: trừ số lượng/tiền ở cột nguồn, cộng số lượng/tiền ở cột đích, hiệu ứng badge flash.
  * (45p) Xây dựng cơ chế Rollback (Hoàn tác UI): Bọc fetch API trong try/catch; nếu server trả về mã lỗi ($400, 500$) hoặc mất mạng thì đưa thẻ trở về vị trí cũ và hiển thị Toast thông báo.
  * (25p) Bổ sung hiệu ứng Loading Spinner mờ trên Card trong thời gian HTTP request đang xử lý.
  * (10p) Kiểm tra kịch bản giả lập ngắt mạng khi kéo thả để đảm bảo card tự động quay về chỗ cũ.
* 📦 **Output:** Trải nghiệm kéo thả hoàn hảo, dữ liệu hiển thị tức thì và xử lý ngoại lệ an toàn.
* ✅ **DoD:** Kéo thẻ khi tắt mạng $\rightarrow$ Thẻ tự động nhảy về cột ban đầu kèm thông báo "Không thể lưu thay đổi".

#### Ngày 20 (Thứ 6) — Toggle View, Đồng bộ Bộ lọc & Sprint 2 Review
* 🎯 **Mục tiêu:** Hoàn thiện chuyển đổi List/Kanban giữ nguyên filter, tối ưu truy vấn và tổng kết Sprint 2.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Tạo Partial View `_ViewToggle.cshtml` gồm 2 icon chuyển đổi: Danh sách (Table) và Bảng (Kanban).
  * (30p) Xử lý đồng bộ Query String: Đang lọc `salesId=xxx` ở List, bấm sang Kanban URL vẫn mang theo `salesId=xxx`.
  * (30p) Tối ưu hóa truy vấn Kanban bằng Projection Select chỉ lấy các trường cần thiết để giảm tải payload DB.
  * (30p) **Sprint 2 Review & Retrospective:** Kiểm tra độ mượt của kéo thả Kanban, chạy toàn bộ Unit test cơ hội.
* 📦 **Output:** Chuyển đổi linh hoạt giữa 2 chế độ hiển thị; Sprint 2 nghiệm thu đạt chuẩn.
* ✅ **DoD:** Chuyển đổi giữa Kanban và List không mất filter; toàn bộ test suite chạy thành công.

---

### SPRINT 3: BÁO GIÁ, ENGINE PDF/EMAIL & KẾ HOẠCH HÀNH ĐỘNG (TUẦN 5 - 6)
* 🎯 **Sprint Goal:** Số hóa hoàn chỉnh quy trình phát hành báo giá (QuestPDF in ấn tiếng Việt, MailKit gửi đính kèm) và xây dựng hệ thống Kế hoạch hành động tự động sinh việc cho Sales.
* ⏱ **Thời lượng:** 10 ngày làm việc (Ngày 21 $\rightarrow$ 30) = 20 giờ.

#### Ngày 21 (Thứ 2) — Repository & QuoteService Core
* 🎯 **Mục tiêu:** Xây dựng nghiệp vụ tính toán báo giá, thuế VAT, chiết khấu và tự động phát sinh mã.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Tạo `IQuoteRepository` và `QuoteRepository`.
  * (60p) Xây dựng `QuoteService`:
    * Hàm `CalculateTotals(List<QuoteItemDto> items)`: Tính thành tiền từng dòng ($Qty \times UnitPrice \times (1 - Discount/100)$), tính Subtotal, VAT và TotalAmount.
    * Hàm `CreateFromOpportunityAsync(Guid opportunityId)`: Kế thừa thông tin khách hàng và items từ deal sang dự thảo báo giá.
    * Tự động sinh mã báo giá dạng `BG-yyyyMM-0001`.
  * (20p) Viết 4 Unit Tests kiểm tra tính toán tiền tệ, làm tròn số học và xử lý chiết khấu 0%.
  * (10p) Verify kết quả chạy test.
* 📦 **Output:** Service báo giá với thuật toán tính toán tài chính chuẩn xác.
* ✅ **DoD:** Unit test tính toán tổng tiền khớp từng đồng với công thức kế toán, không có sai số dấu phẩy động.

#### Ngày 22 (Thứ 3) — Màn hình Tạo Báo giá (Phần 1: Thông tin Chung & Khách hàng)
* 🎯 **Mục tiêu:** Xây dựng màn hình nhập liệu thông tin cơ sở của báo giá bán hàng (`/Quotes/Create`).
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Thiết kế `QuoteCreateViewModel` bao gồm: Khách hàng, Người nhận, Ngày báo giá, Ngày hết hiệu lực (mặc định $+15$ ngày), Điều khoản thanh toán.
  * (55p) Xây dựng `Pages/Quotes/Create.cshtml`: Tích hợp tìm kiếm nhanh khách hàng qua HTMX; hiển thị bảng tóm tắt công nợ hiện tại của khách hàng; validate ngày hết hạn không nhỏ hơn ngày lập.
  * (35p) Kiểm thử binding dữ liệu và validation của tầng giao diện.
* 📦 **Output:** Form tạo báo giá cơ bản với khả năng liên kết dữ liệu khách hàng nhanh.
* ✅ **DoD:** Chọn một khách hàng $\rightarrow$ Địa chỉ, số điện thoại và thông tin liên hệ chính tự động điền vào form.

#### Ngày 23 (Thứ 4) — Màn hình Tạo Báo giá (Phần 2: Bảng Sản phẩm Động)
* 🎯 **Mục tiêu:** Xây dựng bảng chi tiết sản phẩm cho phép Thêm/Xóa dòng và tính toán tổng tiền thời gian thực.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (50p) Xây dựng Table thêm sản phẩm động bằng JavaScript/HTMX: Nút "Thêm dòng sản phẩm", dropdown tìm kiếm sản phẩm có hiển thị giá niêm yết, nút xóa dòng (thùng rác).
  * (45p) Viết hàm JavaScript `recalculateQuote()`: Lắng nghe sự kiện `input` trên các ô số lượng, đơn giá, chiết khấu $\rightarrow$ tính ngay thành tiền và cập nhật khối Summary (Tiền hàng, Giảm giá, VAT $8\%$ hoặc $10\%$, Tổng cộng).
  * (25p) Xử lý Submit form lên Server, map chính xác vào mảng `List<QuoteItemCreateDto>`.
* 📦 **Output:** Giao diện lên báo giá trực quan, tương tác tức thì không cần đợi server tính toán.
* ✅ **DoD:** Thay đổi số lượng hoặc chiết khấu ở bất kỳ dòng nào, tổng tiền phía dưới thay đổi ngay lập tức.

#### Ngày 24 (Thứ 5) — Tích hợp QuestPDF & Thiết kế Template Báo giá Chuẩn
* 🎯 **Mục tiêu:** Sử dụng thư viện QuestPDF tạo mẫu tài liệu PDF chuyên nghiệp, chuẩn font tiếng Việt.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Cài đặt package `QuestPDF` (cấu hình `QuestPDF.Settings.License = LicenseType.Community;`).
  * (65p) Thiết kế class `QuoteDocument : IDocument`:
    * Cấu hình Font chữ Roboto hoặc OpenSans hỗ trợ đầy đủ ký tự Unicode Tiếng Việt.
    * Khối Header: Logo công ty bên trái, thông tin công ty và mã báo giá bên phải.
    * Khối Metadata: Tên khách hàng, Địa chỉ giao hàng, MST, Người liên hệ, Hiệu lực báo giá.
    * Khối Bảng hàng hóa: STT, Mã SP, Tên quy cách, ĐVT, Số lượng, Đơn giá, Thành tiền.
    * Khối Footer: Tổng tiền bằng số và bằng chữ, Điều khoản thanh toán, Khung ký tên hai bên.
  * (30p) Tạo Action preview xuất file PDF mẫu ra ổ cứng cục bộ để kiểm tra layout.
* 📦 **Output:** File PDF mẫu xuất ra có độ nét cao, định dạng thẩm mỹ, không lỗi vỡ font tiếng Việt.
* ✅ **DoD:** Xuất file PDF thử nghiệm, các từ tiếng Việt có dấu ("Báo giá", "Điều khoản", "Thuế GTGT") hiển thị sắc nét.

#### Ngày 25 (Thứ 6) — Hoàn thiện API Xuất PDF & Preview Trực tiếp
* 🎯 **Mục tiêu:** Cung cấp endpoint stream file PDF về trình duyệt và tích hợp nút In PDF vào trang chi tiết.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (40p) Xây dựng Controller Endpoint: `GET /api/quotes/{id}/pdf`: Truy vấn báo giá kèm Khách hàng và Items; biên dịch QuestPDF thành byte array qua memory stream; trả về `File(bytes, "application/pdf", $"Bao-Gia-{code}.pdf")`.
  * (40p) Tích hợp nút "Xem trước / In PDF" trên giao diện Báo giá: Mở tab mới xem trực tiếp bằng PDF Viewer của trình duyệt.
  * (30p) Tối ưu hiệu năng: Cache template hoặc tái sử dụng layout stream để tránh tốn CPU.
  * (10p) Kiểm tra xuất PDF với báo giá nhiều hơn 2 trang để đảm bảo tính năng đánh số trang ($Trang\ X/Y$) hoạt động đúng.
* 📦 **Output:** Endpoint xuất PDF hoàn chỉnh, sẵn sàng cho việc in ấn hoặc gửi khách hàng.
* ✅ **DoD:** Click nút "In PDF" $\rightarrow$ Tab trình duyệt mới bật lên hiển thị trực tiếp file PDF để khách hàng in ấn.

#### Ngày 26 (Thứ 2) — Màn hình Danh sách & Chi tiết Báo giá (`/Quotes/Index` & `/Detail`)
* 🎯 **Mục tiêu:** Hoàn thiện 2 màn hình quản lý vòng đời báo giá (Gửi, Chấp thuận, Từ chối, Hết hạn).
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Xây dựng `Pages/Quotes/Index.cshtml`: Hiển thị bảng báo giá (Mã, Khách hàng, Tổng giá trị, Ngày lập, Hạn sử dụng, Trạng thái); bộ lọc theo trạng thái và khách hàng.
  * (50p) Xây dựng `Pages/Quotes/Detail.cshtml`: Xem lại toàn bộ thông tin; các nút chuyển trạng thái nhanh ("Đánh dấu đã gửi", "Khách đã duyệt", "Khách từ chối").
  * (25p) Xử lý logic tự động: Nếu Báo giá chuyển sang "Accepted" $\rightarrow$ Tự động chuyển Cơ hội bán hàng liên quan sang stage "Chốt thành công (Won)".
* 📦 **Output:** Quản lý tập trung toàn bộ báo giá phát hành của công ty.
* ✅ **DoD:** Đổi trạng thái Báo giá sang "Accepted" $\rightarrow$ Cơ hội tương ứng tự động cập nhật sang stage "Won".

#### Ngày 27 (Thứ 3) — Tích hợp MailKit & Gửi Báo giá Kèm PDF Đính Kèm
* 🎯 **Mục tiêu:** Tự động gửi email chuyên nghiệp đính kèm file PDF báo giá trực tiếp tới hòm thư khách hàng.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Cài đặt package `MailKit` và cấu hình thông số SMTP trong `appsettings.json` (Host, Port, User, Pass, SSL).
  * (50p) Xây dựng `EmailService`: Hàm `SendQuoteEmailAsync(Guid quoteId, string recipientEmail, string note)` tự động render file PDF in-memory, đính kèm vào MimeMessage và dùng template HTML trang trọng.
  * (30p) Thiết kế Modal "Gửi email cho khách" trên giao diện Chi tiết Báo giá.
  * (15p) Ghi nhận một bản ghi vào bảng `interactions` sau khi email được gửi thành công.
* 📦 **Output:** Tính năng gửi email báo giá tự động, chuyên nghiệp.
* ✅ **DoD:** Gửi thử nghiệm tới email thật: Email nhận được có file PDF đính kèm mở đúng nội dung; bảng interaction có log.

#### Ngày 28 (Thứ 4) — Repository & SalesTaskService Core (Logic Kế hoạch Hành động)
* 🎯 **Mục tiêu:** Xây dựng logic quản lý công việc của Sales kèm cơ chế sinh nhiệm vụ tiếp theo dựa trên kết quả.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Tạo `ITaskRepository` với các truy vấn lọc theo ngày đến hạn (`DueDate`).
  * (60p) Xây dựng `SalesTaskService`:
    * `GetMyTasksAsync(Guid userId, TaskFilterDto filter)`: Lấy việc cần làm của nhân viên.
    * `CompleteTaskAsync(Guid taskId, TaskOutcome outcome, string notes, DateTime? nextDate)`:
      * Nếu `outcome == CustomerWillBuy`: Tự động tạo một Cơ hội mới ở stage "Khảo sát nhu cầu".
      * Nếu `outcome == CustomerBusy`: Tự động tạo một Task nhắc gọi lại sau $2$ ngày.
      * Nếu `outcome == Rejected`: Ghi log và không sinh thêm task.
  * (20p) Viết 3 Unit Tests kiểm tra máy trạng thái (State-machine) sinh việc tự động của TaskOutcome.
  * (10p) Verify kết quả chạy test.
* 📦 **Output:** Bộ quy tắc thông minh giúp Sales không bao giờ bị quên khách hàng.
* ✅ **DoD:** Hoàn thành task với kết quả "Khách bận" $\rightarrow$ Task mới tự động xuất hiện trong danh sách sau 2 ngày.

#### Ngày 29 (Thứ 5) — Màn hình Kế hoạch Hành động (`/Tasks/Index`)
* 🎯 **Mục tiêu:** Xây dựng màn hình quản lý công việc hàng ngày của Sales với bộ lọc Tab thời gian linh hoạt.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Thiết kế `TaskListViewModel` phân loại theo tính cấp bách.
  * (65p) Xây dựng `Pages/Tasks/Index.cshtml`: Hệ thống Tab lọc nhanh (**Hôm nay** mặc định, **Quá hạn** màu đỏ, **Tuần này**, **Tất cả**); bảng công việc (Tiêu đề, Khách hàng, Loại hình, Hạn chót, Trạng thái, Cột thao tác).
  * (30p) Viết Code-behind nhận filter tab và gọi `SalesTaskService`.
* 📦 **Output:** Bàn làm việc số hàng ngày giúp Sales kiểm soát 100% công việc phải xử lý.
* ✅ **DoD:** Mở trang Tasks: Mặc định hiển thị việc hôm nay; tab Quá hạn hiển thị đúng việc trễ hạn.

#### Ngày 30 (Thứ 6) — Modal Hoàn thành Task Bằng HTMX & Sprint 3 Review
* 🎯 **Mục tiêu:** Cho phép Sales đóng việc nhanh và ghi chú kết quả bằng HTMX modal; nghiệm thu Sprint 3.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (35p) Thiết kế Partial View `_CompleteTaskModal.cshtml`: Dropdown chọn kết quả (`TaskOutcome`), Textarea nhập ghi chú, khối ngày hẹn tiếp theo.
  * (45p) Tích hợp HTMX: Click nút "Xong" trên từng dòng $\rightarrow$ Modal mở lên $\rightarrow$ Submit kết quả $\rightarrow$ Dòng công việc biến mất mượt mà khỏi danh sách "Hôm nay".
  * (40p) **Sprint 3 Review & Retrospective:** Kiểm thử liên hoàn luồng Tạo cơ hội $\rightarrow$ Lập báo giá $\rightarrow$ Xuất PDF $\rightarrow$ Gửi email $\rightarrow$ Hoàn thành task.
* 📦 **Output:** Trải nghiệm kết thúc công việc cực nhanh; toàn bộ luồng Quote & Task hoàn tất.
* ✅ **DoD:** Đóng task xong UI cập nhật ngay không reload; email báo giá gửi đi chuẩn xác; Sprint 3 nghiệm thu thành công.

---

### SPRINT 4: TỰ ĐỘNG HÓA HANGFIRE & ĐỘNG CƠ BÁO CÁO (TUẦN 7 - 8)
* 🎯 **Sprint Goal:** Thiết lập hệ thống tác vụ nền Hangfire tự động đánh giá sức khỏe khách hàng, sinh việc nhắc nhở, tính toán phễu bán hàng Cohort và đóng băng dữ liệu snapshot hằng đêm.
* ⏱ **Thời lượng:** 10 ngày làm việc (Ngày 31 $\rightarrow$ 40) = 20 giờ.

#### Ngày 31 (Thứ 2) — Cài đặt Hangfire Dashboard & Cấu hình Lưu trữ PostgreSQL
* 🎯 **Mục tiêu:** Tích hợp engine xử lý tác vụ nền Hangfire, bảo mật màn hình dashboard quản trị.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Cài đặt package `Hangfire.Core`, `Hangfire.AspNetCore`, `Hangfire.PostgreSql`.
  * (45p) Cấu hình trong `Program.cs`: Khởi tạo lưu trữ schema `hangfire`, worker pool (2-4 workers), cấu hình `app.UseHangfireDashboard("/hangfire")`.
  * (35p) Tạo Custom Authorization Filter cho Dashboard: Chỉ người dùng có Role `ADMIN` mới được truy cập `/hangfire`.
  * (15p) Tạo một Fire-and-Forget Job đơn giản để verify hệ thống vận hành trơn tru.
* 📦 **Output:** Trang quản trị tác vụ nền `/hangfire` hoạt động an toàn.
* ✅ **DoD:** User thường vào `/hangfire` bị chặn (403 Forbidden); Admin vào được và thấy server Running.

#### Ngày 32 (Thứ 3) — Background Job: Tự động Sinh Task Nhắc Chăm sóc Khách hàng
* 🎯 **Mục tiêu:** Lập trình Worker tự động phân tích chu kỳ mua hàng và sinh công việc chăm sóc cho Sales.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Thuật toán tính ngày tiếp xúc dự kiến:
    $$NextDueDate = LastOrderDate + AverageCycleDays - 2\ \text{ngày}$$
  * (60p) Xây dựng `GenerateRemindersJob` trong `Crm.Jobs`: Quét khách hàng `Active`; nếu chạm ngưỡng $NextDueDate$ và chưa có task mở thì tự động insert task nhắc chăm sóc; nếu quá hạn $> 2$ lần chu kỳ thì sinh task cảnh báo im lặng bất thường.
  * (30p) Chạy thử nghiệm Job qua Trigger Now trên Hangfire Dashboard để kiểm tra kết quả ghi DB.
* 📦 **Output:** Worker tự động sinh việc thông minh, chủ động giữ chân khách hàng.
* ✅ **DoD:** Bấm Trigger Job trên Dashboard $\rightarrow$ Kiểm tra bảng `tasks` xuất hiện các việc nhắc chăm sóc tương ứng.

#### Ngày 33 (Thứ 4) — Background Job: Đánh giá Sức khỏe Khách hàng Hàng đêm
* 🎯 **Mục tiêu:** Tự động tính toán lại chỉ số Health Status của toàn bộ khách hàng theo 4 cấp độ định kỳ.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Thiết lập ma trận quy tắc sức khỏe: `Good` ($\le 1.0$ chu kỳ), `Fair` ($1.0 - 1.5$ chu kỳ), `AtRisk` ($1.5 - 2.0$ chu kỳ), `Churned` ($> 2.0$ chu kỳ).
  * (65p) Xây dựng `UpdateCustomerHealthJob`: Quét batch 100 khách hàng/lần; tính số ngày kể từ đơn cuối ($DaysSince = Today - LastOrderDate$); cập nhật cột `health_status`; ghi log cảnh báo nếu khách VIP rơi vào `AtRisk`.
  * (30p) Kiểm thử với dữ liệu mẫu có các mốc ngày khác nhau.
* 📦 **Output:** Job cập nhật sức khỏe chạy ngầm hiệu quả, đảm bảo tính đúng đắn của dữ liệu.
* ✅ **DoD:** Các khách hàng bỏ quên lâu ngày tự động chuyển sang badge màu vàng/đỏ mà không cần thao tác tay.

#### Ngày 34 (Thứ 5) — Đăng ký Lịch trình Recurring Jobs & Chiến lược Thử lại (Retry)
* 🎯 **Mục tiêu:** Cấu hình Cron biểu thức chạy tự động nửa đêm và thiết lập cơ chế tự phục hồi khi có sự cố.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (40p) Đăng ký danh sách Recurring Jobs chuẩn trong `Program.cs`:
    * `GenerateRemindersJob`: Chạy lúc 02:00 sáng hàng ngày (`0 2 * * *`).
    * `UpdateCustomerHealthJob`: Chạy lúc 03:00 sáng hàng ngày (`0 3 * * *`).
  * (45p) Cấu hình Retry Policy và Báo lỗi: Áp dụng `[AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]`; job fail quá số lần quy định thì ghi log mức `Fatal`.
  * (35p) Viết tài liệu danh mục các Jobs và ý nghĩa vận hành vào thư mục `docs/`.
* 📦 **Output:** Hệ thống tự động hóa hoàn toàn lịch trình tác vụ ngầm.
* ✅ **DoD:** Vào Hangfire Dashboard mục "Recurring Jobs": Thấy đủ danh sách jobs với lịch chạy tiếp theo chính xác.

#### Ngày 35 (Thứ 6) — Đợt Refactor Lớn Tuần 7 & Bổ sung Unit Tests
* 🎯 **Mục tiêu:** Dọn dẹp nợ kỹ thuật sau 7 tuần đầu, nâng độ bao phủ Unit Test tầng Service.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Code Review toàn bộ các Services (`Customer`, `Opportunity`, `Quote`, `Task`): loại bỏ câu query $N+1$ bằng `.Include()`; tách method dài quá 40 dòng thành các private sub-methods.
  * (55p) Bổ sung 8 Unit Tests trọng yếu: kiểm tra tính đúng đắn của phân loại Health Status, kiểm tra logic chuyển stage và khóa cơ hội.
  * (20p) Chạy toàn bộ test suite, đảm bảo 100% pass với thời gian thực thi $< 5$ giây.
* 📦 **Output:** Mã nguồn chuẩn mực, cấu trúc rõ ràng, sẵn sàng cho giai đoạn Dashboard.
* ✅ **DoD:** Tất cả Unit Tests chạy thành công; không có cảnh báo compile (0 Warning, 0 Error).

#### Ngày 36 (Thứ 2) — DashboardService: Tính toán KPI Cơ bản & Tỷ trọng Khách hàng
* 🎯 **Mục tiêu:** Xây dựng tầng tính toán số liệu thống kê tổng hợp phục vụ màn hình lãnh đạo.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Tạo `DashboardRepository` tối ưu hóa bằng các truy vấn `SUM`, `COUNT` nguyên bản.
  * (60p) Xây dựng `DashboardService`:
    * Hàm `GetKpiSummaryAsync(DateTime from, DateTime to)`: Doanh số đạt được, Số cơ hội mới, Tỷ lệ chốt đơn (Win Rate), Số nhiệm vụ hoàn thành.
    * Hàm `GetCustomerHealthRatioAsync()`: Tính tỷ lệ phần trăm giữa 4 nhóm sức khỏe, đảm bảo tổng luôn bằng chính xác $100.0\%$.
  * (25p) Viết 2 Unit Tests kiểm tra tính toán tỷ lệ phần trăm không bị lỗi chia cho 0 khi chưa có dữ liệu.
  * (5p) Verify kết quả chạy test.
* 📦 **Output:** Service cung cấp số liệu thống kê chuẩn xác, có tính đến các trường hợp dữ liệu biên.
* ✅ **DoD:** Tổng 4 nhóm tỷ lệ khách hàng luôn đạt chính xác $100.0\%$ (xử lý sai số làm tròn $99.9\%$ hoặc $100.1\%$).

#### Ngày 37 (Thứ 3) — DashboardService: Tính toán Phễu Bán hàng Theo Cohort (Funnel)
* 🎯 **Mục tiêu:** Xây dựng logic phân tích tỷ lệ chuyển đổi qua từng tầng của Pipeline bán lẻ/sỉ.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Nghiên cứu mô hình Funnel dạng Cohort: Nhóm các cơ hội được tạo trong khoảng thời gian đã chọn và theo dõi lịch sử dịch chuyển qua bảng `opportunity_stage_histories`.
  * (65p) Viết hàm `GetSalesFunnelAsync(DateTime from, DateTime to)`:
    * Tầng 1 (Tiếp cận): $100\%$ (Tổng cơ hội tạo mới trong kỳ).
    * Tầng 2, 3, 4, 5, 6: Đếm số cơ hội đã từng bước chân qua giai đoạn đó dựa vào lịch sử vết.
    * Tính tỷ lệ rơi rụng (Drop-off rate) giữa các tầng kế tiếp.
  * (25p) Viết Unit Test xác minh tính chất bắt buộc: Số lượng deal ở tầng sau không bao giờ lớn hơn tầng trước ($Count_{n+1} \le Count_n$).
* 📦 **Output:** Thuật toán phễu bán hàng chuẩn chỉnh, phản ánh đúng thực tế kinh doanh.
* ✅ **DoD:** Unit test phễu chuyển đổi thông qua; đồ thị phễu đảm bảo hình nón giảm dần.

#### Ngày 38 (Thứ 4) — DashboardService: Doanh thu 6 Tháng & Bảng Vinh danh Top Staff
* 🎯 **Mục tiêu:** Xây dựng dữ liệu doanh thu lũy kế chia theo Khách mới / Khách cũ và xếp hạng nhân viên.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (55p) Viết hàm `GetMonthlyRevenueAsync(int months = 6)`: Truy vấn doanh số phân bổ theo tháng, tách rõ 2 series (Khách mới vs Khách cũ mua lại).
  * (45p) Viết hàm `GetTopPerformersAsync(DateTime from, DateTime to, int top = 5)`: Xếp hạng nhân sự theo tổng doanh số mang về và số lượng hợp đồng chốt thành công.
  * (20p) Kiểm thử với dữ liệu thực tế đã nạp.
* 📦 **Output:** Dữ liệu phân tích doanh số đa chiều sắc bén cho quản lý.
* ✅ **DoD:** Doanh số 6 tháng truy vấn trả về mảng đúng 6 phần tử tương ứng với 6 tháng gần nhất.

#### Ngày 39 (Thứ 5) — Background Job: Đóng băng Dữ liệu Dashboard Hàng đêm (Snapshot)
* 🎯 **Mục tiêu:** Lưu trữ toàn bộ kết quả tính toán phức tạp vào bảng JSON Snapshot để Dashboard ban ngày tải tức thì.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Tạo Entity `DashboardSnapshot` (`SnapshotDate`, `KpiJson`, `FunnelJson`, `RevenueJson`, `CreatedAt`).
  * (60p) Xây dựng `SnapshotDashboardJob` trong `Crm.Jobs`: Lấy dữ liệu từ các hàm thống kê; serialize thành JSONB; lưu đè hoặc thêm mới vào bảng `dashboard_snapshots`; cấu hình cron chạy lúc 04:00 sáng (`0 4 * * *`).
  * (30p) Sửa đổi `DashboardService`: Nếu người dùng xem ngày hôm qua trở về trước $\rightarrow$ Đọc thẳng từ snapshot (tốc độ $< 10ms$).
* 📦 **Output:** Cơ chế tối ưu hiệu năng vượt trội cho hệ thống báo cáo.
* ✅ **DoD:** Truy vấn Dashboard của các ngày trong quá khứ không cần tính toán lại, tải dữ liệu tức thì.

#### Ngày 40 (Thứ 6) — Consistency Check Job & Sprint 4 Review
* 🎯 **Mục tiêu:** Xây dựng chốt chặn kiểm soát chất lượng tự động, phát hiện sai lệch số liệu và tổng kết Sprint 4.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (50p) Xây dựng `ConsistencyCheckJob` kiểm tra 6 ràng buộc vàng:
    * *Ràng buộc 1:* Tỷ lệ khách hàng: $\%Good + \%Fair + \%AtRisk + \%Churned == 100.0\%$.
    * *Ràng buộc 2:* Phễu chuyển đổi: Số lượng tầng sau $\le$ tầng trước.
    * *Ràng buộc 3:* Số hợp đồng chốt trong phễu $=$ Số lượng deal Won ở KPI card.
    * *Ràng buộc 4:* Tổng doanh thu 6 tháng $=$ Tổng doanh thu khách mới $+$ khách cũ.
    * *Ràng buộc 5:* Doanh số của Top 5 nhân viên $\le$ Tổng doanh số toàn công ty.
    * *Ràng buộc 6:* Tầng 1 của phễu $=$ Tổng số cơ hội mới tạo trong kỳ.
  * (30p) Nếu phát hiện bất kỳ vi phạm nào: Tự động ghi log cảnh báo mức `Error` kèm thông số chi tiết.
  * (40p) **Sprint 4 Review & Retrospective:** Kiểm tra hoạt động của toàn bộ 4 Hangfire Recurring Jobs, nghiệm thu các service tính toán.
* 📦 **Output:** Hệ thống tự giám sát tính toàn vẹn dữ liệu tự động 100%; Sprint 4 hoàn tất vững chắc.
* ✅ **DoD:** Job chạy ngầm không phát hiện lỗi; nếu cố tình sửa sai dữ liệu DB, job lập tức ghi nhận lỗi vi phạm.

---

### SPRINT 5: TRỰC QUAN HÓA DASHBOARD & ĐỒNG BỘ KIOTVIET (TUẦN 9 - 10)
* 🎯 **Sprint Goal:** Hoàn thiện giao diện Dashboard kinh doanh tương tác cao với Chart.js và triển khai kết nối hai chiều KiotViet Open API (Khách hàng, Hóa đơn đơn hàng, Danh mục sản phẩm).
* ⏱ **Thời lượng:** 10 ngày làm việc (Ngày 41 $\rightarrow$ 50) = 20 giờ.

#### Ngày 41 (Thứ 2) — Tích hợp Chart.js & Dựng 4 Thẻ KPI Đỉnh Trang
* 🎯 **Mục tiêu:** Xây dựng phần đầu trang Dashboard gồm thông báo quan trọng và 4 chỉ số sinh mệnh.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Nhúng thư viện `Chart.js@4.4.0` vào trang Dashboard.
  * (55p) Xây dựng `Pages/Index.cshtml` (Dashboard chính): Thanh Thông báo (Notify Alert Bar) hiển thị số khách AtRisk cần cứu vãn; 4 Thẻ KPI (Doanh số tháng có so sánh $\%$, Cơ hội mới, Win Rate $\%$, Việc cần xử lý hôm nay).
  * (40p) Kết nối dữ liệu từ `DashboardService` và render lên Razor Pages.
* 📦 **Output:** Giao diện phần đầu Dashboard hiện đại, bắt mắt, cung cấp thông tin cốt lõi.
* ✅ **DoD:** Mở Dashboard: 4 thẻ hiển thị đầy đủ thông số chính xác, màu sắc tương phản rõ ràng.

#### Ngày 42 (Thứ 3) — Trực quan hóa Khách hàng & Phễu Bán hàng (Pie Chart & Funnel)
* 🎯 **Mục tiêu:** Vẽ biểu đồ tròn sức khỏe khách hàng và đồ thị thanh ngang phễu bán hàng.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Vẽ Biểu đồ Donut/Pie Chart (Tỷ trọng sức khỏe khách hàng): 4 màu sắc chuẩn nhận diện (Xanh lá, Xanh dương, Cam, Đỏ); hiển thị Legend chú thích số lượng và $\%$.
  * (55p) Vẽ Biểu đồ Phễu Bán hàng (Sales Funnel): Sử dụng Bar Chart nằm ngang thể hiện 6 tầng giai đoạn, có bảng phụ lục tỷ lệ chuyển đổi bên cạnh.
  * (20p) Tinh chỉnh responsive đảm bảo không vỡ khung hình trên màn hình nhỏ.
* 📦 **Output:** Khu vực phân tích khách hàng và phễu chuyển đổi trực quan, sắc nét.
* ✅ **DoD:** Biểu đồ hiển thị đúng tỷ lệ, rê chuột vào hiển thị tooltip thông tin chi tiết từng phần.

#### Ngày 43 (Thứ 4) — Khu vực Kế hoạch Hành động Ngay trên Dashboard
* 🎯 **Mục tiêu:** Đưa danh sách việc cần làm trong ngày vào Dashboard giúp Sales không bị phân tâm.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Nhúng bảng công việc tóm tắt vào Dashboard: hiển thị tối đa 5 việc khẩn cấp nhất hôm nay; nút "Xem tất cả việc" dẫn sang `/Tasks/Index`.
  * (50p) Tích hợp Modal đóng nhanh công việc bằng HTMX (tái sử dụng modal đã viết ở Tuần 6).
  * (25p) Kiểm tra luồng tương tác: Hoàn thành việc ngay tại Dashboard $\rightarrow$ Thẻ KPI "Việc cần xử lý" tự động trừ đi 1.
* 📦 **Output:** Tính năng thúc đẩy hành động thực tế ngay tại trung tâm điều khiển.
* ✅ **DoD:** Đóng một việc trên Dashboard $\rightarrow$ Thẻ đếm việc giảm từ 5 xuống 4 mà không tải lại trang.

#### Ngày 44 (Thứ 5) — Trực quan hóa Doanh số 6 Tháng & Bảng Top Nhân viên
* 🎯 **Mục tiêu:** Vẽ biểu đồ cột doanh thu kép và bảng xếp hạng nhân viên xuất sắc.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (55p) Vẽ Biểu đồ Grouped Bar Chart: Trục hoành 6 tháng gần nhất; Cột 1 Doanh số khách cũ (xanh đậm); Cột 2 Doanh số khách mới (xanh ngọc).
  * (45p) Xây dựng Bảng Top 5 Nhân viên Bán hàng: Thứ hạng (Vàng, Bạc, Đồng), Họ tên Sales, Doanh số, Số hợp đồng chốt.
  * (20p) Kiểm tra tổng thể thẩm mỹ của Dashboard.
* 📦 **Output:** Bức tranh toàn cảnh về tăng trưởng doanh số và năng suất nhân sự.
* ✅ **DoD:** Biểu đồ doanh số hiển thị rõ ràng 2 cột phân loại; bảng vinh danh sắp xếp đúng thứ tự doanh số giảm dần.

#### Ngày 45 (Thứ 6) — Hoàn thiện Toàn diện Dashboard & Tối ưu Hiệu năng
* 🎯 **Mục tiêu:** Kiểm thử toàn diện Dashboard trên các độ phân giải màn hình và tối ưu thời gian phản hồi.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (40p) Kiểm tra hiển thị Responsive: Desktop $1920\times 1080$, Laptop $1366\times 768$, và Mobile $375\times 667$.
  * (40p) Tối ưu hóa truy vấn SQL: Đảm bảo thời gian load trang toàn bộ Dashboard $< 300ms$.
  * (40p) Viết 4 Unit Tests cho các hàm tổng hợp của Dashboard.
* 📦 **Output:** Màn hình Dashboard đạt chuẩn Production, vận hành mượt mà, tốc độ cao.
* ✅ **DoD:** Tải lại trang Dashboard đạt tốc độ phản hồi dưới nửa giây; kiểm tra mobile không bị tràn ngang.

#### Ngày 46 (Thứ 2) — Nghiên cứu KiotViet API & Xây dựng HttpClient Xác thực OAuth 2.0
* 🎯 **Mục tiêu:** Xây dựng module kết nối KiotViet API, tự động lấy và làm mới Access Token khi hết hạn.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Đọc tài liệu KiotViet Open API: Client Credentials (`client_id`, `client_secret`, lấy token từ `https://id.kiotviet.vn/connect/token`).
  * (50p) Xây dựng `KiotVietAuthHandler` (DelegatingHandler): Quản lý lưu trữ Access Token trong Cache; kiểm tra `expires_in`; tự động gửi request lấy Token mới nếu token hết hạn.
  * (30p) Đăng ký `HttpClient` có tên `KiotVietClient` trong `Program.cs` sử dụng `IHttpClientFactory`.
  * (10p) Viết hàm gọi thử nghiệm: Lấy thông tin chi nhánh cửa hàng thành công.
* 📦 **Output:** Tầng giao tiếp HTTP Client với KiotViet an toàn, tự động quản lý token.
* ✅ **DoD:** Gửi request lên KiotViet API thành công, nhận phản hồi HTTP 200 kèm danh sách chi nhánh.

#### Ngày 47 (Thứ 3) — Đồng bộ Khách hàng từ KiotViet về CRM
* 🎯 **Mục tiêu:** Kéo dữ liệu khách hàng từ KiotViet về, xử lý cập nhật hoặc tạo mới thông minh.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (65p) Xây dựng hàm `SyncCustomersAsync()` trong `KiotVietSyncService`: Lấy danh sách khách hàng phân trang (`pageSize = 100`); ánh xạ qua `KiotVietId` hoặc `TaxCode`/`Phone`; nếu chưa có thì tạo mới; nếu đã có thì cập nhật thông tin cơ bản, tuyệt đối không ghi đè các trường riêng của CRM (`health_status`, `notes`).
  * (35p) Ghi nhận kết quả vào bảng `kiotviet_sync_logs` (Số tạo mới, Số cập nhật, Lỗi nếu có).
  * (20p) Kiểm thử đồng bộ với 50 khách hàng thực tế từ KiotViet test.
* 📦 **Output:** Dữ liệu khách hàng được đồng bộ hóa nhất quán giữa 2 hệ thống.
* ✅ **DoD:** Chạy đồng bộ lần 1: Tạo mới 50 khách hàng; Chạy đồng bộ lần 2 ngay sau đó: Cập nhật 50 bản ghi, không sinh trùng lặp.

#### Ngày 48 (Thứ 4) — Đồng bộ Đơn hàng & Cập nhật Doanh số Khách hàng
* 🎯 **Mục tiêu:** Kéo dữ liệu hóa đơn bán lẻ từ KiotViet để tính toán chính xác doanh thu và ngày mua cuối.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (65p) Xây dựng hàm `SyncOrdersAsync(DateTime fromDate)`: Lấy hóa đơn "Đã hoàn thành" trong 90 ngày gần nhất; cập nhật lại `Revenue90d`, `OrderCount90d`, `LastOrderDate` của Khách hàng.
  * (35p) Xử lý ngoại lệ nếu đơn hàng thuộc khách vãng lai không có mã khách trên KiotViet.
  * (20p) Kiểm tra lại dữ liệu khách hàng sau khi sync đơn hàng.
* 📦 **Output:** Doanh số khách hàng trên CRM phản ánh chính xác từng đồng theo dữ liệu bán hàng thực tế.
* ✅ **DoD:** Sau khi sync, trường `Revenue90d` và `LastOrderDate` của khách hàng được cập nhật chuẩn xác theo KiotViet.

#### Ngày 49 (Thứ 5) — Đồng bộ Danh mục Sản phẩm & Bảng giá
* 🎯 **Mục tiêu:** Tự động đồng bộ hóa danh mục sản phẩm, đơn vị tính và giá niêm yết từ KiotViet.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (60p) Xây dựng hàm `SyncProductsAsync()`: Kéo danh sách sản phẩm đang kinh doanh (`isActive == true`); ánh xạ `Code`, `Name`, Đơn vị tính, `BasePrice`, Danh mục; nếu sản phẩm ngừng bán trên KiotViet thì đánh dấu ngừng sử dụng trên CRM.
  * (40p) Xử lý ánh xạ Danh mục sản phẩm (Categories Mapping).
  * (20p) Test đồng bộ danh mục hàng hóa.
* 📦 **Output:** Danh mục sản phẩm trên CRM luôn luôn khớp với phần mềm quản lý bán hàng.
* ✅ **DoD:** Thay đổi giá 1 sản phẩm trên KiotViet $\rightarrow$ Chạy sync $\rightarrow$ Báo giá trên CRM nhận giá mới.

#### Ngày 50 (Thứ 6) — Đăng ký Recurring Sync Job & Sprint 5 Review
* 🎯 **Mục tiêu:** Thiết lập tác vụ đồng bộ tự động mỗi 15 phút một lần kèm cơ chế chống nghẽn; tổng kết Sprint 5.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (35p) Tạo `SyncKiotVietJob` trong `Crm.Jobs` kết hợp cả 3 tác vụ (Khách hàng, Đơn hàng, Sản phẩm); đăng ký Recurring Job trên Hangfire chạy định kỳ 15 phút/lần (`*/15 * * * *`).
  * (35p) Xây dựng cơ chế chống nghẽn: Áp dụng `[DisableConcurrentExecution(timeoutInSeconds: 300)]`; bắt mã lỗi HTTP 429 (Too Many Requests) để giãn cách gửi request.
  * (50p) **Sprint 5 Review & Retrospective:** Demo toàn diện màn hình Dashboard kinh doanh và quy trình đồng bộ KiotViet tự động.
* 📦 **Output:** Hệ thống đồng bộ KiotViet tự động hóa 100%; Sprint 5 hoàn thành đúng tiến độ.
* ✅ **DoD:** Job chạy định kỳ 15 phút; Dashboard hiển thị số liệu đồng bộ chính xác; Sprint 5 nghiệm thu thành công.

---

### SPRINT 6: HOÀN THIỆN ĐỒNG BỘ, AUDIT TRAIL & BẢO MẬT HỆ THỐNG (TUẦN 11 - 12)
* 🎯 **Sprint Goal:** Hoàn thiện tính năng tra cứu tồn kho, đồng bộ công nợ, xây dựng hệ thống Audit Log tự động truy vết 100% thay đổi, gia cố bảo mật OWASP, backup tự động và cảnh báo qua Telegram.
* ⏱ **Thời lượng:** 10 ngày làm việc (Ngày 51 $\rightarrow$ 60) = 20 giờ.

#### Ngày 51 (Thứ 2) — Đồng bộ Công nợ Khách hàng từ KiotViet
* 🎯 **Mục tiêu:** Lấy số dư công nợ phải thu của khách hàng để hiển thị cảnh báo trên Báo giá và Chi tiết khách hàng.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (60p) Viết hàm `SyncCustomerDebtsAsync()`: Gọi API công nợ khách hàng từ KiotViet; cập nhật trường `CurrentDebt` trong bảng `customers`.
  * (40p) Hiển thị thông tin công nợ trên giao diện: Khối cảnh báo màu đỏ tại `Pages/Customers/Detail.cshtml` nếu công nợ $> 50$ triệu; cảnh báo Sales khi tạo báo giá mới nếu khách hàng đang có nợ quá hạn.
  * (20p) Kiểm thử hiển thị công nợ.
* 📦 **Output:** Kiểm soát rủi ro tài chính trước khi tiếp tục bán hàng cho khách có nợ xấu.
* ✅ **DoD:** Khách hàng có công nợ trên KiotViet hiển thị số dư cảnh báo rõ ràng trên màn hình CRM.

#### Ngày 52 (Thứ 3) — API Tra cứu Tồn kho Realtime Kèm Cơ chế Memory Cache
* 🎯 **Mục tiêu:** Cho phép Sales kiểm tra số lượng tồn kho thực tế khi đang lên báo giá mà không gây quá tải API KiotViet.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (55p) Xây dựng API: `GET /api/kiotviet/stock/{productId}`: Dùng `IMemoryCache` lưu số lượng tồn trong $5$ phút; nếu miss cache mới gọi KiotViet lấy tồn khả dụng (OnHand - Reserved).
  * (45p) Tích hợp vào giao diện Báo giá (`Pages/Quotes/Create.cshtml`): Khi chọn sản phẩm, badge nhỏ hiển thị: *"Còn tồn: 145 cái"* ngay dưới tên sản phẩm.
  * (20p) Kiểm thử tốc độ phản hồi khi có cache và khi không có cache.
* 📦 **Output:** Tính năng kiểm tra hàng tồn thông minh, bảo vệ hạn ngạch API của bên thứ ba.
* ✅ **DoD:** Click tra tồn kho lần 1 mất 300ms; Click lần 2 mất 2ms (ăn cache); Sales biết ngay hàng còn hay hết.

#### Ngày 53 (Thứ 4) — Đợt Viết Unit Test Toàn Hệ Thống (Mục tiêu Coverage > 70%)
* 🎯 **Mục tiêu:** Nâng cao độ tin cậy của toàn bộ hệ thống trước khi bước vào giai đoạn kiểm thử an ninh.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Cài đặt package `coverlet.collector` để đo độ bao phủ mã nguồn (Code Coverage).
  * (65p) Bổ sung bộ Unit Tests: Test tính toán Phễu Cohort Dashboard, Test logic Task outcomes, Test xử lý ngoại lệ đồng bộ KiotViet khi dữ liệu thiếu trường bắt buộc.
  * (25p) Chạy lệnh đo độ bao phủ:
    ```bash
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
    ```
* 📦 **Output:** Bộ test suite vững chắc, đảm bảo hệ thống không bị lỗi tiềm ẩn.
* ✅ **DoD:** Độ bao phủ code (Code Coverage) của tầng `Crm.Business` đạt trên $70\%$; tất cả tests đều màu xanh.

#### Ngày 54 (Thứ 5) — Xây dựng Cơ chế Tự động Ghi Vết Kiểm toán (Audit Log Engine)
* 🎯 **Mục tiêu:** Tự động ghi lại lịch sử ai đã sửa gì, vào lúc nào, giá trị cũ/mới ra sao vào bảng `audit_logs`.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (65p) Override phương thức `SaveChangesAsync` trong `AppDbContext`: Quét danh sách Entity ở trạng thái `Added`, `Modified`, `Deleted`; lấy user info từ `IHttpContextAccessor`; chuyển `OriginalValues` và `CurrentValues` thành JSONB; ghi vào bảng `audit_logs` trong cùng transaction.
  * (35p) Viết giao diện xem lịch sử thay đổi (Audit Trail) tại tab Cài đặt của Admin.
  * (20p) Thử sửa đổi số điện thoại khách hàng và kiểm tra bản ghi trong bảng `audit_logs`.
* 📦 **Output:** Hệ thống có khả năng truy vết trách nhiệm 100% đối với mọi thay đổi dữ liệu.
* ✅ **DoD:** Sửa thông tin bất kỳ $\rightarrow$ Bảng `audit_logs` tự sinh bản ghi ghi rõ trường bị sửa, giá trị trước và sau khi sửa.

#### Ngày 55 (Thứ 6) — Đợt Tối ưu Hóa & Sửa Lỗi Tồn đọng (Zero-Debt Milestone)
* 🎯 **Mục tiêu:** Giải quyết triệt để toàn bộ danh sách bug phát hiện trong tuần và refactor mã nguồn.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (50p) Rà soát mã nguồn (Code Review): Xóa import thừa, chuẩn hóa tên biến, xóa dead-code.
  * (50p) Sửa chữa 3 lỗi giao diện nhỏ phát hiện trong quá trình sync dữ liệu KiotViet.
  * (20p) Chạy lại toàn bộ test suite và build app ở chế độ Release (`dotnet build -c Release`).
* 📦 **Output:** Mã nguồn tinh gọn, hoàn toàn sạch nợ kỹ thuật sau 11 tuần lập trình liên tục.
* ✅ **DoD:** Bản build Release không có warning; toàn bộ tính năng vận hành trơn tru trên môi trường dev.

#### Ngày 56 (Thứ 2) — Gia cố An ninh Ứng dụng Toàn diện
* 🎯 **Mục tiêu:** Thiết lập các chốt chặn phòng chống tấn công mạng phổ biến theo tiêu chuẩn OWASP.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Cấu hình Security Headers trong `Program.cs`: HSTS, X-Content-Type-Options, X-Frame-Options (chống Clickjacking), Content-Security-Policy cơ bản.
  * (35p) Cấu hình Rate Limiting sử dụng middleware tích hợp sẵn của .NET 8: Giới hạn tối đa 60 requests/phút đối với người dùng thông thường để chống DDoS/Spam request.
  * (35p) Bật xác thực Anti-Forgery Token (CSRF) bắt buộc cho 100% các request POST/PUT/DELETE.
  * (20p) Kiểm tra quét lỗ hổng cơ bản bằng công cụ Postman / Browser DevTools.
* 📦 **Output:** Ứng dụng được bảo vệ an toàn trước các nguy cơ tấn công Web cơ bản.
* ✅ **DoD:** Gửi request POST thiếu token CSRF bị từ chối với mã lỗi 400 Bad Request; spam request bị chặn với mã 429.

#### Ngày 57 (Thứ 3) — Tự động hóa Sao lưu Cơ sở Dữ liệu (Backup Script & Cron)
* 🎯 **Mục tiêu:** Viết script sao lưu tự động PostgreSQL hàng đêm và nén lưu trữ an toàn.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (50p) Viết Shell Script `scripts/backup.sh`:
    ```bash
    #!/bin/bash
    TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
    BACKUP_DIR="/var/backups/crm"
    mkdir -p $BACKUP_DIR
    pg_dump -U postgres -d crm_db -F c -b -v -f "$BACKUP_DIR/crm_$TIMESTAMP.dump"
    # Giữ lại các bản backup trong 30 ngày gần nhất, xóa bản cũ hơn
    find $BACKUP_DIR -type f -name "*.dump" -mtime +30 -exec rm {} \;
    ```
  * (40p) Thiết lập quyền thực thi (`chmod +x`) và gắn vào crontab hệ điều hành chạy lúc 01:00 sáng.
  * (30p) Thực hiện kịch bản Khôi phục thử nghiệm (Disaster Recovery Drill): Xóa thử 1 bảng và restore lại từ file dump thành công.
* 📦 **Output:** Quy trình phòng ngừa thảm họa dữ liệu hoạt động tự động và chuẩn xác.
* ✅ **DoD:** Chạy script sao lưu sinh ra file `.dump` nguyên vẹn; lệnh `pg_restore` khôi phục dữ liệu không lỗi.

#### Ngày 58 (Thứ 4) — Health Check Endpoint & Cảnh báo Tự động qua Telegram
* 🎯 **Mục tiêu:** Tạo endpoint giám sát sức khỏe dịch vụ và thông báo tức thì cho kỹ thuật viên khi gặp lỗi.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (40p) Cấu hình Health Checks trong .NET 8: Endpoint `/health` kiểm tra kết nối PostgreSQL và trạng thái Hangfire Server.
  * (50p) Xây dựng `TelegramAlertService`: Tích hợp Telegram Bot API gửi tin nhắn cảnh báo khi có Exception nghiêm trọng (`Fatal` từ Serilog), ổ cứng $> 90\%$, hoặc đồng bộ KiotViet mất kết nối 3 lần liên tiếp.
  * (30p) Thử ngắt kết nối database để verify tin nhắn cảnh báo bắn về điện thoại.
* 📦 **Output:** Hệ thống tự động giám sát và thông báo sự cố trong thời gian thực.
* ✅ **DoD:** Database gặp sự cố $\rightarrow$ Bot Telegram gửi tin nhắn cảnh báo chi tiết tới nhóm quản trị sau 5 giây.

#### Ngày 59 (Thứ 5) — Hoàn thiện Bộ Tài liệu Kỹ thuật Vận hành
* 🎯 **Mục tiêu:** Soạn thảo tài liệu bàn giao kỹ thuật đầy đủ để bất kỳ kỹ sư nào cũng có thể tiếp quản hệ thống.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Vẽ Sơ đồ Quan hệ Thực thể (ERD Diagram) hoàn chỉnh xuất ra định dạng hình ảnh và Markdown.
  * (45p) Viết tài liệu `docs/DEPLOYMENT_GUIDE.md`: Danh mục biến môi trường bắt buộc, hướng dẫn cài đặt PostgreSQL, Nginx, Systemd, quy trình Rollback khi bản cập nhật gặp sự cố.
  * (30p) Tạo Postman Collection mẫu lưu trong `docs/postman/` chứa đầy đủ các API nội bộ.
* 📦 **Output:** Bộ tài liệu bàn giao kỹ thuật chuyên nghiệp, rõ ràng.
* ✅ **DoD:** Người ngoài đọc tài liệu có thể tự thiết lập môi trường và cấu hình app mà không cần hỏi lại dev chính.

#### Ngày 60 (Thứ 6) — Rà soát An ninh & Sprint 6 Review
* 🎯 **Mục tiêu:** Tổng kiểm tra toàn bộ hệ thống từ mã nguồn đến hạ tầng bảo mật; tổng kết Sprint 6.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (40p) Kiểm tra cấu hình môi trường: Đảm bảo không còn bất kỳ secret key, password nào hard-code trong git repository (chuyển 100% sang Environment Variables).
  * (40p) Chạy thử nghiệm toàn bộ hệ thống ở chế độ mô phỏng tải cao.
  * (40p) **Sprint 6 Review & Retrospective:** Diễn tập khôi phục thảm họa (Disaster Recovery), rà soát nhật ký audit log và xác nhận trạng thái sẵn sàng Production.
* 📦 **Output:** Hệ sinh thái phần mềm đã sẵn sàng $100\%$ cho ngày triển khai Production.
* ✅ **DoD:** Không phát hiện lỗ hổng an ninh bảo mật; git repository hoàn toàn sạch thông tin nhạy cảm; Sprint 6 nghiệm thu thành công.

---

### SPRINT 7: TRIỂN KHAI PRODUCTION, ĐÀO TẠO & NGHIỆM THU (TUẦN 13 - 14)
* 🎯 **Sprint Goal:** Triển khai chính thức hệ thống lên máy chủ VPS Ubuntu (Nginx, Let's Encrypt HTTPS), nạp dữ liệu thực tế, đào tạo toàn bộ người dùng doanh nghiệp và ký biên bản nghiệm thu bàn giao dự án.
* ⏱ **Thời lượng:** 10 ngày làm việc (Ngày 61 $\rightarrow$ 70) = 20 giờ.

#### Ngày 61 (Thứ 2) — Thiết lập Môi trường Máy chủ VPS Ubuntu 22.04
* 🎯 **Mục tiêu:** Khởi tạo hạ tầng máy chủ ảo hóa trên Cloud sạch sẽ, an toàn và tối ưu tài nguyên.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (30p) Đăng nhập SSH VPS, cập nhật hệ thống: `apt update && apt upgrade -y`. Cấu hình Firewall UFW (chỉ mở cổng 22, 80, 443).
  * (40p) Cài đặt môi trường vận hành: .NET 8 ASP.NET Core Runtime, PostgreSQL 16 server, tạo User `crm_user` và Database `crm_prod` với mật khẩu an toàn.
  * (30p) Tối ưu hóa thông số PostgreSQL: Cấu hình `postgresql.conf` (bộ nhớ cache, connection pool phù hợp với RAM của VPS).
  * (20p) Kiểm tra kết nối database trên VPS thành công.
* 📦 **Output:** Máy chủ VPS sẵn sàng tiếp nhận mã nguồn triển khai.
* ✅ **DoD:** Đăng nhập máy chủ an toàn qua SSH Key; PostgreSQL sẵn sàng nhận kết nối cục bộ.

#### Ngày 62 (Thứ 3) — Triển khai Ứng dụng với Nginx & Systemd Service
* 🎯 **Mục tiêu:** Đóng gói ứng dụng, cấu hình dịch vụ tự khởi động cùng hệ thống và Proxy ngược qua Nginx.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (35p) Đóng gói và upload ứng dụng lên VPS:
    ```bash
    dotnet publish src/Crm.Web/Crm.Web.csproj -c Release -o /var/www/crm
    ```
  * (40p) Tạo Systemd Service: `/etc/systemd/system/crm.service` để quản lý tiến trình .NET, tự khởi động lại khi crash (`Restart=always`).
  * (35p) Cài đặt và cấu hình Nginx làm Reverse Proxy: Chuyển tiếp cổng 80 vào `http://localhost:5000`, thiết lập proxy headers.
  * (10p) Khởi động service và kiểm tra truy cập ứng dụng qua địa chỉ IP của VPS.
* 📦 **Output:** Ứng dụng CRM vận hành ổn định như một tiến trình ngầm (Daemon) trên Linux.
* ✅ **DoD:** Truy cập địa chỉ IP máy chủ trên trình duyệt $\rightarrow$ Màn hình đăng nhập CRM phản hồi mượt mà.

#### Ngày 63 (Thứ 4) — Cấu hình Tên miền Doanh nghiệp & Chứng chỉ Bảo mật SSL (HTTPS)
* 🎯 **Mục tiêu:** Gắn tên miền chính thức và kích hoạt chứng chỉ mã hóa SSL miễn phí qua Let's Encrypt.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (25p) Trỏ bản ghi DNS (Record A) của tên miền doanh nghiệp (ví dụ: `crm.congty.vn`) về địa chỉ IP của VPS.
  * (45p) Cài đặt Certbot và plugin Nginx:
    ```bash
    apt install certbot python3-certbot-nginx -y
    certbot --nginx -d crm.congty.vn
    ```
  * (30p) Cấu hình Nginx tự động chuyển hướng toàn bộ lưu lượng HTTP sang HTTPS an toàn.
  * (20p) Kiểm tra hạn chứng chỉ và cấu hình tự động gia hạn (Auto-renew cron test).
* 📦 **Output:** Website vận hành trên giao thức bảo mật HTTPS với ổ khóa xanh uy tín.
* ✅ **DoD:** Truy cập `http://crm.congty.vn` tự động nhảy sang `https://crm.congty.vn`, SSL hợp lệ 100%.

#### Ngày 64 (Thứ 5) — Nạp Dữ liệu Thực tế & Chạy Thử nghiệm Toàn diện End-to-End
* 🎯 **Mục tiêu:** Nạp toàn bộ dữ liệu thật của doanh nghiệp và kiểm thử toàn bộ luồng nghiệp vụ trên Production.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Nạp dữ liệu sản xuất chính thức: Khách hàng thật, Danh mục sản phẩm thật, Tài khoản người dùng cho toàn bộ nhân viên.
  * (50p) Thực hiện kiểm thử toàn bộ chu trình bán hàng thực tế trên Production: Đăng nhập Sales $\rightarrow$ Tạo Khách hàng $\rightarrow$ Tạo Cơ hội $\rightarrow$ Kéo thả Kanban $\rightarrow$ Lên Báo giá $\rightarrow$ Xuất file PDF $\rightarrow$ Gửi email có file đính kèm $\rightarrow$ Ghi nhận & Đóng Task $\rightarrow$ Xem Dashboard lãnh đạo.
  * (25p) Kích hoạt chạy thử Job đồng bộ KiotViet trên môi trường thật.
* 📦 **Output:** Hệ thống Production vận hành hoàn hảo với dữ liệu thực tế.
* ✅ **DoD:** Luồng nghiệp vụ từ Tiếp cận đến Chốt đơn và Báo giá chạy trơn tru, không có bất kỳ lỗi 500 nào.

#### Ngày 65 (Thứ 6) — Giám sát Chặt chẽ & Xử lý Phát sinh Ngày Đầu Vận hành
* 🎯 **Mục tiêu:** Trực chiến theo dõi log hệ thống, tinh chỉnh thông số máy chủ và vá lỗi phát sinh tức thì.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (40p) Theo dõi liên tục file log hệ thống trên VPS: `tail -f /var/www/crm/logs/crm-*.log`.
  * (40p) Theo dõi mức độ chiếm dụng phần cứng qua lệnh `htop`: RAM, CPU, I/O của PostgreSQL và ứng dụng .NET.
  * (30p) Xử lý các phát sinh về quyền truy cập thư mục hoặc tinh chỉnh timeout kết nối nếu cần.
  * (10p) Đóng băng phiên bản Production đầu tiên (Tag Git: `v1.0.0-prod`).
* 📦 **Output:** Hệ thống Production đạt trạng thái ổn định tuyệt đối (Rock-solid stability).
* ✅ **DoD:** Tỷ lệ uptime đạt $100\%$ trong suốt 24h chạy đầu tiên; log không ghi nhận lỗi ngoại lệ nghiêm trọng.

#### Ngày 66 (Thứ 2) — Soạn thảo Tài liệu Hướng dẫn Sử dụng (End-User Manual)
* 🎯 **Mục tiêu:** Viết cẩm nang hướng dẫn sử dụng giao diện dễ hiểu dành riêng cho nhân viên kinh doanh và kế toán.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (65p) Biên tập tài liệu Word/PDF (15 trang): Đăng nhập & Đổi mật khẩu; Tra cứu, Thêm mới & Chăm sóc Khách hàng; Kéo thả Cơ hội trên Pipeline Kanban; Tạo Báo giá nhanh & In file PDF; Xem & Đóng lịch làm việc hàng ngày.
  * (40p) Chụp ảnh màn hình thực tế, đóng khung đỏ các nút bấm quan trọng minh họa trực quan.
  * (15p) Xuất file PDF gửi ban giám đốc xem trước.
* 📦 **Output:** Cẩm nang sử dụng thân thiện, giúp người dùng không rành công nghệ vẫn thao tác dễ dàng.
* ✅ **DoD:** Tài liệu hoàn thành mạch lạc, hình ảnh rõ nét, ngôn từ thực tế không dùng thuật ngữ kỹ thuật khó hiểu.

#### Ngày 67 (Thứ 3) — Đào tạo Trực tiếp Buổi 1: Dành cho Đội ngũ Bán hàng (Sales Team)
* 🎯 **Mục tiêu:** Đào tạo đội ngũ kinh doanh làm chủ các tính năng quản lý khách hàng và cơ hội bán lẻ/sỉ.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (50p) Trình bày và Demo trực tiếp trên hệ thống thật: Tìm kiếm nhanh khách hàng tránh trùng lặp; ý nghĩa của huy hiệu sức khỏe (Xanh/Vàng/Đỏ); cập nhật tiến độ deal trên bảng Kanban.
  * (50p) Hướng dẫn từng nhân viên thực hành trực tiếp trên máy tính cá nhân của họ.
  * (20p) Giải đáp thắc mắc và ghi nhận các thói quen sử dụng của nhân viên.
* 📦 **Output:** 100% nhân viên kinh doanh biết cách đăng nhập và thao tác nghiệp vụ hàng ngày.
* ✅ **DoD:** Từng nhân sự Sales tự tay tạo được 1 khách hàng và 1 cơ hội trên hệ thống thành công.

#### Ngày 68 (Thứ 4) — Đào tạo Trực tiếp Buổi 2: Báo giá, Kế hoạch Hành động & Dashboard Lãnh đạo
* 🎯 **Mục tiêu:** Hướng dẫn module Báo giá cho Sales/Kế toán và hướng dẫn đọc Dashboard cho Ban Giám đốc.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Đào tạo Sales & Kế toán: Lập báo giá, áp chiết khấu, xuất file PDF chuẩn mẫu công ty, kiểm tra tồn kho KiotViet.
  * (45p) Đào tạo Ban Lãnh đạo: Cách đọc 4 chỉ số KPI chính; phân tích phễu rơi rụng khách hàng để cải thiện quy trình; theo dõi bảng xếp hạng vinh danh nhân viên xuất sắc.
  * (30p) Thực hành và hỏi đáp trực tiếp.
* 📦 **Output:** Toàn bộ các cấp trong doanh nghiệp nắm vững công cụ thuộc phạm vi trách nhiệm của mình.
* ✅ **DoD:** Sales xuất được báo giá PDF chuẩn gửi thử nghiệm; Lãnh đạo nắm được cách xem báo cáo trên điện thoại.

#### Ngày 69 (Thứ 5) — Hỗ trợ Người dùng Tại chỗ (On-site Support) & Thu thập Góp ý
* 🎯 **Mục tiêu:** Trực tiếp hỗ trợ người dùng trong ngày làm việc thực tế đầu tiên và xử lý vướng mắc tại chỗ.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (70p) Trực chiến hỗ trợ nhân viên kinh doanh thao tác thực tế với các khách hàng phát sinh trong ngày: hỗ trợ gỡ lỗi quên mật khẩu, hướng dẫn điều chỉnh thông tin báo giá khi khách yêu cầu chiết khấu đặc biệt.
  * (35p) Tổng hợp danh sách phản hồi (Feedback Log): Ghi nhận các điểm người dùng thấy tiện lợi hoặc còn bỡ ngỡ.
  * (15p) Tinh chỉnh nhỏ một vài nhãn văn bản hiển thị trên giao diện theo thói quen của công ty.
* 📦 **Output:** Người dùng xóa bỏ tâm lý ngại thay đổi, tự tin sử dụng hệ thống mới thay thế Excel cũ.
* ✅ **DoD:** Không có khiếu nại về gián đoạn công việc kinh doanh; dữ liệu ngày đầu được nhập đầy đủ vào CRM.

#### Ngày 70 (Thứ 6) — Nghiệm thu Dự án Chính thức, Bàn giao Toàn diện & Kết thúc Sprint 7
* 🎯 **Mục tiêu:** Tiến hành nghiệm thu chính thức với Chủ doanh nghiệp, bàn giao quyền quản trị và kết thúc toàn bộ dự án.
* ⏱ **Phân bổ thời gian (120 phút):**
  * (45p) Họp tổng kết nghiệm thu với Ban Giám đốc: Báo cáo kết quả so với mục tiêu ban đầu (hoàn thành 100% chức năng, đúng tiến độ 14 tuần / 7 Sprints, không vượt ngân sách); trình diễn dữ liệu thực tế đang chạy trên Production.
  * (45p) Bàn giao hồ sơ dự án: Master Admin Account, Tài khoản VPS Cloud, Quyền quản trị DNS Tên miền, Toàn bộ Mã nguồn Git Repository, Bộ tài liệu kỹ thuật và Hướng dẫn sử dụng.
  * (30p) Ký kết Biên bản Nghiệm thu Dự án và thống nhất chính sách bảo hành, hỗ trợ kỹ thuật trong 30 ngày tiếp theo.
* 📦 **Output:** Biên bản nghiệm thu dự án có chữ ký xác nhận của Ban Giám đốc; Bàn giao toàn diện thành công.
* ✅ **DoD:** Sếp ký duyệt nghiệm thu; hệ thống vận hành độc lập, ổn định; dự án kết thúc thắng lợi rực rỡ.

---

## PHẦN 4: MA TRẬN ĐỐI SOÁT THEO SPRINT & TIÊU CHÍ CHẤP THUẬN (DoD MATRIX)

| Sprint | Phân hệ nghiệp vụ | Tiêu chuẩn Kỹ thuật (Technical Specs) | Tiêu chí Chấp thuận Nghiệm thu (DoD) |
| :---: | :--- | :--- | :--- |
| **Sprint 1** | **Khách hàng 360** | EF Core 8, Soft-delete, Concurrency Token | Quản lý vòng đời khách hàng, tự sinh mã `KH-xxxx`, hiển thị đầy đủ 4 tabs thông tin, công nợ và dòng thời gian tương tác. |
| **Sprint 2** | **Cơ hội & Kanban** | SortableJS, HTMX, Optimistic UI Update | Kéo thả mượt mà giữa 6 cột, tự động cập nhật tổng giá trị cột tức thì, có cơ chế rollback khi ngắt mạng, lưu vết lịch sử chuyển stage. |
| **Sprint 3** | **Báo giá & PDF** | QuestPDF, ClosedXML, MailKit | Tính toán tài chính chuẩn xác từng đồng, xuất file PDF tiếng Việt sắc nét, gửi email đính kèm PDF trực tiếp đến khách hàng. |
| **Sprint 4** | **Kế hoạch Hành động & Hangfire** | State-machine Engine, Hangfire Recurring Jobs | Lọc việc Hôm nay/Quá hạn; 4 Recurring Jobs vận hành chính xác nửa đêm: cập nhật sức khỏe khách hàng, sinh việc nhắc nhở, đóng băng snapshot. |
| **Sprint 5** | **Dashboard & KiotViet Core** | Chart.js 4, Cohort Funnel, OAuth 2.0 Client Credentials | Dashboard tải $< 300ms$, phễu giảm dần, biểu đồ 6 tháng tách khách mới/cũ; đồng bộ 2 chiều Khách hàng, Hóa đơn và Sản phẩm KiotViet. |
| **Sprint 6** | **Tồn kho, Audit Trail & Bảo mật** | Memory Cache, EF Core Audit Engine, OWASP Hardening | Tra cứu tồn kho tức thì có cache; Audit trail 100% thay đổi; Rate Limiting 60 req/phút; backup DB hàng đêm; cảnh báo qua Telegram. |
| **Sprint 7** | **Hạ tầng Production & Chuyển giao** | Linux VPS, Nginx Reverse Proxy, Let's Encrypt HTTPS | HTTPS ổ khóa xanh; nạp dữ liệu thật; đào tạo 100% Sales và Kế toán; ký biên bản nghiệm thu bàn giao chính thức. |

---

## PHẦN 5: BẢNG DỰ TOÀN THỜI GIAN ĐỆM DỰ PHÒNG THEO SPRINT (BUFFER ALLOCATION)

Dự án được bố trí **13 ngày đệm chiến lược** (tương đương 26 giờ làm việc độc lập) phân bổ đều vào 7 Sprints nhằm đảm bảo tiến độ không bao giờ bị trễ:

1. **Buffer Học công nghệ mới (5 ngày):**
   * Sprint 1: HTMX Proof-of-concept (Ngày 1).
   * Sprint 2: SortableJS integration (Ngày 18).
   * Sprint 3: QuestPDF layout engine (Ngày 24).
   * Sprint 4: Hangfire Background setup (Ngày 31).
   * Sprint 5: Chart.js rendering (Ngày 41).
2. **Buffer Refactor & Dọn nợ kỹ thuật (3 ngày):** Ngày 35 (Sprint 4), Ngày 55 (Sprint 6), Ngày 60 (Sprint 6).
3. **Buffer Kiểm thử chuyên sâu & Viết Test (3 ngày):** Ngày 35 (Sprint 4), Ngày 53 (Sprint 6), Ngày 60 (Sprint 6).
4. **Buffer Trực chiến & Khắc phục sự cố Production (1 ngày):** Ngày 65 (Sprint 7).
5. **Buffer Hỗ trợ người dùng tại chỗ (1 ngày):** Ngày 69 (Sprint 7).

**Cam kết chất lượng:** Bằng việc chia nhỏ 14 tuần thành 7 Sprints nhịp nhàng (mỗi Sprint có mục tiêu, deliverable và nghiệm thu riêng), dự án loại bỏ hoàn toàn rủi ro quá tải (burnout), đảm bảo người phát triển đơn lẻ vẫn kiểm soát 100% chất lượng mã nguồn và bàn giao sản phẩm chuẩn chỉnh cho doanh nghiệp.