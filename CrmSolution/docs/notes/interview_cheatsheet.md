# INTERVIEW CHEATSHEET — BỬU BỐI PHỎNG VẤN B2B CRM

Tài liệu này được sinh ra từ quá trình áp dụng các kỹ năng `[K79]`, `[K80]`, `[K81]`. Sử dụng nó để "chiến đấu" với các Technical Lead / Software Architects trong các vòng phỏng vấn cho vị trí .NET Backend Engineer (Mid-to-Senior).

---

## 1. ELEVATOR PITCH (TRÌNH BÀY TỐC CHIẾN - K80)
*Khi người phỏng vấn hỏi: "Em hãy kể về dự án CRM em từng làm / Tính năng Khách hàng em thiết kế thế nào?"*

> **Văn mẫu trả lời:**
> "Dạ, trong module quản lý Khách hàng của dự án CRM, mục tiêu của hệ thống là một nền tảng **B2B SaaS (Multi-tenant)** cho phép nhiều công ty thuê chung.
>
> Thay vì dùng Service Pattern CRUD đơn giản, em đã kiến trúc lại toàn bộ theo **CQRS Pattern bằng MediatR** để tách bạch rạch ròi luồng Đọc và Ghi, giúp hệ thống dễ scale.
> Để đảm bảo bảo mật và chống rò rỉ dữ liệu giữa các công ty thuê, em triển khai **phòng thủ IDOR 2 Lớp**: Lớp 1 dùng **EF Core Global Query Filter** chặn từ hạ tầng truy vấn theo `TenantId`, Lớp 2 là **Role-based Ownership Check** ngay tại Handler (Sales chỉ được sửa khách của mình).
>
> Vấn đề Data Integrity khi có nhiều người cùng ghi đè được em xử lý bằng **Optimistic Concurrency với cột hệ thống xmin** đặc thù của PostgreSQL. Cuối cùng, toàn bộ logic này được em **unit test độc lập 100% bằng xUnit và Moq** theo chiến lược Test Pyramid."

---

## 2. DEFENSIVE INTERVIEWING (PHÒNG THỦ & CHẤT VẤN NGƯỢC - K81)

### Q1: CQRS Over-Engineering
**Người phỏng vấn (Chất vấn):** *"Dự án em có vẻ bé, em dùng CQRS + MediatR cho một cái bảng Customer có phải là 'dao mổ trâu giết gà' (Over-engineering) không? Tại sao không dùng 1 cái CustomerService cho lẹ?"*

**Cách trả lời (K79 - Đánh đổi):**
"Dạ em hoàn toàn đồng ý với anh là với các ứng dụng nội bộ 1 công ty (Single-tenant), dùng Service Pattern là đủ tốt và tiết kiệm thời gian (Upfront cost).
Tuy nhiên, định vị của em cho dự án này là **SaaS Platform có độ phức tạp nghiệp vụ cao**. Việc dùng CQRS mang lại 3 lợi thế dài hạn (Long-term ROI) khiến sự 'phức tạp hóa' này xứng đáng:
1. **Cô lập rủi ro (Isolation)**: Em có thể gắn các Behavior Pipeline (như Validation, Logging, Caching) riêng biệt cho từng Command/Query mà không sợ God-Object (Service phình to ngàn dòng code).
2. **Read/Write Asymmetry**: Em cấu hình `.AsNoTracking()` và DTO projection cực mượt ở các Query Handler, trong khi Command Handler tập trung xử lý Transaction và Concurrency.
3. **Phân rã (Decoupling)**: UI layer (Controllers/Blazor) hoàn toàn không biết gì về DbContext hay EF Core, nó chỉ gửi message, giúp em dễ dàng thay tầng Data sau này nếu cần."

---

### Q2: Concurrency & PostgreSQL xmin
**Người phỏng vấn (Chất vấn):** *"Tại sao em không dùng một trường `RowVersion` kiểu `byte[]` hay `int` bình thường để check Concurrency mà lại dùng cái `xmin` của PostgreSQL?"*

**Cách trả lời:**
"Dạ, đây là một điểm tối ưu đặc thù nếu mình dùng PostgreSQL (K19). Trong SQL Server, mình hay dùng kiểu `rowversion` / `timestamp`.
Nhưng ở PostgreSQL, mỗi bảng **mặc định đã có sẵn một system column ẩn tên là `xmin`** (đại diện cho Transaction ID cuối cùng ghi vào dòng đó). Việc em dùng Fluent API cấu hình `.HasColumnType("xid")` map thẳng vào thuộc tính `xmin` giúp em **không cần tạo thêm một cột vật lý nào** trong CSDL mà vẫn tận dụng được cơ chế Optimistic Concurrency Token của EF Core. Nó tiết kiệm dung lượng lưu trữ trên quy mô hàng triệu bản ghi và tối ưu I/O ạ."

---

### Q3: Vấn đề Shadow Properties
**Người phỏng vấn (Chất vấn):** *"Trong Entity Framework Core, làm sao em đảm bảo nó không sinh ra các đoạn query rác hoặc lỗi N+1?"*

**Cách trả lời:**
"Dạ, để chống N+1, em thiết lập luật **Split Query** (thêm `.AsSplitQuery()`) cho mọi câu lệnh đọc có từ 2 `Include()` dạng Collection trở lên.
Còn về câu chuyện truy vấn rác, em cực kỳ khắt khe với **Shadow Properties (Cột ảo)** (K17). Khi map quan hệ 2 chiều (Bidirectional Navigation), nếu mình không khai báo rõ ràng, EF Core rất hay tự chế ra các cột ảo như `TeamId1`, làm sụp hệ thống khi lên Production.
Để chặn đứng việc này, em áp dụng luật (K16): Luôn dùng Fluent API cấu hình rõ `.HasOne().WithMany().HasForeignKey().HasConstraintName()` cho cả 2 đầu Entity. Em thậm chí còn viết một cái **Unit Test dùng Reflection chọc vào DbContext Model** để quét toàn bộ Entity, nếu phát hiện bất kỳ `IsShadowProperty()` nào thì bẻ gãy quá trình Build luôn (Test Fail)."

---

## 3. CHECKLIST SẴN SÀNG (ĐỂ IN RA / ÔN BÀI 5 PHÚT TRƯỚC GIỜ G)

- [ ] Hiểu rõ sự khác biệt giữa Single-tenant (Nội bộ) và Multi-tenant (SaaS).
- [ ] Giải thích trôi chảy từ khóa "Global Query Filter" dùng để chống rò rỉ dữ liệu Tenant.
- [ ] Phân biệt được sự khác nhau giữa IDOR Lớp 1 (Lọc theo Tenant) và Lớp 2 (Lọc theo Role Owner).
- [ ] Nhớ cách đọc `xmin` là "Transaction ID hệ thống của Postgres".
- [ ] Khẳng định được CQRS tuy tốn thời gian setup ban đầu nhưng là kiến trúc tiêu chuẩn cho Microservices và Domain-Driven Design (DDD).
