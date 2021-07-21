using System;
using System.Collections.Generic;
using Kantaiko.Hosting.Modules;

namespace Kantaiko.Hosting.Internal
{
    public class ModuleCollection : IModuleCollection
    {
        internal List<Type> ModuleTypes { get; } = new();

        public void Add<T>() where T : class, IModule
        {
            if (!ModuleTypes.Contains(typeof(T)))
                ModuleTypes.Add(typeof(T));
        }

        public void AddModuleTypes(IReadOnlyList<Type> moduleTypes)
        {
            foreach (var type in moduleTypes)
            {
                if (ModuleTypes.Contains(type))
                    return;

                ModuleTypes.Add(type);
            }
        }
    }
}
