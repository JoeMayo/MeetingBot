using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingsLibrary
{
    [Table("Meeting")]
    public class Meeting
    {
        public int MeetingID { get; set; }
        public int VenueID { get; set; }
        public DateTime Time { get; set; }
        public string Subject { get; set; }
        public bool Confirmed { get; set; }
    }
}
