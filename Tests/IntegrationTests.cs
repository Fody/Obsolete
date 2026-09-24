using System.ComponentModel;
using System.Reflection;
using Fody;
using Assembly = System.Reflection.Assembly;
using TestResult = Fody.TestResult;
using ICustomAttributeProvider = System.Reflection.ICustomAttributeProvider;

// the weaver runs against shared files on disk, and the results are cached per state
[NotInParallel]
[InheritsTests]
public class IntegrationTestsDefaultHiding() :
    IntegrationTestsBase(ModuleWeaver.HideObsoleteMembersState.Advanced);

[NotInParallel]
[InheritsTests]
public class IntegrationTestsNeverHiding() :
    IntegrationTestsBase(ModuleWeaver.HideObsoleteMembersState.Never);

[NotInParallel]
[InheritsTests]
public class IntegrationTestsHidingDisabled() :
    IntegrationTestsBase(ModuleWeaver.HideObsoleteMembersState.Off);

public abstract class IntegrationTestsBase
{
    static Dictionary<ModuleWeaver.HideObsoleteMembersState, TestResult> results = [];

    Assembly assembly;
    TestResult testResult;
    ModuleWeaver.HideObsoleteMembersState expectedState;

    protected IntegrationTestsBase(ModuleWeaver.HideObsoleteMembersState state)
    {
        lock (results)
        {
            if (!results.TryGetValue(state, out var result))
            {
                var weaver = new ModuleWeaver
                {
                    HideObsoleteMembers = state
                };
                result = weaver.ExecuteTestRun("AssemblyToProcess.dll");
                results[state] = result;
            }

            testResult = result;
        }

        assembly = testResult.Assembly;
        expectedState = state;
    }

    [Test]
    public async Task Class()
    {
        var type = assembly.GetType("ClassToMark");
        await ValidateMessage(type);
        await ValidateHiddenState(type, expectedState);
        await ValidateIsNotError(type);
    }

    [Test]
    public async Task ClassWithHigherAssumedRemoveInVersion()
    {
        var type = assembly.GetType("ClassToMarkWithHigherAssumedRemoveInVersion");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)attributes.First();
        await Assert.That(obsoleteAttribute.Message).IsEqualTo("Will be treated as an error from version 3.0.0. Will be removed in version 4.0.0.");
        await ValidateIsNotError(type);
    }

    [Test]
    public async Task ClassToMarkWithSameRemoveAndTreatAsError()
    {
        var type = assembly.GetType("ClassToMarkWithSameRemoveAndTreatAsError");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var obsoleteAttribute = (ObsoleteAttribute)attributes.First();
        await Assert.That(obsoleteAttribute.Message).IsEqualTo("Will be treated as an error from version 1.2.0. Will be removed in version 1.2.0.");
        await ValidateIsNotError(type);
    }

    [Test]
    public async Task ClassToMarkWithHigherAssumedTreatAsErrorFromVersion()
    {
        var type = assembly.GetType("ClassToMarkWithHigherAssumedTreatAsErrorFromVersion");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var attribute = (ObsoleteAttribute)attributes.First();
        await Assert.That(attribute.Message).IsEqualTo("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.");
        await ValidateIsNotError(type);
    }

    [Test]
    public async Task ClassWithAssumedRemoveInVersion()
    {
        var type = assembly.GetType("ClassToMarkWithAssumedRemoveInVersion");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var attribute = (ObsoleteAttribute)attributes.First();
        await Assert.That(attribute.Message).IsEqualTo("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.");
        await ValidateIsNotError(type);
    }

    [Test]
    public async Task ClassToMarkWithAssumedTreatAsErrorFromVersion()
    {
        var type = assembly.GetType("ClassToMarkWithAssumedTreatAsErrorFromVersion");
        var attributes = ((ICustomAttributeProvider)type).GetCustomAttributes(typeof(ObsoleteAttribute), false);
        var attribute = (ObsoleteAttribute)attributes.First();
        await Assert.That(attribute.Message).IsEqualTo("Will be removed in version 2.0.0.");
        await ValidateIsError(type);
    }

    [Test]
    public async Task Warnings()
    {
        await Assert.That(testResult.Warnings.Select(_ => _.Text)).Contains("The member `ClassWithObsoleteAttribute` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.");
    }

    [Test]
    public async Task NoWarnings()
    {
        await Assert.That(testResult.Warnings.Select(_ => _.Text)).DoesNotContain("The member `ClassWithObsoleteAttributeToSkip` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.");
    }

    [Test]
    public async Task Errors()
    {
        await Assert.That(testResult.Errors.Select(_ => _.Text)).Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: `System.Void ClassWithObsoleteOnGetSet::set_PropertyToMark(System.String)`.");
        await Assert.That(testResult.Errors.Select(_ => _.Text)).Contains("ObsoleteExAttribute is not valid on property gets or sets. Member: `System.String ClassWithObsoleteOnGetSet::get_PropertyToMark()`.");
    }

    [Test]
    public async Task Interface()
    {
        var type = assembly.GetType("InterfaceToMark");
        await ValidateMessage(type);
        await ValidateHiddenState(type, expectedState);
        await ValidateIsNotError(type);
    }

    [Test]
    public async Task ClassWithIsError()
    {
        var type = assembly.GetType("ClassWithIsError");
        await ValidateIsError(type);
    }

    [Test]
    public async Task ClassWithIsErrorFromInformationalVersion()
    {
        var type = assembly.GetType("ClassWithIsErrorFromInformationalVersion");
        await ValidateIsError(type);
    }

    [Test]
    public async Task Enum()
    {
        var type = assembly.GetType("EnumToMark");
        await ValidateIsNotError(type);
    }

    [Test]
    public async Task Struct()
    {
        var type = assembly.GetType("StructToMark");
        await ValidateIsNotError(type);
    }

    [Test]
    public async Task EnumField()
    {
        var type = assembly.GetType("EnumToMark");
        var info = type.GetField("Foo");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task ClassMethod()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task ClassMethodThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetMethod("MethodWithExceptionToMark");
        var attribute = ReadAttribute(info);
        await Assert.That(attribute.Message).IsEqualTo("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.");
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task InterfaceMethod()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMethod("MethodToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task StructMethod()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMethod("MethodToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task ClassPropertySetThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyWithSetExceptionToMark");
        var attribute = ReadAttribute(info);
        await Assert.That(attribute.Message).IsEqualTo("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.");
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task ClassPropertyGetThatThrows()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyWithGetExceptionToMark");
        var attribute = ReadAttribute(info);
        await Assert.That(attribute.Message).IsEqualTo("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. The member currently throws a NotImplementedException. Will be removed in version 4.0.0.");
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task ClassProperty()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetProperty("PropertyToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task ClassField()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetField("FieldToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task InterfaceEvent()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetMember("EventToMark").First();
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task ClassEvent()
    {
        var type = assembly.GetType("ClassToMark");
        var info = type.GetEvent("EventToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task StructEvent()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetMember("EventToMark").First();
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

#if NET9_0_OR_GREATER

    [Test]
    public async Task ClassWithRequiredMembers()
    {
        await Assert.That(testResult.Warnings.Select(_ => _.Text)).DoesNotContain("The member `System.Void ClassWithRequiredMembers::.ctor()` has an ObsoleteAttribute. Consider replacing it with an ObsoleteExAttribute.");
    }

#endif

    [Test]
    public async Task InterfaceProperty()
    {
        var type = assembly.GetType("InterfaceToMark");
        var info = type.GetProperty("PropertyToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task StructProperty()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetProperty("PropertyToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    [Test]
    public async Task StructField()
    {
        var type = assembly.GetType("StructToMark");
        var info = type.GetField("FieldToMark");
        await ValidateMessage(info);
        await ValidateHiddenState(info, expectedState);
        await ValidateIsNotError(info);
    }

    static async Task ValidateMessage(ICustomAttributeProvider attributeProvider)
    {
        var attribute = ReadAttribute(attributeProvider);
        await Assert.That(attribute.Message).IsEqualTo("Custom message. Use `NewThing` instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.");
    }

    static ObsoleteAttribute ReadAttribute(ICustomAttributeProvider attributeProvider)
    {
        var attributes = attributeProvider.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        return (ObsoleteAttribute)attributes.First();
    }

    static async Task ValidateHiddenState(ICustomAttributeProvider attributeProvider, ModuleWeaver.HideObsoleteMembersState state)
    {
        var attributes = attributeProvider.GetCustomAttributes(typeof(EditorBrowsableAttribute), false);
        var attribute = (EditorBrowsableAttribute)attributes.FirstOrDefault();
        switch (state)
        {
            case ModuleWeaver.HideObsoleteMembersState.Advanced:
                await Assert.That(attribute).IsNotNull();
                await Assert.That(attribute.State).IsEqualTo(EditorBrowsableState.Advanced);
                break;
            case ModuleWeaver.HideObsoleteMembersState.Never:
                await Assert.That(attribute).IsNotNull();
                await Assert.That(attribute.State).IsEqualTo(EditorBrowsableState.Never);
                break;
            case ModuleWeaver.HideObsoleteMembersState.Off:
                await Assert.That(attribute).IsNull();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    static async Task ValidateIsError(ICustomAttributeProvider attributeProvider)
    {
        var attribute = ReadAttribute(attributeProvider);
        await Assert.That(attribute.IsError).IsTrue();
    }

    static async Task ValidateIsNotError(ICustomAttributeProvider attributeProvider)
    {
        var attribute = ReadAttribute(attributeProvider);
        await Assert.That(attribute.IsError).IsFalse();
    }
}