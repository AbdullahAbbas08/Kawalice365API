﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel.AdvertisementTargets
{
    public class AdvertisementTargetToUpdate
    {
        public int ADTargetID { get; set; }
        public string ADTargetTitle { get; set; }
        public string ADTargetType { get; set; }
        public int? ItemID { get; set; }
    }
}
