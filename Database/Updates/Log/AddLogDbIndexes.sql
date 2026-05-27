-- AddLogDbIndexes.sql
-- Adds performance indexes to the ace_log database.
-- Run ONCE after the base schema and all feature scripts are applied.
--
-- MySQL 8.0 compatible — no MariaDB-specific syntax.
-- CREATE INDEX IF NOT EXISTS is NOT supported by MySQL 8.0; idempotency is
-- achieved by querying information_schema.STATISTICS before each CREATE INDEX.

USE `ace_log`;

-- Helper: create an index only if it does not already exist.
-- Uses a prepared statement driven by an information_schema check so no
-- MariaDB-specific syntax is required.

-- account_session_log
SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'account_session_log'
    AND index_name = 'idx_account_session_log_accountId';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_account_session_log_accountId ON account_session_log (accountId)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

-- character_login_log
SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'character_login_log'
    AND index_name = 'idx_character_login_log_characterId';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_character_login_log_characterId ON character_login_log (characterId)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

-- arena_character_stats
SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_character_stats'
    AND index_name = 'idx_arena_character_stats_char';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_character_stats_char ON arena_character_stats (character_id)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_character_stats'
    AND index_name = 'idx_arena_character_stats_event';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_character_stats_event ON arena_character_stats (event_type)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_character_stats'
    AND index_name = 'idx_arena_character_stats_char_event';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_character_stats_char_event ON arena_character_stats (character_id, event_type)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_character_stats'
    AND index_name = 'idx_arena_character_stats_event_rank';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_character_stats_event_rank ON arena_character_stats (event_type, rank_points)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

-- arena_player
SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_player'
    AND index_name = 'idx_arena_player_event';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_player_event ON arena_player (event_id)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;
