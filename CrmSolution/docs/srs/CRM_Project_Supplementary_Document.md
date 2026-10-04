# TÀI LIỆU BỔ SUNG DỰ ÁN CRM (MASTER COMPREHENSIVE DOCUMENT)

* **Mục đích:** Tập hợp tất cả tài liệu kỹ thuật, nghiệp vụ, thiết kế giao diện và vận hành còn thiếu vào một tài liệu duy nhất.
* **Nguyên tắc:** Ngắn gọn · Thống nhất với đặc tả · Không trùng lặp · Sẵn sàng thực thi.

---

## 1. TỔNG QUAN DỰ ÁN (PRD-LITE)

### 1.1 Bối cảnh
Doanh nghiệp nhỏ bán B2B vật tư tiêu hao cho nhà máy cơ khí. Đã dùng KiotViet cho bán hàng, quản lý kho, xuất hóa đơn và theo dõi công nợ. Doanh nghiệp đang thiếu một công cụ chuyên biệt để quản lý phễu khách hàng tiềm năng, chăm sóc định kỳ và giám sát cơ hội bán sỉ/lẻ.

### 1.2 Mục tiêu SMART
* **S (Specific):** Xây dựng hệ thống CRM quản lý khách hàng 360 độ, cơ hội theo pipeline Kanban, tự động hóa nhắc lịch chăm sóc và đồng bộ dữ liệu hai chiều với KiotViet.
* **M (Measurable):** Phục vụ 10 người dùng nội bộ vận hành đồng thời, tỷ lệ chuyển đổi qua phễu đạt $\ge 10\%$.
* **A (Achievable):** Triển khai theo mô hình 7 Sprints (14 tuần · 5 ngày/tuần · 2 giờ/ngày = 140 giờ chuẩn do 1 Solo Full-stack .NET 10 Developer phụ trách).
* **R (Relevant):** Tăng tỷ lệ mua lặp lại, kiểm soát khách hàng có nguy cơ rời bỏ (At-Risk/Dormant), không bỏ sót đầu mối bán lẻ/sỉ.
* **T (Time-bound):** Hoàn thành toàn bộ quy trình từ lập trình đến triển khai production và bàn giao trong 14 tuần.

### 1.3 Phạm vi
* **Trong phạm vi (In-scope):** Quản lý khách hàng, liên hệ, cơ hội bán hàng, báo giá & xuất PDF/Email, nhiệm vụ (sales tasks), bảng điều khiển (dashboard) lãnh đạo, đồng bộ ngầm KiotViet (khách hàng, đơn hàng 90 ngày, danh mục sản phẩm, công nợ, tra cứu tồn kho).
* **Ngoài phạm vi (Out-of-scope):** Quản lý chi tiết nhập-xuất-tồn kho vật lý, tạo hóa đơn bán lẻ trực tiếp, quản lý phiếu thu tiền (toàn bộ do KiotViet đảm nhiệm).

### 1.4 Đối tượng người dùng
| Vai trò | Số lượng | Nhu cầu chính |
|---|---|---|
| **Sales** | 6 | Tra cứu danh bạ, chăm sóc khách theo chu kỳ, tạo cơ hội, kéo thả Kanban, lập báo giá, nhận task nhắc việc tự động. |
| **Manager** | 2 | Xem dashboard tổng quan 4 KPI, phân tích phễu Cohort, duyệt báo giá, theo dõi hiệu suất nhân viên. |
| **Accountant** | 1 | Tra cứu nhanh công nợ khách hàng, kiểm soát danh sách báo giá đã phát hành. |
| **Admin** | 1 | Phân quyền tài khoản (RBAC), kiểm tra log lỗi hệ thống, giám sát tác vụ nền Hangfire, sao lưu dữ liệu. |

### 1.5 Tiêu chí nghiệm thu cốt lõi
* 10 user đăng nhập đúng phân quyền, sử dụng mượt mà 4 phân hệ chính.
* Dashboard hiển thị đúng 4 thẻ KPI, phễu bán hàng Cohort giảm dần, tỷ lệ khách hàng đạt đủ $100.0\%$, biểu đồ doanh thu 6 tháng.
* Job đồng bộ KiotViet chạy định kỳ 15 phút/lần chống nghẽn và ghi nhận log đầy đủ.
* Sao lưu database tự động hàng đêm lúc 01:00 AM, kiểm thử khôi phục (restore) thành công.
* Đầy đủ cẩm nang sử dụng (User Manual) và tài liệu bàn giao kỹ thuật (Runbook).

---

## 2. USER STORIES & ACCEPTANCE CRITERIA

### 2.1 Phân hệ Nhân viên Kinh doanh (Sales)

| # | User Story | Acceptance Criteria (Tiêu chí chấp thuận) |
|---|---|---|
| **S1** | Xem danh sách khách cần chăm sóc hôm nay | Màn hình mặc định lọc tab "Hôm nay", sắp xếp theo hạn xử lý gần nhất, hiển thị rõ badge sức khỏe khách hàng. |
| **S2** | Xem hồ sơ khách hàng 360 độ | Cung cấp 4 tabs thông tin: Thông tin chung, Danh bạ liên hệ, Cơ hội theo khách, và Dòng thời gian tương tác. |
| **S3** | Ghi nhận tương tác sau liên hệ | MudDialog với 8 nhánh Outcome; tự động sinh task nhắc lại hoặc cơ hội tương ứng sau khi submit mà không reload trang. |
| **S4** | Tạo cơ hội bán hàng mới | Tìm kiếm khách hàng qua Blazor data binding debounce 300ms, tự động sinh mã `OPP-XXXX`, gán đúng stage ban đầu. |
| **S5** | Kéo thả cơ hội trên Kanban | Kéo thả chuyển cột mượt mà (MudBlazor Drag and Drop), cập nhật stage và ghi lịch sử vết; tự rollback vị trí nếu mất mạng hoặc API lỗi. |
| **S6** | Tạo báo giá nhiều mặt hàng | Bảng sản phẩm động (thêm/xóa dòng), tính toán chiết khấu, VAT và tổng tiền thời gian thực bằng Blazor. |
| **S7** | Xuất báo giá định dạng PDF | Render file PDF bằng QuestPDF chuẩn in ấn A4, không lỗi font Unicode tiếng Việt, định dạng tiền tệ VND chuẩn. |
| **S8** | Gửi email báo giá tự động | Gửi qua MailKit đính kèm file PDF in-memory, dùng template HTML trang trọng, ghi log tự động vào bảng `interactions`. |
| **S9** | Đóng task công việc hàng ngày | Modal đóng task ghi kết quả nhanh; task tự động ẩn khỏi tab "Hôm nay" và cập nhật chỉ số KPI trên header. |

### 2.2 Phân hệ Quản lý (Manager)

| # | User Story | Acceptance Criteria (Tiêu chí chấp thuận) |
|---|---|---|
| **M1** | Xem Dashboard điều hành kinh doanh | Hiển thị 4 thẻ KPI, phễu Cohort, Donut chart tỷ trọng khách hàng và biểu đồ doanh thu 6 tháng (tải $< 300ms$). |
| **M2** | Lọc Dashboard theo khoảng thời gian | Hỗ trợ lọc theo tháng/quý; nếu xem dữ liệu quá khứ thì đọc trực tiếp từ bảng snapshot với độ trễ $< 10ms$. |
| **M3** | Đánh giá hiệu suất đội ngũ Sales | Bảng Top 5 nhân sự xếp hạng theo doanh số thực tế mang về và số lượng hợp đồng đã chốt thành công. |
| **M4** | Duyệt báo giá vượt thẩm quyền | Nút "Duyệt" trực tiếp tại trang chi tiết báo giá; tự động kích hoạt thông báo cho Sales phụ trách. |

### 2.3 Phân hệ Kế toán (Accountant)

| # | User Story | Acceptance Criteria (Tiêu chí chấp thuận) |
|---|---|---|
| **A1** | Xem công nợ khách hàng | Lấy số dư công nợ thời gian thực từ KiotViet; hiển thị khối cảnh báo màu đỏ nếu nợ vượt hạn mức. |
| **A2** | Kiểm soát danh sách báo giá | Bộ lọc báo giá theo trạng thái (Draft, Sent, Accepted, Rejected) và theo đối tác khách buôn/lẻ. |

### 2.4 Phân hệ Quản trị viên (Admin)

| # | User Story | Acceptance Criteria (Tiêu chí chấp thuận) |
|---|---|---|
| **AD1** | Quản lý tài khoản & phân quyền | Tạo mới/sửa user, gán vai trò RBAC, vô hiệu hóa tài khoản (`is_active = false`) mà không xóa dữ liệu lịch sử. |
| **AD2** | Tra cứu nhật ký vết (Audit Trail) | Xem chi tiết ai đã sửa trường nào, giá trị cũ/mới ra sao và xem log hệ thống từ Serilog. |
| **AD3** | Kích hoạt tác vụ nền thủ công | Truy cập `/hangfire` (có bảo vệ quyền ADMIN), bấm nút "Trigger Now" để chạy kiểm tra job. |

---

## 3. DATA DICTIONARY (CÁC BẢNG & TRƯỜNG CỐT LÕI)

### 3.1 Bảng `customers` (Khách hàng)

| Tên trường | Kiểu dữ liệu | Ý nghĩa | Ràng buộc & Ghi chú |
|---|---|---|---|
| `id` | UUID | Khóa chính | PK, Default `gen_random_uuid()` |
| `code` | VARCHAR(20) | Mã khách hàng | UNIQUE, Format chuẩn `KH-XXXX` |
| `name` | VARCHAR(255) | Tên doanh nghiệp/pháp nhân | NOT NULL |
| `tax_code` | VARCHAR(50) | Mã số thuế | Index tra cứu |
| `phone` | VARCHAR(50) | Số điện thoại chính | NOT NULL, Index |
| `health_status` | INT | Trạng thái sức khỏe | 0: New, 1: Good, 2: Fair, 3: AtRisk, 4: Churned |
| `average_cycle_days` | INT | Chu kỳ mua hàng trung bình | Default 30 ngày ($\ge 1$) |
| `last_order_date` | TIMESTAMPTZ | Ngày phát sinh đơn hàng cuối | Đồng bộ tự động từ KiotViet |
| `next_contact_due` | TIMESTAMPTZ | Hạn liên hệ chăm sóc tiếp theo | Tự động tính: $LastOrderDate + Cycle - 2$ ngày |
| `revenue_90d` | NUMERIC(18,2) | Tổng doanh số 90 ngày gần nhất | Tính từ hóa đơn KiotViet |
| `current_debt` | NUMERIC(18,2) | Dư nợ công nợ hiện tại | Lấy từ KiotViet |
| `assigned_user_id` | UUID | Sales phụ trách chính | FK liên kết bảng Users |
| `is_deleted` | BOOLEAN | Cờ xóa mềm (Soft delete) | Default FALSE, EF Core Global Filter |
| `xmin` | XID | Concurrency Token | PostgreSQL System Column hỗ trợ Optimistic Lock |

### 3.2 Bảng `opportunities` (Cơ hội bán hàng)

| Tên trường | Kiểu dữ liệu | Ý nghĩa | Ràng buộc & Ghi chú |
|---|---|---|---|
| `id` | UUID | Khóa chính | PK |
| `code` | VARCHAR(20) | Mã cơ hội | UNIQUE, Format `OPP-XXXX` |
| `title` | VARCHAR(255) | Tiêu đề cơ hội/deal | NOT NULL |
| `customer_id` | UUID | Khách hàng liên quan | FK `customers(id)` ON DELETE RESTRICT |
| `contact_id` | UUID | Đầu mối liên hệ chính | FK `contacts(id)` |
| `stage_id` | INT | Giai đoạn bán hàng | FK `opportunity_stages(id)` |
| `estimated_value` | NUMERIC(18,2) | Doanh số dự kiến | $\ge 0$ |
| `probability` | INT | Xác suất thành công (%) | Giá trị từ $0 \rightarrow 100$ |
| `expected_close_date` | TIMESTAMPTZ | Ngày dự kiến chốt đơn | NOT NULL |
| `assigned_user_id` | UUID | Sales phụ trách | FK Users |
| `is_won` | BOOLEAN | Cờ chốt thành công | Default NULL/FALSE |
| `loss_reason` | TEXT | Lý do thất bại nếu thua deal | Nhập khi đóng deal Lost |

### 3.3 Bảng `sales_tasks` (Nhiệm vụ & Kế hoạch hành động)

| Tên trường | Kiểu dữ liệu | Ý nghĩa | Ràng buộc & Ghi chú |
|---|---|---|---|
| `id` | UUID | Khóa chính | PK |
| `customer_id` | UUID | Khách hàng mục tiêu | FK `customers(id)` |
| `assigned_user_id` | UUID | Người thực hiện | FK Users |
| `task_type` | INT | Phân loại nhiệm vụ | 1: Call, 2: Meeting, 3: Email, 4: Quote, 5: Care |
| `priority` | INT | Mức độ ưu tiên | 0: Low, 1: Medium, 2: High, 3: Urgent |
| `status` | INT | Trạng thái xử lý | 0: Pending, 1: InProgress, 2: Completed, 3: Cancelled |
| `outcome` | INT | Kết quả sau khi làm | 1: CustomerWillBuy, 2: CustomerBusy, ..., 8: Rejected |
| `due_date` | TIMESTAMPTZ | Hạn hoàn thành | Lưu dạng UTC |
| `completed_at` | TIMESTAMPTZ | Thời điểm hoàn thành thực tế | Ghi nhận khi đóng task |

### 3.4 Bảng `dashboard_snapshots` (Đóng băng báo cáo hàng đêm)

| Tên trường | Kiểu dữ liệu | Ý nghĩa | Ràng buộc & Ghi chú |
|---|---|---|---|
| `snapshot_date` | DATE | Ngày chốt số liệu | PK / UNIQUE Index |
| `total_revenue` | NUMERIC(18,2) | Tổng doanh số lũy kế kỳ | Lưu giá trị số |
| `won_opportunities` | INT | Số deal chốt thành công | Khớp với tầng cuối của phễu |
| `win_rate` | NUMERIC(5,2) | Tỷ lệ chốt đơn (%) | $(Won / Total) \times 100$ |
| `customer_ratio_json` | JSONB | Tỷ trọng 4 nhóm sức khỏe | Cấu trúc `{ good, fair, atRisk, churned }` ($=100\%$) |
| `funnel_json` | JSONB | Dữ liệu 6 tầng phễu Cohort | Lưu số lượng và drop-off rate từng nấc |
| `monthly_revenue_json` | JSONB | Doanh số 6 tháng | Phân tách rõ `{ newCustomers, oldCustomers }` |
| `top_staff_json` | JSONB | Top 5 nhân sự xuất sắc | Mảng chứa `{ userId, name, revenue, dealsCount }` |

---

## 4. SƠ ĐỒ QUAN HỆ THỰC THỂ (ERD DẠNG TEXT)

```
customers (1) ──────────< contacts (n)
customers (1) ──────────< opportunities (n)
customers (1) ──────────< sales_tasks (n)
customers (1) ──────────< interactions (n)
customers (1) ──────────< customer_assignments (n)

opportunity_stages (1) ─< opportunities (n)
opportunities (1) ──────< opportunity_stage_histories (n)
opportunities (1) ──────< quotes (n)

quotes (1) ─────────────< quote_items (n)
products (1) ───────────< quote_items (n)
categories (1) ─────────< products (n)

application_users (1) ──< sales_tasks (n)
application_users (1) ──< customer_assignments (n)
application_users (1) ──< opportunities (n)

* Ghi chú tích hợp KiotViet:
  - Đồng bộ 2 chiều ngầm: KiotViet <---> customers, products, orders.
  - Phân hệ CRM không lưu trữ bảng kho hàng, phiếu xuất nhập vật lý (KiotViet giữ vai trò Master Data).
```

---

## 5. DANH MỤC 12 WEB API CHUẨN

Toàn bộ API tuân thủ cấu trúc phản hồi JSON thống nhất:
```json
{
  "success": true,
  "data": { ... },
  "message": "Thông báo thực thi thành công",
  "errors": null
}
```

| # | Phương thức | Đường dẫn Endpoint | Mục đích nghiệp vụ | Quyền truy cập |
|---|---|---|---|---|
| **1** | `GET` | `/api/dashboard/kpi` | Lấy 4 chỉ số KPI chính đỉnh trang | Manager, Admin |
| **2** | `GET` | `/api/dashboard/funnel` | Lấy dữ liệu phễu chuyển đổi Cohort | Manager, Admin |
| **3** | `GET` | `/api/dashboard/customer-ratio` | Lấy tỷ trọng sức khỏe khách hàng | Manager, Admin |
| **4** | `GET` | `/api/dashboard/revenue-monthly` | Lấy doanh thu 6 tháng (tách mới/cũ) | Manager, Admin |
| **5** | `GET` | `/api/dashboard/top-staff` | Bảng vinh danh Top 5 nhân sự bán hàng | Manager, Admin |
| **6** | `GET` | `/api/dashboard/alerts` | Danh sách thông báo khách At-Risk khẩn | All Authenticated |
| **7** | `POST`| `/api/opportunities/{id}/move` | Nhận lệnh kéo thả card Kanban | Sales, Manager |
| **8** | `POST`| `/api/tasks/{id}/complete` | Đóng task & kích hoạt Outcome state | Sales, Manager |
| **9** | `GET` | `/api/customers/search?q=` | Gợi ý tìm kiếm khách hàng nhanh (Blazor EventCallback) | All Authenticated |
| **10**| `GET` | `/api/products/search?q=` | Tra cứu sản phẩm lên báo giá (Blazor EventCallback) | Sales, Manager |
| **11**| `GET` | `/api/quotes/{id}/pdf` | Stream xuất file PDF báo giá in ấn | All Authenticated |
| **12**| `GET` | `/api/kiotviet/stock/{productId}`| Tra cứu tồn kho thời gian thực (Cache 5p) | Sales, Manager |

---

## 6. KẾ HOẠCH KIỂM THỬ (TEST PLAN 52 CASES)

### 6.1 Chiến lược kiểm thử
* **Unit Test:** Kiểm thử đơn vị tập trung cho toàn bộ CQRS Handlers của `Crm.Business` (Mục tiêu Code Coverage $> 70\%$).
* **Integration Test:** Kiểm tra giao tiếp giữa EF Core và PostgreSQL, test kết nối KiotViet HttpClient mock.
* **End-to-End Test (E2E):** Kiểm thử thủ công theo 4 kịch bản người dùng cốt lõi trên môi trường staging/production.
* **UAT:** 10 người dùng thật chạy thử nghiệm liên tục trong 1 tuần (Sprint 7).

### 6.2 Phân bổ 52 Test Cases chính

| Phân hệ | Số lượng test | Trọng tâm kiểm thử |
|---|---|---|
| **Authentication & RBAC** | 5 | Đăng nhập đúng/sai mật khẩu, session timeout 8 tiếng, kiểm tra phân quyền truy cập endpoint cấm. |
| **Khách hàng 360** | 10 | Tự động sinh mã `KH-XXXX`, CRUD, xóa mềm không mất dữ liệu, import ClosedXML gom lỗi, export Excel. |
| **Cơ hội & Kanban** | 8 | Sinh mã `OPP-XXXX`, chuyển giai đoạn lưu vết lịch sử, chốt deal thắng/thua, rollback thẻ khi ngắt mạng. |
| **Báo giá & In ấn** | 8 | Thuật toán tính tiền $(Qty \times Price \times (1-CK) + VAT)$, render PDF tiếng Việt, gửi email qua MailKit. |
| **Kế hoạch hành động** | 10 | Lọc tabs thời gian, máy trạng thái 8 nhánh Outcome, tự động sinh cơ hội/task tiếp theo. |
| **Báo cáo & Dashboard** | 6 | Tổng tỷ lệ khách luôn bằng $100.0\%$, phễu chuyển đổi hình nón giảm dần, đọc dữ liệu từ snapshot JSON. |
| **Đồng bộ KiotViet** | 5 | Xác thực OAuth 2.0 lấy token, sync đơn hàng cập nhật doanh thu 90 ngày, chống gọi quá tải (Rate limit 429). |
| **Tổng cộng** | **52** | **Bộ test suite chuẩn mực đảm bảo tính toàn vẹn hệ thống** |

### 6.3 Bộ dữ liệu kiểm thử chuẩn (Seed Test Data)
* 20 Khách hàng mẫu: Phân bổ đều các cấp độ sức khỏe (5 New, 5 Good, 5 At-Risk, 5 Churned).
* 15 Cơ hội bán hàng: Trải đều 6 giai đoạn bán lẻ/sỉ.
* 30 Nhiệm vụ bán hàng: Đầy đủ các mức ưu tiên và trạng thái đến hạn.
* 20 Mặt hàng tiêu hao: Đầy đủ mã SKU, đơn giá và danh mục.
* 6 Tháng dữ liệu hóa đơn: Giả lập doanh số để kiểm tra biểu đồ cột kép.

---

## 7. QUY TRÌNH GIT VÀ QUẢN LÝ MÃ NGUỒN

### 7.1 Cấu trúc nhánh (Branching Model)
```
main             <--- Nhánh Production chính thức, chỉ merge khi nghiệm thu Sprint
 └── develop     <--- Nhánh làm việc hàng ngày của Developer
      ├── feature/xxx  <--- Nhánh phát triển tính năng riêng lẻ trong ngày
      └── hotfix/xxx   <--- Nhánh sửa lỗi khẩn cấp trực tiếp trên Production
```

### 7.2 Quy chuẩn Commit (Conventional Commits)
* `feat:` Bổ sung tính năng mới (ví dụ: `feat: tích hợp ClosedXML import khách hàng`).
* `fix:` Vá lỗi phát sinh (ví dụ: `fix: sửa lỗi làm tròn số thập phân trong báo giá`).
* `refactor:` Tối ưu cấu trúc mã nguồn, không làm đổi logic (ví dụ: `refactor: tách nhỏ CustomerService`).
* `test:` Bổ sung unit test hoặc test cases (ví dụ: `test: thêm test case cho máy trạng thái TaskOutcome`).
* `docs:` Cập nhật tài liệu kỹ thuật hoặc schema DDL (ví dụ: `docs: cập nhật ERD và Runbook`).
* `chore:` Nâng cấp thư viện, cấu hình build (ví dụ: `chore: nâng cấp Npgsql lên 8.0.3`).

---

## 8. CHECKLIST QUY TRÌNH TRIỂN KHAI (DEPLOYMENT)

### 8.1 Trước khi Deploy
- [ ] Chạy lệnh `dotnet test` đảm bảo 100% test cases đều pass (màu xanh).
- [ ] Quét sạch toàn bộ các ghi chú `TODO` hoặc `FIXME` chưa xử lý.
- [ ] Kiểm tra biến môi trường: Không để lộ Client Secret, Password trong git.
- [ ] Sao lưu cơ sở dữ liệu production hiện tại bằng lệnh `pg_dump`.
- [ ] Kiểm tra dung lượng ổ cứng VPS trống tối thiểu $\ge 20\%$.

### 8.2 Các bước Deploy thực tế
- [ ] Chạy đóng gói bản phát hành: `dotnet publish -c Release -o /var/www/crm`.
- [ ] Chạy migrate cập nhật database schema: `dotnet ef database update`.
- [ ] Khởi động lại dịch vụ Linux: `sudo systemctl restart crm.service`.
- [ ] Kiểm tra tính khả dụng qua endpoint `/health` (kết quả trả về `Healthy`).
- [ ] Soi nhật ký log Serilog bằng lệnh `tail -f /var/www/crm/logs/crm-*.log`.

### 8.3 Kịch bản Rollback (Khắc phục khi có sự cố)
* **Bước 1:** Chuyển đổi mã nguồn về Git Tag ổn định liền trước: `git checkout v1.x.x`.
* **Bước 2:** Khôi phục cơ sở dữ liệu từ file dump gần nhất nếu có thay đổi schema lỗi: `pg_restore -d crm_db backup_pre_deploy.dump`.
* **Bước 3:** Khởi động lại `crm.service` và xác nhận hệ thống hoạt động trở lại.
* **Bước 4:** Gửi thông báo sự cố chi tiết vào kênh quản trị Telegram Bot.

---

## 9. QUY TRÌNH QUẢN TRỊ HỆ THỐNG (ADMIN RUNBOOK)

### 9.1 Quản trị người dùng & Phân quyền
* **Thêm người dùng mới:** Truy cập `/Admin/Users/Create`, thiết lập thông tin đăng nhập và chọn đúng 1 trong 4 vai trò.
* **Khóa tài khoản:** Chuyển trường `IsActive = false`. Tài khoản bị ngắt phiên đăng nhập ngay lập tức nhưng toàn bộ dữ liệu lịch sử được bảo toàn nguyên vẹn.
* **Cấp lại mật khẩu:** Kích hoạt chức năng "Reset Password", hệ thống sinh mật khẩu tạm thời đạt chuẩn bảo mật.

### 9.2 Vận hành Sao lưu & Phục hồi
* **Lịch sao lưu tự động:** Chạy script `scripts/backup.sh` lúc 01:00 AM hàng ngày qua cron Linux:
  ```bash
  0 1 * * * /bin/bash /var/www/crm/scripts/backup.sh >> /var/log/crm_backup.log 2>&1
  ```
* **Lưu trữ an toàn:** Lưu file `.dump` cục bộ trên VPS (lưu 30 ngày gần nhất) và đồng bộ lên Cloud Storage.
* **Diễn tập phục hồi định kỳ:** Thực hiện kiểm thử restore dữ liệu mẫu mỗi tháng một lần.

### 9.3 Xử lý các sự cố vận hành thường gặp

| Hiện tượng | Nguyên nhân tiềm ẩn | Giải pháp khắc phục chuẩn |
|---|---|---|
| **Sync KiotViet báo lỗi liên tục** | Token hết hạn hoặc sai `client_secret` | Vào Hangfire Dashboard kiểm tra exception; kiểm tra lại cấu hình kết nối KiotViet Open API và chạy trigger thử lại thủ công. |
| **Dashboard không nhảy số mới** | Job snapshot lúc 04:00 AM bị hoãn | Mở `/hangfire`, tìm job `SnapshotDashboardJob` và bấm nút "Trigger Now" để tạo lại snapshot ngay lập tức. |
| **Kéo thả Kanban bị đứng** | Mất kết nối mạng hoặc lỗi 500 API | Thẻ cơ hội tự động rollback về cột ban đầu; kiểm tra file log Serilog xem bản ghi exception chi tiết. |
| **Ổ cứng VPS đầy nhanh** | File log hoặc bản backup tích tụ | Chạy lệnh dọn dẹp các file `.dump` cũ quá 30 ngày và cấu hình lại rolling log Serilog giữ tối đa 14 ngày. |

---

## 10. DANH MỤC KIỂM SOÁT AN TOÀN BẢO MẬT (SECURITY CHECKLIST)

| # | Hạng mục an toàn bảo mật | Phương án kỹ thuật áp dụng | Trạng thái |
|---|---|---|:---:|
| 1 | **HTTPS Toàn diện** | Kích hoạt chứng chỉ Let's Encrypt SSL, chuyển hướng 100% HTTP $\rightarrow$ HTTPS | [x] |
| 2 | **HTTP Strict Transport Security** | Cấu hình HSTS Header với thời hạn `max-age=31536000` | [x] |
| 3 | **Chống tấn công CSRF** | Bật xác thực Anti-Forgery Token cho toàn bộ các request POST/PUT/DELETE | [x] |
| 4 | **Giới hạn lưu lượng (Rate Limiting)** | Middleware .NET 10 giới hạn tối đa 60 requests/phút/IP để chống spam | [x] |
| 5 | **Chống tấn công SQL Injection** | Sử dụng 100% Parameterized Queries thông qua Entity Framework Core 8 | [x] |
| 6 | **Mã hóa mật khẩu an toàn** | ASP.NET Core Identity tích hợp thuật toán băm PBKDF2 / BCrypt | [x] |
| 7 | **Bảo vệ phiên làm việc** | Cookie Authentication với cờ `HttpOnly`, `SameSite=Lax`, thời hạn 8 tiếng | [x] |
| 8 | **Truy vết kiểm toán (Audit Trail)** | Tự động ghi nhận mọi thao tác thêm/sửa/xóa vào bảng `audit_logs` | [x] |
| 9 | **Chống Clickjacking** | Cấu hình Header `X-Frame-Options: SAMEORIGIN` | [x] |
| 10| **Che giấu chi tiết lỗi nhạy cảm** | Trang thông báo lỗi thân thiện, không in StackTrace chi tiết ra UI ngoài production | [x] |

---

## 11. CAM KẾT CHẤT LƯỢNG & KẾ HOẠCH HỖ TRỢ (SUPPORT PLAN)

### 11.1 Các giai đoạn đồng hành
* **Giai đoạn 1 (2 tuần đầu sau Go-Live):** Hỗ trợ trực tiếp on-site hàng ngày tại văn phòng doanh nghiệp, giải đáp vướng mắc và tinh chỉnh nhỏ.
* **Giai đoạn 2 (Tuần 3 - 4):** Hỗ trợ từ xa qua Hotline/Telegram với thời gian phản hồi dưới 4 giờ.
* **Giai đoạn 3 (Tháng 2 - 3):** Bảo hành hệ thống, xử lý lỗi phát sinh nghiêm trọng trong vòng 24 giờ.

### 11.2 Phân loại mức độ sự cố và cam kết SLA

| Mức độ ưu tiên | Mô tả sự cố | Cam kết thời gian xử lý (SLA) |
|---|---|:---:|
| **Critical (Khẩn cấp)** | Hệ thống sập hoàn toàn, database không thể truy cập, mất dữ liệu | **Trong vòng 2 giờ** |
| **High (Mức cao)** | Chức năng cốt lõi bị gián đoạn (không xuất được báo giá, không kéo được Kanban) | **Trong vòng 8 giờ** |
| **Medium (Trung bình)** | Chức năng phụ bị chậm, lỗi giao diện nhỏ không ảnh hưởng luồng chính | **Trong vòng 24 giờ** |
| **Low (Mức thấp)** | Góp ý cải tiến câu chữ, thay đổi màu sắc nhãn hiển thị | **Xử lý trong 3 ngày làm việc** |

---

## 12. RÀ SOÁT & CHUẨN HÓA CÁC ĐIỂM CHƯA THỐNG NHẤT

Sau khi đối soát toàn diện giữa các tài liệu, 5 điểm chưa thống nhất ban đầu được chuẩn hóa triệt để như sau:
1. **Tỷ lệ chuyển đổi phễu bán hàng:** Thống nhất dùng con số **5 hợp đồng thắng trên 50 cơ hội ban đầu (Tỷ lệ 10.0%)** để khớp hoàn toàn với mô hình phễu Cohort thực tế.
2. **Công thức tính tỷ lệ chốt deal:** Chuẩn hóa công thức Win Rate $= \frac{Won}{Won + Lost} \times 100\%$ hoặc $\frac{Won}{Total\ Cohort} \times 100\%$ tùy theo chế độ xem báo cáo.
3. **Phân hệ kho hàng:** Đã loại bỏ hoàn toàn các thẻ "Số lượng SKU sắp hết" trên Dashboard; chỉ giữ lại tính năng **Tra cứu tồn kho nhanh** từ KiotViet API để phục vụ lúc làm báo giá.
4. **Quy mô bộ Test Suite:** Chuẩn hóa chính xác **52 Test Cases** bao quát toàn bộ 7 module chức năng thực tế.
5. **Danh mục Web API:** Chuẩn hóa đúng **12 Endpoints phục vụ CRM**, loại bỏ các API kho vật lý không thuộc phạm vi dự án.

---

## 13. TỔNG HỢP TRẠNG THÁI TÀI LIỆU DỰ ÁN

| # | Hạng mục tài liệu | Vị trí tra cứu | Tình trạng hoàn thiện |
|---|---|---|:---:|
| 1 | PRD-Lite (Tổng quan & SMART) | Mục 1 | ✅ Hoàn thành |
| 2 | User Stories & Acceptance Criteria | Mục 2 | ✅ Hoàn thành |
| 3 | Data Dictionary (Bảng & Trường quan trọng) | Mục 3 | ✅ Hoàn thành |
| 4 | Sơ đồ liên kết ERD dạng văn bản | Mục 4 | ✅ Hoàn thành |
| 5 | Danh mục 12 Web API chuẩn | Mục 5 | ✅ Hoàn thành |
| 6 | Kế hoạch kiểm thử (Test Plan 52 cases) | Mục 6 | ✅ Hoàn thành |
| 7 | Quy trình Git & Chuẩn hóa Commit | Mục 7 | ✅ Hoàn thành |
| 8 | Quy trình Checklist triển khai VPS | Mục 8 | ✅ Hoàn thành |
| 9 | Hướng dẫn vận hành Admin Runbook | Mục 9 | ✅ Hoàn thành |
| 10 | Danh mục kiểm soát An toàn bảo mật | Mục 10 | ✅ Hoàn thành |
| 11 | Cam kết SLA và Kế hoạch hỗ trợ | Mục 11 | ✅ Hoàn thành |
| 12 | Rà soát và chuẩn hóa số liệu | Mục 12 | ✅ Hoàn thành |
| 13 | Wireframe chi tiết 9 màn hình nghiệp vụ | **Mục 15** | ✅ **Đã hoàn thiện** |
| 14 | Khung cẩm nang người dùng (User Manual) | **Mục 16** | ✅ **Đã hoàn thiện** |

---

## 14. TÓM TẮT ĐIỀU HÀNH

Tài liệu này là mảnh ghép hoàn chỉnh đồng hành cùng **Tài liệu Kế hoạch Thực thi Dự án CRM (Sprint Master Plan)**:
* Loại bỏ hoàn toàn sự chồng chéo, chỉ tập trung vào các đặc tả nghiệp vụ, kiến trúc dữ liệu và hướng dẫn vận hành.
* Các số liệu tính toán phễu, KPI, API và Test cases đã được thống nhất 100%.
* Cung cấp cái nhìn trực quan sinh động thông qua hệ thống Wireframe chi tiết cho toàn bộ 9 màn hình ứng dụng.

---

## 15. WIREFRAME CHI TIẾT 9 MÀN HÌNH NGHIỆP VỤ (ASCII TEXT-BASED)

Quy ước chung bố cục màn hình:
* **Sidebar trái (Cố định):** Logo hệ thống, Khách hàng 360, Cơ hội bán hàng, Báo giá thương mại, Kế hoạch công việc, Báo cáo điều hành, Cài đặt hệ thống.
* **Header trên:** Thanh tìm kiếm đa năng (Mã KH, SĐT, MST, Tên deal), Chuông thông báo việc khẩn, Thông tin người dùng và Đăng xuất.
* **Khu vực làm việc chính:** Tải nội dung động thông qua Blazor Server, giữ nguyên trải nghiệm mượt mà không reload trang toàn bộ.

---

### 15.1 Màn hình 1: Danh sách Khách hàng (`/Customers/Index`)

```text
+----------------------------------------------------------------------------------------------------+
| [CRM LOGO]              [ Tìm kiếm nhanh (Mã, Tên, MST)... ]          [(3)🔔] [Nguyễn Văn A (Sales) v] |
+------------------+---------------------------------------------------------------------------------+
| > Khách hàng     | KHÁCH HÀNG > DANH SÁCH                                                          |
|   Cơ hội         +---------------------------------------------------------------------------------+
|   Báo giá        | [ + Thêm Khách Hàng ]    [ ⤓ Xuất Excel ]    [ ⤒ Nhập Excel ]                    |
|   Công việc      +---------------------------------------------------------------------------------+
|   Báo cáo        | Bộ lọc: [Sức khỏe: Tất cả v]  [Phụ trách: Nguyễn Văn A v]  [Tìm: [Nhập tên/SĐT]]  |
|   Hệ thống       +---------------------------------------------------------------------------------+
|                  | Mã KH   | Tên Doanh Nghiệp      | Sức Khỏe   | Phụ Trách | Đơn Cuối   | DS 90 Ngày  | Thao Tác |
|                  |---------+-----------------------+------------+-----------+------------+-------------+----------|
|                  | KH-0012 | Cơ Khí An Phát        | [Healthy ] | Văn A     | 12/09/2026 | 145.000.000 | [Xem][Sửa|
|                  | KH-0089 | Chế Tạo Máy Việt Nhật | [At-Risk ] | Văn A     | 05/06/2026 |  42.000.000 | [Xem][Sửa|
|                  | KH-0104 | Khuôn Mẫu Tân Á       | [Dormant ] | Hoàng B   | 15/01/2026 |           0 | [Xem][Sửa|
|                  | KH-0155 | Bulong Ốc Vít Nam An  | [New     ] | Văn A     | --         |           0 | [Xem][Sửa|
|                  +---------------------------------------------------------------------------------+
|                  | Hiển thị 1 - 4 / 128 khách hàng               [Trang trước] [1] [2] [Trang sau] |
+------------------+---------------------------------------------------------------------------------+
```

---

### 15.2 Màn hình 2: Hồ sơ Khách hàng 360 (`/Customers/Detail/{id}`)

```text
+----------------------------------------------------------------------------------------------------+
| KHÁCH HÀNG > CHI TIẾT: CÔNG TY TNHH CƠ KHÍ AN PHÁT (Mã: KH-0012)                [ Sửa ] [ Xóa (Soft)]|
+----------------------------------------------------------------------------------------------------+
| Sức khỏe: [ Healthy (Xanh) ] | Chu kỳ mua: 28 ngày | Lần mua cuối: 12/09/2026 | Hạn CS: 10/10/2026  |
| Doanh thu 90 ngày: 145.000.000 đ (KiotViet) | Công nợ hiện tại: 18.500.000 đ (KiotViet)             |
+----------------------------------------------------------------------------------------------------+
| [ Tab 1: Thông tin chung ]  [ Tab 2: Người liên hệ (3) ]  [ Tab 3: Cơ hội (2) ]  [ Tab 4: Tương tác ]
+----------------------------------------------------------------------------------------------------+
| (Khi chọn Tab 2: Người liên hệ)                                 [ + Thêm Người Liên Hệ (Modal) ]   |
| • Trần Văn Cường - TP Thu Mua | SĐT: 0912.345.xxx | Email: cuong@anphat.vn | [Chính] [Sửa] [Xóa]   |
| • Lê Thị Mai     - Kế toán kho| SĐT: 0988.765.xxx | Email: mai.lt@anphat.vn|         [Sửa] [Xóa]   |
+----------------------------------------------------------------------------------------------------+
| (Khi chọn Tab 4: Lịch sử tương tác)                             [ + Ghi Nhận Tương Tác (Modal) ]   |
| -------------------------------------------------------------------------------------------------- |
| [20/09/2026 14:30] Gọi điện (Bởi: Nguyễn Văn A) - Kết quả: Hẹn báo giá dao tiện CNC                |
|                    Ghi chú: Khách chuẩn bị vào lô hàng mới cho VinFast, cần chào giá cạnh tranh.   |
| [12/09/2026 09:15] Gặp trực tiếp - Kết quả: Đã chốt đơn PO-2609-01 (KiotViet)                      |
+----------------------------------------------------------------------------------------------------+
```

---

### 15.3 Màn hình 3: Thêm / Sửa Khách hàng (`/Customers/Create` & `/Customers/Edit`)

```text
+----------------------------------------------------------------------------------------------------+
| KHÁCH HÀNG > TẠO MỚI / CHỈNH SỬA                                                                    |
+----------------------------------------------------------------------------------------------------+
| [!] THÔNG TIN BẮT BUỘC                                                                             |
| Mã khách hàng:     [ KH-0156           ] (Tự động sinh nếu để trống)                               |
| Tên doanh nghiệp*: [ Công ty CP Kỹ Thuật Minh Đức                                                ] |
| Mã số thuế:        [ 0108998877        ] [Nút: Tra cứu MST]                                         |
| Số điện thoại*:    [ 024.3388.9999     ]      Email doanh nghiệp: [ contact@minhduc-eng.vn       ] |
| Địa chỉ xuất HĐ*:  [ Số 45 KCN Quang Minh, Mê Linh, Hà Nội                                       ] |
|                                                                                                    |
| [i] PHÂN LOẠI & THIẾT LẬP CHĂM SÓC                                                                 |
| Nhóm ngành hàng:   [ Gia công cơ khí chính xác     v]                                              |
| Nhân viên quản lý: [ Nguyễn Văn A                  v]                                              |
| Chu kỳ mua TB:     [ 30 ] ngày (Hệ thống tự động điều chỉnh theo dữ liệu KiotViet)                 |
| Ghi chú:           [ Khách yêu cầu giao hàng ngoài giờ hành chính                                ] |
+----------------------------------------------------------------------------------------------------+
|                                              [ Hủy bỏ ]    [ Lưu dữ liệu (Ctrl + S) ]              |
+----------------------------------------------------------------------------------------------------+
```

---

### 15.4 Màn hình 4: Bảng Pipeline Kanban Cơ hội (`/Opportunities/Kanban`)

```text
+----------------------------------------------------------------------------------------------------+
| CƠ HỘI BÁN HÀNG                                           [Chế độ xem: [Kanban] | (List)] [ + Tạo ]|
| Lọc: [Nhân viên: Nguyễn Văn A v]  [Tháng: Tháng 10/2026 v]                   Tổng Pipeline: 1.25 Tỷ |
+------------------+------------------+------------------+------------------+------------------------+
| 1. MỚI (15)      | 2. LIÊN HỆ (12)  | 3. BÁO GIÁ (8)   | 4. THƯƠNG LƯỢNG(4| 5. WON (5) / LOST (2)  |
| 120.000.000 đ    | 240.000.000 đ    | 350.000.000 đ    | 280.000.000 đ    | 410.000.000 đ          |
+------------------+------------------+------------------+------------------+------------------------+
| [Card: OPP-0089] | [Card: OPP-0075] | [Card: OPP-0062] | [Card: OPP-0051] | [Card: OPP-0044]       |
| CK Minh Đức      | CK An Phát       | Khuôn Mẫu Tân Á  | Dụng Cụ Hải Nam  | Cơ Khí Thắng Lợi       |
| Deal: Lô dao phay| Deal: Mảnh tiện  | Deal: Dầu làm mát| Deal: Thước kẹp  | 50 Thùng dầu mài CNC   |
| Tiền: 35.000.000 | Tiền: 45.000.000 | Tiền: 80.000.000 | Tiền: 25.000.000 | 120.000.000 đ          |
| XS: 20% | Hạn:15/10| XS: 40% | Hạn:18/10| XS: 60% | Hạn:10/10| XS: 80% | Hạn:05/10| [ĐÃ THẮNG DEAL - WON]  |
| [::: Kéo thả :::]| [::: Kéo thả :::]| [::: Kéo thả :::]| [::: Kéo thả :::]|                        |
|                  +------------------+------------------+------------------+------------------------+
|                  | [Card: OPP-0081] |                  |                  | [Card: OPP-0039]       |
|                  | Bơm Công Nghiệp  |                  |                  | Bị trượt giá thầu      |
|                  | 18.000.000 đ     |                  |                  | [ĐÃ ĐÓNG - THUA DEAL]  |
+------------------+------------------+------------------+------------------+------------------------+
```

---

### 15.5 Màn hình 5: Chi tiết Cơ hội & Chuyển trạng thái (`/Opportunities/Detail/{id}`)

```text
+----------------------------------------------------------------------------------------------------+
| CƠ HỘI: CUNG CẤP LÔ DAO TIỆN CNC THÁNG 10 (Mã: OPP-0075)                                           |
| Khách hàng: Công ty TNHH Cơ Khí An Phát (KH-0012)        Người liên hệ: Trần Văn Cường (0912.345.xxx)|
+----------------------------------------------------------------------------------------------------+
| Giai đoạn: [ 3. Gửi Báo Giá     v] -> [ Cập Nhật ]      [ Nút: Thắng Deal ] [ Nút: Đánh Dấu Thua ]  |
| Giá trị dự kiến: 45.000.000 đ   | Xác suất: 60%        | Dự kiến đóng deal: 18/10/2026             |
+----------------------------------------------------------------------------------------------------+
| DÒNG THỜI GIAN CHUYỂN GIAI ĐOẠN (STAGE HISTORIES)                                                  |
|  [✓] 01/10/2026: Tạo cơ hội mới (Giai đoạn: Mới tiếp cận)                                          |
|  [✓] 03/10/2026: Khảo sát thông số dao tiện (Giai đoạn: Đang liên hệ tìm hiểu)                     |
|  [✓] 05/10/2026: Lập và gửi báo giá BG-26-0045 (Giai đoạn: Đã gửi báo giá)                        |
|  [ ] Tiếp theo : Đàm phán chiết khấu số lượng                                                      |
+----------------------------------------------------------------------------------------------------+
| DANH SÁCH BÁO GIÁ LIÊN KẾT                                             [ + Lập Báo Giá Mới ]       |
| • BG-26-0045 | Tổng: 47.300.000 đ (VAT) | Ngày: 05/10/2026 | [Đã gửi KH] | [In PDF] [Chi tiết]     |
+----------------------------------------------------------------------------------------------------+
```

---

### 15.6 Màn hình 6: Lập Báo giá Động (`/Quotes/Create`)

```text
+----------------------------------------------------------------------------------------------------+
| BÁO GIÁ > LẬP BÁO GIÁ MỚI                                                      Mã: [ QUO-0046     ] |
+----------------------------------------------------------------------------------------------------+
| Chọn khách hàng*: [ An Phát - KH-0012            🔍]  Người nhận: [ Trần Văn Cường - 0912.345.xxx v] |
| Liên kết cơ hội:  [ OPP-0075 - Lô dao tiện CNC   v]  Ngày tạo  : [ 05/10/2026 ]  Hạn: [ 20/10/2026] |
| Điều khoản TT   : [ Chuyển khoản 100% trong 15 ngày sau khi nhận hàng                            v] |
+----------------------------------------------------------------------------------------------------+
| DANH MỤC HÀNG HÓA                                                                                  |
| STT | Mã SP / Tên Hàng Hóa       | Tồn Kho (KV) | ĐVT | SL  | Đơn Giá    | CK(%) | Thành Tiền      | Xóa|
|-----+----------------------------+--------------+-----+-----+------------+-------+-----------------+----|
| 1   | [DP-01] Mảnh tiện Kyocera  | 250 hộp      | Hộp | 10  | 1.200.000  | 5%    | 11.400.000      | [x]|
| 2   | [DA-05] Mũi khoan mạ Titan | 45 cây       | Cây | 20  |   350.000  | 0%    |  7.000.000      | [x]|
| 3   | [ [Gõ mã/tên hàng tra cứu]              🔍]     |     |            |       |                 |    |
+----------------------------------------------------------------------------------------------------+
| [ + Thêm dòng sản phẩm ]                                                                           |
|                                                     Cộng tiền hàng:     18.400.000 đ               |
|                                                     Chiết khấu thương mại:       0 đ               |
|                                                     Thuế VAT (8%):       1.472.000 đ               |
|                                                     TỔNG THANH TOÁN:    19.872.000 đ               |
+----------------------------------------------------------------------------------------------------+
| Ghi chú đơn giá: [ Đơn giá đã bao gồm chi phí vận chuyển tận kho nhà máy An Phát                 ] |
|                                              [ Hủy bỏ ]   [ Lưu Nháp ]   [ Hoàn tất & Render PDF ] |
+----------------------------------------------------------------------------------------------------+
```

---

### 15.7 Màn hình 7: Chi tiết Báo giá & Điều hướng (`/Quotes/Detail/{id}`)

```text
+----------------------------------------------------------------------------------------------------+
| BÁO GIÁ: QUO-0046 — KHÁCH HÀNG: CÔNG TY TNHH CƠ KHÍ AN PHÁT                     Trạng thái: [ĐÃ GỬI]
+----------------------------------------------------------------------------------------------------+
| Tổng giá trị: 19.872.000 đ (Đã gồm 8% VAT) | Ngày lập: 05/10/2026 | Người lập: Nguyễn Văn A        |
+----------------------------------------------------------------------------------------------------+
| THAO TÁC XỬ LÝ:                                                                                    |
| [ 🖨 In / Xem Bản PDF (QuestPDF) ]   [ ✉ Gửi Email Đính Kèm ]   [ Chuyển Thành Đơn Hàng Thành Công ]|
+----------------------------------------------------------------------------------------------------+
| XEM TRƯỚC BẢN IN (PREVIEW PDF CHUẨN IN ẤN A4)                                                      |
| +------------------------------------------------------------------------------------------------+ |
| | CÔNG TY TNHH THƯƠNG MẠI & KỸ THUẬT CÔNG NGHIỆP                                                | |
| | Đ/c: Cầu Giấy, Hà Nội | Hotline: 024.9999.xxxx                                                 | |
| |                                   BẢNG BÁO GIÁ THƯƠNG MẠI                                      | |
| | Kính gửi: CÔNG TY TNHH CƠ KHÍ AN PHÁT - Người liên hệ: Anh Trần Văn Cường                     | |
| | (Bảng kê chi tiết 2 mặt hàng dụng cụ cơ khí...)                                                | |
| | Tổng thanh toán: 19.872.000 VNĐ (Mười chín triệu tám trăm bảy mươi hai nghìn đồng chẵn).       | |
| +------------------------------------------------------------------------------------------------+ |
+----------------------------------------------------------------------------------------------------+
```

---

### 15.8 Màn hình 8: Quản lý Danh sách Công việc (`/Tasks/Index`)

```text
+----------------------------------------------------------------------------------------------------+
| KẾ HOẠCH HÀNH ĐỘNG HÀNG NGÀY                                                   [ + Giao Việc Mới ] |
+----------------------------------------------------------------------------------------------------+
| TAB LỌC: [ Hôm nay (4) ]  [ Quá hạn (1) ]  [ Tuần này (12) ]  [ Sắp tới (8) ]  [ Tất cả nhiệm vụ ] |
+----------------------------------------------------------------------------------------------------+
| Ưu Tiên | Nhiệm Vụ            | Khách Hàng      | Hạn Xử Lý        | Trạng Thái  | Xử Lý           |
+---------+---------------------+-----------------+------------------+-------------+-----------------+
| [CAO]   | Gọi nhắc chu kỳ mua | Cơ Khí An Phát  | Hôm nay 10:00 AM | [Chưa làm]  | [ Ghi Kết Quả ] |
| [TR.BÌNH| Gửi lại báo giá đợt2| Khuôn Mẫu Tân Á | Hôm nay 02:30 PM | [Chưa làm]  | [ Ghi Kết Quả ] |
| [THẤP]  | Gửi catalogue dao mớ| Chế Tạo Máy VN  | Hôm nay 04:30 PM | [Chưa làm]  | [ Ghi Kết Quả ] |
| [KHẨN]  | Xử lý khiếu nại mũi | Bulong Nam An   | Quá hạn 1 ngày   | [Đang xử lý]| [ Ghi Kết Quả ] |
+----------------------------------------------------------------------------------------------------+
| Gợi ý tự động từ Hangfire: Khách hàng "Khuôn Mẫu Tân Á" đã quá 2 lần chu kỳ mà chưa có đơn mới.   |
+----------------------------------------------------------------------------------------------------+
```

---

### 15.9 Màn hình 9: Modal Đóng Task & Ghi nhận Tương tác (`MudDialog Trigger`)

```text
+-----------------------------------------------------------------------------+
| GHI NHẬN KẾT QUẢ CÔNG VIỆC: Gọi nhắc chu kỳ mua (An Phát)             [ X ] |
+-----------------------------------------------------------------------------+
| Hình thức tương tác*: [ Cuộc gọi điện thoại                              v] |
| Người tiếp nhận     : [ Trần Văn Cường - TP Thu Mua                      v] |
|                                                                             |
| KẾT QUẢ ĐẠT ĐƯỢC (OUTCOME)*:                                                |
| (•) 1. Khách đồng ý mua / Có nhu cầu mới                                    |
|     --> Tự động kích hoạt: Tạo cơ hội bán hàng (Opportunity) mới            |
| ( ) 2. Khách hẹn gọi lại sau (Bận / Đang họp)                               |
|     --> Tự động kích hoạt: Tạo công việc nhắc lại sau 2 ngày làm việc        |
| ( ) 3. Khách phàn nàn về chất lượng hàng / khiếu nại                        |
| ( ) 4. Khách từ chối không mua đợt này / Kho còn tồn nhiều                  |
| ( ) 5. Số điện thoại sai / Không nghe máy (Gọi lại sau 4 tiếng)              |
|                                                                             |
| Ghi chú chi tiết trao đổi:                                                  |
| [ Anh Cường báo xưởng đang chuẩn bị chạy lô khuôn đúc cho đối tác Nhật.    ] |
| [ Cần 50 hộp dao tiện rãnh mã Kyocera trước ngày 15/10.                    ] |
+-----------------------------------------------------------------------------+
|                                              [ Đóng ]   [ Lưu & Tự Tạo Việc]|
+-----------------------------------------------------------------------------+
```

---

## 16. KHUNG CẤU TRÚC CẨM NANG NGƯỜI DÙNG (USER MANUAL FRAMEWORK)
*(Biên soạn chi tiết tại Sprint 7 · Quy mô chuẩn: 15–20 trang có ảnh chụp thực tế)*

### Phần I: Bắt đầu Nhanh (Dành cho mọi nhân sự)
1. **Đăng nhập & Bảo mật:**
   * Hướng dẫn truy cập qua domain production (HTTPS chuẩn có ổ khóa xanh).
   * Đổi mật khẩu lần đầu và bảo mật phiên làm việc 8 tiếng.
2. **Quy ước Giao diện Chung:**
   * Ý nghĩa màu sắc nhãn sức khỏe khách hàng:
     * `Healthy` (Xanh lục): Mua đều đặn đúng chu kỳ.
     * `At-Risk` (Vàng): Quá $1.5 - 2.0$ lần chu kỳ mua trung bình mà chưa phát sinh đơn mới.
     * `Dormant` (Đỏ xám): Quá 2 lần chu kỳ (nguy cơ mất khách hàng rất cao).
     * `New` (Xanh dương): Khách hàng mới tạo chưa có lịch sử mua.

### Phần II: Hướng dẫn Dành cho Nhân viên Kinh doanh (Sales)
1. **Quản lý Khách hàng 360:**
   * Tra cứu nhanh mã số thuế, kiểm tra lịch sử doanh thu 90 ngày kéo từ KiotViet.
   * Thêm danh bạ người liên hệ (phân tách người quyết định kỹ thuật, kế toán, thủ kho).
2. **Vận hành Pipeline & Bảng Kéo thả Kanban:**
   * Hiểu rõ ý nghĩa 6 giai đoạn chuyển đổi cơ hội.
   * Thao tác kéo thả thẻ cơ hội và quy tắc bắt buộc khi bấm "Thắng Deal (Won)" hoặc "Thua Deal (Lost)".
3. **Lập Báo giá & Xuất PDF:**
   * Thêm dòng mặt hàng, kiểm tra nhanh lượng tồn kho khả dụng từ KiotViet.
   * Xuất file PDF tiêu chuẩn in ấn A4 và kích hoạt gửi email đính kèm trực tiếp.
4. **Kế hoạch Hành động Mỗi Ngày:**
   * Kiểm tra tab "Hôm nay" vào mỗi buổi sáng.
   * Quy tắc chọn 8 nhánh Outcome để hệ thống tự động sinh việc phễu tiếp theo.

### Phần III: Hướng dẫn Dành cho Quản lý & Ban Giám Đốc (Manager)
1. **Đọc hiểu Báo cáo Dashboard:**
   * Ý nghĩa 4 chỉ số KPI tổng trên cùng.
   * Phân tích phễu chuyển đổi Cohort (tìm điểm nghẽn rơi rụng khách hàng).
   * Theo dõi biểu đồ phân bổ doanh thu khách mới vs khách cũ trong 6 tháng.
2. **Giám sát Hiệu suất Bán hàng:**
   * Đánh giá bảng xếp hạng Top 5 nhân sự xuất sắc.
   * Điều phối phân bổ lại các khách hàng At-Risk/Dormant giữa các nhân viên.

### Phần IV: Hướng dẫn Vận hành Dành cho Quản trị viên (Admin)
1. **Quản lý Tài khoản & Phân quyền RBAC:**
   * Thêm nhân sự mới, gán đúng 1 trong 4 vai trò (Sales, Manager, Accountant, Admin).
   * Khóa tài khoản khi nhân sự nghỉ việc mà không làm gián đoạn dữ liệu quá khứ.
2. **Giám sát Tác vụ Nền Hangfire:**
   * Truy cập Dashboard `/hangfire`.
   * Kiểm tra lịch chạy của 4 Recurring Jobs: Nhắc việc (02:00), Sức khỏe khách (03:00), Snapshot số liệu (04:00), Đồng bộ KiotViet (15 phút/lần).
3. **Quy trình Xử lý Sự cố & Khôi phục Dữ liệu:**
   * Đọc file log Serilog theo ngày (`logs/crm-YYYYMMDD.log`).
   * Kích hoạt khôi phục nhanh cơ sở dữ liệu từ file backup hàng ngày `pg_dump`.

---

## 17. CHECKLIST TÀI LIỆU TOÀN DIỆN SAU BỔ SUNG

| # | Hạng mục tài liệu | Trạng thái trước | Trạng thái hiện tại | Vị trí tra cứu |
|---|---|---|---|---|
| 1 | PRD-Lite (Tổng quan & SMART) | ✅ Đã có | ✅ Hoàn chỉnh | Mục 1 |
| 2 | User Stories & Acceptance Criteria | ✅ Đã có | ✅ Hoàn chỉnh | Mục 2 |
| 3 | Data Dictionary (Bảng & Trường cốt lõi)| ✅ Đã có | ✅ Hoàn chỉnh | Mục 3 |
| 4 | Sơ đồ liên kết dữ liệu ERD | ✅ Đã có | ✅ Hoàn chỉnh | Mục 4 |
| 5 | Danh mục chuẩn 12 Web API | ✅ Đã có | ✅ Hoàn chỉnh | Mục 5 |
| 6 | Kế hoạch kiểm thử (Test Plan 52 cases)| ✅ Đã có | ✅ Hoàn chỉnh | Mục 6 |
| 7 | Quy trình Git & Chuẩn Commit | ✅ Đã có | ✅ Hoàn chỉnh | Mục 7 |
| 8 | Quy trình Checklist triển khai (Deploy) | ✅ Đã có | ✅ Hoàn chỉnh | Mục 8 |
| 9 | Hướng dẫn quản trị hệ thống (Admin Runbook)| ✅ Đã có | ✅ Hoàn chỉnh | Mục 9 |
| 10 | Danh mục kiểm soát An toàn bảo mật | ✅ Đã có | ✅ Hoàn chỉnh | Mục 10 |
| 11 | Cam kết chất lượng & SLA Hỗ trợ (Support)| ✅ Đã có | ✅ Hoàn chỉnh | Mục 11 |
| 12 | Rà soát & Chuẩn hóa sai lệch số liệu | ✅ Đã có | ✅ Hoàn chỉnh | Mục 12 |
| 13 | **Wireframe chi tiết 9 màn hình** | ⬜ **Chưa có** | ✅ **ĐÃ BỔ SUNG** | **Mục 15** |
| 14 | **Khung tài liệu Cẩm nang người dùng** | ⬜ **Chưa có** | ✅ **ĐÃ BỔ SUNG** | **Mục 16** |

**KẾT LUẬN:** Toàn bộ **14/14 hạng mục tài liệu** cấu thành dự án CRM đã được hợp nhất 100% trong tài liệu này, đảm bảo tính chặt chẽ, khép kín và sẵn sàng phục vụ thực thi lập trình ngay lập tức.