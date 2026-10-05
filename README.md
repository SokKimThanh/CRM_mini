# CrmSolution - Hệ thống B2B CRM

Dự án CRM (Quản trị Quan hệ Khách hàng) dành cho mô hình B2B.

## 1. Tech Stack & Runtime Manifest
- **Framework**: .NET 10, C# 13, Blazor Server/WASM.
- **Cơ sở dữ liệu**: PostgreSQL.
- **Xử lý nền**: Hangfire.
- **Kiến trúc**: CQRS với MediatR, Repository Pattern.

## 2. Cây Thư Mục Cấp 1 & Vai Trò
- `CrmSolution/`: Thư mục mã nguồn chính của ứng dụng backend và frontend.
- `CrmSolution/docs/`: Chuyên lưu trữ tài liệu đặc tả, Runbook, sổ nợ kỹ thuật và nhật ký hàng ngày.
- `.git/`: Quản lý phiên bản mã nguồn Git (với các nhánh main, staging, develop).
- `AGENTS.md`: Hệ điều hành chỉ thị bắt buộc dành cho AI/Agent.

## 3. Mục Lục Chỉ Mục Nóng (Hot-Links)
- [Data Dictionary (Từ điển Dữ liệu)](./CrmSolution/docs/specs/data_dictionary.md)
- [API Contracts (Đặc tả Giao tiếp API)](./CrmSolution/docs/specs/api_contracts.md)
- [Runbook Mới Nhất (Day 6 - Sprint 1)](./CrmSolution/docs/sprints/sprint_1/runbooks/day6_customer_repo_service.md)

---
*Ghi chú: README này tuân thủ chuẩn K11, ngắn gọn dưới 100 dòng.*