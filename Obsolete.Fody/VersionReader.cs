﻿using Mono.Cecil;

public static class VersionReader
{
    public static SemanticVersion Read(AssemblyDefinition assembly)
    {
        var informationalAttribute = assembly.CustomAttributes
            .SingleOrDefault(_ => _.AttributeType.FullName == "System.Reflection.AssemblyInformationalVersionAttribute");
        if (informationalAttribute != null)
        {
            var value = (string)informationalAttribute.ConstructorArguments.Single().Value;
            var indexOf = value.IndexOf(_ => _ != '.' &&
                                             !char.IsNumber(_));
            if (indexOf == -1)
            {
                if (SemanticVersion.TryParse(value, out var informationalVersion))
                {
                    return informationalVersion;
                }
            }
            else
            {
                var substring = value.Substring(0, indexOf);
                if (SemanticVersion.TryParse(substring, out var informationalVersion))
                {
                    return informationalVersion;
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