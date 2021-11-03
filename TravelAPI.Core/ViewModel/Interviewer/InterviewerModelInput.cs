using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Interviewer
{
    public class InterviewerModelInput
    {
        public string InterviewerName { get; set; }
        public IFormFile _InterviewerPicture { get; set; }
        public string InterviewerDescription { get; set; }
        public string FacebookUrl { get; set; }
        public string InstgramUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string TiktokUrl { get; set; }
        public DateTime? BirthDate { get; set; }
        public IFormFile InterviewerCover { get; set; }

    }
}
