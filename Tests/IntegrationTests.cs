using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;


[TestFixture]
public class IntegrationTests
{
    Assembly assembly;
    List<string> warnings = new List<string>();
    List<string> errors = new List<string>();
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public IntegrationTests()
    {
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)

        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath);
        var weavingTask = new ModuleWeaver
                              {
                                  ModuleDefinition = moduleDefinition,
                                  AssemblyResolver = new MockAssemblyResolver(),
                                  LogWarning =s => warnings.Add(s),
                                  LogError =s => errors.Add(s)
                              };

        weavingTask.Execute();
        moduleDefinition.Write(afterAssemblyPath);

        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void Class()
    {
        var type = assembly.GetType("ClassToMark");
        ValidateMessage(type);
        ValidateIsNotError(type);
    }
    [Test]
    public void Warnings()
    {
        Assert.Contains("The member 'ClassWithObsoleteAttribute' has an ObsoleteAttribute. You should consider replacing it with an ObsoleteExAttribute.", warnings);
    }
    [Test]
    public void Errors()
    {
        Assert.Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: 'System.Void ClassWithObsoleteOnGetSet::set_PropertyToMark(System.String)'.", errors);
        Assert.Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: 'System.String ClassWithObsoleteOnGetSet::get_PropertyToMark()'.", errors);
    }
    [Test]
    public void Interface()
    {
        var type = assembly.GetType("InterfaceToMark");
        ValidateMessage(type);
        ValidateIsNotError(type);
    }

    [Test]
    public void ClassWithIsError()
    {
        var type = assembly.GetType("ClassWithIsError");
        ValidateIsError(type);
    }

    [Test]
    public void Enum()
    {
        var type = assembly.GetType("EnumToMark");
        ValidateIsNotError(type);
    }
    [Test]
    public void Struct()
    {
        var type = assembly.GetType("StructToMark");
        ValidateIsNotError(type);
    }
    
    [Test]
    public void EnumField()
    {
        var type = assembly.GetType("EnumToMark");
        var info = type.GetField("Foo");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void ClassMethod()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void InterfaceMethod()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void StructMethod()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    
    [Test]
    public void ClassProperty()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void ClassField()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetField("FieldToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void InterfaceEvent()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMember("EventToMark").First();
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void ClassEvent()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetEvent("EventToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void StructEvent()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMember("EventToMark").First();
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void InterfaceProperty()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void StructProperty()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }
    [Test]
    public void StructField()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetField("FieldToMark");
        ValidateMessage(info);
        ValidateIsNotError(info);
    }

    static void ValidateMessage(System.Reflection.ICustomAttributeProvider attributeProvider)
    {
        var customAttributes = attributeProvider.GetCustomAttributes(typeof (ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute) customAttributes.First();
        Assert.AreEqual("Custom message. Please use 'NewThing' instead. Will be treated as an error from version '2.0'. Will be removed in version '3.0'.", obsoleteAttribute.Message);
    }
    
    static void ValidateIsError(System.Reflection.ICustomAttributeProvider attributeProvider)
    {
        var customAttributes = attributeProvider.GetCustomAttributes(typeof (ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute) customAttributes.First();
        Assert.IsTrue(obsoleteAttribute.IsError);
    }

    static void ValidateIsNotError(System.Reflection.ICustomAttributeProvider attributeProvider)
    {
        var customAttributes = attributeProvider.GetCustomAttributes(typeof (ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute) customAttributes.First();
        Assert.IsFalse(obsoleteAttribute.IsError);
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
#endif

}