using MeetingSchedulerSpeak.Models;
using MeetingsLibrary;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingSchedulerSpeak.Dialogs
{
    [Serializable]
    public class AppointmentDialog : IDialog<object>
    {
        public string Venue { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        List<VenueModel> venues;

        public async Task StartAsync(IDialogContext context)
        {
            using (var ctx = new MeetingContext())
            {
                venues =
                    await
                    (from venue in ctx.Venues
                     select new VenueModel
                     {
                         VenueID = venue.VenueID,
                         Name = venue.Name
                     })
                    .Take(5)
                    .ToListAsync();
            }

            string message = "Which venue would you like?";
            string retry = "Please select a venue in the list.";

            var promptOptions = 
                new PromptOptions<string>(
                    prompt: message,
                    retry: retry,
                    options: venues.Select(v => v.Name).ToList(),
                    speak: message,
                    retrySpeak: retry);

            PromptDialog.Choice(context, VenueReceivedAsync, promptOptions);
        }

        async Task VenueReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            Venue = await result;

            string message = "What date is the appointment?";
            Activity reply = (context.Activity as Activity).CreateReply(message);
            reply.Speak = message;
            reply.InputHint = InputHints.ExpectingInput;

            await
                new ConnectorClient(new Uri(reply.ServiceUrl))
                    .Conversations
                    .SendToConversationAsync(reply);

            context.Wait(DateReceivedAsync);
        }

        async Task DateReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity response = await result;
            Date = response.Text;

            string message = $"Received appointment for {Venue} on {Date}.";

            await context.SayAsync(
                text: message, 
                speak: message, 
                options: new MessageOptions { InputHint = InputHints.AcceptingInput });

            context.Done(this);
        }
    }
}