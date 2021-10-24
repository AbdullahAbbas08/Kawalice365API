using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public class ADTARGETS
    {
        [Key]
        public int ADTargetID  { get; set; }
        [Required]
        public string ADTargetTitle   { get; set; }
        [Required]
        public int ADTargetType   { get; set; }
        [Required]
        public int ItemID   { get; set; }
        public virtual ICollection<ADPLACEHOLDER> ADPLACEHOLDERS { get; set; }
    }
}
