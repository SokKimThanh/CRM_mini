# Daily Log — Day 5 / Sprint 1

## Done
- [x] Đã xử lý Pre-flight, chuyển đổi tất cả thẻ style inline sang class CSS trong thư mục Pages/Account.
- [x] 5 enums nghiệp vụ trong `Crm.Domain/Enums`
- [x] 2 utility helpers trong `Crm.Business/Helpers`
- [x] Hệ thống Theme tập trung `CrmTheme.cs`
- [x] Master layout với Dark Mode toggle và Scoped CSS (Đã fix lỗi xung đột `Color` giữa ApexCharts và MudBlazor)
- [x] Serilog: Console + Rolling Daily File
- [x] Zero-Inline Audit PASS (0 vi phạm)
- [x] Hoàn tất quá trình compile, test qua dotnet run.

## Metrics
- Enums: 5 | Helpers: 2 | Layout: 3 | Theme: 1
- Build: 0 Warning, 0 Error
- Log path: `src/Crm.Web/logs/crm-YYYYMMDD.log`

## Lỗi và Hướng giải quyết (Troubleshooting)
- Lỗi CS0104: Xung đột namespace giữa `ApexCharts.Color` và `MudBlazor.Color` trong file `MainLayout.razor` (Do `_Imports.razor` chèn thêm `ApexCharts`). Hướng giải quyết: Đã gọi rõ `Color="MudBlazor.Color.Primary"` và `Color="MudBlazor.Color.Inherit"` để loại bỏ lỗi.
- Lỗi warning MUD0002 trên thuộc tính `Title` trên thẻ `MudIconButton` trong `MainLayout.razor`. Hướng giải quyết: Sử dụng thuộc tính `aria-label="Chuyển chế độ giao diện"` thay cho `Title`.

## Kỹ năng / Bài học (Learn)
- **Zero-Inline Styling (Blazor)**: Đẩy tất cả các CSS inline tĩnh vào `.css` và để component Razor sạch, giúp tránh xung đột UI và tiện quản lý (thay vào class).
- **Blazor Scoped CSS với MudBlazor**: Việc dùng `::deep` cần một thẻ div bao ngoài (`.crm-layout-wrapper`) làm phần tử cha để MudBlazor tự sinh mã hash đúng vào giao diện (shadow root/scope root).
- **Fallback Cơ chế TimeZone (Helpers)**: Sử dụng phương án bắt Exception để dự phòng: SE Asia (Windows) -> Asia/Ho_Chi_Minh (Linux) -> Custom Timezone. Kỹ năng này giúp Deploy an toàn trên nhiều môi trường (Windows/Linux/Docker).

## Next Steps (Day 6)
- CustomerRepository & CustomerService
- Bộ Unit Test cho CustomerService
- Giao diện danh sách `/customers` với MudTable
