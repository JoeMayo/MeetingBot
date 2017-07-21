using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Connector;

namespace MeetingScheduler
{
    public class NegotiationDialog : IDialogResponse
    {
        public async Task<string> GetResponseAsync(ConnectorClient connector, Activity activity)
        {
            return "Not implemented - coming soon!";
        }
    }
}