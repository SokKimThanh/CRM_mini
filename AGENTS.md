# AGENTS.md - BỘ NHỚ LẬP TRÌNH CHO AI (JULES)

Đây là tài liệu bắt buộc (Mandatory) dành cho bất kỳ AI/Agent nào khi làm việc trong dự án này.
Mọi hành động sinh code, chat, tạo nhánh phải tuân thủ nghiêm ngặt các Block dưới đây.

---

## BLOCK 1: [CORE_DIRECTIVES] - NGUYÊN TẮC CỐT LÕI

**[K0] Skill Orchestration (Liên kết đa nhiệm)**
- `THUMB_RULE`: KHÔNG BAO GIỜ dùng 1 kỹ năng đơn lẻ. Mọi phản hồi phải là một chuỗi hành động (Chain of Actions).
- `TRIGGER`: Nhận mọi task từ User.
- `ACTION`: Nhận Task -> Đọc DB/Liên kết (K2) -> Băm nhỏ task (K3) -> Code (K10,K11) -> Cập nhật Docs (K6, K7) -> Lập báo cáo Nợ Kỹ thuật (K4) -> Git Commit (K5).

**[K1] Plain Language Policy (Bình Dân Học Vụ)**
- `THUMB_RULE`: Nói ít làm nhiều. Phải giải thích code/tài liệu bằng từ ngữ cực kỳ đơn giản.
- `TRIGGER`: Khi phải sinh Chat, viết Docs, README, Code Comments.
- `ACTION`: Lọc bỏ từ ngữ học thuật phức tạp. Tập trung vào thực thi (Ví dụ: "Tôi đã tạo hàm để lấy dữ liệu" thay vì "Thực thi Repository Pattern Dependency Injection").

---

## BLOCK 2: [AGILE_WORKFLOW] - QUY TRÌNH QUẢN LÝ DỰ ÁN

**[K2] System Thinking (Tư Duy Hệ Thống - "Nói ít hiểu nhiều")**
- `THUMB_RULE`: KHÔNG BẮT TAY VÀO LÀM NGAY (Code/Docs) nếu chưa nắm rõ bức tranh tổng thể. Hệ thống hóa thông tin trước khi hành động.
- `TRIGGER`: User yêu cầu tính năng, viết tài liệu hoặc xử lý luồng logic có ảnh hưởng diện rộng.
- `ACTION`:
  1. Tự động đọc Master Plan/Roadmap (trong `docs/`) và quét DB để xác định dự án đang ở giai đoạn nào (Sprint mấy).
  2. Bắt mạch bối cảnh: Tự hỏi "Việc này có nằm trong mục tiêu giai đoạn hiện tại không? Có phá vỡ cấu trúc cũ không?".
  3. Chỉ hỏi/đề xuất lại người dùng bằng các câu ngắn gọn, trúng đích, đưa ra 1-2 tùy chọn cụ thể. Tuyệt đối không giải thích dài dòng quá trình mình đã phân tích.

**[K3] Auto-Prompting (Tự động chia nhỏ task)**
- `THUMB_RULE`: Mọi task phải được băm nhỏ thành các "Phiên làm việc" < 30 phút.
- `TRIGGER`: Đầu Sprint hoặc khi nhận 1 Spec lớn.
- `ACTION`: AI tự sinh ra các Prompt thực thi (Actionable Prompts) và lưu vào `docs/sprints/sprint_<X>/prompts/`. Đợi User dùng các prompt này để ra lệnh tiếp.

**[K4] Tech Debt Management (Quản lý Nợ Kỹ Thuật)**
- `THUMB_RULE`: Phải minh bạch về code tạm, mã rác.
- `TRIGGER`: Cuối mỗi task hoặc khi để lại TODO/Hardcode.
- `ACTION`: Tự động tạo hoặc append báo cáo vào `docs/sprints/sprint_<X>/tech_debt_report.md`.

**[K14] Context-First Verification (Bắt mạch dự án)**
- `THUMB_RULE`: CẤM NHẮM MẮT LÀM BỪA khi nhận một yêu cầu chung chung. Phải định vị được Context trước khi thực thi.
- `TRIGGER`: Khi User giao một task thiếu ngữ cảnh (vd: "Viết Runbook", "Tạo API", "Sửa bug").
- `ACTION`:
  1. Tự động tra cứu hệ thống (`list_files`, `README`, Master Plan).
  2. Không báo cáo dài dòng về quá trình tra cứu.
  3. Dùng ngôn ngữ đơn giản chốt lại 1-2 tùy chọn thực thi thực tế nhất với người dùng (vd: "Em thấy dự án có 7 Sprints, anh muốn làm Sprint 1 hay 2?").

---

## BLOCK 3: [GIT_AND_FILESYSTEM] - QUẢN LÝ NHÁNH VÀ FOLDER

**[K5] Git Workflow & Auto-Branching**
- `THUMB_RULE`: AI TỰ ĐỘNG khởi tạo nhánh chuẩn nếu chưa có (`main`, `staging`, `develop`, `docs`).
- `TRIGGER`: Trước khi bắt đầu viết mã mới.
- `ACTION`: Chạy `git branch`. Nếu tạo tính năng, tự checkout sang `feature/<name>`. Khi hoàn thành, format commit chuẩn (Conventional Commits: `feat:`, `fix:`).

**[K8] Runbook Folder Compliance (Tuân thủ cấu trúc thư mục)**
- `THUMB_RULE`: Không vứt file lung tung. Phải đúng Runbook.
- `TRIGGER`: Khi bắt đầu Sprint mới hoặc tạo file tài liệu mới.
- `ACTION`: Tự tạo thư mục `docs/sprints/sprint_<X>/...`. Tự động dời (move) file nếu người dùng lưu sai chỗ.

---

## BLOCK 4: [DOCUMENTATION_RULES] - QUẢN LÝ TÀI LIỆU

**[K6] Append-Only & Document Continuity (Ghi nối tiếp - "Nói ít làm nhiều")**
- `THUMB_RULE`: CẤM TUYỆT ĐỐI ghi đè (Overwrite) hoặc xóa trắng tài liệu cũ. Mọi tài liệu sinh mới phải kế thừa và trích dẫn tài liệu gốc.
- `TRIGGER`: Khi cập nhật SRS, Runbook, API Docs, hoặc tạo tài liệu mới.
- `ACTION`:
  1. Khi cập nhật: Đọc hiểu file cũ, dùng phương pháp thêm nội dung (Append/Insert) vào cuối mục lục phù hợp.
  2. Khi tạo mới: Bắt buộc trích dẫn (link ngược) về tài liệu Master Plan/Tiến trình. Nội dung không được tự phịa thêm scope mới.
  3. Viết trực diện: Đi thẳng vào các bước thực thi (Actionable steps). Không sinh ra các câu văn template sáo rỗng, dài dòng (Kế thừa K1).

**[K7] Archiving (Dọn dẹp Tài liệu cổ đại)**
- `THUMB_RULE`: Tài liệu sai lệch code phải bị cô lập.
- `TRIGGER`: Phát hiện Docs mâu thuẫn với Code mới.
- `ACTION`: Gắn tag `[DEPRECATED]` vào đầu file cũ, tạo file mới, move file cũ vào `docs/archive/`.

**[K9] Auto-README Refactoring (Tự động cắt nhỏ README)**
- `THUMB_RULE`: `README.md` KHÔNG ĐƯỢC dài quá 1 trang A4 (~100 dòng).
- `TRIGGER`: Mọi lúc sửa `README.md`.
- `ACTION`: Tự cắt nội dung dài ra file lẻ, lưu vào `docs/`, chỉ giữ lại Table of Content (Mục lục) ở README gốc.

---

## BLOCK 5: [TECH_STACK_MASTERY] - LUẬT LẬP TRÌNH CHUYÊN SÂU (.NET 8)

**[K10] Backend & Code Convention**
- `THUMB_RULE`: Bắt buộc SOLID, PascalCase, camelCase.
- `TRIGGER`: Khi sinh code C#.
- `ACTION`: Cấm Hardcode. Mọi service phải Inject qua Constructor. Các hàm gọi DB bắt buộc dùng `async/await`.

**[K11] Blazor UI & System Design**
- `THUMB_RULE`: Phân tách rạch ròi Smart UI và Dumb UI.
- `TRIGGER`: Khi sinh UI `.razor`.
- `ACTION`: Dùng MudBlazor. Không inject DbContext vào `.razor`. Bọc ErrorBoundary. Khi dùng JS (SortableJS/Charts) phải giải phóng bộ nhớ (Dispose). Quản lý cẩn thận State khi SignalR đứt gãy.

**[K12] Database Optimization (TỐI ƯU CỰC HẠN EF Core)**
- `THUMB_RULE`: Cấm N+1 query. Cấm tải thừa RAM.
- `TRIGGER`: Khi viết logic lấy Data.
- `ACTION`: Mọi list phải dùng `IQueryable` với `.Skip().Take()`. Dữ liệu chỉ đọc phải có `.AsNoTracking()`. Dùng DTO `.Select()`. Đề xuất Index DB cho các cột filter. Không tính toán Real-time trên UI cho Dashboard mà phải dùng Snapshot qua Hangfire.

**[K13] API Integration (Giao tiếp Ngoại)**
- `THUMB_RULE`: API phải chịu lỗi tốt.
- `TRIGGER`: Gọi API KiotViet.
- `ACTION`: Dùng `HttpClientFactory`, áp dụng `Polly` cho Retry/Circuit Breaker.
