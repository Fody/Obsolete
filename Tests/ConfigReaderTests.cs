using System.Xml.Linq;
using Fody;
using Xunit;
using Xunit.Abstractions;

public class ConfigReaderTests :
    XunitApprovalBase
{
    [Fact]
    public void TrueHideObsoleteMembers()
    {
        var xElement = XElement.Parse(@"<Obsolete HideObsoleteMembers='true'/>");
        var moduleWeaver = new ModuleWeaver
        {
            Config = xElement
        };
        moduleWeaver.ReadConfig();
        Assert.True(moduleWeaver.HideObsoleteMembers);
    }

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

    [Fact]
    public void FalseHideObsoleteMembers()
    {
        var xElement = XElement.Parse(@"<Obsolete HideObsoleteMembers='false'/>");
        var moduleWeaver = new ModuleWeaver
        {
            Config = xElement
        };
        moduleWeaver.ReadConfig();
        Assert.False(moduleWeaver.HideObsoleteMembers);
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
        Assert.True(moduleWeaver.HideObsoleteMembers);
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

    public ConfigReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}