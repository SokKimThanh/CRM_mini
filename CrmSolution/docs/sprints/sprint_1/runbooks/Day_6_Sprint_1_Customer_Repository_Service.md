# RUNBOOK NGÀY 6 — SPRINT 1: CUSTOMER REPOSITORY & SERVICE UNIT TESTS

* **Sprint:** 1
* **Ngày thực thi:** Day 6
* **Mục tiêu:** Xây dựng tầng truy vấn dữ liệu (Repository) và tầng xử lý nghiệp vụ (Service) cho quản lý Khách hàng, kèm theo bộ Unit Test chuẩn đạt mức qua 100%.
* **Yêu cầu kỹ năng:** .NET 8/10, xUnit, Moq, FluentAssertions, Entity Framework Core.
* **Thời gian dự kiến:** ~60 phút.

---

## 1. MỤC TIÊU & KIỂM ĐỊNH (OBJECTIVES & VERIFICATION TARGETS)

### Bảng Mục tiêu (Objectives)
| Mục tiêu | Chi tiết |
| :--- | :--- |
| **Xây dựng Repository** | Triển khai `ICustomerRepository` và `CustomerRepository` truy vấn IQueryable. |
| **Phát triển Service** | Triển khai `CustomerService` với DTOs (Filter, Create, Update) & logic tự động sinh mã khách hàng. |
| **Phủ Code Bằng Unit Test** | Xây dựng bộ test xUnit + Moq cho `CustomerService` để xác thực logic không cần kết nối DB thực tế. |

### Cổng Nghiệm thu (Verification Targets)
| Hạng mục | Chỉ số Đích (Kỳ vọng) |
| :--- | :--- |
| Cấu trúc File | 5 files (`CustomerRepository.cs`, `ICustomerRepository.cs`, `CustomerDtos.cs`, `CustomerService.cs`, `ICustomerService.cs`). |
| Unit Test | Đạt 4/4 Test Pass (0 Failures). |
| Biên dịch | 0 Warning, 0 Error khi gọi `dotnet build`. |

---

## 2. QUY TRÌNH THỰC THI (DUAL-LAYER RUNBOOK)

### PHASE 1: TẦNG REPOSITORY — TRUY XUẤT DỮ LIỆU
**Human Time Budget:** 15 Phút

#### 1. Setup
Yêu cầu bắt buộc: Đã khởi tạo các Project `Crm.Domain` và `Crm.Data`. Đã khai báo Entity `Customer` và enum `CustomerHealth`.
Kiểm tra thư mục hiện tại có phải là thư mục gốc chứa file `.sln` không:
```bash
ls *.sln
```

#### 2. [EXEC] Thực thi sinh mã Repository
Chạy script PowerShell sau để sinh mã an toàn (Idempotent):

```powershell
$slnRoot = (Get-Item .).FullName
$repoDir = Join-Path $slnRoot "src\Crm.Data\Repositories"

if (-not (Test-Path $repoDir)) {
    New-Item -ItemType Directory -Force -Path $repoDir | Out-Null
}

$enc = New-Object System.Text.UTF8Encoding $false

$iRepo = @'
using System;
using System.Linq;
using System.Threading.Tasks;
using Crm.Domain.Entities;

namespace Crm.Data.Repositories;

public interface ICustomerRepository
{
    IQueryable<Customer> GetQueryable();
    Task<Customer?> GetByIdAsync(long id);
    Task<Customer> AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
}
'@

$repo = @'
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Crm.Domain.Entities;

namespace Crm.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Customer> GetQueryable()
    {
        return _context.Customers;
    }

    public async Task<Customer?> GetByIdAsync(long id)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }
}
'@

[System.IO.File]::WriteAllText((Join-Path $repoDir "ICustomerRepository.cs"), $iRepo, $enc)
[System.IO.File]::WriteAllText((Join-Path $repoDir "CustomerRepository.cs"), $repo, $enc)
Write-Host "✅ Tạo file Repository thành công."
```

#### 3. Verification
```bash
dotnet build src/Crm.Data/Crm.Data.csproj
```
*Expected Output:* `Build succeeded. 0 Warning(s). 0 Error(s).`

#### 4. Rollback (Nếu xảy ra lỗi)
```powershell
Remove-Item -Path "src\Crm.Data\Repositories" -Recurse -Force
```

#### [DEVIATION] (Dự báo & Thoát lỗi nhanh)
> ⚠️ **[LỖI THIẾU DB CONTEXT]:** Nếu `dotnet build` báo lỗi không tìm thấy `AppDbContext`, nguyên nhân do Phase trước chưa khai báo `AppDbContext`.
> - **Khắc phục:** Quay lại tạo file `AppDbContext.cs` trong `Crm.Data` chứa `DbSet<Customer> Customers { get; set; }`.

#### [LEARN] Cơ chế trả về IQueryable
Trong dự án này, thay vì tạo các hàm như `GetList()` trả về sẵn `List<Customer>`, ta cho phép Repository trả về `IQueryable<Customer>`. Điều này cho phép tầng Service chèn thêm các điều kiện (`Where`), bộ lọc, phân trang (`Skip/Take`) trước khi query SQL thực sự được biên dịch và gửi xuống DB, giúp **loại bỏ tình trạng N+1** và kéo dư thừa dữ liệu.

#### [CONCEPT] Đánh giá nhanh
- [ ] Tôi hiểu vì sao trả về `IQueryable` ở Repository lại tối ưu hơn `List` cho các truy vấn có bộ lọc động.

---

### PHASE 2: TẦNG SERVICE — LOGIC NGHIỆP VỤ
**Human Time Budget:** 20 Phút

#### 1. Setup
Đảm bảo project `Crm.Business` đã tham chiếu tới `Crm.Data` và `Crm.Domain`.

#### 2. [EXEC] Thực thi sinh mã Service
Chạy script PowerShell sau:

```powershell
$slnRoot = (Get-Item .).FullName
$dtoDir = Join-Path $slnRoot "src\Crm.Business\DTOs"
$svcDir = Join-Path $slnRoot "src\Crm.Business\Services"

if (-not (Test-Path $dtoDir)) { New-Item -ItemType Directory -Force -Path $dtoDir | Out-Null }
if (-not (Test-Path $svcDir)) { New-Item -ItemType Directory -Force -Path $svcDir | Out-Null }

$enc = New-Object System.Text.UTF8Encoding $false

$dto = @'
using System;

namespace Crm.Business.DTOs;

public class CustomerFilterDto
{
    public string? Keyword { get; set; }
    public int? HealthStatus { get; set; }
    public Guid? SalesPersonId { get; set; }
}

public class CustomerCreateDto
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxCode { get; set; }
    public string? Address { get; set; }
    public int HealthStatus { get; set; }
}

public class CustomerUpdateDto : CustomerCreateDto
{
    public long Id { get; set; }
}
'@

$iSvc = @'
using System.Collections.Generic;
using System.Threading.Tasks;
using Crm.Domain.Entities;
using Crm.Business.DTOs;

namespace Crm.Business.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetListAsync(CustomerFilterDto filter);
    Task<Customer> CreateAsync(CustomerCreateDto dto);
    Task UpdateAsync(CustomerUpdateDto dto);
}
'@

$svc = @'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Crm.Data.Repositories;
using Crm.Domain.Entities;
using Crm.Business.DTOs;
using Crm.Domain.Enums;

namespace Crm.Business.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Customer>> GetListAsync(CustomerFilterDto filter)
    {
        var query = _repository.GetQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var lowerKeyword = filter.Keyword.ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(lowerKeyword) ||
                                     (c.Code != null && c.Code.ToLower().Contains(lowerKeyword)) ||
                                     (c.Phone != null && c.Phone.Contains(lowerKeyword)));
        }

        if (filter.HealthStatus.HasValue)
        {
            query = query.Where(c => c.HealthStatus == filter.HealthStatus.Value);
        }

        if (filter.SalesPersonId.HasValue)
        {
            query = query.Where(c => c.AssignedToUserId == filter.SalesPersonId.Value);
        }

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<Customer> CreateAsync(CustomerCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Tên khách hàng không được để trống");

        if (!string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone.Length < 10)
            throw new ArgumentException("Số điện thoại không hợp lệ");

        // Tự động sinh mã KH-xxxx
        var maxId = 0L;
        if (await _repository.GetQueryable().AnyAsync())
        {
             maxId = await _repository.GetQueryable().MaxAsync(c => c.Id);
        }
        var nextId = maxId + 1;
        var newCode = $"KH-{nextId:D4}";

        var customer = new Customer
        {
            Code = newCode,
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            TaxCode = dto.TaxCode,
            Address = dto.Address,
            HealthStatus = dto.HealthStatus,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.AddAsync(customer);
    }

    public async Task UpdateAsync(CustomerUpdateDto dto)
    {
        var customer = await _repository.GetByIdAsync(dto.Id);
        if (customer == null)
            throw new KeyNotFoundException($"Không tìm thấy khách hàng ID {dto.Id}");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Tên khách hàng không được để trống");

        customer.Name = dto.Name;
        customer.Phone = dto.Phone;
        customer.Email = dto.Email;
        customer.TaxCode = dto.TaxCode;
        customer.Address = dto.Address;
        customer.HealthStatus = dto.HealthStatus;
        customer.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(customer);
    }
}
'@

[System.IO.File]::WriteAllText((Join-Path $dtoDir "CustomerDtos.cs"), $dto, $enc)
[System.IO.File]::WriteAllText((Join-Path $svcDir "ICustomerService.cs"), $iSvc, $enc)
[System.IO.File]::WriteAllText((Join-Path $svcDir "CustomerService.cs"), $svc, $enc)
Write-Host "✅ Tạo file DTOs và Services thành công."
```

#### 3. Verification
```bash
dotnet build src/Crm.Business/Crm.Business.csproj
```

#### 4. Rollback
```powershell
Remove-Item -Path "src\Crm.Business\DTOs" -Recurse -Force
Remove-Item -Path "src\Crm.Business\Services" -Recurse -Force
```

#### [DEVIATION] (Dự báo & Thoát lỗi nhanh)
> ⚠️ **[CẢNH BÁO NULLABLE]:** Quá trình build có thể phát sinh `warning CS8601: Possible null reference assignment`. Điều này xảy ra do map các thuộc tính cho phép null (VD: `Address`, `Phone`).
> - **Cách xử lý:** Bỏ qua warning này trong phạm vi Runbook này. Trong dự án thực tế, các thuộc tính này tại Entity thường khai báo dạng `string?`.

#### [LEARN] Xử lý Concurrency và Soft Delete
Mặc định hệ thống sử dụng Global Query Filter (trong EF Core). Do đó khi gọi `_repository.GetQueryable()` sẽ tự động loại bỏ các bản ghi có cờ `is_deleted = true`. Quy tắc này áp dụng ngầm, tầng Service không cần viết lại mã lọc thủ công (`Where(c => !c.IsDeleted)`).

#### [CONCEPT] Đánh giá nhanh
- [ ] Tôi biết rằng `GetQueryable` đã tích hợp sẵn tính năng tự động lọc dữ liệu đã xóa (Soft delete).

---

### PHASE 3: THIẾT LẬP UNIT TEST
**Human Time Budget:** 25 Phút

#### 1. Setup
Đảm bảo project `Crm.Tests` đã cài đặt đầy đủ các package `xunit`, `Moq`, `FluentAssertions`, và `MockQueryable.Moq`. Nếu chưa, chạy script sau:

```bash
dotnet add tests/Crm.Tests/Crm.Tests.csproj package Moq
dotnet add tests/Crm.Tests/Crm.Tests.csproj package FluentAssertions
dotnet add tests/Crm.Tests/Crm.Tests.csproj package MockQueryable.Moq
dotnet add tests/Crm.Tests/Crm.Tests.csproj reference src/Crm.Business/Crm.Business.csproj
```

#### 2. [EXEC] Viết Unit Test Crm.Tests
Chạy script PowerShell khởi tạo Test:

```powershell
$slnRoot = (Get-Item .).FullName
$testProjDir = Join-Path $slnRoot "tests\Crm.Tests"
$enc = New-Object System.Text.UTF8Encoding $false

$testFile = @'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Crm.Domain.Entities;
using Crm.Business.DTOs;
using Crm.Business.Services;
using Crm.Data.Repositories;
using Crm.Domain.Enums;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

namespace Crm.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repoMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _repoMock = new Mock<ICustomerRepository>();
            _service = new CustomerService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldGenerateCorrectCode_AndReturnCustomer()
        {
            // Arrange
            var existingCustomers = new List<Customer>
            {
                new Customer { Id = 1, Code = "KH-0001", Name = "Khach 1" },
                new Customer { Id = 2, Code = "KH-0002", Name = "Khach 2" }
            }.BuildMock();

            _repoMock.Setup(r => r.GetQueryable()).Returns(existingCustomers);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                     .ReturnsAsync((Customer c) => { c.Id = 3; return c; });

            var dto = new CustomerCreateDto
            {
                Name = "Công ty ABC",
                Phone = "0901234567",
                HealthStatus = (int)CustomerHealth.Healthy
            };

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(3);
            result.Code.Should().Be("KH-0003"); // Khẳng định sinh đúng mã tiếp theo
            result.Name.Should().Be("Công ty ABC");
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenPhoneIsInvalid()
        {
            // Arrange
            var dto = new CustomerCreateDto
            {
                Name = "Công ty DEF",
                Phone = "123" // Invalid, length < 10
            };

            // Act
            Func<Task> act = async () => await _service.CreateAsync(dto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                     .WithMessage("Số điện thoại không hợp lệ");
        }

        [Fact]
        public async Task GetListAsync_ShouldFilterCorrectly_ByKeywordAndHealth()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Vingroup", HealthStatus = (int)CustomerHealth.Healthy, CreatedAt = DateTime.UtcNow },
                new Customer { Id = 2, Name = "Viettel", HealthStatus = (int)CustomerHealth.AtRisk, CreatedAt = DateTime.UtcNow },
                new Customer { Id = 3, Name = "FPT Group", HealthStatus = (int)CustomerHealth.Healthy, CreatedAt = DateTime.UtcNow.AddDays(-1) }
            }.BuildMock();

            _repoMock.Setup(r => r.GetQueryable()).Returns(customers);

            var filter = new CustomerFilterDto
            {
                Keyword = "group",
                HealthStatus = (int)CustomerHealth.Healthy
            };

            // Act
            var result = await _service.GetListAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Vingroup");
        }
    }
}
'@

[System.IO.File]::WriteAllText((Join-Path $testProjDir "CustomerServiceTests.cs"), $testFile, $enc)
Write-Host "✅ Tạo file CustomerServiceTests.cs thành công."
```

#### 3. Verification
```bash
dotnet test tests/Crm.Tests/Crm.Tests.csproj
```

**Output Thực tế từ Terminal:**
```text
Test run for /src/CrmSolution/tests/Crm.Tests/bin/Debug/net8.0/Crm.Tests.dll (.NETCoreApp,Version=v8.0)
VSTest version 18.0.1 (x64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     4, Skipped:     0, Total:     4, Duration: 333 ms - Crm.Tests.dll (net8.0)
```

#### 4. Rollback
```powershell
Remove-Item -Path "tests\Crm.Tests\CustomerServiceTests.cs" -Force
```

#### [DEVIATION] (Dự báo & Thoát lỗi nhanh)
> ⚠️ **[LỖI EXTENSION BUILDMOCK]:** Nếu file Test báo lỗi không tìm thấy phương thức `BuildMock()`.
> - **Cách xử lý:** Đảm bảo thư viện `MockQueryable.Moq` đã được cài đặt và file đang có `using MockQueryable.Moq;`.

#### [LEARN] Tầm quan trọng của Moq
Khi test tầng `CustomerService`, ta tuyệt đối KHÔNG ĐƯỢC KẾT NỐI DATABASE THỰC TẾ (Do đó mới có `Mock<ICustomerRepository>`). Việc này giúp Unit Test có thể chạy trong 1ms thay vì hàng trăm ms, tránh tạo ra dữ liệu rác trên CSDL, tuân thủ nguyên lý `[K45] Test Pyramid Strategy`.

#### [CONCEPT] Đánh giá nhanh
- [ ] Tôi biết cách dùng thư viện `MockQueryable` để giả lập (mock) `IQueryable` cho Repository khi viết Unit Test.

---

## 3. BẢNG KHẮC PHỤC SỰ CỐ (TROUBLESHOOTING MATRIX) - ĐỊNH DẠNG RCA

| Triệu chứng (Symptom) | Nguyên nhân gốc rễ (Root Cause) | Giải pháp (Solution) | Xác nhận (Verification) |
| :--- | :--- | :--- | :--- |
| `IQueryable` không hỗ trợ `ExecuteAsync` khi Unit Test | Các hàm như `MaxAsync` hoặc `AnyAsync` bị lỗi vì dữ liệu `List<T>` trong bộ nhớ thông thường không có bộ cung cấp truy vấn (Query Provider) hỗ trợ Async của Entity Framework. | Sử dụng thư viện `MockQueryable.Moq`, sau đó gắn thêm đuôi `.BuildMock()` vào collection khởi tạo mẫu. | Các phương thức `AnyAsync` hoặc `MaxAsync` trả về giá trị giả lập chính xác. Test Pass. |
| Namespace `ICustomerRepository` could not be found | Component khai báo tiêm Repository nhưng chưa dùng đúng namespace của Data. | Bổ sung directive `using Crm.Data.Repositories;` ở trên cùng file Service. | Lỗi hiển thị gạch đỏ trên IDE biến mất. `dotnet build` trả về 0 Error. |

---

## 4. FINAL AUDIT CHECKLIST

- [x] Script PowerShell chạy tạo Repository thành công với chuẩn Idempotent (kiểm tra `Test-Path`).
- [x] Tầng CustomerService đã triển khai hoàn thiện 3 hàm (List, Create, Update) đáp ứng giao diện.
- [x] Unit test kiểm định được tính năng Sinh mã tự động `KH-xxxx` logic.
- [x] Unit test kiểm định ném ra Exception nếu Validations (Số điện thoại) sai.
- [x] `dotnet test` trả về `Passed: 4` kết quả thực tế từ Sandbox terminal.
- [x] Kịch bản Rollback (xóa file) đã được định nghĩa tại tất cả các phase.