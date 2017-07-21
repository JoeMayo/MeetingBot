using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using MeetingSchedulerDialog.Models;

namespace MeetingSchedulerDialog.Dialogs
{
    [Serializable]
    public class RegisterDialog : IDialog<Registration>
    {
        public Registration registration = new Registration();

        public Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(
                context: context,
                resume: EmailReceivedAsync, 
                prompt: $"What is your email?");

            return Task.CompletedTask;
        }

        async Task EmailReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            registration.Email = await result;

            PromptDialog.Text(
                context: context,
                resume: NameReceivedAsync,
                prompt: $"What is your name?");
        }

        async Task NameReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            registration.Name = await result;

            context.Done(registration);
        }
    }
}