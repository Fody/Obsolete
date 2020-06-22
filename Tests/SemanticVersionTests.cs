using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class SemanticVersionTests
{
    [Fact]
    public void TryParse()
    {
        SemanticVersion.TryParse("0.1", out var version1);

        Assert.Equal(0, version1.Major);
        Assert.Equal(1, version1.Minor);
        Assert.Equal(0, version1.Patch);

        SemanticVersion.TryParse("0.1.0", out var version2);

        Assert.Equal(0, version2.Major);
        Assert.Equal(1, version2.Minor);
        Assert.Equal(0, version2.Patch);
    }

    public SemanticVersionTests(ITestOutputHelper output) :
        base(output)
    {
    }
}