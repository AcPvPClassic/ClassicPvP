# 📜 ClassicPvP — Launch Release Notes

---

## 🕹️ Server Era — February 2005 (Infiltration Patch)

ClassicPvP is set in **February 2005**, specifically the **Infiltration patch era** of Asheron's Call. This is the classic, raw PvP experience — before years of power creep, skill consolidation, and late-game systems changed the game's identity.

### Weapon Skills
Weapons use the **original, pre-consolidation skill system**. There is no "Light Weapons" or "Heavy Weapons" umbrella — each weapon type has its own dedicated skill:

> Sword · Axe · Mace · Spear · Dagger · Staff · Bow · Crossbow · Thrown Weapons · Unarmed Combat

Every build that relies on weapons invests in a specific skill, which shapes both your identity and your spec choices meaningfully.

### What's NOT Here (EoR Systems)
The following systems belong to the **End of Retail** era and **do not exist** on ClassicPvP:

- ❌ Enlightenment system
- ❌ Void Magic, Summoning, Dual Wield, Two-Handed Combat, Sneak Attack, and other post-Infiltration skills
- ❌ Ratings
- ❌ Equipment Sets
- ❌ Level 8 Spells
- ❌ XP Augmentations
- ❌ Luminance / Luminance Augmentations
- ❌ Cloaks
- ❌ Aetheria
- ❌ Stipends

If you're coming from a more modern server, a lot of the late-game rating bloat is simply gone. Combat is cleaner.

---

## 🔒 Account Restrictions

ClassicPvP enforces a **strict one-account-per-player** policy, backed by the server itself — not just the rules.

- **One IP address, one account.** Each IP may only be bound to a single account. Playing multiple accounts from the same connection is not permitted.
- **IP binding is automatic.** Your IP is recorded on first login and locked to your account.
- **IP changes are limited.** You are allowed a limited number of IP changes per calendar month. Exceeding this limit triggers an **automatic account ban**.
- Legitimate IP changes (ISP changes, moving, etc.) can be appealed to an admin.

The intent is simple: no multi-boxing, no alt-army farming, no market manipulation through alts. Everyone plays on a level field.

---

## 📈 Rolling Level Cap

ClassicPvP uses a **server-wide rolling level cap**. Every player on the server shares the same ceiling — the maximum level you can achieve goes up on a set daily schedule, and no amount of grinding lets you get ahead of it. If you're at the cap, XP stops accumulating until the next advance. When that happens, you'll receive a chat message letting you know.

The cap opens at **level 15** on launch day. The early season moves fast — you're gaining several levels a day — then the pace slows as you approach the endgame.

**Week-by-week milestones:**

| Season Day | Level Cap |
|------------|-----------|
| Launch (Day 0) | **15** |
| Day 7 | 36 |
| Day 14 | 57 |
| Day 21 | 69 |
| Day 28 | 80 |
| Day 35 | 90 |
| Day 42 | 101 |
| Day 49 | 111 |
| Day 56 | 121 |
| **~Day 60** | **126 — level cap reached** |
| Days 60–120 | Post-cap XP grind |
| Day 121+ | Season cap frozen |

**What happens after level 126?**
The level cap tops out at 126 (the Infiltration-era maximum). Once that's reached, the rolling cap continues — but now as a raw XP ceiling rather than a level. This extra XP goes toward investing in skills and attributes beyond the level cap. This post-cap grind phase runs until approximately day 120, after which the ceiling is frozen for the rest of the season.

---

## ⏱️ XP Cap Categories

The rolling cap isn't just one number — your XP is divided into **three separate categories**, each with its own limit. You cannot reach the global cap by grinding a single activity type. You have to mix it up.

| Category | What earns into it |
|----------|--------------------|
| 🐉 **Monster** | Creature kills · Fellowship XP from kills · Allegiance passup XP · Proficiency XP |
| 📜 **Quest** | Quest completions · Exploration XP |
| ⚔️ **PvP** | Player kills · Arena match rewards · Other PvP Focused Custom Content |

**How the limits work:**
Each category has its own budget calculated from how much XP you still need to reach the current cap — your *remaining headroom*. You can earn up to a set portion of that headroom from each category before the bucket is full. Once a bucket fills, further XP of that type is blocked until the cap advances.

The global cap is still the ultimate ceiling. Maxing one category doesn't let you earn unlimited XP from the others — all three together still can't exceed your total remaining headroom for the current window.

**When do the buckets reset?**
Not at midnight. Not on a daily timer. They reset **when the rolling cap itself advances** — meaning when the server-wide level ceiling ticks up to the next level. When that happens, your buckets clear and your new budgets are calculated fresh based on however much XP gap remains between you and the new cap. Players who are further behind get proportionally larger budgets.

**Allegiance Passup XP**
XP passed up to you through your allegiance chain counts against your **Monster bucket**, the same pool as creature kills. If you're both actively grinding and receiving heavy passup from your vassals, those two sources compete for the same budget.

**PvP Overflow — Ancient Bottles**
PvP is the one category with a safety valve. If your PvP bucket is full — or you're at the global cap — any PvP XP you would have earned doesn't vanish. Instead, it is absorbed by **Ancient Bottles** in your inventory (if you have any). You can then use an Ancient Bottle later when your PvP budget has room, releasing its stored XP at that point. The bottle holds up to 100 million XP and tells you how full it is as it absorbs overflow.

### Checking Your Status

Use `/season status` to see a live snapshot of the current season:

- **Season day** — how many days have elapsed since launch
- **Level cap** — the current maximum level (or "post-cap XP grind" once level 126 is reached)
- **XP cap** — the exact total-XP ceiling in effect right now
- **Next advance** — hours and minutes until the cap ticks up again
- **XP budgets** — your Monster, Quest, and PK XP earned vs. your budget for this window, with a percentage and a `[FULL]` indicator when a bucket is exhausted

---

## 👑 Allegiance Passup XP

Allegiance XP works as it did in the Infiltration era. When your vassals earn XP, a portion accumulates for you as their patron. It is held until you log in, at which point it is delivered in a lump sum and you receive a message showing the amount.

A few things to know:
- Passup XP counts against your **Monster bucket** (same pool as creature kills). If you're actively grinding and also receiving heavy passup from your vassals, both compete for that budget.
- When passup XP is delivered to you, it **does not generate further passup** to your own patron up the chain.
- The amount of passup you can get at a time without spending it is 4.2 billion xp. If you accumulate that much and don't spend any you will start losing new earnings. 

---

## 🏟️ Arena System

The Arena is a **queue-based structured PvP system** that operates independently from open-world PK. You join a queue, get matched, get teleported in, fight, and receive rewards.

### Entering the Arena
Use the `/arena` command to interact with the queue.

**Requirements to join:**
- Must be **Player Killer (PK)** status
- Must **not** be PK-tagged (no active PK timer from a recent kill)
- Must be at least **level ** TODO

**Requirements to receive rewards:**
- Must be at least **level ** TODO
- Must have a minimum amount of in-game play time on your character TODO

### Arena Types

| Type | Format |
|------|--------|
| **1v1** | One vs. one duel |
| **2v2** | Two vs. two team duel |
| **FFA** | Free-for-All — up to 10 players, last one standing wins. At most 2 players from the same allegiance may be in the same match. |
| **Tugak** | Large Free-for-All — up to 15 players, last one standing wins. No allegiance limit per match. Prefers larger player counts before launching. Has its own separate quest achievement tracking. |
| **Group** | Team-based — organized fellowship vs. fellowship |

### Arena Rewards (Winners)

| Type | XP | PK Trophies | Phials of Bloody Tears | Arena Keys |
|------|-----|-------------|----------------------|------------|
| **1v1** | Level-proportional | 5 | 1 | 1 |
| **FFA** | Level-proportional (2×) | 5 | 3 | 5 |
| **Group** | — | 5 per member | 1 per member | — |

- Arena XP counts against your **PvP daily bucket**.
- Eliminated players should stay online until the match ends to be eligible for rewards.
- Rewards are scaled to your level range and the current rolling cap.

### Arena Ranking

Each arena format has its own leaderboard, all viewable with `/arena rank <type>`.

#### 1v1 — Composite Score
1v1 uses a **composite score** rather than raw ELO, designed to reward players who stay active rather than those who grind a good rating and stop queueing to protect it.

**Your score = ELO + (Wins × 8) + (Matches Played × 2)**

- **ELO** updates after every match based on the rating difference between you and your opponent. Starting ELO is 1500.
- **ELO decay** — if you stop playing, your ELO drops **3% per day** once you've gone **3 or more consecutive days without a 1v1 match**, floored at 1500. Decay is written directly to the database each day, so the stored ELO is always your current effective rating. Playing a 2v2 does **not** stop your 1v1 decay clock — each format is tracked independently.
- **Win bonus (+8 per win)** and **match bonus (+2 per match played)** mean an active player with a slightly lower ELO can outrank an inactive player with a higher one.

Use `/arena rank 1v1` to see the leaderboard.

#### 2v2 — Individual + Team Rankings
2v2 tracks two separate leaderboards:

**Individual** — same composite formula as 1v1, plus a **survival bonus**:
- **+30 per match where you were not eliminated** as part of the winning team
- Score = ELO + (Wins × 8) + (Matches × 2) + (Times Survived × 30)
- Decay rules are the same: 3% per day after a 3-day grace period, tracked separately from your 1v1 rating

**Team pairs** — your performance as a specific two-player combination is tracked separately. A team's score uses the same composite formula, with the team's ELO based on the average of both players' individual ELOs at match time.
- Winning teams gain ELO; losing teams lose ELO
- Team ELO also decays if the pair goes inactive; playing with a different partner does not stop the decay clock for this pair
- Survival bonus also applies at the team level

Use `/arena rank 2v2` for individual standings, `/arena rank 2v2team` for team pair standings.

#### FFA & Tugak — Placement Points
FFA and Tugak use a **points-based leaderboard**. Points accumulate across all events you participate in — there is no ELO and no decay.

| Finish Place | Points Awarded |
|---|---|
| 🥇 1st | **100** |
| 🥈 2nd | **50** |
| 🥉 3rd | **25** |
| 4th and beyond | **5** (participation) |
| Disqualified | **0** |

Use `/arena rank ffa` or `/arena rank tugak` to see those leaderboards.

---

## 🛡️ Enhanced Anti-Cheat

ClassicPvP runs a number of anti-cheat and anti-abuse systems beyond standard emulator defaults.

- **IP Binding** — as described above, accounts are hard-bound to an IP address. A second login from an IP already claimed by another account is flagged and acted upon automatically.
- **Comprehensive Server Logging** — the server runs a dedicated logging database that records:
  - All tinkering attempts (success and failure)
  - All PK kill events
  - All Arena match participation and results
  - All rare item drops
  - Account and character login/logout sessions
  - Stuck character force-logoff events
- This gives admins a full audit trail to investigate suspicious activity, item duplication concerns, or systemic exploits.
- Rate limiting is applied to exploit-sensitive player commands to prevent abuse through rapid automated input.

---

## 🎯 Bounty System

The Bounty System is a player-driven PvP economy that creates persistent, targeted hunting objectives on top of open-world PK combat.

### How It Works
1. Visit the **Bounty Hunter NPC** with a **Bounty Purchase Token**.
2. You receive a **Bounty Contract** for a randomly assigned eligible PK player (drawn from online players, excluding your own allegiance and players on cooldown with you).
3. Hunt your target. Kill them to mark the contract complete.
4. Return the completed contract to the Bounty Hunter NPC to collect your reward.

### Rules & Restrictions
- You must be in a **whitelisted allegiance** to participate in the bounty system.
- You cannot be assigned a target from your own allegiance.
- You cannot be assigned a target from the same IP address as you.
- There is a **maximum number of active contracts** you can hold at once.
- After turning in a completed contract, a **cooldown** prevents you from immediately purchasing another.
- Targets have a per-hunter cooldown — you cannot be repeatedly assigned the same player back-to-back.

### Proximity Mechanic
If you spot your bounty target in the world (or they spot you), their **PK timer refreshes** — preventing them from using portals or recalls to escape the encounter. Proximity to your hunter puts you at risk even if you haven't been directly attacked.

### Writs of Pursuit — High Priority Targets
Any player can place a **bounty with a custom reward** on a specific enemy using a **Writ of Pursuit**:

1. Obtain a Writ of Pursuit item.
2. Inscribe it in the format: `PlayerName:Amount`
3. Turn it in to the Bounty Hunter NPC along with the specified currency amount.
4. That player is flagged as a **High Priority Target** server-wide.
5. Any player who already has a contract on that target sees their contract upgraded.
6. The first bounty hunter to complete the contract receives the custom reward and a **server-wide broadcast**.

High Priority Targets have an increased chance of being assigned to new Bounty Contracts.

### Achievement Tracking
The bounty system tracks milestones over time, including:
- Unique players hunted
- Repeat contracts on the same target
- Speed completions (multiple kills within short windows)
- Kill streak targets broken (hunting players on hot streaks)

---

## 🗡️ Creature Slayer & Creature Resistance Ratings

These are **gear-based rating systems** active in the Infiltration ruleset, sourced from items and tinkering.

### Creature Slayer Rating
Increases your damage dealt to **a specific creature type** (e.g., Undead, Shadow, Lugian). The rating accumulates additively across all equipped items that carry it.

> Formula: `(100 + Slayer Rating) / 100 = damage multiplier against that creature type`
> Example: A Slayer Rating of 25 vs. Undead means +25% damage to Undead.

### Creature Resistance Rating
Reduces incoming damage **from a specific creature type**. Also gear-based and additive across equipment.

> Formula: `100 / (100 + Resist Rating) = incoming damage multiplier`
> Example: A Resist Rating of 25 vs. Shadow means you take ~80% of Shadow creature damage instead of 100%.

Both ratings only apply to **players** — monsters do not carry these ratings. They are a meaningful gearing consideration when farming specific content or building a focused PvE loadout.

Note: Many of the ratings introduced in later retail patches (Crit Rating, Damage Resistance Rating, Healing Boost Rating, etc.) **do not function** in this ruleset — they are entirely disabled. Creature Slayer and Creature Resist are among the few rating systems that **are** active and worth building around.

---

*This document will be updated as new systems and content are added. Stay tuned.*
