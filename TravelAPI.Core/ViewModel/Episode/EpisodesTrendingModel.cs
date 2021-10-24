using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Episode
{
    public class EpisodesTrendingModel
    {
        public int? CategoryID { get; set; }
        public int? ProgramTypeID { get; set; }
        public int? ProgramID { get; set; }
        public string IsRecently { get; set; }
    }
}
