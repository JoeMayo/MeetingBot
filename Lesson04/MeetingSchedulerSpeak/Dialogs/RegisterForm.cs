using MeetingSchedulerSpeak.Models;
using MeetingsLibrary;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingSchedulerSpeak.Dialogs
{
    [Serializable]
    public class RegisterForm
    {
        public string Email { get; set; }
        public string Name { get; set; }
        internal int UserID { get; set; }

        public IForm<RegisterForm> BuildForm()
        {
            return new FormBuilder<RegisterForm>()
                .Message("I have a couple quick questions.")
                .OnCompletion(SaveValuesAsync)
                .Build();
        }

        async Task SaveValuesAsync(IDialogContext context, RegisterForm registration)
        {
            MeetingData mtgData = await new MeetingState().GetAsync(context.Activity) ?? new MeetingData();

            //using (var ctx = new MeetingContext())
            //{
            //    User user =
            //        await
            //        (from usr in ctx.Users
            //         where usr.UserID == mtgData.UserDBID ||
            //               usr.Email == registration.Email
            //         select usr)
            //        .SingleOrDefaultAsync();

            //    if (user == null)
            //    {
            //        user = new User
            //        {
            //            Email = registration.Name,
            //            Name = registration.Email
            //        };
            //        ctx.Users.Add(user);
            //    }
            //    else
            //    {
            //        user.Name = registration.Name;
            //        user.Email = registration.Email;
            //    }

            //    await ctx.SaveChangesAsync();
            //}

            await context.PostAsync("Registration succeeded!");
        }
    }
}