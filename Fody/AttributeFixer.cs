using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;


public partial class ModuleWeaver
{

    void ProcessAttributes(IMemberDefinition memberDefinition)
    {
        CheckForNormalAttribute(memberDefinition);
        InnerProcess(memberDefinition);
    }

    void InnerProcess(IMemberDefinition memberDefinition)
    {
        var customAttributes = memberDefinition.CustomAttributes;
        var obsoleteExAttribute = customAttributes.FirstOrDefault(x => x.AttributeType.Name == "ObsoleteExAttribute");
        if (obsoleteExAttribute == null)
        {
            return;
        }
        var methodDefinition = memberDefinition as MethodDefinition;
        if (methodDefinition != null)
        {
            if (methodDefinition.IsGetter || methodDefinition.IsSetter)
            {
                var error = string.Format("ObsoleteExAttribute is not valid on property gets or sets. Member: `{0}`.", memberDefinition.FullName);

                LogError(error);
            }
        }

        customAttributes.Remove(obsoleteExAttribute);   


        var attributeData = DataReader.ReadAttributeData(obsoleteExAttribute);


        ApplyVersionConvention(attributeData);


        ValidateVersion(memberDefinition, attributeData);

        AddObsoleteAttribute(attributeData, customAttributes);
        if (HideObsoleteMembers)
        {
            AddEditorBrowsableAttribute(customAttributes);
        }
    }

    void ApplyVersionConvention(AttributeData attributeData)
    {
        if (attributeData.TreatAsErrorFromVersion == null)
        {
            if (attributeData.RemoveInVersion == null)
            {
                attributeData.TreatAsErrorFromVersion = assemblyVersion.Add(VersionIncrement);
            }
            else
            {
                attributeData.TreatAsErrorFromVersion = attributeData.RemoveInVersion.Subtract(VersionIncrement);
            }
        }
        if (attributeData.RemoveInVersion == null)
        {
            attributeData.RemoveInVersion = attributeData.TreatAsErrorFromVersion.Add(VersionIncrement);
        }
    }

    void AddObsoleteAttribute(AttributeData attributeData, Collection<CustomAttribute> customAttributes)
    {
        var customAttribute = new CustomAttribute(ObsoleteConstructorReference);

        var message = ConvertToMessage(attributeData);
        var messageArgument = new CustomAttributeArgument(ModuleDefinition.TypeSystem.String, message);
        customAttribute.ConstructorArguments.Add(messageArgument);

        var isError = GetIsError(attributeData);
        var isErrorArgument = new CustomAttributeArgument(ModuleDefinition.TypeSystem.Boolean, isError);
        customAttribute.ConstructorArguments.Add(isErrorArgument);

        customAttributes.Add(customAttribute);
    }

    void ValidateVersion(IMemberDefinition memberDefinition, AttributeData attributeData)
    {
        if ( attributeData.RemoveInVersion < attributeData.TreatAsErrorFromVersion)
        {
            var message = string.Format("Cannot process '{0}'. The version specified in 'RemoveInVersion' {1} is less than the version specified in 'TreatAsErrorFromVersion' {2}. The member should be removed or 'RemoveInVersion' increased.", memberDefinition.FullName, attributeData.RemoveInVersion, attributeData.TreatAsErrorFromVersion);
            throw new WeavingException(message);
        }

        if (assemblyVersion >= attributeData.RemoveInVersion)
        {
            var message = string.Format("Cannot process '{0}'. The assembly version {1} is higher than version specified in 'RemoveInVersion' {2}. The member should be removed or 'RemoveInVersion' increased.", memberDefinition.FullName, assemblyVersion, attributeData.RemoveInVersion);
            throw new WeavingException(message);
        }
    }

    bool GetIsError(AttributeData attributeData)
    {
        if (attributeData.TreatAsErrorFromVersion != null)
        {
            if (assemblyVersion >= attributeData.TreatAsErrorFromVersion)
            {
                return true;
            }
        }
        return false;
    }
}