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
        var attributeType = typeDefinitions.FirstOrDefault(x => x.Name == "EditorBrowsableAttribute");
        if (attributeType == null)
        {
            throw new WeavingException("Could not find EditorBrowsableAttribute");
        }
        EditorBrowsableConstructor = ModuleDefinition.ImportReference(attributeType.Methods.First(IsDesiredConstructor));
        EditorBrowsableStateType = typeDefinitions.FirstOrDefault(x => x.Name == "EditorBrowsableState");
        if (EditorBrowsableStateType == null)
        {
            throw new WeavingException("Could not find EditorBrowsableAttribute");
        }
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