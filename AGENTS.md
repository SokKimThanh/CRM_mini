# AGENTS.md - HỆ ĐIỀU HÀNH CHỈ THỊ & BỘ NHỚ THỰC THI CHO AI AGENT

Đây là tài liệu chỉ thị bắt buộc (**Mandatory Deterministic Instructions**) dành cho bất kỳ AI/Agent nào tham gia lập trình và vận hành dự án.
Mọi thao tác đọc hiểu ngữ cảnh, lập kế hoạch, sinh mã nguồn (.NET 10 / Blazor / PostgreSQL), kiểm thử, thao tác Git, cập nhật tài liệu kỹ thuật và xử lý luồng nghiệp vụ B2B CRM bắt buộc phải tuân thủ nghiêm ngặt 7 Siêu Miền kiến trúc với hệ mã chuẩn duy nhất dưới đây.

---

## BẢNG MA TRẬN 7 SIÊU MIỀN KIẾN TRÚC

| Miền Chuyên Môn (Domain) | Phạm Vi Nhiệm Vụ & Trách Nhiệm | Dải Mã Kỹ Năng | Số Lượng |
| :--- | :--- | :--- | :--- |
| **DOMAIN 1: AGENT GOVERNANCE & PROTOCOLS** | Chuỗi điều phối, 10 bước tư duy kỹ sư, giao tiếp súc tích, nợ kỹ thuật, Git & Chốt chặn điểm dừng | `[K01]` – `[K11]`, `[K78]` | 12 |
| **DOMAIN 2: BACKEND & DATABASE EXCELLENCE** | CQRS Handlers, DB-First DDL, SQL Versioning, Outbox Pattern, Split Query, EF Core & Cache | `[K12]` – `[K25]` | 14 |
| **DOMAIN 3: FRONTEND & DESIGN SYSTEM** | Blazor Container/Presentation, Virtualize & Autocomplete, MudBlazor Token, 4 Trạng thái, Scoped CSS | `[K26]` – `[K35]` | 10 |
| **DOMAIN 4: SECURITY & OBSERVABILITY** | Secret, OWASP, Defense-in-Depth IDOR & Data Ownership, Serilog JSON, Correlation ID & APM | `[K36]` – `[K44]` | 9 |
| **DOMAIN 5: QA, ENVIRONMENT & DEVOPS** | Tháp kiểm thử, DoD, Pre-commit, Rollback, Idempotent Scripts, Sao lưu & Zero-downtime | `[K45]` – `[K57]` | 13 |
| **DOMAIN 6: RUNBOOK & META-LEARNING** | Phân tầng Runbook, RCA sự cố, Two-Pass, Khám phá hộp cát & Dọn rác hiện trường | `[K58]` – `[K73]` | 16 |
| **DOMAIN 7: B2B CRM BUSINESS LOGIC** | Phân định thực thể CRM, Phễu bán hàng B2B, SLA Escalation Workflow & Báo cáo Funnel | `[K74]` – `[K77]` | 4 |
| **DOMAIN 8: INTERVIEW & PORTFOLIO EXCELLENCE** | Đóng gói tư duy kiến trúc, phân tích đánh đổi (trade-off), phản biện bảo vệ thiết kế và kỹ năng thuyết trình | `[K79]` – `[K81]` | 3 |
| **TỔNG CỘNG** | **Toàn bộ hệ thống kỹ năng chuẩn hóa duy nhất** | **`[K01]` – `[K81]`** | **81** |

---

## DOMAIN 1: AGENT GOVERNANCE & PROTOCOLS (ĐIỀU PHỐI VÀ QUẢN TRỊ TÁC VỤ)

**[K01] Skill Orchestration (Liên kết Đa nhiệm theo Chuỗi Khép kín)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không thực hiện tác vụ đơn lẻ, tùy tiện. Mọi task thực thi mã nguồn phải tuân theo chuỗi hành động khép kín (Deterministic Execution Chain).
- `TRIGGER`: Nhận yêu cầu tạo tính năng (Feature), sửa lỗi (Bug fix) hoặc tái cấu trúc (Refactor) sau khi đã qua chốt chặn ngữ cảnh ([K02], [K03]).
- `ACTION`:
  1. Neo ngữ cảnh, kiểm soát trần phản biện lý thuyết ([K78]) và chạy 10 bước tư duy tiền khả thi ([K02], [K03]).
  2. Phân rã bài toán thành các phiên thực thi nhỏ $< 30$ phút ([K04]).
  3. Lập trình tuân thủ tuyệt đối quy chuẩn backend/frontend ([K12], [K14], [K18], [K26], [K33]).
  4. Nếu thao tác thực thể CRM, đối chiếu với mô hình phễu và vòng đời đối tượng ([K74] – [K77]).
  5. Viết và thực thi test xác thực ([K45], [K46], [K63]).
  6. Cập nhật tài liệu kỹ thuật liên quan theo cơ chế append-only và đồng bộ hợp đồng kép ([K09], [K15]).
  7. Ghi nhận nợ kỹ thuật nếu có mã tạm hoặc TODO phát sinh ([K07]).
  8. Thực hiện kiểm tra an toàn trước commit và commit chuẩn ([K08], [K49], [K35]).

**[K02] Context-First Verification (Chốt chặn Ngữ cảnh Ban đầu)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Tuyệt đối không suy đoán khi nhận yêu cầu chung chung. Bắt buộc neo ngữ cảnh trước khi kích hoạt chuỗi [K01].
- `TRIGGER`: Khi nhận lệnh mơ hồ (ví dụ: "Tạo bảng dữ liệu", "Viết API khách hàng", "Sửa bug").
- `ACTION`:
  1. Tự động kiểm tra cấu trúc repo (`list_files`, `README.md`, schema DDL hiện có).
  2. Xác định phạm vi thông tin bị thiếu và đưa ra câu hỏi ngắn gọn kèm 1-2 tùy chọn cụ thể để người dùng xác nhận. Tuyệt đối không tự ý sinh mã khi chưa rõ ngữ cảnh.

**[K03] System Thinking & Engineering Mindset (10 Bước Tư Duy Kỹ Sư Cốt Lõi)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: KHÔNG BAO GIỜ viết nháp code (Drafting) ngay lập tức. Bắt buộc chạy qua 10 bước màng lọc tư duy trước khi sinh mã.
- `TRIGGER`: Khi nhận yêu cầu tính năng mới, chỉnh sửa luồng nghiệp vụ hoặc thay đổi dữ liệu diện rộng.
- `ACTION`: Chạy màng lọc 10 bước:
  1. **Requirement Analysis**: Hiểu rõ mục tiêu nghiệp vụ thực tế và lý do người dùng cần tính năng này.
  2. **Domain Modeling**: Nhận diện đúng đối tượng và quan hệ thực thể ([K74]).
  3. **Business Process Analysis**: Đặt tính năng vào chuỗi hành trình nghiệp vụ (End-to-End flow).
  4. **Task Breakdown**: Phân rã bài toán thành các mốc thực thi dưới 30 phút ([K04]).
  5. **Change Impact Analysis**: Đánh giá rủi ro ảnh hưởng tới các module khác trong hệ thống.
  6. **Deep Audit**: Rà soát lỗ hổng tiềm ẩn về hiệu năng, bảo mật và toàn vẹn dữ liệu.
  7. **Decision Making & Trade-off**: Cân nhắc các phương án kỹ thuật và giải thích lý do lựa chọn phương án tối ưu.
  8. **Validation & Verification**: Xác định trước các điều kiện nghiệm thu định lượng ([K51], [K63]).
  9. **Self-Review**: Tự kiểm tra chéo (Cross-check) mã và logic trước khi trình bày; tuân thủ nghiêm ngặt điểm dừng phản biện ([K78]).
  10. **Lesson Learned**: Đúc kết và ghi nhận bài học sau khi hoàn thành.

**[K04] Auto-Prompting (Phân rã Phiên làm việc Độc lập)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi đầu việc lớn phải được chia nhỏ thành các phiên thực thi độc lập dưới 30 phút.
- `TRIGGER`: Khi bắt đầu Sprint mới hoặc khi xử lý một bản đặc tả nghiệp vụ (Spec) phức tạp.
- `ACTION`: Tự động phân rã thành các Actionable Prompts tuần tự và lưu tại `docs/sprints/sprint_<X>/prompts/`. Người dùng duyệt và kích hoạt từng prompt theo tiến độ.

**[K05] Plain Language Policy (Văn phong Súc tích, Thực dụng)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Nói ít làm nhiều. Giao tiếp tập trung vào kết quả thực thi và giải pháp; loại bỏ hoàn toàn các câu văn sáo rỗng hoặc lý thuyết hàn lâm.
- `TRIGGER`: Khi trả lời tin nhắn chat, viết comment giải thích mã, viết README hoặc hướng dẫn sử dụng.
- `ACTION`:
  1. Trong hội thoại: Báo cáo kết quả trực diện (vd: "Đã thêm API lấy danh sách Lead có phân trang" thay vì trình bày dài dòng về lý thuyết CQRS).
  2. Trong tài liệu: Dùng câu chủ động, ngắn gọn, đi thẳng vào các bước thực hành (Actionable steps).
  3. Khi cần làm rõ: Đưa thẳng 1-2 phương án lựa chọn cụ thể, không hỏi lan man.

**[K06] Technical Candor (Định lượng & Định danh Chính xác)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Sử dụng dữ liệu định lượng, tên hàm, đường dẫn tuyệt đối; cấm sử dụng từ ngữ ước lệ, cảm tính.
- `TRIGGER`: Viết tài liệu runbook, comment giải thích mã nguồn, báo cáo lỗi.
- `ACTION`:
  1. Cấm dùng từ ngữ mơ hồ: "chờ một lát", "cấu hình hợp lý", "ở thư mục tương ứng".
  2. Dùng từ định lượng: "chờ 15 giây", "tại dòng 112 của file `src/Crm.Web/App.razor`", "chạy lệnh với timeout 30s".
  3. Dùng bảng Markdown cho ma trận kiểm thử/biến môi trường; dùng checklist `[ ]` cho các bước nghiệm thu.

**[K07] Tech Debt Management (Quản trị Nợ Kỹ thuật)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi đoạn code tạm thời, logic hardcode hoặc TODO đều phải được minh bạch hóa vào sổ nợ kỹ thuật.
- `TRIGGER`: Kết thúc mỗi task hoặc khi buộc phải viết code chưa tối ưu để giải quyết sự cố khẩn cấp.
- `ACTION`: Tự động ghi nhận mục nợ mới vào `docs/sprints/sprint_<X>/tech_debt_report.md` gồm: Vị trí code, Lý do tạm thời, Giải pháp lâu dài, Thời hạn xử lý.

**[K08] Git Workflow & Auto-Branching (Quản lý Phân nhánh)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Không commit trực tiếp vào nhánh chính (`main`/`develop`). Tuân thủ phân nhánh và chuẩn Conventional Commits.
- `TRIGGER`: Khi bắt đầu thực hiện mã nguồn cho một tính năng hoặc bản vá lỗi mới.
- `ACTION`:
  1. Kiểm tra trạng thái kho mã nguồn: `git status`, `git branch`.
  2. Chuyển sang nhánh tính năng theo định dạng: `feature/<tên-tính-năng>` hoặc `fix/<tên-lỗi>`.
  3. Đặt thông điệp commit theo chuẩn: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`.

**[K09] Append-Only, Compaction & ADR Continuity (Ghi Nối tiếp & Hồ Sơ Quyết Định)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Cấm xóa trắng hoặc ghi đè làm mất dấu vết lịch sử; áp dụng chu kỳ cô đọng (Compaction) khi kết thúc Sprint và chuẩn hóa ghi nhận quyết định kiến trúc (ADR).
- `TRIGGER`: Cập nhật tài liệu SRS, Runbook, Data Dictionary, API Specs hoặc khi thông qua quyết định kiến trúc mới.
- `ACTION`:
  1. **Pha trong Sprint (Append-Only)**: Giữ nguyên lịch sử, thêm nội dung mới vào cuối mục lục tương ứng, dẫn link ngược về tài liệu gốc.
  2. **Pha đóng Sprint (Document Compaction Protocol)**: Tổng hợp các đoạn ghi chép bổ sung thành bản quy chuẩn tinh gọn cho Sprint mới; chuyển bản cũ sang `docs/archive/` theo chuẩn [K10].
  3. **Hồ sơ Quyết định Kiến trúc (ADR)**: Mọi quyết định thay đổi nền tảng (chuyển đổi pattern, chọn thư viện, đổi cấu trúc dữ liệu) bắt buộc tạo 1 bản ghi tại `docs/adr/ADR-xxxx_<ten_quyet_dinh>.md` ($< 50$ dòng) theo mẫu: `[Bối cảnh] -> [Quyết định] -> [Hệ quả kỹ thuật]`.

**[K10] Document Archiving (Cô lập & Lưu trữ Tài liệu Cũ)**
- `[SEVERITY]`: 🟢 Medium
- `[SPRINT]`: All
- `THUMB_RULE`: Tài liệu không còn khớp với kiến trúc thực tế phải được cô lập, không để lẫn với tài liệu đang hoạt động.
- `TRIGGER`: Khi thay đổi lớn về kiến trúc hoặc thay thế hoàn toàn một tài liệu hướng dẫn.
- `ACTION`: Thêm thẻ `[DEPRECATED: YYYY-MM-DD - Thay thế bởi path/to/new_file.md]` vào đầu file cũ, sau đó chuyển file vào thư mục `docs/archive/`.

**[K11] Auto-README Refactoring & System Manifest (Bản Đồ Kiến Trúc Tối Giản)**
- `[SEVERITY]`: 🟢 Medium
- `[SPRINT]`: All
- `THUMB_RULE`: File `README.md` tại thư mục gốc không vượt quá 100 dòng nhưng bắt buộc đóng vai trò Bản đồ Hệ thống (System Manifest) đủ thông tin định vị.
- `TRIGGER`: Khi chỉnh sửa hoặc cập nhật `README.md`.
- `ACTION`: Cấu trúc 100 dòng của `README.md` bắt buộc chứa đủ 3 phần:
  1. **Tech Stack & Runtime Manifest**: Phiên bản chuẩn (.NET 10, Blazor Server/WASM, PostgreSQL, Hangfire).
  2. **Cây Thư Mục Cấp 1 & Vai Trò**: Mỗi thư mục gốc đi kèm 1 dòng mô tả ngắn gọn trách nhiệm nghiệp vụ.
  3. **Mục Lục Chỉ Mục Nóng (Hot-Links)**: Trỏ trực tiếp đến 3 tài liệu sống: `docs/specs/data_dictionary.md`, `docs/specs/api_contracts.md`, và Sprint Runbook hiện hành.

**[K78] Finite Deliberation & Exit Criteria Enforcement (Chốt Chặn Điểm Dừng & Thoát Vòng Lặp Phản Biện)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Triệt tiêu hội chứng tê liệt phân tích (Analysis Paralysis). Mọi phiên đánh giá lý thuyết chỉ được tối đa 2 vòng phản biện ($N \le 2$). Thoát ngay sang thực thi khi không còn lỗi kiến trúc; cấm dùng kỹ năng này để bỏ qua cổng nghiệm thu DoD.
- `TRIGGER`: Khi phiên làm việc rơi vào một trong các trạng thái:
  1. Quá trình tự đánh giá/phản biện lý thuyết chạm ngưỡng vòng thứ 2 ($N_{\text{review\_pass}} \ge 2$).
  2. Hai lượt trao đổi không làm thay đổi kết luận kỹ thuật ở mức độ 🔴 Critical hoặc 🟡 High.
  3. Các nhận xét còn lại chỉ thuần túy là sở thích hành văn hoặc phong cách thẩm mỹ không ảnh hưởng đến tính toàn vẹn của hệ thống.
- `ACTION`:
  1. **Khóa Luận Đàm (Freeze Deliberation)**: Ngừng việc đào sâu giả thuyết. Tóm tắt kết luận cuối cùng trong $\le 3$ gạch đầu dòng ngắn gọn.
  2. **Trọng Tài Con Người (Human Fallback)**: Nếu chạm trần 2 vòng phản biện mà vẫn còn 2 phương án kỹ thuật mâu thuẫn, xuất bảng đối chiếu ưu/nhược điểm trong tối đa 5 dòng và nhường quyền quyết định cho người dùng.
  3. **Cưỡng Bức Thực Thi (Forced Execution)**: Chuyển ngay sang viết mã, chạy script CLI thật ([K61]) hoặc kiểm thử hộp cát ([K72]) để lấy dữ liệu thực nghiệm.
  4. **Bảo Toàn Cổng Chất Lượng**: Tuyệt đối không viện dẫn Exit Criteria để bỏ qua các tiêu chuẩn kiểm thử [K45], [K48], [K63]. Mọi mã sinh ra vẫn phải build xanh và vượt qua toàn bộ pre-commit checklist.

---

## DOMAIN 2: BACKEND & DATABASE EXCELLENCE (KIẾN TRÚC BACKEND & CƠ SỞ DỮ LIỆU)

**[K12] Clean Code & CQRS Strict Boundaries (.NET 10 Convention)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Triệt để tuân thủ SOLID; cấm hardcode giá trị logic; phân định rạch ròi trách nhiệm giữa Repository, Handler và Controller/Page.
- `TRIGGER`: Khi sinh mã C# ở các tầng Domain, Application, Infrastructure, Web.
- `ACTION`:
  1. **CQRS Boundaries**: Repositories CHỈ dùng cho các thao tác CRUD cơ bản và nạp dữ liệu. Toàn bộ Business Logic (tính toán chiết khấu, kiểm tra phân quyền, chuyển đổi trạng thái) BẮT BUỘC nằm trong MediatR Handlers.
  2. Controller hoặc Blazor Page chỉ đóng vai trò Dispatcher: nhận request và gửi Command/Query qua Mediator, không trực tiếp xử lý logic.
  3. Bắt buộc Dependency Injection qua Constructor; cấm Service Locator tĩnh.
  4. Sử dụng `async/await` với `CancellationToken` cho toàn bộ tác vụ I/O và truy vấn cơ sở dữ liệu.

**[K13] Resilient API Integration (Tích hợp Ngoại vi Chịu lỗi)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 4
- `THUMB_RULE`: Giao tiếp API bên ngoài (KiotViet, Cổng thanh toán, Zalo ZNS) phải có khả năng chịu lỗi và tự phục hồi.
- `TRIGGER`: Khi viết Client tích hợp API bên ngoài.
- `ACTION`: Quản lý kết nối qua `IHttpClientFactory`, áp dụng chính sách Polly: Retry (với Exponential Backoff), Circuit Breaker và Timeout.

**[K14] DB-First & SQL Version Control (Quản lý Phiên bản DDL Khả lặp)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Cấu trúc cơ sở dữ liệu do tập tin SQL DDL làm chủ; tuyệt đối không dùng EF Migrations (`Add-Migration`). Mọi file script SQL phải có số phiên bản tuần tự và đảm bảo tính khả lặp (Idempotency).
- `TRIGGER`: Khi có yêu cầu thay đổi schema CSDL (tạo bảng, sửa trường, thêm index).
- `ACTION`:
  1. Đặt tên file script theo chuẩn phiên bản: `V<Major>.<Minor>.<Patch>__<Mo_ta_ngan_gon>.sql` (Ví dụ: `V1.0.1__Create_Lead_Table.sql`).
  2. Mỗi script phải bắt đầu bằng lệnh kiểm tra bảng lịch sử:
     ```sql
     CREATE TABLE IF NOT EXISTS sys_schema_history (
         installed_rank SERIAL PRIMARY KEY,
         version VARCHAR(50) NOT NULL UNIQUE,
         description VARCHAR(200) NOT NULL,
         installed_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
     );
     ```
  3. Mọi câu lệnh DDL bên trong phải có mệnh đề an toàn `IF NOT EXISTS` hoặc `IF EXISTS` ([K53]).
  4. Thực thi script SQL qua công cụ psql ([K25]), sau đó chạy EF Core Scaffold (`dotnet ef dbcontext scaffold`) để cập nhật lại Entity.

**[K15] Schema & API Contract Verification (Đồng bộ CSDL, API & Đặc Tả)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không để lệch pha tài liệu ở cả hai tầng: Schema CSDL vật lý và Hợp đồng giao tiếp API/DTO.
- `TRIGGER`: Khi thêm mới/sửa bảng, cột CSDL, hoặc thay đổi trường trong các MediatR Command/Query DTOs.
- `ACTION`:
  1. **Tầng Database**: Đối chiếu $100\%$ giữa file DDL SQL thực thi và Data Dictionary (`docs/specs/data_dictionary.md`).
  2. **Tầng API Contract**: Đối chiếu giữa các lớp DTO Request/Response trong C# và tài liệu đặc tả giao tiếp (`docs/specs/api_contracts.md` hoặc Swagger/OpenAPI spec).
  3. Cập nhật đồng thời tài liệu đặc tả trước khi chốt commit tính năng, không tách rời sang commit sau.

**[K16] Bidirectional Navigation Mapping (Cấu hình Điều hướng 2 Chiều)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Khi quan hệ giữa hai bảng được khai báo điều hướng ở cả hai Entity, bắt buộc phải cấu hình rõ ràng cả hai đầu.
- `TRIGGER`: Viết `OnModelCreating`, cấu hình quan hệ bằng Fluent API trong EF Core.
- `ACTION`:
  1. Nếu Entity Cha chứa `ICollection<Con>` và Entity Con chứa thuộc tính trỏ về `Cha`: Bắt buộc cấu hình `.HasOne(c => c.Parent).WithMany(p => p.Children).HasForeignKey(c => c.ParentId)`.
  2. Tuyệt đối không để hàm `.WithMany()` trống khi phía đối diện đã khai báo Collection.

**[K17] Shadow Property Prevention (Ngăn chặn Thuộc tính Ảo EF Core)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không cho phép EF Core tự sinh các cột ảo (Shadow properties) ngoài tầm kiểm soát. Xóa sạch cảnh báo quan hệ ngầm.
- `TRIGGER`: Khi chạy ứng dụng xuất hiện cảnh báo: `The foreign key property 'X' was created in shadow state`.
- `ACTION`:
  1. Xác định property bị shadow (thường có hậu tố số như `TeamId1`).
  2. Tìm entity liên quan và rà soát lại cấu hình điều hướng ([K16]).
  3. Chỉ định rõ ràng khóa ngoại qua `.HasForeignKey()` để EF Core ánh xạ đúng thuộc tính đã tồn tại.

**[K18] Database Query Optimization & Split Query (Chống Bùng Nổ Tích Descartes)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Tuyệt đối không để xảy ra lỗi N+1 Query và hiện tượng bùng nổ dữ liệu (Cartesian Explosion) làm tràn RAM.
- `TRIGGER`: Khi viết logic truy vấn dữ liệu từ PostgreSQL qua EF Core.
- `ACTION`:
  1. **Split Query Bắt Buộc**: Bất kỳ truy vấn đọc nào có từ 2 mệnh đề `Include()` trở lên (đặc biệt là include các Collection) BẮT BUỘC phải nối thêm `.AsSplitQuery()` để tách nhỏ câu lệnh SQL, tránh nhân bản bản ghi bộ nhớ.
  2. Danh sách hiển thị bắt buộc dùng `IQueryable` kết hợp `.Skip().Take()` ở tầng server.
  3. Truy vấn chỉ đọc (read-only) bắt buộc thêm `.AsNoTracking()`.
  4. Chiếu dữ liệu trực tiếp sang DTO thông qua `.Select()` thay vì nạp toàn bộ Entity lớn về bộ nhớ.

**[K19] Concurrency Control (Xử lý Xung đột Ghi Đồng thời)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 2
- `THUMB_RULE`: Ngăn chặn tình trạng hai phiên làm việc ghi đè dữ liệu lên nhau một cách âm thầm (Lost Update).
- `TRIGGER`: Viết logic cập nhật các bảng dữ liệu nghiệp vụ quan trọng (Đơn hàng, Khách hàng, Báo giá).
- `ACTION`:
  1. Sử dụng cột `xmin` (PostgreSQL) hoặc `row_version` làm Concurrency Token trong Entity Framework.
  2. Bắt ngoại lệ `DbUpdateConcurrencyException`, trả về thông báo rõ ràng cho người dùng: "Dữ liệu đã bị người khác thay đổi, vui lòng làm mới trang và thử lại".

**[K20] Timezone Standardization (UTC Lưu trữ, Địa phương Hiển thị)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Toàn bộ thời gian lưu trữ trong hệ thống phải ở chuẩn UTC; chỉ chuyển đổi sang giờ địa phương tại lớp giao diện.
- `TRIGGER`: Khi thiết kế schema CSDL, viết logic xử lý ngày tháng hoặc hiển thị lên giao diện.
- `ACTION`:
  1. CSDL PostgreSQL: Sử dụng kiểu dữ liệu `TIMESTAMPTZ`.
  2. Backend C#: Chỉ dùng `DateTime.UtcNow`. Cấm tuyệt đối sử dụng `DateTime.Now`.
  3. Giao diện Blazor: Chuyển đổi sang múi giờ người dùng (GMT+7) khi hiển thị ra màn hình.

**[K21] Soft Delete & Immutable Audit History (Chính sách Xóa mềm & Vết Kiểm toán)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 2
- `THUMB_RULE`: Các thực thể kinh doanh cốt lõi không được xóa cứng; dữ liệu lịch sử kiểm toán cấm mọi hành vi chỉnh sửa/xóa.
- `TRIGGER`: Thực hiện thao tác xóa bản ghi dữ liệu.
- `ACTION`:
  1. Dùng cờ `is_deleted` (kèm `deleted_at`, `deleted_by`) cho Lead, Customer, Opportunity, Quote, Task.
  2. Thiết lập Global Query Filter trong EF Core để tự động bỏ qua các bản ghi đã xóa mềm.
  3. Toàn bộ các bảng Audit Log, Transaction Log, Interaction History, Status History ([K76]): Cấm tuyệt đối mọi thao tác UPDATE hoặc DELETE (Immutable Append-Only).

**[K22] Transactional Outbox & Background Processing (Chống Lỗi Dual-Write)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 3
- `THUMB_RULE`: Tuyệt đối không gọi trực tiếp tác vụ ngoại vi trong database transaction. Tách biệt rõ giữa Hangfire Queue đơn thuần và Transactional Outbox Pattern để tránh lỗi dual-write khi transaction bị rollback.
- `TRIGGER`: Gửi email, thông báo đẩy, đồng bộ dữ liệu KiotViet hoặc kích hoạt Webhook khi có sự kiện thay đổi dữ liệu nghiệp vụ.
- `ACTION`:
  1. **Transactional Outbox**: Với các sự kiện cốt lõi (Tạo Lead, Đóng đơn hàng, Xuất hóa đơn), Handler BẮT BUỘC lưu bản ghi sự kiện vào bảng `outbox_messages` trong CÙNG MỘT DbTransaction với dữ liệu nghiệp vụ.
  2. Một Hangfire Worker độc lập định kỳ quét bảng `outbox_messages`, gửi thông điệp tới hệ thống ngoài, và đánh dấu `processed_at` khi thành công.
  3. Với các tác vụ nền không yêu cầu tính toàn vẹn giao dịch (vd: nén ảnh, xuất file excel tạm): Được phép gọi trực tiếp `BackgroundJob.Enqueue(() => ...)`.
  4. Mọi Worker xử lý bắt buộc triển khai tính khả lặp (Idempotency) dựa trên `event_id` hoặc mã định danh duy nhất của hệ thống ngoài.

**[K23] Cache Invalidation & TTL (Chiến lược Bộ đệm Phân tầng)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 4
- `THUMB_RULE`: Chỉ lưu cache dữ liệu đọc nhiều, ít thay đổi. Bắt buộc có cơ chế xóa cache khi có thao tác ghi.
- `TRIGGER`: Lấy dữ liệu cấu hình hệ thống, danh mục sản phẩm, bảng giá chung.
- `ACTION`:
  1. Áp dụng `IMemoryCache` (hoặc Redis cho môi trường phân tán).
  2. Thiết lập thời gian sống (TTL) cụ thể (5-30 phút) cho từng loại dữ liệu.
  3. Xóa cache (Eviction) ngay khi có thao tác tạo mới/sửa/xóa trên dữ liệu đó. Cấm cache thông tin phiên (session/token).

**[K24] Background Snapshot for Dashboards (Bảng Tổng hợp Ngầm Hangfire)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 4
- `THUMB_RULE`: Dashboard báo cáo phân tích không tính toán dữ liệu thô trực tiếp trên bảng nghiệp vụ khi tải trang.
- `TRIGGER`: Xây dựng màn hình Dashboard tổng quan cho cấp quản trị.
- `ACTION`:
  1. Xây dựng Hangfire Background Job chạy định kỳ (vd: mỗi 15 phút hoặc hàng đêm) tính toán trước số liệu.
  2. Lưu kết quả tổng hợp vào các bảng Snapshot chuyên dụng (vd: `crm_funnel_snapshots`).
  3. Giao diện Dashboard chỉ truy vấn trực tiếp từ bảng Snapshot này.

**[K25] PostgreSQL Client Standard (Thiết lập CLI psql UTF-8)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi lệnh thực thi `psql` trên console phải tắt chế độ ngắt trang và bảo toàn mã hóa UTF-8.
- `TRIGGER`: Khi thực thi lệnh `psql` trong terminal (PowerShell/CMD).
- `ACTION`:
  1. Bắt buộc gắn cờ `-P pager=off`.
  2. Cấu hình biến môi trường terminal sang UTF-8.
  3. Mẫu lệnh chuẩn trên PowerShell:
     ```powershell
     chcp 65001
     [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
     $env:PGCLIENTENCODING = "UTF8"
     psql -h localhost -U crm_user -d crm_db -P pager=off -f path/to/script.sql
     ```

---

## DOMAIN 3: FRONTEND & DESIGN SYSTEM (GIAO DIỆN BLAZOR VÀ THIẾT KẾ HỆ THỐNG)

**[K26] Blazor Architecture & DOM Optimization (Smart/Dumb UI & Virtualization)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Phân tách rạch ròi Smart UI (Container) và Dumb UI (Presentation); triệt để tối ưu DOM, cấm nạp toàn bộ danh sách lớn vào trình duyệt.
- `TRIGGER`: Khi xây dựng component giao diện người dùng (.razor).
- `ACTION`:
  1. Chỉ giao tiếp thông qua MediatR Handler/Service; tuyệt đối không inject `DbContext` vào `.razor`.
  2. **Tối Ưu Hóa DOM Bắt Buộc**: Tuyệt đối không bao giờ render `<select>` hoặc `<MudSelect>` cho danh sách có trên 100 bản ghi. Bắt buộc phải sử dụng `<MudAutocomplete>` (tìm kiếm server-side) hoặc thẻ `<Virtualize>` để tránh làm đơ DOM trình duyệt.
  3. Bọc toàn bộ các vùng hiển thị động bằng `<ErrorBoundary>`.
  4. Khi sử dụng JavaScript Interop, bắt buộc hiện thực `IAsyncDisposable` để giải phóng bộ nhớ.

**[K27] Four UI States Compliance (Tuân thủ 4 Trạng thái Giao diện)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 2
- `THUMB_RULE`: Mọi màn hình danh sách, chi tiết hoặc biểu đồ bắt buộc phải thiết kế đủ 4 trạng thái giao diện.
- `TRIGGER`: Khi phát triển component hiển thị dữ liệu người dùng.
- `ACTION`:
  1. **Loading**: Hiển thị Skeleton hoặc Spinner.
  2. **Empty**: Thông báo "Chưa có dữ liệu" kèm nút hành động khởi tạo.
  3. **Error**: Thông báo "Đã xảy ra sự cố tải dữ liệu" kèm nút thử lại (Retry).
  4. **Success**: Hiển thị dữ liệu hoàn chỉnh.

**[K28] Mobile-First Optimization (Tối ưu Hiện trường & Chạm Mobile)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 2
- `THUMB_RULE`: Toàn bộ các thao tác nghiệp vụ ngoài hiện trường phải thao tác mượt mà trên thiết bị di động.
- `TRIGGER`: Thiết kế hoặc lập trình giao diện component Blazor.
- `ACTION`:
  1. Kiểm thử hiển thị trên màn hình có chiều rộng 375px trước tiên.
  2. Không phụ thuộc vào sự kiện hover của chuột máy tính.
  3. Kích thước các nút bấm và vùng chạm (Touch targets) tối thiểu phải đạt 44x44px.

**[K29] Vietnamese Localization Standards (Chuẩn Việt hóa & Định dạng)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 1
- `THUMB_RULE`: Ngôn ngữ hiển thị trên giao diện phải tự nhiên, chính xác theo chuẩn văn phong doanh nghiệp Việt Nam.
- `TRIGGER`: Hiển thị văn bản, ngày tháng, tiền tệ trên giao diện người dùng.
- `ACTION`:
  1. Sử dụng thuật ngữ tiếng Việt chuẩn (Dùng "Lưu", "Hủy", "Cập nhật", "Xác nhận").
  2. Định dạng tiền tệ: `#.##0 ₫` hoặc `#.##0 VNĐ` (vd: `1.500.000 VNĐ`).
  3. Định dạng ngày tháng: `dd/MM/yyyy` hoặc `dd/MM/yyyy HH:mm`.

**[K30] Accessibility Compliance (a11y - Tương phản & Phím Điều hướng)**
- `[SEVERITY]`: 🟢 Medium
- `[SPRINT]`: 4
- `THUMB_RULE`: Đảm bảo giao diện người dùng dễ nhìn, độ tương phản tốt và hỗ trợ điều hướng cơ bản.
- `TRIGGER`: Tinh chỉnh CSS, xây dựng form và cấu trúc thẻ HTML.
- `ACTION`:
  1. Độ tương phản giữa chữ và nền (Contrast Ratio) đạt tối thiểu 4.5:1.
  2. Toàn bộ hình ảnh phải có thuộc tính `alt` mô tả nội dung.
  3. Hỗ trợ đầy đủ việc sử dụng phím Tab để di chuyển qua các trường trong biểu mẫu.

**[K31] Centralized Theming (Quản lý Token Giao diện Tập trung)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Tuyệt đối không hardcode mã màu sắc trên giao diện. Quản lý toàn bộ màu sắc qua hệ thống Token tập trung.
- `TRIGGER`: Khi thiết kế mới, tinh chỉnh màu sắc hoặc font chữ.
- `ACTION`:
  1. Định nghĩa tập trung trong `Theme/CrmTheme.cs`.
  2. Cấu hình đầy đủ cả hai bảng màu: `PaletteLight` và `PaletteDark`.
  3. Sử dụng các biến CSS nội sinh của framework (ví dụ: `var(--mud-palette-primary)`).

**[K32] Theme State Management (Đồng bộ & Lưu giữ Dark/Light Mode)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Tôn trọng thiết lập giao diện của hệ điều hành và lưu giữ lựa chọn của người dùng khi tải lại trang.
- `TRIGGER`: Khởi tạo `MudThemeProvider` hoặc nút bấm chuyển đổi Dark/Light mode.
- `ACTION`:
  1. Đọc và đồng bộ cấu hình qua `GetSystemPreference()` và `WatchSystemPreference()`.
  2. Lưu lựa chọn Theme của người dùng vào `localStorage` thông qua JS Interop để giữ trạng thái sau khi refresh.

**[K33] Dynamic Style Boundary (Ranh giới Utility Class & Inline Động)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Cấm dùng `style="..."` cho các giá trị tĩnh. Bắt buộc dùng Utility Class của MudBlazor; chỉ dùng inline style cho giá trị động.
- `TRIGGER`: Khi căn chỉnh layout, khoảng cách, kích thước trên thẻ Razor.
- `ACTION`:
  1. Tĩnh: Chuyển đổi hoàn toàn sang class có sẵn của MudBlazor (vd: `Class="pa-4 d-flex justify-space-between"` thay cho `style="padding: 16px; display: flex; ..."`).
  2. Động: Chỉ sử dụng `style` khi giá trị phụ thuộc vào biến C# runtime (vd: `style="@($"width: {progress}%;")"`).

**[K34] Safe Scoped CSS with Wrapper (Bọc ngoài An toàn & Toán tử ::deep)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Khi can thiệp sâu vào CSS của MudBlazor, bắt buộc phải dùng thẻ Wrapper bọc ngoài và toán tử `::deep`.
- `TRIGGER`: Viết file Scoped CSS `[Component].razor.css`.
- `ACTION`:
  1. Tạo thẻ bọc ngoài cùng (vd: `<div class="crm-module-wrapper">`).
  2. Viết CSS chỉ tác động trong phạm vi wrapper qua toán tử: `.crm-module-wrapper ::deep .mud-table-head { ... }`.

**[K35] Pre-Commit UI Audit (Kiểm toán Giao diện Tự động Trước Commit)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Chặn commit nếu phát hiện có khai báo inline style tĩnh vi phạm quy chuẩn [K33].
- `TRIGGER`: Khi chạy kiểm tra trước khi commit mã nguồn ([K49]).
- `ACTION`: Chạy script tự động quét các file `.razor`. Nếu phát hiện thuộc tính `style="` chứa chuỗi ký tự cứng mà không có dấu hiệu liên kết động C# (`@`), lập tức báo lỗi và dừng commit.

---

## DOMAIN 4: SECURITY & OBSERVABILITY (BẢO MẬT VÀ NĂNG LỰC QUAN SÁT)

**[K36] Secret Management (Bảo mật Khóa & Biến Môi trường)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Tuyệt đối không lưu trữ Connection String, API Key, Token hoặc Password trong source code hoặc file cấu hình commit lên Git.
- `TRIGGER`: Khi viết code kết nối CSDL, gọi API hoặc cấu hình ứng dụng.
- `ACTION`:
  1. Môi trường Dev: Sử dụng `dotnet user-secrets`.
  2. Môi trường Staging/Prod: Sử dụng biến môi trường (Environment Variables) hoặc KeyVault.
  3. Trong Git: Chỉ đẩy `appsettings.json` chứa các giá trị placeholder dạng `#{DB_CONNECTION_STRING}#`.
  4. Quét regex kiểm tra trước commit để đảm bảo không lọt chuỗi nhạy cảm.

**[K37] Input Validation & OWASP Enforcement (Kiểm thực Server-side)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Không tin tưởng bất kỳ dữ liệu đầu vào nào từ Client; bắt buộc kiểm thực dữ liệu tại Server.
- `TRIGGER`: Khi xử lý Form submission, nhận API request, query string hoặc file upload.
- `ACTION`:
  1. Sử dụng FluentValidation tích hợp vào pipeline xử lý nghiệp vụ cho các rule phức tạp; dùng DataAnnotations cho rule cơ bản.
  2. Kiểm tra phòng chống 10 lỗ hổng bảo mật hàng đầu của OWASP (SQL Injection, XSS, CSRF).
  3. Với file upload: Kiểm tra MIME type thực tế, giới hạn kích thước và đổi tên file bằng GUID ngẫu nhiên khi lưu trữ.

**[K38] Password & Session Policy (Chính sách Mật khẩu & Phiên)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Không tự phát minh cơ chế mã hóa mật khẩu hoặc quản lý phiên đăng nhập mới.
- `TRIGGER`: Khi viết logic xác thực, đăng ký, đăng nhập và quản lý token/cookie.
- `ACTION`:
  1. Băm mật khẩu bằng thuật toán chuẩn (PBKDF2 thông qua ASP.NET Core Identity hoặc BCrypt).
  2. Mật khẩu tối thiểu 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.
  3. Cơ chế Lockout: Tạm khóa tài khoản 15 phút sau 5 lần đăng nhập thất bại liên tiếp.
  4. Cấu hình Cookie an toàn: `HttpOnly = true`, `Secure = true`, `SameSite = SameSiteMode.Lax`.
  5. Thời gian hết hạn phiên làm việc mặc định 8 giờ, áp dụng Sliding Expiration.

**[K39] Defense-in-Depth IDOR & Multi-Tenancy RBAC (Phòng Thủ 2 Lớp Chống IDOR & Phân Quyền)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Không bao giờ tin tưởng ID gửi từ Client. Triển khai mô hình phòng thủ 2 lớp chặt chẽ, kiểm soát dữ liệu đệ quy theo cây phòng ban (Team Hierarchy).
- `TRIGGER`: Viết logic truy xuất, thay đổi dữ liệu (`GetById(id)`, `Update(id)`, `Delete(id)`) hoặc thiết kế Entity nghiệp vụ.
- `ACTION`:
  1. **Quy chuẩn Thực thể**: Mọi thực thể nghiệp vụ cốt lõi (Lead, Customer, Opportunity, Quote) PHẢI có trường `OwnerId` (Người phụ trách) và `TeamId` (Đội ngũ).
  2. **Lớp 1 (Mặc định Hạ tầng)**: Sử dụng EF Core Global Query Filter tự động gắn điều kiện theo `TenantId` cho $100\%$ các truy vấn dữ liệu.
  3. **Lớp 2 (Nghiệp vụ Chống IDOR tại Handlers)**: Handlers BẮT BUỘC kiểm tra quyền sở hữu trước khi trả về hoặc cập nhật dữ liệu:
     - Sales chỉ được thao tác trên bản ghi do mình là `OwnerId` hoặc được chia sẻ ủy quyền.
     - Manager được quyền thao tác trên các bản ghi thuộc `TeamId` do mình phụ trách (phân quyền đệ quy theo cây phòng ban).
     - Admin hệ thống mới có quyền truy cập toàn cục.

**[K40] Structured Logging (Ghi Log Cấu trúc JSON Serilog)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Tuyệt đối không dùng `Console.WriteLine`. Mọi log phải là JSON có cấu trúc.
- `TRIGGER`: Khi ghi nhận thông tin hệ thống, cảnh báo hoặc bắt ngoại lệ.
- `ACTION`:
  1. Sử dụng Serilog để ghi log có cấu trúc (gồm Timestamp, LogLevel, MessageTemplate, ContextProperties).
  2. Tuyệt đối không ghi thông tin nhạy cảm vào log (mật khẩu, chuỗi kết nối, CCCD, thông tin thanh toán).
  3. Phân định đúng cấp độ: Debug (phát triển), Information (mốc nghiệp vụ), Warning (bất thường nhẹ), Error (lỗi xử lý nghiệp vụ), Fatal (sập hệ thống).

**[K41] Global Exception Handling (Bắt Ngoại lệ Toàn cục)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Không để lộ vết ngăn xếp lỗi (Stack Trace) ra ngoài giao diện người dùng.
- `TRIGGER`: Xử lý HTTP Request trong Middleware hoặc vòng đời Blazor Component.
- `ACTION`:
  1. Tầng API: Sử dụng Middleware bắt lỗi toàn cục, trả về định dạng chuẩn RFC 7807 (Problem Details).
  2. Tầng UI: Dùng `<ErrorBoundary>` để hiển thị thông báo thân thiện và ngăn chặn ngắt kết nối Circuit Blazor.

**[K42] Distributed Tracing & Correlation ID (Mã Truy vết Xuyên suốt)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 2
- `THUMB_RULE`: Mọi thao tác từ client qua server và database phải mang theo một mã định danh duy nhất (Correlation ID).
- `TRIGGER`: Tiếp nhận request tại cổng API hoặc bắt đầu một tác vụ người dùng trên giao diện.
- `ACTION`:
  1. Sinh `CorrelationId` (GUID) ở đầu pipeline nếu client chưa cung cấp.
  2. Bổ sung `CorrelationId` vào log context của toàn bộ các thao tác xử lý liên quan.
  3. Trả `CorrelationId` về response header (`X-Correlation-ID`) để phục vụ đối soát khi xảy ra sự cố.

**[K43] Performance Monitoring (Theo dõi Tài nguyên & Cảnh báo Chậm)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 3
- `THUMB_RULE`: Hệ thống phải tự động phát hiện và cảnh báo các nút thắt hiệu năng.
- `TRIGGER`: Xây dựng Middleware hoặc tích hợp công cụ APM.
- `ACTION`:
  1. Theo dõi các chỉ số tài nguyên: CPU, RAM, Disk I/O.
  2. Tự động ghi log Warning đối với bất kỳ tác vụ hoặc truy vấn API nào có thời gian thực thi (Response Time) vượt quá 2000ms.

**[K44] Operational Alert Thresholds (Ngưỡng Kích hoạt Báo động)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 3
- `THUMB_RULE`: Chỉ kích hoạt cảnh báo tới đội ngũ kỹ thuật khi các chỉ số chạm ngưỡng nguy hiểm thực tế.
- `TRIGGER`: Cấu hình hệ thống cảnh báo vận hành.
- `ACTION`: Gửi cảnh báo qua Telegram/Slack khi:
  1. CPU vượt quá 85% liên tục trong 5 phút.
  2. Bộ nhớ RAM khả dụng dưới 10%.
  3. Hangfire Job thất bại liên tiếp 3 lần.

---

## DOMAIN 5: QUALITY ASSURANCE, ENVIRONMENT & DEVOPS (KIỂM THỬ VÀ VẬN HÀNH)

**[K45] Test Pyramid Strategy (Chiến lược Kiểm thử 70/20/10)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Mỗi Service/Handler phải có unit test bao phủ các kịch bản thành công và ngoại lệ. Đạt tối thiểu 70% độ phủ cho tầng Business.
- `TRIGGER`: Khi hoàn thành xử lý logic trong tầng Domain và Application.
- `ACTION`:
  1. Unit Test (70%): Kiểm thử logic nghiệp vụ thuần túy, mock toàn bộ các phụ thuộc ngoại vi.
  2. Integration Test (20%): Kiểm thử tích hợp với cơ sở dữ liệu thật thông qua TestContainers.
  3. End-to-End Test (10%): Kiểm thử luồng màn hình cốt lõi của người dùng.

**[K46] Test Data Isolation (Cô lập Dữ liệu Kiểm thử)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 2
- `THUMB_RULE`: Kiểm thử tự động tuyệt đối không sử dụng dữ liệu thật của khách hàng.
- `TRIGGER`: Khi viết Integration Test hoặc End-to-End Test.
- `ACTION`:
  1. Khởi tạo dữ liệu giả lập (Bogus/AutoFixture) độc lập cho từng ca kiểm thử.
  2. Sử dụng Database Transaction rollback sau mỗi test run để đảm bảo tính cô lập hoàn toàn giữa các test case.

**[K47] External Mocking & Chaos Testing (Giả lập Dịch vụ Ngoại vi)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 2
- `THUMB_RULE`: Các API bên thứ ba phải được giả lập (mock) toàn diện trong môi trường kiểm thử tự động.
- `TRIGGER`: Viết kiểm thử cho các tính năng phụ thuộc hệ thống bên ngoài.
- `ACTION`:
  1. Dùng Moq hoặc WireMock để giả lập phản hồi của hệ thống ngoại vi.
  2. Bắt buộc kiểm thử đầy đủ các kịch bản: Timeout, lỗi 429 Too Many Requests, 500 Internal Server Error và dữ liệu trả về sai cấu trúc.

**[K48] Definition of Done & Doc Gate (Tiêu chuẩn Hoàn tất & Cổng Tài liệu - DoD)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Một tính năng chỉ được nghiệm thu hoàn tất khi thỏa mãn toàn bộ tiêu chuẩn kiểm thử, build sạch và tài liệu liên quan đã được đồng bộ 100%.
- `TRIGGER`: Đánh dấu một User Story hoặc Task là Hoàn thành (Done).
- `ACTION`: Xác nhận đủ 6 tiêu chí:
  1. Code build 0 warning, 0 error.
  2. Có unit test bao phủ các nhánh xử lý cốt lõi.
  3. Đã chạy thử nghiệm thực tế thành công trên môi trường cục bộ.
  4. Đã cập nhật Data Dictionary và API Contracts tương ứng ([K15]).
  5. Toàn bộ nợ kỹ thuật phát sinh đều được log vào `tech_debt_report.md` ([K07]).
  6. Commit tuân thủ chuẩn Conventional Commits ([K08]).

**[K49] Pre-Commit Quality Checklist & Doc Drift Scan (Chuỗi Kiểm tra Bắt buộc)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Chặn đứng mã lỗi, mã sai định dạng, lọt secret hoặc lệch pha tài liệu trước khi ghi vào kho mã nguồn.
- `TRIGGER`: Trước khi chạy lệnh `git commit`.
- `ACTION`: Chạy chuỗi lệnh kiểm tra:
  1. `dotnet format --verify-no-changes`
  2. `dotnet build --configuration Release`
  3. `dotnet test`
  4. Quét kiểm tra bí mật (Secret scan) qua script tự động.
  5. Quét kiểm tra UI Audit ([K35]).
  6. **Doc Drift Scan**: Nếu commit có chứa file DDL `*.sql` hoặc file DTO C#, bắt buộc kiểm tra xem có file tương ứng trong `docs/specs/` được commit kèm hay không; cảnh báo hoặc dừng commit nếu thiếu.

**[K50] Dependency Locking (Khóa Cứng Phiên bản Package)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 1
- `THUMB_RULE`: Cố định chính xác phiên bản của tất cả các package và thư viện sử dụng trong hệ thống.
- `TRIGGER`: Cài đặt hoặc nâng cấp NuGet package.
- `ACTION`: Luôn chỉ định rõ phiên bản số (vd: `PackageVersion="10.0.1"`), tuyệt đối không dùng ký tự đại diện (`*`). Ghi nhận lý do nâng cấp vào `docs/dependencies.md`.

**[K51] Dry-Run & Cross-Check (Chạy Thử Khô & Đối Soát Số Liệu)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Thực hiện rà soát chéo số liệu và chạy thử trong suy luận trước khi đưa ra kết luận hoặc bàn giao sản phẩm.
- `TRIGGER`: Viết runbook, soạn thảo script SQL hoặc sửa mã nguồn có tính phụ thuộc cao.
- `ACTION`:
  1. Sửa cục bộ (Diff): Chỉ sửa đúng các dòng cần tác động, không ghi đè lại cả tệp nếu chỉ thay đổi vài ký tự.
  2. Khớp số liệu 100%: Toàn bộ các con số thống kê, số lượng cột, số bản ghi nêu trong tài liệu mô tả phải khớp chính xác với mã SQL/Code thực tế.
  3. Tự kiểm nghiệm: Tự đặt câu hỏi "Lệnh này nếu chạy lại lần 2 có sinh lỗi không?", "Namespace này có bị xung đột không?".

**[K52] Environment Variable Persistence (Lưu Biến Môi trường User Cố định)**
- `[SEVERITY]`: 🟢 Medium
- `[SPRINT]`: 1
- `THUMB_RULE`: Thiết lập biến môi trường ở phạm vi User một lần duy nhất, tránh gõ lại thủ công mỗi phiên.
- `TRIGGER`: Khởi tạo môi trường phát triển trên máy mới.
- `ACTION`:
  1. Lưu biến cố định bằng PowerShell:
     ```powershell
     [System.Environment]::SetEnvironmentVariable('PGCLIENTENCODING', 'UTF8', 'User')
     [System.Environment]::SetEnvironmentVariable('PGPASSWORD', 'sa_secret', 'User')
     ```
  2. Lưu cấu hình mẫu (không chứa mật khẩu thật) vào `docs/setup/environment.md`.

**[K53] Automation Script Idempotency (Khả lặp & Neo Thư mục Trong Script)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi script tự động hóa khi thực thi nhiều lần trên cùng một máy phải đảm bảo không sinh lỗi và không làm hỏng dữ liệu.
- `TRIGGER`: Viết script CLI (PowerShell, Bash, SQL).
- `ACTION`:
  1. Câu lệnh SQL phải luôn có mệnh đề `IF NOT EXISTS` hoặc `IF EXISTS`.
  2. Thao tác tập tin/thư mục phải kiểm tra sự tồn tại hoặc sử dụng cờ ép buộc (`-Force`).
  3. Luôn neo thư mục gốc của project ở dòng đầu script trước khi thực hiện các đường dẫn con.

**[K54] Safe Rollback Protocol (Quy trình Lùi Phiên bản An toàn)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 3
- `THUMB_RULE`: Mọi đợt phát hành lên Production bắt buộc phải chuẩn bị sẵn kịch bản lùi phiên bản an toàn.
- `TRIGGER`: Trước khi kích hoạt quy trình triển khai phiên bản mới.
- `ACTION`:
  1. Đánh Git Tag cho phiên bản hiện tại trước khi deploy.
  2. Thực hiện backup cơ sở dữ liệu vật lý.
  3. Ghi rõ 3 lệnh phục hồi (Rollback commands) cụ thể trong Runbook của phiên làm việc.

**[K55] Automated Backup & Disaster Recovery (Sao lưu & Phục hồi CSDL)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 3
- `THUMB_RULE`: Dữ liệu là tài sản cốt lõi; phải có quy trình sao lưu tự động và kiểm thử phục hồi định kỳ.
- `TRIGGER`: Thiết lập hạ tầng và định cấu hình máy chủ CSDL.
- `ACTION`:
  1. Lập lịch tự động chạy lệnh `pg_dump` vào 01:00 AM hàng ngày.
  2. Nén và đồng bộ dữ liệu sao lưu lên kho lưu trữ đám mây biệt lập (S3/B2), duy trì tối thiểu 30 bản gần nhất.
  3. Thực hiện diễn tập phục hồi (Restore drill) tối thiểu 1 lần mỗi tháng vào môi trường Staging.

**[K56] Zero-Downtime Deployment (Triển khai Không Gián đoạn Dịch vụ)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 3
- `THUMB_RULE`: Không làm gián đoạn người dùng khi cập nhật phần mềm. Cập nhật CSDL phải luôn tương thích ngược.
- `TRIGGER`: Triển khai cập nhật hệ thống trên Production.
- `ACTION`:
  1. Chạy các **Script SQL cập nhật CSDL** (theo quy tắc đánh version tuần tự [K14]) trước. Mọi thay đổi schema phải tương thích ngược (chỉ thêm cột NULL hoặc có Default).
  2. Triển khai mã nguồn ứng dụng mới.
  3. Sử dụng cơ chế Graceful Reload (`systemctl reload` hoặc Blue-Green switch) thay vì dừng đột ngột.
  4. Chạy kịch bản kiểm tra nhanh (Smoke Test) tự động xác thực hệ thống ngay sau deploy.

**[K57] System Health Check Endpoints (Kiểm tra Sức khỏe Thành phần)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: 3
- `THUMB_RULE`: Hệ sinh thái phải có endpoint cung cấp tình trạng hoạt động thực tế của từng thành phần.
- `TRIGGER`: Khởi tạo và cấu hình ứng dụng web/API.
- `ACTION`: Tạo endpoint `/health` trả về kết quả kiểm tra trạng thái kết nối tới PostgreSQL, Hangfire Server và dung lượng ổ đĩa. Trả về mã lỗi HTTP 503 nếu một thành phần cốt lõi ngừng hoạt động.

---

## DOMAIN 6: RUNBOOK STANDARDS & META-LEARNING (TIÊU CHUẨN RUNBOOK VÀ NĂNG LỰC AI)

**[K58] Runbook Folder Compliance (Quy chuẩn Thư mục Runbook)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: File tài liệu và mã nguồn phải nằm đúng cấu trúc cây thư mục quy định.
- `TRIGGER`: Khi tạo file tài liệu, runbook hoặc script hỗ trợ.
- `ACTION`:
  1. Tạo đúng đường dẫn: `docs/sprints/sprint_<X>/...` hoặc `docs/setup/...`.
  2. Nếu phát hiện file nằm sai vị trí quy ước, thực hiện di dời (move) và cập nhật lại đường dẫn tham chiếu.

**[K59] Objectives, Verification Targets & 2-Tier Sizing (Phân Cấp Quy Chuẩn Runbook)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Chống phình to tài liệu không cần thiết (Anti-Bloat). Phân định rõ cấp độ Runbook ngay từ đầu; bắt buộc có bảng Mục tiêu và Chỉ số định lượng.
- `TRIGGER`: Bắt đầu soạn thảo Runbook mới cho một đầu việc kỹ thuật.
- `ACTION`:
  1. **Phân Tầng Runbook Cụ Thể**:
     - **Cấp độ 1 - Standard / Micro Runbook (Task dưới 30 phút, Hotfix)**: Tối ưu tính tinh gọn ($< 120$ dòng). Chỉ gồm Mục tiêu $\rightarrow$ Chuỗi `Setup` $\rightarrow$ `Exec` $\rightarrow$ `Verify` $\rightarrow$ 1 lệnh `Rollback`. Không bắt buộc ma trận 5 lỗi hoặc phân tầng Learn nếu thao tác đơn giản.
     - **Cấp độ 2 - Enterprise Runbook (Core Migration, Sprint Release, Tích hợp mới)**: Bắt buộc áp dụng đầy đủ $100\%$ các tiêu chí kiểm soát từ `[K58]` đến `[K67]`.
  2. **Bảng Objectives & Verification Targets (Bắt buộc cho cả 2 cấp)**:
     - Nêu rõ 3–5 kết quả công việc cụ thể cần đạt.
     - Con số định lượng cụ thể (số bản ghi tạo ra, số test pass, thời gian chạy).
  3. Cuối Runbook phải có **Final Audit Checklist** đối chiếu từng chỉ số với Targets ban đầu.

**[K60] Root Cause Analysis Enforcement (Phân tích Nguyên nhân Gốc rễ Sự cố)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mỗi sự cố kỹ thuật hoặc bug phát sinh trong quá trình thực thi phải được ghi lại theo mẫu phân tích nguyên nhân gốc rễ.
- `TRIGGER`: Sau khi giải quyết xong một lỗi phát sinh trong phiên làm việc.
- `ACTION`: Trình bày đủ 4 nội dung vào tài liệu:
  1. **Triệu chứng (Symptom)**: Hiện tượng lỗi, mã lỗi cụ thể.
  2. **Nguyên nhân gốc (Root Cause)**: Tại sao lỗi xảy ra ở mức hệ thống/logic.
  3. **Giải pháp khắc phục (Fix Action)**: Đã sửa bằng cách nào.
  4. **Xác nhận (Verification)**: Bằng chứng cho thấy lỗi đã biến mất.

**[K61] Real Console Output Verification & Execution Placeholder (Xác Thực Bằng Kết Quả Console Thật)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không ghi lại kết quả giả định hoặc kỳ vọng lý thuyết. Áp dụng quy trình 2 pha nghiêm ngặt giữa Lập kế hoạch (Planning) và Thực thi (Execution).
- `TRIGGER`: Viết phần kiểm thử hoặc nghiệm thu kết quả dòng lệnh trong Runbook.
- `ACTION`:
  1. **Pha 1 - Drafting / Planning (Chưa chạy terminal thật)**: Agent TUYỆT ĐỐI KHÔNG tự bịa ra log giả lập. Bắt buộc đặt thẻ giữ chỗ tường minh:
     ```markdown
     > [PENDING REAL EXECUTION: Chờ kỹ sư chạy lệnh và paste output xác thực vào đây]
     ```
  2. **Pha 2 - Execution / Verification (Đã chạy trên máy Dev/Staging)**: Chạy lệnh thật, sao chép nguyên văn output trả về từ terminal và thay thế vào thẻ giữ chỗ trong khối code `text`.

**[K62] Step-by-Step Flow & Human Time Budget (Phân Đoạn Dây Chuyền & Ngân Sách Thời Gian Kỹ Sư)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Thiết kế quy trình thực thi một chiều theo dạng dây chuyền; ngân sách thời gian phải tính theo thao tác của kỹ sư con người (Human Operator).
- `TRIGGER`: Soạn thảo kịch bản triển khai hoặc hướng dẫn cài đặt trong Runbook.
- `ACTION`:
  1. Phân chia rõ 4 giai đoạn độc lập: Setup $\rightarrow$ Execution $\rightarrow$ Verification $\rightarrow$ Rollback.
  2. Người thực thi chỉ cần thực hiện tuần tự từ trên xuống dưới mà không cần nhảy cóc bước.
  3. **Định lượng Human Time Budget**: Thời lượng dự kiến cho từng bước phải dựa trên tốc độ đọc hiểu, gõ lệnh, nhập mật khẩu và độ trễ mạng của một kỹ sư con người, cộng thêm $20\%$ biên độ an toàn (Buffer), tuyệt đối không tính theo tốc độ phản hồi tính bằng mili-giây của bot/AI background script.

**[K63] Verification Checkpoints & Assertions (Chốt chặn & Khẳng định Đầu ra)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mỗi câu lệnh làm thay đổi trạng thái hệ thống phải có ngay một câu lệnh kiểm tra kết quả đi kèm.
- `TRIGGER`: Viết hướng dẫn chạy lệnh shell/CLI làm thay đổi hệ thống.
- `ACTION`:
  1. Cung cấp câu lệnh kiểm tra (vd: `Test-Path`, `curl`, `dotnet test`) ngay liền sau lệnh thực thi.
  2. Nêu chính xác kết quả đầu ra kỳ vọng (mã trạng thái, chuỗi text trả về).

**[K64] Troubleshooting Matrix (Ma trận Tra cứu & Xử lý Nhanh Sự cố)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi Runbook triển khai phải cung cấp bảng tra cứu sự cố kèm cách giải quyết ngay lập tức.
- `TRIGGER`: Xây dựng Runbook cho các tác vụ quan trọng.
- `ACTION`: Lập bảng đối soát tối thiểu 5 sự cố thường gặp nhất theo cấu trúc: `Mã/Dấu hiệu lỗi | Nguyên nhân gốc rễ | Lệnh xử lý nhanh`.

**[K65] Deviation Warnings (Cảnh báo Điểm nghẽn & Lệch chuẩn Dự kiến)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mỗi bước triển khai phức tạp trong Runbook phải dự báo trước các lỗi dễ mắc phải và cách thoát lỗi.
- `TRIGGER`: Khi biên soạn các bước kỹ thuật trong Runbook.
- `ACTION`: Bổ sung khối cảnh báo ngay dưới bước thao tác:
  ```markdown
  > ⚠ [LỆCH CHUẨN DỰ KIẾN]: Nếu thao tác sai ở bước này, hệ thống sẽ báo lỗi `CS0104`.
  > - Nguyên nhân: Trùng lặp namespace.
  > - Khắc phục nhanh: Thêm tiền tố định danh rõ ràng.
  ```

**[K66] Dual-Layer Runbook (Runbook Phân tách 2 Tầng Nội dung)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Tách biệt rạch ròi giữa phần hướng dẫn thực thi lệnh và phần giải thích kiến thức nền tảng.
- `TRIGGER`: Khi soạn thảo Runbook có yếu tố chuyển giao công nghệ.
- `ACTION`: Mỗi giai đoạn được chia rõ:
  1. Phân đoạn **[EXEC]** (70%): Chỉ gồm các bước thao tác, dòng lệnh trực tiếp.
  2. Phân đoạn **[LEARN]** (30%): Giải thích lý do kiến trúc, cơ chế hoạt động ngầm.

**[K67] Two-Pass Execution (Quy trình Thực thi 2 Lượt Độc lập)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Tách bạch tuyệt đối giữa việc tạo ra sản phẩm chạy được và việc đào sâu nghiên cứu bản chất.
- `TRIGGER`: Khi triển khai một kỹ thuật hoặc thư viện mới chưa từng sử dụng trước đây.
- `ACTION`:
  1. **Lượt 1 - Thực thi nhanh (30-45 phút)**: Thực hiện chuẩn xác theo các bước hướng dẫn để có ngay bản mẫu hoạt động (Working Prototype).
  2. **Lượt 2 - Mổ xẻ chi tiết (Không giới hạn)**: Đọc lại từng dòng code, phân tích cơ chế hoạt động, đặt câu hỏi tại sao và ghi chép lại nguyên lý.

**[K68] Controlled Sabotage (Chủ động Tạo Lỗi Khám phá Kiến trúc)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Cách tốt nhất để hiểu sâu một kiến trúc là chủ động phá vỡ nó để quan sát hành vi lỗi.
- `TRIGGER`: Sau khi đã có bản mẫu hoạt động từ quy tắc [K67].
- `ACTION`:
  1. Tạo một nhánh Git thử nghiệm riêng biệt.
  2. Thực hiện các thử nghiệm cố tình gây lỗi (xóa interface, sửa sai connection string, đổi sai kiểu dữ liệu).
  3. Ghi lại mã lỗi và cách hệ thống phản ứng vào `docs/notes/sabotage_log.md`.
  4. Hoàn tác lại trạng thái mã nguồn ban đầu (`git restore .`).

**[K69] Concept Mastery Metric (Thước đo Nắm vững Khái niệm)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Đánh giá mức độ thành công của phiên làm việc bằng số lượng khái niệm được làm chủ, không chỉ bằng thời gian hoàn thành.
- `TRIGGER`: Kết thúc mỗi phiên làm việc kỹ thuật.
- `ACTION`: Ghi nhận tối thiểu 3 khái niệm cốt lõi đã nắm vững vào `docs/notes/concepts_mastered.md`.

**[K70] Daily Log Rotation & Context Synchronization (Xoay Vòng Nhật Ký & Đồng Bộ)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không bao giờ làm việc khi mất dấu vết lịch sử; đồng thời ngăn chặn lãng phí bộ nhớ token do nhật ký phình to quá mức (Daily Log Token Inflation).
- `TRIGGER`: Bắt đầu một phiên làm việc mới hoặc chuyển giao giữa các Agent.
- `ACTION`:
  1. **Quy tắc Cửa sổ 3 Ngày (3-Day Active Window)**: File `docs/notes/daily.md` hiện hành CHỈ lưu nhật ký của tối đa 3 ngày làm việc gần nhất.
  2. **Quy tắc Xoay Vòng (Log Rotation Protocol)**: Đầu mỗi tuần hoặc khi nhật ký vượt quá 3 ngày, Agent tự động di dời các ngày cũ hơn vào file lưu trữ theo tháng (`docs/notes/daily_YYYY_MM.md`) hoặc theo Sprint (`docs/sprints/sprint_<X>/daily.md`).
  3. **Đầu phiên**: Đọc `docs/notes/daily.md` để nắm bắt tiến độ nóng.
  4. **Cuối phiên**: Ghi nhận các đầu việc hoàn thành, vướng mắc còn lại vào cuối file `docs/notes/daily.md`.

**[K71] Context Assimilation (Chủ động Tự tra cứu Codebase)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Tự tìm kiếm câu trả lời trong mã nguồn trước khi đặt câu hỏi cho người dùng.
- `TRIGGER`: Khi gặp một biến lạ, bảng dữ liệu chưa rõ nguồn gốc hoặc câu hỏi mở.
- `ACTION`: Chủ động sử dụng các lệnh rà soát codebase (`grep`, `find`, `list_files`) để tìm định nghĩa và mối liên kết thay vì hỏi vị trí từ người dùng.

**[K72] Sandbox Experimentation (Kiểm chứng Lỗi qua Hộp cát)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Tuyệt đối không phỏng đoán nguyên nhân gây lỗi dựa trên lý thuyết. Bắt buộc phải tái hiện lỗi bằng mã thực nghiệm.
- `TRIGGER`: Khi người dùng yêu cầu phân tích một lỗi lập trình phức tạp.
- `ACTION`: Tạo một đoạn code nhỏ hoặc bài test cô lập chạy trực tiếp để lấy log console làm căn cứ chứng minh bản chất lỗi trước khi đưa ra kết luận.

**[K73] Clean Test Artifacts (Dọn dẹp Hiện trường Thử nghiệm)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Không để lại bất kỳ tệp rác, mã thử nghiệm tạm bợ nào trong nhánh mã nguồn chính thức.
- `TRIGGER`: Sau khi hoàn tất quá trình kiểm chứng hộp cát ([K72]) hoặc sửa xong lỗi.
- `ACTION`:
  1. Xóa bỏ hoàn toàn các file test tạm, file log debug phát sinh (vd: `temp_test.cs`, `debug_log.txt`).
  2. Gỡ bỏ các thư viện hoặc package cài tạm thời phục vụ quá trình dò lỗi nếu dự án không sử dụng lâu dài.

---

## DOMAIN 7: B2B CRM BUSINESS LOGIC & WORKFLOW ENGINE (QUY CHUẨN NGHIỆP VỤ B2B CRM)

**[K74] CRM Core Entity Hierarchy & Sales Pipeline (Cấu Trúc Thực Thể B2B CRM)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Phân định rạch ròi 4 đối tượng kinh doanh cốt lõi; mọi cơ hội kinh doanh bắt buộc phải gắn với một Phễu Bán Hàng (Sales Pipeline) cụ thể.
- `TRIGGER`: Thiết kế schema, Entity, DTO hoặc luồng nghiệp vụ CRM.
- `ACTION`:
  1. **Lead**: Đầu mối tiềm năng thô, chưa xác thực danh tính doanh nghiệp hoặc nhu cầu thực.
  2. **Account**: Doanh nghiệp hoặc pháp nhân khách hàng (B2B Client).
  3. **Contact**: Người liên hệ, người đại diện hoặc người ra quyết định thuộc về một Account.
  4. **Opportunity**: Cơ hội bán hàng cụ thể phát sinh từ Account. Opportunity BẮT BUỘC gắn với một `PipelineId` và một Stage (Giai đoạn) trong chu trình bán hàng.

**[K75] CRM Operations & Customer Journey (Hành Trình Bán Hàng B2B)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không thiết kế luồng quy trình cụt. Mọi hành vi tiếp thị và bán hàng phải nối kết xuyên suốt.
- `TRIGGER`: Xây dựng module tiếp nhận Lead, phân bổ và bán hàng.
- `ACTION`:
  1. **Lead Assignment**: Cài đặt thuật toán chia số tự động xoay vòng (Round-robin) hoặc theo tải công việc giữa các nhân viên Sales trong cùng Team.
  2. **Follow-up Protocol**: Tự động sinh lịch hẹn/nhiệm vụ chăm sóc định kỳ cho nhân viên phụ trách khi chuyển giai đoạn.
  3. **Customer Journey B2B**: Chuẩn hóa luồng giao dịch khép kín:
     $$\text{Lead} \longrightarrow \text{Qualified Opportunity} \longrightarrow \text{Quote (Báo giá)} \longrightarrow \text{Sales Order (Đơn hàng)} \longrightarrow \text{Invoice (Hóa đơn)}$$

**[K76] CRM Automation & SLA Escalation Protocol (Tự Động Hóa & Chuyển Cấp SLA)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Mọi tương tác khách hàng B2B phải có thời gian cam kết phản hồi (SLA). Nếu vi phạm phải tự động chuyển cấp báo động.
- `TRIGGER`: Viết Background Worker kiểm tra hạn xử lý hoặc luồng workflow nhắc việc.
- `ACTION`:
  1. Thiết lập cấu hình SLA cho từng giai đoạn (ví dụ: Lead mới phải được liên hệ trong vòng 15 phút).
  2. Định kỳ quét dữ liệu qua Hangfire: Nếu quá hạn SLA, tự động kích hoạt **Escalation**:
     - Gửi thông báo nhắc nhở nhân viên phụ trách qua Notification/Telegram.
     - Nếu tiếp tục quá hạn gấp đôi thời gian SLA, tự động gắn cờ cảnh báo và gửi báo cáo vi phạm lên Manager của Team.

**[K77] CRM Metrics, Funnel Tracking & Revenue Forecast (Đo Lường & Dự Báo Doanh Thu)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Dữ liệu sinh ra là để đo lường; cấm cập nhật trạng thái mù quáng làm mất dấu vết chuyển đổi.
- `TRIGGER`: Thiết kế bảng CSDL, viết API cập nhật trạng thái Opportunity hoặc tính toán Dashboard.
- `ACTION`:
  1. **State History Tracking**: Bắt buộc ghi lại bản ghi lịch sử mỗi khi chuyển Stage (`from_stage`, `to_stage`, `changed_at`, `changed_by`, `duration_in_stage`).
  2. **Công Thức Đo Lường Chuẩn**:
     - **Conversion Rate (Tỷ lệ chuyển đổi Lead $\rightarrow$ Opportunity)**:
       $$\text{Conversion Rate} = \frac{\text{Tổng Lead chuyển đổi thành công}}{\text{Tổng Lead tiếp nhận}} \times 100\%$$
     - **Win Rate (Tỷ lệ thắng)**:
       $$\text{Win Rate} = \frac{\text{Tổng Opportunity Closed-Won}}{\text{Tổng Opportunity (Closed-Won + Closed-Lost)}} \times 100\%$$
     - **Revenue Forecast (Dự báo doanh thu gia quyền)**:
       $$\text{Weighted Forecast} = \sum (\text{Giá trị Deal} \times \text{Xác suất thành công của Stage})$$
  3. Cung cấp dữ liệu sẵn sàng cho Báo cáo Phễu (Funnel Report) và Báo cáo Tỷ lệ Duy trì (Retention Report) qua các bảng Snapshot ngầm ([K24]).

---

## DOMAIN 8: INTERVIEW & PORTFOLIO EXCELLENCE (NGHỆ THUẬT PHỎNG VẤN & XÂY DỰNG HỒ SƠ NĂNG LỰC)

**[K79] Architectural Trade-off Articulation (Phân Tích Đánh Đổi Kiến Trúc)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không bao giờ nói một công nghệ/pattern là "tốt nhất tuyệt đối". Mọi quyết định kiến trúc phức tạp (CQRS, Multi-tenant) bắt buộc phải đi kèm lý luận "Được gì (Pros) / Mất gì (Cons)".
- `TRIGGER`: Khi giải thích mã nguồn, viết tài liệu hoặc thiết kế tính năng mới.
- `ACTION`:
  1. Phân tích chi phí trả trước (Upfront Cost): Thời gian code, số lượng file, độ khó bảo trì cho Fresher.
  2. Phân tích lợi ích dài hạn (Long-term ROI): Khả năng mở rộng (Scale), bảo mật (Security), chống rác dữ liệu.
  3. Chốt lại bằng ngữ cảnh: "Vì mục tiêu của dự án là B2B SaaS, sự đánh đổi này là hoàn toàn xứng đáng."

**[K80] Elevator Pitch & Keyword Injection (Trình Bày Tốc Chiến Bọc Từ Khóa)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Khi tóm tắt một tính năng, phải gói gọn trong 2-3 câu chứa các keyword "đắt giá" (Buzzwords có chiều sâu) mà Technical Lead muốn nghe.
- `TRIGGER`: Khi kết thúc một tính năng cốt lõi (Core feature) hoặc được yêu cầu tóm tắt công việc.
- `ACTION`:
  1. Tránh mô tả CRUD nhàm chán ("Em viết hàm Get, Post").
  2. Sử dụng "Văn mẫu" bọc Keyword: "Để giải quyết bài toán [A], em áp dụng mô hình [B], kết hợp kỹ thuật [C] để phòng chống rủi ro [D], đồng thời đảm bảo [E]."
  3. (VD: "Để giải quyết bài toán SaaS nhiều công ty thuê, em tách luồng Read/Write qua CQRS, kết hợp IDOR 2 lớp để chống rò rỉ dữ liệu, đảm bảo Test Coverage 100% bằng Moq").

**[K81] Defensive Interviewing (Phòng Thủ & Tự Phản Biện Áp Lực)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Luôn chuẩn bị sẵn sàng cho câu hỏi: "Tại sao em làm phức tạp hóa vấn đề như vậy? Làm cách đơn giản hơn không được sao?"
- `TRIGGER`: Khi áp dụng các thiết kế có tính "Over-engineering" (như CQRS, Event-Driven) vào các tính năng có vẻ đơn giản.
- `ACTION`:
  1. Tự đặt mình vào vai người phỏng vấn khó tính.
  2. Tạo bộ câu hỏi Q&A phản biện (Ví dụ: "Dùng CQRS cho một bảng Customer có quá thừa thãi không?").
  3. Lập luận phòng thủ: Đồng ý với người phỏng vấn (Em đồng ý nếu dự án nhỏ thì dư thừa), nhưng đưa ra tầm nhìn xa (Nhưng đây là core engine của CRM SaaS, nó cần độ cách ly (Isolation) và mở rộng (Extensibility) từ Day 1).
