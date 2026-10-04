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

**[K53] Technical Candor (Văn phong Súc tích & Định lượng)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Chỉ dùng con số và vị trí chính xác, tuyệt đối không dùng từ ngữ ước lệ, mơ hồ.
- `TRIGGER`: Giao tiếp với User, viết Comment, viết Docs, hướng dẫn.
- `ACTION`:
  1. KHÔNG dùng các từ cảm tính như "chờ một lúc", "cấu hình tương ứng", "chọn cài đặt hợp lý".
  2. THAY BẰNG từ ngữ định lượng: "đợi 10 giây", "chèn vào dòng 45 sau thẻ X", "thư mục src".
  3. BẮT BUỘC dùng định dạng trực quan: Bảng (Table) cho ma trận kiểm thử/biến môi trường, Markdown Code block có định danh ngôn ngữ, và Checklist `[ ]` cho các cổng nghiệm thu (Final Audit).

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

## BLOCK 15: [TERMINAL_AND_ENVIRONMENT] — Môi trường & Terminal

**[K46] PostgreSQL Client Setup (Thiết lập psql chuẩn)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi lệnh psql phải tắt pager và đảm bảo UTF-8.
- `TRIGGER`: Bất kỳ lệnh `psql` nào chạy trong PowerShell/CMD.
- `ACTION`:
  1. Luôn thêm cờ `-P pager=off` để tránh dừng ở `-- More --`.
  2. Set biến `PGCLIENTENCODING=UTF8` để tiếng Việt không lỗi.
  3. Không gõ password nhiều lần — set qua biến môi trường User (1 lần).
  4. Template chuẩn:
     ```powershell
     chcp 65001
     [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
     $env:PGCLIENTENCODING = "UTF8"
     psql -h localhost -U crm_user -d crm_db -P pager=off -f file.sql
     ```

**[K47] Environment Persistence (Biến môi trường vĩnh viễn)**
- `[SEVERITY]`: 🟢 Medium
- `[SPRINT]`: 1
- `THUMB_RULE`: Setup môi trường 1 lần, không gõ lại mỗi phiên.
- `TRIGGER`: Khi bắt đầu dự án trên máy mới hoặc terminal mới.
- `ACTION`:
  1. Set biến User-scope (chạy 1 lần duy nhất):
     ```powershell
     [System.Environment]::SetEnvironmentVariable('PGCLIENTENCODING', 'UTF8', 'User')
     [System.Environment]::SetEnvironmentVariable('PGPASSWORD', 'sa', 'User')
     ```
  2. Ghi lại vào `docs/setup/environment.md` để biết máy nào đã setup.
  3. Không commit password vào file — chỉ ghi tên biến.

**[K54] Automation First & Idempotency (Script Tự động & Khả lặp)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Lệnh chạy nhiều lần không gây lỗi hoặc đè hỏng dữ liệu. An toàn là số 1.
- `TRIGGER`: Khi sinh Script hoặc Command (PowerShell, Bash, SQL, EF Core, CLI).
- `ACTION`:
  1. Idempotency (Khả lặp): Luôn dùng `-Force`, `IF NOT EXISTS`, hoặc kiểm tra file/thư mục tồn tại trước khi thao tác.
  2. Path Explicit (Đường dẫn rõ ràng): Tuyệt đối tránh path tương đối mơ hồ. Dùng đường dẫn tuyệt đối hoặc neo cố định thư mục gốc (`cd <root>` ngay đầu block).
  3. Pin Dependencies (Khóa cứng phiên bản): Nêu rõ version của thư viện/package (ví dụ: `--version 8.0.3`) để tránh cài nhầm bản mới gây breaking changes.
  4. Xử lý Encoding: Luôn khai báo rõ chuẩn mã hóa (UTF-8 no BOM) để tránh lỗi ký tự trên các OS khác nhau.

---

## BLOCK 16: [EF_CORE_MAPPING] — Cấu hình quan hệ EF Core

**[K48] Bidirectional Navigation Mapping**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Khi entity có navigation 2 chiều, PHẢI chỉ định cả 2 đầu.
- `TRIGGER`: Viết `OnModelCreating`, cấu hình `HasOne` / `WithMany`.
- `ACTION`:
  1. Kiểm tra entity con có navigation sang cha không.
  2. Kiểm tra entity cha có `ICollection<Con>` không.
  3. Nếu cả 2 có → dùng `.WithMany(parent => parent.Children)`.
  4. Nếu chỉ 1 chiều → dùng `.WithMany()`.
  5. Cấm để `.WithMany()` trống khi entity đối diện có collection.

**[K49] Shadow Property Prevention**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: EF Core không được tự sinh cột ảo. Nếu có warning "shadow state" → fix ngay.
- `TRIGGER`: Khi `dotnet build` hoặc `dotnet run` xuất hiện: `The foreign key property 'X' was created in shadow state`.
- `ACTION`:
  1. Đọc tên property bị shadow (ví dụ `TeamId1`).
  2. Tìm entity chứa property gốc (`TeamId`).
  3. Kiểm tra relationship cấu hình 2 chiều đã đúng chưa (K48).
  4. Nếu đúng mà vẫn warning → thêm `.HasForeignKey()` chỉ định rõ.
  5. Test lại `dotnet run` đến khi warning biến mất hoàn toàn.

---

## BLOCK 17: [RUNBOOK_QUALITY] — Chất lượng Runbook

**[K50] Objectives & Verification Targets**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Mỗi runbook phải có 2 bảng ở đầu: Mục tiêu và Chỉ số đích.
- `TRIGGER`: Bắt đầu viết runbook mới.
- `ACTION`:
  1. **Bảng Objectives**: 3–5 mục tiêu cụ thể của phiên làm việc.
  2. **Bảng Verification Targets**: các con số cụ thể phải đạt (số bản ghi, số file, số dòng code).
  3. Cuối runbook phải có bảng **Final Audit Checklist** đối chiếu với Targets ban đầu.

**[K51] RCA for Every Bug (Root Cause Analysis bắt buộc)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mỗi lỗi gặp trong thực tế phải được ghi lại kèm nguyên nhân gốc.
- `TRIGGER`: Khi debug xong một lỗi.
- `ACTION`:
  1. Ghi lại 4 mục:
     - **Triệu chứng**: Lỗi xuất hiện thế nào.
     - **Nguyên nhân gốc**: Tại sao lỗi xảy ra.
     - **Giải pháp**: Đã sửa bằng cách nào.
     - **Xác nhận**: Làm sao biết đã sửa thành công.
  2. Đưa vào runbook tại phase tương ứng (không gộp vào Troubleshooting chung).
  3. Nếu lỗi nghiêm trọng → thêm vào BLOCK 2 (Tech Debt Report).

**[K52] Real Output Verification**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không ghi "kỳ vọng" — ghi "output thực tế đã nghiệm thu".
- `TRIGGER`: Khi viết phần Verify / Smoke Test của runbook.
- `ACTION`:
  1. Chạy lệnh thật → copy output thật vào runbook.
  2. Đặt output trong block code `text` để giữ format.
  3. Nếu output dài → cắt phần không cần thiết, giữ header và dòng cuối.
  4. Ghi rõ ngày/giờ chạy output lần cuối nếu có thể.

**[K55] Step-by-Step Flow & Time Budget (Tư duy Modular & Phân đoạn)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Làm việc theo dây chuyền một chiều, không bắt user nhảy qua nhảy lại.
- `TRIGGER`: Khi viết Runbook, hướng dẫn cài đặt, hoặc kế hoạch thực thi.
- `ACTION`:
  1. Tách quy trình thành từng Phase khép kín: Setup $\rightarrow$ Execution $\rightarrow$ Verification $\rightarrow$ Rollback.
  2. Tuân thủ Line-by-Line Execution: Người thực thi chỉ cần copy/paste hoặc chạy lệnh từ trên xuống dưới.
  3. Gán thời gian dự kiến (Time Budget) cho từng bước để người thực hiện biết tiến độ đang chậm hay đúng hạn.

**[K56] Verification & Assertions (Thiết kế Checkpoints & Output kỳ vọng)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi hành động làm thay đổi hệ thống đều phải có chốt chặn kiểm thử đi kèm.
- `TRIGGER`: Cung cấp bất kỳ câu lệnh hoặc chỉ dẫn thay đổi hệ thống/code nào.
- `ACTION`:
  1. Cung cấp lệnh Verify tiếp theo NGAY SAU lệnh thực thi (ví dụ: `Test-Path`, `Get-ChildItem`, `dotnet build`). Không bao giờ dừng lại ở câu "Chạy lệnh X".
  2. Cung cấp Output kỳ vọng cụ thể: Nêu chính xác màn hình phải in ra dòng gì (mã 200, `Build succeeded, 0 Warning 0 Error`). Kết hợp với nguyên tắc Real Output Verification [K52].

**[K57] Troubleshooting Matrix & Rollback (Quản trị rủi ro)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Luôn chuẩn bị sẵn phao cứu sinh và đường lùi trước khi hành động.
- `TRIGGER`: Cung cấp quy trình deploy, migrate, cài đặt hoặc thay đổi cấu trúc lớn.
- `ACTION`:
  1. Bảng tra cứu lỗi (Troubleshooting Matrix): Liệt kê sẵn ít nhất 5-10 lỗi phổ biến nhất. Format: Mã/Tên lỗi $\rightarrow$ Nguyên nhân gốc (Root cause) $\rightarrow$ Lệnh khắc phục (Fix action).
  2. Kế hoạch lùi (Rollback plan): Luôn có phương án hoàn tác rõ ràng (Git reset, backup DB, xóa file rác) để user xử lý khi gặp sự cố vượt quá thời gian buffer.

---

## BLOCK 18: [UI_STYLING_AND_MUDBLAZOR] — Quản trị Styling & MudBlazor

**[K58] Centralized Theming (Kiến trúc Theme tập trung)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không khai báo màu cứng (hardcode). Mọi màu sắc phải dùng Design Tokens của hệ thống.
- `TRIGGER`: Khi cần đổi màu, font, hoặc thiết lập giao diện gốc.
- `ACTION`:
  1. Tuyệt đối không khai báo `new MudTheme()` trong file `.razor`. Khai báo tập trung ở `Theme/CrmTheme.cs`.
  2. Định nghĩa đủ 2 bảng màu `PaletteLight` và `PaletteDark`.
  3. Chỉ dùng biến CSS nội sinh của MudBlazor (vd: `var(--mud-palette-primary)`). Không ghi đè màu nếu framework đã tự binding.

**[K59] Theme State Management (Quản lý trạng thái giao diện)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Tôn trọng cài đặt hệ điều hành và ghi nhớ lựa chọn của user.
- `TRIGGER`: Khởi tạo `MudThemeProvider` hoặc xử lý nút bấm chuyển đổi Dark/Light mode.
- `ACTION`:
  1. Dùng `GetSystemPreference()` và `WatchSystemPreference()` trong `OnAfterRenderAsync` để bắt theme máy tính.
  2. Lưu cấu hình Theme thủ công của user vào `localStorage` bằng JS Interop để F5 không bị mất.

**[K60] Dynamic Style Boundary (Ranh giới Inline Style & Utility)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Tuyệt đối CẤM dùng `style="..."` tĩnh. Phải dùng Utility Class.
- `TRIGGER`: Khi căn chỉnh thẻ HTML (margin, padding, flex, color...).
- `ACTION`:
  1. Chuyển đổi toàn bộ sang class MudBlazor có sẵn (vd: `style="padding: 16px"` -> `Class="pa-4"`).
  2. Không tự chỉnh chiều cao Viewport thủ công, để `<MudMainContent>` lo.
  3. Chỉ cho phép dùng thuộc tính `style="..."` khi biến đó là động ở runtime (vd: `style="width: @(progress)%"`).

**[K61] Safe Scoped CSS (Cô lập CSS)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: CSS ghi đè (override) phải có thẻ Wrapper (thẻ bọc ngoài) và dùng `::deep`.
- `TRIGGER`: Khi tạo file `[TênComponent].razor.css` để tinh chỉnh UI của MudBlazor.
- `ACTION`:
  1. Bắt buộc tạo một thẻ `<div>` gốc (Root HTML Wrapper) bọc ngoài cùng component.
  2. Dùng toán tử `::deep` trong file `.css` thì CSS mới tác động được vào lớp con của MudBlazor.

**[K62] Pre-Commit UI Audit (Kiểm toán tự động CI)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Code có dính inline style tĩnh thì không được Commit.
- `TRIGGER`: Trước khi chạy lệnh `git commit`.
- `ACTION`:
  1. Chạy đoạn script PowerShell quét thư mục `src\Crm.Web` kiểm tra đuôi `.razor`.
  2. Nếu phát hiện chuỗi `style="[^"]*"` mà không có `@(` (dấu hiệu của biến động), sẽ báo lỗi đỏ (Write-Error) chặn lưu.

### SƠ ĐỒ DÒNG CHẢY QUYẾT ĐỊNH (STYLING DECISION FLOW)

```text
Cần định dạng một thành phần giao diện?
  │
  ├─► 1. Giá trị có thay đổi liên tục theo biến C# runtime không?
  │      └─► CÓ ──► Dùng Inline Style động: style="width: @(x)%"
  │
  ├─► 2. Framework có hỗ trợ sẵn không? (Color, Elevation, pa-*, ma-*, d-flex)
  │      └─► CÓ ──► Dùng thuộc tính Props và MudBlazor Utility Class
  │
  ├─► 3. Là màu thương hiệu, font, bán kính góc dùng chung hệ thống?
  │      └─► CÓ ──► Khai báo trong Theme/CrmTheme.cs
  │
  └─► 4. Là style đặc thù không hỗ trợ (Scrollbar, Keyframe, Animation)?
         └─► CÓ ──► Dùng Root Wrapper + Scoped CSS [TênComponent].razor.css với ::deep
```

---

## BLOCK 19: [LEARNING_MODE] — Chế độ học tập

**[K63] Two-Pass Execution (Chạy 2 lượt)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không vừa chạy vừa học trong cùng 1 lượt.
- `TRIGGER`: Khi thực thi runbook có yếu tố kỹ thuật mới.
- `ACTION`:
  1. **Lượt 1 — Execution Run (30–45 phút):** Copy-paste toàn bộ, chỉ quan tâm build xanh. Mục tiêu: có Working Prototype.
  2. **Lượt 2 — Reverse-Engineering (không giới hạn):** Mở từng file, đọc từng dòng, đặt câu hỏi "tại sao".
  3. Không trộn 2 lượt. Lượt 1 xong mới sang lượt 2.

**[K64] Controlled Sabotage (Cố tình làm hỏng)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Muốn hiểu thì phải tự tay phá code.
- `TRIGGER`: Khi đã có Working Prototype từ K63.
- `ACTION`:
  1. Tạo nhánh Git riêng hoặc chắc chắn đã commit.
  2. Thử nghiệm 1: Đổi tên class/enum trùng với .NET → build xem lỗi `CS0104`.
  3. Thử nghiệm 2: Xóa attribute/wrapper → xem hành vi thay đổi thế nào.
  4. Thử nghiệm 3: Đổi connection string sai → xem exception.
  5. Sau mỗi thử nghiệm: `git restore .` để hoàn tác.
  6. Ghi lại lỗi vào file `docs/notes/sabotage-log.md`.

**[K65] Concept Mastery Metric (Đo bằng khái niệm)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Không đo tiến độ bằng "xong đúng giờ chưa". Đo bằng "nắm được khái niệm gì".
- `TRIGGER`: Cuối mỗi phiên làm việc.
- `ACTION`:
  1. Liệt kê 3–5 khái niệm quan trọng của phiên.
  2. Tự hỏi: "Nếu không nhìn runbook, mình có giải thích lại được không?"
  3. Nếu không → đánh dấu ôn lại.
  4. Ghi vào `docs/notes/concepts-mastered.md`.

**[K66] Deviation Warning (Cảnh báo lệch đường)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mỗi bước quan trọng phải có "nếu làm sai thì lỗi gì".
- `TRIGGER`: Khi soạn bất kỳ runbook nào.
- `ACTION`:
  1. Mỗi bước có khả năng fail → thêm block:
     ```
     [DEVIATION] Nếu <làm X sai> → lỗi `<error code>`.
     Nguyên nhân: <...>
     Fix: <...>
     ```
  2. Không để user tự mò mẫm khi fail.

**[K67] Dual-Layer Runbook (Runbook 2 lớp)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Tách thời gian EXEC và LEARN trong runbook.
- `TRIGGER`: Khi soạn runbook có yếu tố kỹ thuật mới.
- `ACTION`:
  1. Mỗi phase chia 2 phần:
     - **[EXEC]**: các bước chạy lệnh — không lan man.
     - **[LEARN]**: khái niệm, lý do, cách debug.
  2. Phân bổ thời gian: 70–75% EXEC, 25–30% LEARN.
  3. Cuối mỗi phase có **[CONCEPT]**: gạch đầu dòng khái niệm cần nắm.

## BLOCK 20: [AI_META_SKILLS] — Kỹ năng tự học và thích ứng của AI

**[K68] Daily Log & Context Synchronization (Đồng bộ bối cảnh)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không bao giờ code mù. Phải đọc daily log để biết lịch sử.
- `TRIGGER`: Bắt đầu phiên làm việc hoặc khởi tạo task mới.
- `ACTION`: Đọc `docs/notes/daily.md`, nắm bắt ngữ cảnh, và report lại cho User ở đầu session. Cập nhật file ở cuối session.

**[K69] Context Assimilation (Thẩm thấu bối cảnh)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Tự thân vận động. Không hỏi lại user nếu thông tin có thể grep/cat/find.
- `TRIGGER`: Khi gặp câu hỏi mở, hoặc gặp biến/class lạ.
- `ACTION`: Dùng lệnh bash rà quét toàn codebase thay vì yêu cầu user chỉ chỗ.

**[K70] Sandbox Experimentation (Kiểm chứng qua hộp cát)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không dùng suy luận lý thuyết suông để đoán lỗi (Hallucinate). Phải chạy code thật.
- `TRIGGER`: Khi user hỏi "Tại sao code này sinh lỗi/bug?".
- `ACTION`: Tạo ngay 1 file unit test / script nhỏ bằng in-memory DB để chạy và lấy console output làm bằng chứng. Bắt lỗi thực tế và đưa RCA (Root Cause Analysis).

**[K71] Clean Test Artifacts (Dọn dẹp rác hậu kiểm thử)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Không để lại dấu vết sandbox trong codebase chính.
- `TRIGGER`: Sau khi thực thi K70 hoặc chạy xong các Sandbox test scripts.
- `ACTION`:
  1. Xóa toàn bộ file test, log, txt vừa sinh ra phục vụ quá trình dò lỗi (VD: `SchemaTest.cs`, `ef_model_debug.txt`).
  2. Revert các cài đặt package tạm (như `Microsoft.EntityFrameworkCore.InMemory`) nếu không phục vụ mục đích test lâu dài của dự án.
