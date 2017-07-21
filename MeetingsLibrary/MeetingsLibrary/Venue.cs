using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingsLibrary
{
    [Table("Venue")]
    public class Venue
    {
        public int VenueID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
