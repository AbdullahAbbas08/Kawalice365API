#nullable disable

using BalarinaAPI.Core.Model;

namespace BalarinaAPI.Core.ViewModel 
{
    public class EpisodeModel : EpisodesRelatedForRecentlyModel
    {
        public double Hour { get; set; }
        public double Minute { get; set; }
        public string Date { get; set; }
        public string InterviewerName { get; set; } 
    }
}