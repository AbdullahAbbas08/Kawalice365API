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
        public string Hour { get; set; } = "00";
        public string Minute { get; set; } = "00";
        public int?      ProgramViews       { get; set; }

    }
}
