using System;

namespace MeetingSchedulerLuis.Models
{
    [Serializable]
    public class MeetingData
    {
        public string UserChannelID { get; set; }
        public int UserDBID { get; set; }
    }
}