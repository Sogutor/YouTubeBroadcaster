using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Broadcaster.Interfaces.Enums;

namespace Broadcaster.Interfaces
{
    public interface IBroadcast
    {
        string Url { get; }
        string Title { get; }
        string Description { get; }
        PrivacyEnum PrivacyStatus { get; }
        Resolution Resolution { get; }
        FrameRate FrameRate { get; }
        DateTime ScheduledStartTime { get; }
        string PathToStream { get; }
        string Id { get; }
    }


}
