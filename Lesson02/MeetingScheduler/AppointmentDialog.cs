using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MeetingScheduler
{
    public class AppointmentDialog : IDialogResponse
    {
        public async Task<string> GetResponseAsync(ConnectorClient connector, Activity activity)
        {
            return "Not implemented - coming soon!";
        }
    }
}