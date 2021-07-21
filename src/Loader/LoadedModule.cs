using Kantaiko.Hosting.Introspection;
using Kantaiko.Hosting.Modules;

namespace Kantaiko.Hosting.Loader
{
    public class LoadedModule
    {
        public LoadedModule(ModuleInfo moduleInfo, IModule instance)
        {
            ModuleInfo = moduleInfo;
            Instance = instance;
        }

        public ModuleInfo ModuleInfo { get; }
        public IModule Instance { get; }
    }
}
