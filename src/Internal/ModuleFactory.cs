using System;
using System.Collections.Generic;
using Kantaiko.Hosting.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Internal
{
    internal class ModuleFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public ModuleFactory(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public object ConstructModuleInstance(Type type)
        {
            var constructors = type.GetConstructors();

            switch (constructors.Length)
            {
                case 0:
                    throw new ModuleConstructionException(type, "Module must contain accessible constructor");
                case > 1:
                    throw new ModuleConstructionException(type, "Module cannot contain multiple constructors");
            }

            var constructor = constructors[0];
            var constructorParameters = new List<object>();

            foreach (var parameterInfo in constructor.GetParameters())
            {
                if (parameterInfo.ParameterType == typeof(IConfiguration))
                {
                    constructorParameters.Add(_configuration);
                    continue;
                }

                if (parameterInfo.ParameterType == typeof(IHostEnvironment))
                {
                    constructorParameters.Add(_hostEnvironment);
                    continue;
                }

                throw new ModuleConstructionException(type,
                    $"Unable to provide {parameterInfo.ParameterType.Name} to module constructor");
            }

            return constructor.Invoke(constructorParameters.ToArray());
        }
    }
}
