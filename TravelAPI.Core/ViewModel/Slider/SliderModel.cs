using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Slider
{
   public  class SliderModel
    {
        public int SliderId { get; set; }
        public string SliderTitle { get; set; }
        public string SliderImagePath { get; set; }
        public int SliderOrder { get; set; }
        public int SliderViews { get; set; } 
        public int ProgramIDFk { get; set; }
        public int EpisodeId { get; set; }
        public int CategoryId { get; set; }
        public int SessionId { get; set; }
        public string ProgramName { get; set; }
        public string YoutubeUrl { get; set; }
    }
}
