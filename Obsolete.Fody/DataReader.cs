using Mono.Cecil;

public static class DataReader
{
    public static AttributeData ReadAttributeData(CustomAttribute attribute, bool throwsNotImplemented)
    {
        return new AttributeData
        {
            Message = attribute.GetValue("Message"),
            Replacement = attribute.GetValue("ReplacementTypeOrMember"),
            TreatAsErrorFromVersion = attribute.GetValue("TreatAsErrorFromVersion"),
            RemoveInVersion = attribute.GetValue("RemoveInVersion"),
            ThrowsNotImplemented = throwsNotImplemented
        };
    }
}