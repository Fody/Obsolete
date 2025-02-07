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

        if (customAttributes.Where(_ => _.AttributeType.Name == "CompilerFeatureRequiredAttribute")
            .Any(_ => _.ConstructorArguments.Any(_ => _.Value is "RequiredMembers")))
        {
            return;
        }

        WriteWarning($"The member `{member.FullName}` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.");
    }
}