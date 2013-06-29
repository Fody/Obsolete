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
    public void VersionIncrement()
    {
        var xElement = XElement.Parse(@"<Obsolete VersionIncrement='1.0.1'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ReadConfig();
        Assert.AreEqual(new Version(1,0,1), moduleWeaver.VersionIncrement);
    }
    [Test]
    public void InvalidRevision()
    {
        var xElement = XElement.Parse(@"<Obsolete VersionIncrement='1.0.1.1'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};

        var exception = Assert.Throws<Exception>(moduleWeaver.ReadConfig);
        Assert.AreEqual("Could not parse 'VersionIncrement' from '1.0.1.1'. Revision not supported.", exception.Message);
    }

}