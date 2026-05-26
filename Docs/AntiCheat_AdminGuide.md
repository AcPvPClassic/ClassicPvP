# ClassicPvP Anti-Cheat Admin Guide
# Discord-formatted reference — paste each section as a separate message

# ============================================================
# PASTE 1 OF 6 — Overview & Master Switches
# ============================================================

```
🛡️ **ClassicPvP Anti-Cheat — Admin Reference**

All checks are **off by default**. Set values in shard_config. Changes take effect within seconds, no restart needed. GodState players bypass all enforcement.

__Master Switches__ (enable these first)
`enforce_player_movement`       Core rubber-band engine. Rejects client positions that fail physics validation. Enable this to activate the system.
`enforce_player_movement_speed` Gates all scoring/logging checks below. Nothing in this guide fires unless this is ON.
`movement_violation_kick`       Kick players when violation counter ≥ 10. Score ≥ 50 always kicks regardless of this flag.
`movement_violation_webhook`    Discord webhook URL for real-time alerts. Leave blank to disable. (string value)
`movement_packet_rate_limit`    Max movement packets/sec before flood detection fires. Default: 30. (long value)
```

# ============================================================
# PASTE 2 OF 6 — Physics-Based Checks
# ============================================================

```
⚙️ **Physics-Based Checks** (all require enforce_player_movement_speed ON)

`enforce_player_movement_avg`
  3-second and 15-second sliding window average speed checks.
  Catches players who pace teleport packets to stay under the per-packet limit.
  Fires when avg speed over the window exceeds run rate × 1.15.

`enforce_player_movement_raycast`
  Geometry collision. Flags any position update where the physics engine
  could not reach the requested position without passing through solid
  geometry (wall-walk, out-of-bounds glitch, etc.).
  Scores +5 and rubber-bands on the first hit; a 2-second cooldown then
  suppresses follow-on hits at the same location so tight dungeon corridors
  cannot cascade a single wall-touch into an instant kick.  After 2 seconds
  of clean movement the cooldown resets and any new collision scores normally.

`enforce_player_jump_height`
  Jump apex cap. Calculates max legal jump height from the player's
  Strength and Jump skill via InqJumpVelocity. Fires if the player's
  tracked apex exceeds that height × 1.5 (50% fudge for lag/timing).
  Only checked within the same landblock to avoid dungeon coord issues.

`enforce_player_door_collision`
  Door ghost detection. Fires if the physics transition collided with
  a Door whose IsOpen == false. Highest suspicion weight (+8) because
  there is no legitimate way through a closed door.

`enforce_player_spawn_collision`
  Spawn grace-period detection. Fires if the physics transition collided
  with a living creature that spawned within the last 5 seconds.
  Lower weight (+4) because server-side spawn overlap can be coincidental.
```

# ============================================================
# PASTE 3 OF 6 — Script Detection Checks
# ============================================================

```
🤖 **Script Detection Checks** (all require enforce_player_movement_speed ON)

`enforce_player_timing_regularity`
  Inter-packet timing regularity. Measures the coefficient of variation
  (stddev ÷ mean) of movement packet intervals over a 4-second rolling
  window (min 13 entries / 3 seconds). Fires when CV < 0.015.
  Human hands: CV ≈ 0.15–0.40 from natural jitter.
  Legitimate AC client at fixed FPS: CV ≈ 0.02–0.06 (machine-clock loop).
  Scripts (near-zero jitter): CV < 0.005.
  Threshold 0.015 catches only true bot precision, not the client's own
  fixed-rate send loop. Do NOT raise above 0.04 or you will false-flag
  players on stable local/LAN connections.
  Observed column in log = measured CV. Allowed = 0.015.

`enforce_player_packet_rate`
  Packet flood. Counts movement packets received in the last 2 seconds.
  Fires when rate exceeds movement_packet_rate_limit (default 60/s).
  Normal AC client at 30 FPS ≈ 30/s; at 60 FPS ≈ 60/s.
  Scripted/flood clients: 100+/s.
  Default 60 leaves headroom for high-FPS legit clients. Scripts
  flooding at 100+/s are caught; players running at 60 FPS are not.
  Observed column = actual rate. Allowed = configured limit.

`enforce_player_reversal_detection`
  Inhuman direction reversal. Requires 4 consecutive buffer entries
  (3 steps). Fires ONLY when ALL of:
    • All three step intervals < 150 ms (human reaction time floor)
    • All three steps have real displacement (> 0.2 units)
    • Two consecutive heading changes are both within 20° of 180°
  A single quick reversal is never flagged. The double back-and-forth
  oscillation is the characteristic pattern of kiting/dodge scripts.
  Observed column = first reversal angle in radians. Allowed ≈ 2.79.
```

# ============================================================
# PASTE 4 OF 6 — Suspicion Score System
# ============================================================

```
📊 **Suspicion Score System**

Score accumulates each time a check fires. Decays −1.0 per heartbeat
tick when no violations are occurring. Resets when enforcement is toggled.

Score ≥ 50  → immediate kick, always, regardless of movement_violation_kick
Counter ≥ 10 + movement_violation_kick enabled → configurable kick

__Gain per violation type:__
Type                  Gain
─────────────────────────────────────────
speed_packet          (overage × 10), max 15  [borderline: ×0.5]
speed_avg_3s          proportional, max 8
speed_avg_15s         proportional, max 12
geometry              +5  (flat)
jump_height           (overage × 10), max 15
door_ghost            +8  (flat — highest weight)
spawn_ghost           +4  (flat — lowest weight)
script_timing         +6  (flat)
script_packet_rate    +4  (flat)
script_reversal       +7  (flat)
─────────────────────────────────────────
A genuine human player will never sustain violations long enough to
approach 50. The decay ensures brief false positives self-clear.
```

# ============================================================
# PASTE 5 OF 6 — Database Table
# ============================================================

```
🗄️ **DB: ace_log.movement_violation_log**

Column              Type           Notes
──────────────────────────────────────────────────────────────
id                  INT UNSIGNED   Auto-increment PK
character_id        INT UNSIGNED   Player GUID (indexed)
character_name      VARCHAR(255)
account_name        VARCHAR(255)   Indexed
violation_type      VARCHAR(64)    Indexed (see types below)
observed_speed      FLOAT          Measured value (units vary)
allowed_speed       FLOAT          Configured/computed limit
suspicion_score     FLOAT          Running score when fired
location            VARCHAR(512)   Landblock + XYZ string
violation_datetime  DATETIME       UTC, indexed

__violation_type values and what observed/allowed mean:__
speed_packet        dist/s observed vs max speed allowed
speed_avg_3s        avg dist/s vs run rate × 1.15
speed_avg_15s       avg dist/s vs run rate × 1.15
geometry            both 0 (position blocked, no numeric limit)
jump_height         apex delta-Z vs max allowed height (units)
door_ghost          both 0
spawn_ghost         both 0
script_timing       observed CV vs 0.04 threshold
script_packet_rate  observed packets/s vs configured limit
script_reversal     first reversal angle (rad) vs 2.79 (π−0.35)

Migration scripts:
  Database/Updates/Log/AddMovementViolationLog.sql   (initial table)
  Database/Updates/Log/AddViolationTypeColumn.sql    (adds violation_type)
```

# ============================================================
# PASTE 6 OF 6 — Webhook & Useful Queries
# ============================================================

```
📡 **Webhook: movement_violation_webhook**
Set this string key in shard_config to a Discord webhook URL.
Every violation fires a message in this format:

  [AntiCheat] **script_timing** | PlayerName (accountname) | Observed: 0.012 | Allowed: 0.040 | Suspicion: 18.0 | 0xABCD1234 [12.3, 45.6, 7.8]

No-ops silently if the URL is not set.

🔍 **Useful Queries (run against ace_log)**

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
HAVING peak >= 50
ORDER BY peak DESC;

-- All script-detection flags for an account:
SELECT * FROM movement_violation_log
WHERE account_name = 'ACCOUNT'
  AND violation_type IN ('script_timing','script_packet_rate','script_reversal')
ORDER BY violation_datetime;
```
