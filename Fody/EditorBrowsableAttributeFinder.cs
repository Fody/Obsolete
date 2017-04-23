using System;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

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
            var assemblyDefinition = AssemblyResolver.Resolve(new AssemblyNameReference("System", null));
            if (assemblyDefinition != null && assemblyDefinition.MainModule.Types.Any(x => x.Name == "EditorBrowsableAttribute"))
            {
                FindFromTypes(assemblyDefinition.MainModule.Types);
            }
            else
            {
                var systemRuntime = AssemblyResolver.Resolve(new AssemblyNameReference("System.Runtime", null));
                FindFromTypes(systemRuntime.MainModule.Types);
            }
        }
        catch (Exception exception)
        {
            throw new WeavingException($"Could not enable HideObsoleteMembers due to problem finding EditorBrowsableAttribute. Disable HideObsoleteMembers and raise an issue detailing what runtime you are compiling against. Inner Exception:{exception}");
        }
    }

    void FindFromTypes(Collection<TypeDefinition> typeDefinitions)
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