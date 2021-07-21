using System;

namespace Kantaiko.Hosting.Modules
{
    public class ModuleFlagsAttribute : Attribute, IModuleInfoConfigurationMiddleware
    {
        private readonly ModuleFlags _moduleFlags;

        public ModuleFlagsAttribute(ModuleFlags moduleFlags)
        {
            _moduleFlags = moduleFlags;
        }

        public void ConfigureInfo(ModuleInfoOptions options)
        {
            options.Flags = _moduleFlags;
        }
    }
}
