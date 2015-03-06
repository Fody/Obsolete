using Mono.Cecil;

public static class DataReader
{

    public static AttributeData ReadAttributeData(CustomAttribute obsoleteExAttribute)
    {
        var treatAsErrorFromVersionString = obsoleteExAttribute.GetValue("TreatAsErrorFromVersion");
        var removeInVersionString = obsoleteExAttribute.GetValue("RemoveInVersion");

        return new AttributeData
                   {
                       Message = obsoleteExAttribute.GetValue("Message"),
                       Replacement = obsoleteExAttribute.GetValue("ReplacementTypeOrMember"),
                       TreatAsErrorFromVersion = treatAsErrorFromVersionString,
                       RemoveInVersion = removeInVersionString,
                   };
    }

}