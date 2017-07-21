using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;
using MeetingSchedulerFormFlow.Models;

namespace MeetingSchedulerFormFlow.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public static string HelpMessage =
            "Type \"Register\" to get started. " +
            "After that, you can type \"Appointment\" to add a new appointment, " +
            "\"Venue\" to add a new venue, or \"Schedule\" to schedule a new meeting.";

        public static MeetingData MtgData { get; set; }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var actions = new List<string> { "Register", "Appointment", "Venue", "Schedule", "Help" };

            PromptDialog.Choice(
                context: context,
                resume: ActionReceivedAsync,
                options: actions,
                prompt: $"Hi {activity.From.Name}! What would you like to do?",
                retry: "Please select an option in the list.");
        }

        async Task ActionReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            string action = await result;

            switch (action.ToLower())
            {
                case "appointment":
                    IDialog<AppointmentForm> appointmentDlg = FormDialog.FromForm(new AppointmentForm().BuildForm);
                    await context.Forward(appointmentDlg, FormDialogResumedAsync, context.Activity as IMessageActivity);
                    break;
                case "venue":
                    IDialog<VenueForm> venueDlg = FormDialog.FromForm(new VenueForm().BuildForm);
                    await context.Forward(venueDlg, FormDialogResumedAsync, context.Activity as IMessageActivity);
                    break;
                case "schedule":
                    break;
                case "help":
                    await context.PostAsync(HelpMessage);
                    context.Wait(MessageReceivedAsync);
                    break;
                case "register":
                    IDialog<RegisterForm> registerDlg = FormDialog.FromForm(new RegisterForm().BuildForm);
                    await context.Forward(registerDlg, RegisterFormDialogResumedAsync, context.Activity as IMessageActivity);
                    break;
            }
        }

        async Task FormDialogResumedAsync(IDialogContext context, IAwaitable<object> result)
        {
            object response = await result;
            context.Wait(MessageReceivedAsync);
        }

        async Task RegisterFormDialogResumedAsync(IDialogContext context, IAwaitable<RegisterForm> result)
        {
            var mtgState = new MeetingState();
            MeetingData mtgData = await mtgState.GetAsync(context.Activity) ?? new MeetingData();

            RegisterForm form = await result;
            mtgData.UserDBID = form.UserID;
            mtgData.UserChannelID = context.Activity.From.Id;

            MtgData = mtgData;
            //await mtgState.UpdateAsync(context.Activity, mtgData);

            context.Wait(MessageReceivedAsync);
        }
    }
}