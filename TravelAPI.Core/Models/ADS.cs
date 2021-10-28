using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TravelAPI.Models;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public class ADS
    {
        [Key]
        public int AdId { get; set; }
        [Required]
        public string AdTitle { get; set; }
        [Required]
        public string ImagePath { get; set; }
        [Required]
        public string URL { get; set; }
        [Required]
        public int Views { get; set; }
        public int PlaceHolderID { get; set; }
        public string ClientID { get; set; }
        [Required]
        public DateTime PublishStartDate { get; set; }
        [Required]
        public DateTime PublishEndDate { get; set; }

        [JsonIgnore]
        [ForeignKey("PlaceHolderID")]
        public virtual ADPLACEHOLDER ADPLACEHOLDER { get; set; }

        [ForeignKey("ClientID")]
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
