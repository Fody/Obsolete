using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class VersionExtensionTests
{

    [Test]
    public void TrueHideObsoleteMembers()
    {
        SemanticVersion version1 = "2";
        SemanticVersion version2 = "1";
        var version = version1.Add(version2);
        Trace.WriteLine(version.ToString());
    }
}