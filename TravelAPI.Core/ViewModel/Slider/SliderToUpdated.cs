using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Slider
{
    public class SliderToUpdated
    {
        public int SliderId          { get; set; }
        public IFormFile SliderImage { get; set; }
        public string SliderImagePath { get; set; }
        public string SliderTitle  { get; set; }
        public int? SliderOrder       { get; set; }
        public int? EpisodeID          { get; set; }
    }
}
