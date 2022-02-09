using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    public void AddEditorBrowsableAttribute(Collection<CustomAttribute> customAttributes, HideObsoleteMembersState state)
    {
        if (customAttributes.Any(x => x.AttributeType.Name == "EditorBrowsableAttribute") || state == HideObsoleteMembersState.Off)
        {
            return;
        }
        var customAttribute = new CustomAttribute(EditorBrowsableConstructor);
        var customAttributeArgument = new CustomAttributeArgument(EditorBrowsableStateType, state == HideObsoleteMembersState.Advanced ? AdvancedStateConstant : NeverStateConstant);
        customAttribute.ConstructorArguments.Add(customAttributeArgument);
        customAttributes.Add(customAttribute);
    }
}