using System.Linq;
using Mono.Cecil;

public class ObsoleteTypeFinder
{
    ModuleDefinition moduleDefinition;
    IAssemblyResolver assemblyResolver;
    public MethodReference ConstructorReference;

    public ObsoleteTypeFinder(ModuleDefinition moduleDefinition, IAssemblyResolver assemblyResolver)
    {
        this.moduleDefinition = moduleDefinition;
        this.assemblyResolver = assemblyResolver;
    }

    public void Execute()
    {
        var obsoleteDefinition = GetDefinition();
        var constructor = obsoleteDefinition.Methods.First(x =>
            x.Parameters.Count == 2
            && x.Parameters[0].ParameterType.Name == "String"
            && x.Parameters[1].ParameterType.Name == "Boolean");
        ConstructorReference = moduleDefinition.Import(constructor);

    }

    TypeDefinition GetDefinition()
    {

        var msCoreLibDefinition = assemblyResolver.Resolve("mscorlib");
        var obsoleteDefinition = msCoreLibDefinition
            .MainModule
            .Types
            .FirstOrDefault(x => x.Name == "ObsoleteAttribute");
        if (obsoleteDefinition == null)
        {
            var systemRuntime = assemblyResolver.Resolve("System.Runtime");
            obsoleteDefinition = systemRuntime.MainModule.Types.First(x => x.Name == "ObsoleteAttribute");
        }
        return obsoleteDefinition;
    }
}