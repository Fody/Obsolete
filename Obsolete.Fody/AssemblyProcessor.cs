public partial class ModuleWeaver
{
    public void ProcessAssembly()
    {
        foreach (var type in ModuleDefinition.GetTypes())
        {
            ProcessAttributes(type);
            foreach (var property in type.Properties)
            {
                ProcessAttributes(property);
            }
            foreach (var method in type.Methods)
            {
                ProcessAttributes(method);
            }
            foreach (var field in type.Fields)
            {
                ProcessAttributes(field);
            }
            foreach (var @event in type.Events)
            {
                ProcessAttributes(@event);
            }
        }
    }
}