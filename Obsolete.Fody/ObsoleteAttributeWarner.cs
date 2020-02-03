using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void CheckForNormalAttribute(IMemberDefinition memberDefinition)
    {
        var customAttributes = memberDefinition
            .CustomAttributes;
        var doNotWarnAboutObsoleteUsageAttribute = customAttributes
            .FirstOrDefault(x => x.AttributeType.Name == "DoNotWarnAboutObsoleteUsageAttribute");
        if (doNotWarnAboutObsoleteUsageAttribute != null)
        {
            customAttributes.Remove(doNotWarnAboutObsoleteUsageAttribute);
            return;
        }

        var obsoleteExAttribute = customAttributes
            .FirstOrDefault(x => x.AttributeType.Name == "ObsoleteAttribute");
        if (obsoleteExAttribute == null)
        {
            return;
        }
        var warning = $"The member `{memberDefinition.FullName}` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.";
        WriteWarning(warning);
    }
}