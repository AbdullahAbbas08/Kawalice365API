using Microsoft.AspNetCore.Http;

namespace BalarinaAPI.Core.ViewModel.ADPLACEHOLDER
{
    public class PlaceholderToUpdate 
    {
        public int ADPlaceholderID { get; set; }
        public int? ADPlaceholderCode { get; set; }
        public int? AdStyleID { get; set; }
        public int? AdTargetId { get; set; }
        public string Title { get; set; }
        public IFormFile Image { get; set; }
        public string ImagePath { get; set; }
    }
}
