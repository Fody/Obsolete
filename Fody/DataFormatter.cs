using System.Text;

public partial class ModuleWeaver
{

    public string ConvertToMessage(AttributeData attributeData)
    {
        var stringBuilder = new StringBuilder();
        if (attributeData.Message != null)
        {
            stringBuilder.AppendFormat("{0} ", attributeData.Message);
        }
        if (attributeData.Replacement != null)
        {
            stringBuilder.AppendFormat(ReplacementFormat, attributeData.Replacement);
        }
        var treatAsErrorFrom = attributeData.TreatAsErrorFromVersion;
        if (treatAsErrorFrom != null)
        {
            if (assemblyVersion < treatAsErrorFrom)
            {
                stringBuilder.AppendFormat(TreatAsErrorFormat, treatAsErrorFrom.ToSemVer());
            }
        }
        if (attributeData.RemoveInVersion == null)
        {
            if (treatAsErrorFrom != null)
            {
                stringBuilder.AppendFormat(RemoveInVersionFormat, treatAsErrorFrom.Add(VersionIncrement));
            }
        }
        else
        {
            stringBuilder.AppendFormat(RemoveInVersionFormat, attributeData.RemoveInVersion.ToSemVer());
        }

        return stringBuilder.ToString().Trim();
    }
}