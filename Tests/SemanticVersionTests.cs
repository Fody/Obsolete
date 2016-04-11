using NUnit.Framework;

[TestFixture]
public class SemanticVersionTests
{


    [Test]
    public void TryParse()
    {
        SemanticVersion version;
        SemanticVersion.TryParse("0.1", out version);

        Assert.AreEqual(version.Major,0);
        Assert.AreEqual(version.Minor,1);
        Assert.AreEqual(version.Patch,0);

        SemanticVersion.TryParse("0.1.0", out version);

        Assert.AreEqual(version.Major,0);
        Assert.AreEqual(version.Minor,1);
        Assert.AreEqual(version.Patch,0);
    }
}