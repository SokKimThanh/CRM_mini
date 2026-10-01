# AGENTS.md - BỘ NHỚ LẬP TRÌNH CHO AI (JULES)

Đây là tài liệu bắt buộc (Mandatory) dành cho bất kỳ AI/Agent nào khi làm việc trong dự án này.
Mọi hành động sinh code, chat, tạo nhánh phải tuân thủ nghiêm ngặt các Block dưới đây.

---

## BLOCK 1: [CORE_DIRECTIVES] - NGUYÊN TẮC CỐT LÕI

**[K0] Skill Orchestration (Liên kết đa nhiệm)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: KHÔNG BAO GIỜ dùng 1 kỹ năng đơn lẻ. Mọi phản hồi phải là một chuỗi hành động (Chain of Actions).
- `TRIGGER`: Nhận mọi task từ User.
- `ACTION`: Nhận Task -> Đọc DB/Liên kết (K2) -> Băm nhỏ task (K3) -> Code (K10,K11) -> Cập nhật Docs (K6, K7) -> Lập báo cáo Nợ Kỹ thuật (K4) -> Git Commit (K5).

**[K1] Plain Language Policy (Bình Dân Học Vụ)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Nói ít làm nhiều. Phải giải thích code/tài liệu bằng từ ngữ cực kỳ đơn giản.
- `TRIGGER`: Khi phải sinh Chat, viết Docs, README, Code Comments.
- `ACTION`: Lọc bỏ từ ngữ học thuật phức tạp. Tập trung vào thực thi (Ví dụ: "Tôi đã tạo hàm để lấy dữ liệu" thay vì "Thực thi Repository Pattern Dependency Injection").

---

## BLOCK 2: [AGILE_WORKFLOW] - QUY TRÌNH QUẢN LÝ DỰ ÁN

**[K2] System Thinking (Tư Duy Hệ Thống - "Nói ít hiểu nhiều")**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: KHÔNG BẮT TAY VÀO LÀM NGAY (Code/Docs) nếu chưa nắm rõ bức tranh tổng thể. Hệ thống hóa thông tin trước khi hành động.
- `TRIGGER`: User yêu cầu tính năng, viết tài liệu hoặc xử lý luồng logic có ảnh hưởng diện rộng.
- `ACTION`:
  1. Tự động đọc Master Plan/Roadmap (trong `docs/`) và quét DB để xác định dự án đang ở giai đoạn nào (Sprint mấy).
  2. Bắt mạch bối cảnh: Tự hỏi "Việc này có nằm trong mục tiêu giai đoạn hiện tại không? Có phá vỡ cấu trúc cũ không?".
  3. Chỉ hỏi/đề xuất lại người dùng bằng các câu ngắn gọn, trúng đích, đưa ra 1-2 tùy chọn cụ thể. Tuyệt đối không giải thích dài dòng quá trình mình đã phân tích.

**[K3] Auto-Prompting (Tự động chia nhỏ task)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi task phải được băm nhỏ thành các "Phiên làm việc" < 30 phút.
- `TRIGGER`: Đầu Sprint hoặc khi nhận 1 Spec lớn.
- `ACTION`: AI tự sinh ra các Prompt thực thi (Actionable Prompts) và lưu vào `docs/sprints/sprint_<X>/prompts/`. Đợi User dùng các prompt này để ra lệnh tiếp.

**[K4] Tech Debt Management (Quản lý Nợ Kỹ Thuật)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Phải minh bạch về code tạm, mã rác.
- `TRIGGER`: Cuối mỗi task hoặc khi để lại TODO/Hardcode.
- `ACTION`: Tự động tạo hoặc append báo cáo vào `docs/sprints/sprint_<X>/tech_debt_report.md`.

**[K14] Context-First Verification (Bắt mạch dự án)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: CẤM NHẮM MẮT LÀM BỪA khi nhận một yêu cầu chung chung. Phải định vị được Context trước khi thực thi.
- `TRIGGER`: Khi User giao một task thiếu ngữ cảnh (vd: "Viết Runbook", "Tạo API", "Sửa bug").
- `ACTION`:
  1. Tự động tra cứu hệ thống (`list_files`, `README`, Master Plan).
  2. Không báo cáo dài dòng về quá trình tra cứu.
  3. Dùng ngôn ngữ đơn giản chốt lại 1-2 tùy chọn thực thi thực tế nhất với người dùng (vd: "Em thấy dự án có 7 Sprints, anh muốn làm Sprint 1 hay 2?").

---

## BLOCK 3: [GIT_AND_FILESYSTEM] - QUẢN LÝ NHÁNH VÀ FOLDER

**[K5] Git Workflow & Auto-Branching**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: AI TỰ ĐỘNG khởi tạo nhánh chuẩn nếu chưa có (`main`, `staging`, `develop`, `docs`).
- `TRIGGER`: Trước khi bắt đầu viết mã mới.
- `ACTION`: Chạy `git branch`. Nếu tạo tính năng, tự checkout sang `feature/<name>`. Khi hoàn thành, format commit chuẩn (Conventional Commits: `feat:`, `fix:`).

**[K8] Runbook Folder Compliance (Tuân thủ cấu trúc thư mục)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Không vứt file lung tung. Phải đúng Runbook.
- `TRIGGER`: Khi bắt đầu Sprint mới hoặc tạo file tài liệu mới.
- `ACTION`: Tự tạo thư mục `docs/sprints/sprint_<X>/...`. Tự động dời (move) file nếu người dùng lưu sai chỗ.

---

## BLOCK 4: [DOCUMENTATION_RULES] - QUẢN LÝ TÀI LIỆU

**[K6] Append-Only & Document Continuity (Ghi nối tiếp - "Nói ít làm nhiều")**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: CẤM TUYỆT ĐỐI ghi đè (Overwrite) hoặc xóa trắng tài liệu cũ. Mọi tài liệu sinh mới phải kế thừa và trích dẫn tài liệu gốc.
- `TRIGGER`: Khi cập nhật SRS, Runbook, API Docs, hoặc tạo tài liệu mới.
- `ACTION`:
  1. Khi cập nhật: Đọc hiểu file cũ, dùng phương pháp thêm nội dung (Append/Insert) vào cuối mục lục phù hợp.
  2. Khi tạo mới: Bắt buộc trích dẫn (link ngược) về tài liệu Master Plan/Tiến trình. Nội dung không được tự phịa thêm scope mới.
  3. Viết trực diện: Đi thẳng vào các bước thực thi (Actionable steps). Không sinh ra các câu văn template sáo rỗng, dài dòng (Kế thừa K1).

**[K7] Archiving (Dọn dẹp Tài liệu cổ đại)**
- `[SEVERITY]`: 🟢 Medium
- `[SPRINT]`: All
- `THUMB_RULE`: Tài liệu sai lệch code phải bị cô lập.
- `TRIGGER`: Phát hiện Docs mâu thuẫn với Code mới.
- `ACTION`: Gắn tag `[DEPRECATED]` vào đầu file cũ, tạo file mới, move file cũ vào `docs/archive/`.

**[K9] Auto-README Refactoring (Tự động cắt nhỏ README)**
- `[SEVERITY]`: 🟢 Medium
- `[SPRINT]`: All
- `THUMB_RULE`: `README.md` KHÔNG ĐƯỢC dài quá 1 trang A4 (~100 dòng).
- `TRIGGER`: Mọi lúc sửa `README.md`.
- `ACTION`: Tự cắt nội dung dài ra file lẻ, lưu vào `docs/`, chỉ giữ lại Table of Content (Mục lục) ở README gốc.

---

## BLOCK 5: [TECH_STACK_MASTERY] - LUẬT LẬP TRÌNH CHUYÊN SÂU (.NET 8)

**[K10] Backend & Code Convention**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Bắt buộc SOLID, PascalCase, camelCase.
- `TRIGGER`: Khi sinh code C#.
- `ACTION`: Cấm Hardcode. Mọi service phải Inject qua Constructor. Các hàm gọi DB bắt buộc dùng `async/await`.

**[K11] Blazor UI & System Design**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Phân tách rạch ròi Smart UI và Dumb UI.
- `TRIGGER`: Khi sinh UI `.razor`.
- `ACTION`: Dùng MudBlazor. Không inject DbContext vào `.razor`. Bọc ErrorBoundary. Khi dùng JS (SortableJS/Charts) phải giải phóng bộ nhớ (Dispose). Quản lý cẩn thận State khi SignalR đứt gãy.

**[K12] Database Optimization (TỐI ƯU CỰC HẠN EF Core)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Cấm N+1 query. Cấm tải thừa RAM.
- `TRIGGER`: Khi viết logic lấy Data.
- `ACTION`: Mọi list phải dùng `IQueryable` với `.Skip().Take()`. Dữ liệu chỉ đọc phải có `.AsNoTracking()`. Dùng DTO `.Select()`. Đề xuất Index DB cho các cột filter. Không tính toán Real-time trên UI cho Dashboard mà phải dùng Snapshot qua Hangfire.

**[K13] API Integration (Giao tiếp Ngoại)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 4
- `THUMB_RULE`: API phải chịu lỗi tốt.
- `TRIGGER`: Gọi API KiotViet.
- `ACTION`: Dùng `HttpClientFactory`, áp dụng `Polly` cho Retry/Circuit Breaker.

---

## BLOCK 6: [SECURITY_AND_SECRETS] — Bảo mật

**[K15] Secret Management**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Connection string, API key, password KHÔNG BAO GIỜ trong source code.
- `TRIGGER`: Mọi file config, mọi lệnh scaffold, mọi đoạn code kết nối DB/API.
- `ACTION`:
  1. Dev: dùng `dotnet user-secrets`.
  2. Prod: dùng Environment Variables hoặc Vault.
  3. Trong git chỉ có `appsettings.json` với placeholder `#{SECRET}#`.
  4. Trước khi commit: chạy check regex tìm `password=`, `secret=`, `key=`, `token=` trong toàn repo.

**[K16] Input Validation & OWASP Top 10**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Mọi input từ user PHẢI được validate server-side trước khi chạm tới DB.
- `TRIGGER`: Form, API endpoint, query string, file upload.
- `ACTION`:
  1. Server-side validation bắt buộc (không tin client).
  2. Dùng FluentValidation tích hợp vào CQRS Pipeline cho business rules phức tạp, DataAnnotations cho rule đơn giản.
  3. Check OWASP Top 10 trước khi release: SQL Injection, XSS, CSRF.

**[K17] Password & Session Policy**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Không được tự nghĩ ra cơ chế auth mới.
- `TRIGGER`: Mọi code liên quan đến user login, password, session, token.
- `ACTION`:
  1. Password hash dùng `BCrypt.Net` hoặc Identity mặc định (PBKDF2).
  2. Password tối thiểu 8 ký tự, có số + chữ + ký tự đặc biệt.
  3. Lockout sau 5 lần sai, khóa 15 phút.
  4. Cookie: `HttpOnly=true`, `Secure=true`, `SameSite=Lax`.
  5. Session timeout 8h, sliding expiration.

**[K18] Authorization (RBAC)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Phân quyền theo role rõ ràng.
- `TRIGGER`: Truy cập dữ liệu nhạy cảm hoặc tính năng quản trị.
- `ACTION`:
  1. Sales chỉ thấy khách hàng và cơ hội của chính mình.
  2. Manager/Admin thấy toàn bộ hệ thống.
  3. Kiểm tra Role ở mức Handler/Service, không chỉ ở mức UI.

---

## BLOCK 7: [TESTING_STRATEGY] — Kiểm thử

**[K19] Test Pyramid (70/20/10)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Mỗi Service/Handler PHẢI có ít nhất 1 test cho mỗi public method.
- `TRIGGER`: Viết xong logic trong tầng Business/Application.
- `ACTION`:
  1. Unit test (70%): Test logic, không chạm DB.
  2. Integration test (20%): Test với DB PostgreSQL thật.
  3. E2E test (10%): Test luồng UI cơ bản.
  4. Coverage target: >70% cho tầng Business.

**[K20] Test Data Hygiene**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 2
- `THUMB_RULE`: Test KHÔNG được dùng data thật của khách hàng.
- `TRIGGER`: Mọi test cần dữ liệu từ DB.
- `ACTION`:
  1. Dùng `TestContainers` để khởi tạo DB tạm.
  2. Mỗi test tự tạo data riêng, không share data.
  3. Rollback sau mỗi test (dùng transaction).

**[K21] Mocking External Services**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 2
- `THUMB_RULE`: KiotViet API hoặc External Service phải được mock khi test.
- `TRIGGER`: Viết test cho logic tích hợp.
- `ACTION`:
  1. Dùng thư viện Mock (như `Moq`) để tạo Service giả.
  2. Chuẩn bị file JSON mẫu cho các HTTP response.
  3. Test các case lỗi phổ biến: timeout, 429 Too Many Requests, 401 Unauthorized.

---

## BLOCK 8: [ERROR_HANDLING_AND_LOGGING] — Xử lý lỗi & Logging

**[K22] Structured Logging (Serilog)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Không dùng `Console.WriteLine`. Log có cấu trúc JSON.
- `TRIGGER`: Xử lý ngoại lệ, luồng nghiệp vụ quan trọng.
- `ACTION`:
  1. Dùng Serilog với structured logging.
  2. Tuyệt đối không log: password, token, PII (số thẻ, CCCD).
  3. Phân cấp Level rõ ràng: Debug, Info, Warn, Error, Fatal.

**[K23] Global Exception Handler**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Không để exception rò rỉ ra UI.
- `TRIGGER`: Xử lý HTTP Request hoặc vòng đời Blazor Component.
- `ACTION`:
  1. Dùng `UseExceptionHandler` middleware cho API.
  2. Dùng `ErrorBoundary` ở `MainLayout` cho Blazor UI để tránh crash SignalR.
  3. User thấy message thân thiện. Log lưu chi tiết kèm stack trace.

**[K24] Correlation ID**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 2
- `THUMB_RULE`: Mọi request phải có ID định danh để trace lỗi hệ thống.
- `TRIGGER`: HTTP Middleware.
- `ACTION`:
  1. Sinh `CorrelationId` (GUID) ở đầu pipeline.
  2. Bổ sung ID này vào tất cả các log entry của request đó.
  3. Trả về response header để hỗ trợ debug khi user báo lỗi.

**[K25] UI Error States (4 States)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 2
- `THUMB_RULE`: Không có màn hình trắng bao giờ.
- `TRIGGER`: Thiết kế/Code UI list, form, dashboard.
- `ACTION`:
  1. Loading: Skeleton hoặc Spinner.
  2. Empty: "Chưa có dữ liệu" + Nút Call-to-action.
  3. Error: "Có lỗi xảy ra" + Nút "Thử lại".
  4. Success: Hiển thị data bình thường.

---

## BLOCK 9: [DEPLOYMENT_AND_DEVOPS] — Triển khai & DevOps

**[K26] Rollback Plan**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 3
- `THUMB_RULE`: Mọi lần deploy PHẢI có đường lùi an toàn.
- `TRIGGER`: Trước khi deploy lên Production.
- `ACTION`:
  1. Git tag phiên bản trước khi deploy.
  2. Backup Database.
  3. Có sẵn 3 bước hướng dẫn rollback trong runbook.

**[K27] Backup & Restore**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 3
- `THUMB_RULE`: Dữ liệu là mạng sống. Phải có bản sao lưu định kỳ.
- `TRIGGER`: Thiết lập hạ tầng/DB.
- `ACTION`:
  1. Cronjob `pg_dump` mỗi đêm (1h sáng).
  2. Nén và upload sang Cloud Storage (VD: Backblaze B2/S3). Giữ 30 bản.
  3. Thử nghiệm test restore DB ít nhất 1 lần/tháng.

**[K28] Zero-Downtime Deploy**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 3
- `THUMB_RULE`: User không được thấy downtime lúc deploy code mới.
- `TRIGGER`: Triển khai Production.
- `ACTION`:
  1. Dùng `systemctl reload` (graceful reload) thay vì restart cứng.
  2. Script Migration DB phải chạy trước, an toàn rồi mới deploy code.
  3. Luôn có bước Smoke Test tự động sau deploy.

**[K29] Health Check Endpoint**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 3
- `THUMB_RULE`: Phải có công cụ biết hệ thống đang "sống" hay "chết".
- `TRIGGER`: Setup project / DevOps config.
- `ACTION`:
  1. Tạo endpoint `/health` trả về HTTP 200.
  2. Endpoint này kiểm tra kết nối DB, Hangfire, và dung lượng ổ cứng.

---

## BLOCK 10: [QUALITY_GATES] — Cổng chất lượng

**[K30] Definition of Done (DoD)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: "Done" không phải là "code chạy trên máy tôi".
- `TRIGGER`: Đánh dấu hoàn thành tính năng.
- `ACTION`:
  1. Code build sạch (không warning/error).
  2. Có unit test.
  3. Đã test manual trên local.
  4. Không còn TODO chưa log.
  5. Commit theo chuẩn Conventional.

**[K31] Pre-commit Checklist**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Chặn lỗi rác vào repo.
- `TRIGGER`: Trước khi `git commit`.
- `ACTION`:
  1. Chạy `dotnet format`.
  2. Chạy `dotnet build`.
  3. Chạy `dotnet test`.
  4. Đảm bảo không commit secret.

**[K32] Dependency Version Lock**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 1
- `THUMB_RULE`: Quản lý chặt version thư viện, tránh lỗi ngầm do auto-update.
- `TRIGGER`: Cài đặt package mới.
- `ACTION`:
  1. Khóa version cụ thể (vd: 10.0.0), không dùng wildcard (10.*).
  2. Có lịch sử ghi chú upgrade version trong `docs/dependencies.md`.

**[K33] DB First Rule (Không Migration)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 2
- `THUMB_RULE`: Dự án áp dụng DB First, KHÔNG dùng EF Migrations.
- `TRIGGER`: Thay đổi cấu trúc database.
- `ACTION`:
  1. Viết SQL script thủ công để update database.
  2. Chạy script SQL.
  3. Dùng lệnh EF Core Scaffold để cập nhật lại Entities từ DB.

**[K45] Dry-run & Cross-Check (Chạy khô & Kiểm tra chéo)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Làm việc như một Biên tập viên: Đọc kỹ, sửa đúng chỗ, tự đếm lại số liệu, và "chạy thử trong đầu" trước khi bàn giao. Không đoán bừa.
- `TRIGGER`: Khi viết/sửa Runbook, sinh SQL Script, hoặc copy mã nguồn/Entity.
- `ACTION`:
  1. Đọc kỹ & Sửa cục bộ (Diff): KHÔNG ĐƯỢC sinh lại toàn bộ file nếu chỉ sửa vài dòng. Phải tìm đúng vị trí và chỉ thay đổi vị trí đó.
  2. Cross-Check Toán học: Mọi con số trong text mô tả phải khớp tuyệt đối với số liệu trong Code/SQL/Bảng biểu đi kèm. Tự cộng/trừ lại.
  3. Dry-run (Phòng ngừa rủi ro): Tự hỏi "Chạy lệnh này lần 2 có lỗi không?", "Copy class này Namespace có sai không?".
  4. Minh bạch môi trường: Ghi chú rõ những gì là suy luận chưa test thật để User chú ý.

---

## BLOCK 11: [DATA_INTEGRITY] — Toàn vẹn dữ liệu

**[K34] Concurrency Handling**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 2
- `THUMB_RULE`: Bắt buộc xử lý đồng thời, không để user ghi đè dữ liệu của nhau.
- `TRIGGER`: Update các entity quan trọng (VD: Hóa đơn, Khách hàng).
- `ACTION`:
  1. Sử dụng cột `row_version` (hoặc xmin) làm concurrency token.
  2. Khi có conflict, chặn save và báo lỗi "Dữ liệu đã bị người khác sửa đổi".

**[K35] Timezone Rule (UTC ↔ GMT+7)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Chuẩn hóa múi giờ hệ thống.
- `TRIGGER`: Lưu/Hiển thị ngày tháng.
- `ACTION`:
  1. DB luôn lưu giờ UTC (`TIMESTAMPTZ` ở PostgreSQL).
  2. Code C# dùng `DateTime.UtcNow`. Tuyệt đối không dùng `DateTime.Now`.
  3. Tại tầng UI (Blazor), convert giờ UTC sang local (GMT+7) để hiển thị cho user.

**[K36] Soft Delete Policy**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 2
- `THUMB_RULE`: Không xóa cứng dữ liệu quan trọng.
- `TRIGGER`: Xóa bản ghi.
- `ACTION`:
  1. Dùng `is_deleted` (hoặc `deleted_at`) cho Customer, Opportunity, Quote, Task.
  2. Các log lịch sử, AuditLog, Interaction: KHÔNG BAO GIỜ CHO PHÉP XÓA.

**[K37] Idempotency (Sync)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 3
- `THUMB_RULE`: Chạy job đồng bộ 2 lần không được sinh ra 2 bản ghi trùng.
- `TRIGGER`: Viết code đồng bộ từ external API (KiotViet).
- `ACTION`:
  1. Dùng ID của hệ thống ngoài (VD: `kiotviet_id`) làm khóa định danh.
  2. Áp dụng cơ chế Upsert (Update nếu có, Insert nếu chưa). Không Insert mù quáng.
  3. Ghi log "Skip vì đã tồn tại và không đổi" để dễ debug.

---

## BLOCK 12: [CACHING_STRATEGY] — Chiến lược Cache

**[K38] Cache Rules**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 4
- `THUMB_RULE`: Áp dụng cache cho dữ liệu đọc nhiều, ít thay đổi.
- `TRIGGER`: Lấy data tĩnh hoặc ít đổi (Danh mục, Settings).
- `ACTION`:
  1. Sử dụng `IMemoryCache` (hoặc Redis nếu có phân tán).
  2. Cache data tĩnh với TTL (VD: 5-30 phút).
  3. Đảm bảo invalidate cache khi có lệnh Write. Tuyệt đối không cache token/session user.

**[K39] Snapshot cho Dashboard**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 4
- `THUMB_RULE`: Dashboard KHÔNG tính toán realtime từ bảng giao dịch để tránh quá tải DB.
- `TRIGGER`: Truy vấn cho màn hình Dashboard tổng quan.
- `ACTION`:
  1. Tạo Job Hangfire chạy ngầm (ví dụ: mỗi đêm) để tính toán số liệu.
  2. Lưu kết quả vào bảng Snapshot. Dashboard chỉ cần SELECT từ bảng Snapshot này.

---

## BLOCK 13: [OBSERVABILITY] — Quan sát hệ thống

**[K40] Performance Monitoring**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 3
- `THUMB_RULE`: Hệ thống phải cảnh báo khi chậm chạp.
- `TRIGGER`: Middleware / Monitor Tools.
- `ACTION`:
  1. Theo dõi % CPU, RAM, Disk.
  2. Đo thời gian phản hồi API (Response Time). Log lại mọi request chạy chậm > 2s.

**[K41] Alert Thresholds**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 3
- `THUMB_RULE`: Chỉ báo động khi đạt ngưỡng (Threshold) nghiêm trọng.
- `TRIGGER`: Devops Monitor.
- `ACTION`:
  1. CPU > 80%, RAM > 90%, Disk > 85%.
  2. Cảnh báo lỗi job: Job Hangfire fail liên tiếp > 3 lần.

---

## BLOCK 14: [UX_AND_LOCALIZATION] — Trải nghiệm người dùng

**[K42] Mobile-First Rule**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 2
- `THUMB_RULE`: UI phải hoạt động tốt cho nhân viên đi thị trường dùng điện thoại.
- `TRIGGER`: Thiết kế/Code giao diện mới.
- `ACTION`:
  1. Test giao diện trên kích thước 375px trước.
  2. Không phụ thuộc vào trạng thái `hover` (vì mobile không có chuột).
  3. Nút bấm (Touch target) phải đạt tối thiểu 44x44px.

**[K43] Vietnamese Localization**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 1
- `THUMB_RULE`: Ngôn ngữ phải tự nhiên với người Việt Nam.
- `TRIGGER`: Hiển thị text ra UI.
- `ACTION`:
  1. Dùng từ Việt ngữ chuẩn (VD: Dùng "Lưu", "Gửi" thay vì "Submit", "Save").
  2. Format tiền tệ chuẩn VN: `1.250.000 VNĐ`.
  3. Format ngày tháng: `dd/MM/yyyy`.

**[K44] Accessibility (a11y)**
- `[SEVERITY]`: 🟢 Medium
- `[SPRINT]`: 4
- `THUMB_RULE`: Giao diện phải dễ tiếp cận, dễ nhìn.
- `TRIGGER`: Styling / CSS.
- `ACTION`:
  1. Tỷ lệ tương phản chữ/nền (Contrast ratio) tối thiểu 4.5:1.
  2. Mọi thẻ hình ảnh (`<img>`) phải có thuộc tính `alt`.
  3. Đảm bảo form có thể navigate bằng bàn phím (Tab).
