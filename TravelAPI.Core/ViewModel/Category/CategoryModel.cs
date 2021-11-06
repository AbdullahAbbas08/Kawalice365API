using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Category
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryImg { get; set; }
        public DateTime CreationDate { get; set; }
        public string CategoryDescription { get; set; }
        public bool CategoryVisible { get; set; }
        public int CategoryOrder { get; set; }
        public int CategoryViews { get; set; }
        public int ProgramsCount { get; set; }
    }
}
