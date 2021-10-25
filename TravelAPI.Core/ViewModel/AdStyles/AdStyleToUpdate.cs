using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.AdStyles
{
    public class AdStyleToUpdate
    {
        public int ADStyleId { get; set; }
        public string ADStyleTitle { get; set; }
        public float? ADWidth { get; set; }
        public float? ADHeight { get; set; }
    }
}
