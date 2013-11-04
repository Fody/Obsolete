
public static class VersionExtensions
{

    public static SemanticVersion Subtract(this SemanticVersion target, SemanticVersion toSubtract)
    {
        var major = target.Major - toSubtract.Major;
        var minor = target.Minor - toSubtract.Minor;
        var patch = target.Patch - toSubtract.Patch;
        return new SemanticVersion { Major = major, Minor = minor, Patch = patch };
    }
    public static SemanticVersion Add(this SemanticVersion target, SemanticVersion toAdd)
    {
        var major = target.Major + toAdd.Major;
        var minor = target.Minor + toAdd.Minor;
        var patch = target.Patch + toAdd.Patch;
        return new SemanticVersion { Major = major, Minor = minor, Patch = patch };
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