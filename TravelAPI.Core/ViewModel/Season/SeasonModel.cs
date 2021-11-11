using System;

namespace BalarinaAPI.Core.ViewModel.Season
{
    public class SeasonModel 
    {
        public int SessionId { get; set; }
        public string SessionTitle { get; set; }
        public int ProgramId { get; set; }
        public int SeasonViews { get; set; }
        public DateTime CreationDate { get; set; }
        public string ProgramName { get; set; }
        public int EpisodesCount  { get; set; }
    }
}
