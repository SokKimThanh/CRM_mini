# AGENTS.md - HỆ ĐIỀU HÀNH TƯ DUY CHO AI (JULES)

Đây là tài liệu bắt buộc (Mandatory) dành cho bất kỳ AI/Agent nào khi làm việc trong dự án này.
Mọi hành động sinh code, chat, tạo nhánh phải tuân thủ nghiêm ngặt 5 Trụ cột (Pillars) dưới đây.

---

## PILLAR 1: [AI_META_COGNITION] - HỆ ĐIỀU HÀNH TƯ DUY KỸ SƯ

**[K00] Master Skill Orchestration (Liên kết đa nhiệm tối cao)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: KHÔNG BAO GIỜ dùng 1 kỹ năng đơn lẻ. Mọi phản hồi phải là một chuỗi hành động xuyên suốt 5 Pillars.
- `TRIGGER`: Nhận mọi task từ User.
- `ACTION`: Nhận Task -> Bật Tư duy Kỹ sư [P1] -> Thẩm định Nghiệp vụ CRM [P2] -> Code tuân thủ Kiến trúc/Bảo mật [P3] -> Sinh Runbook/Test [P5] -> Commit & Cập nhật Docs [P4].

**[K01] 10 Bước Tư Duy Bắt Buộc (Core Engineering Mindset)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: KHÔNG BAO GIỜ viết nháp code (Drafting) ngay lập tức. Phải chạy qua 10 bước màng lọc tư duy trước khi sinh code.
- `TRIGGER`: Nhận mọi task từ User.
- `ACTION`:
  1. **Requirement Analysis:** Hiểu tại sao phải làm tính năng này.
  2. **Domain Modeling:** Nhận diện đối tượng nghiệp vụ (Lead, Contact, Account...).
  3. **Business Process Analysis:** Đặt tính năng vào chuỗi quy trình kinh doanh.
  4. **Task Breakdown:** Chia nhỏ công việc thành các bước thực thi < 30 phút.
  5. **Change Impact Analysis:** Đánh giá rủi ro ảnh hưởng đến module khác hoặc database hiện tại.
  6. **Deep Audit:** Rà soát lỗ hổng tiềm ẩn.
  7. **Decision Making / Trade-off Analysis:** Phân tích ưu/nhược và đề xuất phương án tối ưu nhất cho CRM.
  8. **Validation & Verification:** Tự thiết lập checkpoints để nghiệm thu.
  9. **Self-Review:** Tự kiểm tra chéo (Cross-check) code của chính mình.
  10. **Lesson Learned:** Ghi chú lại lỗi và bài học sau khi hoàn thành.

**[K02] System Thinking & Auto-Prompting (Tư Duy Hệ Thống - "Nói ít hiểu nhiều")**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không làm bừa khi chưa rõ Context.
- `TRIGGER`: Nhận yêu cầu chung chung.
- `ACTION`: Đọc định hướng từ `docs/`, đánh giá tiến độ Sprint. Tự động sinh ra các Actionable Prompts cho User.

---

## PILLAR 2: [CRM_DOMAIN_EXPERTISE] - NGHIỆP VỤ B2B CRM

**[K03] Khung Quản lý Đối tượng (CRM Domain)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Phân định rạch ròi các đối tượng cốt lõi.
- `TRIGGER`: Phân tích nghiệp vụ CRM.
- `ACTION`:
  - Hiểu rõ sự khác biệt giữa **Lead** (Tiềm năng chưa định danh), **Contact** (Người liên hệ), **Account** (Khách hàng doanh nghiệp), **Opportunity** (Cơ hội bán hàng).
  - Đưa mọi Opportunity vào một **Sales Pipeline** cụ thể.

**[K04] Quy trình và Hành trình Khách hàng (CRM Operations)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không thiết kế luồng cụt.
- `TRIGGER`: Code các chức năng liên quan đến chăm sóc khách hàng.
- `ACTION`:
  - Luôn tính đến **Lead Assignment** (Chia số tự động).
  - Có các quy tắc **Follow-up** (Chăm sóc) rõ ràng.
  - Vẽ **Customer Journey** (Từ lúc là Lead đến khi thành Khách hàng trung thành).
  - Hiểu luồng vòng đời: **Quote → Order → Invoice**.

**[K05] Tự động hóa và Cảnh báo (CRM Automation & SLA)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: B2B CRM phải có cảnh báo tự động.
- `TRIGGER`: Thiết kế Job, Nhắc nhở.
- `ACTION`: Cấu hình **SLA** (Thời gian cam kết phản hồi). Nếu trễ phải có **Escalation** (Leo thang cảnh báo lên Quản lý) thông qua **Auto Workflow**.

**[K06] Đo lường và Báo cáo (CRM KPI & Reporting)**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Dữ liệu sinh ra là để đo lường.
- `TRIGGER`: Thiết kế Database, cập nhật Status.
- `ACTION`: Lưu giữ vết thay đổi để tính được **Conversion Rate** (Tỷ lệ chuyển đổi), **Win Rate** (Tỷ lệ thắng), **Revenue Forecast** (Dự báo doanh thu). Thiết kế bảng lịch sử để phục vụ **Funnel Report** và **Retention Report**.

---

## PILLAR 3: [ENTERPRISE_STANDARDS] - CHUẨN KIẾN TRÚC, BẢO MẬT & HIỆU NĂNG

**[K07] Phân Quyền Sở Hữu & Lưu Vết (Data Ownership & Audit Trail)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Không thiết kế entity vô chủ.
- `TRIGGER`: Cấu trúc Database.
- `ACTION`:
  - Mọi thực thể kinh doanh (Lead, Account, Quote) PHẢI có `OwnerId` và `TeamId`. Phân quyền đệ quy theo cấp bậc phòng ban.
  - Bắt buộc có **Entity Audit Trail** (Lịch sử thay đổi: Ai đổi, đổi từ gì sang gì, khi nào).

**[K08] CQRS Strict Boundaries & Async Jobs**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Phân tách rạch ròi nhiệm vụ trong CQRS. Không block UI bằng tác vụ ngoài.
- `TRIGGER`: Code API, Handlers, Tích hợp bên thứ 3.
- `ACTION`:
  - Controllers/Pages chỉ gửi Command/Query.
  - Repositories CHỈ dùng cho CRUD.
  - Toàn bộ Business Logic phải nằm trong Handlers.
  - Gửi Email, Đồng bộ dữ liệu (KiotViet) PHẢI dùng **Outbox Pattern / Background Jobs (Hangfire)**. Không chạy đồng bộ.

**[K09] Ngăn chặn Lỗ hổng Bảo mật (IDOR & XSS)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: 1
- `THUMB_RULE`: Không bao giờ tin tưởng Input và ID từ Client.
- `TRIGGER`: Truy vấn dữ liệu theo ID, hiển thị Text.
- `ACTION`:
  - Ngừa **IDOR**: Luôn check `OwnerId` hoặc quyền trong Handlers trước khi `GetById(id)` hoặc `Update(id)`.
  - Ngừa **XSS**: Dùng Sanitizer lọc mã độc trước khi render HTML từ Note/Email của người dùng trong Blazor.

**[K10] Tối ưu Hiệu năng EF Core & UI (Performance & Cartesian Explosion)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Tránh làm nổ RAM Server.
- `TRIGGER`: Query DB nhiều bảng, render UI danh sách.
- `ACTION`:
  - Truy vấn đọc có >= 2 `Include` bắt buộc dùng `.AsSplitQuery()`.
  - Danh sách thả xuống > 100 dòng bắt buộc dùng `<MudAutocomplete>` hoặc `<Virtualize>` thay vì `<MudSelect>` thường.
  - Dùng **Global Query Filter** cho Soft Delete. Không dùng N+1 Query.

---

## PILLAR 4: [AGILE_&_WORKFLOW] - QUẢN LÝ DỰ ÁN & TÀI LIỆU

**[K11] Git Workflow & Tech Debt**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Rõ ràng tiến trình, không để rác vào repo.
- `TRIGGER`: Mọi thao tác git.
- `ACTION`: Tự động branching chuẩn (`feature/`, `bugfix/`). Dùng Conventional Commits. Cuối task tự ghi nhận Tech Debt vào `docs/sprints/...`.

**[K12] SQL Version Control (Không Migration)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Dự án áp dụng DB First, KHÔNG dùng EF Migrations. Cấm chạy script loạn xạ.
- `TRIGGER`: Cập nhật cấu trúc Database.
- `ACTION`: Tạo script SQL cập nhật và **đánh số version** (VD: `V1.0.1__Create_Lead_Table.sql`). Chạy script xong mới dùng EF Core Scaffold.

**[K13] Clean Docs & Schema-Spec Sync (Nói ít làm nhiều)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Tài liệu không được mâu thuẫn với Code/Schema. README luôn < 1 trang.
- `TRIGGER`: Cập nhật DB, thay đổi luồng.
- `ACTION`:
  - Luôn đối chiếu DDL SQL với Data Dictionary. Nếu lệch, cập nhật Docs.
  - Áp dụng Append-only cho tài liệu (chỉ thêm mới, không đè trừ khi refactor).
  - Cô lập tài liệu cũ vào `docs/archive/`. Cắt nhỏ README nếu quá dài.

---

## PILLAR 5: [EXECUTION_&_RUNBOOK] - QUY CHUẨN THỰC THI & GIAO DIỆN

**[K14] Step-by-Step Runbook & Real Outputs**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Runbook phải dùng được ngay, có bằng chứng.
- `TRIGGER`: Sinh tài liệu hướng dẫn/Runbook.
- `ACTION`:
  - Phải có bảng Mục tiêu & Xác nhận.
  - Theo trình tự Setup -> Execution -> Verification -> Rollback.
  - Chèn Output THỰC TẾ (Real Output) sau khi chạy thử, không viết output kỳ vọng suông.
  - Cung cấp sẵn Troubleshooting Matrix.
  - RCA (Root Cause Analysis) 4 bước cho mọi lỗi.

**[K15] Centralized UI Styling (MudBlazor)**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Cấm Inline Style tĩnh.
- `TRIGGER`: Thiết kế UI `.razor`.
- `ACTION`:
  - Khai báo Theme trong `CrmTheme.cs`. Tôn trọng Dark/Light mode của OS.
  - Chỉ dùng MudBlazor Utility Classes (Utility-First).
  - Khi cần override CSS, dùng Root HTML Wrapper và `::deep`. Không dùng `style="..."` trừ phi là biến động.
  - Phân định rõ UX của Sales (Mobile-first, thao tác nhanh) vs UX của CSKH (Desktop, nhiều thông tin).

**[K16] Secret Management & Environment Consistency**
- `[SEVERITY]`: 🔴 Critical
- `[SPRINT]`: All
- `THUMB_RULE`: Bảo mật và Idempotency (Khả lặp).
- `TRIGGER`: Script cấu hình, psql.
- `ACTION`:
  - Không nhét password vào source code (dùng Secret/Env variables).
  - `psql` luôn dùng `-P pager=off` và `PGCLIENTENCODING=UTF8`.
  - Mọi script phải có `-Force`, `IF NOT EXISTS` để có thể chạy nhiều lần không lỗi.
  - Giữ nguyên tắc giờ hệ thống: DB `UTC`, Code `UtcNow`, UI convert `GMT+7`.

**[K17] Clean Test Artifacts & Sandbox Experimentation**
- `[SEVERITY]`: 🟡 High
- `[SPRINT]`: All
- `THUMB_RULE`: Không đoán lỗi, không để lại rác.
- `TRIGGER`: Debug lỗi, thử nghiệm Kỹ thuật mới.
- `ACTION`: Tự sinh file test nhỏ để chạy thực tế -> Dọn sạch file test, log, thư viện tạm sau khi xong.

---

## APPENDIX: EXECUTION TEMPLATES (BIỂU MẪU THỰC THI BẮT BUỘC)

Để đảm bảo khả năng thực thi (Execution) chính xác 100%, mọi Agent phải sử dụng các biểu mẫu sau khi sinh tài liệu/code:

### 1. Form Dual-Layer Runbook (Chuẩn K14)
Mỗi Phase trong Runbook bắt buộc tuân theo cấu trúc:
```markdown
### Phase 1: [Tên Phase] (Time budget: X phút)

**[EXEC] - Thực thi:**
1. Chạy lệnh:
   ` ` `bash
   <lệnh>
   ` ` `
2. Output thực tế (Real Output):
   ` ` `text
   <copy output thật vào đây>
   ` ` `

**[DEVIATION] - Xử lý sự cố:**
- Lỗi: <Tên lỗi>
- Fix: <Lệnh khắc phục>

**[LEARN] - Khái niệm:**
- Tại sao phải làm bước này: <giải thích ngắn gọn>

**[CONCEPT] - Xác nhận:**
- [ ] Đã hiểu khái niệm X.
```

### 2. Form Root Cause Analysis (RCA chuẩn K14)
Khi báo cáo lỗi/Tech Debt, phải điền đủ 4 mục:
```markdown
- **Symptom (Triệu chứng):** Hệ thống văng lỗi X khi bấm nút Y.
- **Root Cause (Nguyên nhân):** Do query thiếu .AsSplitQuery() làm tràn RAM.
- **Solution (Giải pháp):** Đổi thành .AsSplitQuery() trong Handler.
- **Verification (Nghiệm thu):** Lệnh test `dotnet test` trả về Passed.
```

### 3. Form PowerShell/psql chuẩn (Chuẩn K16)
```powershell
chcp 65001
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:PGCLIENTENCODING = "UTF8"
psql -h localhost -U crm_user -d crm_db -P pager=off -f file.sql
```
