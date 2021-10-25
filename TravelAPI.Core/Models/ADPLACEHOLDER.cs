using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual ADSTYLES ADStyles  { get; set; }
        [JsonIgnore]
        public virtual ADTARGETS ADTargets { get; set; }
    }
}
