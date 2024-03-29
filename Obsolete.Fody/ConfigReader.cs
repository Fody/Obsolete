using Fody;

public partial class ModuleWeaver
{
    public string TreatAsErrorFormat = "Will be treated as an error from version {0}. ";
    public StepType StepType = StepType.Major;
    public string RemoveInVersionFormat = "Will be removed in version {0}.";
    public string ThrowsNotImplementedText = "The member currently throws a NotImplementedException. ";
    public string ReplacementFormat = "Use `{0}` instead. ";

    public HideObsoleteMembersState HideObsoleteMembers = HideObsoleteMembersState.Advanced;

    public void ReadConfig()
    {
        ReadFormats();
        ReadHideObsoleteMembers();
        ReadVersionIncrement();
        ReadStepType();
    }

    void ReadFormats()
    {
        var treatAsErrorFormat = Config.Attributes("TreatAsErrorFormat").FirstOrDefault();
        if (treatAsErrorFormat != null)
        {
            TreatAsErrorFormat = treatAsErrorFormat.Value;
        }

        var throwsNotImplementedText = Config.Attributes("ThrowsNotImplementedText").FirstOrDefault();
        if (throwsNotImplementedText != null)
        {
            ThrowsNotImplementedText = throwsNotImplementedText.Value;
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

    void ReadHideObsoleteMembers()
    {
        var xAttribute = Config.Attribute("HideObsoleteMembers");
        if (xAttribute == null)
        {
            return;
        }

        if (Enum.TryParse<HideObsoleteMembersState>(xAttribute.Value, true, out var state))
        {
            HideObsoleteMembers = state;
            return;
        }

        throw new Exception($"Could not parse 'HideObsoleteMembers' from '{xAttribute.Value}'.");
    }

    void ReadVersionIncrement()
    {
        var xAttribute = Config.Attribute("VersionIncrement");
        if (xAttribute == null)
        {
            return;
        }

        throw new WeavingException("VersionIncrement is no longer supported. Use StepType instead.");
    }

    void ReadStepType()
    {
        var xAttribute = Config.Attribute("StepType");
        if (xAttribute == null)
        {
            return;
        }

        if (Enum.TryParse(xAttribute.Value, out StepType))
        {
            return;
        }

        throw new Exception($"Could not parse 'StepType' from '{xAttribute.Value}'.");
    }
}