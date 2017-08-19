using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;
using ICustomAttributeProvider = System.Reflection.ICustomAttributeProvider;


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
        beforeAssemblyPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll"));
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        using (var moduleDefinition = ModuleDefinition.ReadModule(beforeAssemblyPath))
        {
            var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = moduleDefinition,
                AssemblyResolver = new DefaultAssemblyResolver(),
                LogWarning = s => warnings.Add(s),
                LogError = s => errors.Add(s),
                HideObsoleteMembers = true
            };

            weavingTask.Execute();
            moduleDefinition.Write(afterAssemblyPath);
        }

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
    public void ClassWithHigherAssumedRemoveInVersion()
    {
        var type = assembly.GetType("ClassToMarkWithHigherAssumedRemoveInVersion");
        var customAttributes = ((ICustomAttributeProvider) type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute) customAttributes.First();
        Assert.AreEqual("Will be treated as an error from version 3.0.0. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Test]
    public void ClassToMarkWithSameRemoveAndTreatAsError()
    {
        var type = assembly.GetType("ClassToMarkWithSameRemoveAndTreatAsError");
        var customAttributes = ((ICustomAttributeProvider) type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute) customAttributes.First();
        Assert.AreEqual("Will be treated as an error from version 1.2.0. Will be removed in version 1.2.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Test]
    public void ClassToMarkWithHigherAssumedTreatAsErrorFromVersion()
    {
        var type = assembly.GetType("ClassToMarkWithHigherAssumedTreatAsErrorFromVersion");
        var customAttributes = ((ICustomAttributeProvider) type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute) customAttributes.First();
        Assert.AreEqual("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Test]
    public void ClassWithAssumedRemoveInVersion()
    {
        var type = assembly.GetType("ClassToMarkWithAssumedRemoveInVersion");
        var customAttributes = ((ICustomAttributeProvider) type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute) customAttributes.First();
        Assert.AreEqual("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Test]
    public void ClassToMarkWithAssumedTreatAsErrorFromVersion()
    {
        var type = assembly.GetType("ClassToMarkWithAssumedTreatAsErrorFromVersion");
        var customAttributes = ((ICustomAttributeProvider) type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute) customAttributes.First();
        Assert.AreEqual("Will be removed in version 2.0.0.", obsoleteAttribute.Message);
        ValidateIsError(type);
    }

    [Test]
    public void Warnings()
    {
        Assert.Contains("The member `ClassWithObsoleteAttribute` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.", warnings);
    }

    [Test]
    public void Errors()
    {
        Assert.Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: `System.Void ClassWithObsoleteOnGetSet::set_PropertyToMark(System.String)`.", errors);
        Assert.Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: `System.String ClassWithObsoleteOnGetSet::get_PropertyToMark()`.", errors);
    }

    [Test]
    public void Interface()
    {
        var type = assembly.GetType("InterfaceToMark");
        ValidateMessage(type);
        ValidateHidden(type);
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
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void ClassMethod()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void ClassMethodThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodWithExceptionToMark");
        var obsoleteAttribute = ReadAttribute(info);
        Assert.AreEqual("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void InterfaceMethod()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void StructMethod()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void ClassPropertySetThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyWithSetExceptionToMark");
        var obsoleteAttribute = ReadAttribute(info);
        Assert.AreEqual("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void ClassPropertyGetThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyWithGetExceptionToMark");
        var obsoleteAttribute = ReadAttribute(info);
        Assert.AreEqual("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void ClassProperty()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void ClassField()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetField("FieldToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void InterfaceEvent()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMember("EventToMark").First();
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void ClassEvent()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetEvent("EventToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void StructEvent()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMember("EventToMark").First();
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void InterfaceProperty()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void StructProperty()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Test]
    public void StructField()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetField("FieldToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    static void ValidateMessage(ICustomAttributeProvider attributeProvider)
    {
        var obsoleteAttribute = ReadAttribute(attributeProvider);
        Assert.AreEqual("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
    }

    static ObsoleteAttribute ReadAttribute(ICustomAttributeProvider attributeProvider)
    {
        var customAttributes = attributeProvider.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        return (ObsoleteAttribute) customAttributes.First();
    }

    static void ValidateHidden(ICustomAttributeProvider attributeProvider)
    {
        var customAttributes = attributeProvider.GetCustomAttributes(typeof(EditorBrowsableAttribute), false);
        var attribute = (EditorBrowsableAttribute) customAttributes.First();
        Assert.AreEqual(EditorBrowsableState.Advanced, attribute.State);
    }

    static void ValidateIsError(ICustomAttributeProvider attributeProvider)
    {
        var obsoleteAttribute = ReadAttribute(attributeProvider);
        Assert.IsTrue(obsoleteAttribute.IsError);
    }

    static void ValidateIsNotError(ICustomAttributeProvider attributeProvider)
    {
        var obsoleteAttribute = ReadAttribute(attributeProvider);
        Assert.IsFalse(obsoleteAttribute.IsError);
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath, afterAssemblyPath);
    }
#endif

}