using System;
#pragma warning disable 660,661

public class SemanticVersion
{
    public int Major;
    public int Minor;
    public int Patch;

    public bool Equals(SemanticVersion obj)
    {
        if (obj == null)
        {
            return false;
        }
        return Major == obj.Major &&
               Minor == obj.Minor &&
               Patch == obj.Patch;
    }

    public static implicit operator string(SemanticVersion d)
    {
        return $"{d.Major}.{d.Minor}.{d.Patch}";
    }

    public static implicit operator SemanticVersion(string d)
    {
        if (TryParse(d, out var semver))
        {
            return semver;
        }
        throw new Exception("Could not parse " + d);
    }

    public override string ToString()
    {
        return this;
    }

    public static bool TryParse(string versionString, out SemanticVersion semanticVersion)
    {
        if (versionString == null)
        {
            semanticVersion = null;
            return true;
        }
        var stableParts = versionString.Split('.');

        if (stableParts.Length > 3)
        {
            semanticVersion = null;
            return false;
        }

        if (!int.TryParse(stableParts[0], out var major))
        {
            semanticVersion = null;
            return false;
        }
        var parsedVersion = new SemanticVersion
        {
            Major = major
        };
        if (stableParts.Length > 1)
        {
            if (!int.TryParse(stableParts[1], out var minor))
            {
                semanticVersion = null;
                return false;
            }
            parsedVersion.Minor = minor;
        }

        if (stableParts.Length > 2)
        {
            if (!int.TryParse(stableParts[2], out var patch))
            {
                semanticVersion = null;
                return false;
            }
            parsedVersion.Patch = patch;
        }

        semanticVersion = parsedVersion;
        return true;
    }

    public static bool operator ==(SemanticVersion v1, SemanticVersion v2)
    {
        if (ReferenceEquals(v1, null))
        {
            return ReferenceEquals(v2, null);
        }
        return v1.Equals(v2);
    }

    public static bool operator !=(SemanticVersion v1, SemanticVersion v2)
    {
        return !(v1 == v2);
    }

    public static bool operator >(SemanticVersion v1, SemanticVersion v2)
    {
        return v2 < v1;
    }

    public static bool operator >=(SemanticVersion v1, SemanticVersion v2)
    {
        return v2 <= v1;
    }

    public static bool operator <=(SemanticVersion v1, SemanticVersion v2)
    {
        if (v1 == null)
        {
            throw new ArgumentNullException("v1");
        }
        return v1.CompareTo(v2) <= 0;
    }

    public static bool operator <(SemanticVersion v1, SemanticVersion v2)
    {
        if (v1 == null)
        {
            throw new ArgumentNullException("v1");
        }
        return v1.CompareTo(v2) < 0;
    }

    public int CompareTo(SemanticVersion value)
    {
        if (value == null)
        {
            return 1;
        }
        if (Major != value.Major)
        {
            if (Major > value.Major)
            {
                return 1;
            }
            return -1;
        }
        if (Minor != value.Minor)
        {
            if (Minor > value.Minor)
            {
                return 1;
            }
            return -1;
        }
        if (Patch != value.Patch)
        {
            if (Patch > value.Patch)
            {
                return 1;
            }
            return -1;
        }
        return 0;
    }
}