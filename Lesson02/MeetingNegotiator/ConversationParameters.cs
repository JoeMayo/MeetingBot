using Microsoft.Bot.Connector;

namespace MeetingNegotiator
{
    public class ConversationParameters
    {
        public ConversationAccount Conversation { get; set; }
        public ChannelAccount Chatbot { get; set; }
        public ChannelAccount User { get; set; }
        public string ServiceUrl { get; internal set; }
    }
}
