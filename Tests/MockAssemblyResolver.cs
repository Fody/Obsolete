using System;
using System.Reflection;
using Mono.Cecil;

public class MockAssemblyResolver : IAssemblyResolver
{
    public AssemblyDefinition Resolve(AssemblyNameReference name)
    {
        return Resolve(name.FullName);
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    {
        return Resolve(name.FullName);
    }

    public AssemblyDefinition Resolve(string fullName)
    {
        var codeBase = Assembly.Load(fullName).CodeBase.Replace("file:///","");

        return AssemblyDefinition.ReadAssembly(codeBase);
    }

    public void Dispose()
    {
    }
}