using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;
using MeetingsLibrary;

namespace MeetingSchedulerChain.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            IEnumerable<string> actions = new List<string> { "Register", "Appointment", "Venue", "Schedule" };

            string prompt = $"Hi {activity.From.Name}! What would you like to do?";
            string retry = "Please select an option in the list.";
            int attempts = 10;

            var chain =
                from choice in new PromptDialog.PromptChoice<string>(actions, prompt, retry, attempts)
                select choice;

            await context.Forward(
                chain,
                ResumeAfterChainLinqAsync,
                context.Activity.AsMessageActivity());
        }

        async Task ResumeAfterChainLinqAsync(IDialogContext context, IAwaitable<string> result)
        {
            IDialog<string> chain =
                Chain
                    .PostToChain()
                    .Select(msg => msg.Text)
                    .Switch(
                        new RegexCase<IDialog<string>>(new Regex("^Register", RegexOptions.IgnoreCase),
                            (reContext, choice) =>
                            {
                                return Chain.From(() => DoRegisterCase());
                            }),
                        new Case<string, IDialog<string>>(choice => choice.Contains("Venue"),
                            (manageContext, txt) =>
                            {
                                return Chain.From(() => DoVenueCase());
                            }),
                        new DefaultCase<string, IDialog<string>>(
                            (defaultCtx, txt) =>
                            {
                                return Chain.Return("Not Implemented.");
                            })
                    )
                    .Unwrap();

            await context.Forward(
                chain,
                ResumeAfterChainAsync,
                context.Activity.AsMessageActivity());
        }

        async Task ResumeAfterChainAsync(IDialogContext context, IAwaitable<string> result)
        {
            string message = await result;
            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }

        IDialog<string> DoRegisterCase()
        {
            return
                Chain
                    .PostToChain()
                    .Select(msg => "What is your email?")
                    .PostToUser()
                    .WaitToBot()
                    .Then(async (ctx, res) => await res)
                    .Select(msg => $"Thanks - your email, {msg.Text}, is updated");
        }

        IDialog<string> DoVenueCase()
        {
            return
                Chain.From(() => FormDialog.FromForm(new VenueForm().BuildForm, FormOptions.PromptInStart))
                     .ContinueWith(async (dlgCtx, result) =>
                     {
                         VenueForm venue = await result;

                         using (var ctx = new MeetingContext())
                         {
                             ctx.Venues.Add(
                                 new Venue
                                 {
                                     Name = venue.Name,
                                     Address = venue.Address
                                 });

                             await ctx.SaveChangesAsync();
                         }

                         return Chain.Return("Venue added!");
                     });
        }
    }
}