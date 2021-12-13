#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class EpisodesRelatedForSeasons 
    {
        public int SessionId { get; set; }
        public string SeasonTitle { get; set; }
        public int SeasonIndex { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramImg { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public int ProgramTypeId { get; set; }
        public string ProgramTypeTitle { get; set; }
    }

}