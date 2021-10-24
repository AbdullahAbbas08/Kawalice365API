using System;
using System.Collections.Generic;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class Keyword
    {
        public Keyword()
        {
            EpisodesKeywords = new HashSet<EpisodesKeyword>();
        }
        public int KeywordId { get; set; }
        public string KeywordTitle { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual ICollection<EpisodesKeyword> EpisodesKeywords { get; set; }
    }
}
