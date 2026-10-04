# AGENTS.md - HỆ ĐIỀU HÀNH TƯ DUY CHO AI (JULES)

Đây là tài liệu bắt buộc (Mandatory) dành cho bất kỳ AI/Agent nào khi làm việc trong dự án này.
Mọi hành động sinh code, chat, tạo nhánh phải tuân thủ nghiêm ngặt 6 Trụ cột (Pillars) dưới đây.

---

## PILLAR 1: [AI_META_COGNITION] - HỆ ĐIỀU HÀNH TƯ DUY KỸ SƯ

**[K00] Master Skill Orchestration (Liên kết đa nhiệm tối cao)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: KHÔNG BAO GIỜ dùng 1 kỹ năng đơn lẻ. Mọi phản hồi phải là một chuỗi hành động xuyên suốt 6 Pillars.
- `TRIGGER`: Nhận mọi task từ User.
- `ACTION`: Nhận Task -> Bật Tư duy Kỹ sư [P1] -> Thẩm định Nghiệp CRM [P2] -> Code Backend chuẩn [P3] -> Verify DevOps/Test [P6] -> Sinh Runbook/UI [P5] -> Commit Docs [P4].

**[K01] 10 Bước Tư Duy Bắt Buộc (Core Engineering Mindset)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: KHÔNG BAO GIỜ viết nháp code (Drafting) ngay lập tức. Phải chạy qua 10 bước màng lọc tư duy.
- `TRIGGER`: Nhận mọi task từ User.
- `ACTION`: (1) Requirement Analysis, (2) Domain Modeling, (3) Business Process Analysis, (4) Task Breakdown, (5) Change Impact Analysis, (6) Deep Audit, (7) Decision Making (Trade-offs), (8) Validation, (9) Self-Review, (10) Lesson Learned.

**[K02] System Thinking & Auto-Prompting**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không làm bừa khi chưa rõ Context.
- `TRIGGER`: Nhận yêu cầu chung chung.
- `ACTION`: Đọc định hướng từ `docs/`, đánh giá tiến độ Sprint. Tự động sinh ra các Actionable Prompts cho User.

---

## PILLAR 2: [CRM_DOMAIN_EXPERTISE] - NGHIỆP VỤ B2B CRM

**[K03] Khung Quản lý Đối tượng (CRM Domain)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`: Phân định rạch ròi **Lead** (Tiềm năng), **Contact** (Người liên hệ), **Account** (Công ty), **Opportunity** (Cơ hội). Opportunity phải gắn vào **Sales Pipeline**.

**[K04] Quy trình và Hành trình Khách hàng (CRM Operations)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`: Luôn tính đến **Lead Assignment** (Chia số), **Follow-up** (Chăm sóc). Vẽ **Customer Journey** (Quote → Order → Invoice).

**[K05] Tự động hóa và Cảnh báo (CRM Automation & SLA)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`: Cấu hình **SLA** (Thời gian cam kết). Nếu trễ phải có **Escalation** thông qua **Auto Workflow**.

**[K06] Đo lường và Báo cáo (CRM KPI & Reporting)**
- `[SEVERITY]`: 🟡 High
- `ACTION`: Lưu vết để tính **Conversion Rate**, **Win Rate**, **Revenue Forecast**. Phục vụ **Funnel Report** và **Retention Report**.

---

## PILLAR 3: [ENTERPRISE_BACKEND_&_SECURITY] - CHUẨN KIẾN TRÚC & BẢO MẬT

**[K07] Phân Quyền Sở Hữu & Concurrency (Data Integrity)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - Mọi thực thể kinh doanh PHẢI có `OwnerId` và `TeamId`. Phân quyền đệ quy. Bắt buộc có **Entity Audit Trail**.
  - Update quan trọng phải dùng `row_version` (xmin) làm Concurrency Token chặn ghi đè.

**[K08] CQRS, Async Jobs & Caching**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - Controllers/Pages chỉ gửi Command/Query. Repositories CHỈ dùng CRUD. Business Logic ở Handlers.
  - Gọi KiotViet/Email PHẢI dùng **Outbox Pattern / Hangfire**.
  - Dashboard dùng **Hangfire Snapshot**. Data tĩnh dùng `IMemoryCache` (Có TTL và Invalidate).

**[K09] Bảo Mật, Lỗi & Logging (Security & Observability)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - **IDOR**: Luôn check `OwnerId` trong Handler trước khi `GetById/Update`.
  - **XSS**: Sanitizer lọc mã độc khi render Note/Email trong Blazor.
  - **Logging**: Dùng **Serilog** (JSON). Pipeline phải sinh **Correlation ID**. Global Exception Handler không để rò rỉ lỗi.

**[K10] Tối ưu Hiệu năng EF Core & UI (Performance)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - Truy vấn >= 2 `Include` bắt buộc dùng `.AsSplitQuery()`.
  - Không N+1. Dùng Global Query Filter cho Soft Delete.
  - UI Danh sách > 100 dòng bắt buộc dùng `<MudAutocomplete>` hoặc `<Virtualize>`.

---

## PILLAR 4: [AGILE_&_WORKFLOW] - QUẢN LÝ DỰ ÁN & TÀI LIỆU

**[K11] Git Workflow & Tech Debt**
- `[SEVERITY]`: 🟡 High
- `ACTION`: Tự động branching (`feature/`, `bugfix/`). Dùng Conventional Commits. Cuối task tự ghi nhận Tech Debt vào `docs/sprints/`.

**[K12] SQL Version Control (Không Migration)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`: Áp dụng DB First, KHÔNG dùng EF Migrations. Cập nhật bằng file SQL đánh số version (VD: `V1.0.1__Create.sql`) trước khi EF Scaffold.

**[K13] Clean Docs & Schema-Spec Sync (Nói ít làm nhiều)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`: Đối chiếu DDL SQL với Data Dictionary. Áp dụng Append-only cho tài liệu (cô lập file cũ). Cắt nhỏ README < 1 trang.

---

## PILLAR 5: [EXECUTION_&_UI_STANDARDS] - THỰC THI & GIAO DIỆN

**[K14] Runbook, Real Outputs & Deviation**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - Runbook có Bảng Mục tiêu. Chèn Output THỰC TẾ. RCA 4 bước.
  - Bắt buộc có cảnh báo **[DEVIATION]** (Nếu làm sai thì sinh lỗi gì) tại các bước quan trọng.

**[K15] UI Styling, 4 States & Localization (MudBlazor)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - Khai báo Theme tập trung. Cấm Inline Style tĩnh, dùng Utility-first và `::deep`.
  - Mọi màn hình phải có 4 states: Loading, Empty, Error, Success.
  - Bản địa hóa chuẩn VN (Tiền tệ VNĐ, ngày `dd/MM/yyyy`). UX tách biệt Mobile (Sales) và Desktop (CSKH).

**[K16] Secret Management & Environment Consistency**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - Không commit password (dùng User Secrets/Env).
  - Lệnh `psql` luôn có `-P pager=off`, UTF8. Giờ hệ thống: DB `UTC`, Code `UtcNow`, UI `GMT+7`.

---

## PILLAR 6: [DEVOPS_&_TESTING_OPS] - VẬN HÀNH & KIỂM THỬ

**[K17] Testing Strategy (Pyramid 70/20/10)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - Ưu tiên Unit Test (70%). Integration Test (20%) phải dùng **TestContainers** (Database tạm cách ly). E2E (10%).
  - Bắt buộc mock các service ngoài (Moq KiotViet).
  - Sandbox test xong phải xóa rác (Clean Artifacts).

**[K18] Deploy, Backup & Health (Zero-Downtime)**
- `[SEVERITY]`: 🔴 Critical
- `ACTION`:
  - Phải có Rollback Plan (Git tag, DB restore) cho mọi deploy.
  - Setup Backup Database định kỳ (pg_dump ban đêm).
  - Triển khai Zero-Downtime (Graceful reload, migrate trước code sau).
  - Setup `/health` check và Monitor cảnh báo (CPU > 80%, RAM > 90%).

---

## APPENDIX: EXECUTION TEMPLATES (BIỂU MẪU THỰC THI BẮT BUỘC)

### 1. Form Dual-Layer Runbook (Chuẩn K14)
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

### 2. Form Root Cause Analysis (RCA chuẩn K14)
```markdown
- **Symptom:** Hệ thống văng lỗi X khi...
- **Root Cause:** Do thiếu .AsSplitQuery()...
- **Solution:** Đổi thành .AsSplitQuery()...
- **Verification:** `dotnet test` trả về Passed.
```

### 3. Form PowerShell/psql chuẩn (Chuẩn K16)
```powershell
chcp 65001
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:PGCLIENTENCODING = "UTF8"
psql -h localhost -U crm_user -d crm_db -P pager=off -f file.sql
```
