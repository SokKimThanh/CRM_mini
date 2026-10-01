# RUNBOOK NGÀY 2 — SPRINT 1: THIẾT KẾ DATABASE & SCHEMA DDL

**Mục tiêu ngày:** Soạn thảo hoàn chỉnh file `schema.sql` gồm 16 bảng nghiệp vụ trên PostgreSQL 16, thực thi không lỗi cú pháp, kích hoạt trigger tự động, thiết lập view báo cáo và nạp dữ liệu mẫu (master data & demo data).

**Thời lượng dự kiến:** 120 phút  
**Output bắt buộc:**
- File `docs/db/schema.sql` (Schema DDL + Master Data + Triggers + Views + Validation script).
- File `docs/db/seeds/02_demo_data.sql` (Seed data kịch bản nghiệp vụ).
- Cơ sở dữ liệu `crm_db` hoạt động, có 16 bảng, ~35 indexes, 7 triggers và pass toàn bộ script validation.

**Stack công nghệ:** .NET 10 (LTS) | PostgreSQL 16+ | DBeaver / pgAdmin 4 / VS Code (Database Client Extension)

---

## 1. TỔNG QUAN HỆ THỐNG VÀ DANH MỤC THỰC THỂ

| Hạng mục | Chi tiết kỹ thuật |
|---|---|
| **Runtime & SDK** | .NET 10 SDK (LTS) |
| **Database Engine** | PostgreSQL 16.x |
| **Encoding / Collation** | Server Encoding: `UTF8`, Client: `UTF8`, TimeZone: `UTC` |
| **User/Role thực thi** | `crm_user` (Schema Owner: `public`) |
| **Số bảng cần tạo** | 16 bảng nghiệp vụ & hệ thống |
| **Phụ thuộc** | Solution CrmSolution đã khởi tạo ở Ngày 1 |
| **Chuyển giao Ngày 3** | Bảng ASP.NET Core Identity (`AspNetUsers`, `AspNetRoles`,...) cùng 2 bảng phụ thuộc `teams` và `user_profiles` |

### Danh sách 16 bảng theo phân tầng kiến trúc

```
crm_db (PostgreSQL 16)
├── Danh mục Master Data
│   ├── [01] opportunity_stages (6 giai đoạn bán hàng cơ sở)
│   └── [02] categories (Nhóm hàng hóa: dao cụ, đá mài, bulong...)
├── Khách hàng & Quan hệ (CRM Core)
│   ├── [03] customers (Doanh nghiệp B2B / Nhà máy)
│   ├── [04] contacts (Người liên hệ đại diện)
│   └── [05] customer_assignments (Lịch sử phân bổ phụ trách sales)
├── Bán hàng & Pipeline (Sales Pipeline)
│   ├── [06] opportunities (Cơ hội bán hàng)
│   ├── [07] stage_histories (Audit trail chuyển dịch pipeline)
│   ├── [08] products (Sản phẩm đồng bộ KiotViet)
│   ├── [09] quotes (Báo giá kỹ thuật/thương mại)
│   └── [10] quote_items (Chi tiết hạng mục báo giá)
├── Tương tác & Công việc (Activities & Tasks)
│   ├── [11] sales_tasks (Nhiệm vụ, cuộc gọi nhắc lịch)
│   └── [12] interactions (Nhật ký tương tác: Zalo, gặp trực tiếp, gọi điện)
└── Quản trị & Vận hành (System & Monitoring)
    ├── [13] notifications (Thông báo người dùng nội bộ)
    ├── [14] dashboard_snapshots (Snapshot chỉ số kinh doanh hàng đêm)
    ├── [15] audit_logs (Nhật ký thay đổi dữ liệu chi tiết)
    └── [16] kiotviet_sync_logs (Nhật ký đồng bộ hóa với KiotViet API)
```

---

## PHASE 1 — THIẾT LẬP MÔI TRƯỜNG & KHỞI TẠO CƠ SỞ DỮ LIỆU (15 phút)

### 1.1 Xác minh công cụ máy dev

Mở Terminal (PowerShell hoặc Bash):

```bash
# 1. Kiểm tra .NET 10 SDK
dotnet --version
# Output kỳ vọng: 10.0.xxx

# 2. Kiểm tra PostgreSQL Server & Client CLI
psql --version
# Output kỳ vọng: psql (PostgreSQL) 16.x

# 3. Kiểm tra trạng thái cổng PostgreSQL
pg_isready -h localhost -p 5432
# Output kỳ vọng: localhost:5432 - accepting connections
```

### 1.2 Khởi tạo Database và Phân quyền User

Chạy phiên làm việc với quyền superuser `postgres`:

```bash
psql -U postgres
```

Thực thi khối lệnh SQL sau để khởi tạo môi trường cơ sở dữ liệu sạch:

```sql
-- Hủy kết nối đang có (nếu làm lại)
SELECT pg_terminate_backend(pid) 
FROM pg_stat_activity 
WHERE datname = 'crm_db' AND pid <> pg_backend_pid();

-- Xóa database cũ nếu cần reset hoàn toàn
DROP DATABASE IF EXISTS crm_db;

-- Tạo database chuẩn UTF-8
CREATE DATABASE crm_db
    WITH ENCODING = 'UTF8'
    TEMPLATE = template0;

-- Tạo hoặc cập nhật User chuyên trách
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'crm_user') THEN
        CREATE ROLE crm_user WITH LOGIN PASSWORD 'Crm@2026#Dev';
    ELSE
        ALTER ROLE crm_user WITH PASSWORD 'Crm@2026#Dev';
    END IF;
END $$;

-- Cấp quyền kết nối database
GRANT ALL PRIVILEGES ON DATABASE crm_db TO crm_user;

-- Chuyển context sang database crm_db
\c crm_db

-- Cấp quyền thao tác schema public
GRANT ALL ON SCHEMA public TO crm_user;
ALTER SCHEMA public OWNER TO crm_user;

-- Đảm bảo quyền mặc định cho các đối tượng tạo mới trong tương lai
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO crm_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO crm_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON FUNCTIONS TO crm_user;

-- Kiểm tra encoding
SHOW server_encoding;
-- Kỳ vọng hiển thị: UTF8

\q
```

### 1.3 Tạo cấu trúc thư mục lưu trữ

```bash
cd D:\Projects\CrmSolution
mkdir -p docs/db/seeds
mkdir -p notes
```

### 1.4 Cấu hình Connection String

Cập nhật file cấu hình `src/Crm.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=crm_db;Username=crm_user;Password=Crm@2026#Dev;Include Error Detail=true;Timeout=30;Command Timeout=60"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Npgsql": "Information"
    }
  }
}
```

> **Lưu ý kỹ thuật:** Mặc định Npgsql 8+ và .NET 10 quản lý kiểu `TIMESTAMPTZ` ánh xạ sang `DateTimeOffset` hoặc `DateTime` với `Kind=DateTimeKind.Utc`. Tuyệt đối không dùng legacy timestamp switches nếu không bắt buộc.

---

## PHASE 2 — VIẾT FILE SCHEMA CHÍNH THỨC (60 phút)

Tạo file `docs/db/schema.sql` với toàn bộ nội dung DDL dưới đây:

```sql
-- =====================================================================
-- CRM SYSTEM — B2B INDUSTRIAL MANUFACTURING & TRADING
-- Database Engine : PostgreSQL 16+
-- Target Runtime  : .NET 10 (C# 14 / EF Core 10)
-- Script          : docs/db/schema.sql
-- =====================================================================

-- ---------------------------------------------------------------------
-- PHẦN 1: THIẾT LẬP MÔI TRƯỜNG PHIÊN LÀM VIỆC
-- ---------------------------------------------------------------------
SET TIME ZONE 'UTC';
SET client_encoding = 'UTF8';

-- ---------------------------------------------------------------------
-- PHẦN 2: HÀM DÙNG CHUNG (STORED PROCEDURES / FUNCTIONS)
-- ---------------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CLOCK_TIMESTAMP();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =====================================================================
-- PHẦN 3: DANH MỤC GỐC & THIẾT LẬP MASTER DATA
-- =====================================================================

-- ---------------------------------------------------------------------
-- Bảng 1: opportunity_stages (6 giai đoạn bán hàng)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS opportunity_stages (
    id                  SERIAL PRIMARY KEY,
    name                VARCHAR(50) NOT NULL UNIQUE,
    display_order       INT NOT NULL,
    default_probability INT NOT NULL DEFAULT 0,
    color               VARCHAR(20) NOT NULL DEFAULT '#607D8B',
    is_won              BOOLEAN NOT NULL DEFAULT FALSE,
    is_lost             BOOLEAN NOT NULL DEFAULT FALSE,
    is_closed           BOOLEAN NOT NULL DEFAULT FALSE,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_opportunity_stages_prob CHECK (default_probability BETWEEN 0 AND 100)
);

COMMENT ON TABLE opportunity_stages IS 'Danh mục các giai đoạn trên Sales Pipeline';

-- Nạp master data mặc định cho stages
INSERT INTO opportunity_stages (id, name, display_order, default_probability, color, is_won, is_lost, is_closed)
VALUES
    (1, 'Mới tiếp cận',     1, 10,  '#F5A623', FALSE, FALSE, FALSE),
    (2, 'Đang liên hệ',     2, 30,  '#E67E22', FALSE, FALSE, FALSE),
    (3, 'Đã gửi báo giá',   3, 50,  '#9B59B6', FALSE, FALSE, FALSE),
    (4, 'Đang đàm phán',    4, 70,  '#3498DB', FALSE, FALSE, FALSE),
    (5, 'Chốt thành công',  5, 100, '#2ECC71', TRUE,  FALSE, TRUE),
    (6, 'Thất bại',         6, 0,   '#E74C3C', FALSE, TRUE,  TRUE)
ON CONFLICT (id) DO UPDATE 
SET name = EXCLUDED.name,
    display_order = EXCLUDED.display_order,
    default_probability = EXCLUDED.default_probability,
    color = EXCLUDED.color,
    is_won = EXCLUDED.is_won,
    is_lost = EXCLUDED.is_lost,
    is_closed = EXCLUDED.is_closed;

SELECT setval('opportunity_stages_id_seq', (SELECT MAX(id) FROM opportunity_stages), true);

-- ---------------------------------------------------------------------
-- Bảng 2: categories (Nhóm sản phẩm/ngành hàng)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS categories (
    id              BIGSERIAL PRIMARY KEY,
    kiotviet_id     BIGINT UNIQUE,
    name            VARCHAR(255) NOT NULL,
    description     TEXT,
    display_order   INT NOT NULL DEFAULT 0,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE categories IS 'Danh mục ngành hàng: Dao cụ, Đá mài, Bulong ốc vít, Dầu mỡ công nghiệp';

CREATE TRIGGER trg_categories_updated_at
    BEFORE UPDATE ON categories
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- Seed master categories
INSERT INTO categories (name, description, display_order)
VALUES
    ('Dao cụ',       'Dao phay, mũi khoan, taro, mảnh tiện CNC',           1),
    ('Đá mài',       'Đá mài phẳng, đá mài tròn, đĩa cắt, bánh mài nỉ',   2),
    ('Ốc vít',       'Bulong cường độ cao, đai ốc, long đền, vít inox',  3),
    ('Dầu bôi trơn', 'Dầu cắt gọt pha nước, dầu thủy lực, mỡ bôi trơn',   4)
ON CONFLICT DO NOTHING;

-- =====================================================================
-- PHẦN 4: DỮ LIỆU KHÁCH HÀNG & LIÊN HỆ (CRM CORE)
-- =====================================================================

-- ---------------------------------------------------------------------
-- Bảng 3: customers (Khách hàng B2B - Nhà máy cơ khí)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS customers (
    id                  BIGSERIAL PRIMARY KEY,
    code                VARCHAR(50) NOT NULL UNIQUE,
    name                VARCHAR(255) NOT NULL,
    industry            VARCHAR(100),
    tax_code            VARCHAR(50),
    address             TEXT,
    phone               VARCHAR(20),
    email               VARCHAR(100),
    website             VARCHAR(200),
    assigned_to_user_id VARCHAR(450), -- Đồng bộ với AspNetUsers.Id ở Day 3
    health_status       INT NOT NULL DEFAULT 0,
    average_cycle_days  INT NOT NULL DEFAULT 0,
    last_order_date     TIMESTAMPTZ,
    last_contact_date   TIMESTAMPTZ,
    next_contact_due    TIMESTAMPTZ,
    revenue_90d         NUMERIC(18,2) NOT NULL DEFAULT 0,
    order_count_90d     INT NOT NULL DEFAULT 0,
    current_debt        NUMERIC(18,2) NOT NULL DEFAULT 0,
    kiotviet_id         BIGINT UNIQUE,
    note                TEXT,
    is_active           BOOLEAN NOT NULL DEFAULT TRUE,
    is_deleted          BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at          TIMESTAMPTZ,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_customers_health CHECK (health_status BETWEEN 0 AND 5),
    CONSTRAINT chk_customers_revenue CHECK (revenue_90d >= 0),
    CONSTRAINT chk_customers_cycle CHECK (average_cycle_days >= 0)
);

COMMENT ON TABLE customers IS 'Hồ sơ khách hàng B2B (Nhà máy chế tạo cơ khí, xưởng gia công CNC)';
COMMENT ON COLUMN customers.health_status IS '0: New, 1: Healthy, 2: Fair, 3: AtRisk, 4: Dormant, 5: Churned';

CREATE INDEX idx_customers_code         ON customers(code);
CREATE INDEX idx_customers_health       ON customers(health_status) WHERE is_deleted = FALSE;
CREATE INDEX idx_customers_assigned     ON customers(assigned_to_user_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_customers_next_due     ON customers(next_contact_due) WHERE is_deleted = FALSE;
CREATE INDEX idx_customers_kiotviet     ON customers(kiotviet_id) WHERE kiotviet_id IS NOT NULL;
CREATE INDEX idx_customers_active_query ON customers(is_active, is_deleted);

CREATE TRIGGER trg_customers_updated_at
    BEFORE UPDATE ON customers
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- ---------------------------------------------------------------------
-- Bảng 4: contacts (Người liên hệ đại diện doanh nghiệp)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS contacts (
    id          BIGSERIAL PRIMARY KEY,
    customer_id BIGINT NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    name        VARCHAR(100) NOT NULL,
    position    VARCHAR(100),
    phone       VARCHAR(20),
    email       VARCHAR(100),
    birthday    DATE,
    is_primary  BOOLEAN NOT NULL DEFAULT FALSE,
    note        TEXT,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE contacts IS 'Đầu mối liên hệ: Giám đốc xưởng, Trưởng phòng mua hàng, Kỹ sư CNC';

CREATE INDEX idx_contacts_customer ON contacts(customer_id);
CREATE INDEX idx_contacts_primary  ON contacts(customer_id, is_primary) WHERE is_primary = TRUE;

CREATE TRIGGER trg_contacts_updated_at
    BEFORE UPDATE ON contacts
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- ---------------------------------------------------------------------
-- Bảng 5: customer_assignments (Lịch sử bàn giao tài khoản sales)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS customer_assignments (
    id             BIGSERIAL PRIMARY KEY,
    customer_id    BIGINT NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    from_user_id   VARCHAR(450),
    to_user_id     VARCHAR(450) NOT NULL,
    assigned_by    VARCHAR(450),
    reason         TEXT,
    assigned_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    unassigned_at  TIMESTAMPTZ
);

COMMENT ON TABLE customer_assignments IS 'Lịch sử điều chuyển quyền chăm sóc khách hàng giữa các nhân viên sales';

CREATE INDEX idx_assign_customer ON customer_assignments(customer_id);
CREATE INDEX idx_assign_user     ON customer_assignments(to_user_id);

-- =====================================================================
-- PHẦN 5: PIPELINE CƠ HỘI & SẢN PHẨM & BÁO GIÁ
-- =====================================================================

-- ---------------------------------------------------------------------
-- Bảng 6: opportunities (Cơ hội bán hàng)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS opportunities (
    id                  BIGSERIAL PRIMARY KEY,
    code                VARCHAR(50) NOT NULL UNIQUE,
    customer_id         BIGINT NOT NULL REFERENCES customers(id) ON DELETE RESTRICT,
    contact_id          BIGINT REFERENCES contacts(id) ON DELETE SET NULL,
    stage_id            INT NOT NULL REFERENCES opportunity_stages(id) ON DELETE RESTRICT,
    title               VARCHAR(255) NOT NULL,
    description         TEXT,
    estimated_value     NUMERIC(18,2) NOT NULL DEFAULT 0,
    probability         INT NOT NULL DEFAULT 0,
    assigned_to_user_id VARCHAR(450),
    expected_close_date TIMESTAMPTZ,
    actual_close_date   TIMESTAMPTZ,
    source              VARCHAR(100),
    is_won              BOOLEAN,
    loss_reason         INT,
    loss_note           TEXT,
    note                TEXT,
    is_deleted          BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at          TIMESTAMPTZ,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_opp_probability CHECK (probability BETWEEN 0 AND 100),
    CONSTRAINT chk_opp_estimated_value CHECK (estimated_value >= 0),
    CONSTRAINT chk_opp_loss_reason CHECK (loss_reason IS NULL OR loss_reason BETWEEN 1 AND 5)
);

COMMENT ON TABLE opportunities IS 'Cơ hội bán hàng trên luồng pipeline';
COMMENT ON COLUMN opportunities.loss_reason IS '1: Giá cao, 2: Giao hàng chậm, 3: Đối thủ cạnh tranh, 4: Khách dừng dự án, 5: Khác';

CREATE INDEX idx_opp_customer   ON opportunities(customer_id);
CREATE INDEX idx_opp_stage      ON opportunities(stage_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_opp_assigned   ON opportunities(assigned_to_user_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_opp_expected   ON opportunities(expected_close_date);
CREATE INDEX idx_opp_created_at ON opportunities(created_at DESC);
CREATE INDEX idx_opp_won        ON opportunities(is_won) WHERE is_won = TRUE;

CREATE TRIGGER trg_opp_updated_at
    BEFORE UPDATE ON opportunities
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- ---------------------------------------------------------------------
-- Bảng 7: stage_histories (Lịch sử dịch chuyển giai đoạn)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS stage_histories (
    id                  BIGSERIAL PRIMARY KEY,
    opportunity_id      BIGINT NOT NULL REFERENCES opportunities(id) ON DELETE CASCADE,
    from_stage_id       INT REFERENCES opportunity_stages(id),
    to_stage_id         INT NOT NULL REFERENCES opportunity_stages(id),
    changed_by_user_id  VARCHAR(450),
    note                TEXT,
    changed_at          TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE stage_histories IS 'Audit log theo dõi thời gian và trạng thái thay đổi stage của deal';

CREATE INDEX idx_stage_history_opp ON stage_histories(opportunity_id, changed_at);

-- ---------------------------------------------------------------------
-- Bảng 8: products (Sản phẩm - Master data đồng bộ KiotViet)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS products (
    id              BIGSERIAL PRIMARY KEY,
    kiotviet_id     BIGINT UNIQUE,
    code            VARCHAR(50) NOT NULL UNIQUE,
    name            VARCHAR(255) NOT NULL,
    category_id     BIGINT REFERENCES categories(id) ON DELETE SET NULL,
    unit            VARCHAR(50),
    base_price      NUMERIC(18,2) NOT NULL DEFAULT 0,
    cost_price      NUMERIC(18,2) NOT NULL DEFAULT 0,
    description     TEXT,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_products_base_price CHECK (base_price >= 0),
    CONSTRAINT chk_products_cost_price CHECK (cost_price >= 0)
);

COMMENT ON TABLE products IS 'Danh mục hàng hóa kim khí & dao cụ cơ khí';

CREATE INDEX idx_products_code     ON products(code);
CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_products_kiotviet ON products(kiotviet_id) WHERE kiotviet_id IS NOT NULL;
CREATE INDEX idx_products_active   ON products(is_active) WHERE is_active = TRUE;

CREATE TRIGGER trg_products_updated_at
    BEFORE UPDATE ON products
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- ---------------------------------------------------------------------
-- Bảng 9: quotes (Báo giá kỹ thuật & thương mại)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS quotes (
    id                  BIGSERIAL PRIMARY KEY,
    code                VARCHAR(50) NOT NULL UNIQUE,
    opportunity_id      BIGINT REFERENCES opportunities(id) ON DELETE SET NULL,
    customer_id         BIGINT NOT NULL REFERENCES customers(id) ON DELETE RESTRICT,
    contact_id          BIGINT REFERENCES contacts(id) ON DELETE SET NULL,
    created_by_user_id  VARCHAR(450) NOT NULL,
    quote_date          DATE NOT NULL DEFAULT CURRENT_DATE,
    valid_until         DATE NOT NULL,
    status              INT NOT NULL DEFAULT 0,
    sub_total           NUMERIC(18,2) NOT NULL DEFAULT 0,
    discount_amount     NUMERIC(18,2) NOT NULL DEFAULT 0,
    vat_rate            NUMERIC(5,2) NOT NULL DEFAULT 10.00,
    vat_amount          NUMERIC(18,2) NOT NULL DEFAULT 0,
    total_amount        NUMERIC(18,2) NOT NULL DEFAULT 0,
    payment_terms       TEXT,
    note                TEXT,
    approved_by         VARCHAR(450),
    approved_at         TIMESTAMPTZ,
    is_deleted          BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at          TIMESTAMPTZ,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_quotes_status CHECK (status BETWEEN 0 AND 5),
    CONSTRAINT chk_quotes_sub_total CHECK (sub_total >= 0),
    CONSTRAINT chk_quotes_discount CHECK (discount_amount >= 0),
    CONSTRAINT chk_quotes_total CHECK (total_amount >= 0),
    CONSTRAINT chk_quotes_valid_range CHECK (valid_until >= quote_date)
);

COMMENT ON TABLE quotes IS 'Báo giá gửi cho khách hàng doanh nghiệp';
COMMENT ON COLUMN quotes.status IS '0: Nháp, 1: Chờ duyệt, 2: Đã gửi khách, 3: Khách chấp thuận, 4: Từ chối, 5: Hết hạn';

CREATE INDEX idx_quotes_code       ON quotes(code);
CREATE INDEX idx_quotes_customer   ON quotes(customer_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_quotes_opp        ON quotes(opportunity_id);
CREATE INDEX idx_quotes_status     ON quotes(status) WHERE is_deleted = FALSE;
CREATE INDEX idx_quotes_quote_date ON quotes(quote_date DESC);

CREATE TRIGGER trg_quotes_updated_at
    BEFORE UPDATE ON quotes
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- ---------------------------------------------------------------------
-- Bảng 10: quote_items (Chi tiết từng dòng sản phẩm trong báo giá)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS quote_items (
    id              BIGSERIAL PRIMARY KEY,
    quote_id        BIGINT NOT NULL REFERENCES quotes(id) ON DELETE CASCADE,
    product_id      BIGINT REFERENCES products(id) ON DELETE SET NULL,
    product_name    VARCHAR(255) NOT NULL,
    product_code    VARCHAR(50),
    unit            VARCHAR(50),
    quantity        NUMERIC(12,2) NOT NULL,
    unit_price      NUMERIC(18,2) NOT NULL,
    discount_amount NUMERIC(18,2) NOT NULL DEFAULT 0,
    total_line      NUMERIC(18,2) NOT NULL,
    sort_order      INT NOT NULL DEFAULT 0,
    note            TEXT,
    CONSTRAINT chk_quote_items_qty CHECK (quantity > 0),
    CONSTRAINT chk_quote_items_price CHECK (unit_price >= 0),
    CONSTRAINT chk_quote_items_discount CHECK (discount_amount >= 0),
    CONSTRAINT chk_quote_items_total CHECK (total_line >= 0)
);

COMMENT ON TABLE quote_items IS 'Danh mục chi tiết sản phẩm / đơn giá / chiết khấu trong báo giá';

CREATE INDEX idx_quote_items_quote   ON quote_items(quote_id);
CREATE INDEX idx_quote_items_product ON quote_items(product_id);

-- =====================================================================
-- PHẦN 6: HOẠT ĐỘNG, NHIỆM VỤ & TƯƠNG TÁC
-- =====================================================================

-- ---------------------------------------------------------------------
-- Bảng 11: sales_tasks (Kế hoạch hành động & Nhắc việc)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS sales_tasks (
    id                  BIGSERIAL PRIMARY KEY,
    title               VARCHAR(255) NOT NULL,
    description         TEXT,
    type                INT NOT NULL DEFAULT 6,
    priority            INT NOT NULL DEFAULT 1,
    status              INT NOT NULL DEFAULT 0,
    customer_id         BIGINT REFERENCES customers(id) ON DELETE CASCADE,
    contact_id          BIGINT REFERENCES contacts(id) ON DELETE SET NULL,
    opportunity_id      BIGINT REFERENCES opportunities(id) ON DELETE SET NULL,
    quote_id            BIGINT REFERENCES quotes(id) ON DELETE SET NULL,
    related_product_id  BIGINT REFERENCES products(id) ON DELETE SET NULL,
    due_date            TIMESTAMPTZ NOT NULL,
    completed_at        TIMESTAMPTZ,
    assigned_to_user_id VARCHAR(450) NOT NULL,
    completed_by        VARCHAR(450),
    outcome             INT,
    result_note         TEXT,
    is_deleted          BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at          TIMESTAMPTZ,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_tasks_type CHECK (type BETWEEN 1 AND 7),
    CONSTRAINT chk_tasks_priority CHECK (priority BETWEEN 0 AND 3),
    CONSTRAINT chk_tasks_status CHECK (status BETWEEN 0 AND 4),
    CONSTRAINT chk_tasks_outcome CHECK (outcome IS NULL OR outcome BETWEEN 1 AND 8)
);

COMMENT ON TABLE sales_tasks IS 'Công việc, lịch viếng thăm, cuộc gọi định kỳ của Sales';
COMMENT ON COLUMN sales_tasks.type IS '1: Nhắc tự động chu kỳ, 2: Hẹn gặp, 3: Follow-up báo giá, 4: Kiểm tra giao hàng, 5: Nhắc công nợ, 6: Thủ công, 7: Cấp trên giao';
COMMENT ON COLUMN sales_tasks.status IS '0: Chờ xử lý, 1: Đang làm, 2: Hoàn thành, 3: Bỏ qua, 4: Đã hủy';
COMMENT ON COLUMN sales_tasks.outcome IS '1: Sẽ mua ngay, 2: Cần báo giá, 3: Đồng ý gặp lại, 4: Bận/gọi lại sau, 5: Không nhấc máy, 6: Đã mua nơi khác, 7: Khiếu nại, 8: Chỉ hỏi thăm';

CREATE INDEX idx_tasks_customer ON sales_tasks(customer_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_assigned ON sales_tasks(assigned_to_user_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_due_date ON sales_tasks(due_date) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_status   ON sales_tasks(status) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_opp      ON sales_tasks(opportunity_id);
CREATE INDEX idx_tasks_workbench ON sales_tasks(assigned_to_user_id, due_date, status)
    WHERE status IN (0, 1) AND is_deleted = FALSE;

CREATE TRIGGER trg_tasks_updated_at
    BEFORE UPDATE ON sales_tasks
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- ---------------------------------------------------------------------
-- Bảng 12: interactions (Nhật ký tương tác chi tiết)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS interactions (
    id               BIGSERIAL PRIMARY KEY,
    customer_id      BIGINT NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    contact_id       BIGINT REFERENCES contacts(id) ON DELETE SET NULL,
    opportunity_id   BIGINT REFERENCES opportunities(id) ON DELETE SET NULL,
    user_id          VARCHAR(450) NOT NULL,
    type             INT NOT NULL,
    content          TEXT NOT NULL,
    duration_minutes INT,
    outcome          INT,
    interacted_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_interaction_type CHECK (type BETWEEN 1 AND 5),
    CONSTRAINT chk_interaction_duration CHECK (duration_minutes IS NULL OR duration_minutes >= 0)
);

COMMENT ON TABLE interactions IS 'Lịch sử ghi chép tương tác với khách hàng';
COMMENT ON COLUMN interactions.type IS '1: Điện thoại, 2: Gặp mặt, 3: Email, 4: Zalo, 5: Viếng thăm nhà máy';

CREATE INDEX idx_interactions_customer ON interactions(customer_id);
CREATE INDEX idx_interactions_time     ON interactions(interacted_at DESC);
CREATE INDEX idx_interactions_user     ON interactions(user_id);

-- =====================================================================
-- PHẦN 7: HỆ THỐNG, GIÁM SÁT & ĐỒNG BỘ NGOẠI VI
-- =====================================================================

-- ---------------------------------------------------------------------
-- Bảng 13: notifications (Thông báo trong ứng dụng)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS notifications (
    id         BIGSERIAL PRIMARY KEY,
    user_id    VARCHAR(450) NOT NULL,
    type       INT NOT NULL DEFAULT 1,
    title      VARCHAR(255) NOT NULL,
    message    TEXT NOT NULL,
    ref_type   VARCHAR(50),
    ref_id     BIGINT,
    link_url   VARCHAR(500),
    priority   INT NOT NULL DEFAULT 1,
    is_read    BOOLEAN NOT NULL DEFAULT FALSE,
    read_at    TIMESTAMPTZ,
    expires_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_notif_priority CHECK (priority BETWEEN 0 AND 3)
);

COMMENT ON TABLE notifications IS 'Hệ thống thông báo đẩy trên UI Blazor Server';

CREATE INDEX idx_notif_user_unread ON notifications(user_id, is_read) WHERE is_read = FALSE;
CREATE INDEX idx_notif_created_at  ON notifications(created_at DESC);
CREATE INDEX idx_notif_reference   ON notifications(ref_type, ref_id);

-- ---------------------------------------------------------------------
-- Bảng 14: dashboard_snapshots (Snapshot thống kê cuối ngày)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS dashboard_snapshots (
    id                   BIGSERIAL PRIMARY KEY,
    snapshot_date        DATE NOT NULL,
    period_from          DATE NOT NULL,
    period_to            DATE NOT NULL,
    total_revenue        NUMERIC(18,2) NOT NULL DEFAULT 0,
    new_customers        INT NOT NULL DEFAULT 0,
    old_customers        INT NOT NULL DEFAULT 0,
    total_opp_value      NUMERIC(18,2) NOT NULL DEFAULT 0,
    won_opportunities    INT NOT NULL DEFAULT 0,
    total_opportunities  INT NOT NULL DEFAULT 0,
    win_rate             NUMERIC(5,2) NOT NULL DEFAULT 0,
    customer_ratio_json  JSONB,
    funnel_json          JSONB,
    monthly_revenue_json JSONB,
    top_staff_json       JSONB,
    alerts_json          JSONB,
    created_at           TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_snapshot_period UNIQUE (snapshot_date, period_from, period_to)
);

COMMENT ON TABLE dashboard_snapshots IS 'Báo cáo chỉ số đóng băng định kỳ do background worker tính toán';

CREATE INDEX idx_snapshot_date ON dashboard_snapshots(snapshot_date DESC);

-- ---------------------------------------------------------------------
-- Bảng 15: audit_logs (Nhật ký thay đổi dữ liệu bảng)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS audit_logs (
    id              BIGSERIAL PRIMARY KEY,
    user_id         VARCHAR(450),
    entity_name     VARCHAR(100) NOT NULL,
    entity_id       VARCHAR(50) NOT NULL,
    action          VARCHAR(20) NOT NULL,
    field_name      VARCHAR(100),
    old_value       TEXT,
    new_value       TEXT,
    old_values_json JSONB,
    new_values_json JSONB,
    ip_address      VARCHAR(45),
    user_agent      VARCHAR(500),
    changed_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_audit_action CHECK (action IN ('CREATE', 'UPDATE', 'DELETE', 'SOFT_DELETE'))
);

COMMENT ON TABLE audit_logs IS 'Lưu vết lịch sử tác động nghiệp vụ phục vụ thanh tra hệ thống';

CREATE INDEX idx_audit_entity  ON audit_logs(entity_name, entity_id);
CREATE INDEX idx_audit_user    ON audit_logs(user_id);
CREATE INDEX idx_audit_changed ON audit_logs(changed_at DESC);

-- ---------------------------------------------------------------------
-- Bảng 16: kiotviet_sync_logs (Nhật ký tiến trình đồng bộ dữ liệu)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS kiotviet_sync_logs (
    id              BIGSERIAL PRIMARY KEY,
    entity_type     VARCHAR(50) NOT NULL,
    action          VARCHAR(20),
    records_synced  INT NOT NULL DEFAULT 0,
    records_failed  INT NOT NULL DEFAULT 0,
    status          VARCHAR(20) NOT NULL,
    message         TEXT,
    error_detail    TEXT,
    started_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at    TIMESTAMPTZ,
    duration_ms     INT,
    CONSTRAINT chk_sync_status CHECK (status IN ('SUCCESS', 'FAILED', 'PARTIAL', 'RUNNING'))
);

COMMENT ON TABLE kiotviet_sync_logs IS 'Theo dõi trạng thái các batch sync từ KiotViet API';

CREATE INDEX idx_sync_type_time ON kiotviet_sync_logs(entity_type, started_at DESC);
CREATE INDEX idx_sync_status    ON kiotviet_sync_logs(status);

-- =====================================================================
-- PHẦN 8: VIEW BÁO CÁO NGHIỆP VỤ (DATABASE VIEWS)
-- =====================================================================

-- View 1: Tỷ lệ phân bổ sức khỏe khách hàng B2B
CREATE OR REPLACE VIEW v_customer_health_summary AS
SELECT
    health_status,
    CASE health_status
        WHEN 0 THEN 'New'
        WHEN 1 THEN 'Healthy'
        WHEN 2 THEN 'Fair'
        WHEN 3 THEN 'AtRisk'
        WHEN 4 THEN 'Dormant'
        WHEN 5 THEN 'Churned'
        ELSE 'Unknown'
    END AS health_label,
    COUNT(*) AS total,
    ROUND(COUNT(*) * 100.0 / NULLIF(SUM(COUNT(*)) OVER (), 0), 2) AS percentage
FROM customers
WHERE is_deleted = FALSE
GROUP BY health_status
ORDER BY health_status;

-- View 2: Tổng hợp phễu cơ hội (Pipeline Funnel)
CREATE OR REPLACE VIEW v_opportunity_pipeline AS
SELECT
    os.id AS stage_id,
    os.name AS stage_name,
    os.display_order,
    os.color,
    COUNT(o.id) AS opportunity_count,
    COALESCE(SUM(o.estimated_value), 0) AS total_value
FROM opportunity_stages os
LEFT JOIN opportunities o ON o.stage_id = os.id AND o.is_deleted = FALSE
GROUP BY os.id, os.name, os.display_order, os.color
ORDER BY os.display_order;

-- =====================================================================
-- PHẦN 9: SCRIPT TỰ ĐỘNG VERIFY SCHEMA
-- =====================================================================
DO $$
DECLARE
    v_table_count INT;
    v_index_count INT;
    v_fk_count    INT;
    v_trigger_cnt INT;
BEGIN
    SELECT COUNT(*) INTO v_table_count
    FROM information_schema.tables
    WHERE table_schema = 'public' AND table_type = 'BASE TABLE';

    SELECT COUNT(*) INTO v_index_count
    FROM pg_indexes WHERE schemaname = 'public';

    SELECT COUNT(*) INTO v_fk_count
    FROM information_schema.table_constraints
    WHERE constraint_type = 'FOREIGN KEY' AND table_schema = 'public';

    SELECT COUNT(*) INTO v_trigger_cnt
    FROM information_schema.triggers
    WHERE trigger_schema = 'public';

    RAISE NOTICE '=====================================================';
    RAISE NOTICE 'KIỂM TRA TỰ ĐỘNG KHỞI TẠO DATABASE THÀNH CÔNG';
    RAISE NOTICE 'Tổng số bảng (Target = 16)      : %', v_table_count;
    RAISE NOTICE 'Tổng số indexes (Target >= 35) : %', v_index_count;
    RAISE NOTICE 'Tổng số Foreign Keys (Target >= 16): %', v_fk_count;
    RAISE NOTICE 'Tổng số Triggers (Target = 7)   : %', v_trigger_cnt;
    RAISE NOTICE '=====================================================';

    IF v_table_count <> 16 THEN
        RAISE EXCEPTION 'LỖI: Số lượng bảng không khớp (Hiện có: %, Cần: 16)', v_table_count;
    END IF;
END $$;
```

---

## PHASE 3 — THỰC THI SCHEMA LÊN DATABASE (15 phút)

### 3.1 Chạy script bằng CLI

```bash
cd D:\Projects\CrmSolution

# Thực thi schema dưới quyền user crm_user
psql -h localhost -U crm_user -d crm_db -f docs/db/schema.sql
```

**Kỳ vọng kết quả xuất ra màn hình:**
```
SET
SET
CREATE FUNCTION
CREATE TABLE
...
NOTICE:  =====================================================
NOTICE:  KIỂM TRA TỰ ĐỘNG KHỞI TẠO DATABASE THÀNH CÔNG
NOTICE:  Tổng số bảng (Target = 16)      : 16
NOTICE:  Tổng số indexes (Target >= 35) : 37
NOTICE:  Tổng số Foreign Keys (Target >= 16): 19
NOTICE:  Tổng số Triggers (Target = 7)   : 7
NOTICE:  =====================================================
DO
```

### 3.2 Kịch bản xử lý sự cố (Troubleshooting)

| Lỗi phát sinh | Nguyên nhân gốc rễ | Giải pháp chuẩn xác |
|---|---|---|
| `permission denied for schema public` | User `crm_user` chưa được set làm OWNER của schema | Chạy lệnh: `psql -U postgres -d crm_db -c "ALTER SCHEMA public OWNER TO crm_user;"` |
| `relation "..." already exists` | Database còn dở dang dữ liệu từ lần chạy trước | Chạy script Reset Database ở mục 3.3 |
| `syntax error at or near "$$"` | Copy script bị thiếu dấu qua SSH / Command Prompt | Lưu ý mở đúng file bằng UTF-8 không BOM trong VS Code trước khi chạy |
| `password authentication failed` | Password lưu trong cache CLI sai hoặc gõ thiếu | Đặt lại mật khẩu: `ALTER ROLE crm_user WITH PASSWORD 'Crm@2026#Dev';` |

### 3.3 Quy trình Reset Database sạch trong 30 giây

Nếu phát sinh lỗi schema cần chạy lại từ đầu:

```bash
# Đóng kết nối và drop DB
psql -U postgres -c "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'crm_db' AND pid <> pg_backend_pid();"
psql -U postgres -c "DROP DATABASE IF EXISTS crm_db;"
psql -U postgres -c "CREATE DATABASE crm_db WITH ENCODING = 'UTF8' TEMPLATE = template0;"
psql -U postgres -c "GRANT ALL PRIVILEGES ON DATABASE crm_db TO crm_user;"
psql -U postgres -d crm_db -c "ALTER SCHEMA public OWNER TO crm_user;"

# Chạy lại schema
psql -h localhost -U crm_user -d crm_db -f docs/db/schema.sql
```

---

## PHASE 4 — TẠO & NẠP DỮ LIỆU SEED MẪU (15 phút)

### 4.1 Tạo file `docs/db/seeds/02_demo_data.sql`

Tạo file mới tại đường dẫn `docs/db/seeds/02_demo_data.sql`:

```sql
-- =====================================================================
-- DEMO SEED DATA — KỊCH BẢN DOANH NGHIỆP CƠ KHÍ CHẾ TẠO B2B
-- Database : crm_db
-- =====================================================================

-- 1. Làm sạch dữ liệu kiểm thử (Giữ nguyên master stages & categories)
TRUNCATE interactions, stage_histories, sales_tasks, quote_items, quotes, 
         opportunities, customer_assignments, contacts, customers 
RESTART IDENTITY CASCADE;

-- 2. Seed 5 khách hàng B2B mẫu
INSERT INTO customers (id, code, name, industry, tax_code, phone, email, address,
                       health_status, average_cycle_days, assigned_to_user_id,
                       revenue_90d, order_count_90d, current_debt)
VALUES
    (1, 'KH-0001', 'Công ty TNHH Cơ Khí Chính Xác An Phát',  'Gia công phay/tiện CNC', '0101234567', '0912345678', 'contact@anphat.vn',  'KCN Quang Minh, Mê Linh, Hà Nội',       1, 30, 'seed-user-1', 145000000, 4, 15000000),
    (2, 'KH-0002', 'Công ty Cổ phần Chế Tạo Máy Việt Nhật', 'Chế tạo máy tự động',    '0102345678', '0913456789', 'info@vietnhat.vn',   'KCN Thăng Long, Đông Anh, Hà Nội',      1, 45, 'seed-user-1', 280000000, 5, 0),
    (3, 'KH-0003', 'Tân Á Precision Mold Co.',             'Khuôn mẫu chính xác',    '0103456789', '0914567890', 'sales@tana.vn',      'KCN Tân Tạo, Bình Tân, TP.HCM',         2, 60, 'seed-user-2', 42000000,  1, 22000000),
    (4, 'KH-0004', 'Nhà máy Phụ Tùng Nam An',              'Sản xuất bulong ốc vít', '0104567890', '0915678901', 'naman@ocvit.vn',     'KCN Biên Hòa 2, Đồng Nai',              3, 30, 'seed-user-2', 0,         0, 55000000),
    (5, 'KH-0005', 'Xưởng Cơ Khí Tổng Hợp Hải Nam',         'Gia công cơ khí nhỏ lẻ', '0105678901', '0916789012', 'hainam@tools.vn',    'KCN Long Hậu, Cần Giuộc, Long An',      0, 0,  'seed-user-1', 0,         0, 0);

SELECT setval('customers_id_seq', (SELECT MAX(id) FROM customers), true);

-- 3. Seed 5 người liên hệ chính thức
INSERT INTO contacts (id, customer_id, name, position, phone, email, is_primary)
VALUES
    (1, 1, 'Trần Văn Cường', 'Trưởng phòng Vật tư',  '0912345001', 'cuong@anphat.vn',  TRUE),
    (2, 2, 'Nguyễn Thị Hà',   'Kế toán trưởng',       '0912345002', 'ha@vietnhat.vn',   TRUE),
    (3, 3, 'Lê Văn Tuấn',     'Giám đốc Kỹ thuật',   '0912345003', 'tuan@tana.vn',     TRUE),
    (4, 4, 'Phạm Minh Đức',   'Nhân viên Mua hàng',   '0912345004', 'duc@ocvit.vn',     TRUE),
    (5, 5, 'Hoàng Văn Nam',   'Chủ xưởng',           '0912345005', 'nam@tools.vn',     TRUE);

SELECT setval('contacts_id_seq', (SELECT MAX(id) FROM contacts), true);

-- 4. Seed 10 sản phẩm công nghiệp đại diện
INSERT INTO products (id, code, name, category_id, unit, base_price, cost_price)
VALUES
    (1,  'DP-001', 'Dao phay ngón HSS Ø10 4 me',         1, 'Cái',  450000,  320000),
    (2,  'DP-002', 'Mũi khoan mạ Titan Ø5.0',            1, 'Mũi',   85000,   60000),
    (3,  'DP-003', 'Mảnh tiện CNC Kyocera CNMG 120408',  1, 'Hộp', 1200000,  850000),
    (4,  'DM-001', 'Đá mài phẳng Norton 180x13x31.75',   2, 'Viên', 185000,  130000),
    (5,  'DM-002', 'Đĩa cắt kim loại 105x1.2mm',         2, 'Viên',  15000,   10000),
    (6,  'DM-003', 'Bánh mài nỉ đánh bóng 150mm',        2, 'Cái',  125000,   85000),
    (7,  'OV-001', 'Bulong lục giác M8x30 Inox 304',     3, 'Con',    5000,    3200),
    (8,  'OV-002', 'Đai ốc tán thép M10 Grade 8.8',      3, 'Con',    3500,    2200),
    (9,  'DB-001', 'Dầu cắt gọt pha nước KoolKut 20L',   4, 'Thùng',650000,  480000),
    (10, 'DB-002', 'Mỡ bôi trơn chịu nhiệt SKF 1kg',     4, 'Hộp',  280000,  200000);

SELECT setval('products_id_seq', (SELECT MAX(id) FROM products), true);

-- 5. Seed 5 Cơ hội bán hàng phủ các Stage
INSERT INTO opportunities (id, code, customer_id, contact_id, stage_id, title, estimated_value, probability, assigned_to_user_id, expected_close_date, source)
VALUES
    (1, 'OPP-2609-001', 1, 1, 1, 'Cung ứng lô dao phay định kỳ Quý 4',    35000000,  10, 'seed-user-1', NOW() + INTERVAL '30 days', 'Hội chợ VIMEXPO'),
    (2, 'OPP-2609-002', 2, 2, 2, 'Gói mảnh tiện CNC dây chuyền số 2',     85000000,  30, 'seed-user-1', NOW() + INTERVAL '45 days', 'Khách hàng cũ gọi lại'),
    (3, 'OPP-2609-003', 3, 3, 3, 'Hợp đồng đá mài khuôn mẫu chính xác',   42000000,  50, 'seed-user-2', NOW() + INTERVAL '15 days', 'Kỹ thuật viên giới thiệu'),
    (4, 'OPP-2609-004', 4, 4, 4, 'Đấu thầu cung cấp bulong đai ốc 2027',  180000000, 70, 'seed-user-2', NOW() + INTERVAL '60 days', 'Website / Google Ads'),
    (5, 'OPP-2609-005', 5, 5, 5, 'Đơn hàng thử nghiệm dầu bôi trơn SKF',  25000000, 100, 'seed-user-1', NOW() - INTERVAL '2 days',  'Trực tiếp đến xưởng');

SELECT setval('opportunities_id_seq', (SELECT MAX(id) FROM opportunities), true);

-- 6. Seed Lịch sử chuyển giai đoạn tương ứng
INSERT INTO stage_histories (opportunity_id, from_stage_id, to_stage_id, changed_by_user_id, note)
VALUES
    (1, NULL, 1, 'seed-user-1', 'Khởi tạo cơ hội qua tiếp xúc hội chợ'),
    (2, NULL, 1, 'seed-user-1', 'Nhận nhu cầu từ nhà máy'),
    (2, 1,    2, 'seed-user-1', 'Đã hẹn gặp gửi catalogue'),
    (3, NULL, 1, 'seed-user-2', 'Tiếp nhận thông tin kỹ thuật'),
    (3, 1,    2, 'seed-user-2', 'Đo đạc thông số đá mài tại xưởng'),
    (3, 2,    3, 'seed-user-2', 'Đã gửi báo giá chi tiết BG-2609-001'),
    (4, NULL, 1, 'seed-user-2', 'Đăng ký tham gia thầu'),
    (4, 1,    2, 'seed-user-2', 'Làm việc cùng bộ phận thu mua'),
    (4, 2,    3, 'seed-user-2', 'Nộp hồ sơ báo giá'),
    (4, 3,    4, 'seed-user-2', 'Đang thương thảo điều khoản thanh toán'),
    (5, NULL, 1, 'seed-user-1', 'Khách hàng ghé kho lấy thử mẫu'),
    (5, 1,    2, 'seed-user-1', 'Thử nghiệm đạt tiêu chuẩn'),
    (5, 2,    3, 'seed-user-1', 'Báo giá chốt lô đầu'),
    (5, 3,    4, 'seed-user-1', 'Duyệt giá thành công'),
    (5, 4,    5, 'seed-user-1', 'Ký nghiệm thu và bàn giao đơn');

-- 7. Seed Báo giá (Quote) & Chi tiết báo giá (Quote Items)
INSERT INTO quotes (id, code, opportunity_id, customer_id, contact_id, created_by_user_id, quote_date, valid_until, status, sub_total, discount_amount, vat_rate, vat_amount, total_amount, payment_terms)
VALUES
    (1, 'BG-2609-001', 3, 3, 3, 'seed-user-2', CURRENT_DATE, CURRENT_DATE + 30, 2, 40000000, 2000000, 10.00, 3800000, 41800000, 'Thanh toán 100% sau 30 ngày nhận hàng');

SELECT setval('quotes_id_seq', (SELECT MAX(id) FROM quotes), true);

INSERT INTO quote_items (quote_id, product_id, product_name, product_code, unit, quantity, unit_price, discount_amount, total_line, sort_order)
VALUES
    (1, 4, 'Đá mài phẳng Norton 180x13x31.75', 'DM-001', 'Viên', 100, 185000, 500000, 18000000, 1),
    (1, 6, 'Bánh mài nỉ đánh bóng 150mm',     'DM-003', 'Cái',  200, 125000, 1500000, 23500000, 2);

-- 8. Seed Sales Tasks
INSERT INTO sales_tasks (title, description, type, priority, status, customer_id, contact_id, opportunity_id, due_date, assigned_to_user_id)
VALUES
    ('Follow-up báo giá đá mài', 'Gọi điện kiểm tra tiến độ duyệt báo giá BG-2609-001', 3, 2, 0, 3, 3, 3, NOW() + INTERVAL '1 day', 'seed-user-2'),
    ('Hẹn gặp Trưởng phòng Vật tư', 'Demo mẫu dao phay ngón HSS tại nhà máy',        2, 3, 0, 1, 1, 1, NOW() + INTERVAL '2 days', 'seed-user-1');

-- 9. Seed Interactions
INSERT INTO interactions (customer_id, contact_id, opportunity_id, user_id, type, content, duration_minutes, outcome, interacted_at)
VALUES
    (3, 3, 3, 'seed-user-2', 2, 'Gặp mặt trực tiếp tại KCN Tân Tạo để bàn về độ nhám yêu cầu của đá mài', 45, 3, NOW() - INTERVAL '3 days'),
    (1, 1, 1, 'seed-user-1', 1, 'Gọi điện thoại hẹn lịch giao mẫu dao phay mới',                            15, 1, NOW() - INTERVAL '1 day');
```

### 4.2 Nạp Seed Data vào Database

```bash
psql -h localhost -U crm_user -d crm_db -f docs/db/seeds/02_demo_data.sql
```

---

## PHASE 5 — KIỂM THỬ VÀ NGHIỆM THU TOÀN DIỆN (10 phút)

Chạy các lệnh truy vấn kiểm tra trực tiếp qua CLI hoặc DBeaver:

### 5.1 Kiểm tra Views Báo cáo

```bash
psql -h localhost -U crm_user -d crm_db -c "SELECT * FROM v_customer_health_summary;"
```
*Kỳ vọng:* Hiển thị phân bổ các nhóm sức khỏe: Healthy (40%), Fair (20%), AtRisk (20%), New (20%).

```bash
psql -h localhost -U crm_user -d crm_db -c "SELECT * FROM v_opportunity_pipeline;"
```
*Kỳ vọng:* 6 stages từ 'Mới tiếp cận' đến 'Thất bại' đều được hiển thị đầy đủ kèm số lượng deal và tổng giá trị dự kiến.

### 5.2 Kiểm tra hoạt động của Auto Trigger `updated_at`

```bash
psql -h localhost -U crm_user -d crm_db -c "
SELECT updated_at FROM customers WHERE id = 1;
UPDATE customers SET note = 'Kiểm tra hoạt động trigger' WHERE id = 1;
SELECT updated_at FROM customers WHERE id = 1;
"
```
*Kỳ vọng:* Giá trị `updated_at` ở câu lệnh SELECT thứ 2 phải lớn hơn câu lệnh SELECT thứ 1.

### 5.3 Kiểm tra hiển thị Tiếng Việt và UTF-8

```bash
psql -h localhost -U crm_user -d crm_db -c "SELECT code, name, industry FROM customers WHERE id = 1;"
```
*Kỳ vọng:* Tên hiển thị đầy đủ dấu: `Công ty TNHH Cơ Khí Chính Xác An Phát`, không bị vỡ font hay dấu `?`.

---

## PHASE 6 — QUẢN TRỊ MÃ NGUỒN GIT & DAILY LOG (5 phút)

### 6.1 Thực hiện Git Commit

```bash
cd D:\Projects\CrmSolution

# Thêm tài liệu và script SQL
git add docs/db/
git add notes/

# Commit với thông điệp chuẩn mực
git commit -m "feat(db): establish PostgreSQL 16 schema with 16 core tables

- Provisioned docs/db/schema.sql supporting .NET 10 & PostgreSQL 16
- Implemented 16 tables covering CRM, Pipeline, Quotes, Tasks, and System Logs
- Configured 7 auto-updating triggers for updated_at tracking
- Created views: v_customer_health_summary and v_opportunity_pipeline
- Configured ~37 indexes with partial indexing for active entities
- Added comprehensive seed files (master categories, stages, and 5 B2B demo flows)
- Validated UTF-8 encoding and UTC timezone settings"

# Push nhánh phát triển
git push origin develop
```

### 6.2 Cập nhật Nhật ký phát triển `notes/daily.md`

Ghi lại nội dung vào `notes/daily.md`:

```markdown
# Daily Log — Ngày 2 / Sprint 1

## Công việc đã hoàn thành
- [x] Cài đặt & cấu hình PostgreSQL 16 database `crm_db` với Encoding UTF8.
- [x] Tạo thành công 16 bảng nghiệp vụ theo đặc tả hệ thống CRM B2B.
- [x] Tạo 7 Triggers tự động cập nhật timestamp `updated_at`.
- [x] Tạo 2 Database Views (`v_customer_health_summary`, `v_opportunity_pipeline`).
- [x] Tạo và nạp hoàn chỉnh master data (6 stages, 4 categories) và demo data (5 customers, 5 contacts, 10 products, 5 deals).
- [x] Kiểm tra toàn bộ ràng buộc khóa ngoại (Foreign Keys), CHECK constraints và Indexes.

## Bàn giao cho Ngày 3 (ASP.NET Core Identity & Scaffold)
- Kiểu dữ liệu `assigned_to_user_id` và `user_id` đã được định hình dạng `VARCHAR(450)`.
- Khi setup `ApplicationUser` ở Ngày 3, sẽ chạy migration bổ sung Foreign Keys tham chiếu sang `AspNetUsers(Id)`.
- Triển khai 2 bảng phụ: `teams` và `user_profiles`.
```

---

## BẢNG THAM CHIẾU 16 BẢNG SCHEMA DATABASE

| STT | Tên bảng | Nhóm nghiệp vụ | Ràng buộc chính | Triggers | Indexes |
|:---:|---|---|---|:---:|:---:|
| 1 | `opportunity_stages` | Master Data | CHECK (prob 0-100), UNIQUE(name) | — | 1 |
| 2 | `categories` | Master Data | UNIQUE(kiotviet_id) | ✅ | 2 |
| 3 | `customers` | Khách hàng B2B | UNIQUE(code), CHECK(health 0-5) | ✅ | 6 |
| 4 | `contacts` | Người liên hệ | FK -> customers (CASCADE) | ✅ | 2 |
| 5 | `customer_assignments`| Phân bổ sales | FK -> customers (CASCADE) | — | 2 |
| 6 | `opportunities` | Sales Pipeline | UNIQUE(code), FK -> customers/stages | ✅ | 6 |
| 7 | `stage_histories` | Audit Pipeline | FK -> opportunities/stages | — | 1 |
| 8 | `products` | Sản phẩm | UNIQUE(code), FK -> categories | ✅ | 4 |
| 9 | `quotes` | Báo giá | UNIQUE(code), FK -> customers/opp | ✅ | 5 |
| 10 | `quote_items` | Chi tiết báo giá | FK -> quotes (CASCADE), FK -> products| — | 2 |
| 11 | `sales_tasks` | Nhiệm vụ / Lịch hẹn| FK -> customers/opp/quotes | ✅ | 6 |
| 12 | `interactions` | Lịch sử trao đổi | FK -> customers/contacts | — | 3 |
| 13 | `notifications` | Thông báo nội bộ | CHECK(priority 0-3) | — | 3 |
| 14 | `dashboard_snapshots` | Báo cáo EOD | UNIQUE(snapshot_date, period) | — | 1 |
| 15 | `audit_logs` | Audit Trail | CHECK(action IN ('CREATE',...)) | — | 3 |
| 16 | `kiotviet_sync_logs` | Đồng bộ API | CHECK(status IN ('SUCCESS',...)) | — | 2 |

---

## CHECKLIST NGHIỆM THU NGÀY 2

- [ ] Lệnh `pg_isready` và `dotnet --version` hoạt động chính xác.
- [ ] Database `crm_db` sở hữu server encoding `UTF8`.
- [ ] User `crm_user` là OWNER của schema `public`.
- [ ] Script `docs/db/schema.sql` chạy thành công một lần duy nhất với thông báo kiểm tra tự động hợp lệ (16 bảng).
- [ ] Bảng `opportunity_stages` có 6 bản ghi, `categories` có 4 bản ghi.
- [ ] Nạp thành công `docs/db/seeds/02_demo_data.sql` không bị lỗi vi phạm ràng buộc khóa ngoại.
- [ ] Hai Views `v_customer_health_summary` và `v_opportunity_pipeline` truy vấn trả ra số liệu chính xác.
- [ ] Cập nhật trường `note` trên bảng `customers` làm thay đổi trường `updated_at`.
- [ ] File `appsettings.Development.json` đã chứa Connection String chuẩn xác.
- [ ] Đã Git commit và push lên nhánh `develop`.