using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class Interviewer
    {
        public Interviewer()
        {
            Programs = new HashSet<Program>();
        }
        public int InterviewerId { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewerPicture { get; set; }
        public string InterviewerCover { get; set; }
        public string InterviewerDescription { get; set; }
        public string FacebookUrl { get; set; }
        public string InstgramUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public DateTime? CreationDate { get; set; }
        public string TiktokUrl { get; set; }
        [JsonIgnore]
        public virtual ICollection<Program> Programs { get; set; }
    }
}
