using Fody;

public static class VersionExtensions
{
    public static SemanticVersion Decrement(this SemanticVersion target, StepType stepType)
    {
        switch (stepType)
        {
            case StepType.Major:
                if (target.Major == 0)
                {
                    throw new WeavingException($"Can not derive `TreatAsErrorFromVersion` from '{target}' since Major is 0.");
                }

                return new SemanticVersion
                {
                    Major = target.Major - 1,
                    Minor = 0,
                    Patch = 0
                };
            case StepType.Minor:
                if (target.Minor == 0)
                {
                    throw new WeavingException($"Can not derive `TreatAsErrorFromVersion` from '{target}' since Minor is 0.");
                }

                return new SemanticVersion
                {
                    Major = target.Major,
                    Minor = target.Minor - 1,
                    Patch = 0
                };
            case StepType.Patch:
                if (target.Patch == 0)
                {
                    throw new WeavingException($"Can not derive `TreatAsErrorFromVersion` from '{target}' since Patch is 0.");
                }

                return new SemanticVersion
                {
                    Major = target.Major,
                    Minor = target.Minor,
                    Patch = target.Patch - 1
                };
            default:
                throw new Exception("Unknown StepType: " + stepType);
        }
    }

    public static SemanticVersion Increment(this SemanticVersion target, StepType stepType)
    {
        switch (stepType)
        {
            case StepType.Major:
                return new SemanticVersion
                {
                    Major = target.Major + 1,
                    Minor = 0,
                    Patch = 0
                };
            case StepType.Minor:
                return new SemanticVersion
                {
                    Major = target.Major,
                    Minor = target.Minor + 1,
                    Patch = 0
                };
            case StepType.Patch:
                return new SemanticVersion
                {
                    Major = target.Major,
                    Minor = target.Minor,
                    Patch = target.Patch + 1
                };
            default:
                throw new Exception("Unknown StepType: " + stepType);
        }
    }

    public static SemanticVersion ToSemVer(this SemanticVersion target)
    {
        return new SemanticVersion
        {
            Major = target.Major,
            Minor = target.Minor,
            Patch = target.Patch
        };
    }

    public static int OrZero(this int target)
    {
        if (target == -1)
        {
            return 0;
        }

        return target;
    }
}