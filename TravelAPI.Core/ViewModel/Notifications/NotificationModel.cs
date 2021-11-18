using Microsoft.AspNetCore.Http;

namespace BalarinaAPI.Core.ViewModel
{
    public class NotificationModel 
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string Descriptions { get; set; }
        public string IMG { get; set; }
        public int? EpisodeID { get; set; }
        public string EpisodeName { get; set; }
        public string ProgramName { get; set; }
        public bool Visible { get; set; }
    }
}

 