using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference ObsoleteConstructorReference;

    public void FindObsoleteType(List<TypeDefinition> systemTypes)
    {
        var obsoleteDefinition = systemTypes
            .Single(x => x.Name == "ObsoleteAttribute");
        var constructor = obsoleteDefinition.Methods.First(x =>
            x.Parameters.Count == 2
            && x.Parameters[0].ParameterType.Name == "String"
            && x.Parameters[1].ParameterType.Name == "Boolean");
        ObsoleteConstructorReference = ModuleDefinition.ImportReference(constructor);
    }
}