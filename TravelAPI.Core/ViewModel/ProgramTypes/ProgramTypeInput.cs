using Microsoft.AspNetCore.Http;

namespace BalarinaAPI.Core.ViewModel.ProgramTypes
{
    public class ProgramTypeInput 
    {
        public string ProgramTypeTitle { get; set; }
        public IFormFile ProgramTypeImg { get; set; }
        public int ProgramTypeOrder { get; set; }
    }
}
