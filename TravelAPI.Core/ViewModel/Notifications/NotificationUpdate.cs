using Microsoft.AspNetCore.Http;

namespace BalarinaAPI.Core.ViewModel
{
    public class NotificationUpdate
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string Descriptions { get; set; }
        public IFormFile IMG { get; set; }
        public int? EpisodeID { get; set; }
        public bool Visible { get; set; }
    }
} 
 