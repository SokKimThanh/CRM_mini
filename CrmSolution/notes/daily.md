# day 1 completed 28/9/2026
# Daily Log — Ngày 2 / Sprint 1
## Công việc đã hoàn thành
- [x] Cài đặt & cấu hình PostgreSQL 16 database `crm_db` với Encoding UTF8.
- [x] Tạo thành công 16 bảng nghiệp vụ theo đặc tả hệ thống CRM B2B.
- [x] Tạo 7 Triggers tự động cập nhật timestamp `updated_at`.
- [x] Tạo 2 Database Views (`v_customer_health_summary`, `v_opportunity_pipeline`).
- [x] Tạo và nạp hoàn chỉnh master data (6 stages, 4 categories) và demo data (5 customers, 5
contacts, 10 products, 5 deals).
- [x] Kiểm tra toàn bộ ràng buộc khóa ngoại (Foreign Keys), CHECK constraints và Indexes.
## Bàn giao cho Ngày 3 (ASP.NET Core Identity & Scaffold)
- Kiểu dữ liệu `assigned_to_user_id` và `user_id` đã được định hình dạng `VARCHAR(450)`.- Khi setup `ApplicationUser` ở Ngày 3, sẽ chạy migration bổ sung Foreign Keys tham chiếu
sang `AspNetUsers(Id)`.
- Triển khai 2 bảng phụ: `teams` và `user_profiles`.