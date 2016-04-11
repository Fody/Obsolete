using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class ConfigReaderTests
{

    [Test]
    public void TrueHideObsoleteMembers()
    {
        var xElement = XElement.Parse(@"<Obsolete HideObsoleteMembers='true'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ReadConfig();
        Assert.IsTrue(moduleWeaver.HideObsoleteMembers);
    }

    [Test]
    public void MemberThrowsNotImplementedText()
    {
        var xElement = XElement.Parse(@"<Obsolete MemberThrowsNotImplementedText='Custom Text'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ReadConfig();
        Assert.AreEqual("Custom Text", moduleWeaver.MemberThrowsNotImplementedText);
    }

    [Test]
    public void FalseHideObsoleteMembers()
    {
        var xElement = XElement.Parse(@"<Obsolete HideObsoleteMembers='false'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ReadConfig();
        Assert.IsFalse(moduleWeaver.HideObsoleteMembers);
    }

    [Test]
    public void EmptyHideObsoleteMembers()
    {
        var xElement = XElement.Parse(@"<Obsolete/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ReadConfig();
        Assert.IsTrue(moduleWeaver.HideObsoleteMembers);
    }

    [Test]
    public void CanParseStepType()
    {
        var xElement = XElement.Parse(@"<Obsolete StepType='Minor'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ReadConfig();
        Assert.AreEqual(StepType.Minor, moduleWeaver.StepType);
    }

    [Test]
    public void VersionIncrementThrows()
    {
        var xElement = XElement.Parse(@"<Obsolete VersionIncrement='1.0.1'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        var exception = Assert.Throws<WeavingException>(moduleWeaver.ReadConfig);
        Assert.AreEqual("VersionIncrement is no longer supported. Use StepType instead.", exception.Message);
    }


}