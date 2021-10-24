using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.ProgramTypes
{
    public class ProgramTypeToUpdate
    {
        public int? ProgramTypeId { get; set; }
        public string ProgramTypeTitle { get; set; }
        public string ProgramTypeImgPath { get; set; }
        public IFormFile ProgramTypeImg { get; set; }
        public int? ProgramTypeOrder { get; set; }
    }
}
