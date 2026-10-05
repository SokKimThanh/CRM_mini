-- ============================================================================
-- V1.0.2: Bổ sung bảng teams, users và các cột CRM cho bảng customers
-- Tính chất: Idempotent — Có thể thực thi an toàn nhiều lần liên tiếp (K14, K53)
-- ============================================================================

CREATE TABLE IF NOT EXISTS sys_schema_history (
    installed_rank SERIAL PRIMARY KEY,
    version VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(200) NOT NULL,
    installed_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- ----------------------------------------------------------------------------
-- 1. Bảng teams: Cấu trúc tổ chức phòng ban và đội nhóm kinh doanh (K16, K74)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS teams (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID NOT NULL DEFAULT '00000000-0000-0000-0000-000000000001',
    name            VARCHAR(200) NOT NULL,
    parent_team_id  UUID NULL,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_teams_parent_team_id
        FOREIGN KEY (parent_team_id) REFERENCES teams(id) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ix_teams_tenant_parent
    ON teams (tenant_id, parent_team_id) WHERE is_deleted = FALSE;

-- ----------------------------------------------------------------------------
-- 2. Bảng users: Nhân sự kinh doanh và quản trị hệ thống (K16, K74)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS users (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID NOT NULL DEFAULT '00000000-0000-0000-0000-000000000001',
    team_id         UUID NULL,
    email           VARCHAR(200) NOT NULL,
    role            VARCHAR(20) NOT NULL DEFAULT 'SALES',
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_users_email UNIQUE (email),
    CONSTRAINT fk_users_team_id
        FOREIGN KEY (team_id) REFERENCES teams(id) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ix_users_tenant_team
    ON users (tenant_id, team_id) WHERE is_deleted = FALSE;

-- ----------------------------------------------------------------------------
-- 3. Bổ sung các cột CRM quản trị cho bảng customers (K20, K21, K39)
-- Ghi chú: Cột 'xmin' là system column sẵn có trong PostgreSQL, không dùng ADD COLUMN
-- ----------------------------------------------------------------------------
ALTER TABLE customers
  ADD COLUMN IF NOT EXISTS tenant_id    UUID         NOT NULL DEFAULT '00000000-0000-0000-0000-000000000001',
  ADD COLUMN IF NOT EXISTS owner_id     UUID         NULL,
  ADD COLUMN IF NOT EXISTS team_id      UUID         NULL,
  ADD COLUMN IF NOT EXISTS is_deleted   BOOLEAN      NOT NULL DEFAULT FALSE,
  ADD COLUMN IF NOT EXISTS deleted_at   TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by   UUID         NULL,
  ADD COLUMN IF NOT EXISTS updated_at   TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS updated_by   UUID         NULL;

-- ----------------------------------------------------------------------------
-- 4. Bổ sung các ràng buộc khóa ngoại tường minh cho bảng customers (K16)
-- ----------------------------------------------------------------------------
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_customers_team_id') THEN
        ALTER TABLE customers
            ADD CONSTRAINT fk_customers_team_id
                FOREIGN KEY (team_id) REFERENCES teams(id) ON DELETE RESTRICT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_customers_owner_id') THEN
        ALTER TABLE customers
            ADD CONSTRAINT fk_customers_owner_id
                FOREIGN KEY (owner_id) REFERENCES users(id) ON DELETE RESTRICT;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS ix_customers_tenant_owner
    ON customers (tenant_id, owner_id) WHERE is_deleted = FALSE;

CREATE INDEX IF NOT EXISTS ix_customers_tenant_team
    ON customers (tenant_id, team_id) WHERE is_deleted = FALSE;

-- ----------------------------------------------------------------------------
-- 5. Ghi vết lịch sử migration vào sys_schema_history (K14)
-- ----------------------------------------------------------------------------
INSERT INTO sys_schema_history (version, description)
VALUES ('V1.0.2', 'Add teams, users, CRM fields to customers')
ON CONFLICT (version) DO NOTHING;
