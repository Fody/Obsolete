using System;
using System.Xml.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public Version assemblyVersion;
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
        FindSystemTypes();
        assemblyVersion = ModuleDefinition.Assembly.Name.Version;
        FindObsoleteType();

        ReadConfig();

        ProcessAssembly();

        CleanReferences();
    }
}