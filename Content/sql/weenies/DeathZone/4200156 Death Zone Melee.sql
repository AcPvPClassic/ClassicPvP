/* Death Zone (Hotspot)
   Converted from an immobile Creature to a HotSpot. A creature-based death zone
   was bypassed by jumping (an immobile monster cannot reliably land a melee swing
   on a player jumping past). This weenie sets UseRadius (> 0), which activates the
   custom proximity-scan logic in Hotspot.cs: it scans for players every 0.25s and
   instantly kills any player within UseRadius meters, measured horizontally (Z is
   ignored), so jumping does not bypass it.

   Identical to 4200155; kept as a separate class_Id so existing placements of
   either wcid continue to work.

   Tuning:
     - UseRadius (float 54)   : horizontal kill radius in meters. 3 = ~3m.
     - DefaultScale (float 39): visual size of the effect. Tune so the visible
                                footprint roughly matches UseRadius, or cluster
                                multiple instances along a hallway.
     - Setup (d_i_d 1)        : the visible model/effect. Currently the bonfire
                                fire (0x020005AE); swap for a different look.
*/

DELETE FROM `weenie` WHERE `class_Id` = 4200156;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200156, 'ace4200156-DeathZoneHotspot', 13, '2026-07-09 00:00:00') /* HotSpot */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200156,   1,        128) /* ItemType - Misc */
     , (4200156,   5,         10) /* EncumbranceVal */
     , (4200156,   8,         10) /* Mass */
     , (4200156,  16,          1) /* ItemUseable - No */
     , (4200156,  19,          5) /* Value */
     , (4200156,  44,       9999) /* Damage */
     , (4200156,  45,         16) /* DamageType - Fire */
     , (4200156,  93,       3084) /* PhysicsState - Ethereal, ReportCollisions, Gravity, LightingOn */
     , (4200156, 119,          0) /* Active */
;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200156,   1, True ) /* Stuck */
     , (4200156,  11, False) /* IgnoreCollisions */
     , (4200156,  12, True ) /* ReportCollisions */
     , (4200156,  13, True ) /* Ethereal - players pass through (not wall-blocked) but still enter the zone */
     , (4200156,  14, True ) /* GravityStatus */
     , (4200156,  15, True ) /* LightsStatus */
     , (4200156,  24, True ) /* UiHidden */
     , (4200156,  55, True ) /* IsHot - required for the hotspot to deal damage */
     , (4200156,  57, False) /* AffectsAis - only affects players */
;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200156,   1,        1) /* HeartbeatInterval - drives the proximity scan startup */
     , (4200156,  22,      0.5) /* DamageVariance */
     , (4200156,  39,        2) /* DefaultScale - visual size; tune to taste */
     , (4200156,  54,        3) /* UseRadius - jump-proof kill radius in meters (activates proximity kill) */
     , (4200156, 105,        1) /* HotspotCycleTime */
     , (4200156, 106,      0.2) /* HotspotCycleTimeVariance */
;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200156,   1, 'Death Zone') /* Name */
     , (4200156,  17, 'The death zone consumes you!') /* Activation talk */
;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200156,   1, 0x020005AE) /* Setup - bonfire fire (visible effect) */
     , (4200156,   3, 0x20000052) /* SoundTable */
     , (4200156,   8, 0x0600192F) /* Icon */
;
