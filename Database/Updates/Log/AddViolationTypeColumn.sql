-- AddViolationTypeColumn.sql
-- Adds violation_type column + index to movement_violation_log.
-- Safe to re-run: information_schema guards prevent duplicate-column errors.
-- Compatible with MySQL 5.7+ and MySQL 8.x.

USE `ace_log`;

-- Add violation_type column only if it does not already exist.
SET @col_exists = (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = 'ace_log'
      AND TABLE_NAME   = 'movement_violation_log'
      AND COLUMN_NAME  = 'violation_type'
);
SET @add_col = IF(
    @col_exists = 0,
    'ALTER TABLE `movement_violation_log` ADD COLUMN `violation_type` VARCHAR(64) NOT NULL DEFAULT ''unknown'' AFTER `account_name`',
    'SELECT ''violation_type column already exists, skipping.'''
);
PREPARE stmt FROM @add_col;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Add index only if it does not already exist.
SET @idx_exists = (
    SELECT COUNT(*) FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA = 'ace_log'
      AND TABLE_NAME   = 'movement_violation_log'
      AND INDEX_NAME   = 'idx_mvl_violation_type'
);
SET @add_idx = IF(
    @idx_exists = 0,
    'ALTER TABLE `movement_violation_log` ADD INDEX `idx_mvl_violation_type` (`violation_type`)',
    'SELECT ''idx_mvl_violation_type already exists, skipping.'''
);
PREPARE stmt FROM @add_idx;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
