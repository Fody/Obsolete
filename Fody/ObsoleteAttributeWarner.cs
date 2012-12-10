using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    

    public void CheckForNormalAttribute(IMemberDefinition memberDefinition)
    {
        var obsoleteExAttribute = memberDefinition
            .CustomAttributes
            .FirstOrDefault(x => x.AttributeType.Name == "ObsoleteAttribute");
        if (obsoleteExAttribute == null)
        {
            return;
        }
        var warning = string.Format("The member '{0}' has an ObsoleteAttribute. You should consider replacing it with an ObsoleteExAttribute.", memberDefinition.FullName);
        LogWarning(warning);

    }
}