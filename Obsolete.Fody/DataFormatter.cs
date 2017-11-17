using System.Text;

public partial class ModuleWeaver
{
    public string ConvertToMessage(AttributeData attributeData)
    {
        var stringBuilder = new StringBuilder();
        var message = attributeData.Message;
        if (message != null)
        {
            message = message.Trim();
            message = message.Trim('.');
            stringBuilder.AppendFormat("{0}. ", message);
        }

        if (attributeData.Replacement != null)
        {
            stringBuilder.AppendFormat(ReplacementFormat, attributeData.Replacement);
        }

        if (assemblyVersion < attributeData.TreatAsErrorFromVersion)
        {
            stringBuilder.AppendFormat(TreatAsErrorFormat, attributeData.TreatAsErrorFromVersion.ToSemVer());
        }
        if (attributeData.ThrowsNotImplemented)
        {
            stringBuilder.Append(ThrowsNotImplementedText);
        }
        stringBuilder.AppendFormat(RemoveInVersionFormat, attributeData.RemoveInVersion.ToSemVer());

        return stringBuilder.ToString().Trim();
    }
}