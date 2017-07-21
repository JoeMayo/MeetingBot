using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;

namespace MeetingNegotiator
{
    class Program
    {
        public static string MicrosoftAppId { get; set; }
            = ConfigurationManager.AppSettings["MicrosoftAppId"];
        public static string MicrosoftAppPassword { get; set; }
            = ConfigurationManager.AppSettings["MicrosoftAppPassword"];

        static void Main()
        {
            ConversationParameters convParams = GetConversationParameters();

            var serviceUrl = new Uri(convParams.ServiceUrl);

            var connector = new ConnectorClient(serviceUrl, MicrosoftAppId, MicrosoftAppPassword);

            Console.Write(value: "Choose 1 for existing conversation or 2 for new conversation: ");
            ConsoleKeyInfo response = Console.ReadKey();

            if (response.KeyChar == '1')
                SendToExistingConversation(convParams, connector.Conversations);
            else
                StartNewConversation(convParams, connector.Conversations);
        }

        static void SendToExistingConversation(ConversationParameters convParams, IConversations conversations)
        {
            var existingConversationMessage = new Activity
            {
                Type = ActivityTypes.Message,
                Conversation = convParams.Conversation,
                From = convParams.Chatbot,
                Recipient = convParams.User,
                Text = $"Hi, We've scheduled a meeting at the time you requested."
            };

            conversations.SendToConversation(existingConversationMessage);
        }

        static void StartNewConversation(ConversationParameters convParams, IConversations conversations)
        {
            ConversationResourceResponse convResponse =
                conversations.CreateDirectConversation(convParams.Chatbot, convParams.User);
            var convAccount = new ConversationAccount(id: convResponse.Id);

            var notificationMessage = new Activity
            {
                Type = ActivityTypes.Message,
                Conversation = convAccount,
                From = convParams.Chatbot,
                Recipient = convParams.User,
                Text = $"Hi, Someone would like to schedule a meeting."
            };

            conversations.SendToConversation(notificationMessage);
        }

        static ConversationParameters GetConversationParameters()
        {
            string fileText = File.ReadAllText(path: @"..\..\InputData.json");
            Activity activityParams = JsonConvert.DeserializeObject<Activity>(fileText);

            return new ConversationParameters
            {
                Conversation = activityParams.Conversation,
                Chatbot = activityParams.From,
                User = activityParams.Recipient,
                ServiceUrl = activityParams.ServiceUrl,
            };
        }
    }
}
