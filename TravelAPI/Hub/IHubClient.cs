using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BalarinaAPI.Hub
{
    public interface IHubClient
    {
        Task BroadCastNotification();
    }
}
