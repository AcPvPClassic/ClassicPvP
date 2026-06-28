using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using log4net;

namespace ACE.Server.Managers
{
    /// <summary>
    /// Centralised Discord webhook dispatcher for ClassicPvP.
    /// Each channel type reads its own PropertyManager URL so webhooks can be
    /// routed to different Discord channels independently.
    ///
    /// All public methods are fire-and-forget (no await required at call sites)
    /// and silently no-op when the configured URL is empty.
    /// </summary>
    public static class DiscordWebhookManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Shared HttpClient — thread-safe and intentionally reused for the lifetime of the process.
        private static readonly HttpClient _http = new HttpClient();

        // -----------------------------------------------------------------------
        // Public channel methods
        // -----------------------------------------------------------------------

        /// <summary>
        /// Sends an in-game general chat message to the configured general-chat webhook.
        /// PropertyManager key: <c>turbine_chat_webhook</c>
        /// </summary>
        public static void SendGeneralChat(string senderName, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var url = PropertyManager.GetString("turbine_chat_webhook").Item;
            if (string.IsNullOrWhiteSpace(url)) return;

            var content = $"[General] {senderName}: \"{SanitiseMessage(message)}\"";
            _ = SendAsync(url, content);
        }

        /// <summary>
        /// Sends an admin/audit event to the configured audit webhook.
        /// PropertyManager key: <c>turbine_chat_webhook_audit</c>
        /// </summary>
        public static void SendAudit(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var url = PropertyManager.GetString("turbine_chat_webhook_audit").Item;
            if (string.IsNullOrWhiteSpace(url)) return;

            _ = SendAsync(url, SanitiseMessage(message));
        }

        /// <summary>
        /// Sends a PK/PKL kill notification to the configured PK-kill webhook.
        /// PropertyManager key: <c>pk_kill_webhook</c>
        /// </summary>
        public static void SendPkKill(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var url = PropertyManager.GetString("pk_kill_webhook").Item;
            if (string.IsNullOrWhiteSpace(url)) return;

            _ = SendAsync(url, SanitiseMessage(message));
        }

        /// <summary>
        /// Sends a movement anti-cheat violation alert to the configured movement violation webhook.
        /// PropertyManager key: <c>movement_violation_webhook</c>
        /// </summary>
        /// <param name="violationType">Short type tag, e.g. "script_timing", "speed_packet".</param>
        /// <param name="playerName">In-game character name.</param>
        /// <param name="accountName">Account login name.</param>
        /// <param name="observed">Measured value (units depend on violation type).</param>
        /// <param name="allowed">Allowed limit for comparison.</param>
        /// <param name="suspicionScore">Running suspicion score at time of violation.</param>
        /// <param name="location">String representation of the player's location.</param>
        public static void SendMovementViolation(string violationType, string playerName, string accountName,
            float observed, float allowed, float suspicionScore, string location)
        {
            // Anti-cheat webhook broadcasting disabled
        }

        /// <summary>
        /// Sends a Season milestone / weekly leader notification to the configured Season webhook.
        /// PropertyManager key: <c>season_milestone_webhook</c>
        /// </summary>
        public static void SendSeasonMilestone(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var url = PropertyManager.GetString("season_milestone_webhook").Item;
            if (string.IsNullOrWhiteSpace(url)) return;

            _ = SendAsync(url, SanitiseMessage(message));
        }

        /// <summary>
        /// Sends an Allegiance Hometown global broadcast to the configured Hometown webhook.
        /// PropertyManager key: <c>hometown_webhook</c>
        /// </summary>
        public static void SendHometownBroadcast(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var url = PropertyManager.GetString("hometown_webhook").Item;
            if (string.IsNullOrWhiteSpace(url)) return;

            _ = SendAsync(url, SanitiseMessage(message));
        }

        /// <summary>
        /// Sends a Hot Dungeon announcement to the configured Hot Dungeon webhook.
        /// PropertyManager key: <c>hot_dungeon_webhook</c>
        /// </summary>
        public static void SendHotDungeon(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var url = PropertyManager.GetString("hot_dungeon_webhook").Item;
            if (string.IsNullOrWhiteSpace(url)) return;

            _ = SendAsync(url, SanitiseMessage(message));
        }

        // -----------------------------------------------------------------------
        // Internal helpers
        // -----------------------------------------------------------------------

        /// <summary>
        /// Prevents @everyone / @here pings and escapes characters that would
        /// break the Discord JSON payload.
        /// </summary>
        private static string SanitiseMessage(string message)
        {
            return message
                .Replace("@", "@ ")          // neutralise Discord @mentions
                .Replace("\\", "\\\\")
                .Replace("\"",  "\\\"")
                .Replace("\r\n", "\\n")
                .Replace("\n",  "\\n")
                .Replace("\r",  "\\n");
        }

        /// <summary>
        /// Core fire-and-forget HTTP POST to a Discord webhook endpoint.
        /// Builds the minimal <c>{"content":"..."}</c> payload Discord requires.
        /// </summary>
        private static async Task SendAsync(string webhookUrl, string content)
        {
            try
            {
                // Truncate to Discord's 2 000-character message limit.
                if (content.Length > 2000)
                    content = content.Substring(0, 1997) + "...";

                var json    = Newtonsoft.Json.JsonConvert.SerializeObject(new { content });
                var payload = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _http.PostAsync(webhookUrl, payload).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    log.Warn($"[DiscordWebhookManager] Webhook POST returned {(int)response.StatusCode} for URL ending ...{webhookUrl.Substring(Math.Max(0, webhookUrl.Length - 30))}");
            }
            catch (Exception ex)
            {
                log.Error($"[DiscordWebhookManager] Exception posting webhook: {ex.Message}");
            }
        }
    }
}
