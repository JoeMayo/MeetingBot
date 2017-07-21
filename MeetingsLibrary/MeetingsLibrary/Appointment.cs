using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingsLibrary
{
    [Table("Appointment")]
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int UserID { get; set; }
        public int VenueID { get; set; }
        public DateTime Time { get; set; }
    }
}
