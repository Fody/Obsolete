using Mono.Cecil;

public partial class ModuleWeaver
{
    public void CheckForNormalAttribute(IMemberDefinition member)
    {
        var customAttributes = member
            .CustomAttributes;
        var doNotWarnAboutObsoleteUsageAttribute = customAttributes
            .FirstOrDefault(_ => _.AttributeType.Name == "DoNotWarnAboutObsoleteUsageAttribute");
        if (doNotWarnAboutObsoleteUsageAttribute != null)
        {
            customAttributes.Remove(doNotWarnAboutObsoleteUsageAttribute);
            return;
        }

        var obsoleteExAttribute = customAttributes
            .FirstOrDefault(_ => _.AttributeType.Name == "ObsoleteAttribute");
        if (obsoleteExAttribute == null)
        {
            return;
        }

        if (customAttributes.Where(unknown => unknown.AttributeType.Name == "CompilerFeatureRequiredAttribute")
            .Any(unknown => unknown.ConstructorArguments.Any(_ => _.Value is "RequiredMembers")))
        {
            return;
        }

        var warning = $"The member `{member.FullName}` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.";
        WriteWarning(warning);
    }
}