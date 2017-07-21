using Microsoft.Bot.Builder.FormFlow;
using System;

namespace MeetingSchedulerChain.Dialogs
{
    [Serializable]
    public class VenueForm
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public IForm<VenueForm> BuildForm()
        {
            return new FormBuilder<VenueForm>()
                .Message("I have a couple quick questions.")
                .Build();
        }
    }
}