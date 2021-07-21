using System;
using System.Collections.Generic;

namespace Kantaiko.Hosting.Runtime
{
    public interface IRuntimeHostState
    {
        bool RestartRequested { get; }
        DateTime RestartRequestedAt { get; }
        Dictionary<string, object> Properties { get; }

        bool HostRestarted { get; }
        bool RestartFailed { get; }
        DateTime RestartCompletedAt { get; }
        Exception? StartupException { get; }
    }
}