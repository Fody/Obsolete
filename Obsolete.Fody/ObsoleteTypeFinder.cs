using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference ObsoleteConstructorReference;

    public void FindObsoleteType()
    {
        var obsoleteDefinition = FindType("System.ObsoleteAttribute");
        var constructor = obsoleteDefinition.Methods.First(x =>
            x.Parameters.Count == 2
            && x.Parameters[0].ParameterType.Name == "String"
            && x.Parameters[1].ParameterType.Name == "Boolean");
        ObsoleteConstructorReference = ModuleDefinition.ImportReference(constructor);
    }
}