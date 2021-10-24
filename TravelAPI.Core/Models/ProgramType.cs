using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class ProgramType
    {
        public ProgramType()
        {
            Programs = new HashSet<Program>();
        }
        public int ProgramTypeId { get; set; }
        public string ProgramTypeTitle { get; set; }
        public string ProgramTypeImgPath  { get; set; }
        public int ProgramTypeOrder  { get; set; }
        public int ProgramTypeViews  { get; set; }


        [JsonIgnore]
        public virtual ICollection<Program> Programs { get; set; }
    }
}
