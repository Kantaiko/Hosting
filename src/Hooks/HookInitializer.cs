using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kantaiko.Hosting.Hooks
{
    internal class HookInitializer
    {
        private readonly IHookDispatcher _hookDispatcher;
        private readonly HookHandlerCollection _hookHandlerCollection;

        public HookInitializer(IHookDispatcher hookDispatcher, HookHandlerCollection hookHandlerCollection)
        {
            _hookDispatcher = hookDispatcher;
            _hookHandlerCollection = hookHandlerCollection;
        }

        public void Initialize(IReadOnlyList<Assembly> assemblies)
        {
            var hookHandlerTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract)
                .ToArray();

            foreach (var hookHandlerType in hookHandlerTypes)
            {
                var hookInterface = hookHandlerType
                    .GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType &&
                                         (x.GetGenericTypeDefinition() == typeof(IHookHandler<>) ||
                                          x.GetGenericTypeDefinition() == typeof(IAsyncHookHandler<>)));

                if (hookInterface is null)
                    continue;

                var eventType = hookInterface.GetGenericArguments()[0];
                if (!_hookHandlerCollection.HookHandlers.Contains(eventType, hookHandlerType))
                    _hookHandlerCollection.HookHandlers.Add(eventType, hookHandlerType);
            }

            var applicationInitializedHook = new ApplicationInitializedHook(assemblies);
            _hookDispatcher.Dispatch(applicationInitializedHook);
        }
    }
}
