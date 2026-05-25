-- AddViolationTypeColumn.sql
-- Adds the violation_type column to movement_violation_log so that rows can be
-- distinguished by what kind of anti-cheat check fired (speed_packet, geometry,
-- script_timing, etc.).  Run ONCE after AddMovementViolationLog.sql.
-- Safe to re-run: ADD COLUMN IF NOT EXISTS is a no-op on MySQL 8.0+ if already present.

USE `ace_log`;

ALTER TABLE `movement_violation_log`
  ADD COLUMN IF NOT EXISTS `violation_type` VARCHAR(64) NOT NULL DEFAULT 'unknown'
  AFTER `account_name`;

-- Index lets admins quickly pull all rows of a given type for ban review.
CREATE INDEX IF NOT EXISTS `idx_mvl_violation_type`
  ON `movement_violation_log` (`violation_type`);
