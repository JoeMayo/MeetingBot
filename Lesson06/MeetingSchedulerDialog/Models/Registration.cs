using System;

namespace MeetingSchedulerDialog.Models
{
    [Serializable]
    public class Registration
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}