-- =============================================================================
-- ClassicPvP Pre-Launch Wipe Script
-- =============================================================================
-- PURPOSE : Wipe all test accounts, characters, and items while preserving a
--           small whitelist. Resets the season to Day 0 / level-15 cap.
--
-- BEFORE YOU RUN:
--   1. STOP THE SERVER — the server caches shard data in memory. Running this
--      while the server is online will cause data corruption or partial wipes.
--   2. TAKE A BACKUP — mysqldump ace_auth ace_shard ace_log > backup.sql
--   3. Run Step 0 first and confirm exactly 4 rows before proceeding.
--
-- DATABASES TOUCHED: ace_auth, ace_shard, ace_log
-- =============================================================================

-- ─────────────────────────────────────────────────────────────────────────────
-- WHITELIST — accounts whose characters and items are preserved
-- ─────────────────────────────────────────────────────────────────────────────
--   Spacemonkeyadmin
--   spacemonkeya
--   spacemonkeyb
--   spacemonkeyc


-- =============================================================================
-- STEP 0: Pre-flight check — confirm whitelist accounts exist
-- =============================================================================
-- Run this block alone first. You should see exactly 4 rows.
-- If any account is missing, fix the name before continuing.

SELECT accountId, accountName, accessLevel
FROM ace_auth.account
WHERE LOWER(accountName) IN ('spacemonkeyadmin', 'spacemonkeya', 'spacemonkeyb', 'spacemonkeyc')
ORDER BY accountName;

-- =============================================================================
-- STEP 1: Build working sets of surviving account and character IDs
-- =============================================================================

DROP TEMPORARY TABLE IF EXISTS _keep_accounts;
CREATE TEMPORARY TABLE _keep_accounts AS
    SELECT accountId
    FROM ace_auth.account
    WHERE LOWER(accountName) IN ('spacemonkeyadmin', 'spacemonkeya', 'spacemonkeyb', 'spacemonkeyc');

DROP TEMPORARY TABLE IF EXISTS _keep_chars;
CREATE TEMPORARY TABLE _keep_chars AS
    SELECT id
    FROM ace_shard.`character`
    WHERE account_Id IN (SELECT accountId FROM _keep_accounts);

-- =============================================================================
-- STEP 2: ace_shard — delete non-whitelisted characters
--
-- CASCADE handles all character_properties_* child tables:
--   contract_registry, fill_comp_book, friend_list, quest_registry,
--   camp_registry, shortcut_bar, spell_bar, squelch, title_book
-- Also cascades biota_properties_allegiance rows keyed on character_Id.
-- =============================================================================

DELETE FROM ace_shard.`character`
WHERE id NOT IN (SELECT id FROM _keep_chars);

-- =============================================================================
-- STEP 3: ace_shard — delete all biotas not reachable from surviving characters
--
-- Uses a recursive CTE to walk the Container/Wielder ownership chain so that
-- surviving characters keep all their inventory items, equipped items, and
-- items inside containers they hold — nested to any depth.
--
--   PropertyInstanceId.Container = 2
--   PropertyInstanceId.Wielder   = 3
--
-- Deleting a biota row CASCADE-deletes all its biota_properties_* children.
-- =============================================================================

DROP TEMPORARY TABLE IF EXISTS _keep_biotas;
CREATE TEMPORARY TABLE _keep_biotas AS
    WITH RECURSIVE reachable (id) AS (
        -- Base: the surviving character biotas themselves
        SELECT id FROM _keep_chars

        UNION

        -- Recursive: items whose Container or Wielder resolves to something
        -- already in the reachable set
        SELECT iid.object_Id
        FROM ace_shard.biota_properties_i_i_d iid
        INNER JOIN reachable r ON iid.value = r.id
        WHERE iid.type IN (2, 3)
    )
    SELECT id FROM reachable;

DELETE FROM ace_shard.biota
WHERE id NOT IN (SELECT id FROM _keep_biotas);

-- =============================================================================
-- STEP 4: ace_shard — clean up house_permission orphans
-- (no FK cascade from house_permission to character)
-- =============================================================================

DELETE FROM ace_shard.house_permission
WHERE player_Id NOT IN (SELECT id FROM _keep_chars);

-- =============================================================================
-- STEP 5: ace_shard — truncate legacy shard-side log tables
-- These predate the ace_log migration; rows may still exist from testing.
-- =============================================================================

TRUNCATE TABLE ace_shard.pkkills;
TRUNCATE TABLE ace_shard.account_session_log;
TRUNCATE TABLE ace_shard.character_login_log;

-- =============================================================================
-- STEP 6: ace_auth — delete non-whitelisted accounts
-- =============================================================================

DELETE FROM ace_auth.account
WHERE accountId NOT IN (SELECT accountId FROM _keep_accounts);

-- =============================================================================
-- STEP 7: ace_auth — clear all IP binding data
-- Kept accounts will re-bind from their real IPs on first login.
-- =============================================================================

TRUNCATE TABLE ace_auth.account_ip_binding;
TRUNCATE TABLE ace_auth.account_ip_change_log;

-- =============================================================================
-- STEP 8: ace_log — truncate all log and leaderboard tables (full fresh start)
-- =============================================================================

TRUNCATE TABLE ace_log.tinker_log;
TRUNCATE TABLE ace_log.account_session_log;
TRUNCATE TABLE ace_log.character_login_log;
TRUNCATE TABLE ace_log.pk_kills_log;
TRUNCATE TABLE ace_log.arena_event;
TRUNCATE TABLE ace_log.arena_player;
TRUNCATE TABLE ace_log.arena_character_stats;
TRUNCATE TABLE ace_log.arena_team_stats;
TRUNCATE TABLE ace_log.rare_log;
TRUNCATE TABLE ace_log.stuck_character_log;
TRUNCATE TABLE ace_log.movement_violation_log;
TRUNCATE TABLE ace_log.town_control_event;
TRUNCATE TABLE ace_log.town_control_town;
TRUNCATE TABLE ace_log.allegiance_hometown_event;
TRUNCATE TABLE ace_log.allegiance_hometown_town;
TRUNCATE TABLE ace_log.allegiance_hometown_blacklist;
TRUNCATE TABLE ace_log.season_character_stats;
TRUNCATE TABLE ace_log.season_milestone;
TRUNCATE TABLE ace_log.season_milestone_leader;

-- =============================================================================
-- STEP 9: ace_shard — reset season / rolling level cap to Day 0
--
-- rolling_level_cap_start_timestamp → NOW (UTC) so Day 0 starts immediately.
-- rolling_xp_cap_timestamp          → 0 forces a recalculation on first server
--                                      tick; at Day 0 the cap resolves to the
--                                      level-15 XP floor per the schedule.
-- rolling_xp_cap                    → 0 (server overwrites on first tick).
-- pvp_dmg_mod_preset_applied_level  → 0 so the Day-1 PvP preset fires on
--                                      the first daily tick after restart.
-- xp_modifier                       → 0.25 (Day-0 quadratic curve floor;
--                                      server updates this once per day).
-- =============================================================================

UPDATE ace_shard.config_properties_long
SET value = UNIX_TIMESTAMP(UTC_TIMESTAMP())
WHERE `key` = 'rolling_level_cap_start_timestamp';

UPDATE ace_shard.config_properties_long
SET value = 0
WHERE `key` IN ('rolling_xp_cap', 'rolling_xp_cap_timestamp', 'pvp_dmg_mod_preset_applied_level');

UPDATE ace_shard.config_properties_double
SET value = 0.25
WHERE `key` = 'xp_modifier';

-- =============================================================================
-- STEP 10: Cleanup temp tables
-- =============================================================================

DROP TEMPORARY TABLE IF EXISTS _keep_accounts;
DROP TEMPORARY TABLE IF EXISTS _keep_chars;
DROP TEMPORARY TABLE IF EXISTS _keep_biotas;

-- =============================================================================
-- STEP 11: Verify
-- =============================================================================

-- Remaining accounts (expect exactly 4):
SELECT accountId, accountName, accessLevel
FROM ace_auth.account
ORDER BY accountName;

-- Remaining characters (expect only chars belonging to the 4 accounts):
SELECT c.id, c.name, c.account_Id, a.accountName
FROM ace_shard.`character` c
JOIN ace_auth.account a ON c.account_Id = a.accountId
ORDER BY a.accountName, c.name;

-- Season config (confirm start timestamp and zeroed caps):
SELECT `key`, value
FROM ace_shard.config_properties_long
WHERE `key` IN (
    'rolling_level_cap_start_timestamp',
    'rolling_xp_cap',
    'rolling_xp_cap_timestamp',
    'pvp_dmg_mod_preset_applied_level'
)
ORDER BY `key`;

SELECT `key`, value
FROM ace_shard.config_properties_double
WHERE `key` = 'xp_modifier';
