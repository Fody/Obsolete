using System;

public static class VersionExtensions
{

    public static Version Add(this Version target, Version toAdd)
    {
        var major = target.Major.OrZero() + toAdd.Major.OrZero();
        var minor = target.Minor.OrZero() + toAdd.Minor.OrZero();
        var build = target.Build.OrZero() + toAdd.Build.OrZero();
        return new Version(major, minor, build);
    }

    public static Version ToSemVer(this Version target)
    {
        var major = target.Major.OrZero();
        var minor = target.Minor.OrZero();
        var build = target.Build.OrZero() ;
        return new Version(major, minor, build);
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