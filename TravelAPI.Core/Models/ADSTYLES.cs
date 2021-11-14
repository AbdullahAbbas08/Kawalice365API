using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public class ADSTYLES
    {
        [Key]
        public int          ADStyleId       { get; set; }
        [Required]
        public string       ADStyleTitle    { get; set; }
        [Required]
        public float        ADWidth         { get; set; }
        [Required]
        public float        ADHeight        { get; set; }
        [JsonIgnore]
        public virtual ICollection<ADPLACEHOLDER> ADPLACEHOLDERS { get; set; }
    }
}
