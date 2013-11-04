using System;
using System.Xml.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public SemanticVersion assemblyVersion;
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string> LogError { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public XElement Config { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
        LogError = s => { };
    }

    public void Execute()
    {
        ReadConfig();
        FindSystemTypes();
        var version = ModuleDefinition.Assembly.Name.Version;
        assemblyVersion = new SemanticVersion
        {
            Major = version.Major.OrZero(),
            Minor = version.Minor.OrZero(),
            Patch = version.Build.OrZero()
        };
        FindObsoleteType();


        ProcessAssembly();

        CleanReferences();
    }
}