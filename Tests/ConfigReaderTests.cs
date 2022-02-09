using System.Xml.Linq;
using Fody;

public class ConfigReaderTests
{
    [Fact]
    public void ThrowsNotImplementedText()
    {
        var element = XElement.Parse("<Obsolete ThrowsNotImplementedText='Custom Text'/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        weaver.ReadConfig();
        Assert.Equal("Custom Text", weaver.ThrowsNotImplementedText);
    }

    [Theory]
    [InlineData("false", ModuleWeaver.HideObsoleteMembersState.Off)]
    [InlineData("False", ModuleWeaver.HideObsoleteMembersState.Off)]
    [InlineData("true", ModuleWeaver.HideObsoleteMembersState.Advanced)]
    [InlineData("True", ModuleWeaver.HideObsoleteMembersState.Advanced)]
    [InlineData("advanced", ModuleWeaver.HideObsoleteMembersState.Advanced)]
    [InlineData("Advanced", ModuleWeaver.HideObsoleteMembersState.Advanced)]
    [InlineData("never", ModuleWeaver.HideObsoleteMembersState.Never)]
    [InlineData("Never", ModuleWeaver.HideObsoleteMembersState.Never)]
    [InlineData("off", ModuleWeaver.HideObsoleteMembersState.Off)]
    [InlineData("Off", ModuleWeaver.HideObsoleteMembersState.Off)]
    public void HideObsoleteMembers(string state, ModuleWeaver.HideObsoleteMembersState expected)
    {
        var element = XElement.Parse($"<Obsolete HideObsoleteMembers='{state}'/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        weaver.ReadConfig();
        Assert.Equal(expected, weaver.HideObsoleteMembers);
    }

    [Fact]
    public void EmptyHideObsoleteMembers()
    {
        var element = XElement.Parse("<Obsolete/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        weaver.ReadConfig();
        Assert.Equal(ModuleWeaver.HideObsoleteMembersState.Advanced, weaver.HideObsoleteMembers);
    }

    [Fact]
    public void CanParseStepType()
    {
        var element = XElement.Parse("<Obsolete StepType='Minor'/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        weaver.ReadConfig();
        Assert.Equal(StepType.Minor, weaver.StepType);
    }

    [Fact]
    public void VersionIncrementThrows()
    {
        var element = XElement.Parse("<Obsolete VersionIncrement='1.0.1'/>");
        var weaver = new ModuleWeaver
        {
            Config = element
        };
        var exception = Assert.Throws<WeavingException>(() => weaver.ReadConfig());
        Assert.Equal("VersionIncrement is no longer supported. Use StepType instead.", exception.Message);
    }
}