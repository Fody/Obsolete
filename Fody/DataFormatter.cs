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
        if (attributeData.TreatAsErrorFromVersion != null)
        {
            if (assemblyVersion < attributeData.TreatAsErrorFromVersion)
            {
                stringBuilder.AppendFormat(TreatAsErrorFormat, attributeData.TreatAsErrorFromVersion);
            }
        }
        if (attributeData.RemoveInVersion != null)
        {
            stringBuilder.AppendFormat(RemoveInVersionFormat, attributeData.RemoveInVersion);
        }

        return stringBuilder.ToString().Trim();
    }
}