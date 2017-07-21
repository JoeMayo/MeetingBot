using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace MeetingSchedulerFormFlow.Models
{
    public class MeetingState
    {
        const string MeetingDataProperty = "meetingData";

        public async Task<MeetingData> GetAsync(IActivity activity)
        {
            using (StateClient stateClient = activity.GetStateClient())
            {
                IBotState chatbotState = stateClient.BotState;
                BotData chatbotData =
                    await chatbotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                return chatbotData.GetProperty<MeetingData>(MeetingDataProperty);
            }
        }

        public async Task UpdateAsync(IActivity activity, MeetingData inData)
        {
            using (StateClient stateClient = activity.GetStateClient())
            {
                IBotState chatbotState = stateClient.BotState;
                BotData chatbotData = await chatbotState.GetUserDataAsync(
                    activity.ChannelId, activity.From.Id);

                MeetingData meetingData =
                    chatbotData.GetProperty<MeetingData>(MeetingDataProperty);

                if (meetingData == null)
                    meetingData = new MeetingData();

                meetingData.UserChannelID = activity.From.Id;
                meetingData.UserDBID = inData.UserDBID;

                chatbotData.SetProperty(MeetingDataProperty, data: meetingData);
                await chatbotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, chatbotData);
            }
        }

        public async Task DeleteAsync(Activity activity)
        {
            using (StateClient stateClient = activity.GetStateClient())
            {
                IBotState chatbotState = stateClient.BotState;

                await chatbotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
            }
        }
    }
}