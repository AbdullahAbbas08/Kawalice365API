using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class EpisodesFilterForRecentlyInputs
    {
        public int? InterviewerID { get; set; }
        public int? CategoryID { get; set; }
        public int? ProgramTypeID { get; set; }
        public int? ProgramID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string IsRecently { get; set; }
    }
}
 