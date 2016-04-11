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
        var throwsNotImplmented = false;
        var methodDefinition = memberDefinition as MethodDefinition;
        if (methodDefinition != null)
        {
            throwsNotImplmented = ThrowsNotImplmented(methodDefinition);
            if (methodDefinition.IsGetter || methodDefinition.IsSetter)
            {
                var error = $"ObsoleteExAttribute is not valid on property gets or sets. Member: `{memberDefinition.FullName}`.";
                LogError(error);
            }
        }
        else
        {
            var propertyDefinition = memberDefinition as PropertyDefinition;
            if (propertyDefinition != null)
            {
                throwsNotImplmented = ThrowsNotImplmented(propertyDefinition);
            }
        }

        customAttributes.Remove(obsoleteExAttribute);


        var attributeData = DataReader.ReadAttributeData(obsoleteExAttribute, throwsNotImplmented);

        try
        {
            ApplyVersionConvention(attributeData);
        }
        catch (WeavingException exception)
        {
            throw new WeavingException($"Could not process {memberDefinition.FullName}. {exception.Message}");
        }

        ValidateVersion(memberDefinition, attributeData);

        AddObsoleteAttribute(attributeData, customAttributes);
        if (HideObsoleteMembers)
        {
            AddEditorBrowsableAttribute(customAttributes);
        }
    }

    static bool ThrowsNotImplmented(PropertyDefinition propertyDefinition)
    {
        if (propertyDefinition.SetMethod != null)
        {
            if (ThrowsNotImplmented(propertyDefinition.SetMethod))
            {
                return true;
            }
        }
        if (propertyDefinition.GetMethod != null)
        {
            if (ThrowsNotImplmented(propertyDefinition.GetMethod))
            {
                return true;
            }
        }
        return false;
    }

    static bool ThrowsNotImplmented(MethodDefinition methodDefinition)
    {
        return methodDefinition.HasBody && methodDefinition.Body.Instructions
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
        var messageArgument = new CustomAttributeArgument(ModuleDefinition.TypeSystem.String, message);
        customAttribute.ConstructorArguments.Add(messageArgument);

        var isError = GetIsError(attributeData);
        var isErrorArgument = new CustomAttributeArgument(ModuleDefinition.TypeSystem.Boolean, isError);
        customAttribute.ConstructorArguments.Add(isErrorArgument);

        customAttributes.Add(customAttribute);
    }

    void ValidateVersion(IMemberDefinition memberDefinition, AttributeData attributeData)
    {
        if (attributeData.RemoveInVersion < attributeData.TreatAsErrorFromVersion)
        {
            var message = $"Cannot process '{memberDefinition.FullName}'. The version specified in 'RemoveInVersion' {attributeData.RemoveInVersion} is less than the version specified in 'TreatAsErrorFromVersion' {attributeData.TreatAsErrorFromVersion}. The member should be removed or 'RemoveInVersion' increased.";
            throw new WeavingException(message);
        }

        if (assemblyVersion >= attributeData.RemoveInVersion)
        {
            var message = $"Cannot process '{memberDefinition.FullName}'. The assembly version {assemblyVersion} is equal to or greater than version specified in 'RemoveInVersion' {attributeData.RemoveInVersion}. The member should be removed or 'RemoveInVersion' increased.";
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