using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace MeetingScheduler
{
    /// <summary>
    /// Interface for Lesson 2 dialogs
    /// </summary>
    /// <remarks>
    /// Example of why you want to use Bot Builder dialogs (Lesson 3) to avoid all this work.
    /// </remarks>
    public interface IDialogResponse
    {
        Task<string> GetResponseAsync(ConnectorClient connector, Activity activity);
    }
}