using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.Clients
{
    public class AdsClientModel
    {
        public int AdId { get; set; }
        public string AdTitle { get; set; }
        public string ImagePath { get; set; }
        public string URL { get; set; }
        public int Views { get; set; }
        public int PlaceHolderID { get; set; }
        public int PlaceHolderCode { get; set; } 
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public DateTime PublishStartDate { get; set; }
        public DateTime PublishEndDate { get; set; }
        public string Dates { get; set; }
        public string Dated { get; set; }
        public double Minutes { get; set; }
        public double Minuted { get; set; }
        public double Hours { get; set; }
        public double Hourd { get; set; }
    }
}
