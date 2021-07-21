using System;

namespace Kantaiko.Hosting.Modules
{
    [Flags]
    public enum ModuleFlags
    {
        None = 0,
        Library = 1 << 0,
        Universal = 1 << 1
    }
}
