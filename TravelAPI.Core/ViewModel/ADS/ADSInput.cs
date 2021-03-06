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
        public string PublishStartDate { get; set; }
        public string PublishEndDate { get; set; }
        public string Dates { get; set; }
        public string Dated { get; set; }
        public double Minutes { get; set; }
        public double Minuted { get; set; }
        public double Hours { get; set; }
        public double Hourd { get; set; }
    }
}
