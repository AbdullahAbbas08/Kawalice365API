using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.ADPLACEHOLDER
{
    public  class PlaceholderInput
    {
        public int ADPlaceholderCode { get; set; }

        public int AdTargetId { get; set; }
        public int AdStyleID { get; set; }

        public string Title { get; set; }
        public IFormFile Image { get; set; }
    }
}
