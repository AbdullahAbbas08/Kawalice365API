using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.DetailsAPI
{
    public class DetailAPIModel
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramImg { get; set; }
        public string ProgramDescription { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public int ProgramTypeId { get; set; }
        public string ProgramTypeTitle { get; set; }
        public int InterviewerId { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewerPicture { get; set; }
        public int SessionId { get; set; }
        public int EpisodesCount { get; set; }
        public string SessionTitle { get; set; }
        public int EpisodeId { get; set; }
        public string EpisodeTitle { get; set; }
        public string EpisodeDescription { get; set; }
        public string EpisodeIamgePath { get; set; }
        public DateTime EpisodePublishDate { get; set; }
        public DateTime ProgramStartDate { get; set; }
        public int EpisodeViews { get; set; }
        public string YoutubeUrl { get; set; }
    }
}
