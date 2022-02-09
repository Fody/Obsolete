using Mono.Cecil;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    public void AddEditorBrowsableAttribute(Collection<CustomAttribute> attributes, HideObsoleteMembersState state)
    {
        if (attributes.Any(x => x.AttributeType.Name == "EditorBrowsableAttribute") || state == HideObsoleteMembersState.Off)
        {
            return;
        }

        var attribute = new CustomAttribute(EditorBrowsableConstructor);
        var memberState = state == HideObsoleteMembersState.Advanced ? AdvancedStateConstant : NeverStateConstant;
        var attributeArgument = new CustomAttributeArgument(EditorBrowsableStateType, memberState);
        attribute.ConstructorArguments.Add(attributeArgument);
        attributes.Add(attribute);
    }
}