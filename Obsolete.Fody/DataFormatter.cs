using System.Text;

public partial class ModuleWeaver
{
    public string ConvertToMessage(AttributeData attributeData)
    {
        var builder = new StringBuilder();
        var message = attributeData.Message;
        if (message != null)
        {
            message = message.Trim();
            message = message.Trim('.');
            builder.AppendFormat("{0}. ", message);
        }

        if (attributeData.Replacement != null)
        {
            builder.AppendFormat(ReplacementFormat, attributeData.Replacement);
        }

        if (assemblyVersion < attributeData.TreatAsErrorFromVersion)
        {
            builder.AppendFormat(TreatAsErrorFormat, attributeData.TreatAsErrorFromVersion.ToSemVer());
        }

        if (attributeData.ThrowsNotImplemented)
        {
            builder.Append(ThrowsNotImplementedText);
        }

        builder.AppendFormat(RemoveInVersionFormat, attributeData.RemoveInVersion.ToSemVer());

        return builder.ToString().Trim();
    }
}