using System;
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
                       Replacement = obsoleteExAttribute.GetValue("Replacement"),
                       TreatAsErrorFromVersion = ConvertToVersion(treatAsErrorFromVersionString),
                       RemoveInVersion = ConvertToVersion(removeInVersionString),
                   };
    }

   public  static Version ConvertToVersion(string versionString)
    {
        if (versionString != null)
        {
            Version version;
            if (!versionString.Contains("."))
            {
                if (char.IsDigit(versionString, 0))
                {
                    versionString += ".0";
                }
            }
            if (!(Version.TryParse(versionString, out version)))
            {
                throw new WeavingException(string.Format("Could not convert '{0}' to a Version.", versionString));
            }
            if (version.Revision != -1)
            {
                throw new WeavingException(string.Format("Could not convert '{0}' to a Version. Obsolete Fody does not support Revision numbers.", versionString));
            }
            return version;
        }
        return null;
    }
}