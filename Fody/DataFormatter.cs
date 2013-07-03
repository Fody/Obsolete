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


        if (assemblyVersion < attributeData.TreatAsErrorFromVersion)
        {
            stringBuilder.AppendFormat(TreatAsErrorFormat, attributeData.TreatAsErrorFromVersion.ToSemVer());
        }
        stringBuilder.AppendFormat(RemoveInVersionFormat, attributeData.RemoveInVersion.ToSemVer());

        return stringBuilder.ToString().Trim();
    }
}