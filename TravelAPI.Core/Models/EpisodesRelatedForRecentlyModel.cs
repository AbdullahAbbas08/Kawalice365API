using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class EpisodesRelatedForRecentlyModel
    {
        public int      SessionId           { get; set; }  
        public string      SeasonTitle           { get; set; }  
        public int      EpisodeId           { get; set; }
        public string   EpisodeTitle        { get; set; }
        public string EpisodeDescription { get; set; }
        public string   EpisodeImg        { get; set; }
        public int      EpisodeViews        { get; set; }
        public DateTime EpisodePublishDate  { get; set; }
        public int      ProgramId           { get; set; }
        public string   ProgramName         { get; set; }
        public string   ProgramImg          { get; set; }
        public int      CategoryId          { get; set; }
        public string   CategoryTitle       { get; set; }
        public int      ProgramTypeId       { get; set; }
        public string   ProgramTypeTitle    { get; set; }
        public string   EpisodeUrl          { get; set; }
        public Boolean EpisodeVisible       { get; set; }
    }
}