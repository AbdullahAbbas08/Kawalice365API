using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class InterviewerModel
    {
        public int InterviewerId { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewerPicture { get; set; }
        public string InterviewerDescription { get; set; }
        public string FacebookUrl { get; set; }
        public string InstgramUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public DateTime? CreationDate { get; set; }
        public string TiktokUrl { get; set; }
    }
}
