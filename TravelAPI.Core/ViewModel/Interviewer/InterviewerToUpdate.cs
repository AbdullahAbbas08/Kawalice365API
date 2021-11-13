using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class InterviewerToUpdate 
    {
        public int InterviewerId { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewerPicturePath { get; set; }
        public string InterviewerCoverePath { get; set; }
        public IFormFile InterviewerPicture { get; set; }
        public IFormFile InterviewerCover { get; set; }
        public string InterviewerDescription { get; set; }
        public string FacebookUrl { get; set; }
        public string InstgramUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string TiktokUrl { get; set; } 
        public string BirthDate { get; set; }
    }
}
