using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Interviewer
{
    public class MostViewProgramByInterviewerInput
    {
        public int InterviewerID { get; set; }
        public int? Top { get; set; }
        public string Order { get; set; }
    }
}
