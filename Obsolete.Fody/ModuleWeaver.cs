using System.Collections.Generic;
using Fody;

public partial class ModuleWeaver:BaseModuleWeaver
{
    public SemanticVersion assemblyVersion;

    public override void Execute()
    {
        ReadConfig();
        FindEditorBrowsableTypes();
        FindObsoleteType();

        var version = ModuleDefinition.Assembly.Name.Version;
        assemblyVersion = new SemanticVersion
        {
            Major = version.Major.OrZero(),
            Minor = version.Minor.OrZero(),
            Patch = version.Build.OrZero()
        };

        ProcessAssembly();
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System";
        yield return "System.Runtime";
        yield return "netstandard";
    }

    public override bool ShouldCleanReference => true;
}