using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class RetrieveData<T>
    {
        public string Url  { get; set; }
        public List<T> DataList { get; set; } = new List<T>();
    }
}
