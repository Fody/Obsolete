using System;
using System.Text;

public class DataFormatter
{

    Version assemblyVersion;
    FormatterConfigReader formatterConfigReader;

    public DataFormatter(Version assemblyVersion, FormatterConfigReader formatterConfigReader)
    {
        this.assemblyVersion = assemblyVersion;
        this.formatterConfigReader = formatterConfigReader;
    }

    public string ConvertToMessage(AttributeData attributeData)
    {
        var stringBuilder = new StringBuilder();
        if (attributeData.Message != null)
        {
            stringBuilder.AppendFormat("{0} ", attributeData.Message);
        }
        if (attributeData.Replacement != null)
        {
            stringBuilder.AppendFormat(formatterConfigReader.ReplacementFormat, attributeData.Replacement);
        }
        if (attributeData.TreatAsErrorFromVersion != null)
        {
            if (assemblyVersion < attributeData.TreatAsErrorFromVersion)
            {
                stringBuilder.AppendFormat(formatterConfigReader.TreatAsErrorFormat, attributeData.TreatAsErrorFromVersion);
            }
        }
        if (attributeData.RemoveInVersion != null)
        {
            stringBuilder.AppendFormat(formatterConfigReader.RemoveInVersionFormat, attributeData.RemoveInVersion);
        }

        return stringBuilder.ToString().Trim();
    }
}