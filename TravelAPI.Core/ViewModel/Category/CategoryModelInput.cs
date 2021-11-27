using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public  class CategoryModelInput
    {
        public string CategoryTitle { get; set; }
        public IFormFile CategoryImg { get; set; }
        public string CategoryDescription { get; set; }
        public bool CategoryVisible { get; set; }
        public int CategoryOrder { get; set; }
        //public int CategoryViews { get; set; }
        public string CategoryImgPath { get; set; }

    }
}
