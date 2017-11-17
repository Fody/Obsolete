using System.Linq;
using Mono.Cecil;

public static class AttributeExtensions
{
    public static string GetValue(this CustomAttribute obsoleteExAttribute, string propertyName)
    {
        var argument = obsoleteExAttribute.Properties.FirstOrDefault(x => x.Name == propertyName);
        return (string) argument.Argument.Value;
    }
}