# AGENTS.md - HỆ ĐIỀU HÀNH TƯ DUY CHO AI (JULES)

Đây là tài liệu bắt buộc (Mandatory) dành cho bất kỳ AI/Agent nào khi làm việc trong dự án này.
Mọi hành động sinh code, chat, tạo nhánh phải tuân thủ nghiêm ngặt 6 Trụ cột (Pillars) dưới đây. Cấm nén bớt, cấm lược bỏ.

---

## PILLAR 1: [AI_META_COGNITION] - HỆ ĐIỀU HÀNH TƯ DUY KỸ SƯ

**[K01] Master Skill Orchestration (Liên kết đa nhiệm tối cao)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: KHÔNG BAO GIỜ dùng 1 kỹ năng đơn lẻ. Mọi phản hồi phải là chuỗi hành động.
- `ACTION`: Nhận Task -> Tư duy [P1] -> Thẩm định Nghiệp vụ CRM [P2] -> Code Backend chuẩn [P3] -> Verify DevOps/Test [P6] -> Sinh Runbook/UI [P5] -> Commit Docs [P4].

**[K02] 10 Bước Tư Duy Bắt Buộc (Core Engineering Mindset)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: KHÔNG BAO GIỜ viết nháp code (Drafting) ngay lập tức.
- `ACTION`: Phải chạy qua 10 bước: (1) Requirement Analysis, (2) Domain Modeling, (3) Business Process Analysis, (4) Task Breakdown, (5) Change Impact Analysis, (6) Deep Audit, (7) Decision Making, (8) Validation, (9) Self-Review, (10) Lesson Learned.

**[K03] Daily Log & Context Synchronization (Đồng bộ bối cảnh)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Không bao giờ code mù. Phải đọc lịch sử trước khi làm.
- `ACTION`: Bắt đầu phiên: Mở đọc `docs/notes/daily.md` để nắm ngữ cảnh. Cuối phiên: Cập nhật file log báo cáo tiến độ.

**[K04] Context Assimilation (Thẩm thấu bối cảnh tự động)**
- `[SEVERITY]`: 🟡 High
- `THUMB_RULE`: Tự thân vận động.
- `ACTION`: Khi gặp biến lạ/class thiếu, phải dùng `grep`, `find`, `cat` rà quét codebase thay vì hỏi lại User vị trí file. Tự sinh Actionable Prompts đề xuất việc tiếp theo.

---

## PILLAR 2: [CRM_DOMAIN_EXPERTISE] - NGHIỆP VỤ B2B CRM

**[K05] Khung Quản lý Đối tượng (CRM Domain)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Phân định rạch ròi các đối tượng cốt lõi.
- `ACTION`: Nhận diện rõ **Lead** (Chưa định danh), **Contact** (Người), **Account** (Công ty), **Opportunity** (Cơ hội - phải gắn với **Sales Pipeline**).

**[K06] Quy trình và Hành trình Khách hàng (CRM Operations)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Không thiết kế luồng cụt.
- `ACTION`: Luôn kèm cơ chế **Lead Assignment** (Chia số), **Follow-up** (Chăm sóc). Thiết kế chuẩn **Customer Journey** (Quote → Order → Invoice).

**[K07] Tự động hóa và Cảnh báo (CRM Automation & SLA)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: B2B CRM phải có cảnh báo tự động.
- `ACTION`: Cấu hình **SLA** (Thời gian phản hồi). Nếu trễ phải kích hoạt **Escalation** thông qua **Auto Workflow**.

**[K08] Đo lường và Báo cáo (CRM KPI & Reporting)**
- `[SEVERITY]`: 🟡 High
- `THUMB_RULE`: Dữ liệu sinh ra là để đo lường.
- `ACTION`: Lưu vết thời gian ở từng State để tính **Conversion Rate**, **Win Rate**, **Revenue Forecast**.

---

## PILLAR 3: [ENTERPRISE_BACKEND_&_SECURITY] - CHUẨN KIẾN TRÚC & BẢO MẬT

**[K09] Phân Quyền Sở Hữu & Lịch sử (Data Integrity & Soft Delete)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Không thực thể vô chủ, không xóa cứng.
- `ACTION`:
  - Mọi dữ liệu kinh doanh phải có `OwnerId` và `TeamId`. Phân quyền đệ quy phòng ban.
  - Áp dụng Soft Delete (`is_deleted`) kết hợp **Global Query Filter**.
  - Bảng AuditLog, Lịch sử tương tác: CẤM TUYỆT ĐỐI XÓA MỀM VÀ CỨNG.

**[K10] EF Core Mapping & Concurrency Control**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Không tạo shadow properties, không đè dữ liệu.
- `ACTION`:
  - Cấu hình Navigation 2 chiều rõ ràng bằng `.WithMany()`. Nếu EF báo Shadow State, phải `.HasForeignKey()` rõ ràng.
  - Bắt buộc dùng `row_version` (xmin) làm Concurrency Token ở bảng trọng yếu.

**[K11] CQRS Strict Boundaries & Async Background Jobs**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Phân tách rạch ròi nhiệm vụ. Không block request.
- `ACTION`:
  - Controllers/Pages gửi Command. Repositories chỉ CRUD. Handler chứa toàn bộ Business Logic.
  - Gọi KiotViet API, gửi Email PHẢI dùng **Outbox Pattern** hoặc **Hangfire**. Không chạy đồng bộ.

**[K12] Bảo Mật (IDOR & XSS)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Không tin tưởng Client Input.
- `ACTION`:
  - **IDOR**: Handler luôn check `OwnerId` của user hiện tại trước khi `GetById/Update(id)`.
  - **XSS**: Dùng Sanitizer (HTML Filter) trước khi render Note của User lên Blazor UI.

**[K13] Hiệu Năng Truy Vấn & UI (Performance & Caching)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Chống nổ RAM.
- `ACTION`:
  - Truy vấn đọc >= 2 `Include` bắt buộc dùng `.AsSplitQuery()`. Không N+1 query.
  - Dropdown > 100 dòng bắt buộc dùng `<MudAutocomplete>` hoặc `<Virtualize>`.
  - Dashboard dùng bảng **Hangfire Snapshot**. Data tĩnh dùng `IMemoryCache`.

---

## PILLAR 4: [AGILE_&_WORKFLOW] - QUẢN LÝ DỰ ÁN & TÀI LIỆU

**[K14] Git Workflow, Auto-Branching & Tech Debt**
- `[SEVERITY]`: 🟡 High
- `THUMB_RULE`: Không dùng trực tiếp main/develop.
- `ACTION`: Tự checkout `feature/` hoặc `bugfix/` chuẩn. Khi để lại Code tạm, phải báo cáo vào `docs/sprints/...`.

**[K15] SQL Version Control (Không Migration)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Dự án áp dụng DB First, KHÔNG dùng EF Migrations.
- `ACTION`: Cập nhật cấu trúc DB bằng file SQL thủ công, đánh số thứ tự (VD: `V1.0.1__...`). Chạy SQL xong mới dùng lệnh EF Core Scaffold.

**[K16] Dependency Version Lock (Khóa phiên bản)**
- `[SEVERITY]`: 🟡 High
- `THUMB_RULE`: Quản lý chặt version thư viện, chống lỗi vỡ code ngầm.
- `ACTION`: Cài package phải dùng Version cụ thể (VD: `--version 8.0.3`), CẤM sử dụng wildcard (`*`).

**[K17] Clean Docs & Schema-Spec Sync (Đồng bộ Đặc tả)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Tài liệu là mạng sống, cấm lệch pha với Code.
- `ACTION`:
  - Khi cấu trúc DB đổi, phải check và update lại Data Dictionary.
  - Ghi nối tiếp (Append-only) vào file markdown. README quá 1 trang A4 phải bị cắt nhỏ.

---

## PILLAR 5: [EXECUTION_&_UI_STANDARDS] - THỰC THI & GIAO DIỆN

**[K18] Runbook, Real Outputs & Deviation Warning**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Hướng dẫn phải thực chiến, có output thật.
- `ACTION`:
  - Runbook phải có Bảng Objectives & Targets.
  - Các bước theo chuẩn: Setup -> Exec -> Verify -> Rollback. Chèn output thật sau khi chạy lệnh.
  - Bước nào rủi ro cao phải có block `[DEVIATION]` (Nếu sai thì ra lỗi gì, cách sửa). RCA (Root Cause) phải đủ 4 trường: Triệu chứng -> Nguyên nhân -> Fix -> Verify.

**[K19] Automation First & Idempotency**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Script chạy 10 lần vẫn an toàn.
- `ACTION`: Dùng đường dẫn tuyệt đối/neo gốc, không dùng relative mù mờ. Mọi bash/powershell script phải có `-Force` hoặc `IF NOT EXISTS`.

**[K20] UI Styling & MudBlazor Centralization**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Cấm Inline Style tĩnh.
- `ACTION`:
  - Khai báo Theme trong `CrmTheme.cs`. Tôn trọng hệ màu OS của User (Dark/Light).
  - Dùng MudBlazor Utility Class (pa-4, d-flex). CSS riêng phải dùng Wrapper + `::deep`. Không `style="..."` trừ khi biến động runtime.
  - Mọi View phải cover 4 States: Loading, Empty, Error, Success.

**[K21] UX & Localization (Bản địa hóa VN)**
- `[SEVERITY]`: 🟡 High
- `THUMB_RULE`: Dùng tiếng Việt chuẩn nghiệp vụ.
- `ACTION`:
  - UI Mobile-first cho Sales, Desktop-first (Nhiều Tab) cho CSKH.
  - Format tiền: VNĐ. Ngày tháng: `dd/MM/yyyy`.
  - Giờ hệ thống tuân thủ: DB lưu `UTC`, C# dùng `UtcNow`, Blazor UI convert sang `GMT+7`.

**[K22] Secret Management & Setup Environment**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Mật khẩu nằm ngoài Source Code.
- `ACTION`:
  - Không push mật khẩu lên git. Dùng User-secrets hoặc Environment Variable.
  - Lệnh `psql` luôn có `-P pager=off` và biến môi trường `PGCLIENTENCODING=UTF8`. Cài đặt 1 lần vĩnh viễn ở User Scope.

---

## PILLAR 6: [DEVOPS_&_TESTING_OPS] - VẬN HÀNH, KIỂM THỬ & THỬ NGHIỆM

**[K23] Testing Strategy & Test Data Hygiene**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Đảm bảo quy tắc kim tự tháp (70/20/10).
- `ACTION`: Unit Test cho Handler. Integration test BẮT BUỘC dùng `TestContainers` (DB tạm). Bắt buộc dùng `Moq` để giả lập API ngoài.

**[K24] Sandbox Experiment & Clean Artifacts (Kiểm chứng Hộp cát & Dọn rác)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Không đoán lỗi chay. Test xong phải xóa rác.
- `ACTION`:
  - Gặp bug khó/logic lạ: Tự sinh file test hoặc cài in-memory package để chạy thử lấy error code thật.
  - Test xong: PHẢI XÓA file test tạm, xóa file log, và REVERT/UNINSTALL các package tạm bợ để trả repo về sự trong sạch.

**[K25] Learning Mode (Chạy 2 lượt & Cố tình làm hỏng)**
- `[SEVERITY]`: 🟡 High
- `THUMB_RULE`: Trải nghiệm để hiểu bản chất sâu xa.
- `ACTION`: Chạy code lượt 1 để ra prototype. Lượt 2, chủ động "sửa sai" code, đổi biến, tắt setting (Controlled Sabotage) xem báo lỗi gì để nắm khái niệm lõi thay vì làm thợ gõ code.

**[K26] Logging & Observability (Giám sát hệ thống)**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Theo dõi sức khỏe hệ thống.
- `ACTION`:
  - Dùng **Serilog** lưu JSON. Sinh **Correlation ID** để trace log cho mỗi request.
  - Tạo endpoint `/health`. Bật cảnh báo (Alert) nếu CPU > 80%, RAM > 90%.

**[K27] Deploy, Backup & Zero-Downtime**
- `[SEVERITY]`: 🔴 Critical
- `THUMB_RULE`: Có đường lùi khi triển khai.
- `ACTION`: Backup DB tự động qua `pg_dump` mỗi đêm. Deploy phải theo nguyên tắc Zero-Downtime (Graceful reload). Luôn có Rollback Plan rõ ràng (Git tag + Restore).

---

## APPENDIX: EXECUTION TEMPLATES (BIỂU MẪU THỰC THI BẮT BUỘC)

### 1. Form Dual-Layer Runbook (Chuẩn K18)
```markdown
### Phase 1: [Tên Phase] (Time budget: X phút)
**[EXEC] - Thực thi:**
1. Chạy lệnh: `bash ...`
2. Output thực tế: `text ...`

**[DEVIATION] - Xử lý sự cố:**
- Lỗi: <Tên lỗi> -> Fix: <Lệnh khắc phục>

**[LEARN] - Khái niệm:**
- Tại sao phải làm bước này: <giải thích>
**[CONCEPT] - Xác nhận:**
- [ ] Đã hiểu khái niệm X.
```

### 2. Form Root Cause Analysis (RCA chuẩn K18)
```markdown
- **Symptom:** Hệ thống văng lỗi X khi...
- **Root Cause:** Do thiếu .AsSplitQuery()...
- **Solution:** Đổi thành .AsSplitQuery()...
- **Verification:** Lệnh test trả về Passed.
```

### 3. Form PowerShell/psql chuẩn (Chuẩn K22)
```powershell
chcp 65001
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:PGCLIENTENCODING = "UTF8"
psql -h localhost -U crm_user -d crm_db -P pager=off -f file.sql
```
