using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Episode
{
    public class EpisodeModelInput 
    {
        public string    EpisodeTitle       { get; set; }
        public string    EpisodeDescription { get; set; }
        public string    YoutubeUrl         { get; set; }
        public bool      EpisodeVisible     { get; set; }
        public int?      LikeRate           { get; set; }
        public int?      DislikeRate        { get; set; }
        public int?      EpisodeViews       { get; set; }
        public int       SeasonId           { get; set; }
        public IFormFile EpisodeIamge       { get; set; }
        public DateTime  EpisodePublishDate { get; set; }
    }
}
