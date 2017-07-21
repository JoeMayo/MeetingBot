using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace MeetingScheduler
{
    /// <summary>
    /// Welcome and delegation to other dialogs
    /// </summary>
    /// <remarks>
    /// Example of why you want to use Bot Builder dialogs (Lesson 3) to avoid all this work.
    /// </remarks>
    public class RootDialog : IDialogResponse
    {
        public async Task<string> GetResponseAsync(ConnectorClient connector, Activity activity)
        {
            //Activity typingActivity = activity.BuildTypingActivity();
            //await connector.Conversations.ReplyToActivityAsync(typingActivity);
            //await Task.Delay(millisecondsDelay: 10000);

            IDialogResponse dialog;

            MeetingData mtgData = await new MeetingState().GetAsync(activity);

            switch (mtgData?.Dialog)
            {
                case nameof(RegistrationDialog):
                default:
                    dialog = new RegistrationDialog();
                    break;
            }

            return await dialog.GetResponseAsync(connector, activity);
        }
    }
}