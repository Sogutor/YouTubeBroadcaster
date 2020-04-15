using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcaster.Interfaces
{
    public interface IBroadcastCreator
    {
        IBroadcast Create(IBroadcast broadcast);
    }
}
