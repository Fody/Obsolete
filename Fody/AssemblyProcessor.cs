public class AssemblyProcessor
{
    AllTypesFinder typesFinder;
    AttributeFixer attributeFixer;

    public AssemblyProcessor(AllTypesFinder typesFinder, AttributeFixer attributeFixer)
    {
        this.typesFinder = typesFinder;
        this.attributeFixer = attributeFixer;
    }

    public void Execute()
    {
        foreach (var typeDefinition in typesFinder.AllTypes)
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