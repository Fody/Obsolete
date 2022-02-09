using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference EditorBrowsableConstructor;
    public TypeDefinition EditorBrowsableStateType;
    public int AdvancedStateConstant;
    public int NeverStateConstant;

    void FindEditorBrowsableTypes()
    {
        if (HideObsoleteMembers == HideObsoleteMembersState.Off)
        {
            return;
        }

        var attributeType = FindTypeDefinition("System.ComponentModel.EditorBrowsableAttribute");
        EditorBrowsableConstructor = ModuleDefinition.ImportReference(attributeType.Methods.First(IsDesiredConstructor));
        EditorBrowsableStateType = FindTypeDefinition("System.ComponentModel.EditorBrowsableState");
        var advancedFieldDefinition = EditorBrowsableStateType.Fields.First(x => x.Name == "Advanced");
        AdvancedStateConstant = (int)advancedFieldDefinition.Constant;
        var neverFieldDefinition = EditorBrowsableStateType.Fields.First(x => x.Name == "Never");
        NeverStateConstant = (int)neverFieldDefinition.Constant;
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

    public enum HideObsoleteMembersState
    {
        Advanced,

        // some dirty trickery to be backward compatible
        True = Advanced,
        Never,
        Off,

        // some dirty trickery to be backward compatible
        False = Off,
    }
}