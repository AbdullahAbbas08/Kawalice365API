using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class ProgramFilterInputs
    {
        public int? CategoryID { get; set; }
        public int? ProgramTypeID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string ProgramName { get; set; }
    }
}
