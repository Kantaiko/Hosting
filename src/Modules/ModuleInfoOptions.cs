using System;
using System.Collections.Generic;

namespace Kantaiko.Hosting.Modules
{
    public class ModuleInfoOptions
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Version? Version { get; set; }
        public ModuleFlags Flags { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}
