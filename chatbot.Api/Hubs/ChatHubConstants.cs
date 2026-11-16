namespace chatbot.Api.Hubs
{
    public static class ChatHubConstants
    {
        public const string HubRoute = "/hubs/chat";

        public const string DeviceSessionIdClaim = "device_session_id";

        private const string UserGroupPrefix = "user:";
        private const string ConversationGroupPrefix = "conversation:";

        public static string GetUserGroup(Guid userId)
        {
            return $"{UserGroupPrefix}{userId}";
        }

        public static string GetConversationGroup(Guid conversationId)
        {
            return $"{ConversationGroupPrefix}{conversationId}";
        }
    }
}
