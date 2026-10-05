# B2B CRM SaaS Platform

Hệ thống B2B CRM hướng tới mô hình SaaS (Multi-tenant) được xây dựng với kiến trúc Clean Architecture và CQRS.

## Tech Stack & Runtime Manifest
- **Backend:** .NET 10 (C# 13), MediatR (CQRS)
- **Frontend:** Blazor Server, MudBlazor
- **Database:** PostgreSQL 15, Entity Framework Core 10 (Code-first with explicit explicit constraints & xmin concurrency)
- **Background Jobs:** Hangfire

## Cây Thư Mục Cấp 1 & Vai Trò
- `/docs`: Tài liệu đặc tả kỹ thuật, Runbooks triển khai, và các ghi chép học tập/phỏng vấn.
- `/src/Crm.Domain`: Lõi nghiệp vụ chứa Entities, Enums và Interfaces chung.
- `/src/Crm.Data`: Triển khai hạ tầng Database, DbContext và Repositories.
- `/src/Crm.Business`: Triển khai các Command/Query Handlers theo kiến trúc CQRS.
- `/src/Crm.Web`: Giao diện người dùng Blazor và điểm khởi chạy Web API.
- `/tests`: Các dự án kiểm thử xUnit và Moq.

## Chỉ Mục Nóng (Hot-Links)
- [Data Dictionary](docs/specs/data_dictionary.md) - Đặc tả chi tiết cơ sở dữ liệu.
- [API Contracts](docs/specs/api_contracts.md) - Đặc tả luồng Command/Query CQRS.
- [Current Sprint Runbook](docs/sprints/sprint_1/runbooks/Day_6_Sprint_1_Customer_Repository_Service.md) - Hướng dẫn thiết lập CQRS.
- [🔥 Bửu bối Phỏng vấn (Interview Cheatsheet)](docs/notes/interview_cheatsheet.md) - Cẩm nang trả lời phỏng vấn các quyết định kiến trúc dự án.
