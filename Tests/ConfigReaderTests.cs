using System.Xml.Linq;
using Fody;

public class ConfigReaderTests
{
    [Test]
    public async Task ThrowsNotImplementedText()
    {
        var element = XElement.Parse("<Obsolete ThrowsNotImplementedText='Custom Text'/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        weaver.ReadConfig();
        await Assert.That(weaver.ThrowsNotImplementedText).IsEqualTo("Custom Text");
    }

    [Test]
    [Arguments("false", ModuleWeaver.HideObsoleteMembersState.Off)]
    [Arguments("False", ModuleWeaver.HideObsoleteMembersState.Off)]
    [Arguments("true", ModuleWeaver.HideObsoleteMembersState.Advanced)]
    [Arguments("True", ModuleWeaver.HideObsoleteMembersState.Advanced)]
    [Arguments("advanced", ModuleWeaver.HideObsoleteMembersState.Advanced)]
    [Arguments("Advanced", ModuleWeaver.HideObsoleteMembersState.Advanced)]
    [Arguments("never", ModuleWeaver.HideObsoleteMembersState.Never)]
    [Arguments("Never", ModuleWeaver.HideObsoleteMembersState.Never)]
    [Arguments("off", ModuleWeaver.HideObsoleteMembersState.Off)]
    [Arguments("Off", ModuleWeaver.HideObsoleteMembersState.Off)]
    public async Task HideObsoleteMembers(string state, ModuleWeaver.HideObsoleteMembersState expected)
    {
        var element = XElement.Parse($"<Obsolete HideObsoleteMembers='{state}'/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        weaver.ReadConfig();
        await Assert.That(weaver.HideObsoleteMembers).IsEqualTo(expected);
    }

    [Test]
    public async Task EmptyHideObsoleteMembers()
    {
        var element = XElement.Parse("<Obsolete/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        weaver.ReadConfig();
        await Assert.That(weaver.HideObsoleteMembers).IsEqualTo(ModuleWeaver.HideObsoleteMembersState.Advanced);
    }

    [Test]
    public async Task CanParseStepType()
    {
        var element = XElement.Parse("<Obsolete StepType='Minor'/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        weaver.ReadConfig();
        await Assert.That(weaver.StepType).IsEqualTo(StepType.Minor);
    }

    [Test]
    public async Task VersionIncrementThrows()
    {
        var element = XElement.Parse("<Obsolete VersionIncrement='1.0.1'/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        var exception = await Assert.That(() => weaver.ReadConfig()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("VersionIncrement is no longer supported. Use StepType instead.");
    }
}