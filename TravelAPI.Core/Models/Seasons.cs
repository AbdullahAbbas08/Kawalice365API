using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class Seasons
    {
        public Seasons()
        {
            Episodes = new HashSet<Episode>();
            CreationDate = DateTime.Now;
        }
        public int      SessionId       { get; set; }
        public string   SessionTitle    { get; set; }
        public int      ProgramId       { get; set; }
        public int      SeasonViews     { get; set; }
        public int      SeasonIndex     { get; set; }
        public DateTime CreationDate  { get; set; }

        [JsonIgnore]
        public virtual Program Program { get; set; }
        [JsonIgnore]
        public virtual ICollection<Episode> Episodes { get; set; }
    }
}
