using System.Collections.Generic;
using Kantaiko.Hosting.Introspection;

namespace Kantaiko.Hosting.Loader
{
    public class LoadedHost
    {
        public LoadedHost(HostInfo hostInfo, IReadOnlyList<LoadedModule> modules)
        {
            HostInfo = hostInfo;
            Modules = modules;
        }

        public HostInfo HostInfo { get; }
        public IReadOnlyList<LoadedModule> Modules { get; }
    }
}
