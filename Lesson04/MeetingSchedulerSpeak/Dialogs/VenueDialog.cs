using MeetingsLibrary;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MeetingSchedulerSpeak.Dialogs
{
    [Serializable]
    public class VenueDialog : IDialog<object>
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(
                context: context,
                resume: NameReceivedAsync,
                prompt: $"What is the venue name?");

            return Task.CompletedTask;
        }

        async Task NameReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            Name = await result;

            string message = "What is your name?";
            PromptDialog.Text(
                context: context,
                resume: AddressReceivedAsync,
                prompt: message);
        }

        async Task AddressReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            Address = await result;

            //using (var ctx = new MeetingContext())
            //{
            //    ctx.Venues.Add(
            //        new Venue
            //        {
            //            Name = Name,
            //            Address = Address
            //        });

            //    await ctx.SaveChangesAsync();
            //}

            string message = $"Saved {Name} at {Address}!";

            await context.SayAsync(text: message, speak: message);

            context.Done(this);
        }
    }
}