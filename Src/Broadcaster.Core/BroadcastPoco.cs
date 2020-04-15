using System;
using Broadcaster.Interfaces;
using Broadcaster.Interfaces.Enums;

namespace Broadcaster.Core
{
    public class BroadcastPoco : IBroadcast
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PrivacyEnum PrivacyStatus { get; set; }
        public Resolution Resolution { get; set; }
        public FrameRate FrameRate { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public string PathToStream { get; set; }
        public string Id { get; set; }
   
    }
}