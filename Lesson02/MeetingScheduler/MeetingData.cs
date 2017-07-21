using System;

namespace MeetingScheduler
{
    [Serializable]
    public class MeetingData
    {
        public string UserChannelID { get; set; }
        public int UserDBID { get; set; }
        public string Dialog { get; set; }
        public string Method { get; set; }
    }
}