using System;
using System.Xml.Linq;
using Mono.Cecil;

public class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public XElement Config { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        var obsoleteTypeFinder = new ObsoleteTypeFinder(ModuleDefinition, AssemblyResolver);
        obsoleteTypeFinder.Execute();

        var obsoleteAttributeWarner = new ObsoleteAttributeWarner(LogWarning);
        var formatterConfigReader = new FormatterConfigReader(Config);
        formatterConfigReader.Execute();
        var attributeDataFormatter = new DataFormatter(ModuleDefinition.Assembly.Name.Version, formatterConfigReader);

        var attributeFixer = new AttributeFixer(obsoleteTypeFinder, ModuleDefinition, attributeDataFormatter, obsoleteAttributeWarner);
        var assemblyProcessor = new AssemblyProcessor(ModuleDefinition, attributeFixer);
        assemblyProcessor.Execute();

        new ReferenceCleaner(ModuleDefinition, LogInfo).Execute();
    }
}