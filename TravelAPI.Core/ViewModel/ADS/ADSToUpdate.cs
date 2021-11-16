using Microsoft.AspNetCore.Http;
using System;

namespace BalarinaAPI.Core.ViewModel.ADS
{
    public class ADSToUpdate 
    {
        public int AdId { get; set; }
        public string AdTitle { get; set; }
        public IFormFile Image { get; set; }
        public string ImagePath { get; set; }
        public string URL { get; set; }
        public int? Views { get; set; }
        public int? PlaceHolderID { get; set; }
        public string ClientID { get; set; }
        public string PublishStartDate { get; set; }
        public string PublishEndDate { get; set; }
    }
}
