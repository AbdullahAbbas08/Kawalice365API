using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class EpisodesKeyword
    {
        public int EpisodeKeywordId { get; set; }
        public int? KeywordId { get; set; }
        public int? EpisodeId { get; set; }

        public virtual Episode Episode { get; set; }
        public virtual Keyword Keyword { get; set; }
    }
}
