using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.ADS
{
    public  class ADSInput
    {
        public string AdTitle { get; set; }
        public IFormFile Image { get; set; }
        public string URL { get; set; }
        public int Views { get; set; }
        public int PlaceHolderID { get; set; }
        public string ClientID { get; set; }
        public DateTime PublishStartDate { get; set; }
        public DateTime PublishEndDate { get; set; }
    }
}
