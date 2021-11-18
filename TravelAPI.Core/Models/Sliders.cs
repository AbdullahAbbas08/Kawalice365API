using BalarinaAPI.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.Models
{
    public class Sliders
    {
        [Key]
        public int      SliderId            { get; set; }
        public string   SliderTitle         { get; set; }
        public string   SliderImagePath     { get; set; }
        public int      SliderOrder         { get; set; }
        [JsonIgnore]
        public int      SliderViews         { get; set; }
        public int      ProgramIDFk          { get; set; }

        [JsonIgnore]
        [ForeignKey("ProgramIDFk")]
        public virtual ICollection<Program> Programs { get; set; }
    }
}
