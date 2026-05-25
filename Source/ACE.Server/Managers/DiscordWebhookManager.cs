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
        /// Sends a Town Control conflict start/end announcement to the configured TC webhook.
        /// PropertyManager key: <c>town_control_globals_webhook</c>
        /// </summary>
        public static void SendTownControl(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var url = PropertyManager.GetString("town_control_globals_webhook").Item;
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

                var json    = $"{{\"content\":\"{content}\"}}";
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
