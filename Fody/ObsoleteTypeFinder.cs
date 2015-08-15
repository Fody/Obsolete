using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference ObsoleteConstructorReference;


    public void FindObsoleteType()
    {
        var obsoleteDefinition = GetDefinition();
        var constructor = obsoleteDefinition.Methods.First(x =>
            x.Parameters.Count == 2
            && x.Parameters[0].ParameterType.Name == "String"
            && x.Parameters[1].ParameterType.Name == "Boolean");
        ObsoleteConstructorReference = ModuleDefinition.ImportReference(constructor);

    }

    TypeDefinition GetDefinition()
    {

        var msCoreLibDefinition = AssemblyResolver.Resolve("mscorlib");
        var obsoleteDefinition = msCoreLibDefinition
            .MainModule
            .Types
            .FirstOrDefault(x => x.Name == "ObsoleteAttribute");
        if (obsoleteDefinition == null)
        {
            var systemRuntime = AssemblyResolver.Resolve("System.Runtime");
            obsoleteDefinition = systemRuntime.MainModule.Types.First(x => x.Name == "ObsoleteAttribute");
        }
        return obsoleteDefinition;
    }
}