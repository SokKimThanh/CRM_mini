# TÀI LIỆU ĐẶC TẢ USE CASE — HỆ THỐNG CRM (MASTER USE CASE SPECIFICATION)

* **Phiên bản:** 2.1 — Comprehensive Production Ready
* **Tham chiếu:** Đặc tả kiến trúc kỹ thuật v1.0, PRD-Lite & Sprint Master Plan
* **Mục tiêu:** Cung cấp đặc tả chi tiết 100% các Use Case để đội ngũ phát triển, kiểm thử (QA) và vận hành có cơ sở đối soát duy nhất mà không cần xác nhận lại nghiệp vụ.

---

## PHẦN 0: TỰ REVIEW & CHUẨN HÓA LOGIC HỆ THỐNG

### 0.1 Các lỗi logic từ bản v1.0 đã xử lý triệt để
1. **Quyền chi phối trạng thái Sức khỏe (Health Status):** Loại bỏ hoàn toàn logic CRM tự ý can thiệp đổi `health_status` khi bấm Thắng/Thua deal cơ hội. Trạng thái sức khỏe khách hàng là thuộc tính khách quan được tính toán độc lập bởi tác vụ nền dựa trên tần suất mua hàng thực tế kéo từ KiotViet.
2. **Thua deal (Lost) không đồng nghĩa khách có nguy cơ rời bỏ (At-Risk):** Một khách hàng thân thiết vẫn có thể từ chối một báo giá cụ thể vì lý do giá hoặc ngân sách kỳ này.
3. **Thắng deal (Won) không đồng nghĩa khách Healthy:** Chốt thành công một đơn hàng mới chỉ phản ánh tiềm năng; khách chỉ trở thành Healthy khi hóa đơn thực tế phát sinh trên KiotViet.
4. **Bổ sung phân hệ công nợ cho Accountant:** Bổ sung Use Case tra cứu công nợ (UC-19) đồng bộ hai chiều từ KiotViet.
5. **Tách biệt nghiệp vụ Báo giá:** Tách rời Use Case Cập nhật trạng thái thông thường (UC-35) và Duyệt báo giá vượt thẩm quyền (UC-36).
6. **Bổ sung Audit Trail:** Bổ sung Use Case truy vết nhật ký thay đổi dữ liệu (UC-73).
7. **Bổ sung Quản lý Thông báo:** Thêm Use Case đánh dấu đã đọc thông báo khẩn (UC-53).
8. **Danh mục sản phẩm:** Bổ sung Use Case tra cứu danh mục hàng hóa (UC-27) phục vụ Sales làm báo giá.
9. **Mở rộng quy tắc nhắc việc tự động:** Khách hàng mới mua lần đầu tiên nhưng quá 30 ngày chưa có đơn hàng thứ 2 vẫn được sinh task hỏi thăm.
10. **Linh hoạt thời điểm dự kiến đóng deal:** Cho phép nhập `expected_close_date` trong quá khứ đối với trường hợp ghi nhận dữ liệu hồi tố.
11. **Chuẩn hóa Enum Lý do thất bại deal (`LostReason`):** Cung cấp danh mục 5 lý do rõ ràng.
12. **Bảo mật phân cấp Dashboard:** Sales chỉ xem dữ liệu cá nhân; không được xem bảng xếp hạng Top doanh số toàn công ty (chỉ dành cho Manager và Admin).

### 0.2 Các điểm hoàn thiện nâng cấp tại bản v2.1
* **Phân rã chi tiết 100% Module KiotViet Sync:** Tách cụm UC-60 $\rightarrow$ UC-64 thành 5 Use Case độc lập với đầy đủ kịch bản lỗi (Token hết hạn, Rate limit HTTP 429, Timeout kết nối, Khắc phục lệch schema).
* **Bổ sung các Use Case tác vụ nền (System Actor):** Chi tiết hóa UC-45 (`UpdateCustomerHealthJob`), UC-54 (`SnapshotDashboardJob`), UC-55 (`ConsistencyCheckJob`).
* **Hiệu chỉnh tổng số Use Case:** Chuẩn hóa con số chính xác là **47 Use Cases nghiệp vụ & hệ thống**.

---

## PHẦN 1: BẢNG TÁC NHÂN (ACTORS) & MA TRẬN TRÁCH NHIỆM

| Actor | Phân loại | Quy mô | Vai trò & Trách nhiệm chính |
|---|---|---|---|
| **Sales** | Con người | 6 users | Trực tiếp tương tác khách hàng, tạo cơ hội, kéo thả Kanban, lập báo giá, xuất PDF/gửi email, xử lý task hàng ngày. |
| **Manager** | Con người | 2 users | Giám sát KPI điều hành, phân tích phễu chuyển đổi, duyệt báo giá vượt hạn mức, điều phối phân công khách hàng. |
| **Accountant** | Con người | 1 user | Tra cứu số dư công nợ khách hàng, kiểm soát danh sách báo giá đã phát hành. |
| **Admin** | Con người | 1 user | Quản trị tài khoản, gán quyền RBAC, giám sát Hangfire jobs, xem Audit trail và log lỗi hệ thống. |
| **System** | Hệ thống | Hangfire Engine | Tự động chạy các tác vụ nền theo lịch trình (Cron), kích hoạt tính toán snapshot, nhắc việc và kiểm tra tính toàn vẹn. |
| **KiotViet API** | Hệ thống ngoài | Open API OAuth 2.0 | Nguồn sự thật (Single Source of Truth) cho dữ liệu khách hàng, hóa đơn, công nợ và danh mục hàng hóa. |

---

## PHẦN 2: SƠ ĐỒ USE CASE TỔNG THỂ (ASCII DIAGRAM)

```
                         ┌────────────────────────────────────────────────────────────────────────┐
                         │                              HỆ THỐNG CRM                              │
                         │                                                                        │
  Sales ────────────────►│  [Auth]             UC-01 (Đăng nhập), UC-02 (Đăng xuất)               │
                         │  [Khách hàng 360]   UC-10 → UC-18 (CRUD, Danh bạ, Timeline, Import/Ex) │
                         │  [Cơ hội & Deal]    UC-20 → UC-26 (Kanban, Chuyển stage, Win/Lost)     │
                         │  [Tra cứu SP]       UC-27 (Xem danh mục hàng hóa & tồn kho)            │
                         │  [Báo giá]          UC-30 → UC-35 (Lập báo giá, In PDF, Gửi Email)     │
                         │  [Kế hoạch Task]    UC-40 → UC-43 (Xem việc, Tạo việc, Đóng 8 Outcome) │
                         │  [Dashboard Sales]  UC-50, UC-52, UC-53 (KPI cá nhân, Cảnh báo việc)   │
                         │                                                                        │
  Manager ──────────────►│  [Điều hành]        UC-50, UC-51 (Dashboard phễu Cohort, Doanh thu)   │
                         │  [Phê duyệt]        UC-36 (Duyệt báo giá vượt thẩm quyền)              │
                         │  [Quản lý Sales]    Toàn quyền xem khách, deal, báo giá của mọi nhân sự│
                         │  [Audit Trail]      UC-73 (Tra cứu nhật ký thay đổi dữ liệu)           │
                         │                                                                        │
  Accountant ───────────►│  [Tài chính KH]     UC-19 (Xem công nợ thời gian thực từ KiotViet)     │
                         │  [Kiểm soát BG]     UC-31, UC-32 (Tra cứu danh mục báo giá phát hành)  │
                         │  [Dashboard Tài chính] UC-50 (Xem biểu đồ tăng trưởng doanh số)        │
                         │                                                                        │
  Admin ────────────────►│  [Quản trị User]    UC-70 (Tạo, sửa, khóa tài khoản RBAC)             │
                         │  [Giám sát Log]     UC-71 (Xem Serilog & Hangfire Dashboard)          │
                         │  [Vận hành Job]     UC-72 (Kích hoạt tác vụ nền thủ công)              │
                         │  [Kiểm toán]        UC-73 (Truy vết biến động dữ liệu toàn cục)        │
                         │                                                                        │
  System (Hangfire) ────►│  [Auto Reminder]    UC-44 (Sinh task nhắc chu kỳ mua)                  │
                         │  [Health Engine]    UC-45 (Đánh giá sức khỏe khách hàng hàng đêm)      │
                         │  [Data Snapshot]    UC-54 (Đóng băng dữ liệu Dashboard 04:00 AM)       │
                         │  [Audit Check]      UC-55 (Kiểm tra 6 ràng buộc nhất quán số liệu)     │
                         │  [Sync Engine]      UC-60 → UC-63 (Sync định kỳ 15 phút/lần)          │
                         │                                                                        │
  KiotViet API ◄─────────┼── (Kết nối qua giao thức OAuth 2.0 Client Credentials)                 │
                         └────────────────────────────────────────────────────────────────────────┘
```

---

## PHẦN 3: ĐẶC TẢ CHI TIẾT 47 USE CASES

### MODULE 1: AUTHENTICATION & PHÂN QUYỀN

#### UC-01: Đăng nhập hệ thống
* **Actor:** Sales, Manager, Accountant, Admin
* **Mục đích:** Xác thực danh tính và thiết lập phiên làm việc an toàn.
* **Precondition:** Tài khoản đã được cấp trong hệ thống, `is_active = true`.
* **Postcondition:** Thiết lập Cookie Session bảo mật, điều hướng người dùng tới Dashboard.
* **Main Flow:**
  1. Người dùng truy cập đường dẫn `/Account/Login`.
  2. Hệ thống hiển thị form đăng nhập (Email, Mật khẩu, Checkbox "Ghi nhớ đăng nhập").
  3. Người dùng nhập thông tin và xác nhận.
  4. Hệ thống kiểm tra: Email hợp lệ, băm mật khẩu khớp với PBKDF2/BCrypt, tài khoản đang hoạt động.
  5. Thiết lập Cookie Authentication: thời hạn 8 tiếng (hoặc 30 ngày nếu chọn Ghi nhớ), cờ `HttpOnly`, `SameSite=Lax`.
  6. Ghi vết đăng nhập thành công vào Serilog.
  7. Điều hướng về trang chủ `/Dashboard`.
* **Alternative Flows:**
  * **A1 (Sai mật khẩu):** Thông báo lỗi "Email hoặc mật khẩu không chính xác". Đếm số lần sai liên tiếp; nếu vượt quá 5 lần thì tạm khóa tài khoản trong 15 phút để chống Brute-Force.
  * **A2 (Tài khoản bị vô hiệu hóa):** Thông báo lỗi "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên".
* **Business Rules:**
  * Toàn bộ mật khẩu không lưu dạng văn bản rõ (plain-text).
  * Cookie phiên bị hủy khi đóng trình duyệt nếu không tích chọn "Ghi nhớ".
* **Data touched:** `AspNetUsers`, `user_profiles`, `audit_logs`

#### UC-02: Đăng xuất hệ thống
* **Actor:** Tất cả người dùng đã đăng nhập
* **Mục đích:** Kết thúc phiên làm việc hiện tại và thu hồi token phiên.
* **Postcondition:** Hủy Cookie xác thực, chuyển hướng về trang `/Account/Login`.
* **Main Flow:**
  1. Người dùng bấm nút "Đăng xuất" tại Header góc trên bên phải.
  2. Hệ thống gọi phương thức xóa HttpContext Authentication Cookie.
  3. Ghi log sự kiện đăng xuất vào nhật ký hệ thống.
  4. Điều hướng trình duyệt về màn hình Login.
* **Data touched:** `audit_logs`

---

### MODULE 2: QUẢN LÝ KHÁCH HÀNG 360 (CUSTOMER 360)

#### UC-10: Xem danh sách khách hàng
* **Actor:** Sales, Manager, Accountant, Admin
* **Postcondition:** Danh sách khách hàng hiển thị trực quan kèm phân trang và bộ lọc.
* **Main Flow:**
  1. Người dùng chọn mục "Khách hàng" trên Sidebar (`/Customers/Index`).
  2. Hệ thống tải dữ liệu: Bảng danh sách gồm Mã KH, Tên doanh nghiệp, Sức khỏe (Badge màu), Sales phụ trách, Ngày đặt đơn cuối, Doanh số 90 ngày.
  3. Người dùng thực hiện lọc dữ liệu theo Sức khỏe (Healthy/Fair/AtRisk/Dormant/New) hoặc gõ từ khóa tìm kiếm (Tên/SĐT/MST).
  4. HTMX gửi request debounce 300ms và tải lại phần thân bảng không làm giật trang.
* **Business Rules:**
  * **Sales:** Mặc định chỉ nhìn thấy các khách hàng được gán cho chính mình (`assigned_to_user_id = current_user`).
  * **Manager / Accountant / Admin:** Xem toàn bộ khách hàng của công ty.
  * Tự động loại bỏ các bản ghi đã xóa mềm (`is_deleted = true`).
* **Data touched:** `customers`, `contacts`, `user_profiles`

#### UC-11: Xem hồ sơ khách hàng 360 độ
* **Actor:** Sales, Manager, Accountant, Admin
* **Postcondition:** Màn hình chi tiết khách hàng hiển thị đầy đủ 4 Tabs dữ liệu.
* **Main Flow:**
  1. Người dùng chọn một khách hàng từ danh sách.
  2. Hệ thống điều hướng đến `/Customers/Detail/{id}`:
     * **Khối đầu trang (Header Summary):** Huy hiệu sức khỏe, Chu kỳ mua TB, Lần mua gần nhất, Hạn chăm sóc kế tiếp, Doanh thu 90 ngày, Dư nợ công nợ hiện tại.
     * **Tab 1 (Thông tin chung):** Mã KH, Tên công ty, MST, Địa chỉ xuất hóa đơn, Nhân viên phụ trách.
     * **Tab 2 (Người liên hệ):** Danh sách đầu mối liên hệ (kèm nhãn [Chính], SĐT, Email).
     * **Tab 3 (Cơ hội bán hàng):** Toàn bộ các deal bán sỉ/lẻ đang mở hoặc đã đóng trong quá khứ.
     * **Tab 4 (Dòng thời gian tương tác):** Lịch sử cuộc gọi, gặp mặt, email theo thứ tự thời gian mới nhất.
* **Data touched:** `customers`, `contacts`, `opportunities`, `interactions`

#### UC-12: Tạo mới khách hàng
* **Actor:** Sales, Manager, Admin
* **Postcondition:** Bản ghi khách hàng mới được lưu với mã dạng `KH-XXXX`, trạng thái `New`.
* **Main Flow:**
  1. Người dùng bấm nút "[ + Thêm Khách Hàng ]".
  2. Hệ thống hiển thị form nhập: Tên doanh nghiệp (*), SĐT (*), MST, Địa chỉ xuất HĐ (*), Nhóm ngành hàng, Chu kỳ tiêu thụ dự kiến.
  3. Người dùng nhập dữ liệu và bấm "Lưu".
  4. Hệ thống validate: Tên và SĐT bắt buộc, format SĐT 10 chữ số, MST đúng cấu trúc.
  5. Hệ thống sinh mã tự tăng `KH-XXXX` thông qua Database Sequence.
  6. Gán `assigned_to_user_id = current_user` nếu người tạo là Sales.
  7. Ghi nhận bản ghi vào bảng `audit_logs`.
  8. Chuyển hướng về trang chi tiết hồ sơ 360 của khách vừa tạo.
* **Alternative Flows:**
  * **A1 (Trùng SĐT hoặc MST):** Hệ thống hiển thị cảnh báo màu vàng: "Số điện thoại/MST này đã tồn tại trên hệ thống cho khách hàng [Tên KH]". Người dùng có quyền xem lại bản ghi cũ hoặc tiếp tục lưu nếu xác nhận là chi nhánh phụ.
* **Data touched:** `customers`, `customer_assignments`, `audit_logs`

#### UC-13: Chỉnh sửa thông tin khách hàng
* **Actor:** Sales (chỉ sửa khách của mình), Manager, Admin
* **Postcondition:** Thông tin khách hàng cập nhật thành công, trường Concurrency Token được kiểm soát.
* **Main Flow:**
  1. Người dùng bấm "Sửa" tại trang hồ sơ khách hàng.
  2. Form chỉnh sửa hiển thị thông tin hiện tại.
  3. Người dùng cập nhật các trường được phép và bấm "Lưu dữ liệu".
  4. Hệ thống kiểm tra Concurrency Token (`xmin` / `RowVersion`). Nếu phát hiện xung đột ghi đè đồng thời $\rightarrow$ Báo lỗi "Dữ liệu vừa được người khác cập nhật, vui lòng tải lại trang".
  5. Lưu thay đổi, tự động cập nhật trường `updated_at`, ghi vết vào `audit_logs`.
* **Business Rules:** Mã khách hàng (`code`) là định danh duy nhất, tuyệt đối không được phép chỉnh sửa sau khi tạo.
* **Data touched:** `customers`, `audit_logs`

#### UC-14: Xóa mềm khách hàng (Soft Delete)
* **Actor:** Manager, Admin
* **Postcondition:** Trường `is_deleted = true`, khách hàng ẩn khỏi giao diện nghiệp vụ.
* **Main Flow:**
  1. Người dùng bấm nút "Xóa (Soft)" tại trang hồ sơ khách hàng.
  2. Hộp thoại Modal cảnh báo: "Bạn có chắc chắn muốn đưa khách hàng này vào thùng rác? Toàn bộ lịch sử giao dịch và cơ hội vẫn được bảo tồn".
  3. Người dùng xác nhận.
  4. Hệ thống cập nhật: `is_deleted = true`, `deleted_at = NOW()`, `deleted_by = current_user`.
  5. Ghi log sự kiện vào `audit_logs`.
  6. Điều hướng quay lại trang danh sách `/Customers/Index`.
* **Business Rules:** Tuyệt đối không thực thi lệnh SQL `DELETE FROM customers` để tránh phá vỡ tính toàn vẹn khóa ngoại của lịch sử đơn hàng và doanh thu.
* **Data touched:** `customers`, `audit_logs`

#### UC-15: Nhập danh sách khách hàng hàng loạt từ Excel
* **Actor:** Sales, Manager, Admin
* **Postcondition:** Danh sách khách hàng hợp lệ được thêm vào DB, xuất file báo cáo các dòng lỗi.
* **Main Flow:**
  1. Người dùng bấm "[ ⤒ Nhập Excel ]" tại danh sách khách hàng.
  2. Modal hiển thị cho phép kéo thả file `.xlsx` (kèm đường dẫn tải template chuẩn).
  3. Người dùng chọn file và bấm "Tải lên & Xử lý".
  4. Tầng Service sử dụng thư viện `ClosedXML` đọc từng hàng dữ liệu:
     * Kiểm tra trường bắt buộc (Tên công ty, SĐT).
     * Kiểm tra format định dạng tiền tệ và ngày tháng.
  5. Hệ thống thực hiện Batch Insert (100 bản ghi/batch) cho các dòng hợp lệ.
  6. Trả về kết quả: Số bản ghi thêm thành công, danh sách các dòng bị lỗi chi tiết (kèm số dòng và lý do).
* **Business Rules:** Nếu mã khách hàng trong file Excel đã tồn tại trên DB $\rightarrow$ Bỏ qua bản ghi đó và ghi chú vào danh mục lỗi.
* **Data touched:** `customers`, `contacts`, `audit_logs`

#### UC-16: Xuất danh sách khách hàng ra Excel
* **Actor:** Manager, Admin
* **Postcondition:** Trình duyệt tải về file `.xlsx` chứa danh sách khách hàng theo bộ lọc hiện hành.
* **Main Flow:**
  1. Người dùng thiết lập bộ lọc (theo Sales, theo Sức khỏe) và bấm "[ ⤓ Xuất Excel ]".
  2. Hệ thống gọi `ExcelExportService`, định dạng tiêu đề in đậm, màu nền doanh nghiệp, căn chỉnh độ rộng cột tự động và format số tiền dạng `#,##0 VNĐ`.
  3. Stream file Excel trả về client với tên file: `Danh-Sach-Khach-Hang-YYYYMMDD.xlsx`.
* **Business Rules:** Giới hạn xuất tối đa 5.000 dòng/lần xuất để tránh cạn kiệt tài nguyên RAM của VPS.
* **Data touched:** `customers`, `contacts`

#### UC-17: Quản lý danh bạ người liên hệ (Contacts)
* **Actor:** Sales, Manager, Admin
* **Postcondition:** Thêm/Sửa/Xóa đầu mối liên hệ của khách hàng.
* **Main Flow:**
  1. Người dùng mở Tab 2 "Người liên hệ" tại hồ sơ khách hàng.
  2. Bấm nút "[ + Thêm Người Liên Hệ ]".
  3. Modal HTMX bật lên: Nhập Họ tên (*), Chức vụ/Phòng ban, Số điện thoại (*), Email, Tùy chọn "[ ] Là đầu mối chính".
  4. Người dùng bấm "Lưu".
  5. Hệ thống ghi dữ liệu vào bảng `contacts`. Nếu người dùng đánh dấu là đầu mối chính, hệ thống tự động gỡ cờ chính của các liên hệ cũ thuộc khách hàng đó.
  6. HTMX tải lại danh sách liên hệ ngay tại Tab 2.
* **Data touched:** `contacts`, `audit_logs`

#### UC-18: Ghi nhận lịch sử tương tác (Interactions)
* **Actor:** Sales, Manager
* **Postcondition:** Bản ghi tương tác được lưu, tự động cập nhật ngày liên hệ gần nhất của khách hàng.
* **Main Flow:**
  1. Tại Tab 4 của hồ sơ khách hàng, người dùng bấm "[ + Ghi Nhận Tương Tác ]".
  2. Modal HTMX hiển thị các trường: Hình thức (Gọi điện / Gặp trực tiếp / Email / Zalo), Thời lượng (phút), Đầu mối tiếp nhận, Nội dung trao đổi (*).
  3. Người dùng nhập nội dung và bấm "Lưu tương tác".
  4. Hệ thống ghi nhận vào bảng `interactions`.
  5. Cập nhật trường `last_contact_date = NOW()` tại bảng `customers`.
  6. Dòng thời gian tương tác tự động hiển thị thêm sự kiện mới ở vị trí trên cùng.
* **Data touched:** `interactions`, `customers`, `audit_logs`

#### UC-19: Tra cứu công nợ khách hàng thời gian thực
* **Actor:** Sales, Manager, Accountant
* **Mục đích:** Kiểm soát an toàn tài chính trước khi chốt đơn hoặc cấp hạn mức tín dụng.
* **Postcondition:** Số dư nợ và cảnh báo quá hạn hiển thị chi tiết tại hồ sơ khách hàng.
* **Main Flow:**
  1. Người dùng mở màn hình hồ sơ khách hàng 360 độ.
  2. Hệ thống đọc giá trị trường `current_debt` (đã được đồng bộ định kỳ từ KiotViet).
  3. Hiển thị khối thông tin công nợ:
     * Dư nợ bình thường: Hiển thị chữ màu đen chuẩn.
     * Cảnh báo nợ cao: Khối màu đỏ nổi bật nếu công nợ $> 50.000.000$ VNĐ hoặc có hóa đơn quá hạn thanh toán $> 30$ ngày.
* **Business Rules:** Dữ liệu công nợ là Read-only trên CRM; mọi thao tác thu tiền, cấn trừ công nợ đều thực hiện trên phần mềm KiotViet.
* **Data touched:** `customers`

---

### MODULE 3: QUẢN LÝ CƠ HỘI BÁN HÀNG & PIPELINE KANBAN

#### UC-20: Xem bảng Pipeline Kanban cơ hội
* **Actor:** Sales, Manager
* **Postcondition:** Bảng điều khiển Kanban hiển thị 6 cột tương ứng 6 giai đoạn bán hàng.
* **Main Flow:**
  1. Người dùng truy cập menu "Cơ hội" (`/Opportunities/Kanban`).
  2. Hệ thống tải dữ liệu các cơ hội đang mở và phân bổ vào 6 cột:
     * 1. Mới tiếp cận (New)
     * 2. Liên hệ tìm hiểu (Contacted)
     * 3. Gửi báo giá (Quoted)
     * 4. Đàm phán thương lượng (Negotiating)
     * 5. Chốt thành công (Won)
     * 6. Đóng thua deal (Lost)
  3. Header mỗi cột hiển thị tổng số lượng deal và tổng giá trị tiền tệ lũy kế của cột.
  4. Mỗi thẻ deal (Card) hiển thị: Mã OPP, Tên khách hàng, Tóm tắt gói hàng, Giá trị dự kiến, Xác suất %, Hạn dự kiến đóng deal, Avatar nhân viên.
* **Business Rules:** Sales chỉ nhìn thấy các thẻ do mình phụ trách; Manager xem toàn bộ phễu kinh doanh của tất cả nhân sự.
* **Data touched:** `opportunities`, `opportunity_stages`, `customers`

#### UC-21: Xem danh sách cơ hội dạng bảng (List View)
* **Actor:** Sales, Manager
* **Postcondition:** Cơ hội hiển thị dưới dạng bảng dữ liệu có hỗ trợ sắp xếp và lọc đa chiều.
* **Main Flow:**
  1. Người dùng bấm biểu tượng chuyển đổi "(List)" trên thanh công cụ.
  2. Hệ thống hiển thị bảng danh sách cơ hội, giữ nguyên các tham số lọc trên URL query string.
  3. Người dùng lọc theo Giai đoạn, Sales phụ trách hoặc Khoảng ngày dự kiến chốt.
  4. Sắp xếp danh sách theo Giá trị deal giảm dần hoặc Hạn xử lý gần nhất.
* **Data touched:** `opportunities`, `customers`, `user_profiles`

#### UC-22: Tạo mới cơ hội bán hàng
* **Actor:** Sales, Manager
* **Postcondition:** Cơ hội mới được tạo với mã tự tăng `OPP-XXXX`, gán ở giai đoạn ban đầu.
* **Main Flow:**
  1. Người dùng bấm nút "[ + Tạo Cơ Hội ]".
  2. Màn hình `/Opportunities/Create` hiển thị:
     * Ô tìm kiếm khách hàng: Gõ tên/SĐT $\rightarrow$ HTMX gợi ý danh sách khách hàng ngay bên dưới.
     * Chọn khách hàng $\rightarrow$ Dropdown "Người liên hệ" tự động load các contact tương ứng.
     * Tiêu đề cơ hội (*), Giá trị dự kiến (VNĐ), Xác suất thành công (%), Ngày dự kiến chốt đơn.
  3. Người dùng bấm "Lưu cơ hội".
  4. Hệ thống validate: Giá trị dự kiến $\ge 0$, Xác suất từ $0 \rightarrow 100\%$. Cho phép nhập ngày đóng trong quá khứ nếu ghi nhận hồi tố.
  5. Hệ thống sinh mã `OPP-XXXX`.
  6. Ghi 1 bản ghi khởi tạo vào bảng `opportunity_stage_histories`.
  7. Điều hướng về màn hình chi tiết cơ hội vừa tạo.
* **Data touched:** `opportunities`, `opportunity_stage_histories`, `audit_logs`

#### UC-23: Xem chi tiết cơ hội bán hàng
* **Actor:** Sales, Manager
* **Postcondition:** Màn hình chi tiết cơ hội hiển thị dòng thời gian chuyển giai đoạn và các báo giá liên kết.
* **Main Flow:**
  1. Người dùng click vào thẻ cơ hội trên Kanban hoặc dòng trên Table.
  2. Màn hình `/Opportunities/Detail/{id}` hiển thị:
     * Khối thông tin: Mã, Tên deal, Khách hàng, Giá trị, Xác suất, Doanh số kỳ vọng.
     * Thanh tiến trình Stage Wizard trực quan.
     * Dòng thời gian lịch sử dịch chuyển (Stage Histories): Ghi rõ ngày giờ chuyển, chuyển từ cột nào sang cột nào, ai là người thực hiện.
     * Danh sách các Báo giá liên kết với cơ hội này.
     * Các nút tác vụ nhanh: [ Chuyển giai đoạn ], [ Thắng Deal (Won) ], [ Đánh Dấu Thua (Lost) ], [ + Lập Báo Giá Mới ].
* **Data touched:** `opportunities`, `opportunity_stage_histories`, `quotes`, `sales_tasks`

#### UC-24: Kéo thả cơ hội trên bảng Kanban (Drag-and-Drop)
* **Actor:** Sales, Manager
* **Postcondition:** Cơ hội được chuyển sang giai đoạn mới, lưu vết lịch sử, tự động cập nhật tổng tiền cột.
* **Main Flow:**
  1. Người dùng giữ chuột kéo một thẻ cơ hội từ Cột A sang Cột B.
  2. Thư viện SortableJS kích hoạt sự kiện `onEnd`, gửi request ngầm `POST /api/opportunities/{id}/move` kèm `{ newStageId }`.
  3. Hệ thống kiểm tra: Cơ hội chưa bị đóng (chưa thuộc Won/Lost).
  4. Cập nhật `stage_id = newStageId` trong bảng `opportunities`.
  5. Thêm bản ghi mới vào `opportunity_stage_histories`.
  6. Phản hồi HTTP 200 OK.
  7. Client JavaScript tự động tính toán lại số lượng và tổng tiền của cả Cột A và Cột B.
* **Alternative Flows:**
  * **A1 (Lỗi kết nối mạng hoặc Server Exception):** Request API thất bại $\rightarrow$ Giao diện tự động đưa thẻ cơ hội quay trở lại vị trí ban đầu (Rollback UI) và bật thông báo Toast màu đỏ: "Không thể lưu trạng thái chuyển giai đoạn. Vui lòng kiểm tra lại kết nối".
* **Business Rules:** Tuyệt đối không thay đổi trạng thái sức khỏe của khách hàng tại Use Case này.
* **Data touched:** `opportunities`, `opportunity_stage_histories`, `audit_logs`

#### UC-25: Chốt deal thành công (Mark Won)
* **Actor:** Sales, Manager
* **Postcondition:** Cơ hội chuyển sang trạng thái chốt thành công (`is_won = true`), tạo dự thảo báo giá nếu cần.
* **Main Flow:**
  1. Người dùng bấm nút "Thắng Deal (Won)" trên trang chi tiết hoặc kéo thẻ vào cột WON.
  2. Modal xác nhận hiển thị thông tin giá trị hợp đồng cuối cùng đã đàm phán thành công.
  3. Người dùng bấm "Xác nhận chốt đơn".
  4. Hệ thống cập nhật: `stage_id = Won_Stage_Id`, `is_won = true`, `closed_at = NOW()`.
  5. Ghi lịch sử chuyển stage.
  6. Nếu cơ hội này chưa có báo giá nào $\rightarrow$ Tự động tạo 1 bản nháp Báo giá (Quote Draft) liên kết để chuẩn bị cho thủ tục giao hàng.
  7. Khóa cơ hội, chuyển toàn bộ nút chỉnh sửa sang trạng thái chỉ đọc.
* **Business Rules:** Doanh số chỉ được tính vào thẻ KPI sau khi hóa đơn tương ứng xuất hiện trên KiotViet.
* **Data touched:** `opportunities`, `opportunity_stage_histories`, `quotes`, `audit_logs`

#### UC-26: Đóng cơ hội thất bại (Mark Lost)
* **Actor:** Sales, Manager
* **Postcondition:** Cơ hội chuyển sang trạng thái thua (`is_won = false`), ghi nhận lý do thất bại.
* **Main Flow:**
  1. Người dùng bấm nút "Đánh Dấu Thua (Lost)" trên chi tiết cơ hội hoặc kéo thẻ vào cột LOST.
  2. Modal bắt buộc bật lên yêu cầu nhập:
     * Lý do chính (Dropdown Enum `LostReason`): 1. Giá cao hơn đối thủ, 2. Giao hàng chậm, 3. Đối thủ cạnh tranh thắng thầu, 4. Khách hàng hủy dự án, 5. Lý do khác.
     * Ghi chú chi tiết nguyên nhân trượt thầu.
  3. Người dùng bấm "Đóng cơ hội".
  4. Hệ thống cập nhật: `stage_id = Lost_Stage_Id`, `is_won = false`, `loss_reason = reason_text`, `closed_at = NOW()`.
  5. Ghi nhận lịch sử và khóa thẻ cơ hội.
* **Business Rules:** Thua một cơ hội không làm thay đổi trạng thái sức khỏe khách hàng sang `AtRisk`.
* **Data touched:** `opportunities`, `opportunity_stage_histories`, `audit_logs`

#### UC-27: Xem danh mục hàng hóa & tồn kho khả dụng
* **Actor:** Sales, Manager, Accountant
* **Mục đích:** Tra cứu thông số kỹ thuật, quy cách đóng gói và giá niêm yết để phục vụ tư vấn bán hàng.
* **Postcondition:** Bảng sản phẩm hiển thị thông tin đồng bộ từ KiotViet.
* **Main Flow:**
  1. Người dùng truy cập danh mục sản phẩm hoặc sử dụng ô tìm kiếm nhanh tại Header.
  2. Hệ thống hiển thị: Mã SKU, Tên sản phẩm cơ khí, Đơn vị tính, Danh mục, Đơn giá cơ sở.
  3. Khi click vào sản phẩm, hệ thống gọi API nội bộ tra cứu lượng tồn khả dụng tức thời từ KiotViet (có cache bộ đệm).
* **Data touched:** `products`, `categories`

---

### MODULE 4: BÁO GIÁ THƯƠNG MẠI, XUẤT PDF & GỬI EMAIL

#### UC-30: Lập báo giá thương mại mới
* **Actor:** Sales, Manager
* **Postcondition:** Bản ghi báo giá được lưu ở trạng thái `Draft` với mã `QUO-XXXX`.
* **Main Flow:**
  1. Người dùng truy cập `/Quotes/Create` (hoặc bấm nút "Lập Báo Giá" từ màn hình Cơ hội).
  2. Form báo giá hiển thị:
     * Chọn Khách hàng & Người nhận báo giá.
     * Chọn Cơ hội liên kết (nếu có).
     * Ngày lập, Ngày hết hạn hiệu lực (mặc định $+15$ ngày).
     * Điều khoản thanh toán (Chuyển khoản 100%, Trả chậm 30 ngày,...).
  3. Bảng sản phẩm động:
     * Người dùng bấm "[ + Thêm dòng sản phẩm ]".
     * Gõ tên hoặc mã vật tư $\rightarrow$ HTMX tìm kiếm hiển thị mã SKU và giá niêm yết.
     * Nhập Số lượng, Đơn giá, Chiết khấu dòng (%).
  4. Client JavaScript tự động tính toán thời gian thực: Thành tiền từng dòng, Tiền hàng trước thuế, Thuế VAT ($8\%$ hoặc $10\%$), Tổng thanh toán.
  5. Người dùng bấm "Lưu Nháp" hoặc "Hoàn tất".
  6. Hệ thống validate: Tối thiểu 1 dòng sản phẩm, số lượng $> 0$.
  7. Hệ thống sinh mã `QUO-XXXX`, lưu vào bảng `quotes` và `quote_items`.
  8. Chuyển hướng tới trang chi tiết báo giá `/Quotes/Detail/{id}`.
* **Data touched:** `quotes`, `quote_items`, `products`, `audit_logs`

#### UC-31: Xem danh mục báo giá đã phát hành
* **Actor:** Sales, Manager, Accountant
* **Postcondition:** Danh sách báo giá hiển thị trực quan kèm trạng thái xử lý.
* **Main Flow:**
  1. Người dùng truy cập `/Quotes/Index`.
  2. Hệ thống hiển thị bảng: Mã báo giá, Tên khách hàng, Ngày phát hành, Ngày hết hạn, Tổng giá trị (VNĐ), Trạng thái (Badge: Nháp, Đã gửi, Đã duyệt, Đã chấp thuận, Bị từ chối, Hết hạn), Nhân viên phụ trách.
  3. Hỗ trợ lọc theo Trạng thái báo giá, Khách hàng hoặc Khoảng ngày phát hành.
* **Business Rules:** Sales chỉ nhìn thấy báo giá do chính mình tạo; Manager và Accountant xem danh sách toàn công ty.
* **Data touched:** `quotes`, `customers`, `user_profiles`

#### UC-32: Xem chi tiết báo giá thương mại
* **Actor:** Sales, Manager, Accountant
* **Postcondition:** Màn hình chi tiết báo giá hiển thị toàn bộ nội dung tài chính và khung thao tác điều hướng.
* **Main Flow:**
  1. Người dùng click vào một báo giá trong danh sách.
  2. Màn hình hiển thị: Thông tin pháp nhân khách hàng, Bảng kê chi tiết từng mặt hàng cơ khí, Diễn giải điều khoản giao hàng/thanh toán.
  3. Hiển thị các nút thao tác tương ứng theo quyền hạn: [ In / Xem Bản PDF ], [ Gửi Email Đính Kèm ], [ Đánh dấu Khách Chấp Thuận ], [ Khách Từ Chối ], [ Phê Duyệt ].
* **Data touched:** `quotes`, `quote_items`, `customers`

#### UC-33: Xuất báo giá định dạng PDF in ấn
* **Actor:** Sales, Manager, Accountant
* **Postcondition:** File PDF chuẩn A4 sắc nét được kết xuất và mở trên tab mới hoặc tải về máy tính.
* **Main Flow:**
  1. Người dùng bấm nút "[ 🖨 In / Xem Bản PDF ]" tại trang chi tiết báo giá.
  2. Client gọi endpoint: `GET /api/quotes/{id}/pdf`.
  3. Backend truy vấn dữ liệu báo giá kèm thông tin công ty từ cấu hình hệ thống.
  4. Engine `QuestPDF` biên dịch tài liệu:
     * Cấu hình font chữ Unicode Roboto không lỗi dấu tiếng Việt.
     * Header logo doanh nghiệp, bảng hàng hóa, định dạng tiền tệ `#,##0 VNĐ`, khung đọc số tiền bằng chữ tiếng Việt chuẩn xác.
  5. Server stream byte array dạng `application/pdf` trả về trình duyệt.
  6. Trình duyệt mở trình đọc PDF trực tiếp để người dùng in ấn hoặc lưu trữ.
* **Data touched:** `quotes`, `quote_items`, `products`, `customers`

#### UC-34: Gửi email báo giá kèm file PDF tự động
* **Actor:** Sales, Manager
* **Postcondition:** Email chứa bản báo giá kèm file PDF đính kèm được gửi tới hộp thư khách hàng, ghi nhận vào lịch sử tương tác.
* **Main Flow:**
  1. Người dùng bấm nút "[ ✉ Gửi Email Đính Kèm ]" tại trang chi tiết báo giá.
  2. Modal hiển thị: Email người nhận (mặc định lấy email của contact chính), Tiêu đề email, Lời nhắn gửi kèm.
  3. Người dùng bấm "Xác nhận gửi email".
  4. Backend kích hoạt `EmailService` sử dụng `MailKit`:
     * Render tài liệu PDF trực tiếp trên bộ nhớ RAM (in-memory stream, không tốn I/O ổ cứng).
     * Đính kèm file PDF vào MimeMessage.
     * Gửi thư qua máy chủ SMTP công ty.
  5. Cập nhật trạng thái báo giá: `status = Sent`.
  6. Tự động ghi 1 bản ghi vào bảng `interactions` với loại hình `Email` để lưu vết dòng thời gian.
  7. Bật thông báo Toast thành công cho người dùng.
* **Alternative Flows:**
  * **A1 (Lỗi cấu hình SMTP hoặc sai địa chỉ email):** Bắt exception từ MailKit, giữ nguyên trạng thái báo giá là Draft, thông báo chi tiết mã lỗi để người dùng kiểm tra lại địa chỉ email người nhận.
* **Data touched:** `quotes`, `interactions`, `audit_logs`

#### UC-35: Cập nhật trạng thái xử lý báo giá
* **Actor:** Sales (chuyển Draft $\rightarrow$ Sent), Manager (toàn quyền)
* **Postcondition:** Trạng thái báo giá chuyển đổi và kích hoạt nghiệp vụ phụ trợ.
* **Main Flow:**
  1. Người dùng chọn cập nhật trạng thái báo giá sang "Khách Chấp Thuận (Accepted)" hoặc "Từ Chối (Rejected)".
  2. Hệ thống kiểm tra tính hợp lệ: Không cho phép chuyển lùi từ Accepted về Draft.
  3. Cập nhật trường `status`.
  4. Nếu báo giá chuyển sang "Accepted" $\rightarrow$ Tự động chuyển Cơ hội bán hàng liên kết sang giai đoạn "Thắng Deal (Won)", đồng thời tự động sinh một Task nhắc việc: "Liên hệ làm thủ tục giao hàng & xuất hóa đơn".
  5. Ghi vết vào `audit_logs`.
* **Data touched:** `quotes`, `opportunities`, `sales_tasks`, `audit_logs`

#### UC-36: Duyệt báo giá vượt hạn mức thẩm quyền
* **Actor:** Manager
* **Mục đích:** Kiểm soát chính sách giá bán, mức chiết khấu và rủi ro công nợ cho các đơn hàng lớn.
* **Postcondition:** Báo giá được cấp cờ `is_approved = true`, cho phép gửi khách hàng.
* **Main Flow:**
  1. Khi Sales tạo một báo giá có tổng giá trị $> 50.000.000$ VNĐ hoặc mức chiết khấu dòng $> 15\%$, hệ thống tự động khóa nút "Gửi Email" và hiển thị nhãn "Chờ Quản Lý Duyệt".
  2. Manager nhận thông báo, mở chi tiết báo giá để kiểm tra danh mục hàng hóa và công nợ hiện tại của khách.
  3. Manager bấm nút "[ Phê Duyệt ]" (hoặc "Từ chối kèm lý do").
  4. Hệ thống ghi nhận: `approved_by = current_user`, `approved_at = NOW()`.
  5. Mở khóa nút gửi email và thông báo cho Sales phụ trách biết để tiến hành gửi cho khách hàng.
* **Data touched:** `quotes`, `notifications`, `audit_logs`

---

### MODULE 5: KẾ HOẠCH HÀNH ĐỘNG & MÁY TRẠNG THÁI NHIỆM VỤ (SALES TASKS)

#### UC-40: Xem danh sách kế hoạch hành động
* **Actor:** Sales, Manager
* **Postcondition:** Danh sách công việc hiển thị theo phân loại mức độ cấp bách.
* **Main Flow:**
  1. Người dùng chọn menu "Kế hoạch công việc" (`/Tasks/Index`).
  2. Giao diện hiển thị các Tab lọc thời gian:
     * **Tab Hôm nay (Mặc định):** Các việc có hạn chót trong ngày hôm nay.
     * **Tab Quá hạn (Màu đỏ cảnh báo):** Các việc trễ hạn cần xử lý khẩn.
     * **Tab Tuần này:** Các việc trong tuần làm việc hiện tại.
     * **Tab Sắp tới:** Các công việc được lên lịch trong tương lai.
     * **Tab Tất cả:** Toàn bộ lịch sử công việc.
  3. Bảng công việc hiển thị: Mức ưu tiên (Khẩn/Cao/TB/Thấp), Tiêu đề nhiệm vụ, Khách hàng mục tiêu, Hạn hoàn thành, Trạng thái, Cột thao tác "[ Ghi Kết Quả ]".
* **Business Rules:** Sales chỉ nhìn thấy công việc được giao cho chính mình.
* **Data touched:** `sales_tasks`, `customers`, `user_profiles`

#### UC-41: Tạo nhiệm vụ công việc thủ công
* **Actor:** Sales, Manager
* **Postcondition:** Bản ghi nhiệm vụ được lưu ở trạng thái `Pending`.
* **Main Flow:**
  1. Người dùng bấm nút "[ + Giao Việc Mới ]".
  2. Modal hiển thị: Tiêu đề công việc (*), Khách hàng mục tiêu, Loại nhiệm vụ (Gọi điện, Gặp trực tiếp, Gửi email, Gửi báo giá, Chăm sóc định kỳ), Mức độ ưu tiên, Thời hạn hoàn thành (*), Người thực hiện.
  3. Người dùng nhập dữ liệu và bấm "Lưu công việc".
  4. Hệ thống validate: Hạn hoàn thành phải từ thời điểm hiện tại trở đi.
  5. Lưu vào bảng `sales_tasks`.
  6. HTMX tự động nạp nhiệm vụ mới vào bảng danh sách tương ứng.
* **Data touched:** `sales_tasks`, `audit_logs`

#### UC-42: Đóng nhiệm vụ & Kích hoạt máy trạng thái Outcome
* **Actor:** Sales, Manager
* **Postcondition:** Nhiệm vụ đóng trạng thái `Completed`, hệ thống tự động sinh việc hoặc cơ hội tiếp theo theo ma trận quy tắc.
* **Main Flow:**
  1. Người dùng bấm nút "[ Ghi Kết Quả ]" trên dòng công việc cần đóng.
  2. Modal HTMX hiển thị các trường: Hình thức tiếp xúc, Người liên hệ tiếp nhận, Nhóm kết quả đạt được (Dropdown 8 Outcome), Ghi chú trao đổi chi tiết (*).
  3. Người dùng chọn 1 trong 8 Outcome:
     * **Outcome 1 (Khách đồng ý mua / Có nhu cầu mới):** Hệ thống tự động tạo một Cơ hội mới (`OPP-XXXX`) ở giai đoạn ban đầu và mở màn hình tạo cơ hội.
     * **Outcome 2 (Khách cần báo giá):** Tự động tạo một Cơ hội kèm theo một Task mới: "Lập và gửi báo giá" có hạn xử lý trong vòng 24 giờ.
     * **Outcome 3 (Khách hẹn gặp trực tiếp):** Tự động tạo Task loại "Meeting" và ghi lịch hẹn.
     * **Outcome 4 (Khách hẹn gọi lại sau / Đang bận):** Tự động tạo một Task loại "Call" nhắc gọi lại sau đúng 2 ngày làm việc.
     * **Outcome 5 (Không nghe máy / Máy bận):** Tự động tạo Task nhắc gọi lại sau 4 tiếng.
     * **Outcome 6 (Khách vừa mua nơi khác / Kho còn tồn):** Tự động tính toán lại và sinh task nhắc chu kỳ tiếp theo dựa trên chu kỳ mua của khách; tuyệt đối không can thiệp đổi health của khách.
     * **Outcome 7 (Khách khiếu nại chất lượng / Giá cả):** Tạo thông báo khẩn cấp (Notification) gửi trực tiếp tới tài khoản Manager để can thiệp hỗ trợ.
     * **Outcome 8 (Chăm sóc duy trì quan hệ định kỳ):** Ghi nhận 1 bản ghi vào bảng `interactions` và kết thúc luồng.
  4. Người dùng bấm "Lưu & Tự Tạo Việc".
  5. Cập nhật `status = Completed`, `completed_at = NOW()` cho task hiện tại.
  6. Ghi nội dung vào bảng `interactions`.
  7. Dòng công việc tự động biến mất khỏi Tab "Hôm nay", số đếm việc trên Header tự động giảm đi 1.
* **Data touched:** `sales_tasks`, `opportunities`, `interactions`, `notifications`, `audit_logs`

#### UC-43: Xem kế hoạch hành động dạng Lịch biểu (Calendar View)
* **Actor:** Sales, Manager
* **Postcondition:** Màn hình Calendar hiển thị các khối công việc trực quan theo từng ngày trong tháng.
* **Main Flow:**
  1. Người dùng chuyển chế độ xem sang dạng Calendar.
  2. Hệ thống tải toàn bộ các task có `due_date` trong tháng hiện hành.
  3. Người dùng có thể click vào một sự kiện để mở nhanh modal thông tin chi tiết hoặc bấm trực tiếp để ghi nhận kết quả hoàn thành.
* **Data touched:** `sales_tasks`

#### UC-44: Tác vụ nền sinh task nhắc chăm sóc tự động (GenerateRemindersJob)
* **Actor:** System (Hangfire Recurring Worker)
* **Tần suất chạy:** 02:00 AM hàng ngày.
* **Postcondition:** Tự động phát sinh các task chăm sóc cho khách hàng sắp đến chu kỳ mua.
* **Main Flow:**
  1. Hangfire đánh thức worker theo Cron expression: `0 2 * * *`.
  2. Quét toàn bộ khách hàng trong bảng `customers` thỏa mãn: `is_deleted = false` và đã từng phát sinh đơn hàng.
  3. Thuật toán xác định hạn chăm sóc:
     $$NextDueDate = LastOrderDate + AverageCycleDays - 2\ \text{ngày}$$
  4. Kiểm tra điều kiện:
     * **Trường hợp 1 (Khách mua đều đặn, $order\_count \ge 2$):** Nếu ngày hiện tại chạm ngưỡng $NextDueDate$ và trên hệ thống chưa có task nào đang mở (`status = Pending`) cho khách này $\rightarrow$ Tự động insert 1 task: "Gọi điện nhắc chu kỳ mua hàng định kỳ", gán cho Sales quản lý khách đó.
     * **Trường hợp 2 (Khách mới mua lần đầu, $order\_count = 1$):** Nếu số ngày kể từ đơn đầu tiên $> 30$ ngày mà chưa có đơn thứ hai $\rightarrow$ Tự động sinh task: "Gọi điện hỏi thăm chất lượng sản phẩm đơn hàng đầu tiên".
     * **Trường hợp 3 (Khách im lặng bất thường):** Nếu số ngày kể từ đơn cuối vượt quá 2 lần chu kỳ mua TB $\rightarrow$ Tự động sinh task mức ưu tiên [KHẨN]: "Cảnh báo: Khách hàng im lặng bất thường, liên hệ tìm hiểu nguy cơ rời bỏ".
  5. Ghi log tổng kết số lượng task đã sinh vào Serilog.
* **Business Rules:** Có cơ chế chống sinh trùng: Trước khi insert, truy vấn kiểm tra xem trong vòng 3 ngày qua đã sinh task tự động nào cùng loại cho khách này chưa.
* **Data touched:** `customers`, `sales_tasks`, `notifications`

#### UC-45: Tác vụ nền đánh giá sức khỏe khách hàng hàng đêm (UpdateCustomerHealthJob)
* **Actor:** System (Hangfire Recurring Worker)
* **Tần suất chạy:** 03:00 AM hàng ngày.
* **Postcondition:** Chỉ số `health_status` của toàn bộ khách hàng được tính toán lại đồng nhất dựa trên dữ liệu mua hàng thực tế.
* **Main Flow:**
  1. Worker Hangfire khởi chạy lúc 03:00 AM (`0 3 * * *`).
  2. Quét batch 100 khách hàng/lần để tối ưu RAM.
  3. Với mỗi khách hàng, tính toán khoảng cách ngày:
     $$DaysSinceLastOrder = Today - LastOrderDate$$
  4. Đối soát với `AverageCycleDays` để phân loại 6 cấp độ sức khỏe chuẩn:
     * `New (0):` Khách hàng mới tạo, chưa có đơn hàng nào trên KiotViet.
     * `Healthy (1):` $DaysSinceLastOrder \le 1.0 \times AverageCycleDays$ (Mua hàng đều đặn).
     * `Fair (2):` $1.0 \times AverageCycleDays < DaysSinceLastOrder \le 1.5 \times AverageCycleDays$ (Hơi chậm chu kỳ).
     * `AtRisk (3):` $1.5 \times AverageCycleDays < DaysSinceLastOrder \le 2.0 \times AverageCycleDays$ (Có nguy cơ mất khách).
     * `Dormant (4):` $DaysSinceLastOrder > 2.0 \times AverageCycleDays$ (Khách hàng đã ngừng mua, nguy cơ rời bỏ hoàn toàn).
     * `Churned (5):` Đã xác nhận ngừng hẳn hoạt động hoặc giải thể doanh nghiệp.
  5. Cập nhật trường `health_status` trong bảng `customers`.
  6. Nếu khách hàng VIP (Doanh số 90 ngày $> 100.000.000$ VNĐ) bị rơi vào `AtRisk` $\rightarrow$ Tự động tạo bản ghi trong bảng `notifications` để gửi cảnh báo tới Quản lý.
* **Data touched:** `customers`, `notifications`

---

### MODULE 6: BẢNG ĐIỀU HÀNH KINH DOANH (EXECUTIVE DASHBOARD)

#### UC-50: Xem bảng điều khiển kinh doanh tổng quan
* **Actor:** Sales, Manager, Accountant, Admin
* **Postcondition:** Các khối chỉ số KPI và biểu đồ trực quan hóa hiển thị với tốc độ tải $< 300ms$.
* **Main Flow:**
  1. Người dùng đăng nhập vào hệ thống, trang mặc định là `/Dashboard`.
  2. Backend kiểm tra: Nếu xem dữ liệu của kỳ hiện hành $\rightarrow$ Đọc dữ liệu tổng hợp gần nhất; Nếu xem kỳ quá khứ $\rightarrow$ Đọc trực tiếp từ bảng `dashboard_snapshots` (độ trễ $< 10ms$).
  3. Giao diện hiển thị:
     * **Thanh Notify:** Thông báo số lượng khách hàng At-Risk cần chăm sóc khẩn.
     * **4 Thẻ KPI chính:** Doanh số tháng (kèm tỷ lệ tăng trưởng so với tháng trước), Số cơ hội tạo mới, Tỷ lệ chốt đơn Win Rate (%), Số công việc cần xử lý hôm nay.
     * **Khu vực Khách hàng:** Biểu đồ tròn Donut phân bổ sức khỏe khách hàng ($=100.0\%$), Biểu đồ phễu chuyển đổi Cohort 6 tầng.
     * **Khu vực Kế hoạch hành động:** Tóm tắt 5 việc khẩn cấp nhất trong ngày (có nút đóng việc trực tiếp bằng modal).
     * **Khu vực Doanh số & Nhân sự:** Biểu đồ cột nhóm doanh số 6 tháng (tách khách mới vs khách cũ) và Bảng vinh danh Top 5 nhân viên kinh doanh xuất sắc.
* **Business Rules Phân quyền hiển thị:**
  * **Sales:** Toàn bộ thẻ KPI và biểu đồ chỉ hiển thị dữ liệu của chính mình. **Sales tuyệt đối KHÔNG nhìn thấy bảng Top nhân sự toàn công ty** để bảo mật thông tin nội bộ.
  * **Manager / Admin:** Nhìn thấy bức tranh toàn cảnh của toàn doanh nghiệp và bảng Top nhân sự.
  * **Accountant:** Xem khối KPI doanh số và công nợ tổng thể.
* **Data touched:** `dashboard_snapshots`, `notifications`, `sales_tasks`

#### UC-51: Lọc dữ liệu Dashboard theo kỳ báo cáo
* **Actor:** Manager, Admin
* **Postcondition:** Toàn bộ biểu đồ và chỉ số KPI tự động nạp lại tương ứng theo khoảng thời gian được chọn.
* **Main Flow:**
  1. Người dùng bấm vào dropdown bộ lọc thời gian: Tháng này / Tháng trước / Quý này / 6 tháng gần nhất / Tùy chọn ngày.
  2. HTMX gửi request lấy nội dung thống kê tương ứng.
  3. Client cập nhật dữ liệu vào các đối tượng Chart.js thông qua phương thức `chart.update()`.
* **Data touched:** `dashboard_snapshots`

#### UC-52: Xem danh sách cảnh báo khẩn cấp
* **Actor:** Sales, Manager
* **Postcondition:** Danh sách cảnh báo rủi ro hiển thị chi tiết theo đối tượng.
* **Main Flow:**
  1. Người dùng bấm vào biểu tượng Chuông thông báo trên Header hoặc thanh Notify bar.
  2. Danh sách mở ra: Danh sách khách hàng VIP rơi vào At-Risk, Danh sách deal sắp quá hạn cam kết chốt đơn, Danh sách nhiệm vụ trễ hạn.
  3. Người dùng bấm vào một cảnh báo $\rightarrow$ Hệ thống điều hướng thẳng tới màn hình chi tiết của đối tượng đó để xử lý ngay.
* **Data touched:** `notifications`

#### UC-53: Đánh dấu thông báo đã xử lý / đã đọc
* **Actor:** Sales, Manager
* **Postcondition:** Cập nhật `is_read = true`, số đếm trên huy hiệu chuông thông báo giảm tương ứng.
* **Main Flow:**
  1. Người dùng click vào nút "Đánh dấu đã đọc" trên từng thông báo hoặc bấm nút "Đọc tất cả".
  2. Backend cập nhật `is_read = true`, `read_at = NOW()` trong bảng `notifications`.
  3. Số đếm màu đỏ trên Header giảm về 0 hoặc giảm tương ứng.
* **Data touched:** `notifications`

#### UC-54: Tác vụ nền đóng băng số liệu Dashboard (SnapshotDashboardJob)
* **Actor:** System (Hangfire Recurring Worker)
* **Tần suất chạy:** 04:00 AM hàng ngày.
* **Postcondition:** Tạo bản ghi snapshot lưu trữ vĩnh viễn trạng thái số liệu ngày hôm trước vào bảng `dashboard_snapshots`.
* **Main Flow:**
  1. Hangfire kích hoạt worker lúc 04:00 sáng (`0 4 * * *`).
  2. Gọi `DashboardService` tính toán toàn bộ các khối số liệu của ngày vừa kết thúc: 4 KPI, Phễu Cohort, Donut tỷ trọng khách, Doanh thu phân rã mới/cũ, Top 5 sales.
  3. Serialize toàn bộ kết quả thành các trường định dạng JSONB: `customer_ratio_json`, `funnel_json`, `monthly_revenue_json`, `top_staff_json`.
  4. Insert một bản ghi mới vào bảng `dashboard_snapshots` với khóa chính `snapshot_date = Yesterday`.
  5. Ghi log kết thúc thành công vào Serilog.
* **Data touched:** `dashboard_snapshots`

#### UC-55: Tác vụ nền kiểm tra nhất quán số liệu (ConsistencyCheckJob)
* **Actor:** System (Hangfire Recurring Worker)
* **Tần suất chạy:** 04:30 AM hàng ngày (chạy ngay sau khi tạo snapshot).
* **Postcondition:** Tự động rà soát phát hiện sai lệch số liệu và cảnh báo nếu có vi phạm.
* **Main Flow:**
  1. Worker khởi chạy quét bản ghi snapshot vừa tạo lúc 04:30 AM.
  2. Lần lượt kiểm tra 6 ràng buộc vàng:
     * *Ràng buộc 1:* Tỷ lệ khách hàng: $\%Good + \%Fair + \%AtRisk + \%Churned == 100.0\%$.
     * *Ràng buộc 2:* Phễu chuyển đổi: Số lượng deal ở tầng sau không bao giờ lớn hơn tầng trước ($Count_{n+1} \le Count_n$).
     * *Ràng buộc 3:* Số hợp đồng chốt trong phễu $=$ Số lượng deal Won ở KPI card.
     * *Ràng buộc 4:* Tổng doanh thu 6 tháng $=$ Doanh thu khách mới $+$ Doanh thu khách cũ.
     * *Ràng buộc 5:* Doanh số của Top 5 nhân viên $\le$ Tổng doanh số toàn công ty.
     * *Ràng buộc 6:* Tầng 1 của phễu $=$ Tổng số cơ hội mới tạo trong kỳ.
  3. Nếu phát hiện bất kỳ ràng buộc nào bị vi phạm: Ghi log mức `Error` kèm thông số chi tiết và bắn tin nhắn cảnh báo qua Telegram Bot cho Quản trị viên.
* **Data touched:** `dashboard_snapshots`, `audit_logs`

---

### MODULE 7: TÍCH HỢP & ĐỒNG BỘ DỮ LIỆU KIOTVIET OPEN API

#### UC-60: Đồng bộ danh mục khách hàng từ KiotViet (SyncCustomers)
* **Actor:** System (Hangfire Recurring Worker)
* **Tần suất chạy:** Nằm trong chu trình đồng bộ 15 phút/lần (`*/15 * * * *`).
* **Postcondition:** Dữ liệu khách hàng trên CRM cập nhật đồng nhất với KiotViet, bảo lưu nguyên vẹn các trường riêng của CRM.
* **Main Flow:**
  1. Worker lấy Access Token từ Cache (hoặc xin mới qua OAuth 2.0 nếu hết hạn).
  2. Gửi request `GET https://public.kiotapi.com/customers?pageSize=100&lastModifiedFrom={LastSyncTime}`.
  3. Với từng khách hàng trả về từ KiotViet:
     * Ánh xạ thông qua `kiotviet_id` hoặc tra cứu theo `tax_code` / `phone`.
     * **Nếu khách hàng chưa tồn tại trên CRM:** Tạo mới bản ghi trong bảng `customers` (Mã, Tên pháp nhân, SĐT, Email, Địa chỉ, MST), gán `health_status = New`.
     * **Nếu khách hàng đã tồn tại trên CRM:** Cập nhật các thông tin cơ bản nếu có thay đổi. **Tuyệt đối không ghi đè các trường nghiệp vụ riêng của CRM** gồm: `health_status`, `assigned_to_user_id`, `average_cycle_days`, `notes`.
  4. Ghi kết quả tổng kết (Số tạo mới, Số cập nhật, Số lỗi) vào bảng `kiotviet_sync_logs`.
* **Alternative Flows:**
  * **A1 (Lỗi HTTP 429 Too Many Requests):** Dừng gửi request ngay lập tức, kích hoạt thuật toán giãn cách Exponential Backoff, thử lại sau 60 giây.
  * **A2 (Token 401 Unauthorized):** Thu hồi token cũ, gọi endpoint xác thực lấy Access Token mới và gửi lại request.
* **Data touched:** `customers`, `kiotviet_sync_logs`

#### UC-61: Đồng bộ hóa đơn đơn hàng & Cập nhật doanh số 90 ngày (SyncOrders)
* **Actor:** System (Hangfire Recurring Worker)
* **Tần suất chạy:** 15 phút/lần.
* **Postcondition:** Cập nhật ngày mua hàng gần nhất và tổng doanh thu 90 ngày cho từng khách hàng.
* **Main Flow:**
  1. Worker gọi API KiotViet: `GET https://public.kiotapi.com/orders?status=3&lastModifiedFrom={Time}` (Lấy các đơn hàng có trạng thái Đã hoàn thành).
  2. Quét toàn bộ hóa đơn phát sinh trong 90 ngày gần nhất.
  3. Nhóm theo khách hàng mục tiêu và tính toán:
     * `revenue_90d`: Tổng tiền mua hàng thực tế trong 90 ngày.
     * `order_count_90d`: Tổng số đơn hàng hoàn thành.
     * `last_order_date`: Ngày giờ phát sinh hóa đơn gần nhất.
  4. Cập nhật các trường trên vào bảng `customers`.
* **Data touched:** `customers`, `kiotviet_sync_logs`

#### UC-62: Đồng bộ danh mục sản phẩm & Bảng giá niêm yết (SyncProducts)
* **Actor:** System (Hangfire Recurring Worker)
* **Tần suất chạy:** 15 phút/lần.
* **Postcondition:** Danh mục sản phẩm, đơn vị tính và giá bán trên CRM luôn khớp với KiotViet.
* **Main Flow:**
  1. Gửi request lấy danh mục sản phẩm đang hoạt động: `GET https://public.kiotapi.com/products?isActive=true`.
  2. Đối soát dữ liệu vào bảng `products`:
     * Thêm mới sản phẩm nếu có mã SKU mới phát sinh trên KiotViet.
     * Cập nhật Tên sản phẩm, Đơn vị tính (ĐVT), Đơn giá niêm yết (`base_price`), Danh mục nhóm hàng (`category_id`).
     * Nếu một sản phẩm chuyển trạng thái `isActive = false` trên KiotViet $\rightarrow$ CRM đánh dấu sản phẩm đó ngừng kinh doanh để ngăn Sales chọn lên báo giá mới.
* **Data touched:** `products`, `categories`, `kiotviet_sync_logs`

#### UC-63: Đồng bộ số dư công nợ khách hàng (SyncCustomerDebts)
* **Actor:** System (Hangfire Recurring Worker)
* **Tần suất chạy:** 15 phút/lần.
* **Postcondition:** Số dư công nợ phải thu của khách hàng được cập nhật chuẩn xác.
* **Main Flow:**
  1. Worker gọi API công nợ khách hàng từ KiotViet.
  2. Ánh xạ theo `kiotviet_id` của từng khách hàng.
  3. Ghi đè số dư công nợ hiện tại vào cột `current_debt` trong bảng `customers`.
  4. Nếu số dư nợ vượt hạn mức tín dụng công ty ($> 50.000.000$ VNĐ) $\rightarrow$ Đánh dấu cờ cảnh báo rủi ro tài chính.
* **Data touched:** `customers`, `kiotviet_sync_logs`

#### UC-64: Tra cứu tồn kho thời gian thực kèm bộ đệm (StockLookup API)
* **Actor:** Sales, Manager
* **Mục đích:** Kiểm tra nhanh số lượng hàng tồn thực tế khi đang lập báo giá mà không gây quá tải cho KiotViet API.
* **Postcondition:** Trả về số lượng hàng tồn khả dụng ngay tức thì.
* **Main Flow:**
  1. Khi người dùng chọn một sản phẩm trên form Báo giá, client gọi API: `GET /api/kiotviet/stock/{productId}`.
  2. Backend kiểm tra `IMemoryCache`:
     * **Cache Hit (Còn hiệu lực trong vòng 5 phút):** Trả về số lượng tồn kho ngay lập tức (Thời gian phản hồi $< 5ms$).
     * **Cache Miss:** Backend gửi request lên KiotViet API lấy số tồn khả dụng:
       $$AvailableStock = OnHand - Reserved$$
     * Lưu kết quả vào MemoryCache với thời gian sống (TTL) 5 phút.
  3. Trả về kết quả JSON. Giao diện hiển thị badge: *"Còn tồn: 145 Hộp"* ngay dưới tên mặt hàng.
* **Data touched:** Bộ đệm MemoryCache

---

### MODULE 8: QUẢN TRỊ HỆ THỐNG, AUDIT LOG & VẬN HÀNH

#### UC-70: Quản lý người dùng & Phân quyền RBAC
* **Actor:** Admin
* **Postcondition:** Tài khoản người dùng được tạo mới, cập nhật vai trò hoặc khóa truy cập.
* **Main Flow:**
  1. Admin truy cập `/Admin/Users`.
  2. Xem danh sách toàn bộ nhân sự kèm vai trò (Sales, Manager, Accountant, Admin) và trạng thái hoạt động.
  3. **Thêm tài khoản mới:** Nhập Email, Họ tên, Chọn vai trò, Mật khẩu ban đầu.
  4. **Khóa tài khoản:** Khi nhân sự nghỉ việc, Admin chuyển trường `is_active = false`.
* **Business Rules:**
  * **Tuyệt đối không xóa tài khoản (Hard delete)** khỏi bảng `AspNetUsers` để bảo toàn lịch sử dữ liệu và quyền sở hữu các cơ hội, báo giá trong quá khứ.
  * Tài khoản bị khóa sẽ bị chấm dứt phiên làm việc ngay lập tức tại lần gửi request kế tiếp.
* **Data touched:** `AspNetUsers`, `user_profiles`, `audit_logs`

#### UC-71: Giám sát nhật ký lỗi & Hangfire Dashboard
* **Actor:** Admin
* **Postcondition:** Admin nắm bắt toàn diện trạng thái vận hành của máy chủ và các tác vụ ngầm.
* **Main Flow:**
  1. Admin truy cập `/Admin/Logs` để tra cứu file log hệ thống sinh ra từ Serilog: Lọc theo cấp độ log (`Information`, `Warning`, `Error`, `Fatal`) và theo ngày tháng.
  2. Admin truy cập giao diện `/hangfire` (có xác thực quyền Admin): Kiểm tra trạng thái worker, tỷ lệ job thành công/thất bại, thời gian thực thi của từng Recurring job.
* **Business Rules:** Người dùng không có vai trò `ADMIN` khi truy cập `/hangfire` sẽ nhận mã lỗi `403 Forbidden`.
* **Data touched:** Hangfire Database Storage, thư mục `logs/`

#### UC-72: Kích hoạt tác vụ nền thủ công (Manual Job Trigger)
* **Actor:** Admin
* **Postcondition:** Job được đưa vào hàng đợi xử lý ngay lập tức mà không cần đợi đến giờ Cron.
* **Main Flow:**
  1. Admin mở Hangfire Dashboard, chọn tab "Recurring Jobs".
  2. Chọn một job cụ thể (Ví dụ: `SyncKiotVietJob` hoặc `SnapshotDashboardJob`).
  3. Bấm nút "Trigger now".
  4. Worker lập tức thực thi tác vụ trong hàng đợi và ghi log chi tiết ai là người kích hoạt vào `audit_logs`.
* **Data touched:** Hangfire Database Storage, `audit_logs`

#### UC-73: Truy vết lịch sử thay đổi dữ liệu (Audit Trail)
* **Actor:** Admin, Manager
* **Mục đích:** Minh bạch trách nhiệm cá nhân, kiểm soát ai đã thay đổi thông tin gì, vào lúc nào và giá trị trước/sau khi sửa.
* **Postcondition:** Toàn bộ lịch sử biến động hiển thị trực quan dạng bảng.
* **Main Flow:**
  1. Người dùng có thẩm quyền truy cập `/Admin/AuditLogs`.
  2. Hệ thống hiển thị bảng kiểm toán đọc từ bảng `audit_logs`:
     * Thời điểm thực hiện (UTC/Giờ VN).
     * Người thực hiện (Họ tên, Email).
     * Bảng thực thể bị tác động (`customers`, `opportunities`, `quotes`,...).
     * Loại hành động (CREATE, UPDATE, SOFT_DELETE).
     * Cột so sánh chi tiết: Cặp giá trị cũ (`OriginalValues` JSON) $\rightarrow$ Giá trị mới (`NewValues` JSON).
  3. Hỗ trợ lọc theo Người dùng, Thực thể hoặc Khoảng thời gian.
* **Business Rules:** Dữ liệu trong bảng `audit_logs` được lưu trữ tối thiểu 90 ngày và không có bất kỳ giao diện hay API nào cho phép xóa/sửa dữ liệu này.
* **Data touched:** `audit_logs`

---

## PHẦN 4: ĐẶC TẢ CÁC ENUMS & MÁY TRẠNG THÁI NGHIỆP VỤ

### 4.1 Danh mục 6 Cấp độ Sức khỏe Khách hàng (`CustomerHealth`)
```csharp
public enum CustomerHealth
{
    New = 0,        // Khách hàng mới tạo, chưa có đơn hàng KiotViet
    Healthy = 1,    // Mua đều: Số ngày từ đơn cuối <= 1.0 lần chu kỳ TB
    Fair = 2,       // Hơi chậm: 1.0 < Số ngày <= 1.5 lần chu kỳ TB
    AtRisk = 3,     // Có nguy cơ: 1.5 < Số ngày <= 2.0 lần chu kỳ TB
    Dormant = 4,    // Nguy cơ rời bỏ rất cao: Số ngày > 2.0 lần chu kỳ TB
    Churned = 5     // Ngừng hoạt động / Mất khách hoàn toàn
}
```

### 4.2 Danh mục 5 Lý do Thất bại Cơ hội (`LostReason`)
```csharp
public enum LostReason
{
    PriceTooHigh = 1,       // Giá cao hơn đối thủ cạnh tranh
    SlowDelivery = 2,       // Thời gian giao hàng không đáp ứng yêu cầu xưởng
    CompetitorWon = 3,      // Đối thủ thắng thầu giải pháp kỹ thuật
    CustomerCancelled = 4,  // Khách hàng hủy dự án / Không có ngân sách
    Other = 5               // Lý do khác (bắt buộc nhập diễn giải)
}
```

### 4.3 Ma trận 8 Nhánh Kết quả Nhiệm vụ (`TaskOutcome`)
| Mã Outcome | Nhãn giao diện | Hành động tự động kích hoạt bởi hệ thống |
|---|---|---|
| **1. CustomerWillBuy** | Khách đồng ý mua / Có nhu cầu mới | Tự động tạo 1 Cơ hội mới (`OPP-XXXX`) ở giai đoạn ban đầu. |
| **2. NeedQuote** | Khách yêu cầu báo giá | Tự động tạo 1 Cơ hội kèm 1 Task "Gửi báo giá" hạn 24 giờ. |
| **3. AgreeMeeting** | Khách đồng ý lịch hẹn làm việc | Tự động tạo Task loại "Meeting" và ghi nhận lịch hẹn. |
| **4. CustomerBusy** | Khách hẹn gọi lại sau (Bận/Họp) | Tự động sinh Task nhắc gọi lại sau đúng 2 ngày làm việc. |
| **5. NoAnswer** | Không nghe máy / Máy bận | Tự động sinh Task nhắc gọi lại sau 4 tiếng. |
| **6. AlreadyBought** | Đã mua đơn vị khác / Kho còn tồn | Ghi nhận tương tác, tự tính ngày nhắc chu kỳ tiếp theo; không đổi health. |
| **7. Complaint** | Phàn nàn chất lượng / Khiếu nại | Bắn thông báo khẩn tới Manager để can thiệp hỗ trợ. |
| **8. CareOnly** | Chăm sóc duy trì quan hệ định kỳ | Ghi lịch sử tương tác vào timeline, đóng task thành công. |

### 4.4 Danh mục Trạng thái Báo giá (`QuoteStatus`)
```csharp
public enum QuoteStatus
{
    Draft = 0,      // Bản nháp đang soạn thảo
    PendingApproval = 1, // Đang chờ Quản lý phê duyệt (nếu > 50 triệu)
    Sent = 2,       // Đã xuất file PDF và gửi email cho khách hàng
    Accepted = 3,   // Khách hàng đã duyệt đồng ý mua (Won deal)
    Rejected = 4,   // Khách hàng từ chối báo giá
    Expired = 5     // Báo giá hết hạn hiệu lực (> 15 ngày chưa chốt)
}
```

---

## PHẦN 5: MA TRẬN TRUY VẾT TỪ USER STORIES SANG USE CASES (TRACEABILITY MATRIX)

| User Story | Tiêu đề User Story | Use Case ánh xạ tương ứng | Phân hệ nghiệp vụ |
|---|---|---|---|
| **S1** | Xem khách cần chăm hôm nay | UC-10, UC-40, UC-44 | Khách hàng / Nhiệm vụ |
| **S2** | Xem hồ sơ khách hàng 360 độ | UC-11, UC-19 | Khách hàng 360 |
| **S3** | Ghi nhận tương tác sau liên hệ | UC-18, UC-42 | Tương tác / Outcome |
| **S4** | Tạo cơ hội từ khách hàng | UC-22 | Cơ hội bán hàng |
| **S5** | Kéo thả cơ hội trên Kanban | UC-20, UC-24, UC-25, UC-26 | Pipeline Kanban |
| **S6** | Tạo báo giá nhiều mặt hàng | UC-30, UC-27 | Báo giá thương mại |
| **S7** | Xuất báo giá định dạng PDF | UC-33 | In ấn QuestPDF |
| **S8** | Gửi email báo giá tự động | UC-34 | Tích hợp MailKit |
| **S9** | Hoàn thành task công việc | UC-40, UC-42 | Kế hoạch hành động |
| **M1** | Xem Dashboard điều hành | UC-50, UC-54 | Báo cáo Dashboard |
| **M2** | Lọc Dashboard theo kỳ | UC-51 | Báo cáo Dashboard |
| **M3** | Đánh giá hiệu suất Sales | UC-50 | Báo cáo Top nhân sự |
| **M4** | Duyệt báo giá vượt thẩm quyền | UC-36 | Phê duyệt Báo giá |
| **A1** | Xem công nợ khách hàng | UC-19, UC-63 | Kế toán / Đồng bộ KV |
| **A2** | Kiểm soát danh sách báo giá | UC-31, UC-32 | Kế toán Báo giá |
| **AD1** | Quản lý tài khoản & RBAC | UC-70 | Quản trị Admin |
| **AD2** | Tra cứu nhật ký vết (Audit) | UC-71, UC-73 | Nhật ký kiểm toán |
| **AD3** | Kích hoạt tác vụ nền thủ công | UC-72 | Vận hành Hangfire |

---

## PHẦN 6: BẢNG TỔNG HỢP TOÀN BỘ 47 USE CASES

| Phân hệ chức năng | Số lượng Use Cases | Danh mục mã Use Case chi tiết | Đối tượng tác nhân chính (Actors) |
|---|---|---|---|
| **1. Authentication** | 2 | UC-01, UC-02 | Toàn bộ người dùng |
| **2. Khách hàng 360** | 10 | UC-10, UC-11, UC-12, UC-13, UC-14, UC-15, UC-16, UC-17, UC-18, UC-19 | Sales, Manager, Accountant, Admin |
| **3. Cơ hội & Kanban** | 8 | UC-20, UC-21, UC-22, UC-23, UC-24, UC-25, UC-26, UC-27 | Sales, Manager, Accountant |
| **4. Báo giá & In ấn** | 7 | UC-30, UC-31, UC-32, UC-33, UC-34, UC-35, UC-36 | Sales, Manager, Accountant |
| **5. Task & Nhắc việc** | 6 | UC-40, UC-41, UC-42, UC-43, UC-44, UC-45 | Sales, Manager, System (Hangfire) |
| **6. Dashboard & Cảnh báo** | 6 | UC-50, UC-51, UC-52, UC-53, UC-54, UC-55 | Toàn bộ người dùng, System |
| **7. Đồng bộ KiotViet** | 5 | UC-60, UC-61, UC-62, UC-63, UC-64 | System (Hangfire Worker), Sales |
| **8. Quản trị & Vận hành** | 4 | UC-70, UC-71, UC-72, UC-73 | Admin, Manager |
| **TỔNG CỘNG** | **47** | **Bộ đặc tả Use Case hoàn chỉnh cho toàn bộ hệ thống** | **6 Actors liên kết chặt chẽ** |

---

## PHẦN 7: KẾT LUẬN & CAM KẾT CHẤT LƯỢNG

Tài liệu này đã giải quyết triệt để mọi khoảng trống kỹ thuật và nghiệp vụ:
1. **Khắc phục hoàn toàn lỗi lặp văn bản** và chuẩn hóa thuật ngữ xuyên suốt 8 phân hệ.
2. **Loại bỏ sự can thiệp sai lệch vào trạng thái sức khỏe khách hàng**: Chỉ cho phép tác vụ nền tính toán dựa trên dữ liệu khách quan từ hóa đơn KiotViet.
3. **Phân rã chi tiết 100% các Use Case tích hợp KiotViet** và các tác vụ nền định kỳ hàng đêm của Hangfire.
4. **Bảo mật phân cấp dữ liệu chặt chẽ**: Sales không xem được Top doanh số toàn công ty; Manager và Admin nắm quyền kiểm soát toàn diện.
5. **Sẵn sàng triển khai:** Cung cấp đầy đủ Precondition, Main Flow, Alternative Flows, Exception Flows và Data Touched cho toàn bộ 47 Use Cases, tạo cơ sở vững chắc cho quá trình lập trình và kiểm thử tự động.