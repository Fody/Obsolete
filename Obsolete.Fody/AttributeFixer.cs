using Fody;
using Mono.Cecil;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    void ProcessAttributes(IMemberDefinition member)
    {
        CheckForNormalAttribute(member);

        InnerProcess(member);
    }

    void InnerProcess(IMemberDefinition member)
    {
        var customAttributes = member.CustomAttributes;
        var obsoleteExAttribute = customAttributes.FirstOrDefault(x => x.AttributeType.Name == "ObsoleteExAttribute");
        if (obsoleteExAttribute == null)
        {
            return;
        }
        var throwsNotImplemented = false;
        if (member is MethodDefinition method)
        {
            throwsNotImplemented = ThrowsNotImplemented(method);
            if (method.IsGetter || method.IsSetter)
            {
                var error = $"ObsoleteExAttribute is not valid on property gets or sets. Member: `{member.FullName}`.";
                WriteError(error);
            }
        }
        else if (member is PropertyDefinition property)
        {
            throwsNotImplemented = ThrowsNotImplemented(property);
        }

        customAttributes.Remove(obsoleteExAttribute);


        var attributeData = DataReader.ReadAttributeData(obsoleteExAttribute, throwsNotImplemented);

        try
        {
            ApplyVersionConvention(attributeData);
        }
        catch (WeavingException exception)
        {
            throw new WeavingException($"Could not process {member.FullName}. {exception.Message}");
        }

        ValidateVersion(member, attributeData);

        AddObsoleteAttribute(attributeData, customAttributes);
        AddEditorBrowsableAttribute(customAttributes, HideObsoleteMembers);
    }

    static bool ThrowsNotImplemented(PropertyDefinition property)
    {
        if (property.SetMethod != null)
        {
            if (ThrowsNotImplemented(property.SetMethod))
            {
                return true;
            }
        }
        if (property.GetMethod != null)
        {
            if (ThrowsNotImplemented(property.GetMethod))
            {
                return true;
            }
        }
        return false;
    }

    static bool ThrowsNotImplemented(MethodDefinition method)
    {
        return method.HasBody && method.Body.Instructions
            .Select(instruction => instruction.Operand)
            .OfType<MethodReference>()
            .Select(methodReference => methodReference.FullName)
            .Any(fullName => fullName == "System.Void System.NotImplementedException::.ctor()");
    }

    void ApplyVersionConvention(AttributeData attributeData)
    {
        if (attributeData.TreatAsErrorFromVersion == null)
        {
            if (attributeData.RemoveInVersion == null)
            {
                attributeData.TreatAsErrorFromVersion = assemblyVersion.Increment(StepType);
            }
            else
            {
                attributeData.TreatAsErrorFromVersion = attributeData.RemoveInVersion.Decrement(StepType);
            }
        }
        if (attributeData.RemoveInVersion == null)
        {
            attributeData.RemoveInVersion = attributeData.TreatAsErrorFromVersion.Increment(StepType);
        }
    }

    void AddObsoleteAttribute(AttributeData attributeData, Collection<CustomAttribute> customAttributes)
    {
        var customAttribute = new CustomAttribute(ObsoleteConstructorReference);

        var message = ConvertToMessage(attributeData);
        var messageArgument = new CustomAttributeArgument(TypeSystem.StringReference, message);
        customAttribute.ConstructorArguments.Add(messageArgument);

        var isError = GetIsError(attributeData);
        var isErrorArgument = new CustomAttributeArgument(TypeSystem.BooleanReference, isError);
        customAttribute.ConstructorArguments.Add(isErrorArgument);

        customAttributes.Add(customAttribute);
    }

    void ValidateVersion(IMemberDefinition member, AttributeData attributeData)
    {
        if (attributeData.RemoveInVersion < attributeData.TreatAsErrorFromVersion)
        {
            var message = $"Cannot process '{member.FullName}'. The version specified in 'RemoveInVersion' {attributeData.RemoveInVersion} is less than the version specified in 'TreatAsErrorFromVersion' {attributeData.TreatAsErrorFromVersion}. The member should be removed or 'RemoveInVersion' increased.";
            throw new WeavingException(message);
        }

        if (assemblyVersion >= attributeData.RemoveInVersion)
        {
            var message = $"Cannot process '{member.FullName}'. The assembly version {assemblyVersion} is equal to or greater than version specified in 'RemoveInVersion' {attributeData.RemoveInVersion}. The member should be removed or 'RemoveInVersion' increased.";
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