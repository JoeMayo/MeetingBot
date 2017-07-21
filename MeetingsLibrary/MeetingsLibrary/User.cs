using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingsLibrary
{
    [Table("User")]
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
