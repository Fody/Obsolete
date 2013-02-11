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

}