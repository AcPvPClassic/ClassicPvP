-- AddLogDbIndexes.sql
-- Adds performance indexes to the ace_log database.
-- Run ONCE after the base schema and all feature scripts are applied.
-- Uses CREATE INDEX IF NOT EXISTS (MySQL 8.0+) so it is safe to re-run.

USE `ace_log`;

-- account_session_log
CREATE INDEX IF NOT EXISTS idx_account_session_log_accountId
  ON account_session_log (accountId);

-- character_login_log
CREATE INDEX IF NOT EXISTS idx_character_login_log_characterId
  ON character_login_log (characterId);

-- arena_character_stats
CREATE INDEX IF NOT EXISTS idx_arena_character_stats_char
  ON arena_character_stats (character_id);

CREATE INDEX IF NOT EXISTS idx_arena_character_stats_event
  ON arena_character_stats (event_type);

CREATE INDEX IF NOT EXISTS idx_arena_character_stats_char_event
  ON arena_character_stats (character_id, event_type);

CREATE INDEX IF NOT EXISTS idx_arena_character_stats_event_rank
  ON arena_character_stats (event_type, rank_points);

-- arena_player
CREATE INDEX IF NOT EXISTS idx_arena_player_event
  ON arena_player (event_id);
