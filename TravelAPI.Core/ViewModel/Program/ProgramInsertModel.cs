using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public  class ProgramInsertModel
    {
        public int?      ProgramId          { get; set; }
        public int?      InterviewerId      { get; set; }
        public int?      ProgramTypeId      { get; set; }
        public int?      CategoryId         { get; set; }
        public int?      ProgramOrder       { get; set; }
        public string    ProgramDescription { get; set; }
        public string    ProgramName        { get; set; }
        public string    ProgramImgPath     { get; set; }
        public IFormFile ProgramImg         { get; set; }
        public bool?     ProgramVisible     { get; set; }

        public string ProgramStartDate   { get; set; }
        public double Hour { get; set; } = 0;
        public double Minute { get; set; } = 0;
        public int?      ProgramViews       { get; set; }
        public Boolean changeDate { get; set; }


    }
}
