-- =====================================================================
-- USER PROFILES & TEAMS + FK CONSTRAINTS
-- Chạy SAU 02_identity_tables.sql
-- =====================================================================

CREATE TABLE IF NOT EXISTS teams (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(100) NOT NULL,
    manager_id      UUID REFERENCES "AspNetUsers"("Id") ON DELETE SET NULL,
    is_active       BOOLEAN DEFAULT TRUE,
    created_at      TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_teams_manager ON teams(manager_id);

CREATE TABLE IF NOT EXISTS user_profiles (
    user_id         UUID PRIMARY KEY REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE,
    full_name       VARCHAR(100) NOT NULL,
    employee_code   VARCHAR(20) UNIQUE,
    phone           VARCHAR(20),
    avatar_url      VARCHAR(500),
    role_code       VARCHAR(20) NOT NULL DEFAULT 'SALES',
    team_id         INT REFERENCES teams(id) ON DELETE SET NULL,
    monthly_target  NUMERIC(18,2) DEFAULT 0,
    is_active       BOOLEAN DEFAULT TRUE,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_role_code CHECK (role_code IN ('ADMIN', 'MANAGER', 'SALES', 'ACCOUNTANT'))
);
CREATE INDEX IF NOT EXISTS idx_user_profiles_role ON user_profiles(role_code);
CREATE INDEX IF NOT EXISTS idx_user_profiles_team ON user_profiles(team_id);

CREATE TRIGGER trg_user_profiles_updated_at
    BEFORE UPDATE ON user_profiles
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_name = 'fk_customers_user' AND table_name = 'customers') THEN
        ALTER TABLE customers
            ADD CONSTRAINT fk_customers_user
            FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id")
            ON DELETE SET NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_name = 'fk_opp_user' AND table_name = 'opportunities') THEN
        ALTER TABLE opportunities
            ADD CONSTRAINT fk_opp_user
            FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id")
            ON DELETE SET NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_name = 'fk_tasks_user' AND table_name = 'sales_tasks') THEN
        ALTER TABLE sales_tasks
            ADD CONSTRAINT fk_tasks_user
            FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id")
            ON DELETE RESTRICT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_name = 'fk_interactions_user' AND table_name = 'interactions') THEN
        ALTER TABLE interactions
            ADD CONSTRAINT fk_interactions_user
            FOREIGN KEY (user_id) REFERENCES "AspNetUsers"("Id")
            ON DELETE RESTRICT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_name = 'fk_quotes_user' AND table_name = 'quotes') THEN
        ALTER TABLE quotes
            ADD CONSTRAINT fk_quotes_user
            FOREIGN KEY (created_by_user_id) REFERENCES "AspNetUsers"("Id")
            ON DELETE RESTRICT;
    END IF;
END $$;