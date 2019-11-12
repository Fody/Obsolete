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

        assemblyVersion = VersionReader.Read(ModuleDefinition.Assembly);

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