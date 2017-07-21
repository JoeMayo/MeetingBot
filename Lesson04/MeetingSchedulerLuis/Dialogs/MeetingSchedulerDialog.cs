using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace MeetingSchedulerLuis.Dialogs
{
    [LuisModel(
        modelID: "",
        subscriptionKey: "")]
    [Serializable]
    public class MeetingSchedulerDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            string message = @"
Sorry, I didn't get that. Here examples of the things I can do:
'Please register me', 'Can I create an existing appointment?',
'Let me add a new Venue', or 'Schedule a new meeting' - just ask.";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Appointment")]
        public async Task AppointmentIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Called Appointments: " + result.Query);

            context.Wait(MessageReceived);
        }

        [LuisIntent("Register")]
        public async Task RegisterIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Called Register: " + result.Query);

            context.Wait(MessageReceived);
        }

        [LuisIntent("Venue")]
        public async Task VenueIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Called Venue: " + result.Query);

            context.Wait(MessageReceived);
        }

    }
}