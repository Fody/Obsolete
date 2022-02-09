using System.Xml.Linq;
using Fody;
using Xunit;

public class ConfigReaderTests
{
    [Fact]
    public void ThrowsNotImplementedText()
    {
        var xElement = XElement.Parse(@"<Obsolete ThrowsNotImplementedText='Custom Text'/>");
        var moduleWeaver = new ModuleWeaver
        {
            Config = xElement
        };
        moduleWeaver.ReadConfig();
        Assert.Equal("Custom Text", moduleWeaver.ThrowsNotImplementedText);
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
        var xElement = XElement.Parse($"<Obsolete HideObsoleteMembers='{state}'/>");
        var moduleWeaver = new ModuleWeaver
        {
            Config = xElement
        };
        moduleWeaver.ReadConfig();
        Assert.Equal(expected, moduleWeaver.HideObsoleteMembers);
    }
    
    [Fact]
    public void EmptyHideObsoleteMembers()
    {
        var xElement = XElement.Parse("<Obsolete/>");
        var moduleWeaver = new ModuleWeaver
        {
            Config = xElement
        };
        moduleWeaver.ReadConfig();
        Assert.Equal(ModuleWeaver.HideObsoleteMembersState.Advanced, moduleWeaver.HideObsoleteMembers);
    }

    [Fact]
    public void CanParseStepType()
    {
        var xElement = XElement.Parse(@"<Obsolete StepType='Minor'/>");
        var moduleWeaver = new ModuleWeaver
        {
            Config = xElement
        };
        moduleWeaver.ReadConfig();
        Assert.Equal(StepType.Minor, moduleWeaver.StepType);
    }

    [Fact]
    public void VersionIncrementThrows()
    {
        var xElement = XElement.Parse(@"<Obsolete VersionIncrement='1.0.1'/>");
        var moduleWeaver = new ModuleWeaver
        {
            Config = xElement
        };
        var exception = Assert.Throws<WeavingException>(() => moduleWeaver.ReadConfig());
        Assert.Equal("VersionIncrement is no longer supported. Use StepType instead.", exception.Message);
    }
}