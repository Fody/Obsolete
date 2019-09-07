using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Fody;
using Xunit;
using Xunit.Abstractions;
using ICustomAttributeProvider = System.Reflection.ICustomAttributeProvider;

public class IntegrationTests :
    XunitApprovalBase
{
    static Assembly assembly;
    static TestResult testResult;

    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver
        {
            HideObsoleteMembers = true
        };
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
        assembly = testResult.Assembly;
    }

    [Fact]
    public void Class()
    {
        var type = assembly.GetType("ClassToMark");
        ValidateMessage(type);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassWithHigherAssumedRemoveInVersion()
    {
        var type = assembly.GetType("ClassToMarkWithHigherAssumedRemoveInVersion");
        var customAttributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)customAttributes.First();
        Assert.Equal("Will be treated as an error from version 3.0.0. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassToMarkWithSameRemoveAndTreatAsError()
    {
        var type = assembly.GetType("ClassToMarkWithSameRemoveAndTreatAsError");
        var customAttributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)customAttributes.First();
        Assert.Equal("Will be treated as an error from version 1.2.0. Will be removed in version 1.2.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassToMarkWithHigherAssumedTreatAsErrorFromVersion()
    {
        var type = assembly.GetType("ClassToMarkWithHigherAssumedTreatAsErrorFromVersion");
        var customAttributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)customAttributes.First();
        Assert.Equal("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassWithAssumedRemoveInVersion()
    {
        var type = assembly.GetType("ClassToMarkWithAssumedRemoveInVersion");
        var customAttributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)customAttributes.First();
        Assert.Equal("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassToMarkWithAssumedTreatAsErrorFromVersion()
    {
        var type = assembly.GetType("ClassToMarkWithAssumedTreatAsErrorFromVersion");
        var customAttributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)customAttributes.First();
        Assert.Equal("Will be removed in version 2.0.0.", obsoleteAttribute.Message);
        ValidateIsError(type);
    }

    [Fact]
    public void Warnings()
    {
        Assert.Contains("The member `ClassWithObsoleteAttribute` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.", testResult.Warnings.Select(x => x.Text));
    }

    [Fact]
    public void NoWarnings()
    {
        Assert.DoesNotContain("The member `ClassWithObsoleteAttributeToSkip` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.", testResult.Warnings.Select(x => x.Text));
    }

    [Fact]
    public void Errors()
    {
        Assert.Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: `System.Void ClassWithObsoleteOnGetSet::set_PropertyToMark(System.String)`.", testResult.Errors.Select(x => x.Text));
        Assert.Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: `System.String ClassWithObsoleteOnGetSet::get_PropertyToMark()`.", testResult.Errors.Select(x => x.Text));
    }

    [Fact]
    public void Interface()
    {
        var type = assembly.GetType("InterfaceToMark");
        ValidateMessage(type);
        ValidateHidden(type);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassWithIsError()
    {
        var type = assembly.GetType("ClassWithIsError");
        ValidateIsError(type);
    }

    [Fact]
    public void Enum()
    {
        var type = assembly.GetType("EnumToMark");
        ValidateIsNotError(type);
    }

    [Fact]
    public void Struct()
    {
        var type = assembly.GetType("StructToMark");
        ValidateIsNotError(type);
    }

    [Fact]
    public void EnumField()
    {
        var type = assembly.GetType("EnumToMark");
        var info = type.GetField("Foo");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassMethod()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassMethodThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodWithExceptionToMark");
        var obsoleteAttribute = ReadAttribute(info);
        Assert.Equal("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void InterfaceMethod()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void StructMethod()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassPropertySetThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyWithSetExceptionToMark");
        var obsoleteAttribute = ReadAttribute(info);
        Assert.Equal("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassPropertyGetThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyWithGetExceptionToMark");
        var obsoleteAttribute = ReadAttribute(info);
        Assert.Equal("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassProperty()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassField()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetField("FieldToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void InterfaceEvent()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMember("EventToMark").First();
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassEvent()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetEvent("EventToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void StructEvent()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMember("EventToMark").First();
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void InterfaceProperty()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
    public void StructProperty()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHidden(info);
        ValidateIsNotError(info);
    }

    [Fact]
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
        Assert.Equal("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
    }

    static ObsoleteAttribute ReadAttribute(ICustomAttributeProvider attributeProvider)
    {
        var customAttributes = attributeProvider.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        return (ObsoleteAttribute)customAttributes.First();
    }

    static void ValidateHidden(ICustomAttributeProvider attributeProvider)
    {
        var customAttributes = attributeProvider.GetCustomAttributes(typeof(EditorBrowsableAttribute), false);
        var attribute = (EditorBrowsableAttribute)customAttributes.First();
        Assert.Equal(EditorBrowsableState.Advanced, attribute.State);
    }

    static void ValidateIsError(ICustomAttributeProvider attributeProvider)
    {
        var obsoleteAttribute = ReadAttribute(attributeProvider);
        Assert.True(obsoleteAttribute.IsError);
    }

    static void ValidateIsNotError(ICustomAttributeProvider attributeProvider)
    {
        var obsoleteAttribute = ReadAttribute(attributeProvider);
        Assert.False(obsoleteAttribute.IsError);
    }

    public IntegrationTests(ITestOutputHelper output) : base(output)
    {
    }
}