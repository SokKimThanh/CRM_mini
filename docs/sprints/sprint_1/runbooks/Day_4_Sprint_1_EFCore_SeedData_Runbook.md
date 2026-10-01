# TÀI LIỆU HƯỚNG DẪN THỰC THI (RUNBOOK) - NGÀY 4

**Dự án:** CRM B2B System
**Giai đoạn:** Sprint 1 — Nền tảng kỹ thuật & Khách hàng 360
**Tác giả:** Trợ lý AI (Tuân thủ Plain Language & Block 1/AGILE_WORKFLOW)
**Thời lượng thực thi:** 120 Phút

---

## 🎯 TỔNG QUAN MỤC TIÊU NGÀY 4

1. **Đồng bộ Database-First (Scaffold):** Tự động sinh mã C# (Entities) từ 16 bảng nghiệp vụ PostgreSQL đã tạo ở Ngày 2, ĐẢM BẢO KHÔNG ghi đè lên các bảng Identity đã làm ở Ngày 3.
2. **Cấu hình AppDbContext (Cấu hình liên kết DB):** Cấu hình Entity Framework Core sử dụng Fluent API, bật tính năng tự động chuyển đổi thời gian sang chuẩn UTC.
3. **Tạo dữ liệu mẫu (Seed Data):** Viết class `DbInitializer` để tự động tạo dữ liệu mẫu phong phú (Khách hàng, Sản phẩm, Liên hệ, Cơ hội, Nhiệm vụ) giúp việc test UI thuận lợi.

---

## ⏱ KẾ HOẠCH THỰC THI CHI TIẾT (120 PHÚT)

### PHASE 1 — TẠO ENTITY TỪ DATABASE (SCAFFOLD) (20 phút)

**Mục tiêu:** Kéo cấu trúc 16 bảng từ Database về Code C# thành các class Model, nhưng loại bỏ các bảng liên quan đến Phân quyền (Auth) đã làm.

**Lệnh thực thi trong Bash/Terminal:**

```bash
cd src/Crm.Data

# Lưu ý: Chạy lệnh này sẽ tự động tạo file trong thư mục Entities (hoặc thư mục gốc của Data tùy cấu hình)
# Cờ --table giúp CỤ THỂ những bảng cần kéo, tránh kéo bảng AspNetRoles, AspNetUsers... làm hỏng code cũ
dotnet ef dbcontext scaffold "Host=localhost;Database=crm_db;Username=crm_user;Password=YourPassword" Npgsql.EntityFrameworkCore.PostgreSQL \
  -o Entities \
  --context AppDbContext \
  -f \
  --table categories \
  --table contacts \
  --table customer_assignments \
  --table customers \
  --table interactions \
  --table notifications \
  --table opportunities \
  --table opportunity_stages \
  --table products \
  --table quote_items \
  --table quotes \
  --table sales_tasks \
  --table stage_histories \
  --table dashboard_snapshots \
  --table audit_logs \
  --table kiotviet_sync_logs
```

*Cảnh báo (Block 2 - Tech Debt): Không bao giờ dùng cờ kéo toàn bộ DB (không có --table) vì nó sẽ gây lỗi trùng lặp với IdentityDbContext đã khai báo ngày hôm qua.*

---

### PHASE 2 — CẤU HÌNH APP-DB-CONTEXT & CHUẨN HOÁ THỜI GIAN (30 phút)

**Mục tiêu:** Làm sạch file `AppDbContext.cs` vừa được sinh ra, bỏ bớt cấu hình chuỗi kết nối bị fix cứng (Hardcode) và thêm tính năng ép múi giờ về UTC.

**Bước 2.1: Chỉnh sửa `AppDbContext.cs`**
- Mở file `src/Crm.Data/AppDbContext.cs`.
- Xóa hàm `OnConfiguring` (nếu có dòng chứa chuỗi kết nối `Host=localhost...`). Việc này để tuân thủ Block 5 (Cấm Hardcode, cấu hình phải lấy từ appsettings.json).
- Đảm bảo class `AppDbContext` kế thừa từ `IdentityDbContext<ApplicationUser, ApplicationRole, Guid>` (như đã cấu hình ở Ngày 3).

**Bước 2.2: Thêm đoạn code ép kiểu Thời gian thành UTC**
Entity Framework thường cảnh báo nếu lưu ngày giờ không có múi giờ (Timezone). Ta dùng đoạn code sau trong hàm `OnModelCreating` để tự động biến mọi `DateTime` thành `UTC`.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder); // Bắt buộc gọi base cho Identity

    // Giữ nguyên các cấu hình Fluent API (modelBuilder.Entity<...>) mà công cụ Scaffold vừa sinh ra ở đây...

    // CHUẨN HÓA UTC DATE TIME
    // Duyệt qua tất cả các Entity, nếu trường nào là DateTime thì ép thành UTC trước khi lưu vào DB
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        foreach (var property in entityType.GetProperties())
        {
            if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
            {
                property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                    v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
            }
        }
    }
}
```

---

### PHASE 3 — VIẾT FILE TẠO DỮ LIỆU MẪU (DbInitializer.cs) (50 phút)

**Mục tiêu:** Tạo bộ dữ liệu mẫu đa dạng phong phú cho hệ thống để test tính năng Kanban, Danh sách Khách hàng.

- Tạo file mới: `src/Crm.Data/Seeders/DbInitializer.cs`.

```csharp
using Crm.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crm.Data.Seeders
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 1. Kiểm tra nếu có Khách hàng rồi thì không tạo nữa (Tránh duplicate khi khởi động app nhiều lần)
            if (await context.Customers.AnyAsync())
            {
                return;
            }

            // 2. Tạo 20 Khách hàng (Minh họa vài khách hàng cơ bản, các bạn copy thêm cho đủ 20 nhé)
            var customers = new List<Customer>
            {
                new Customer { Code = "KH-001", Name = "Công ty TNHH Cơ Khí An Phát", Industry = "Cơ khí", Health = 1 /* Good */, CreatedAt = DateTime.UtcNow },
                new Customer { Code = "KH-002", Name = "CTCP Đầu tư Xây dựng Hòa Bình", Industry = "Xây dựng", Health = 2 /* Fair */, CreatedAt = DateTime.UtcNow },
                new Customer { Code = "KH-003", Name = "Nhà máy Nhựa Chợ Lớn", Industry = "Nhựa", Health = 3 /* AtRisk */, CreatedAt = DateTime.UtcNow },
                new Customer { Code = "KH-004", Name = "Gara Ô Tô Thanh Phong", Industry = "Dịch vụ", Health = 4 /* Churned */, CreatedAt = DateTime.UtcNow }
                // ... Thêm 16 khách hàng nữa để đạt KPI
            };
            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();

            // 3. Tạo 20 Sản phẩm
            var products = new List<Product>
            {
                new Product { Code = "SP-001", Name = "Dao phay CNC HSS 10mm", Unit = "Cái", BasePrice = 450000 },
                new Product { Code = "SP-002", Name = "Đá mài hợp kim 150x20", Unit = "Viên", BasePrice = 120000 },
                new Product { Code = "SP-003", Name = "Dầu cắt gọt pha nước", Unit = "Can 20L", BasePrice = 1850000 }
                // ...
            };
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // 4. Tạo 15 Cơ hội bán hàng (Sales Pipeline)
            var stages = await context.OpportunityStages.ToListAsync();
            var opps = new List<Opportunity>
            {
                new Opportunity { Code = "OPP-001", CustomerId = customers[0].Id, StageId = stages[0].Id, Title = "Đơn hàng dao phay quý 3", EstimatedValue = 15000000, ExpectedCloseDate = DateTime.UtcNow.AddDays(15) },
                new Opportunity { Code = "OPP-002", CustomerId = customers[1].Id, StageId = stages[1].Id, Title = "Hợp đồng cấp vật tư 1 năm", EstimatedValue = 450000000, ExpectedCloseDate = DateTime.UtcNow.AddDays(45) }
                // ... Phân bổ đều các Stage
            };
            await context.Opportunities.AddRangeAsync(opps);
            await context.SaveChangesAsync();

            // 5. Tạo 30 Nhiệm vụ chăm sóc
            var tasks = new List<SalesTask>
            {
                new SalesTask { Title = "Gọi lại hỏi thăm", CustomerId = customers[0].Id, Status = 0 /* Pending */, DueDate = DateTime.UtcNow.AddDays(1) },
                new SalesTask { Title = "Gửi Catalogue mẫu mới", CustomerId = customers[1].Id, Status = 1 /* Completed */, DueDate = DateTime.UtcNow.AddDays(-1) }
            };
            await context.SalesTasks.AddRangeAsync(tasks);
            await context.SaveChangesAsync();
        }
    }
}
```

---

### PHASE 4 — KÍCH HOẠT SEED DATA (20 phút)

**Mục tiêu:** Cấu hình để mỗi khi chạy ứng dụng `dotnet run`, hệ thống sẽ tự động gọi file `DbInitializer.cs` để bơm dữ liệu vào Database.

- Mở file `src/Crm.Web/Program.cs` và thêm đoạn code sau trước hàm `app.Run();`:

```csharp
// TỰ ĐỘNG MIGRATE VÀ SEED DỮ LIỆU KHI CHẠY APP
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Nếu DB chưa có, hàm này sẽ cập nhật các Migration mới nhất
        await context.Database.MigrateAsync();

        // Gọi hàm tạo dữ liệu mẫu
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi trong quá trình khởi tạo Database.");
    }
}
```

---

## 🛠 BẢNG TỔNG KẾT 5 LỖI THƯỜNG GẶP (POST-MORTEM)

| # | Triệu chứng lỗi (Error Message) | Nguyên nhân | Biện pháp xử lý |
|---|---|---|---|
| 1 | `Cannot use multiple DbContext instances...` khi chạy Scaffold | Không chỉ định bảng cụ thể nên EF Core kéo đè lên bảng của Identity. | Phải dùng các tham số `--table` khi gõ lệnh `Scaffold`. |
| 2 | `Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone'` | Do lưu thẳng `DateTime.Now` chưa có múi giờ. | EF Core 8 yêu cầu UTC. Viết vòng lặp Converter UTC trong `OnModelCreating` (Xem Phase 2). |
| 3 | Lỗi vi phạm Khóa Ngoại (Foreign Key Constraint) khi Seed Data | Hàm `SeedAsync` chèn `Opportunity` nhưng `CustomerId` chưa tồn tại trong DB. | Tuân thủ thứ tự sinh dữ liệu: Bảng Cha (Customer, Stage) sinh trước, bảng Con (Opportunity, Task) sinh sau. Gọi `SaveChanges` tuần tự. |
| 4 | DB có nhiều dữ liệu lặp (Duplicate Data) | Khởi động app nhiều lần hàm Seed chạy nhiều lần. | Ở đầu file `DbInitializer` luôn kiểm tra: `if (context.Customers.Any()) return;`. |
| 5 | Compile Error `CS0103 The name 'DateTimeKind' does not exist...` | Thiếu using không gian tên (namespace). | Đảm bảo có `using System;` ở đầu file. |

---

## 📝 CHECKLIST NGHIỆM THU NGÀY 4 (DoD)

- [ ] Lệnh Scaffold đã chạy thành công, thư mục `Entities` có đúng 16 class nghiệp vụ.
- [ ] KHÔNG TỒN TẠI các file như `AspNetUser.cs` sinh ra trong thư mục Entities (Identity được giữ nguyên vẹn của Ngày 3).
- [ ] File `AppDbContext.cs` không chứa chuỗi kết nối (`Host=localhost...`).
- [ ] Hàm Convert DateTime sang UTC đã nằm trong `OnModelCreating`.
- [ ] Khởi chạy app (`dotnet run`), kiểm tra trong pgAdmin / DBeaver bằng lệnh `SELECT count(*) FROM customers;` thấy trả về số lượng $\ge 20$ bản ghi.
- [ ] Thời gian được lưu vào bảng `Customers.CreatedAt` đang ở định dạng giờ UTC chuẩn (không lỗi Timezone).
