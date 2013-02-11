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
        var customAttribute = new CustomAttribute(EditorBrowsableConstructor);
        customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(EditorBrowsableStateType, AdvancedStateConstant));
        customAttributes.Add(customAttribute);
    }
}