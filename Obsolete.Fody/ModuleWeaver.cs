using System;
using System.Collections.Generic;
using System.Linq;
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
        var systemTypes = FindSystemTypes();
        FindEditorBrowsableTypes(systemTypes);
        FindObsoleteType(systemTypes);

        var version = ModuleDefinition.Assembly.Name.Version;
        assemblyVersion = new SemanticVersion
        {
            Major = version.Major.OrZero(),
            Minor = version.Minor.OrZero(),
            Patch = version.Build.OrZero()
        };

        ProcessAssembly();

        CleanReferences();
    }

    public List<TypeDefinition> FindSystemTypes()
    {
        var types = new List<TypeDefinition>();

        AddAssemblyIfExists("mscorlib", types);
        AddAssemblyIfExists("System", types);
        AddAssemblyIfExists("System.Runtime", types);
        AddAssemblyIfExists("netstandard", types);

        return types;
    }

    void AddAssemblyIfExists(string name, List<TypeDefinition> types)
    {
        try
        {
            var msCoreLibDefinition = AssemblyResolver.Resolve(new AssemblyNameReference(name, null));

            if (msCoreLibDefinition != null)
            {
                var module = msCoreLibDefinition.MainModule;
                types.AddRange(module.Types);
                types.AddRange(module.ExportedTypes.Select(x => x.Resolve()).Where(x => x != null));
            }
        }
        catch (AssemblyResolutionException)
        {
            LogInfo($"Failed to resolve '{name}'. So skipping its types.");
        }
    }
}