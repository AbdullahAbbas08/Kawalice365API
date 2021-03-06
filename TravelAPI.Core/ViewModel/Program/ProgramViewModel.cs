using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class ProgramViewModel
    {
       
        public string ProgramDescription { get; set; }
        public string ProgramName { get; set; }
        public IFormFile ProgramImg { get; set; }
        public string ProgramPromoUrl { get; set; }
        public bool? ProgramVisible { get; set; }
        public int CategoryId { get; set; }
        public string ProgramStartDate { get; set; }
        public double Hour { get; set; }
        public double Minute { get; set; }
        public int InterviewerId { get; set; }
        public int ProgramOrder { get; set; } 
        public int ProgramTypeId { get; set; }
        //public string ProgramTypeTitle { get; set; }

    }
}
