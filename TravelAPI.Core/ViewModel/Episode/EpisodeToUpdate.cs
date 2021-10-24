using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.Model
{
    public class EpisodeToUpdate 
    {
        public int EpisodeId { get; set; }
        public string EpisodeTitle { get; set; }
        public string EpisodeDescription { get; set; }
        public string EpisodeImagePath { get; set; }
        public IFormFile EpisodeImage { get; set; }
        public string YoutubeUrl { get; set; }
        public bool EpisodeVisible { get; set; } 
        public DateTime CreationDate { get; set; }
        public int? SeasonId { get; set; }
        public DateTime? EpisodePublishDate { get; set; }

    }
}
