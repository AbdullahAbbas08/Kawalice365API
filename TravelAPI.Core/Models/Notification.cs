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
    public class Notification
    {
        [Key]
        public int ID { get; set; }
        public string title { get; set; }
        public string Descriptions { get; set; }
        public string IMG { get; set; }
        public int EpisodeID { get; set; }
        public bool Visible  { get; set; }

        [JsonIgnore]
        [ForeignKey("EpisodeID")]
        public virtual Episode Episode { get; set; }
    }
}
