using Microsoft.CodeAnalysis;

namespace Kantaiko.Hosting.Modularity.Generator.Models;

public class ModuleExtensionInfo
{
    public ModuleExtensionInfo(ITypeSymbol moduleType,
        string moduleName,
        ModuleExtensionFlags flags,
        ITypeSymbol? additionType)
    {
        ModuleType = moduleType;
        ModuleName = moduleName;
        Flags = flags;
        AdditionType = additionType;
    }

    public ITypeSymbol ModuleType { get; }
    public string ModuleName { get; }
    public ModuleExtensionFlags Flags { get; }
    public ITypeSymbol? AdditionType { get; }
}
