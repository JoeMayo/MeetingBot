using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using MeetingSchedulerSpeak.Dialogs;

namespace MeetingSchedulerSpeak
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new MeetingSchedulerDialog());
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        async Task HandleSystemMessageAsync(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                const string WelcomeMessage =
                    "### Welcome to Meeting Scheduler!\n" +
                    "* Ask \"please register me\" to get started.\n" +
                    "* \"Add an appointment\" to add a new appointment,\n" +
                    "* \"Add a venue\" to add a new venue, or\n" +
                    "* \"schedule a meeting\" to schedule a new meeting.";

                Func<ChannelAccount, bool> isChatbot =
                    channelAcct => channelAcct.Id == message.Recipient.Id;

                if (message.MembersAdded?.Any(isChatbot) ?? false)
                {
                    Activity reply = (message as Activity).CreateReply(WelcomeMessage);

                    reply.Speak = WelcomeMessage;
                    reply.InputHint = InputHints.ExpectingInput;

                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
        }
    }
}