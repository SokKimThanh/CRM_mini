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
| **Phát triển Service** | Triển khai `CustomerService` với DTOs (Filter, Create, Update) & tự sinh mã KH. |
| **Phủ Code Bằng Unit Test** | Xây dựng bộ test xUnit + Moq cho `CustomerService` để xác thực logic. |

### Cổng Nghiệm thu (Verification Targets)
| Hạng mục | Chỉ số Đích (Kỳ vọng) |
| :--- | :--- |
| Cấu trúc File | 5 files (`CustomerRepository.cs`, `ICustomerRepository.cs`, `CustomerDtos.cs`, `CustomerService.cs`, `ICustomerService.cs`). |
| Unit Test | Đạt 3/3 Test Pass (0 Failures). |
| Biên dịch | 0 Warning, 0 Error khi gọi `dotnet build`. |

---

## 2. QUY TRÌNH THỰC THI (DUAL-LAYER RUNBOOK)

### PHASE 1: TẦNG REPOSITORY — TRUY XUẤT DỮ LIỆU
**Time Budget:** 15 Phút

#### [EXEC] Bước 1.1: Tạo Interface & Implementation Repository
Chạy script PowerShell sau để sinh mã:

```powershell
Set-Location $SolutionRoot
$dataDir = "src\Crm.Data"
$repoDir = "src\Crm.Data\Repositories"
New-Item -ItemType Directory -Force -Path $repoDir | Out-Null
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

[System.IO.File]::WriteAllText("$repoDir\ICustomerRepository.cs", $iRepo, $enc)
[System.IO.File]::WriteAllText("$repoDir\CustomerRepository.cs", $repo, $enc)
```

**[Verify] Lệnh kiểm tra:**
```bash
dotnet build src/Crm.Data/Crm.Data.csproj
```
*Expected Output:* `Build succeeded. 0 Warning(s). 0 Error(s).`

#### [LEARN] Cơ chế trả về IQueryable
Trong dự án này, thay vì tạo các hàm như `GetList()` trả về sẵn `List<Customer>`, ta cho phép Repository trả về `IQueryable<Customer>`. Điều này cho phép tầng Service chèn thêm các điều kiện (`Where`), bộ lọc, phân trang (`Skip/Take`) trước khi query SQL thực sự được biên dịch và gửi xuống DB, giúp **loại bỏ tình trạng N+1** và kéo dư thừa dữ liệu.

---

### PHASE 2: TẦNG SERVICE — LOGIC NGHIỆP VỤ
**Time Budget:** 20 Phút

#### [EXEC] Bước 2.1: Tạo DTOs và Services
Chạy script PowerShell sau:

```powershell
Set-Location $SolutionRoot
$dtoDir = "src\Crm.Business\DTOs"
$svcDir = "src\Crm.Business\Services"
New-Item -ItemType Directory -Force -Path $dtoDir | Out-Null
New-Item -ItemType Directory -Force -Path $svcDir | Out-Null
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

[System.IO.File]::WriteAllText("$dtoDir\CustomerDtos.cs", $dto, $enc)
[System.IO.File]::WriteAllText("$svcDir\ICustomerService.cs", $iSvc, $enc)
[System.IO.File]::WriteAllText("$svcDir\CustomerService.cs", $svc, $enc)
```

**[Verify] Lệnh kiểm tra:**
```bash
dotnet build src/Crm.Business/Crm.Business.csproj
```

#### [LEARN] Xử lý Concurrency và Soft Delete
Mặc định hệ thống sử dụng Query Filter (trong EF Core). Do đó `_repository.GetQueryable()` sẽ tự động không lấy các bản ghi có `is_deleted = true`. Quy tắc này áp dụng ngầm, do đó tầng Service không cần viết lại mã lọc thủ công!

---

### PHASE 3: THIẾT LẬP UNIT TEST
**Time Budget:** 25 Phút

#### [EXEC] Bước 3.1: Viết Unit Test Crm.Tests
Khởi tạo và thêm mock tests:
```powershell
Set-Location $SolutionRoot
$testProjDir = "tests\Crm.Tests"

if (-not (Test-Path $testProjDir)) {
    dotnet new xunit -n Crm.Tests -o $testProjDir
    dotnet sln add "$testProjDir\Crm.Tests.csproj"
    dotnet add "$testProjDir\Crm.Tests.csproj" reference "src\Crm.Business\Crm.Business.csproj"
    dotnet add "$testProjDir\Crm.Tests.csproj" package Moq
    dotnet add "$testProjDir\Crm.Tests.csproj" package FluentAssertions
    dotnet add "$testProjDir\Crm.Tests.csproj" package MockQueryable.Moq
}

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

[System.IO.File]::WriteAllText("$testProjDir\CustomerServiceTests.cs", $testFile, $enc)
```

**[Verify] Lệnh kiểm tra:**
```bash
dotnet test tests/Crm.Tests/Crm.Tests.csproj
```

**Real Output Khảo sát:**
```text
Test run for Crm.Tests.dll (.NETCoreApp,Version=v10.0)
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     3, Skipped:     0, Total:     3, Duration: 320 ms
```

#### [LEARN] Tầm quan trọng của Moq
Khi test tầng `CustomerService`, ta tuyệt đối KHÔNG ĐƯỢC KẾT NỐI DATABASE THỰC TẾ (Do đó mới có `Mock<ICustomerRepository>`). Việc này giúp Unit Test có thể chạy trong 1ms thay vì hàng trăm ms, tránh tạo ra dữ liệu rác trên CSDL, tuân thủ nguyên lý `[K19] Test Pyramid`.

---

## 3. BẢNG KHẮC PHỤC SỰ CỐ (TROUBLESHOOTING MATRIX)

| Hiện tượng | Nguyên nhân gốc (Root Cause) | Biện pháp Khắc phục |
| :--- | :--- | :--- |
| Lỗi `IQueryable` không hỗ trợ `ExecuteAsync` khi Unit Test | Các hàm `MaxAsync` hoặc `AnyAsync` bị lỗi vì dữ liệu `List<T>` cơ bản không có cơ chế xử lý Async của Entity Framework. | Sử dụng thư viện `MockQueryable.Moq` và nối `.BuildMock()` vào đuôi collection mẫu. |
| Lỗi Namespace `ICustomerRepository` not found | Các component chưa using đúng namespace `Crm.Data.Repositories` | Kiểm tra lại directive `@using Crm.Data.Repositories` trong tầng Service. |

---

## 4. FINAL AUDIT CHECKLIST

- [x] Script Powershell chạy Idempotent hoàn chỉnh cho Repository
- [x] CustomerService đã có đủ 3 hàm (List, Create, Update)
- [x] Unit test kiểm định tính năng Sinh mã tự động `KH-xxxx`
- [x] Unit test kiểm định Exception khi Validate số điện thoại
- [x] `dotnet test` trả về `Passed: 3` thực tế.
