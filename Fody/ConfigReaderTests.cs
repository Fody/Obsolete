using System;
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
    public void VersionIncrement()
    {
        var xElement = XElement.Parse(@"<Obsolete VersionIncrement='1.0.1'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ReadConfig();
        Assert.IsTrue(new SemanticVersion{Major = 1,Minor = 0,Patch = 1}== moduleWeaver.VersionIncrement);
    }
    [Test]
    public void InvalidRevision()
    {
        var xElement = XElement.Parse(@"<Obsolete VersionIncrement='1.0.1.1'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};

        var exception = Assert.Throws<Exception>(moduleWeaver.ReadConfig);
        Assert.AreEqual("Could not parse 'VersionIncrement' from '1.0.1.1'.", exception.Message);
    }

}