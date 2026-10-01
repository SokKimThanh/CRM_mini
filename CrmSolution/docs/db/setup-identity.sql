DO $$ BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='customers' AND column_name='assigned_to_user_id' AND data_type='character varying') THEN ALTER TABLE customers ALTER COLUMN assigned_to_user_id TYPE UUID USING NULL; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='opportunities' AND column_name='assigned_to_user_id' AND data_type='character varying') THEN ALTER TABLE opportunities ALTER COLUMN assigned_to_user_id TYPE UUID USING NULL; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='sales_tasks' AND column_name='assigned_to_user_id' AND data_type='character varying') THEN ALTER TABLE sales_tasks ALTER COLUMN assigned_to_user_id TYPE UUID USING NULL; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='interactions' AND column_name='user_id' AND data_type='character varying') THEN ALTER TABLE interactions ALTER COLUMN user_id TYPE UUID USING NULL; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='quotes' AND column_name='created_by_user_id' AND data_type='character varying') THEN ALTER TABLE quotes ALTER COLUMN created_by_user_id TYPE UUID USING NULL; END IF;
END $$;

CREATE TABLE IF NOT EXISTS "AspNetRoles" ("Id" UUID PRIMARY KEY, "Name" VARCHAR(256), "NormalizedName" VARCHAR(256), "ConcurrencyStamp" TEXT);
CREATE UNIQUE INDEX IF NOT EXISTS "RoleNameIndex" ON "AspNetRoles" ("NormalizedName") WHERE "NormalizedName" IS NOT NULL;
CREATE TABLE IF NOT EXISTS "AspNetUsers" (
    "Id" UUID PRIMARY KEY, "UserName" VARCHAR(256), "NormalizedUserName" VARCHAR(256),
    "Email" VARCHAR(256), "NormalizedEmail" VARCHAR(256), "EmailConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "PasswordHash" TEXT, "SecurityStamp" TEXT, "ConcurrencyStamp" TEXT, "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL DEFAULT FALSE, "TwoFactorEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "LockoutEnd" TIMESTAMPTZ, "LockoutEnabled" BOOLEAN NOT NULL DEFAULT FALSE, "AccessFailedCount" INT NOT NULL DEFAULT 0);
CREATE UNIQUE INDEX IF NOT EXISTS "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName") WHERE "NormalizedUserName" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
CREATE TABLE IF NOT EXISTS "AspNetUserRoles" ("UserId" UUID NOT NULL REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE, "RoleId" UUID NOT NULL REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE, PRIMARY KEY ("UserId","RoleId"));
CREATE TABLE IF NOT EXISTS "AspNetUserClaims" ("Id" SERIAL PRIMARY KEY, "UserId" UUID NOT NULL REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE, "ClaimType" TEXT, "ClaimValue" TEXT);
CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
CREATE TABLE IF NOT EXISTS "AspNetRoleClaims" ("Id" SERIAL PRIMARY KEY, "RoleId" UUID NOT NULL REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE, "ClaimType" TEXT, "ClaimValue" TEXT);
CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");
CREATE TABLE IF NOT EXISTS "AspNetUserLogins" ("LoginProvider" VARCHAR(128) NOT NULL, "ProviderKey" VARCHAR(128) NOT NULL, "ProviderDisplayName" TEXT, "UserId" UUID NOT NULL REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE, PRIMARY KEY ("LoginProvider","ProviderKey"));
CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
CREATE TABLE IF NOT EXISTS "AspNetUserTokens" ("UserId" UUID NOT NULL REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE, "LoginProvider" VARCHAR(128) NOT NULL, "Name" VARCHAR(128) NOT NULL, "Value" TEXT, PRIMARY KEY ("UserId","LoginProvider","Name"));

CREATE OR REPLACE FUNCTION update_updated_at_column() RETURNS TRIGGER AS $$ BEGIN NEW.updated_at = NOW(); RETURN NEW; END; $$ LANGUAGE plpgsql;

CREATE TABLE IF NOT EXISTS teams (
    id SERIAL PRIMARY KEY, name VARCHAR(100) NOT NULL,
    manager_id UUID REFERENCES "AspNetUsers"("Id") ON DELETE SET NULL,
    is_active BOOLEAN DEFAULT TRUE, created_at TIMESTAMPTZ DEFAULT NOW());
CREATE INDEX IF NOT EXISTS idx_teams_manager ON teams(manager_id);

CREATE TABLE IF NOT EXISTS user_profiles (
    user_id UUID PRIMARY KEY REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE,
    full_name VARCHAR(100) NOT NULL, employee_code VARCHAR(20) UNIQUE,
    phone VARCHAR(20), avatar_url VARCHAR(500), role_code VARCHAR(20) NOT NULL DEFAULT 'SALES',
    team_id INT REFERENCES teams(id) ON DELETE SET NULL, monthly_target NUMERIC(18,2) DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE, created_at TIMESTAMPTZ DEFAULT NOW(), updated_at TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_role_code CHECK (role_code IN ('ADMIN','MANAGER','SALES','ACCOUNTANT')));
CREATE INDEX IF NOT EXISTS idx_user_profiles_role ON user_profiles(role_code);
CREATE INDEX IF NOT EXISTS idx_user_profiles_team ON user_profiles(team_id);
DROP TRIGGER IF EXISTS trg_user_profiles_updated_at ON user_profiles;
CREATE TRIGGER trg_user_profiles_updated_at BEFORE UPDATE ON user_profiles FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DO $$ BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_customers_user') THEN ALTER TABLE customers ADD CONSTRAINT fk_customers_user FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id") ON DELETE SET NULL; END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_opp_user') THEN ALTER TABLE opportunities ADD CONSTRAINT fk_opp_user FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id") ON DELETE SET NULL; END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_tasks_user') THEN ALTER TABLE sales_tasks ADD CONSTRAINT fk_tasks_user FOREIGN KEY (assigned_to_user_id) REFERENCES "AspNetUsers"("Id") ON DELETE RESTRICT; END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_interactions_user') THEN ALTER TABLE interactions ADD CONSTRAINT fk_interactions_user FOREIGN KEY (user_id) REFERENCES "AspNetUsers"("Id") ON DELETE RESTRICT; END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name='fk_quotes_user') THEN ALTER TABLE quotes ADD CONSTRAINT fk_quotes_user FOREIGN KEY (created_by_user_id) REFERENCES "AspNetUsers"("Id") ON DELETE RESTRICT; END IF;
END $$;