using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using MeetingSchedulerDialog.Models;
using MeetingsLibrary;
using System.Data.Entity;

namespace MeetingSchedulerDialog.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public static string HelpMessage =
            "Type \"Register\" to get started. " +
            "After that, you can type \"Appointment\" to add a new appointment, " +
            "\"Venue\" to add a new venue, or \"Schedule\" to schedule a new meeting.";

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
                    break;
                case "venue":
                    break;
                case "schedule":
                    break;
                case "help":
                    await context.PostAsync(HelpMessage);
                    context.Wait(MessageReceivedAsync);
                    break;
                case "register":
                    context.Call(new RegisterDialog(), RegisterDialogResumedAsync);
                    break;
            }

        }

        async Task RegisterDialogResumedAsync(IDialogContext context, IAwaitable<Registration> result)
        {
            Registration registration = await result;

            IActivity activity = context.Activity;

            var mtgState = new MeetingState();
            MeetingData mtgData = await mtgState.GetAsync(activity) ?? new MeetingData();

            using (var ctx = new MeetingContext())
            {
                User user =
                    await
                    (from usr in ctx.Users
                     where usr.UserID == mtgData.UserDBID ||
                           usr.Email == registration.Email
                     select usr)
                    .SingleOrDefaultAsync();

                if (user == null)
                {
                    user = new User
                    {
                        Email = registration.Name,
                        Name = registration.Email
                    };
                    ctx.Users.Add(user);
                }
                else
                {
                    user.Name = registration.Name;
                    user.Email = registration.Email;
                }

                await ctx.SaveChangesAsync();
            }

            await context.PostAsync("Registration succeeded!");

            context.Wait(MessageReceivedAsync);
        }
    }
}