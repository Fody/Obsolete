public class SemanticVersionTests
{
    [Test]
    public async Task TryParse()
    {
        SemanticVersion.TryParse("0.1", out var version1);

        await Assert.That(version1.Major).IsEqualTo(0);
        await Assert.That(version1.Minor).IsEqualTo(1);
        await Assert.That(version1.Patch).IsEqualTo(0);

        SemanticVersion.TryParse("0.1.0", out var version2);

        await Assert.That(version2.Major).IsEqualTo(0);
        await Assert.That(version2.Minor).IsEqualTo(1);
        await Assert.That(version2.Patch).IsEqualTo(0);
    }
}