-- =====================================================================================
-- HelpfulQueries.sql  --  ClassicPvP admin investigation queries
-- =====================================================================================
-- Focus: detecting PK alt-farming / allegiance abuse (feeding throwaway characters to
-- an allegiance mate to farm PK-quest rewards or PK-kills leaderboard rank).
--
-- DATABASES (adjust these names to match your Config.js "MySql" section if different):
--   Shard : classicpvp_shard   (character, biota_properties_i_i_d, biota_properties_int)
--   Auth  : classicpvp_auth     (account, account_ip_binding)
--   Log   : classicpvp_log                    (pk_kills_log)
-- Cross-database joins work because all three live on the same MySQL 8.0 instance.
-- If your names differ, find/replace: classicpvp_shard / classicpvp_auth / classicpvp_log
--
-- KEY SCHEMA FACTS
--   classicpvp_log.pk_kills_log        : killer_id, victim_id  = CHARACTER ids (biota guids).
--                                 killer_monarch_id / victim_monarch_id = each side's
--                                 monarch guid CAPTURED AT KILL TIME (NULL = solo / no allegiance).
--                                 killer_arena_player_id / victim_arena_player_id set => ARENA kill
--                                 (arena has its own ELO system; excluded from every query below).
--   character.account_Id        : links all characters on one account.
--   biota_properties_i_i_d      : type 26 = Monarch, type 25 = Patron. value = target guid.
--                                 A character's CURRENT allegiance key = its Monarch(26) value.
--   biota_properties_int        : type 25 = Level.
--
-- CAVEAT ON MONARCHS: these queries key allegiance identity on the Monarch(26) instance
-- property, exactly like the server's account-lock code (IsPledgable). A player who leads
-- their own allegiance but has no stored self-Monarch row will read as "solo". This only
-- affects a handful of top monarchs; their vassals' kills are still captured normally.
-- =====================================================================================


-- =====================================================================================
-- QUERY 1 -- Killing alts sitting on the accounts of your OWN allegiance mates
-- -------------------------------------------------------------------------------------
-- Flags kills where the KILLER was in an allegiance, and the VICTIM's account also holds
-- a (different) character sworn into that SAME allegiance -- i.e. an allegiance mate is
-- parking a throwaway character on their account and feeding it to the killer.
--
-- Read: high kill_count for a (killer, victim_account) pair = likely coordinated farming.
-- =====================================================================================
SELECT
    kl.killer_id,
    kc.name                              AS killer_name,
    kl.killer_monarch_id,
    va.accountId                         AS victim_account_id,
    va.accountName                       AS victim_account_name,
    kl.victim_id,
    vc.name                              AS victim_name,
    mate.id                              AS mate_char_id,
    mate.name                            AS allegiance_mate_on_victim_account,
    COUNT(*)                             AS kill_count,
    MIN(kl.kill_datetime)                AS first_kill,
    MAX(kl.kill_datetime)                AS last_kill
FROM classicpvp_log.pk_kills_log kl
    -- killer + victim character rows
    JOIN classicpvp_shard.`character` kc ON kc.id = kl.killer_id
    JOIN classicpvp_shard.`character` vc ON vc.id = kl.victim_id
    -- victim's account
    JOIN classicpvp_auth.account      va ON va.accountId = vc.account_Id
    -- another character on the victim's account whose CURRENT monarch = the killer's allegiance
    JOIN classicpvp_shard.`character` mate
         ON mate.account_Id = vc.account_Id
        AND mate.id <> vc.id
    JOIN classicpvp_shard.biota_properties_i_i_d mate_m
         ON mate_m.object_Id = mate.id
        AND mate_m.type = 26                       -- Monarch
        AND mate_m.value = kl.killer_monarch_id     -- ...belongs to the killer's allegiance
WHERE kl.killer_monarch_id IS NOT NULL
  AND kl.killer_arena_player_id IS NULL             -- exclude arena kills
  AND kl.victim_arena_player_id IS NULL
GROUP BY kl.killer_id, kc.name, kl.killer_monarch_id,
         va.accountId, va.accountName, kl.victim_id, vc.name, mate.id, mate.name
ORDER BY kill_count DESC, last_kill DESC;


-- =====================================================================================
-- QUERY 2 -- Accounts with characters in TWO different allegiances
-- -------------------------------------------------------------------------------------
-- The account-lock should make this impossible going forward, so any hit is either a
-- legacy account, an offlineswear/break edge case, or a bug worth investigating.
-- Allegiance identity = current Monarch(26) value; only characters that have a monarch
-- are considered (solo/unsworn characters are ignored).
-- =====================================================================================
SELECT
    c.account_Id,
    a.accountName,
    COUNT(DISTINCT m.value)                          AS distinct_allegiances,
    GROUP_CONCAT(DISTINCT m.value ORDER BY m.value)  AS monarch_ids,
    GROUP_CONCAT(DISTINCT c.name ORDER BY c.name SEPARATOR ', ') AS characters
FROM classicpvp_shard.`character` c
    JOIN classicpvp_shard.biota_properties_i_i_d m
         ON m.object_Id = c.id
        AND m.type = 26                              -- Monarch
    LEFT JOIN classicpvp_auth.account a ON a.accountId = c.account_Id
WHERE c.is_Deleted = 0
GROUP BY c.account_Id, a.accountName
HAVING COUNT(DISTINCT m.value) > 1
ORDER BY distinct_allegiances DESC, c.account_Id;


-- =====================================================================================
-- QUERY 3 -- P2 audit: unsworn victims whose account HAS an allegiance character
-- -------------------------------------------------------------------------------------
-- This is the exact "account-aware ineligibility" rule: a victim that is NOT in any
-- allegiance, but whose account holds at least one character that IS. These victims are
-- the throwaway-alt pattern regardless of who killed them. Shows how often each has been
-- killed and by whom, so you can spot who benefits.
-- =====================================================================================
SELECT
    vc.account_Id                        AS victim_account_id,
    va.accountName                       AS victim_account_name,
    vc.name                              AS victim_name,
    COALESCE(vlvl.value, 0)              AS victim_level,
    vc.total_Logins                      AS victim_total_logins,
    COUNT(*)                             AS times_killed,
    COUNT(DISTINCT kl.killer_id)         AS distinct_killers,
    GROUP_CONCAT(DISTINCT kc.name ORDER BY kc.name SEPARATOR ', ') AS killers
FROM classicpvp_log.pk_kills_log kl
    JOIN classicpvp_shard.`character` vc ON vc.id = kl.victim_id
    JOIN classicpvp_auth.account      va ON va.accountId = vc.account_Id
    JOIN classicpvp_shard.`character` kc ON kc.id = kl.killer_id
    -- victim is NOT currently in an allegiance
    LEFT JOIN classicpvp_shard.biota_properties_i_i_d vm
         ON vm.object_Id = vc.id AND vm.type = 26
    -- victim level (optional display)
    LEFT JOIN classicpvp_shard.biota_properties_int vlvl
         ON vlvl.object_Id = vc.id AND vlvl.type = 25   -- Level
WHERE kl.killer_arena_player_id IS NULL
  AND kl.victim_arena_player_id IS NULL
  AND vm.value IS NULL                                 -- victim unsworn
  -- ...but some OTHER character on the victim's account IS in an allegiance
  AND EXISTS (
        SELECT 1
        FROM classicpvp_shard.`character` sib
        JOIN classicpvp_shard.biota_properties_i_i_d sm
             ON sm.object_Id = sib.id AND sm.type = 26
        WHERE sib.account_Id = vc.account_Id
          AND sib.id <> vc.id
  )
GROUP BY vc.account_Id, va.accountName, vc.name, vlvl.value, vc.total_Logins
ORDER BY times_killed DESC;


-- =====================================================================================
-- QUERY 4 -- Repeat-kill concentration (farming volume), any allegiance
-- -------------------------------------------------------------------------------------
-- Ranks killer -> victim-account relationships by kill volume. Legit rivalry produces a
-- spread; farming shows one killer hammering one victim account many times. Tune the
-- HAVING threshold to your server's kill rate.
-- =====================================================================================
SELECT
    kc.name                              AS killer_name,
    kl.killer_id,
    vc.account_Id                        AS victim_account_id,
    va.accountName                       AS victim_account_name,
    COUNT(*)                             AS kills,
    COUNT(DISTINCT kl.victim_id)         AS distinct_victim_chars,
    MIN(kl.kill_datetime)                AS first_kill,
    MAX(kl.kill_datetime)                AS last_kill
FROM classicpvp_log.pk_kills_log kl
    JOIN classicpvp_shard.`character` kc ON kc.id = kl.killer_id
    JOIN classicpvp_shard.`character` vc ON vc.id = kl.victim_id
    JOIN classicpvp_auth.account      va ON va.accountId = vc.account_Id
WHERE kl.killer_arena_player_id IS NULL
  AND kl.victim_arena_player_id IS NULL
GROUP BY kc.name, kl.killer_id, vc.account_Id, va.accountName
HAVING kills >= 10                                    -- <-- tune
ORDER BY kills DESC, last_kill DESC;


-- =====================================================================================
-- QUERY 5 -- Shared-IP kills (killer & victim accounts seen from the same IP)
-- -------------------------------------------------------------------------------------
-- Strongest single collusion signal: the killer's account and the victim's account have
-- logged in from a common IP address. (Beware false positives: households / same LAN.)
-- Uses account_ip_binding, which records every IP an account has connected from.
-- =====================================================================================
SELECT DISTINCT
    kc.name           AS killer_name,
    ka.accountName    AS killer_account,
    vc.name           AS victim_name,
    va.accountName    AS victim_account,
    kib.ip_address    AS shared_ip,
    COUNT(*) OVER (PARTITION BY ka.accountId, va.accountId) AS kills_between_accounts
FROM classicpvp_log.pk_kills_log kl
    JOIN classicpvp_shard.`character` kc ON kc.id = kl.killer_id
    JOIN classicpvp_shard.`character` vc ON vc.id = kl.victim_id
    JOIN classicpvp_auth.account      ka ON ka.accountId = kc.account_Id
    JOIN classicpvp_auth.account      va ON va.accountId = vc.account_Id
    JOIN classicpvp_auth.account_ip_binding kib ON kib.account_id = ka.accountId
    JOIN classicpvp_auth.account_ip_binding vib
         ON vib.account_id = va.accountId
        AND vib.ip_address = kib.ip_address
WHERE ka.accountId <> va.accountId                    -- different accounts
  AND kl.killer_arena_player_id IS NULL
  AND kl.victim_arena_player_id IS NULL
ORDER BY kills_between_accounts DESC, killer_name;


-- =====================================================================================
-- QUERY 6 -- Throwaway-victim profile (low level / barely played, dying repeatedly)
-- -------------------------------------------------------------------------------------
-- Characters that exist mainly to die: low level, few logins, lots of PK deaths and
-- (near) zero kills of their own. Complements Query 3 (which keys on allegiance state);
-- this one keys purely on the "worthless victim" economics.
-- =====================================================================================
SELECT
    vc.account_Id                        AS victim_account_id,
    va.accountName                       AS victim_account_name,
    vc.name                              AS victim_name,
    COALESCE(vlvl.value, 0)              AS victim_level,
    vc.total_Logins,
    SUM(CASE WHEN kl.victim_id = vc.id THEN 1 ELSE 0 END) AS deaths,
    (SELECT COUNT(*) FROM classicpvp_log.pk_kills_log k2
       WHERE k2.killer_id = vc.id
         AND k2.killer_arena_player_id IS NULL) AS kills_dealt
FROM classicpvp_log.pk_kills_log kl
    JOIN classicpvp_shard.`character` vc ON vc.id = kl.victim_id
    JOIN classicpvp_auth.account      va ON va.accountId = vc.account_Id
    LEFT JOIN classicpvp_shard.biota_properties_int vlvl
         ON vlvl.object_Id = vc.id AND vlvl.type = 25   -- Level
WHERE kl.killer_arena_player_id IS NULL
  AND kl.victim_arena_player_id IS NULL
GROUP BY vc.account_Id, va.accountName, vc.name, vlvl.value, vc.total_Logins
HAVING deaths >= 5                                     -- <-- tune
   AND COALESCE(vlvl.value, 0) <= 20                   -- <-- tune "low level"
   AND kills_dealt = 0
ORDER BY deaths DESC;


-- =====================================================================================
-- QUERY 7 -- Sanity check: same-account kills (should be impossible)
-- -------------------------------------------------------------------------------------
-- One login per account + one account per person means killer and victim should never
-- share an account. Any row here indicates a rule was bypassed -- investigate.
-- =====================================================================================
SELECT
    kc.account_Id     AS account_id,
    a.accountName,
    kc.name           AS killer_name,
    vc.name           AS victim_name,
    kl.kill_datetime
FROM classicpvp_log.pk_kills_log kl
    JOIN classicpvp_shard.`character` kc ON kc.id = kl.killer_id
    JOIN classicpvp_shard.`character` vc ON vc.id = kl.victim_id
    LEFT JOIN classicpvp_auth.account a ON a.accountId = kc.account_Id
WHERE kc.account_Id = vc.account_Id
  AND kl.killer_arena_player_id IS NULL
  AND kl.victim_arena_player_id IS NULL
ORDER BY kl.kill_datetime DESC;


-- =====================================================================================
-- QUERY 8 -- XP integrity: available XP vs. (total XP - XP spent on skills/attributes)
-- -------------------------------------------------------------------------------------
-- The baseline AC bookkeeping identity is:
--
--     TotalExperience = AvailableExperience + SUM(XP spent on skills, attributes, vitals)
--
-- Every earn (total+=x, available+=x) and spend (available-=x, spent+=x) preserves it.
-- BUT the identity does NOT hold exactly, because of a deliberate offset:
--
--   *** Character-creation "trained skill" bonus (multiples of 526) ***
--   Each skill you pick as TRAINED at character creation is handed a 5-rank head start =
--   526 skill "PP" (biota_properties_skill.p_p), written straight into the skill's spent
--   total WITHOUT ever passing through Total/Available (Player.TrainSkill applyCreationBonusXP,
--   called from PlayerFactory). Specialized skills do NOT get this (their head start is an
--   innate InitLevel, not PP). So a perfectly healthy character reads:
--
--       available = (total - spent) + 526 * (number of trained-at-creation skills)
--
--   e.g. a char that trained 10 skills at creation shows discrepancy = +5,260 = 526 * 10.
--   This is exactly why the in-game `verify-xp` command ignores any diff that is a clean
--   multiple of 526. It is intended retail behavior, NOT corruption.
--
-- Other legitimate reasons the stored available can sit BELOW (total - spent) -- these are XP
-- sinks that leave AvailableExperience but are not captured in the p_p / c_P_Spent buckets, so
-- this query CANNOT see them and will show them as a negative or non-526 discrepancy:
--   * Augmentation purchases (AugmentationDevice deducts AvailableExperience directly)
--   * Enlightenment / character resets, admin XP adjustments, /delevel
-- For an aug-accurate, fully authoritative check, use the console command `verify-xp`
-- (DeveloperFixCommands) -- it models aug cost and the VerifyXp correction property too.
--
-- SCHEMA:
--   biota_properties_int64      : type 1 = TotalExperience, type 2 = AvailableExperience (value)
--   biota_properties_skill      : p_p       = XP spent on that skill
--   biota_properties_attribute  : c_P_Spent = XP spent on that attribute
--   biota_properties_attribute_2nd (vitals) : c_P_Spent = XP spent on that vital
--
-- This query reports the raw discrepancy but FILTERS OUT the benign creation-bonus rows
-- (positive multiples of 526), leaving only the characters worth a closer look:
--   discrepancy < 0                -> available is lower than even the naive baseline
--                                     (augmentation spend, or genuine XP loss)
--   discrepancy % 526 <> 0         -> a residual not explained by whole creation bonuses
-- implied_creation_bonuses = discrepancy / 526 should land on a small whole number (~ the
-- character's trained-skill count) for a clean character; fractions/odd values are the tell.
-- =====================================================================================
SELECT
    c.id                                 AS character_id,
    c.name                               AS character_name,
    c.account_Id,
    a.accountName,
    COALESCE(tot.value,   0)             AS total_xp,
    COALESCE(avail.value, 0)             AS available_xp,
    COALESCE(sk.spent, 0)                AS skill_xp_spent,
    COALESCE(att.spent, 0)               AS attribute_xp_spent,
    COALESCE(vit.spent, 0)               AS vital_xp_spent,
    COALESCE(sk.spent,0) + COALESCE(att.spent,0) + COALESCE(vit.spent,0) AS total_xp_spent,
    COALESCE(tot.value,0)
        - (COALESCE(sk.spent,0) + COALESCE(att.spent,0) + COALESCE(vit.spent,0)) AS expected_available_xp,
    COALESCE(avail.value,0)
        - (COALESCE(tot.value,0)
            - (COALESCE(sk.spent,0) + COALESCE(att.spent,0) + COALESCE(vit.spent,0))) AS discrepancy,
    ROUND(
      (COALESCE(avail.value,0)
        - (COALESCE(tot.value,0)
            - (COALESCE(sk.spent,0) + COALESCE(att.spent,0) + COALESCE(vit.spent,0)))) / 526.0, 3) AS implied_creation_bonuses
FROM classicpvp_shard.`character` c
    LEFT JOIN classicpvp_auth.account a ON a.accountId = c.account_Id
    LEFT JOIN classicpvp_shard.biota_properties_int64 tot
         ON tot.object_Id = c.id AND tot.type = 1          -- TotalExperience
    LEFT JOIN classicpvp_shard.biota_properties_int64 avail
         ON avail.object_Id = c.id AND avail.type = 2      -- AvailableExperience
    LEFT JOIN (SELECT object_Id, SUM(p_p)       AS spent FROM classicpvp_shard.biota_properties_skill        GROUP BY object_Id) sk
         ON sk.object_Id = c.id
    LEFT JOIN (SELECT object_Id, SUM(c_P_Spent) AS spent FROM classicpvp_shard.biota_properties_attribute     GROUP BY object_Id) att
         ON att.object_Id = c.id
    LEFT JOIN (SELECT object_Id, SUM(c_P_Spent) AS spent FROM classicpvp_shard.biota_properties_attribute_2nd GROUP BY object_Id) vit
         ON vit.object_Id = c.id
WHERE c.is_Deleted = 0
-- keep only rows NOT explained by whole trained-skill creation bonuses (526 * N, N >= 0)
HAVING discrepancy < 0 OR (discrepancy % 526) <> 0
ORDER BY discrepancy ASC;
