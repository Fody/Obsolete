using System.Linq;
using System.Xml.Linq;

public class FormatterConfigReader
{
    XElement element;
    public string TreatAsErrorFormat = "Will be treated as an error from version '{0}'. ";
    public string RemoveInVersionFormat = "Will be removed in version '{0}'. ";
    public string ReplacementFormat = "Please use '{0}' instead. ";

    public FormatterConfigReader(XElement element)
    {
        this.element = element;
    }

    public void Execute()
    {
        if (element != null)
        {
            var treatAsErrorFormat = element.Attributes("TreatAsErrorFormat").FirstOrDefault();
            if (treatAsErrorFormat != null)
            {
                TreatAsErrorFormat = treatAsErrorFormat.Value;
            }

            var removeInVersionFormat = element.Attributes("RemoveInVersionFormat").FirstOrDefault();
            if (removeInVersionFormat != null)
            {
                RemoveInVersionFormat = removeInVersionFormat.Value;
            }
            var replacementFormat = element.Attributes("ReplacementFormat").FirstOrDefault();
            if (replacementFormat != null)
            {
                ReplacementFormat = replacementFormat.Value;
            }
        }
    }
}