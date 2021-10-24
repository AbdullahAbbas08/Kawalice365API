using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class ProgramFilterModel
    {
        public int ProgramId { get; set; }
        public string ProgramDescription { get; set; }
        public string ProgramName { get; set; }
        public string CategoryName { get; set; }
        public string InterviewerName { get; set; }
        public string ProgramTypeName { get; set; }
        public string ProgramImg { get; set; }
        public DateTime CreationDate { get; set; }
        public int CategoryId { get; set; }
        public DateTime ProgramStartDate { get; set; }
        public int InterviewerId { get; set; }
        public int ProgramOrder { get; set; }
        public int ProgramTypeId { get; set; }
        public bool ProgramVisible { get; set; }

    }
}
