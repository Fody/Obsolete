using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference EditorBrowsableConstructor;
    public TypeDefinition EditorBrowsableStateType;
    public int AdvancedStateConstant;



    void FindEditorBrowsableTypes(List<TypeDefinition> typeDefinitions)
    {
        if (!HideObsoleteMembers)
        {
            return;
        }
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