using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public class Category2
    {
        public Category2()
        {
            Programs = new HashSet<Program>();
        }
        [Key]
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryImg { get; set; }
        public DateTime CreationDate { get; set; }
        public string CategoryDescription { get; set; } 
        public bool CategoryVisible { get; set; }
        public int CategoryOrder { get; set; }
        public int CategoryViews { get; set; }

        //[JsonIgnore]
        public virtual ICollection<Program> Programs { get; set; }
    }
}
