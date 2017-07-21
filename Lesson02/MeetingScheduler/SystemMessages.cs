using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace MeetingScheduler
{
    public class SystemMessages
    {
        public async Task HandleAsync(ConnectorClient connector, Activity message)
        {
            switch (message.Type)
            {
                case ActivityTypes.ContactRelationUpdate:
                    HandleContactRelation(message);
                    break;
                case ActivityTypes.ConversationUpdate:
                    await HandleConversationUpdateAsync(connector, message);
                    break;
                case ActivityTypes.DeleteUserData:
                    await HandleDeleteUserDataAsync(message);
                    break;
                case ActivityTypes.Ping:
                    HandlePing(message);
                    break;
                case ActivityTypes.Typing:
                    HandleTyping(message);
                    break;
                default:
                    break;
            }
        }

        void HandleContactRelation(IContactRelationUpdateActivity activity)
        {
            if (activity.Action == "add")
            {
                // user added chatbot to contact list
            }
            else // activity.Action == "remove"
            {
                // user removed chatbot from contact list
            }
        }

        async Task HandleConversationUpdateAsync(
            ConnectorClient connector, IConversationUpdateActivity activity)
        {
            const string WelcomeMessage =
                "Welcome to Meeting Scheduler! " +
                "Type \"register\" to get started. " +
                "After that, you can type \"appointment\" to add a new appointment, " +
                "\"venue\" to add a new venue, or \"schedule\" to schedule a new meeting.";

            Func<ChannelAccount, bool> isChatbot =
                channelAcct => channelAcct.Id == activity.Recipient.Id;

            if (activity.MembersAdded?.Any(isChatbot) ?? false)
            {
                Activity reply = (activity as Activity).CreateReply(WelcomeMessage);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }

            if (activity.MembersRemoved?.Any(isChatbot) ?? false)
            {
                // to be determined
            }
        }

        async Task HandleDeleteUserDataAsync(Activity activity)
        {
            await new MeetingState().DeleteAsync(activity);
        }

        // random methods to test different ping responses
        bool IsAuthorized(IActivity activity) => DateTime.Now.Ticks % 3 != 0;
        bool IsForbidden(IActivity activity) => DateTime.Now.Ticks % 7 == 0;

        void HandlePing(IActivity activity)
        {
            if (!IsAuthorized(activity))
                throw new HttpException(
                    httpCode: (int)HttpStatusCode.Unauthorized,
                    message: "Unauthorized");
            if (IsForbidden(activity))
                throw new HttpException(
                    httpCode: (int)HttpStatusCode.Forbidden,
                    message: "Forbidden");
        }

        void HandleTyping(ITypingActivity activity)
        {
            // user has started typing, but hasn't submitted message yet
        }
    }
}