-- =====================================================================
-- CRM SYSTEM — DATABASE SCHEMA CHUẨN POSTGRESQL 16/18
-- Database: crm_db
-- =====================================================================
SET TIME ZONE 'UTC';
SET client_encoding = 'UTF8';

-- 1. TRIGGER FUNCTION CẬP NHẬT updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 2. MASTER DATA: opportunity_stages
CREATE TABLE opportunity_stages (
    id                  SERIAL PRIMARY KEY,
    name                VARCHAR(50) NOT NULL UNIQUE,
    display_order       INT NOT NULL,
    default_probability INT NOT NULL DEFAULT 0,
    color               VARCHAR(20),
    is_won              BOOLEAN DEFAULT FALSE,
    is_lost             BOOLEAN DEFAULT FALSE,
    is_closed           BOOLEAN DEFAULT FALSE,
    created_at          TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_probability_range CHECK (default_probability >= 0 AND default_probability <= 100)
);

INSERT INTO opportunity_stages (id, name, display_order, default_probability, color, is_won, is_lost, is_closed)
VALUES
    (1, 'Mới tiếp cận',    1, 10,  '#F5A623', FALSE, FALSE, FALSE),
    (2, 'Đang liên hệ',    2, 30,  '#E67E22', FALSE, FALSE, FALSE),
    (3, 'Đã gửi báo giá',  3, 50,  '#9B59B6', FALSE, FALSE, FALSE),
    (4, 'Đang đàm phán',   4, 70,  '#3498DB', FALSE, FALSE, FALSE),
    (5, 'Chốt thành công', 5, 100, '#2ECC71', TRUE,  FALSE, TRUE),
    (6, 'Thất bại',        6, 0,   '#E74C3C', FALSE, TRUE,  TRUE);

SELECT setval('opportunity_stages_id_seq', 6, true);

-- 3. MASTER DATA: categories
CREATE TABLE categories (
    id            BIGSERIAL PRIMARY KEY,
    kiotviet_id   BIGINT UNIQUE,
    name          VARCHAR(255) NOT NULL,
    description   TEXT,
    display_order INT DEFAULT 0,
    is_active     BOOLEAN DEFAULT TRUE,
    created_at    TIMESTAMPTZ DEFAULT NOW(),
    updated_at    TIMESTAMPTZ DEFAULT NOW()
);

INSERT INTO categories (name, description, display_order) VALUES
    ('Dao cụ',       'Dao phay, mũi khoan, mảnh tiện CNC',        1),
    ('Đá mài',       'Đá mài, đĩa cắt, bánh mài',                 2),
    ('Ốc vít',       'Bulong, đai ốc, vít các loại',              3),
    ('Dầu bôi trơn', 'Dầu cắt gọt, dầu thủy lực, mỡ công nghiệp', 4);

CREATE TRIGGER trg_categories_updated_at
    BEFORE UPDATE ON categories
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- 4. BẢNG customers
CREATE TABLE customers (
    id                  BIGSERIAL PRIMARY KEY,
    code                VARCHAR(50) NOT NULL UNIQUE,
    name                VARCHAR(255) NOT NULL,
    industry            VARCHAR(100),
    tax_code            VARCHAR(50),
    address             TEXT,
    phone               VARCHAR(20),
    email               VARCHAR(100),
    website             VARCHAR(200),
    assigned_to_user_id UUID,
    health_status       INT NOT NULL DEFAULT 0,
    average_cycle_days  INT DEFAULT 0,
    last_order_date     TIMESTAMPTZ,
    last_contact_date   TIMESTAMPTZ,
    next_contact_due    TIMESTAMPTZ,
    revenue_90d         NUMERIC(18,2) DEFAULT 0,
    order_count_90d     INT DEFAULT 0,
    current_debt        NUMERIC(18,2) DEFAULT 0,
    kiotviet_id         BIGINT UNIQUE,
    note                TEXT,
    is_active           BOOLEAN DEFAULT TRUE,
    is_deleted          BOOLEAN DEFAULT FALSE,
    deleted_at          TIMESTAMPTZ,
    created_at          TIMESTAMPTZ DEFAULT NOW(),
    updated_at          TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_health_status CHECK (health_status IN (0, 1, 2, 3, 4, 5)),
    CONSTRAINT chk_revenue_90d CHECK (revenue_90d >= 0)
);

CREATE INDEX idx_customers_code     ON customers(code);
CREATE INDEX idx_customers_health   ON customers(health_status) WHERE is_deleted = FALSE;
CREATE INDEX idx_customers_assigned ON customers(assigned_to_user_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_customers_next_due ON customers(next_contact_due) WHERE is_deleted = FALSE;
CREATE INDEX idx_customers_kiotviet ON customers(kiotviet_id);
CREATE INDEX idx_customers_active   ON customers(is_active) WHERE is_deleted = FALSE;

CREATE TRIGGER trg_customers_updated_at
    BEFORE UPDATE ON customers
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- 5. BẢNG contacts
CREATE TABLE contacts (
    id          BIGSERIAL PRIMARY KEY,
    customer_id BIGINT NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    name        VARCHAR(100) NOT NULL,
    position    VARCHAR(100),
    phone       VARCHAR(20),
    email       VARCHAR(100),
    birthday    DATE,
    is_primary  BOOLEAN DEFAULT FALSE,
    note        TEXT,
    created_at  TIMESTAMPTZ DEFAULT NOW(),
    updated_at  TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_contacts_customer ON contacts(customer_id);
CREATE INDEX idx_contacts_primary  ON contacts(customer_id, is_primary) WHERE is_primary = TRUE;

CREATE TRIGGER trg_contacts_updated_at
    BEFORE UPDATE ON contacts
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- 6. BẢNG customer_assignments
CREATE TABLE customer_assignments (
    id            BIGSERIAL PRIMARY KEY,
    customer_id   BIGINT NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    from_user_id  UUID,
    to_user_id    UUID,
    assigned_by   UUID,
    reason        TEXT,
    assigned_at   TIMESTAMPTZ DEFAULT NOW(),
    unassigned_at TIMESTAMPTZ
);

CREATE INDEX idx_assign_customer ON customer_assignments(customer_id);
CREATE INDEX idx_assign_user     ON customer_assignments(to_user_id);

-- 7. BẢNG opportunities
CREATE TABLE opportunities (
    id                  BIGSERIAL PRIMARY KEY,
    code                VARCHAR(50) NOT NULL UNIQUE,
    customer_id         BIGINT NOT NULL REFERENCES customers(id) ON DELETE RESTRICT,
    contact_id          BIGINT REFERENCES contacts(id) ON DELETE SET NULL,
    stage_id            INT NOT NULL REFERENCES opportunity_stages(id),
    title               VARCHAR(255) NOT NULL,
    description         TEXT,
    estimated_value     NUMERIC(18,2) DEFAULT 0,
    probability         INT DEFAULT 0,
    assigned_to_user_id UUID,
    expected_close_date TIMESTAMPTZ,
    actual_close_date   TIMESTAMPTZ,
    source              VARCHAR(100),
    is_won              BOOLEAN,
    loss_reason         INT,
    loss_note           TEXT,
    note                TEXT,
    is_deleted          BOOLEAN DEFAULT FALSE,
    deleted_at          TIMESTAMPTZ,
    created_at          TIMESTAMPTZ DEFAULT NOW(),
    updated_at          TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_probability     CHECK (probability >= 0 AND probability <= 100),
    CONSTRAINT chk_estimated_value CHECK (estimated_value >= 0),
    CONSTRAINT chk_loss_reason     CHECK (loss_reason IS NULL OR loss_reason IN (1,2,3,4,5))
);

CREATE INDEX idx_opp_customer ON opportunities(customer_id);
CREATE INDEX idx_opp_stage    ON opportunities(stage_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_opp_assigned ON opportunities(assigned_to_user_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_opp_expected ON opportunities(expected_close_date);
CREATE INDEX idx_opp_created  ON opportunities(created_at);
CREATE INDEX idx_opp_won      ON opportunities(is_won) WHERE is_won = TRUE;

CREATE TRIGGER trg_opp_updated_at
    BEFORE UPDATE ON opportunities
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- 8. BẢNG stage_histories
CREATE TABLE stage_histories (
    id                 BIGSERIAL PRIMARY KEY,
    opportunity_id     BIGINT NOT NULL REFERENCES opportunities(id) ON DELETE CASCADE,
    from_stage_id      INT REFERENCES opportunity_stages(id),
    to_stage_id        INT NOT NULL REFERENCES opportunity_stages(id),
    changed_by_user_id UUID,
    note               TEXT,
    changed_at         TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_history_opp ON stage_histories(opportunity_id);

-- 9. BẢNG products
CREATE TABLE products (
    id          BIGSERIAL PRIMARY KEY,
    kiotviet_id BIGINT UNIQUE,
    code        VARCHAR(50) NOT NULL,
    name        VARCHAR(255) NOT NULL,
    category_id BIGINT REFERENCES categories(id) ON DELETE SET NULL,
    unit        VARCHAR(50),
    base_price  NUMERIC(18,2) DEFAULT 0,
    cost_price  NUMERIC(18,2) DEFAULT 0,
    description TEXT,
    is_active   BOOLEAN DEFAULT TRUE,
    created_at  TIMESTAMPTZ DEFAULT NOW(),
    updated_at  TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_base_price CHECK (base_price >= 0),
    CONSTRAINT chk_cost_price CHECK (cost_price >= 0)
);

CREATE INDEX idx_products_code     ON products(code);
CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_products_kiotviet ON products(kiotviet_id);
CREATE INDEX idx_products_active   ON products(is_active) WHERE is_active = TRUE;

CREATE TRIGGER trg_products_updated_at
    BEFORE UPDATE ON products
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- 10. BẢNG quotes
CREATE TABLE quotes (
    id                 BIGSERIAL PRIMARY KEY,
    code               VARCHAR(50) NOT NULL UNIQUE,
    opportunity_id     BIGINT REFERENCES opportunities(id) ON DELETE SET NULL,
    customer_id        BIGINT NOT NULL REFERENCES customers(id) ON DELETE RESTRICT,
    contact_id         BIGINT REFERENCES contacts(id) ON DELETE SET NULL,
    created_by_user_id UUID,
    quote_date         DATE NOT NULL DEFAULT CURRENT_DATE,
    valid_until        DATE NOT NULL,
    status             INT NOT NULL DEFAULT 0,
    sub_total          NUMERIC(18,2) DEFAULT 0,
    discount_amount    NUMERIC(18,2) DEFAULT 0,
    vat_rate           NUMERIC(5,2) DEFAULT 10,
    vat_amount         NUMERIC(18,2) DEFAULT 0,
    total_amount       NUMERIC(18,2) DEFAULT 0,
    payment_terms      TEXT,
    note               TEXT,
    approved_by        UUID,
    approved_at        TIMESTAMPTZ,
    is_deleted         BOOLEAN DEFAULT FALSE,
    deleted_at         TIMESTAMPTZ,
    created_at         TIMESTAMPTZ DEFAULT NOW(),
    updated_at         TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_quote_status CHECK (status IN (0, 1, 2, 3, 4, 5)),
    CONSTRAINT chk_quote_total  CHECK (total_amount >= 0),
    CONSTRAINT chk_date_range   CHECK (valid_until >= quote_date)
);

CREATE INDEX idx_quotes_code       ON quotes(code);
CREATE INDEX idx_quotes_customer   ON quotes(customer_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_quotes_opp        ON quotes(opportunity_id);
CREATE INDEX idx_quotes_status     ON quotes(status) WHERE is_deleted = FALSE;
CREATE INDEX idx_quotes_quote_date ON quotes(quote_date DESC);

CREATE TRIGGER trg_quotes_updated_at
    BEFORE UPDATE ON quotes
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- 11. BẢNG quote_items
CREATE TABLE quote_items (
    id              BIGSERIAL PRIMARY KEY,
    quote_id        BIGINT NOT NULL REFERENCES quotes(id) ON DELETE CASCADE,
    product_id      BIGINT REFERENCES products(id) ON DELETE SET NULL,
    product_name    VARCHAR(255) NOT NULL,
    product_code    VARCHAR(50),
    unit            VARCHAR(50),
    quantity        NUMERIC(12,2) NOT NULL,
    unit_price      NUMERIC(18,2) NOT NULL,
    discount_amount NUMERIC(18,2) DEFAULT 0,
    total_line      NUMERIC(18,2) NOT NULL,
    sort_order      INT DEFAULT 0,
    note            TEXT,
    CONSTRAINT chk_quantity   CHECK (quantity > 0),
    CONSTRAINT chk_unit_price CHECK (unit_price >= 0),
    CONSTRAINT chk_discount   CHECK (discount_amount >= 0)
);

CREATE INDEX idx_quote_items_quote   ON quote_items(quote_id);
CREATE INDEX idx_quote_items_product ON quote_items(product_id);

-- 12. BẢNG sales_tasks
CREATE TABLE sales_tasks (
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
    assigned_to_user_id UUID,
    completed_by        UUID,
    outcome             INT,
    result_note         TEXT,
    is_deleted          BOOLEAN DEFAULT FALSE,
    deleted_at          TIMESTAMPTZ,
    created_at          TIMESTAMPTZ DEFAULT NOW(),
    updated_at          TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_task_type     CHECK (type IN (1, 2, 3, 4, 5, 6, 7)),
    CONSTRAINT chk_task_priority CHECK (priority IN (0, 1, 2, 3)),
    CONSTRAINT chk_task_status   CHECK (status IN (0, 1, 2, 3, 4)),
    CONSTRAINT chk_task_outcome  CHECK (outcome IS NULL OR outcome IN (1,2,3,4,5,6,7,8))
);

CREATE INDEX idx_tasks_customer ON sales_tasks(customer_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_assigned ON sales_tasks(assigned_to_user_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_due      ON sales_tasks(due_date) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_status   ON sales_tasks(status) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_opp      ON sales_tasks(opportunity_id);
CREATE INDEX idx_tasks_today    ON sales_tasks(assigned_to_user_id, due_date, status)
    WHERE status IN (0, 1) AND is_deleted = FALSE;

CREATE TRIGGER trg_tasks_updated_at
    BEFORE UPDATE ON sales_tasks
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- 13. BẢNG interactions
CREATE TABLE interactions (
    id               BIGSERIAL PRIMARY KEY,
    customer_id      BIGINT NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    contact_id       BIGINT REFERENCES contacts(id) ON DELETE SET NULL,
    opportunity_id   BIGINT REFERENCES opportunities(id) ON DELETE SET NULL,
    user_id          UUID,
    type             INT NOT NULL,
    content          TEXT NOT NULL,
    duration_minutes INT,
    outcome          INT,
    interacted_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_at       TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_interaction_type CHECK (type IN (1, 2, 3, 4, 5)),
    CONSTRAINT chk_duration         CHECK (duration_minutes IS NULL OR duration_minutes >= 0)
);

CREATE INDEX idx_interactions_customer ON interactions(customer_id);
CREATE INDEX idx_interactions_date     ON interactions(interacted_at DESC);
CREATE INDEX idx_interactions_user     ON interactions(user_id);

-- 14. BẢNG notifications
CREATE TABLE notifications (
    id         BIGSERIAL PRIMARY KEY,
    user_id    UUID,
    type       INT NOT NULL DEFAULT 1,
    title      VARCHAR(255) NOT NULL,
    message    TEXT NOT NULL,
    ref_type   VARCHAR(50),
    ref_id     BIGINT,
    link_url   VARCHAR(500),
    priority   INT DEFAULT 1,
    is_read    BOOLEAN DEFAULT FALSE,
    read_at    TIMESTAMPTZ,
    expires_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_notif_priority CHECK (priority IN (0, 1, 2, 3))
);

CREATE INDEX idx_notif_user_unread ON notifications(user_id, is_read) WHERE is_read = FALSE;
CREATE INDEX idx_notif_created     ON notifications(created_at DESC);
CREATE INDEX idx_notif_ref         ON notifications(ref_type, ref_id);

-- 15. BẢNG dashboard_snapshots
CREATE TABLE dashboard_snapshots (
    id                   BIGSERIAL PRIMARY KEY,
    snapshot_date        DATE NOT NULL,
    period_from          DATE NOT NULL,
    period_to            DATE NOT NULL,
    total_revenue        NUMERIC(18,2) DEFAULT 0,
    new_customers        INT DEFAULT 0,
    old_customers        INT DEFAULT 0,
    total_opp_value      NUMERIC(18,2) DEFAULT 0,
    won_opportunities    INT DEFAULT 0,
    total_opportunities  INT DEFAULT 0,
    win_rate             NUMERIC(5,2) DEFAULT 0,
    customer_ratio_json  JSONB,
    funnel_json          JSONB,
    monthly_revenue_json JSONB,
    top_staff_json       JSONB,
    alerts_json          JSONB,
    created_at           TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(snapshot_date, period_from, period_to)
);

CREATE INDEX idx_snapshot_date ON dashboard_snapshots(snapshot_date DESC);

-- 16. BẢNG audit_logs
CREATE TABLE audit_logs (
    id              BIGSERIAL PRIMARY KEY,
    user_id         UUID,
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
    changed_at      TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_audit_action CHECK (action IN ('CREATE', 'UPDATE', 'DELETE', 'SOFT_DELETE'))
);

CREATE INDEX idx_audit_entity  ON audit_logs(entity_name, entity_id);
CREATE INDEX idx_audit_user    ON audit_logs(user_id);
CREATE INDEX idx_audit_changed ON audit_logs(changed_at DESC);

-- 17. BẢNG kiotviet_sync_logs
CREATE TABLE kiotviet_sync_logs (
    id             BIGSERIAL PRIMARY KEY,
    entity_type    VARCHAR(50) NOT NULL,
    action         VARCHAR(20),
    records_synced INT DEFAULT 0,
    records_failed INT DEFAULT 0,
    status         VARCHAR(20) NOT NULL,
    message        TEXT,
    error_detail   TEXT,
    started_at     TIMESTAMPTZ DEFAULT NOW(),
    completed_at   TIMESTAMPTZ,
    duration_ms    INT,
    CONSTRAINT chk_sync_status CHECK (status IN ('SUCCESS', 'FAILED', 'PARTIAL', 'RUNNING'))
);

CREATE INDEX idx_sync_logs_type   ON kiotviet_sync_logs(entity_type, started_at DESC);
CREATE INDEX idx_sync_logs_status ON kiotviet_sync_logs(status);

-- 18. VIEWS
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
    END AS health_label,
    COUNT(*) AS total,
    ROUND(COUNT(*) * 100.0 / NULLIF(SUM(COUNT(*)) OVER (), 0), 2) AS percentage
FROM customers
WHERE is_deleted = FALSE
GROUP BY health_status
ORDER BY health_status;

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

-- 19. KHỐI TỰ ĐỘNG ĐỐI SOÁT CUỐI FILE
DO $$
DECLARE
    v_table_count INT;
    v_index_count INT;
    v_fk_count INT;
BEGIN
    SELECT COUNT(*) INTO v_table_count
    FROM information_schema.tables
    WHERE table_schema = 'public' AND table_type = 'BASE TABLE';

    SELECT COUNT(*) INTO v_index_count
    FROM pg_indexes WHERE schemaname = 'public';

    SELECT COUNT(*) INTO v_fk_count
    FROM information_schema.table_constraints
    WHERE constraint_type = 'FOREIGN KEY' AND table_schema = 'public';

    RAISE NOTICE '===========================================';
    RAISE NOTICE 'SCHEMA CREATED SUCCESSFULLY';
    RAISE NOTICE 'Total tables:  %', v_table_count;
    RAISE NOTICE 'Total indexes: %', v_index_count;
    RAISE NOTICE 'Total FKs:     %', v_fk_count;
    RAISE NOTICE '===========================================';
END $$;