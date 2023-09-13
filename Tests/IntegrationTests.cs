using System.ComponentModel;
using System.Reflection;
using Fody;
using ICustomAttributeProvider = System.Reflection.ICustomAttributeProvider;

public class IntegrationTestsDefaultHiding : IntegrationTestsBase
{
    public IntegrationTestsDefaultHiding(IntegrationTestFixture fixture) : base(fixture, ModuleWeaver.HideObsoleteMembersState.Advanced)
    {
    }
}

public class IntegrationTestsNeverHiding : IntegrationTestsBase
{
    public IntegrationTestsNeverHiding(IntegrationTestFixture fixture) : base(fixture, ModuleWeaver.HideObsoleteMembersState.Never)
    {
    }
}

public class IntegrationTestsHidingDisabled :
    IntegrationTestsBase
{
    public IntegrationTestsHidingDisabled(IntegrationTestFixture fixture) :
        base(fixture, ModuleWeaver.HideObsoleteMembersState.Off)
    {
    }
}

public class IntegrationTestFixture :
    IDisposable
{
    public void Initialize(ModuleWeaver.HideObsoleteMembersState state)
    {
        if (TestResult != null)
        {
            return;
        }

        var weavingTask = new ModuleWeaver
        {
            HideObsoleteMembers = state
        };
        TestResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
        Assembly = TestResult.Assembly;
    }

    public TestResult TestResult { get; set; }
    public Assembly Assembly { get; set; }

    public void Dispose()
    {
    }
}

[Collection("IntegrationTestsBase")]
public abstract class IntegrationTestsBase :
    IClassFixture<IntegrationTestFixture>
{
    Assembly assembly;
    TestResult testResult;
    ModuleWeaver.HideObsoleteMembersState expectedState;

    protected IntegrationTestsBase(IntegrationTestFixture fixture, ModuleWeaver.HideObsoleteMembersState state)
    {
        fixture.Initialize(state);
        assembly = fixture.Assembly;
        testResult = fixture.TestResult;
        expectedState = state;
    }

    [Fact]
    public void Class()
    {
        var type = assembly.GetType("ClassToMark");
        ValidateMessage(type);
        ValidateHiddenState(type, expectedState);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassWithHigherAssumedRemoveInVersion()
    {
        var type = assembly.GetType("ClassToMarkWithHigherAssumedRemoveInVersion");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)attributes.First();
        Assert.Equal("Will be treated as an error from version 3.0.0. Will be removed in version 4.0.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassToMarkWithSameRemoveAndTreatAsError()
    {
        var type = assembly.GetType("ClassToMarkWithSameRemoveAndTreatAsError");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)attributes.First();
        Assert.Equal("Will be treated as an error from version 1.2.0. Will be removed in version 1.2.0.", obsoleteAttribute.Message);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassToMarkWithHigherAssumedTreatAsErrorFromVersion()
    {
        var type = assembly.GetType("ClassToMarkWithHigherAssumedTreatAsErrorFromVersion");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var attribute = (ObsoleteAttribute)attributes.First();
        Assert.Equal("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", attribute.Message);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassWithAssumedRemoveInVersion()
    {
        var type = assembly.GetType("ClassToMarkWithAssumedRemoveInVersion");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var attribute = (ObsoleteAttribute)attributes.First();
        Assert.Equal("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", attribute.Message);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassToMarkWithAssumedTreatAsErrorFromVersion()
    {
        var type = assembly.GetType("ClassToMarkWithAssumedTreatAsErrorFromVersion");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var attribute = (ObsoleteAttribute)attributes.First();
        Assert.Equal("Will be removed in version 2.0.0.", attribute.Message);
        ValidateIsError(type);
    }

    [Fact]
    public void Warnings()
    {
        Assert.Contains("The member `ClassWithObsoleteAttribute` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.", testResult.Warnings.Select(_ => _.Text));
    }

    [Fact]
    public void NoWarnings()
    {
        Assert.DoesNotContain("The member `ClassWithObsoleteAttributeToSkip` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.", testResult.Warnings.Select(_ => _.Text));
    }

    [Fact]
    public void Errors()
    {
        Assert.Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: `System.Void ClassWithObsoleteOnGetSet::set_PropertyToMark(System.String)`.", testResult.Errors.Select(_ => _.Text));
        Assert.Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: `System.String ClassWithObsoleteOnGetSet::get_PropertyToMark()`.", testResult.Errors.Select(_ => _.Text));
    }

    [Fact]
    public void Interface()
    {
        var type = assembly.GetType("InterfaceToMark");
        ValidateMessage(type);
        ValidateHiddenState(type, expectedState);
        ValidateIsNotError(type);
    }

    [Fact]
    public void ClassWithIsError()
    {
        var type = assembly.GetType("ClassWithIsError");
        ValidateIsError(type);
    }

    [Fact]
    public void ClassWithIsErrorFromInformationalVersion()
    {
        var type = assembly.GetType("ClassWithIsErrorFromInformationalVersion");
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
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassMethod()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassMethodThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodWithExceptionToMark");
        var attribute = ReadAttribute(info);
        Assert.Equal("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", attribute.Message);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void InterfaceMethod()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void StructMethod()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMethod("MethodToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassPropertySetThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyWithSetExceptionToMark");
        var attribute = ReadAttribute(info);
        Assert.Equal("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", attribute.Message);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassPropertyGetThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyWithGetExceptionToMark");
        var attribute = ReadAttribute(info);
        Assert.Equal("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.", attribute.Message);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassProperty()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassField()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetField("FieldToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void InterfaceEvent()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMember("EventToMark").First();
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void ClassEvent()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetEvent("EventToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void StructEvent()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMember("EventToMark").First();
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void InterfaceProperty()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void StructProperty()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetProperty("PropertyToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    [Fact]
    public void StructField()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetField("FieldToMark");
        ValidateMessage(info);
        ValidateHiddenState(info, expectedState);
        ValidateIsNotError(info);
    }

    static void ValidateMessage(ICustomAttributeProvider attributeProvider)
    {
        var attribute = ReadAttribute(attributeProvider);
        Assert.Equal("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.", attribute.Message);
    }

    static ObsoleteAttribute ReadAttribute(ICustomAttributeProvider attributeProvider)
    {
        var attributes = attributeProvider.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        return (ObsoleteAttribute)attributes.First();
    }

    static void ValidateHiddenState(ICustomAttributeProvider attributeProvider, ModuleWeaver.HideObsoleteMembersState state)
    {
        var attributes = attributeProvider.GetCustomAttributes(typeof(EditorBrowsableAttribute), false);
        var attribute = (EditorBrowsableAttribute)attributes.FirstOrDefault();
        switch (state)
        {
            case ModuleWeaver.HideObsoleteMembersState.Advanced:
                Assert.NotNull(attribute);
                Assert.Equal(EditorBrowsableState.Advanced, attribute.State);
                break;
            case ModuleWeaver.HideObsoleteMembersState.Never:
                Assert.NotNull(attribute);
                Assert.Equal(EditorBrowsableState.Never, attribute.State);
                break;
            case ModuleWeaver.HideObsoleteMembersState.Off:
                Assert.Null(attribute);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    static void ValidateIsError(ICustomAttributeProvider attributeProvider)
    {
        var attribute = ReadAttribute(attributeProvider);
        Assert.True(attribute.IsError);
    }

    static void ValidateIsNotError(ICustomAttributeProvider attributeProvider)
    {
        var attribute = ReadAttribute(attributeProvider);
        Assert.False(attribute.IsError);
    }
}