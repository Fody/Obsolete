using Mono.Cecil;

public class AssemblyProcessor
{
    ModuleDefinition moduleDefinition;
    AttributeFixer attributeFixer;

    public AssemblyProcessor(ModuleDefinition moduleDefinition, AttributeFixer attributeFixer)
    {
        this.moduleDefinition = moduleDefinition;
        this.attributeFixer = attributeFixer;
    }

    public void Execute()
    {
        foreach (var typeDefinition in moduleDefinition.GetTypes())
        {
            attributeFixer.ProcessAttributes(typeDefinition);
            foreach (var property in typeDefinition.Properties)
            {
                attributeFixer.ProcessAttributes(property);
            }
            foreach (var method in typeDefinition.Methods)
            {
                attributeFixer.ProcessAttributes(method);
            }
            foreach (var field in typeDefinition.Fields)
            {
                attributeFixer.ProcessAttributes(field);
            }
            foreach (var @event in typeDefinition.Events)
            {
                attributeFixer.ProcessAttributes(@event);
            }
        }
    }

}