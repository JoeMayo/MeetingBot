using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using MeetingsLibrary;
using System.Data.Entity;

namespace MeetingScheduler
{
    /// <summary>
    /// Asks questions to add a new User to the DB
    /// </summary>
    /// <remarks>
    /// Example of why you want to use Bot Builder dialogs (Lesson 3) to avoid all this work.
    /// </remarks>
    public class RegistrationDialog : IDialogResponse
    {
        MeetingState mtgState = new MeetingState();

        public async Task<string> GetResponseAsync(ConnectorClient connector, Activity activity)
        {
            MeetingData mtgData = await mtgState.GetAsync(activity);

            string response;

            switch (mtgData?.Method)
            {
                case nameof(HandleNameResponseAsync):
                    response = await HandleNameResponseAsync(activity, mtgData);
                    break;
                case nameof(HandleEmailResponseAsync):
                    response = await HandleEmailResponseAsync(activity, mtgData);
                    break;
                default:
                    response = StartRegistration(ref mtgData);
                    break;
            }

            await mtgState.UpdateAsync(activity, mtgData);

            return response;
        }

        string StartRegistration(ref MeetingData mtgData)
        {
            if (mtgData == null)
                mtgData = new MeetingData();

            mtgData.Dialog = nameof(RegistrationDialog);
            mtgData.Method = nameof(HandleEmailResponseAsync);

            return "What is your email?";
        }

        async Task<string> HandleEmailResponseAsync(Activity activity, MeetingData mtgData)
        {
            string userEmail = activity.Text;

            using (var ctx = new MeetingContext())
            {
                User user =
                    await
                    (from usr in ctx.Users
                     where usr.UserID == mtgData.UserDBID ||
                           usr.Email == userEmail
                     select usr)
                    .SingleOrDefaultAsync();

                if (user == null)
                {
                    user = new User
                    {
                        Email = userEmail,
                        Name = userEmail
                    };
                    ctx.Users.Add(user);
                }
                else
                {
                    user.Email = userEmail;
                }

                await ctx.SaveChangesAsync();

                mtgData.UserChannelID = activity.From.Id;
                mtgData.UserDBID = user.UserID;
            }

            mtgData.Method = nameof(HandleNameResponseAsync);

            return "What is your name?";
        }

        async Task<string> HandleNameResponseAsync(Activity activity, MeetingData mtgData)
        {
            using (var ctx = new MeetingContext())
            {
                User user =
                    await
                    (from usr in ctx.Users
                     where usr.UserID == mtgData.UserDBID
                     select usr)
                    .SingleOrDefaultAsync();

                if (user == null)
                {
                    user = new User
                    {
                        Email = activity.Text,
                        Name = activity.Text
                    };
                    ctx.Users.Add(user);
                }
                else
                {
                    user.Name = activity.Text;
                }

                await ctx.SaveChangesAsync();

                mtgData.UserChannelID = activity.From.Id;
                mtgData.UserDBID = user.UserID;
            }

            mtgData.Dialog = string.Empty;
            mtgData.Method = string.Empty;

            return "Registration succeeded!";
        }
    }
}