using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class Episode
    {
        public Episode()
        {
            EpisodesKeywords = new HashSet<EpisodesKeyword>();
        }
        public int EpisodeId { get; set; }
        public string EpisodeTitle { get; set; }
        public string EpisodeDescription { get; set; }
        public string YoutubeUrl { get; set; }
        public string EpisodeIamgePath { get; set; }
        public bool EpisodeVisible { get; set; }
        public DateTime CreationDate { get; set; }
        public int? LikeRate { get; set; }
        public int? DislikeRate { get; set; }
        public int EpisodeViews { get; set; }
        public DateTime EpisodePublishDate { get; set; }
        public int? SessionId { get; set; }

        [JsonIgnore]
        public virtual Seasons Session { get; set; }

        [JsonIgnore]
        public virtual ICollection<EpisodesKeyword> EpisodesKeywords { get; set; }
    }
}
