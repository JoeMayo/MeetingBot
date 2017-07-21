using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using System;

namespace MeetingScheduler
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
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            if (activity.Type == ActivityTypes.Message)
            {
                if (activity?.Type == ActivityTypes.Message)
                {
                    string chatbotResponse = await new RootDialog().GetResponseAsync(connector, activity);

                    //Activity reply = activity.CreateReply(chatbotResponse);
                    Activity reply = activity.BuildMessageActivity(chatbotResponse);

                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else
            {
                await new SystemMessages().HandleAsync(connector, activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}