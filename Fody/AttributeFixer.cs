using System;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;


public partial class ModuleWeaver
{
    
    public void ProcessAttributes(IMemberDefinition memberDefinition)
    {
        try
        {
            CheckForNormalAttribute(memberDefinition);
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
        var methodDefinition = memberDefinition as MethodDefinition;
        if (methodDefinition != null)
        {
            if (methodDefinition.IsGetter || methodDefinition.IsSetter)
            {
                var error = string.Format("ObsoleteExAttribute is not valid on property gets or sets. Member: '{0}'.", memberDefinition.FullName);

                LogError(error);
            }
        }

        customAttributes.Remove(obsoleteExAttribute);   


        var attributeData = DataReader.ReadAttributeData(obsoleteExAttribute);

        ValidateVersion(attributeData);

        AddObsoleteAttribute(attributeData, customAttributes);
        if (HideObsoleteMembers)
        {
            AddEditorBrowsableAttribute(customAttributes);
        }
    }

    void AddObsoleteAttribute(AttributeData attributeData, Collection<CustomAttribute> customAttributes)
    {
        var customAttribute = new CustomAttribute(ObsoleteConstructorReference);

        var message = ConvertToMessage(attributeData);
        var messageArg = new CustomAttributeArgument(ModuleDefinition.TypeSystem.String, message);
        customAttribute.ConstructorArguments.Add(messageArg);

        var isError = GetIsError(attributeData);
        var isErrorArg = new CustomAttributeArgument(ModuleDefinition.TypeSystem.Boolean, isError);
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