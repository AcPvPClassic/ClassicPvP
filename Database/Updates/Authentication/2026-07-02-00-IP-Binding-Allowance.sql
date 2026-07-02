USE `ace_auth`;

-- Relax the global one-account-per-IP database constraint so that more than one
-- account may share an IP address (e.g. a household running two accounts).
--
-- Previously `uidx_ip` was a UNIQUE index on (ip_address) alone, which hard-blocked
-- any second account from ever binding an IP already claimed by another account.
-- The actual per-IP account limit is now enforced in application code via the
-- `ip_binding_ip_allowance` property (default 1 = original one-account-per-IP behavior),
-- so the DB only needs to prevent duplicate (ip_address, account_id) rows.
--
-- Idempotent: safe to re-run. Uses information_schema checks (MySQL 8.0 compatible —
-- no DROP INDEX IF EXISTS / CREATE INDEX IF NOT EXISTS, which are MariaDB-only).

-- Drop the single-column unique index on ip_address if it still exists.
SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'account_ip_binding'
    AND index_name = 'uidx_ip';
SET @s = IF(@x > 0,
  'ALTER TABLE `account_ip_binding` DROP INDEX `uidx_ip`',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

-- Add the composite unique index (ip_address, account_id) if it does not exist.
-- This still prevents duplicate rows for the same account+IP, while allowing
-- multiple distinct accounts to be bound to the same IP.
SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'account_ip_binding'
    AND index_name = 'uidx_ip_account';
SET @s = IF(@x = 0,
  'ALTER TABLE `account_ip_binding` ADD UNIQUE KEY `uidx_ip_account` (`ip_address`, `account_id`)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;
