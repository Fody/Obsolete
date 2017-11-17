using NUnit.Framework;

[TestFixture]
public class SemanticVersionTests
{
    [Test]
    public void TryParse()
    {
        SemanticVersion.TryParse("0.1", out var version1);

        Assert.AreEqual(version1.Major,0);
        Assert.AreEqual(version1.Minor,1);
        Assert.AreEqual(version1.Patch,0);

        SemanticVersion.TryParse("0.1.0", out var version2);

        Assert.AreEqual(version2.Major,0);
        Assert.AreEqual(version2.Minor,1);
        Assert.AreEqual(version2.Patch,0);
    }
}