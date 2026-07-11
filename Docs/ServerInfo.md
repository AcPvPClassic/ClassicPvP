# 🗺️ ClassicPvP — Server Info & Mechanics Guide

> This is the **living reference** for how ClassicPvP works right now. It is kept current as mechanics change — older behavior is replaced in place rather than dated. For the history of changes over time, see **[ReleaseNotes.md](ReleaseNotes.md)**.

---

## 🚀 Getting Started

### Server Info

| Field | Value |
|-------|-------|
| **URL** | doctide.online |
| **Port** | 9000 |
| **Name** | Classic PvP |
| **Type** | ACE |

### Client Setup

1. Download the `.7z` file from [mega.nz/folder/xi4jiKjJ#jpuTVa7CQYyNxyp-UHC_GA](https://mega.nz/folder/xi4jiKjJ#jpuTVa7CQYyNxyp-UHC_GA)
2. Unzip it with 7-Zip (free utility — google it if you don't have it)
3. Go to `C:\Turbine` and make a copy of your `Asheron's Call` folder. Name the copy **ClassicPvP**
4. Copy the DAT and client (`.exe`) files you downloaded and paste them into `C:\Turbine\ClassicPvP`, overwriting the existing files
5. In **Thwarg Launcher**, click the three dots next to the client path at the bottom and select `C:\Turbine\ClassicPvP\acclient_Infiltration.exe`
6. You're ready — log in to ClassicPvP. To return to an End of Retail server like Doctide, switch the path back to `C:\Turbine\Asheron's Call\acclient.exe`

For more detail, see the full [Getting Started guide](GettingStarted.md).

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

- **One IP address, one account.** Each IP may only be associated with a single account. Playing multiple accounts from the same connection is not permitted.
- **IP tracking is automatic.** Every time you log in, your IP is recorded against your account. If your IP changes — because your ISP rotated it, you switched networks, or anything else — the new IP is simply added to your account's list and login proceeds normally. There is no penalty for IP changes.
- **What is blocked:** if you connect from an IP that is already registered to a *different* account, your login will be rejected and you will be prompted to contact an admin.

Common legitimate causes for an IP conflict: a household member plays on the same internet connection, you're connecting from a location another player has used (library, café, friend's house), or a VPN exit node was previously used by another player. Admins can review the binding history and resolve conflicts.

The intent is simple: no multi-boxing, no alt-army farming, no market manipulation through alts. Everyone plays on a level field.

> **For admins:** see **Section 1** of the Admin Guide for the `enforce_account_ip_binding` property, the IP whitelist, and the `/checkipbinding` and `/clearipbinding` commands.

---

## 🏠 Housing

ClassicPvP relaxes the retail purchase requirements so housing is broadly accessible:

- **No character level requirement.** You can buy any dwelling regardless of your level.
- **No account-age requirement.** The 15-day account age gate does not apply to any house.
- **No allegiance-rank requirement.** Mansions can be purchased without any allegiance rank.
- **No purchase cooldown.** The 30-day wait between purchases does not apply.
- **One house per character** (rather than one per account).

> **For admins:** the level requirement is controlled by the `house_min_level` property and the mansion rank requirement by `mansion_min_rank` (for each: `-1` uses the slumlord's value, `0` disables it, `>0` sets a custom minimum). ClassicPvP defaults both to `0`.

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

### 📊 Rolling XP Rate Bonus

As the season progresses and the level cap rises, the global XP rate bonus increases alongside it — rewarding players who stay active later in the season.

The bonus starts low in the opening days, holds near 1× (normal rate) through the mid-season, and then accelerates sharply toward the end. Players grinding during the final weeks of the season earn XP significantly faster than those who played only in the early days.

**How the rate scales:**

| Season Stage | Approx. Level Cap | XP Rate |
|---|---|---|
| Launch (Day 0) | 15 | **0.25×** |
| Day 7 | 36 | ~0.39× |
| Day 14 | 57 | ~0.52× |
| Day 21 | 69 | ~0.66× |
| Day ~44 | 101 | **1.0×** (normal rate) |
| Day 63 | 126 (level cap reached) | ~1.56× |
| Day 84 | post-cap XP grind | ~2.24× |
| **Day 96** | post-cap XP grind | **3.0×** |
| Days 96–120 | post-cap XP grind | **3.0×** (maintained) |

The rate accelerates on a quadratic curve — the gains are small at first and ramp up faster as the season matures. By the time level cap is reached (~day 60), you are already earning at 1.5× base rate. The last stretch of the season hits 3× and holds there through the final weeks.

The maximum rate (3×) is configurable by admins and may change between seasons.

**Custom PvP rewards are exempt from this rate.** Arena match XP, PK quest XP, hometown capture XP, and open-world PK kill XP are granted as fixed percentages of your level, so they are *not* multiplied by the rolling XP rate. They neither shrink in the low-rate opening days nor balloon when the rate climbs above 1× late in the season — a given PvP reward is worth the same fraction of a level all season long.

---

## ⏱️ XP Cap Categories

The rolling cap isn't just one number — your XP is divided into **three separate categories**, each with its own limit. You cannot reach the global cap by grinding a single activity type. You have to mix it up.

| Category | What earns into it |
|----------|--------------------|
| 🐉 **Monster** | Creature kills · Fellowship XP from kills · Allegiance passup XP · Proficiency XP |
| 📜 **Quest** | Quest completions · Exploration XP |
| ⚔️ **PvP** | Player kills · Arena match rewards · Other PvP Focused Custom Content |

**How the limits work:**
Each category has its own budget calculated from how much XP you still need to reach the current cap — your *remaining headroom*. You can earn up to the following portion of that headroom from each category before the bucket is full:

| Category | Budget (% of remaining headroom) |
|----------|----------------------------------|
| 🐉 Monster | 60% |
| 📜 Quest | 60% |
| ⚔️ PvP | 100% |

PvP is uncapped relative to the global ceiling — if you're willing to PK, you can fill your entire remaining headroom from PvP alone. Monster and Quest are each limited to 60%, so neither can carry you to the cap on its own. Once a bucket fills, further XP of that type is blocked until the cap advances.

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
- Passup cascades **automatically up the chain** the moment the XP is originally earned — your patron receives a share, their patron a smaller share, and so on up the tree (see the chain mechanics below). What does **not** happen is a *second* cascade when you personally collect your held passup: the lump sum delivered to you on login is not treated as freshly-earned XP, so receiving it does not generate new passup up your own chain.
- The amount of passup you can get at a time without spending it is 4.2 billion xp. If you accumulate that much and don't spend any you will start losing new earnings. 
- You may swear allegiance to a **lower-level patron**, but no passup is generated to them until they surpass their vassal's level. Once the patron out-levels the vassal, passup for that link begins automatically.

### XP Chain Mechanics (Loyalty & Leadership)

The amount of XP that passes through each link in the chain is determined by two skills — one on each end of the link.

**Loyalty** (vassal's skill) controls how much of the vassal's earned XP is *generated* for passup. **Leadership** (patron's skill) controls how much of that generated XP the patron actually *receives*. The final amount the patron gets is the product of both percentages — both sides of the link need to invest for maximum effect.

**Vassal → Patron (first hop):**
- Minimum: ~25% of earned XP passes up
- Maximum: ~90% of earned XP passes up
- Both skills cap at **291** for formula purposes (buffs count)

**Patron → Grandpatron (second hop and beyond):**
- Maximum: **10%** of whatever was received at the previous link
- Every subsequent hop applies the same reduced factors, so the chain burns out quickly regardless of skills

This reflects the behavior patched into the live servers on **January 12, 2004**. Before that patch, the second hop could pass up as much as 94%, making deep chains of well-spec'd characters extremely effective at funneling XP up to a monarch. After the patch (and on this server), the chain collapses after the first link. Loyalty and Leadership are still worth investing in for the direct vassal-to-patron link — 25% vs. 90% is a significant range — but building long XP chains to push XP deep up the tree is not viable. The second hop caps at 10% no matter what.

**Vassal count matters for Leadership.** A patron with only 1 vassal gets 25% of Leadership's bonus. The full benefit requires **4 or more vassals**.

---

## 👥 Swearing Allegiance to Same-Account Characters

You can swear allegiance to another character on your own account using the `/OfflineSwear <CharacterName>` command. Because you cannot have two characters logged in simultaneously, the target must be offline.

All normal allegiance rules apply — the target must be higher or equal level, must not already be your vassal, and the account-wide allegiance lock still applies (both characters must end up in the same monarch's chain).

`/OfflineSwear` is **exempt from the swear cooldown**: it never starts a cooldown and is never blocked by one, so you can freely organize your own account's characters into a chain at any time.

---

## 🤝 Allegiance Swear Restrictions

ClassicPvP enforces rules around allegiance oaths to prevent abuse and kill-trading between alts.

### Account-Wide Allegiance Lock

All characters on a single account must belong to the **same monarch's allegiance**. Once any character on your account has sworn to an allegiance, your other characters can only swear to someone within that same chain. Attempting to swear into a different allegiance will be blocked.

### Swear Cooldown

After swearing allegiance, a **7-day cooldown** applies before you can swear again.

- Your **first oath ever** is free — no cooldown is set.
- **Re-arranging your own chain is free (three times).** You may break and swear back into the **same allegiance** — for example, swearing under a different patron beneath the same monarch — up to **three times** without triggering the cooldown. Once those three are used up, the normal cooldown applies. Genuinely swearing into a **different** allegiance still costs the cooldown, and doing so refreshes your three free re-swears for the new allegiance.
- The cooldown applies to voluntary changes only. If your patron or someone above them in the chain **breaks their oath**, causing you to be broken from your allegiance involuntarily, you can re-swear back into the **original allegiance chain** without waiting.
- If your **monarch moves their entire allegiance** by swearing to a new patron, that is their oath change — your relationship to your own patron is unchanged and no cooldown is triggered for you.
- Swearing to a character on your **own account** with `/OfflineSwear` never counts against the cooldown (see [above](#-swearing-allegiance-to-same-account-characters)).

### Break Cascade & Account Protection

If someone above you in the chain breaks and it would leave your account with characters in two different allegiances, the server automatically breaks the affected character from their patron.

When this cascade propagates downward:
- Characters sworn to another character **on the same account** as their patron are **not broken** from that bond — the same-account relationship is preserved.
- The cascade continues through them, severing any **different-account** vassals further down the chain.

### Allegiance-Mate Alt Rewards

You cannot earn PvP rewards by killing a throwaway character parked on the account of one of your own allegiance mates. If the character you kill sits on an account that holds **another character in your allegiance**, that kill earns you nothing:

- It does **not** count toward the season **PK-kills leaderboard**, your kill/death ratio, or your kill streak.
- It does **not** advance **PK quest** or **bounty** progress.
- In the **Arena**, if any opponent you defeat is such an alt, that match pays **no rewards**.

The same principle governs hometown warfare: you cannot help attack a hometown **held by an allegiance that another character on your account belongs to**. Those characters do not count toward starting an assault, and their kills during a siege do not advance it.

---

## 🗡️ Same-Target Kill Diminishing Returns

Repeatedly killing the same player yields diminishing returns to prevent coordinated kill-trading.

| Rule | Value |
|---|---|
| Window | 1 hour |
| Kill threshold before suppression | 3 kills |
| Suppression duration | 3 hours |

Once you kill the same player more than **3 times within 1 hour**, rewards are suppressed for the next **3 hours**. During suppression:
- No PvP XP is granted for that kill
- The kill does **not** count toward season leaderboard ranking
- The kill does **not** advance PK quest progress

The killer receives a message when a kill is suppressed. The window and suppression timers are configurable by admins.

---

## 🏟️ Arena System

The Arena is a **queue-based structured PvP system** that operates independently from open-world PK. You join a queue, get matched, get teleported in, fight, and receive rewards.

### Entering the Arena
Use the `/arena` command to interact with the queue.

**Requirements to join:**
- Must be **Player Killer (PK)** status
- Must **not** be PK-tagged (no active PK timer from a recent kill)

### Arena Types

| Type | Format |
|------|--------|
| **1v1** | One vs. one duel |
| **2v2** | Two vs. two team duel |
| **FFA** | Free-for-All — up to 10 players, last one standing wins. At most 2 players from the same allegiance may be in the same match. |
| **Tugak** | Large Free-for-All — up to 15 players, last one standing wins. No allegiance limit per match. Prefers larger player counts before launching. Fought exclusively with the **Martyr's Hecatomb** (Health Bolt) line of spells — no weapons or other spells deal damage. Has its own separate quest achievement tracking. |
| **Group** | Team-based — organized fellowship vs. fellowship |

### Arena Combat Rules

Arenas run under specific combat restrictions that do not apply in the open world.

- **No damage before the match starts.** After you're teleported into the arena there is a short countdown before the match officially begins. During this window you can cast beneficial spells (buffs, vulns, and other preparation) but cannot deal any damage — melee, missile, magic, or damage-over-time. Damage only begins landing once the match starts.
- **Overtime healing restrictions.** If a match runs long enough to reach overtime, chugging food and potions is disabled and all other healing — heal-over-time spells, life-magic heals, and stamina-to-health transfers — is heavily reduced, weakening further as overtime continues. This forces stalled matches to a decisive finish.
- **Ineptitude spells are suppressed.** Creature enchantment debuffs (inepts) and all item enchantment spells are blocked in arena matches. Only the three defense-lowering spell categories are permitted — Magic Defense Lowering, Melee Defense Lowering, and Missile Defense Lowering. This prevents NPC pets, item procs, or other external debuff sources from influencing match outcomes.
- **Healing kit bonuses are capped in 1v1 matches.** The skill bonus from a healing kit is capped at 150 effective bonus skill, and the restoration multiplier is capped at 1.5×. High-end healing kits still function — they just can't fully carry a fight in the structured 1v1 format.
- **Tugak War is spell-only.** In Tugak War the only weapon is the **Martyr's Hecatomb** (Health Bolt) line of spells, tiers I through VII. Any other harmful spell you try to cast on an opponent simply fails ("you cannot affect anyone"), and weapon attacks and damage-over-time do nothing — everyone fights on equal footing with the same spell.

### Arena Rewards (Winners)

| Type | XP (of level) | PK Trophies | Phials of Bloody Tears | Darkbeat Keys |
|------|-----|-------------|----------------------|------------|
| **1v1** | 10% | 5 | 1 | 1 |
| **2v2** | 15% | 5 | 1 | 1 |
| **FFA** | 35% | 5 | 3 | 5 |
| **Tugak** | 35% | 5 | 3 | 5 |
| **Group** | 30% (60% on a clean 1st-place win) | 5 per member | 1 per member | 2 per member |

- Arena XP counts against your **PvP daily bucket**.
- Eliminated players should stay online until the match ends to be eligible for rewards.
- Arena XP is a **fixed percentage of your XP to the next level** and is not scaled by the seasonal rolling XP rate — the same result is worth the same fraction of a level all season long. Losers receive **3.5%** in 1v1/2v2, for FFA/Tugak non-podium finishes, and in a draw; FFA/Tugak **2nd** and **3rd** place receive 25% and 15%, and a **group loss** 10%.

### Daily PK Quest Rewards

In addition to the per-match rewards above, completing arena and PK milestones each day earns **Phials of Bloody Tears** and **PK Trophies** through the daily quest system. Quests reset each day and stack — hitting a higher threshold also awards all lower tiers. Selected highlights:

| Quest | Threshold | Phials | PK Trophies |
|-------|-----------|--------|-------------|
| Participate in arena matches | 5 / 15 / 30 / 50 | 1 / 2 / 3 / 3 | 5 / 15 / 25 / 50 |
| Win arena matches (any type) | 10 / 20 / 30 | 2 / 3 / 5 | 25 / 50 / 100 |
| Tugak War — participate | 2 / 25 matches | 1 / 5 | 15 / 75 |
| Tugak War — win | 1 / 20 wins | 1 / 5 | 25 / 75 |
| Tugak War — top 3 | 1 | 1 | 15 |
| Open world kills (opposing allegiance) | 10 / 30 | 1 / 3 | 20 / 60 |
| Complete bounty contracts | 1 / 5 / 25 | 1 / 3 / 5 | 15 / 35 / 100 |
| Complete high priority bounties | 1 / 5 | 2 / 5 | 25 / 50 |

Town Control kill quests (PKKILL_TC_1/5/30) have been disabled and no longer appear in rotation.

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

## ⚔️ PvP Combat Rules

### Logout Penalty
Logging out during PvP does not protect you. Any spell projectile (War spells, Void spells) that hits a player who is actively logging out will **critical hit 100% of the time** — matching existing melee behavior. Pulling the plug to escape a spell in flight doesn't work.

### Portal Space Behavior
Melee swings and missile attacks can **initiate against targets who are in portal space** (the purple bubble state). The attack animation and windup begin normally, but damage is not applied until the target exits portal space. This matches retail behavior — you could already be mid-swing when a target finishes porting in, and the attack resolves the moment they materialize.

### Dispel Protection After Taking Damage
For a window after being struck in PK combat, your own dispel spells will **not remove vulnerability spells** on your target. This prevents the tactic of attacking someone, getting hit once, then immediately dispelling their vulns to bleed off the damage setup. The protection window is 5 minutes by default and is configurable by admins.

### Jump Spam
Jumping rapidly in succession triggers accelerated stamina drain. After exceeding the jump threshold within a rolling 10-second window, every subsequent jump costs PK-rate stamina for a short penalty period. This eliminates the movement speed advantage gained through rapid jump-chaining.

### Wand Monkeying Disabled
A caster's **built-in spell** (the spell baked into a wand, orb, or other casting implement via its item spell) deals **no damage to other players**. This disables "wand monkeying" in PvP. Regular war magic cast from your own spellbook is unaffected, and built-in caster spells still function normally against creatures — the zero-damage rule applies only when the target is another player.

### PK Trophy Drops
Killing another player in open-world PvP has a chance to drop a **PK Trophy** on their corpse, subject to a few limits:

- **Level range** — no trophy drops if the victim is above the level 126 cap, or if the victim is more than **15 levels below** the killer. This keeps low-level twinks from being farmed by much higher-level killers.
- **Same allegiance** — no trophy drops if the killer and victim share the same monarch.
- **Rate limit (victim-side)** — a given victim can have at most **3 trophies** dropped on their corpse(s) within a rolling **1-hour** window, and at most **10 per day**. Once either limit is hit, further kills on that victim stop producing trophies until the window/day resets.

---

## 🛡️ Enhanced Anti-Cheat

ClassicPvP runs a number of anti-cheat and anti-abuse systems beyond standard emulator defaults.

- **IP Binding** — accounts accumulate IP addresses over time. A login from an IP already registered to a *different* account is rejected automatically (see **Account Restrictions** above).
- **Movement Validation** — the server independently validates player movement against server-side speed limits, so client-side speed and quickness hacks are detected and corrected rather than trusted. The system is terrain-aware — it accounts for legitimate movement over hills and uneven ground while still catching artificially fast movement — and repeat offenders are logged and removed.
- **Comprehensive Server Logging** — the server runs a dedicated logging database that records:
  - All tinkering attempts (success and failure)
  - All PK kill events
  - All Arena match participation and results
  - All rare item drops
  - Account and character login/logout sessions
  - Stuck character force-logoff events
- This gives admins a full audit trail to investigate suspicious activity, item duplication concerns, or systemic exploits.
- Rate limiting is applied to exploit-sensitive player commands to prevent abuse through rapid automated input.
- **War Detect Countermeasure** — TurnTo motions between two PK players use an absolute compass heading rather than a target GUID in the network packet. Plugins that parse network data to identify your spell target (commonly called "War Detect") are unable to extract any player identity from these packets.

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

## 🏆 Season Leaderboards

ClassicPvP tracks a **Season leaderboard** across 12 categories spanning both arena and open-world PvP. Every week the top players in each category are recognized and rewarded.

### Leaderboard Categories

#### Arena
| Category | What It Ranks |
|---|---|
| **1v1 Arena** | Composite score (ELO + wins + matches) |
| **2v2 Arena** | Composite score (ELO + wins + matches + survival bonus) |
| **FFA Arena** | Lifetime placement points across all FFA events |
| **Tugak Arena** | Lifetime placement points across all Tugak events |
| **Group Arena** | Total Group arena wins |
| **Arena Wins** | Total wins across all arena types combined |
| **Arena Kills** | Total kills recorded inside arena matches |
| **Arena Matches** | Total arena matches played (any type) |

#### Open World
| Category | What It Ranks |
|---|---|
| **PK Kills** | Total open-world player kills |
| **K/D Ratio** | Kill/death ratio (minimum 10 kills to qualify) |
| **Kill Streak** | Best consecutive open-world kill streak without dying |
| **Bounty Hunter** | Total bounty contracts completed |

#### Overall
| Category | What It Ranks |
|---|---|
| **Season Champion** | Weighted rank-points across 11 categories (all except Arena Kills) |

The Season Champion score gives more weight to categories that require skill and consistency. **Arena Kills** is tracked on the leaderboard but does not contribute to the Season Champion score. The 11 weighted categories are:

| Category | Weight |
|---|---|
| PK Kills | 2.5 |
| Arena Wins | 2.0 |
| Kill Streak | 1.75 |
| Bounty Hunter | 1.25 |
| 1v1 Arena, 2v2 Arena, Group Arena | 1.0 each |
| K/D Ratio | 0.75 |
| FFA Arena, Tugak Arena, Arena Matches | 0.5 each |

For each category you are ranked in, you earn `max(0, 11 − rank)` rank-points, multiplied by the category's weight. Your Season Champion score is the total across all 11 categories.

### Weekly Milestones

Every **Sunday**, the server automatically snapshots the top 10 players in each category. This is the weekly **milestone**.

- A server-wide broadcast announces the #1 finisher in each category.
- A full **top 10 in every category**, along with the reward legend, is posted to the ClassicPvP Discord Season channel.
- The **top 10** players in each category earn rewards for that week.

**Milestone rewards by rank:**

| Rank | XP | A-Boxes | Darkbeat Keys | Phials of Bloody Tears | PK Trophies |
|---|---|---|---|---|---|
| 🥇 1st | +200% to next level | 10 | 10 | 20 | 250 |
| 🥈 2nd | +100% to next level | 5 | 5 | 10 | 100 |
| 🥉 3rd | +75% to next level | 3 | 3 | 5 | 50 |
| 4th–10th | +50% to next level | 1 | 1 | 3 | 25 |

Rewards are **not delivered automatically** — you must claim them with `/season rewards`. Unclaimed rewards accumulate and can be collected at any time.

### Commands

| Command | Description |
|---|---|
| `/season status` | Season day, current level cap, and your XP budget usage |
| `/season top` | Current #1 leader in every category |
| `/season top <category>` | Full top 10 for a specific category |
| `/season stats` | Your rank in every leaderboard category |
| `/season stats <name>` | Another player's standings |
| `/season rewards` | Collect any unclaimed weekly milestone reward items |
| `/season info` | Category list and descriptions |
| `/season help` | Full help text and category aliases |

**Category shorthand aliases** — you can type `/season top <alias>` or just `/season <alias>`:

| Alias(es) | Category |
|---|---|
| `1v1` | 1v1 Arena |
| `2v2` | 2v2 Arena |
| `ffa` | FFA Arena |
| `tugak` | Tugak Arena |
| `group` | Group Arena |
| `wins` | Arena Wins |
| `matches`, `veteran` | Arena Matches |
| `reaper`, `kills` | PK Kills |
| `kd`, `ratio`, `precision` | K/D Ratio |
| `streak`, `unstoppable` | Kill Streak |
| `bounty`, `bountyhunter` | Bounty Hunter |
| `champion` | Season Champion |

---

## 🔥 Hot Dungeons

Periodically, up to **3 dungeons** across Dereth will become **Hot** — offering bonus experience and extra loot for players who venture inside.

### How It Works

- Every **12–36 hours**, a new dungeon is selected from a curated list and becomes Hot.
- Each Hot Dungeon stays active for **24–48 hours**, then expires independently.
- A **global broadcast** announces each dungeon when it becomes Hot, and again every hour while it remains active. A final announcement goes out when the dungeon cools down.
- Use the command **`/hotdungeons`** at any time to see all currently active Hot Dungeons, their XP multipliers, and time remaining.

### Dungeon Eligibility

Each dungeon in the pool has a **level bracket** (minimum and maximum server level cap). A dungeon only becomes eligible when the rolling level cap falls within that bracket, ensuring the featured content is always appropriate for the current progression stage of the season.

### Rewards While a Dungeon is Hot

| Reward | Details |
|--------|---------|
| **XP Multiplier** | All monster and PK kills inside the dungeon have their XP multiplied (multiplier varies per dungeon, ranging from 1.5× to 4×). The multiplier is applied before fellowship sharing. |
| **Double Loot** | Monster corpses receive two independent loot rolls, effectively doubling item generation. |
| **A Box** | Each monster kill has a per-dungeon configurable chance to drop **A Box** on the corpse. |
| **PK Rewards** | When a PK kill occurs inside a Hot Dungeon between players of **different allegiances**, the victim's corpse will contain a **Phial of Bloody Tears** and **A Box**. |

---

## 🏘️ Allegiance Hometown Capture

Allegiances can conquer and hold **towns across Dereth** through a two-phase PvP assault system.

### Owning Towns

Any allegiance member can walk up to a **Bind Stone** in an unowned town and use it to **claim the town for free**. Once claimed, the town becomes your allegiance's hometown and all members can recall there.

- `/ah` — Recalls to a random town owned by your allegiance
- `/ahtown <name>` — Recalls to a specific owned town (e.g. `/ahtown Arwic`)
- `/towns` — Lists all 25 capturable towns and their current ownership status

### Capturing an Enemy Town

To take a town owned by a rival allegiance, use the Bind Stone to begin the assault.

**Phase 1 — Perimeter Control (up to 60 minutes)**
- Phase 1 begins **automatically** when at least **2 members** of a single attacking allegiance are within **5 meters** of the Bind Stone and no other enemy allegiances are within **50 meters** — no player action required
- If an enemy PK enters within 50 meters, a warning is broadcast. If they remain for **30 continuous seconds**, Phase 1 progress resets. Leaving the area before 30 seconds have passed cancels the threat with no penalty.
- Hold the zone for **4 uninterrupted minutes** to trigger Phase 2
- Failing to reach Phase 2 within 60 minutes announces a global failure and applies a **3-hour cooldown** on that town for your allegiance

**Phase 2 — Destroy the Bind Stone (30 minutes)**
- The Bind Stone becomes attackable — chip down its HP with melee, missile, or war magic
- **All damage types are equal** — no element (slashing, fire, cold, etc.) is more or less effective than another, for both physical and magic
- **Melee and missile damage is reduced** so that a weapon user's DPS stays in line with a mage's, rather than vastly outpacing it. War magic is unaffected
- **Damage falls off with distance** — attacks deal full damage within **15 meters** of the Bind Stone, taper off beyond that, and deal **nothing past 20 meters**. You must fight up close to bring it down
- Bind Stone HP scales with the current rolling level cap
- Each kill on the defending allegiance in the combat zone deals **5% max HP** bonus damage to the Bind Stone
- Each kill on the attacking allegiance in the combat zone **heals the Bind Stone** by 5% max HP
- Destroy the Bind Stone within 30 minutes → **Attackers win**
- Survive 30 minutes with the Bind Stone intact → **Defenders win**; the Bind Stone heals and becomes unattackable again

Two allegiances cannot attack the same town simultaneously. An allegiance can maintain at most **2 active assaults** at once.

### Cooldowns & Protection

| Event | Cooldown |
|---|---|
| Phase 1 timeout (failed to reach Phase 2) | 3 hours — attacking allegiance only |
| Phase 2 failure (Bind Stone survived) | 6 hours — attacking allegiance only |
| Successful capture | 24 hours — new owner protected from attack (configurable) |

### Rewards

Winners within **100 meters of the Bind Stone** (on the town landblock or an adjacent one) at the moment of resolution share the rewards. **Defenders are rewarded more generously than attackers** — attackers already gain the town itself on a successful capture, so holding a town pays out the larger loot:

| Reward | Attackers (capture) | Defenders (hold) |
|---|---|---|
| PK Trophies (split among players) | 40 | 80 |
| MMDs (split among players) | 20 | 40 |
| XP to next level (per player) | 5% | 15% |
| Phials of Bloody Tears (per player) | — | 1 |
| Darkbeat Keys (per player) | — | 2 |

Losing allegiance PKs within **100 meters of the Bind Stone** at the moment of resolution are **smited**.

### Using the Bind Stone

Clicking (using) the Bind Stone at any time gives you a status message:
- **Unowned town** — instantly claims it for your allegiance
- **Your town** — confirms ownership and prompts you to defend
- **Enemy town, Phase 1 active** — informs you that an assault is already in progress
- **Enemy town, Phase 2 active** — informs you that the Bind Stone creature is under attack
- **Enemy town, no active assault** — shows any cooldown or blacklist block reason, or tells you the gather requirements to trigger Phase 1

During **Phase 2**, the real Bind Stone becomes invisible (cloaked) and an attackable **Bind Stone creature** appears in its place. Destroying the creature ends Phase 2 and awards the town to the attackers. If the creature survives the 30-minute timer the town remains with the defenders.

### Allegiance Blacklist

Server admins can suspend an allegiance from participating in hometown warfare via the blacklist. Blacklisted allegiances cannot initiate Phase 1 and are informed when they attempt to do so.

### Open-World PK Kill XP

When you kill an enemy PK in the open world (different allegiance, no diminishing returns), you earn PvP XP calculated as follows:

**Base XP:**
```
Base XP = 5–10% of your XP-to-next-level (random roll per kill)
```
The random roll is re-rolled on every kill, so repeated kills against the same target vary slightly each time.

**Level gap penalty:**
If the victim is below your level, the base XP is multiplied by a decay factor for each level of difference:
```
Modifier = 0.85 ^ (your level − victim's level)
```
Killing someone at or above your level applies no penalty. Killing someone 5 levels below you reduces the reward to ~44% of base; 10 levels below ~20%.

**Bonuses (applied after the decay modifier, all stack):**

| Condition | Effect |
|---|---|
| Hot Dungeon kill | × dungeon XP multiplier (1.5× – 4×, varies per dungeon) |
| +5% per hometown your allegiance owns | Passive, stacks, no cap |
| Active hometown conflict on the kill landblock (either phase) | × 2 |

**Diminishing returns:**
Killing the same player more than **3 times within a 1-hour window** suppresses all rewards from that target for **3 hours**. No XP, no quest credit, no season credit. You'll receive a message when a kill is suppressed.

---

## 🛒 Vendors

### Darkbeat

**Darkbeat** is a special vendor located in the Afterlife area. He accepts **Phials of Bloody Tears** as currency (not pyreals) and sells rare crafting and upgrade items. Phials are earned through PK quests, arena rewards, and hometown captures.

| Item | Cost (Phials) | Description |
|------|--------------|-------------|
| Imbue Altering Morph Gem | 20 | Randomizes a weapon's imbue between Crippling Blow, Armor Rending, and Critical Strike. |
| Empyrean Tuning Fork | 25 | Randomizes the legendary cantrips on armor, jewelry, or shields that already have legendaries. One use per item. |
| Slayer Upgrade Gem | 25 | Upgrades an existing slayer damage bonus to 1.8 on weapons that rolled a slayer via the tinkering lottery. |
| Ancient Bottle | 50 | Absorbs PvP XP overflow up to 100M. Bonded & Attuned. |
| Ancient Empyrean Tool | 75 | Guarantees the next tinker will not fail. |
| Empyrean Jeweler's Sawblade | 50 | Randomizes the slot of a ring, bracelet, or necklace between finger, wrist, and neck. |
| Oil of Creature Slaying | 75 | Adds a random slayer (1.8 damage bonus) to a weapon or magic caster that does not already have one. |
| Skill and Attribute Reset Gem | 50 | Clears quest stamps for the Temple of Enlightenment and Temple of Forgetfulness. Each use costs an escalating number of PK Trophies (see below). Bonded & Attuned. |

---

### Anti Parazi

**Anti Parazi** is a vendor located in the Abandoned Mine alongside Darkbeat. He accepts **PK Trophies** as currency (not pyreals) and sells bounty consumables and item requirement morph gems. PK Trophies are earned at a higher rate than Phials, reflected in Anti Parazi's pricing.

| Item | Cost (PK Trophies) | Description |
|------|-------------------|-------------|
| Bounty Purchase Token | 100 | Used to purchase a Bounty Contract from the Bounty Hunter NPC. |
| Writ of Pursuit | 200 | Inscribe with `PlayerName:Amount` and turn in to flag a player as a High Priority Target. |
| Workmanship Morph Gem | 500 | Randomizes the Workmanship of a loot item (1–10). |
| Arcane Lore Morph Gem | 350 | 75% chance to reduce Arcane Lore requirement by 5–25; 15% chance of no effect; 10% chance to increase it by 5–15. |
| Missile Defense Requirement Morph Gem | 400 | Removes the Missile Defense activation requirement from an item. |
| Melee Defense Requirement Morph Gem | 400 | Removes the Melee Defense activation requirement from an item. |
| Player Wield Requirement Morph Gem | 500 | Removes the wield restriction binding an item to a specific player. |
| Slayer Morph Gem | 100 | Randomizes the creature-slayer type on a loot-gen weapon or caster that already has a slayer, or on loot-gen armor with a Creature Slayer Rating. |
| Creature Resistance Morph Gem | 100 | Randomizes the creature-resistance type on loot-gen armor/jewelry that has a Creature Resist Rating. |
| A Dick (Vitae Removal) | 1 | Eat it to remove your Vitae penalty (no XP granted). Does nothing if you have no penalty. |

> **Level Requirement Removal Morph Gem** has been discontinued — no item in the Infiltration era has a level requirement, so it never had a use.

> **Impenetrability Morph Gem** — not sold by either vendor. Obtainable only from **Mythic Mystery Boxes**.

**Vitae Removal.** Anti Parazi also stocks **A Dick**, a consumable that costs **1 PK Trophy**. Eat it to clear your **Vitae penalty** — no XP is granted, it just removes the penalty. If you have no Vitae penalty, eating it does nothing and the item is not consumed.

---

### Custom Character Titles — `/buytitle`

Spend PK Trophies to give your character a **custom title**. Use `/BuyTitle <New Title>` in game — the title is applied to your character immediately and costs **200 PK Trophies** per purchase. New titles are screened against the server's taboo word filter, so disallowed words are rejected.

---

### Darkbeat's Storage Locker

The Storage Locker is a locked chest that always contains one tier 6 loot item and up to three randomly selected bonus items per opening. Each opening also has an independent **~20% chance to contain a Sturdy Iron Key**.

Each opening makes three independent rolls from the bonus table. Each roll has a 10% cumulative chance to land on a salvage bag, distributed evenly across 11 salvage types (~0.91% each):

| Salvage | Use |
|---------|-----|
| Sunstone | Armor Rend |
| Red Garnet | Fire Rend |
| Black Garnet | Pierce Rend |
| Imperial Topaz | Slash Rend |
| Jet | Lightning Rend |
| Aquamarine | Cold Rend |
| White Sapphire | Bludgeon Rend |
| Emerald | Acid Rend |
| Fire Opal | Crippling Blow |
| Black Opal | Critical Strike |
| Bloodstone | Minor Endurance (jewelry only) |

All salvage bags are full WS10 (100-unit) bags. Other possible bonus items include foolproof tinkering gems, Trade Notes, PK Trophies, Phials of Bloody Tears, consumables, and Massive Mana Stones.

### Skill and Attribute Reset Gem — PK Trophy Cost

Using the gem requires both the Phial purchase price **and** an additional PK Trophy cost paid at the time of use. The trophy cost scales exponentially with each use:

| Use # | PK Trophies |
|-------|-------------|
| 1st | 100 |
| 2nd | ~135 |
| 3rd | ~182 |
| 4th | ~246 |
| 5th+ | Continues growing (~1.35× per use, capped at 10,000) |

The gem is consumed on use. If you do not have enough PK Trophies in your inventory, the gem is not consumed and you are told the current cost.

---

## 📦 Mystery Boxes

The Common, Rare, and Mythic Mystery Boxes each contain a weighted loot table of currencies, salvage, and morph gems.

**A Box tiers.** Opening **A Box** rolls: 10% A Dick, 64% Common Mystery Box, 25% Rare Mystery Box, 1% Mythic Mystery Box.

### Common Mystery Box

| Item | Chance |
|------|--------|
| Workmanship Morph Gem | ~2.9% |
| Missile Defense Requirement Morph Gem | ~2.9% |
| Melee Requirement Morph Gem | ~2.9% |
| Player Wield Requirement Morph Gem | ~2.9% |
| Sturdy Iron Key | ~8.8% |
| Arcane Lore Morph Gem | ~8.8% |
| Steel Salvage (WS10, 100 units) | ~8.8% |
| Granite Salvage (WS10, 100 units) | ~8.8% |
| Iron Salvage (WS10, 100 units) | ~8.8% |
| Opal Salvage (WS10, 100 units) | ~8.8% |
| Rare Mystery Box | ~8.8% |
| MMD ×1 | ~8.8% |
| PK Trophies ×10 | ~8.8% |
| Bounty Purchase Token | ~8.8% |

Darkbeat's Lost Storage Key, Green Garnet Salvage, and the Level Requirement Removal Morph Gem no longer drop from Common Mystery Boxes.

### Rare Mystery Box

| Item | Chance |
|------|--------|
| Workmanship Morph Gem | ~5.6% |
| Missile Defense Requirement Morph Gem | ~5.6% |
| Melee Requirement Morph Gem | ~5.6% |
| Player Wield Requirement Morph Gem | ~5.6% |
| Slayer Upgrade Morph Gem | ~5.6% |
| Slayer Morph Gem | ~1.9% |
| Creature Resistance Morph Gem | ~1.9% |
| Sunstone Salvage WS10 — Armor Rend | ~3.7% |
| Red Garnet Salvage WS10 — Fire Rend | ~3.7% |
| Black Garnet Salvage WS10 — Pierce Rend | ~3.7% |
| Imperial Topaz Salvage WS10 — Slash Rend | ~3.7% |
| Jet Salvage WS10 — Lightning Rend | ~3.7% |
| Aquamarine Salvage WS10 — Cold Rend | ~3.7% |
| White Sapphire Salvage WS10 — Bludgeon Rend | ~3.7% |
| Emerald Salvage WS10 — Acid Rend | ~3.7% |
| Fire Opal Salvage WS10 — Crippling Blow | ~3.7% |
| Black Opal Salvage WS10 — Critical Strike | ~3.7% |
| Bloodstone Salvage WS10 — Minor Endurance (jewelry only) | ~3.7% |
| Sturdy Iron Keys ×3 | ~5.6% |
| Darkbeat's Lost Storage Key | ~5.6% |
| Mythic Mystery Box | ~5.6% |
| MMDs ×5 | ~5.6% |
| PK Trophies ×30 | ~5.6% |

All salvage bags are full WS10 bags (100 units). Ancient Bottle no longer drops from Rare Mystery Boxes — it's Mythic-only now. The Slayer Upgrade Morph Gem moved in here from the Mythic Mystery Box, and the Level Requirement Removal Morph Gem no longer drops here at all. The Slayer Morph Gem and Creature Resistance Morph Gem appear here at their rarest (~1.9% each) — they're more common in the Mythic box.

### Mythic Mystery Box

| Item | Chance |
|------|--------|
| Ancient Bottle (XP Bottle) | ~3.8% |
| Impenetrability Morph Gem | ~11.5% |
| Oil of Creature Slaying | ~11.5% |
| Skill and Attribute Reset Gem | ~11.5% |
| Imbue Altering Morph Gem | ~11.5% |
| Slayer Morph Gem | ~11.5% |
| Creature Resistance Morph Gem | ~11.5% |
| MMDs ×20 | ~11.5% |
| PK Trophies ×250 | ~11.5% |
| Shimmering Skeleton Key | ~3.8% |

The Slayer Upgrade Morph Gem moved out to the Rare Mystery Box; Oil of Creature Slaying takes its slot here. The Slayer Morph Gem and Creature Resistance Morph Gem also drop here at ~11.5% each — far more likely than in the Rare box.

> **Shimmering Skeleton Key** — a single-use key that unlocks **any** locked door or chest, no matter the lock. It crumbles to dust after one use and is **slippery**, so it drops on death (into your corpse for a killer to loot). Obtainable only from the Mythic Mystery Box.

---

## 🐗 Tusker Tusk & Olthoi Pincer Turn-In Timers

The repeat timer on the Tusker Tusk and Olthoi Pincer turn-in quests is **20 hours**, so you can farm and turn in tusks and pincers frequently rather than waiting weeks between rewards.

This covers all 14 Tusker Tusk turn-ins and all 8 Olthoi Pincer turn-ins (Harvester, Gardener, Soldier, Legionary, Eviscerator, Worker, Warrior, and Mutilator pincers turned in to Behdo Yii).

---

## 🔧 Tinker Characters — `/FlagTinker`

You can dedicate a character to be a **pure crafting specialist** using the `/FlagTinker` command. A Tinker is a support/crafting alt with every tinkering and crafting skill maxed out — perfect for salvaging, imbuing, and tinkering gear for yourself and your allegiance without having to level a combat character first.

### How to Flag a Tinker

Log in a **brand-new level 1 character** and type `/FlagTinker`. That's it. The conversion is applied instantly.

**Requirements:**
- The character must be **level 1** (a character that has already earned levels cannot be converted).
- Your account must **not already have a Tinker** — you get **one Tinker per account**.

> ⚠️ **This is permanent and irreversible.** There is no un-flag command. Only run `/FlagTinker` on a character you intend to keep as a dedicated crafter.

### What You Get

When you flag a Tinker, the character is instantly transformed:

- ✅ **All eight crafting skills are specialized and maxed** — Item Tinkering, Weapon Tinkering, Armor Tinkering, Magic Item Tinkering, Alchemy, Lockpick, Fletching, and Cooking.
- ✅ **All attributes are maxed** (Strength, Endurance, Coordination, Quickness, Focus, Self) and your health, stamina, and mana are refreshed to full.
- ✅ **A Tinkering Trinket** is placed in your inventory.
- ❌ **All combat skills are removed** — every weapon skill, shield, and all offensive magic (War, Void, Life, Creature Enchantment, Item Enchantment) is untrained. A Tinker is not built to fight.

### Living as a Tinker

- 🛡️ **No vitae on death.** Tinker characters never suffer the vitae experience penalty when they die — a mistake at the crafting bench or a stray death costs you nothing.
- 🔒 **Skills are locked.** A Tinker cannot train or specialize any new skills. Your crafting kit is set the moment you flag, and that's your loadout for good.
- 👑 **No allegiance passup.** A Tinker does not pass XP up the allegiance chain to its patron.

The intent is simple: a Tinker is a maxed-out crafting workstation in character form. Flag one, park it in your allegiance, and let it handle all your tinkering, salvaging, and item work.
