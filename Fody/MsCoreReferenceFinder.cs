using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference EditorBrowsableConstructor;
    public TypeDefinition EditorBrowsableStateType;
    public int AdvancedStateConstant;



    public void FindSystemTypes()
    {
        var assemblyDefinition = ModuleDefinition.AssemblyResolver.Resolve("System");
        var msCoreTypes = assemblyDefinition.MainModule.Types;

        var attribyteType = msCoreTypes.First(x => x.Name == "EditorBrowsableAttribute");
        EditorBrowsableConstructor = ModuleDefinition.Import(attribyteType.Methods.First(IsDesiredConstructor));
        EditorBrowsableStateType = msCoreTypes.First(x => x.Name == "EditorBrowsableState");
        var fieldDefinition = EditorBrowsableStateType.Fields.First(x => x.Name == "Advanced");
        AdvancedStateConstant =(int) fieldDefinition.Constant;
    }

    static bool IsDesiredConstructor(MethodDefinition x)
    {
        if (!x.IsConstructor)
        {
            return false;
        }
        if (x.Parameters.Count != 1)
        {
            return false;
        }
        return x.Parameters[0].ParameterType.Name == "EditorBrowsableState";
    }

}