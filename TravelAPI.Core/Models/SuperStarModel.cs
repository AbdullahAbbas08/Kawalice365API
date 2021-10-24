using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Interviewer
{
    [Keyless]
    public class SuperStarModel
    { 
        public int InterviewerID { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewerDescription { get; set; }
        public string InterviewerPicture { get; set; }
        public int EpisodeViews { get; set; }
    }
}
 