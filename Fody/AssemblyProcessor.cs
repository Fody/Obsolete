public partial class ModuleWeaver
{

    public void ProcessAssembly()
    {
        foreach (var typeDefinition in ModuleDefinition.GetTypes())
        {
            ProcessAttributes(typeDefinition);
            foreach (var property in typeDefinition.Properties)
            {
                ProcessAttributes(property);
            }
            foreach (var method in typeDefinition.Methods)
            {
                ProcessAttributes(method);
            }
            foreach (var field in typeDefinition.Fields)
            {
                ProcessAttributes(field);
            }
            foreach (var @event in typeDefinition.Events)
            {
                ProcessAttributes(@event);
            }
        }
    }

}