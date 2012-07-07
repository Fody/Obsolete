using System;
using System.Linq;
using Mono.Cecil;


public class AttributeFixer
{
    ObsoleteTypeFinder obsoleteTypeFinder;
    TypeSystem typeSystem;
    DataFormatter dataFormatter;
    ObsoleteAttributeWarner obsoleteAttributeWarner;
    Version assemblyVersion;

    public AttributeFixer(ObsoleteTypeFinder obsoleteTypeFinder, ModuleDefinition moduleDefinition, DataFormatter dataFormatter,ObsoleteAttributeWarner obsoleteAttributeWarner)
    {
        this.obsoleteTypeFinder = obsoleteTypeFinder;
        typeSystem = moduleDefinition.TypeSystem;
        this.dataFormatter = dataFormatter;
        this.obsoleteAttributeWarner = obsoleteAttributeWarner;
        assemblyVersion = moduleDefinition.Assembly.Name.Version;
    }

    public void ProcessAttributes(IMemberDefinition memberDefinition)
    {
        try
        {
            obsoleteAttributeWarner.ProcessAttributes(memberDefinition);
            InnerProcess(memberDefinition);
        }
        catch (Exception exception)
        {
            throw new WeavingException(string.Format("An error occurred processing '{0}'. Error: {1}", memberDefinition.FullName, exception.Message));
        }
    }

    void InnerProcess(IMemberDefinition memberDefinition)
    {
        var customAttributes = memberDefinition.CustomAttributes;
        var obsoleteExAttribute = customAttributes.FirstOrDefault(x => x.AttributeType.Name == "ObsoleteExAttribute");
        if (obsoleteExAttribute == null)
        {
            return;
        }

        customAttributes.Remove(obsoleteExAttribute);


        var attributeData = DataReader.ReadAttributeData(obsoleteExAttribute);

        ValidateVersion(attributeData);

        var customAttribute = new CustomAttribute(obsoleteTypeFinder.ConstructorReference);

        var message = dataFormatter.ConvertToMessage(attributeData);
        var messageArg = new CustomAttributeArgument(typeSystem.String, message);
        customAttribute.ConstructorArguments.Add(messageArg);

        var isError = GetIsError(attributeData);
        var isErrorArg = new CustomAttributeArgument(typeSystem.Boolean, isError);
        customAttribute.ConstructorArguments.Add(isErrorArg);

        customAttributes.Add(customAttribute);
    }

    void ValidateVersion(AttributeData attributeData)
    {
        if (attributeData.RemoveInVersion == null)
        {
            return;
        }
        if (assemblyVersion >= attributeData.RemoveInVersion)
        {
            throw new WeavingException("Version of assembly is higher than version specified in 'RemoveInVersion'. Member should be removed");
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