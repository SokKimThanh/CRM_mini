# day 1 completed 28/9/2026
# DAILY LOG — SPRINT 1 (NGÀY 2)
**Ngày thực hiện:** 30/09/2026  
**Dự án:** CRM Mini (.NET 10 + PostgreSQL 18)  
**Trạng thái:** Hoàn thành 100% mục tiêu Ngày 2

---

### 1. Công việc đã hoàn thành
- **Môi trường & Quyền:**
  - Cấu hình database `crm_db` chuẩn encoding `UTF-8`.
  - Phân quyền owner schema `public` cho user `crm_user` (mật khẩu: `sa`).
- **Schema & DDL:**
  - Khởi tạo thành công 16 bảng lõi (`schema.sql`).
  - Cấu hình 70 indexes và 22 Foreign Keys đảm bảo toàn vẹn dữ liệu.
  - Tạo 7 triggers tự động cập nhật trường `updated_at`.
  - Khởi tạo 2 views báo cáo: `v_customer_health_summary` và `v_opportunity_pipeline`.
- **Seed Data:**
  - Nạp Master data: 6 stages (`opportunity_stages`), 4 danh mục (`categories`).
  - Nạp bộ dữ liệu mẫu (`02_demo_data.sql`): 5 customers, 5 contacts, 10 products, 5 opportunities, 15 stage histories, 2 quotes (6 items), 5 tasks, 5 interactions.
- **Nghiệm thu (Checklist):**
  - Đạt 10/10 tiêu chí kiểm thử (kiểm tra trigger, view, đối soát số lượng bản ghi).

---

### 2. Khó khăn & Giải pháp
- **Vấn đề:** Terminal Windows gặp lỗi mã hóa `WIN1252` với ký tự tiếng Việt khi chạy script SQL.
- **Giải pháp:** Thiết lập `SET client_encoding = 'UTF8';` trong SQL script và gán biến môi trường `$env:PGCLIENTENCODING = "utf-8"` trước khi thực thi `psql`.

---

### 3. Kế hoạch Ngày 3
- Tích hợp **ASP.NET Core Identity** vào dự án `Crm.Web`.
- Cấu hình Authentication/Authorization (Cookie, JWT nếu cần).
- Tạo luồng Login, Logout, Quản lý Roles (Admin, Sales, Manager).# DAILY LOG — SPRINT 1 (NGÀY 2)
**Ngày thực hiện:** 30/09/2026  
**Dự án:** CRM Mini (.NET 10 + PostgreSQL 18)  
**Trạng thái:** Hoàn thành 100% mục tiêu Ngày 2

---

### 1. Công việc đã hoàn thành
- **Môi trường & Quyền:**
  - Cấu hình database `crm_db` chuẩn encoding `UTF-8`.
  - Phân quyền owner schema `public` cho user `crm_user` (mật khẩu: `sa`).
- **Schema & DDL:**
  - Khởi tạo thành công 16 bảng lõi (`schema.sql`).
  - Cấu hình 70 indexes và 22 Foreign Keys đảm bảo toàn vẹn dữ liệu.
  - Tạo 7 triggers tự động cập nhật trường `updated_at`.
  - Khởi tạo 2 views báo cáo: `v_customer_health_summary` và `v_opportunity_pipeline`.
- **Seed Data:**
  - Nạp Master data: 6 stages (`opportunity_stages`), 4 danh mục (`categories`).
  - Nạp bộ dữ liệu mẫu (`02_demo_data.sql`): 5 customers, 5 contacts, 10 products, 5 opportunities, 15 stage histories, 2 quotes (6 items), 5 tasks, 5 interactions.
- **Nghiệm thu (Checklist):**
  - Đạt 10/10 tiêu chí kiểm thử (kiểm tra trigger, view, đối soát số lượng bản ghi).

---

### 2. Khó khăn & Giải pháp
- **Vấn đề:** Terminal Windows gặp lỗi mã hóa `WIN1252` với ký tự tiếng Việt khi chạy script SQL.
- **Giải pháp:** Thiết lập `SET client_encoding = 'UTF8';` trong SQL script và gán biến môi trường `$env:PGCLIENTENCODING = "utf-8"` trước khi thực thi `psql`.

---

### 3. Kế hoạch Ngày 3
- Tích hợp **ASP.NET Core Identity** vào dự án `Crm.Web`.
- Cấu hình Authentication/Authorization (Cookie, JWT nếu cần).
- Tạo luồng Login, Logout, Quản lý Roles (Admin, Sales, Manager).