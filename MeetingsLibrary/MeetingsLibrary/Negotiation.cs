using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingsLibrary
{
    [Table("Negotiation")]
    public class Negotiation
    {
        [Key, Column(Order = 1)]
        public int UserID { get; set; }
        [Key, Column(Order = 2)]
        public int MeetingID { get; set; }
        public int RequestedVenueID { get; set; }
        public DateTime RequestedTime { get; set; }
    }
}
