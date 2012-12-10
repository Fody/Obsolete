using System.Linq;

public partial class ModuleWeaver
{
    public string TreatAsErrorFormat = "Will be treated as an error from version '{0}'. ";
    public string RemoveInVersionFormat = "Will be removed in version '{0}'. ";
    public string ReplacementFormat = "Please use '{0}' instead. ";


    public void ReadConfig()
    {
        if (Config != null)
        {
            var treatAsErrorFormat = Config.Attributes("TreatAsErrorFormat").FirstOrDefault();
            if (treatAsErrorFormat != null)
            {
                TreatAsErrorFormat = treatAsErrorFormat.Value;
            }

            var removeInVersionFormat = Config.Attributes("RemoveInVersionFormat").FirstOrDefault();
            if (removeInVersionFormat != null)
            {
                RemoveInVersionFormat = removeInVersionFormat.Value;
            }
            var replacementFormat = Config.Attributes("ReplacementFormat").FirstOrDefault();
            if (replacementFormat != null)
            {
                ReplacementFormat = replacementFormat.Value;
            }
        }
    }
}