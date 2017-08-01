using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference EditorBrowsableConstructor;
    public TypeDefinition EditorBrowsableStateType;
    public int AdvancedStateConstant;

    public void FindSystemTypes()
    {
        if (!HideObsoleteMembers)
        {
            return;
        }

        try
        {
            var types = new List<TypeDefinition>();

            AddAssemblyIfExists("System", types);
            AddAssemblyIfExists("System.Runtime", types);
            AddAssemblyIfExists("netstandard", types);

            FindFromTypes(types);
        }
        catch (Exception exception)
        {
            throw new WeavingException($"Could not enable HideObsoleteMembers due to problem finding EditorBrowsableAttribute. Disable HideObsoleteMembers and raise an issue detailing what runtime you are compiling against. Inner Exception:{exception}");
        }
    }

    void AddAssemblyIfExists(string name, List<TypeDefinition> types)
    {
        try
        {
            var msCoreLibDefinition = AssemblyResolver.Resolve(new AssemblyNameReference(name, null));

            if (msCoreLibDefinition != null)
            {
                types.AddRange(msCoreLibDefinition.MainModule.Types);
            }
        }
        catch (AssemblyResolutionException)
        {
            LogInfo($"Failed to resolve '{name}'. So skipping its types.");
        }
    }

    void FindFromTypes(List<TypeDefinition> typeDefinitions)
    {
        var attributeType = typeDefinitions.First(x => x.Name == "EditorBrowsableAttribute");
        EditorBrowsableConstructor = ModuleDefinition.ImportReference(attributeType.Methods.First(IsDesiredConstructor));
        EditorBrowsableStateType = typeDefinitions.First(x => x.Name == "EditorBrowsableState");
        var fieldDefinition = EditorBrowsableStateType.Fields.First(x => x.Name == "Advanced");
        AdvancedStateConstant = (int) fieldDefinition.Constant;
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