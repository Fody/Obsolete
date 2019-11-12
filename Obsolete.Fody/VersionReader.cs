using System;
using System.Linq;
using Mono.Cecil;

static class VersionReader
{
    public static SemanticVersion Read(AssemblyDefinition assembly)
    {
        var informationalAttribute = assembly.CustomAttributes
            .SingleOrDefault(x => x.AttributeType.FullName == "System.Reflection.AssemblyInformationalVersionAttribute");
        if (informationalAttribute != null)
        {
            var value = (string)informationalAttribute.ConstructorArguments.Single().Value;
            var indexOf = value.IndexOf(x => x != '.' && !char.IsNumber(x));
            if (indexOf != -1)
            {
                var substring = value.Substring(0, indexOf);
                if (SemanticVersion.TryParse(substring, out var versionFromInformational))
                {
                    return versionFromInformational;
                }
            }
        }

        var version = assembly.Name.Version;
        var semanticVersion = new SemanticVersion
        {
            Major = version.Major.OrZero(),
            Minor = version.Minor.OrZero(),
            Patch = version.Build.OrZero()
        };
        return semanticVersion;
    }

    static int IndexOf(this string source, Func<char, bool> predicate)
    {
        for (var index = 0; index < source.Length; index++)
        {
            var item = source[index];
            if (predicate.Invoke(item))
            {
                return index;
            }
        }

        return -1;
    }
}