# day 1 completed 28/9/2026
# DAILY LOG — SPRINT 1 (NGÀY 2)
**Ngày thực hiện:** 30/09/2026  
**Dự án:** CRM Mini (.NET 10 + PostgreSQL 18)  
**Trạng thái:** Hoàn thành 100% mục tiêu Ngày 2

---

### 1. Công việc đã hoàn thành
- **Môi trường & Quyền:**
  - Cấu hình database `crm_db` chuẩn encoding `UTF-8`.
  - Phân quyền owner schema `public` cho user `crm_user` (mật khẩu: `sa`).
- **Schema & DDL:**
  - Khởi tạo thành công 16 bảng lõi (`schema.sql`).
  - Cấu hình 70 indexes và 22 Foreign Keys đảm bảo toàn vẹn dữ liệu.
  - Tạo 7 triggers tự động cập nhật trường `updated_at`.
  - Khởi tạo 2 views báo cáo: `v_customer_health_summary` và `v_opportunity_pipeline`.
- **Seed Data:**
  - Nạp Master data: 6 stages (`opportunity_stages`), 4 danh mục (`categories`).
  - Nạp bộ dữ liệu mẫu (`02_demo_data.sql`): 5 customers, 5 contacts, 10 products, 5 opportunities, 15 stage histories, 2 quotes (6 items), 5 tasks, 5 interactions.
- **Nghiệm thu (Checklist):**
  - Đạt 10/10 tiêu chí kiểm thử (kiểm tra trigger, view, đối soát số lượng bản ghi).

---

### 2. Khó khăn & Giải pháp
- **Vấn đề:** Terminal Windows gặp lỗi mã hóa `WIN1252` với ký tự tiếng Việt khi chạy script SQL.
- **Giải pháp:** Thiết lập `SET client_encoding = 'UTF8';` trong SQL script và gán biến môi trường `$env:PGCLIENTENCODING = "utf-8"` trước khi thực thi `psql`.

---

### 3. Kế hoạch Ngày 3
- Tích hợp **ASP.NET Core Identity** vào dự án `Crm.Web`.
- Cấu hình Authentication/Authorization (Cookie, JWT nếu cần).
- Tạo luồng Login, Logout, Quản lý Roles (Admin, Sales, Manager).# DAILY LOG — SPRINT 1 (NGÀY 2)
**Ngày thực hiện:** 30/09/2026  
**Dự án:** CRM Mini (.NET 10 + PostgreSQL 18)  
**Trạng thái:** Hoàn thành 100% mục tiêu Ngày 2

---

### 1. Công việc đã hoàn thành
- **Môi trường & Quyền:**
  - Cấu hình database `crm_db` chuẩn encoding `UTF-8`.
  - Phân quyền owner schema `public` cho user `crm_user` (mật khẩu: `sa`).
- **Schema & DDL:**
  - Khởi tạo thành công 16 bảng lõi (`schema.sql`).
  - Cấu hình 70 indexes và 22 Foreign Keys đảm bảo toàn vẹn dữ liệu.
  - Tạo 7 triggers tự động cập nhật trường `updated_at`.
  - Khởi tạo 2 views báo cáo: `v_customer_health_summary` và `v_opportunity_pipeline`.
- **Seed Data:**
  - Nạp Master data: 6 stages (`opportunity_stages`), 4 danh mục (`categories`).
  - Nạp bộ dữ liệu mẫu (`02_demo_data.sql`): 5 customers, 5 contacts, 10 products, 5 opportunities, 15 stage histories, 2 quotes (6 items), 5 tasks, 5 interactions.
- **Nghiệm thu (Checklist):**
  - Đạt 10/10 tiêu chí kiểm thử (kiểm tra trigger, view, đối soát số lượng bản ghi).

---

### 2. Khó khăn & Giải pháp
- **Vấn đề:** Terminal Windows gặp lỗi mã hóa `WIN1252` với ký tự tiếng Việt khi chạy script SQL.
- **Giải pháp:** Thiết lập `SET client_encoding = 'UTF8';` trong SQL script và gán biến môi trường `$env:PGCLIENTENCODING = "utf-8"` trước khi thực thi `psql`.

---

### 3. Kế hoạch Ngày 3
- Tích hợp **ASP.NET Core Identity** vào dự án `Crm.Web`.
- Cấu hình Authentication/Authorization (Cookie, JWT nếu cần).
- Tạo luồng Login, Logout, Quản lý Roles (Admin, Sales, Manager).

# Daily Log — Day 3 / Sprint 1

## Done
- [x] ApplicationUser, UserProfile, Team entities
- [x] AppDbContext với 2 DbSet (UserProfile, Team)
- [x] 7 bảng Identity + user_profiles + teams trong DB
- [x] 5 FK constraints
- [x] Seed 4 roles + 5 users
- [x] Trang Login MudBlazor hiển thị đẹp
- [x] Login/Logout qua MVC Controller + JS Interop
- [x] AuthorizeRouteView bảo vệ toàn app
- [x] Test login admin@crm.local / Admin@2026: PASS

## Credentials
- Admin:       admin@crm.local / Admin@2026
- Sales 1:     sales1@crm.local / Sales@2026
- Sales 2:     sales2@crm.local / Sales@2026
- Manager:     manager@crm.local / Manager@2026
- Accountant:  accountant@crm.local / Acc@2026

## Next Steps (Day 4)
- Scaffold 16 entities từ database bằng dotnet-ef
- Thêm DbSet vào AppDbContext
- Seed data demo (customers, opportunities, ...)

# Daily Log — Day 5 / Sprint 1: Foundation (Enums, Helpers, Layout, Theme & Serilog)

* **Ngày thực hiện:** Sprint 1 — Day 5

* **Target Framework:** .NET 10 | MudBlazor | Serilog

* **Trạng thái:** Hoàn thành 100% mục tiêu (Build succeeded, 0 Warning, 0 Error, Zero-Inline Audit PASS)

## 1. Hạng mục đã hoàn tất (Done)

* \[x\] **Pre-flight & Cleanup:**

  * Git checkpoint an toàn trước khi vào phiên làm việc.

  * Quét và chuyển đổi toàn bộ inline style tĩnh sang class CSS tương ứng tại `Login.razor` và `AccessDenied.razor`.

* \[x\] **5 Domain Enums (`src/Crm.Domain/Enums`):**

  * `CustomerHealth.cs`: New, Healthy, NeedAttention, AtRisk, Dormant, Churned.

  * `SalesTaskStatus.cs`: Pending, InProgress, Completed, Skipped, Cancelled (tránh trùng tên BCL).

  * `TaskOutcome.cs`: 8 trạng thái kết quả tương tác khách hàng.

  * `QuoteStatus.cs`: Draft, PendingApproval, Sent, Accepted, Rejected, Expired.

  * `InteractionType.cs`: Call, Meeting, Email, Zalo, Visit.

* \[x\] **2 Business Helpers (`src/Crm.Business/Helpers`):**

  * `CurrencyHelper.cs`: Định dạng số tiền chuẩn văn hóa Việt Nam (`vi-VN`), hỗ trợ `FormatShort` rút gọn (K, tr, tỷ) tối ưu cho Mobile.

  * `DateHelper.cs`: Chuyển đổi hai chiều UTC $\leftrightarrow$ GMT+7 với cơ chế fallback 3 tầng chống crash trên môi trường Linux/Docker.

* \[x\] **Theme tập trung (`src/Crm.Web/Theme`):**

  * `CrmTheme.cs`: Khai báo bảng màu `PaletteLight` và `PaletteDark`, cấu hình bán kính viền mặc định.

* \[x\] **Master Layout & Điều hướng (`src/Crm.Web/Components/Layout`):**

  * `EmptyLayout.razor`: Dành cho trang độc lập (Login, AccessDenied) với các Provider cốt lõi.

  * `MainLayout.razor` & `MainLayout.razor.css`: Điều hướng Responsive với Scoped CSS (`::deep`), chuyển đổi Dark/Light mode tức thì, đóng mở Drawer và nút Đăng xuất qua JS Interop.

  * `NavMenu.razor`: Cấu hình 6 menu chức năng (Dashboard kích hoạt, 5 module nghiệp vụ đặt trạng thái disabled chờ Sprint tiếp theo).

* \[x\] **Hạ tầng Logging (Serilog):**

  * Cài đặt `Serilog.AspNetCore (8.0.3)` và `Serilog.Sinks.File (5.0.0)`.

  * Cấu hình file `appsettings.json` ghi song song ra Console và file xoay vòng hàng ngày: `logs/crm-.log`.

  * Cấu hình Bootstrap Logger trong `Program.cs` bắt lỗi crash ngay từ pha khởi tạo host.

* \[x\] **Bảng điều khiển trang chủ (`Home.razor`):**

  * Thiết kế Dashboard tóm tắt tiến độ Day 5 sử dụng các thẻ MudBlazor (`MudCard`, `MudAvatar`, `MudList`, `MudIcon`).

  * Bảo vệ trang bằng directive `@attribute [Authorize]`.

* \[x\] **Kiểm tra chất lượng mã nguồn (Quality Gate):**

  * Zero-Inline CSS Audit đạt chuẩn 100% không chứa inline style tĩnh.

  * Kiểm tra E2E thủ công qua `dotnet run`: Đăng nhập, đổi theme, mở drawer và kiểm tra file log sinh thực tế.

## 2. Số liệu kỹ thuật (Metrics & Deliverables)

| 

| **Hạng mục** | **Số lượng / Kết quả** | **Chi tiết** | 
| **Domain Enums** | 5 files | `Crm.Domain/Enums/*.cs` | 
| **Business Helpers** | 2 files | `CurrencyHelper.cs`, `DateHelper.cs` | 
| **Web Layout & Theme** | 5 files | `CrmTheme.cs`, `EmptyLayout.razor`, `MainLayout.razor`, `MainLayout.razor.css`, `NavMenu.razor` | 
| **Web Dashboard** | 1 file | `Home.razor` | 
| **Trạng thái Build** | PASS | 0 Error, 0 Warning (`dotnet build`) | 
| **CSS Audit** | PASS | 0 vi phạm inline style tĩnh trên toàn bộ cây thư mục `.razor` | 
| **Vị trí Log File** | Active | `src/Crm.Web/logs/crm-YYYYMMDD.log` | 

## 3. Nhật ký sự cố & Cách khắc phục (Troubleshooting Matrix)

| **Mã lỗi / Hiện tượng** | **Vị trí phát sinh** | **Nguyên nhân gốc** | **Biện pháp xử lý dứt điểm** | 
| **CS0103: The name 'CrmTheme' does not exist** | `EmptyLayout.razor`  `MainLayout.razor` | Layout chưa nhận diện namespace do file `_Imports.razor` chưa nạp hoặc cache biên dịch chưa nhận. | Thêm trực tiếp `@using Crm.Web.Theme` vào đầu file layout hoặc chuẩn hóa trong `_Imports.razor`. | 
| **Warning MUD0002: Illegal Attribute 'Title'** | `MainLayout.razor` | `MudIconButton` không hỗ trợ thuộc tính viết hoa `Title` theo quy chuẩn MudBlazor analyzer. | Đổi thành chữ thường `title="..."` hoặc thuộc tính trợ năng `aria-label="..."`. | 
| **CS0128: A local variable named 'builder' is already defined** | `Program.cs` | Khai báo trùng lặp `var builder = WebApplication.CreateBuilder(args);` khi chèn cấu hình Bootstrap Logger. | Giữ duy nhất 1 lần khai báo `var builder` sau khối cấu hình `Log.Logger`. | 
| **CS0246: The type or namespace name 'Authorize' could not be found** | `Home.razor` | Dùng `@attribute [Authorize]` nhưng thiếu namespace `Microsoft.AspNetCore.Authorization`. | Bổ sung `@using Microsoft.AspNetCore.Authorization` vào đầu file hoặc `_Imports.razor`. | 
| **CS0104: Xung đột namespace `Color`** | Các file Razor có dùng Chart | Xung đột định danh enum `Color` giữa `ApexCharts.Color` và `MudBlazor.Color`. | Chỉ định tường minh `MudBlazor.Color.Primary` thay vì gọi tắt `Color.Primary`. | 
| **Sai vị trí cấu hình `.gitignore`** | Thư mục giải pháp | File `.gitignore` bị tạo nhầm trong `CrmSolution` thay vì thư mục gốc của repository. | Xóa file thừa và thêm quy tắc bỏ qua `**/logs/` vào file `.gitignore` ở gốc dự án. | 
| **Vi phạm Zero-Inline CSS Audit** | `Login.razor`  `AccessDenied.razor` | Tồn tại các thẻ có `Style="..."` tĩnh trên `MudPaper` và `MudIcon`. | Di chuyển toàn bộ định dạng tĩnh vào các class tương ứng trong file `.razor.css`. | 

## 4. Kiến thức & Bài học cốt lõi (Key Learnings)

1. **Quy chuẩn Zero-Inline Styling:**

   * Việc tách rời hoàn toàn CSS tĩnh vào file scoped `.razor.css` giúp giao diện độc lập với logic C#, không bị đè style ngoài ý muốn và đáp ứng chuẩn thiết kế enterprise.

2. **Cơ chế Scoped CSS với thư viện bên thứ ba (`::deep`):**

   * Blazor Scoped CSS gắn mã hash định danh (`b-xxxxxx`) vào phần tử HTML thuần. Do các component của MudBlazor (`<MudLayout>`, `<MudAppBar>`) là C# Razor Components nên không nhận trực tiếp hash này. Bắt buộc cần thẻ HTML bao ngoài (ví dụ `<div class="crm-layout-wrapper">`) làm điểm neo cho selector `.crm-layout-wrapper ::deep .mud-...`.

3. **Cơ chế Fallback múi giờ 3 tầng (`DateHelper`):**

   * Windows Host sử dụng Registry ID (`SE Asia Standard Time`).

   * Linux/Docker sử dụng IANA ID (`Asia/Ho_Chi_Minh`).

   * Alpine Container tối giản: Tự động fallback sang `CustomTimeZone` mang nhãn `ICT` với độ lệch cố định $+07:00$. Thiết kế này đảm bảo an toàn tuyệt đối khi đóng gói container.

4. **Bootstrap Logging với Serilog:**

   * Khởi tạo `Log.Logger` và nạp cấu hình `appsettings.json` trước lệnh `WebApplication.CreateBuilder` đảm bảo mọi lỗi nghiêm trọng trong quá trình Dependency Injection hoặc khởi tạo service (Startup Crash) đều được ghi nhận đầy đủ vào file log.

## 5. Kế hoạch tiếp theo (Next Steps — Day 6)

* Xây dựng tầng truy cập dữ liệu: `CustomerRepository` và giao diện `ICustomerRepository`.

* Phát triển tầng nghiệp vụ: `CustomerService` và DTOs tương ứng.

* Thiết lập bộ Unit Tests kiểm thử toàn bộ luồng nghiệp vụ của `CustomerService`.

* Xây dựng màn hình danh sách khách hàng (`/customers`) sử dụng `MudTable` hỗ trợ phân trang, tìm kiếm thời gian thực và lọc trạng thái sức khỏe (`CustomerHealth`).
# Daily Log — Day 6 / Sprint 1: Kế hoạch Customer Repository & Service Unit Tests

* **Ngày thực hiện:** Sprint 1 — Day 6 (Plan/Runbook Generation)
* **Target Framework:** .NET 10 | xUnit | Moq

## 1. Hạng mục đã hoàn tất (Done)
- [x] Tạo file Actionable Prompts cho Day 6 tại `docs/sprints/sprint_1/prompts/day6_prompts.md`.
- [x] Tiến hành Sandbox Experimentation (Dry-run mã C#): Xây dựng giả lập `CustomerRepository` và `CustomerService`.
- [x] Áp dụng thư viện `MockQueryable.Moq` và `.BuildMock()` để bypass lỗi `ExecuteAsync` của EF Core trong Unit Tests.
- [x] Đạt Passed! 100% (3/3 Tests Passed) trong môi trường thử nghiệm.
- [x] Thu thập Real Output và biên soạn thành Runbook Markdown chuẩn Dual-Layer tại `docs/sprints/sprint_1/runbooks/Day_6_Sprint_1_Customer_Repository_Service.md`.
- [x] Khôi phục toàn bộ mã nguồn về sạch (chỉ giữ lại file Runbook và Prompts).

## 2. Số liệu kỹ thuật
- **Tests Passed (Sandbox):** 3
- **Runbook:** 1 file Markdown hoàn thiện.
- **Prompts:** 3 phiên làm việc băm nhỏ.

## 3. Bài học cốt lõi (Meta-Skills)
- Củng cố kỹ năng `[K69] Self-Correction & Context Assimilation`: Tự lùng sục codebase thay vì đặt câu hỏi ngớ ngẩn với user.
- Thấy được lỗi thực tế `IQueryable không hỗ trợ Async` khi Unit Test Service, nhờ có thói quen Sandbox Experimentation (`K70`), đã fix bằng `MockQueryable.Moq` trước khi ghi vào Runbook.

## Next Steps (Phiên sau)
- User hoặc AI sẽ sử dụng prompt đã tạo để thực sự sinh mã nguồn vào dự án cho Day 6.
- Tiến tới Day 7: Xây dựng màn hình hiển thị danh sách Khách hàng trên MudBlazor (`Customers/Index.razor`).

---

## [LEARN] RCA & Đúc kết khái niệm (K51, K65, K67) - Ngày 4 (Sửa lỗi mapping EF Core)

### 1. Root Cause Analysis (K51)
- **Triệu chứng:** Cấu hình EF Core Fluent API bị thiếu mapping tường minh (`.HasForeignKey()`), có nguy cơ sinh shadow properties hoặc hiểu sai quan hệ (đặc biệt là 1-Nhiều ở `Team` và `UserProfile`). Script Scaffold cũng vô tình xóa các config gốc do Regex sai.
- **Nguyên nhân gốc:**
  1. Trong script `AppDbContextSetup.ps1`, một regex (`-replace '\.HasConstraintName\("[^"]+"\)', ''`) được dùng để "dọn dẹp" config tự sinh nhưng lại gây side-effect xóa luôn các thiết lập liên quan đến ràng buộc (Constraint).
  2. Các file cấu hình viết tay như `TeamConfiguration` có `.WithMany()` bị để trống và thiếu `.HasForeignKey()` đi kèm.
- **Giải pháp:**
  1. Xóa lệnh regex replace sai trái trong `AppDbContextSetup.ps1`.
  2. Sửa trực tiếp các file configuration: Cập nhật `TeamConfiguration` với `.WithMany().HasForeignKey(t => t.ManagerId)`.
- **Xác nhận:** Đã viết file test xUnit sử dụng `Microsoft.EntityFrameworkCore.InMemory` duyệt qua metadata của EF (`context.Model.GetEntityTypes()`). Kết quả cho thấy 0 Shadow Properties và đúng chuẩn cấu hình One-To-Many/One-To-One.

### 2. Concept Mastery (K65)
- **EF Core Model Traversing (K70):** Hiểu rõ cách móc vào `context.Model` trong runtime để duyệt `GetEntityTypes()`, `GetForeignKeys()`, và kiểm tra `IsShadowProperty()`. Giúp test schema trước khi migrate/run mà không cần phụ thuộc vào console error.
- **Quyền lực của Fluent API (K48):** Dù có attribute DataAnnotation ở model, nhưng nếu trong Fluent API có khai báo `WithMany()` lửng lơ, EF Core sẽ tạo ra mapping ảo. Bắt buộc phải tường minh 2 chiều.
- **Hệ quả của Code Generation (K64):** Tuyệt đối cẩn trọng khi dùng Regex/Script để chỉnh sửa code tự sinh của ORM (Scaffold) vì rất dễ xóa nhầm logic quan trọng.

### 3. Kỹ năng mở rộng (K71)
- **Dọn dẹp Sandbox:** Áp dụng K71 xóa mọi rác thải sinh ra trong quá trình Sandbox (K70) sau khi debug thành công, giữ cho Codebase sạch sẽ trước khi commit.
