using System;

namespace BalarinaAPI.Core.ViewModel
{
    public class EpisodesRecently
    {
        public int? InterviewerID { get; set; } = null;
        public int? CategoryID    { get; set; } = null;
        public DateTime? DateFrom { get; set; } = null;
        public int? ProgramTypeID { get; set; } = null;
        public int? ProgramID { get; set; } = null;
    }

}
