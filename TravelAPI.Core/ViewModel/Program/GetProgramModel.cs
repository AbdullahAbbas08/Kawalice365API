using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Program
{
    public class GetProgramModel : ProgramFilterModel
    {
        public double Hour { get; set; }
        public double Minute { get; set; }
        public string Date { get; set; }

    }
}
