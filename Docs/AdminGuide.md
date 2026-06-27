# ClassicPvP — Admin Guide

All server properties are stored in `shard_config` and are readable/writable at runtime via `/modifybool`, `/modifylong`, `/modifydouble`, and `/modifystring`. Changes take effect within seconds — no restart needed unless noted. `GodState` accounts bypass most enforcement checks.

---

## Table of Contents

1. [One-Account-Per-IP Enforcement](#1-one-account-per-ip-enforcement)
2. [Rolling Level Cap](#2-rolling-level-cap)
3. [Rolling XP Modifier](#3-rolling-xp-modifier)
4. [PvP Damage Modifier Presets](#4-pvp-damage-modifier-presets)
5. [PvP XP on Player Kills](#5-pvp-xp-on-player-kills)
6. [XP Cap Categories](#6-xp-cap-categories)
7. [Hot Dungeons](#7-hot-dungeons)
8. [Town Control](#8-town-control)
9. [Season Leaderboard](#9-season-leaderboard)
10. [Tinkering Lotto](#10-tinkering-lotto)
11. [Tinker Character Designation](#11-tinker-character-designation)
12. [Discord Webhooks](#12-discord-webhooks)
13. [Anti-Cheat (Movement Enforcement)](#13-anti-cheat-movement-enforcement)
14. [Bounty System](#14-bounty-system)
15. [Admin Command Quick Reference](#15-admin-command-quick-reference)
16. [Loot-to-Weenie Export](#16-loot-to-weenie-export)
17. [Spell Management](#17-spell-management)

---

## 1. One-Account-Per-IP Enforcement

Each IP address may only be associated with one account. Accounts accumulate every IP they have ever logged in from; if any of those IPs is later used by a different account, that login is rejected. Players whose ISP rotates their IP, or who occasionally connect through a VPN by mistake, are not penalised — they simply add a new IP to their account's known set.

### How It Works

| Scenario | Behavior |
|---|---|
| First login from any IP | IP is bound to the account silently |
| Login from a previously seen IP | Login proceeds normally |
| Login from a new IP (not yet seen for this account) | New IP added to account's known-IP set; login proceeds |
| IP already claimed by a *different* account | Session terminated; player told to contact admin |

- Localhost (`127.0.0.1` / `::1`) and `Admin+` accounts are always exempt from all checks.
- Every new IP is logged to `account_ip_change_log` for audit purposes.

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `enforce_account_ip_binding` | bool | `true` | Master on/off for the IP binding system |
| `ip_binding_ip_whitelist` | string | `""` | Comma-separated list of IPs exempt from all binding enforcement (e.g. `192.168.1.1,10.0.0.5`). Accounts logging in from a whitelisted IP bypass the conflict check entirely. Use for LAN setups or trusted staff locations where multiple accounts sharing an IP is expected. |

### IP Whitelist

To allow multiple accounts from a shared IP (e.g. a home LAN, internet café, or staff office):

```
/modifystring ip_binding_ip_whitelist 192.168.1.100,203.0.113.42
```

To clear the whitelist:

```
/modifystring ip_binding_ip_whitelist 
```

Changes take effect immediately — no restart required.

### Admin Commands

| Command | Description |
|---|---|
| `/checkipbinding <account>` | Lists all known IPs for the account and recent IP change history |
| `/clearipbinding <account>` | Removes all IP bindings for the account. Player's next login creates a fresh binding. Use when an account is legitimately moving to a new household. |

### Database Tables (`ace_auth`)

| Table | Contents |
|---|---|
| `account_ip_binding` | One row per IP per account — accumulates every IP the account has ever used |
| `account_ip_change_log` | Audit log of every new IP seen per account |

---

## 2. Rolling Level Cap

The rolling cap advances the server-wide XP ceiling once per day using a three-phase schedule. Players at the cap stop earning XP until the cap advances and are notified when it does.

### Schedule

| Phase | Days | Rate | Milestone |
|---|---|---|---|
| Phase 1 | 0–14 | +3.00 levels/day | Level 57 at end of day 14 |
| Phase 2 | 15–44 | +1.50 levels/day | Level 101 at end of day 42 |
| Phase 3 | 45–59 | +1.40 levels/day | Level 126 (cap) at day 60 |
| Phase 4 | 60–120 | Linear XP growth | Season max XP reached at day 120 |
| Day 121+ | — | Frozen at `season_max_xp` | — |

The cap starts at **level 15** on day 0 (season launch). After level 126 the cap continues as a raw total-XP ceiling to cover the post-cap skill/attribute grind.

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `rolling_level_cap_enabled` | bool | `false` | Master on/off switch |
| `rolling_level_cap_start_timestamp` | long | `0` | Unix timestamp of season day 0 (UTC midnight). Set before starting |
| `season_max_xp` | long | `80,000,000,000` | Total XP ceiling at end of season (day 120) |
| `rolling_xp_cap` | long | — | **Auto-managed.** Current computed XP cap. Do not set manually |
| `rolling_xp_cap_timestamp` | long | — | **Auto-managed.** Timestamp of last recalculation |
| `pvp_dmg_mod_preset_applied_level` | long | `-1` | **Auto-managed.** Threshold of the last applied pvp_dmg_mod preset |

### Startup Procedure

1. Set `season_max_xp` to the desired end-of-season XP ceiling.
2. Optionally configure `rolling_xp_modifier_enabled` and `rolling_xp_modifier_max` (see Section 3).
3. Optionally create `pvp_dmg_mod_presets.json` (see Section 4).
4. Run `/startrollingcap` on launch day.

### Admin Commands

| Command | Description |
|---|---|
| `/startrollingcap` | Starts the season from today (UTC midnight). Enables the cap, sets `rolling_level_cap_start_timestamp`, forces immediate recalculation |
| `/forcerollingcap` | Forces an immediate recalculation. Use after changing `rolling_level_cap_start_timestamp` or `season_max_xp`. Also re-applies the rolling XP modifier and pvp_dmg_mod preset if enabled |
| `/rollingcapstatus` | Full status: enabled flag, start date, current XP cap, season day, progress %, and rolling XP modifier state |

### Tick Behavior

The manager runs every 15 minutes (`WorldManager.Tick`). It only takes action once per UTC calendar day. Each daily update:
1. Recalculates `rolling_xp_cap`
2. Updates `xp_modifier` if `rolling_xp_modifier_enabled` (Section 3)
3. Applies any pending pvp_dmg_mod preset (Section 4)

---

## 3. Rolling XP Modifier

Automatically adjusts the global `xp_modifier` each day as the season progresses. The modifier follows a quadratic curve — slow early, accelerating late — to reward players who stay active through the end of the season.

### Curve (default max = 3.0)

| Season Day | Approx. Level Cap | XP Modifier |
|---|---|---|
| 0 (launch) | 15 | **0.25×** |
| 7 | 36 | ~0.39× |
| 14 | 57 | ~0.52× |
| 21 | 69 | ~0.66× |
| **~44** | **101** | **1.0×** (normal rate) |
| 63 | 126 (level cap) | ~1.56× |
| 84 | post-cap grind | ~2.24× |
| **96** | post-cap grind | **3.0×** (peak) |
| 97–120 | post-cap grind | **3.0×** (held at cap) |

### How It Works

The curve is a quadratic `f(t) = a·t² + b·t + 0.25` where `t = daysSinceStart / 120`.  Coefficients are re-derived each tick from `rolling_xp_modifier_max`, so changing the max live is reflected on the next daily update without a restart. The floor is always 0.25 and the curve is capped at `rolling_xp_modifier_max`.

The three design anchors are:
- `t = 0.000` → 0.25 (season start, hardcoded floor)
- `t ≈ 0.364` → 1.0 (the "normal rate" crossover, at day ~44 / level ~101)
- `t = 0.800` → `rolling_xp_modifier_max` (day 96; capped from here through season end)

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `rolling_xp_modifier_enabled` | bool | `false` | Enable automatic daily `xp_modifier` updates. Requires `rolling_level_cap_enabled` |
| `rolling_xp_modifier_max` | double | `3.0` | Peak modifier applied from day 96 through season end |
| `xp_modifier` | double | `1.0` | **Managed automatically when enabled.** Do not set manually while the rolling modifier is active |

### Notes

- `/rollingcapstatus` shows the current value, expected value for today's day, and a sync warning if they differ.
- If the values drift (e.g. the modifier was set manually mid-season), run `/forcerollingcap` to resync.
- Disabling `rolling_xp_modifier_enabled` mid-season leaves `xp_modifier` at whatever value it was last set to. Reset it manually with `/modifydouble xp_modifier <value>` if needed.

---

## 4. PvP Damage Modifier Presets

Allows defining sets of `pvp_dmg_mod` property overrides that are automatically applied when the rolling level cap crosses a configured threshold. Useful for tightening damage balance as players get stronger.

### Configuration File

`pvp_dmg_mod_presets.json` — placed in the server output directory alongside the executable. The file is loaded at startup and can be hot-reloaded without a restart.

**Format:**
```json
{
  "Presets": [
    {
      "LevelThreshold": 50,
      "Description": "Early game — moderate restrictions",
      "Properties": {
        "pk_damage_modifier": 0.8,
        "pvp_dmg_mod_melee": 0.9
      }
    },
    {
      "LevelThreshold": 100,
      "Description": "Mid game — tighter caps",
      "Properties": {
        "pk_damage_modifier": 0.7
      }
    }
  ]
}
```

- Presets are sorted by `LevelThreshold` ascending at load time.
- The **active** preset is the one with the highest threshold ≤ current level cap.
- A preset is only applied once per threshold — `pvp_dmg_mod_preset_applied_level` tracks the last applied value across restarts.
- Any `Properties` key that doesn't exist in `PropertyManager` is silently skipped (logged as a warning).

### Admin Commands

| Command | Description |
|---|---|
| `/pvpdmgpresets` | Lists all loaded presets, which is active, and which has been applied |
| `/reloadpvpdmgpresets` | Hot-reloads `pvp_dmg_mod_presets.json` from disk. Does NOT re-apply — use `/applypvpdmgpreset` after if needed |
| `/applypvpdmgpreset [threshold]` | Force-applies the preset at the given threshold. Omit argument to apply the currently active preset for the live level cap |

---

## 5. PvP XP on Player Kills

Open-world PK kills award XP to the killer that flows into the PvP XP category (subject to the daily PvP budget and Ancient Bottle overflow).

### Formula

```
pvpXp = baseXp × randPercent × levelDecay
```

- **`baseXp`** = 1–4% of the killer's XP-to-next-level (configurable range)
- **`randPercent`** = random value in the `[pk_xp_min_percent, pk_xp_max_percent]` range
- **`levelDecay`** = `pk_xp_level_diff_decay ^ max(0, killerLevel − victimLevel)`
  - At decay=0.85: victim 10 levels below killer → ~0.85¹⁰ ≈ 20% reward
  - Same level or victim is higher: full reward

### Eligibility Guards

- Killer and victim must be in **different allegiances** (`IsSameAllegiance` check).
- Repeat-kill cooldown: the same killer cannot earn XP from the same victim again until `pk_xp_repeat_cooldown_minutes` elapses (in-memory, resets on restart).
- Hot Dungeon bonus: if the kill occurs inside an active Hot Dungeon, `pvpXp` is multiplied by the dungeon's `XpMultiplier` before being awarded.

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `pk_xp_level_diff_decay` | double | `0.85` | Exponential decay per level the victim is below the killer |
| `pk_xp_repeat_cooldown_minutes` | double | `60.0` | Minutes before the same killer earns XP from killing the same victim again |

---

## 6. XP Cap Categories

Player XP is divided into three independent buckets: **Monster**, **Quest**, and **PvP**. Each bucket has its own daily budget calculated as a fraction of the player's remaining XP headroom to the rolling cap.

### Budget Ratios

| Property | Type | Default | Description |
|---|---|---|---|
| `daily_monster_xp_category_ratio` | double | (configured) | Max fraction of remaining cap XP earnable from monster kills per window |
| `daily_quest_xp_category_ratio` | double | (configured) | Max fraction earnable from quests per window |
| `daily_pvp_xp_category_ratio` | double | `0.70` | Max fraction earnable from PvP (kills + arenas) per window |

Buckets reset when the rolling cap advances, not on a daily timer. Players further behind the cap get proportionally larger budgets.

**PvP overflow** goes into Ancient Bottles (WCID 490071). A bottle holds up to 100 million XP. Players consume it manually when their PvP budget has room.

---

## 7. Hot Dungeons

Up to 3 dungeons can be Hot simultaneously. Each is selected from a pool gated by current level cap brackets, runs for 24–48 hours, and grants bonus XP, double loot, and special PvP drops.

### Rewards While Hot

| Reward | Details |
|---|---|
| XP multiplier | Monster and PK kill XP × dungeon `XpMultiplier` (applied before fellowship sharing) |
| Double loot | Two independent loot rolls per monster corpse |
| A Box | Per-kill drop chance per dungeon's `BoxDropChance` config |
| PvP drop | Cross-allegiance PK kill inside the dungeon drops a Phial of Bloody Tears + A Box on the victim corpse |

### Dungeon Pool

Defined in `HotDungeonManager.cs` (`PossibleDungeons` list). Each entry has:
- `Landblock` — upper-16-bit landblock ID
- `MinLevel` / `MaxLevel` — cap range in which the dungeon is eligible (MaxLevel = 0 means no upper limit)
- `XpMultiplier` — kill XP multiplier while hot (e.g. `2.5` = 2.5× XP)
- `BoxDropChance` — per-kill Box drop probability (0.0–1.0)

> **⚠ Note:** The dungeon pool currently uses placeholder data. Real dungeon entries must be filled in before launch.

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `hot_dungeon_enabled` | bool | `false` | Master on/off switch. Requires Infiltration ruleset |
| `hot_dungeon_interval` | double | `7800` | Min seconds before a new dungeon can auto-roll after one was previously activated |
| `hot_dungeon_duration` | double | `7200` | Total seconds a hot dungeon stays active |
| `hot_dungeon_roll_delay` | double | `1200` | Seconds between each auto-roll attempt while slots are available |
| `hot_dungeon_chance` | double | `0.33` | Probability (0–1) a new dungeon is selected on each roll attempt |
| `hot_dungeon_bonus_xp` | double | `1.0` | Legacy extra XP flat bonus (1.0 = +100%). Prefer per-dungeon `XpMultiplier` |
| `hot_dungeon_webhook` | string | `""` | Discord webhook for Hot Dungeon activation/expiry announcements |

### Admin Commands

| Command | Description |
|---|---|
| `/SwitchHotDungeon` | Forces an immediate roll for a new Hot Dungeon |
| `/ForceHotDungeon` | Forces your current landblock to become a Hot Dungeon |
| `/ProlongHotDungeon` | Extends all active Hot Dungeons by 1 hour |
| `/hotdungeons` | *(player command)* Lists all currently active Hot Dungeons with XP multipliers and time remaining |

### Tick Behavior

- Initializes with a random first-roll delay of 30 min–3 hours after server start (so players don't wait 12+ hours on a fresh boot).
- Active dungeons expire independently; each tracks its own `ExpiresAt` timestamp.
- Hourly re-announcements fire for each active dungeon until it expires.

---

## 8. Town Control

Town Control is a structured PvP objective system. Eligible allegiances compete to control three towns: **Arwic**, **Al-Jalima**, and **Tou-Tou**. Control is contested by killing boss creatures; the killing allegiance captures the town and gains access to vendors and other rewards.

### Conflict Flow

1. An **init boss** spawns at a town. Any eligible allegiance can attack it.
2. When the init boss dies, the killing allegiance triggers a **conflict** and a **conflict boss** spawns.
3. When the conflict boss dies, the attacking allegiance **captures the town**.
4. Broadcasts and Discord notifications fire at each phase transition.
5. HP threshold broadcasts fire at 50%, 20%, and 5% remaining on the conflict boss.

### Eligibility

Only allegiances whose monarch GUID appears in `town_control_alleglist` can initiate conflicts. GUIDs are unsigned integers separated by commas.

```
/modifystring town_control_alleglist "1234567,8901234,5678901"
```

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `town_control_alleglist` | string | `""` | Comma-separated monarch GUIDs of eligible allegiances |
| `town_control_globals_webhook` | string | `""` | Discord webhook for conflict start/end broadcasts |
| `town_control_enable_debug_log` | bool | `false` | Writes verbose Town Control diagnostics to the server log (`TownControl` logger) |

### Database Tables (`ace_log`)

| Table | Contents |
|---|---|
| `town_control_town` | Current controller, last conflict timestamps, vendor state per town |
| `town_control_event` | Full audit log of every conflict event (phase, attacker, timestamp) |

> **Migration:** Run `Database/Updates/Log/AddTownControlFeature.sql` on any instance that doesn't have the tables yet.

---

## 9. Season Leaderboard

Weekly Sunday snapshots capture the top 10 players across 13 scored categories. Rewards are held until claimed with `/season rewards`.

### Admin Commands

| Command | Description |
|---|---|
| `/seasons forcemilestone` | Forces an immediate weekly milestone snapshot regardless of day. Broadcasts results in-game and to Discord |
| `/seasons resetcache` | Flushes all in-memory leaderboard and player standing caches |
| `/seasons status` | Shows current week number, last milestone date, cache entry counts, and active streak count |

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `season_cache_ttl_minutes` | long | `5` | How long (minutes) the top-10 cache is considered fresh before a DB re-fetch |
| `season_milestone_webhook` | string | `""` | Discord webhook for Sunday milestone announcements |

### ELO Decay

1v1 and 2v2 ELO decays **3% per day** after a player goes **3+ consecutive days without a match** in that format. Decay is written to the database each day so the stored ELO is always current. Playing a different arena format does not reset the decay clock for a specific format.

---

## 10. Tinkering Lotto

When enabled, every tinkering attempt has a chance to trigger a special bonus outcome beyond the normal tink result. The lotto fires at tink time (not at item creation) and sends a bonus message to the player if it triggers.

### Active Salvage Types

| Salvage | Lotto Effect |
|---|---|
| Steel | Bonus Armor Level (jackpot: +10 AL; normal: +1–5 AL); chance at Creature Resist/Slayer rating |
| Iron | Bonus +1 damage (capped at 1 per item) |
| Granite | Bonus +1 variance improvement |
| Opal | Bonus cast bonus |
| Mahogany | Bonus melee defense bonus |
| Velvet | Bonus melee defense bonus |
| Brass | Bonus range defense bonus |
| Aquamarine / Black Garnet / Emerald / Imperial Topaz / Jet / Red Garnet / White Sapphire | Resistance/Cleavage imbu |
| Sunstone / Fire Opal / Black Opal | ARC/SC/BL bonus |
| Zircon / Peridot / Yellow Topaz | Defense imbue bonus |

Green Garnet lotto is currently disabled (stub code exists, commented out).

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `tinker_lotto_enabled` | bool | `false` | Enables the Tinkering Lotto system |

---

## 11. Tinker Character Designation

Players can flag a level-1 character as a **Tinker** using `/FlagTinker`. This is a permanent, one-per-account designation that converts the character into a crafting specialist.

### What Gets Applied on Flag

- `GameplayMode` is set to `Tinker` (30000)
- All crafting skills are **auto-specialized and maxed** with XP:
  - Item Tinkering, Weapon Tinkering, Armor Tinkering, Magic Item Tinkering
  - Alchemy, Lockpick, Fletching, Cooking
- All combat/offensive skills are **untrained** (the character cannot function as a combatant)
- All attributes are **maxed** with XP and vitals are refreshed

### Restrictions on Tinker Characters

- Cannot train new skills (blocked at the server)
- Cannot specialize skills post-creation
- No vitae penalty on death

### Admin Notes

- The `/FlagTinker` command is player-issued but admins should be aware it's **irreversible**. There is no admin undo command — if a player flags incorrectly, a character rebuild via DB edit is the only recourse.
- The one-per-account guard checks all characters on the account, including offline ones. If the check needs to be bypassed for testing, it requires a direct DB intervention (clear `GameplayMode = 30000` on the existing Tinker character).

---

## 12. Discord Webhooks

All webhooks are set via string properties in `shard_config`. Leave empty to disable that channel. Each channel can route to a different Discord webhook URL.

| Property | What It Posts |
|---|---|
| `turbine_chat_webhook` | In-game general chat messages (General channel) |
| `turbine_chat_webhook_audit` | Admin/audit log events (admin commands, IP binding actions, cap changes, etc.) |
| `pk_kill_webhook` | Open-world PK kills (player kills and hardcore PKL kills) |
| `hot_dungeon_webhook` | Hot Dungeon activation, hourly re-announce, and expiry |
| `town_control_globals_webhook` | Town Control conflict start, boss deaths, capture events |
| `season_milestone_webhook` | Weekly Sunday season milestone snapshots and leader announcements |
| `arena_globals_webhook` | Arena match global broadcasts |
| `movement_violation_webhook` | Anti-cheat movement violation alerts (all types: speed, geometry, jump, door ghost, scripts) |

**Format for all channels:** Plain text messages via HTTP POST. The `DiscordWebhookManager` uses `HttpClient` with fire-and-forget async dispatch — webhook failures are logged but do not affect gameplay.

---

## 13. Anti-Cheat (Movement Enforcement)

All movement checks are **disabled by default**. Enable `enforce_player_movement_speed` first — nothing else in this section fires without it.

### Master Switches

| Property | Description |
|---|---|
| `enforce_player_movement` | Core rubber-band engine. Rejects client positions that fail physics validation |
| `enforce_player_movement_speed` | Gates all scoring and logging checks. Must be ON for anything below to fire |
| `movement_violation_kick` | Kick when violation counter ≥ 10. Score ≥ 50 always kicks regardless of this flag |
| `movement_violation_webhook` | Discord webhook URL for real-time alerts. Leave blank to disable |
| `movement_packet_rate_limit` | Max movement packets/sec before flood detection fires (default: 60) |

### Physics-Based Checks

| Property | What It Detects |
|---|---|
| `enforce_player_movement_avg` | 3-second and 15-second sliding window average speed. Fires when avg exceeds run rate × 1.15 |
| `enforce_player_movement_raycast` | Geometry collision — flags positions the physics engine cannot reach without passing through solid geometry (wall-walk, out-of-bounds). 2-second cooldown after first hit prevents cascade false kicks in tight corridors |
| `enforce_player_jump_height` | Jump apex cap via InqJumpVelocity(Strength, Jump). Fires if apex exceeds max height × 1.5 (50% lag fudge). Same-landblock only |
| `enforce_player_door_collision` | Door ghost detection. +8 score — highest weight. No legitimate way through a closed door |
| `enforce_player_spawn_collision` | Spawn overlap detection. +4 score — lowest weight. Server-side spawn timing can coincide |

### Script Detection Checks

| Property | What It Detects |
|---|---|
| `enforce_player_timing_regularity` | Inter-packet timing regularity. CV < 0.015 over a 4-second window flags bot-level precision. Human hands: CV ≈ 0.15–0.40. AC client fixed-rate: CV ≈ 0.02–0.06. Scripts: CV < 0.005. Do NOT raise above 0.04 |
| `enforce_player_packet_rate` | Packet flood. Fires above `movement_packet_rate_limit` (default 60/s). Normal client at 30 FPS ≈ 30/s; at 60 FPS ≈ 60/s; scripts: 100+/s |
| `enforce_player_reversal_detection` | Inhuman direction reversal. Requires 4 consecutive buffer entries. Fires only when ALL of: all three intervals < 150 ms, all three steps have real displacement (> 0.2 units), two consecutive headings both within 20° of 180°. Characteristic of kiting/dodge scripts |

### Suspicion Score System

Score accumulates each violation and decays −1.0 per heartbeat tick during clean movement.

| Violation Type | Score Gain |
|---|---|
| `speed_packet` | `overage × 10`, max 15 (borderline: ×0.5) |
| `speed_avg_3s` | proportional, max 8 |
| `speed_avg_15s` | proportional, max 12 |
| `geometry` | +5 |
| `jump_height` | `overage × 10`, max 15 |
| `door_ghost` | +8 |
| `spawn_ghost` | +4 |
| `script_timing` | +6 |
| `script_packet_rate` | +4 |
| `script_reversal` | +7 |

**Score ≥ 50** → immediate kick, always. **Counter ≥ 10** + `movement_violation_kick=true` → configurable kick.

### Database Table (`ace_log.movement_violation_log`)

| Column | Type | Notes |
|---|---|---|
| `id` | INT UNSIGNED | Auto-increment PK |
| `character_id` | INT UNSIGNED | Player GUID (indexed) |
| `character_name` | VARCHAR(255) | |
| `account_name` | VARCHAR(255) | Indexed |
| `violation_type` | VARCHAR(64) | Indexed |
| `observed_speed` | FLOAT | Measured value (units vary by type) |
| `allowed_speed` | FLOAT | Configured/computed limit |
| `suspicion_score` | FLOAT | Running score at time of violation |
| `location` | VARCHAR(512) | Landblock + XYZ string |
| `violation_datetime` | DATETIME | UTC, indexed |

### Useful Queries

```sql
-- All violations for a suspect, oldest first:
SELECT violation_datetime, violation_type, observed_speed, allowed_speed, suspicion_score, location
FROM movement_violation_log WHERE account_name = 'ACCOUNT' ORDER BY violation_datetime;

-- Top offenders last 7 days, grouped by type:
SELECT account_name, character_name, violation_type, COUNT(*) AS hits, MAX(suspicion_score) AS peak_score
FROM movement_violation_log
WHERE violation_datetime > DATE_SUB(NOW(), INTERVAL 7 DAY)
GROUP BY account_name, character_name, violation_type
ORDER BY hits DESC;

-- Accounts that ever crossed the kick threshold:
SELECT account_name, character_name, MAX(suspicion_score) AS peak
FROM movement_violation_log
GROUP BY account_name, character_name
HAVING peak >= 50 ORDER BY peak DESC;
```

---

## 14. Bounty System

| Property | Type | Default | Description |
|---|---|---|---|
| `bounty_system_enabled` | bool | `true` | Master on/off for the bounty system |
| `writ_of_pursuit_enabled` | bool | `true` | Enable Writs of Pursuit (player-placed custom bounties) |
| `bounty_allow_all_locations` | bool | `true` | Allow bounty contracts to be valid at any location (recommended for ClassicPvP) |
| `bounty_allow_logged_out` | bool | `false` | Allow offline players to be bounty targets |
| `bounty_pk_timer_active_enabled` | bool | `true` | Extend PK timer when a hunter is near their bounty target |
| `bounty_expirations_enabled` | bool | `true` | Enable contract expiration |
| `bounty_expiration_time` | long | `60` | Minutes until a contract expires after purchase |
| `bounty_cooldown_expiration_time` | long | `0` | Minutes a hunter must wait after turning in a bounty before buying another (0 = no cooldown) |

---

## 16. Loot-to-Weenie Export

Captures a live loot-generated item and writes it out as a permanent weenie SQL file in the content folder. Use this to freeze an interesting or well-rolled item into a static weenie that can be spawned, placed as a vendor item, or given as a quest reward.

### Usage

1. ID the item in-game (`Alt+click` or use the Assessment skill on it).
2. Run `@loot-to-weenie` as an admin.

The last item you ID'd is used automatically — no argument needed.

### What It Does

- Verifies the item has an `ItemWorkmanship` property (confirms it is loot-generated, not a static world weenie).
- Allocates the next available WCID in the custom range (≥ 1,000,000), queried live from the world database.
- Copies all Biota properties (Int, Bool, Float, String, DID, AnimPart, Palette, TextureMap, SpellBook) into a new weenie template. Live-object instance references (owner GUID, wielder GUID, container GUID) are **excluded** — these have no meaning in a static weenie.
- Exports the weenie as a `.sql` file into the content folder under the appropriate subfolder for the item's WeenieType/ItemType (e.g. `content/sql/weenies/MeleeWeapon/Sword/`).

### After Export

The SQL file is written to disk only — it is **not** automatically inserted into the world database. To make the weenie live:

```sql
SOURCE path/to/content/sql/weenies/.../filename.sql;
```

or import it via the ACE SQL import tooling, then restart or reload the weenie cache.

### Notes

- The command uses `CurrentAppraisalTarget`, which is the last item the admin's character appraised. If you've appraised multiple items in succession, only the most recent one is captured.
- WCID assignment is based on `MAX(class_Id)` among all weenies with class_Id ≥ 1,000,000 at the time the command runs. If two admins run the command simultaneously, both will read the same max and produce conflicting WCIDs — coordinate accordingly.
- The exported file name follows the standard convention: `{WCID} {Name} - {ClassName}.sql`.

---

## 17. Spell Management

### Grant School Spells

Grants all spells of a specific magic school and level to an online target player.

```
@grantschoolspells <player name> <school> <level>
```

| Argument | Valid Values |
|---|---|
| `player name` | Any online character name (multi-word names supported) |
| `school` | `War`, `Life`, `Creature`, `Item`, `Void` (case-insensitive) |
| `level` | `1` – `8` |

**Examples:**

```
@grantschoolspells Jimmy War 7
@grantschoolspells Jimmy Life 6
@grantschoolspells Some Player Creature 5
```

The target must be online. Spells are added silently (no purple particle effect per spell) but the player's spellbook updates immediately. Already-known spells are skipped.

> **Note:** For Infiltration ruleset, levels 1–7 are the valid range. Level 8 exists in the enum but no Infiltration spells are defined at that tier.

---

## 15. Admin Command Quick Reference

### Rolling Cap & XP

| Command | Summary |
|---|---|
| `/startrollingcap` | Start the season rolling cap from today |
| `/forcerollingcap` | Force-recalculate rolling_xp_cap (and xp_modifier if enabled) |
| `/rollingcapstatus` | Show cap status, season day, XP modifier state |
| `/pvpdmgpresets` | List pvp_dmg_mod presets and active one |
| `/reloadpvpdmgpresets` | Hot-reload pvp_dmg_mod_presets.json |
| `/applypvpdmgpreset [n]` | Force-apply preset at threshold n |

### Hot Dungeons

| Command | Summary |
|---|---|
| `/SwitchHotDungeon` | Force a new Hot Dungeon roll |
| `/ForceHotDungeon` | Make your current landblock Hot |
| `/ProlongHotDungeon` | Extend all active Hot Dungeons by 1 hour |

### Account / IP

| Command | Summary |
|---|---|
| `/checkipbinding <account>` | Show IP binding and change history |
| `/clearipbinding <account>` | Remove IP binding and reset monthly counter |

### Season

| Command | Summary |
|---|---|
| `/seasons forcemilestone` | Force a weekly milestone snapshot now |
| `/seasons resetcache` | Flush all leaderboard caches |
| `/seasons status` | Show season manager status |

### Spells

| Command | Summary |
|---|---|
| `@grantschoolspells <player> <school> <level>` | Grant all spells of a school+level to an online player |

### Content

| Command | Summary |
|---|---|
| `/loot-to-weenie` | Capture the last ID'd loot item as a weenie SQL file in the content folder |
