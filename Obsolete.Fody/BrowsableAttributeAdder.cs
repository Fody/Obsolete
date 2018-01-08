using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    public void AddEditorBrowsableAttribute(Collection<CustomAttribute> customAttributes)
    {
        if (customAttributes.Any(x => x.AttributeType.Name == "EditorBrowsableAttribute"))
        {
            return;
        }
        var customAttribute = new CustomAttribute(EditorBrowsableConstructor);        var customAttributeArgument = new CustomAttributeArgument(EditorBrowsableStateType, AdvancedStateConstant);        customAttribute.ConstructorArguments.Add(customAttributeArgument);
        customAttributes.Add(customAttribute);
    }
}