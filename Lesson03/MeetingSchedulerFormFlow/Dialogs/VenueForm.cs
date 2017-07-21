using MeetingsLibrary;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Threading.Tasks;

namespace MeetingSchedulerFormFlow.Dialogs
{
    [Serializable]
    public class VenueForm
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public IForm<VenueForm> BuildForm()
        {
            return new FormBuilder<VenueForm>()
                .Message("I have a couple quick questions.")
                .OnCompletion(SaveValuesAsync)
                .Build();
        }

        async Task SaveValuesAsync(IDialogContext context, VenueForm venue)
        {
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

            await context.PostAsync("Venue added!");
        }
    }
}