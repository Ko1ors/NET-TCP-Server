using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_TCP_Server.Services
{
    public interface IDataService
    {
        Dictionary<string, string> GetData();
    }
}
