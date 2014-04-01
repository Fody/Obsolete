
using System;

public static class VersionExtensions
{

    public static SemanticVersion Decrement(this SemanticVersion target, StepType stepType)
    {
        switch (stepType)
        {
            case StepType.Major:
                if (target.Major == 0)
                {
                    throw new WeavingException(string.Format("Can not derive `TreatAsErrorFromVersion` from '{0}' since Major is 0.", target));
                }
                return new SemanticVersion
                    {
                        Major = target.Major -1,
                        Minor = 0,
                        Patch = 0
                    };
            case StepType.Minor:
                if (target.Minor == 0)
                {
                    throw new WeavingException(string.Format("Can not derive `TreatAsErrorFromVersion` from '{0}' since Minor is 0.", target));
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
                    throw new WeavingException(string.Format("Can not derive `TreatAsErrorFromVersion` from '{0}' since Patch is 0.", target));
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
        var major = target.Major;
        var minor = target.Minor;
        var patch = target.Patch;
        return new SemanticVersion{Major = major,Minor = minor, Patch = patch};
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