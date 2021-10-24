using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public class ADPLACEHOLDER
    {
        [Key]
        public int ADPlaceholderID { get; set; }
        public int AdStyleID  { get; set; }
        public int AdTargetId   { get; set; }
        [Required]
        public string Title  { get; set; }
        [Required]
        public string ImagePath  { get; set; }

        public virtual ADSTYLES ADStyles  { get; set; }
        public virtual ADTARGETS ADTargets { get; set; }
    }
}
