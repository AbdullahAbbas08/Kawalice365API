using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class Program
    {
        public Program()
        {
            Sessions = new HashSet<Seasons>();
        }
        public int ProgramId { get; set; }
        public string ProgramDescription { get; set; }
        public string ProgramName { get; set; }
        public string ProgramImg { get; set; }
        public string ProgramPromoUrl { get; set; }
        public bool? ProgramVisible { get; set; }
        public int CategoryId { get; set; }
        public DateTime ProgramStartDate { get; set; }
        public int InterviewerId { get; set; }
        public int ProgramOrder { get; set; }
        public int ProgramTypeId { get; set; }
        public int ProgramViews { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryId")]
        public virtual Category Category  { get; set; }
        [JsonIgnore]
        public virtual Interviewer Interviewer { get; set; }
        [JsonIgnore]
        public virtual ProgramType ProgramType { get; set; }
        [JsonIgnore]
        public virtual ICollection<Seasons> Sessions { get; set; }
    }
}
