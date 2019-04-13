using Fody;
using Xunit;
using Xunit.Abstractions;

public class VersionExtensionTests :
    XunitLoggingBase
{
    [Fact]
    public void IncrementMajor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Major);
        Assert.Equal("3.0.0",version);
    }

    [Fact]
    public void IncrementMajorWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Increment(StepType.Major);
        Assert.Equal("3.0.0",version);
    }

    [Fact]
    public void IncrementMajorWithMinorAndPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Increment(StepType.Major);
        Assert.Equal("3.0.0",version);
    }

    [Fact]
    public void IncrementMinor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Minor);
        Assert.Equal("2.1.0",version);
    }

    [Fact]
    public void IncrementMinorWithPatch()
    {
        SemanticVersion version1 = "2.0.1";
        var version = version1.Increment(StepType.Minor);
        Assert.Equal("2.1.0",version);
    }

    [Fact]
    public void IncrementPatch()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Patch);
        Assert.Equal("2.0.1",version);
    }

    [Fact]
    public void IncrementPatchWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Increment(StepType.Patch);
        Assert.Equal("2.1.1",version);
    }

    [Fact]
    public void DecrementMajor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Decrement(StepType.Major);
        Assert.Equal("1.0.0", version);
    }

    [Fact]
    public void DecrementMajorWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Decrement(StepType.Major);
        Assert.Equal("1.0.0", version);
    }

    [Fact]
    public void DecrementMajorWithMinorAndPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Major);
        Assert.Equal("1.0.0", version);
    }

    [Fact]
    public void DecrementMajorError()
    {
        SemanticVersion version1 = "0";
        Assert.Throws<WeavingException>(() => version1.Decrement(StepType.Major));
    }

    [Fact]
    public void DecrementMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Decrement(StepType.Minor);
        Assert.Equal("2.0.0", version);
    }

    [Fact]
    public void DecrementMinorWithPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Minor);
        Assert.Equal("2.0.0", version);
    }

    [Fact]
    public void DecrementMinorError()
    {
        SemanticVersion version1 = "2.0";
        Assert.Throws<WeavingException>(() => version1.Decrement(StepType.Minor));
    }

    [Fact]
    public void DecrementPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Patch);
        Assert.Equal("2.1.0", version);
    }

    [Fact]
    public void DecrementPatchError()
    {
        SemanticVersion version1 = "2.1.0";
        Assert.Throws<WeavingException>(() => version1.Decrement(StepType.Patch));
    }

    public VersionExtensionTests(ITestOutputHelper output) :
        base(output)
    {
    }
}