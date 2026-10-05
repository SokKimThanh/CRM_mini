## customers — Cập nhật V1.0.2 (2026-10-05)

| Cột | Kiểu dữ liệu | Nullable | Ghi chú kỹ thuật |
|:---|:---|:---|:---|
| tenant_id | UUID | NO | [K39] Phân lập dữ liệu tổ chức |
| owner_id | UUID | YES | [K39] Khóa ngoại `fk_customers_owner_id` $\rightarrow$ users(id) |
| team_id | UUID | YES | [K39] Khóa ngoại `fk_customers_team_id` $\rightarrow$ teams(id) |
| is_deleted | BOOLEAN | NO | [K21] Cờ đánh dấu xóa mềm |
| deleted_at | TIMESTAMPTZ | YES | Thời điểm xóa mềm |
| deleted_by | UUID | YES | Người thực hiện xóa |
| updated_at | TIMESTAMPTZ | YES | [K20] Thời điểm sửa đổi gần nhất (UTC) |
| updated_by | UUID | YES | Người thực hiện sửa đổi |
| xmin | xid | NO | [K19] System column PostgreSQL làm Optimistic Concurrency Token |

## teams — Mới V1.0.2

| Cột | Kiểu dữ liệu | Nullable | Ghi chú kỹ thuật |
|:---|:---|:---|:---|
| id | UUID | NO | Khóa chính (gen_random_uuid()) |
| tenant_id | UUID | NO | Phân lập tenant |
| name | VARCHAR(200) | NO | Tên phòng ban / đội ngũ |
| parent_team_id | UUID | YES | Tự tham chiếu `fk_teams_parent_team_id` $\rightarrow$ teams(id) |
| is_deleted | BOOLEAN | NO | Xóa mềm |
| created_at | TIMESTAMPTZ | NO | Thời điểm tạo (UTC) |

## users — Mới V1.0.2

| Cột | Kiểu dữ liệu | Nullable | Ghi chú kỹ thuật |
|:---|:---|:---|:---|
| id | UUID | NO | Khóa chính (gen_random_uuid()) |
| tenant_id | UUID | NO | Phân lập tenant |
| team_id | UUID | YES | Khóa ngoại `fk_users_team_id` $\rightarrow$ teams(id) |
| email | VARCHAR(200) | NO | Duy nhất (uq_users_email) |
| role | VARCHAR(20) | NO | Vai trò: SALES, MANAGER, ADMIN |
| is_deleted | BOOLEAN | NO | Xóa mềm |
| created_at | TIMESTAMPTZ | NO | Thời điểm tạo (UTC) |
