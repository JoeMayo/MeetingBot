using MeetingSchedulerFormFlow.Models;
using MeetingsLibrary;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MeetingSchedulerFormFlow.Dialogs
{
    [Serializable]
    public class AppointmentForm
    {
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public bool SetReminder { get; set; }
        [Optional, Numeric(0, 480)]
        public int? ReminderMinutes { get; set; }

        List<VenueModel> venues;

        public IForm<AppointmentForm> BuildForm()
        {
            return new FormBuilder<AppointmentForm>()
                .Message("I have a couple quick questions.")
                .Field(nameof(SetReminder))
                .Field(nameof(ReminderMinutes), apptFrm => SetReminder)
                .Field(new FieldReflector<AppointmentForm>(nameof(Venue))
                    .SetType(null)
                    .SetFieldDescription("Please select venue")
                    .SetDefine(async (appointmentForm, field) =>
                    {
                        using (var ctx = new MeetingContext())
                        {
                            if (venues == null)
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

                        foreach (var venue in venues)
                            field
                                .AddDescription(venue.Name, venue.Name)
                                .AddTerms(venue.Name, Language.GenerateTerms(venue.Name, 6));

                        return true;
                    }))
                .Field(nameof(Date))
                .Field(
                    name: nameof(Time),
                    validate: async (appointmentForm, field) =>
                    {
                        var result = new ValidateResult { IsValid = true, Value = field };

                        DateTime time = (DateTime)field;
                        if (time.Hour < 8 || time.Hour > 17)
                        {
                            result.Feedback = "Meeting times should be between 8:00am and 5:00pm.";
                            result.IsValid = false;
                        }

                        return await Task.FromResult(result);
                    })
                .OnCompletion(SaveValuesAsync)
                .Build();
        }

        async Task SaveValuesAsync(IDialogContext context, AppointmentForm appointment)
        {
            MeetingData mtgData = await new MeetingState().GetAsync(context.Activity) ?? new MeetingData();

            using (var ctx = new MeetingContext())
            {
                ctx.Appointments.Add(
                    new Appointment
                    {
                        UserID = mtgData.UserDBID,
                        VenueID = 
                            (from venue in venues
                             where venue.Name == appointment.Venue
                             select venue.VenueID)
                            .Single(),
                        Time = new DateTime(
                            appointment.Date.Year, appointment.Date.Month, appointment.Date.Day,
                            appointment.Time.Hour, appointment.Time.Minute, 0)
                    });

                await ctx.SaveChangesAsync();
            }

            await context.PostAsync("Venue added!");
        }
    }
}