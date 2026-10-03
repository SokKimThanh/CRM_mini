# ACTIONABLE PROMPTS — SPRINT 1 / DAY 6

Dưới đây là các prompt băm nhỏ task (chia thành các phiên < 30 phút) mà bạn có thể copy-paste giao cho AI để thực thi chính xác những gì được mô tả trong Runbook Ngày 6:

## Phiên 1: Xây dựng Repository (15 phút)
> "Jules, hãy đọc file `Day_6_Sprint_1_Customer_Repository_Service.md` ở Phase 1. Hãy tạo `ICustomerRepository` và `CustomerRepository` tại `src/Crm.Data/Repositories/`. Đảm bảo có IQueryable hỗ trợ phân trang và tìm kiếm động, tuân thủ DB First và Soft-delete. Không làm gì thêm cho tới khi hoàn tất Phase này."

## Phiên 2: Xây dựng Service & DTOs (20 phút)
> "Jules, hãy tiếp tục thực thi Phase 2 trong Runbook Ngày 6. Tạo các DTO (FilterDto, CreateDto, UpdateDto) trong `src/Crm.Business/DTOs/` và `CustomerService` trong `src/Crm.Business/Services/`. Đảm bảo sinh mã khách hàng tự động (KH-xxxx) và xử lý Concurrency. Đăng ký Dependency Injection trong Program.cs."

## Phiên 3: Thiết lập Unit Test (25 phút)
> "Jules, khởi tạo Project `tests/Crm.Tests` với xUnit và Moq như mô tả trong Phase 3 của Runbook. Viết 3 test cases cho `CustomerService` (Sinh mã tăng dần, Validate lỗi, Lọc kết quả). Chạy lệnh `dotnet test` cho đến khi tất cả các test pass xanh 100%."
