using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class NotificationsInsert
    {
        public string title { get; set; }
        public string Descriptions { get; set; }
        public IFormFile IMG { get; set; }
        public int EpisodeID { get; set; }
    }
} 
 