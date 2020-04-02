using System;
using System.Collections.Generic;

namespace OrchardCore.Notifications
{
    public interface IEvent
    {
        ICollection<string> Tags { get; }
		
        DateTimeOffset TimeStamp { get; }
    }
}
