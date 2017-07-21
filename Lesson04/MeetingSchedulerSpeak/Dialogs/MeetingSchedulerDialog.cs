using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MeetingSchedulerSpeak.Dialogs
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

            await context.SayAsync(text: message, speak: message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Appointment")]
        public async Task AppointmentIntent(IDialogContext context, LuisResult result)
        {
            context.Call(new AppointmentDialog(), ResumeAfterCallAsync);
        }

        Task ResumeAfterCallAsync(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
            return Task.CompletedTask;
        }

        [LuisIntent("Register")]
        public async Task RegisterIntent(IDialogContext context, LuisResult result)
        {
            var entityValues = new List<EntityRecommendation>();

            if (result.TryFindEntity("UserName", out EntityRecommendation userNameEntity))
            {
                userNameEntity.Type = "Name";
                entityValues.Add(userNameEntity);
            }

            if (result.TryFindEntity("builtin.email", out EntityRecommendation emailEntity))
            {
                emailEntity.Type = "Email";
                entityValues.Add(emailEntity);
            }

            var registerForm = new RegisterForm();
            var registerFormDlg = new FormDialog<RegisterForm>(
                registerForm, registerForm.BuildForm, FormOptions.PromptInStart, entityValues);

            context.Call(registerFormDlg, ResumeAfterCallAsync);
        }

        [LuisIntent("Venue")]
        public async Task VenueIntent(IDialogContext context, LuisResult result)
        {
            string message = $"Called Venue: '{result.Query}'";

            await context.SayAsync(text: message, speak: message);

            context.Wait(MessageReceived);
        }
    }
}