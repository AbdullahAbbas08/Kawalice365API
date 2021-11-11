namespace BalarinaAPI.Core.ViewModel.Season
{
    public class SeasonToUpdate
    {
        public int SessionId { get; set; }
        public string SessionTitle { get; set; }
        public int? ProgramId { get; set; }
        public int? SeasonViews  { get; set; }
    }
}
