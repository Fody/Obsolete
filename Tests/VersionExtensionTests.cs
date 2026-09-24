using Fody;

public class VersionExtensionTests
{
    [Test]
    public async Task IncrementMajor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Major);
        await Assert.That(version.ToString()).IsEqualTo("3.0.0");
    }

    [Test]
    public async Task IncrementMajorWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Increment(StepType.Major);
        await Assert.That(version.ToString()).IsEqualTo("3.0.0");
    }

    [Test]
    public async Task IncrementMajorWithMinorAndPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Increment(StepType.Major);
        await Assert.That(version.ToString()).IsEqualTo("3.0.0");
    }

    [Test]
    public async Task IncrementMinor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Minor);
        await Assert.That(version.ToString()).IsEqualTo("2.1.0");
    }

    [Test]
    public async Task IncrementMinorWithPatch()
    {
        SemanticVersion version1 = "2.0.1";
        var version = version1.Increment(StepType.Minor);
        await Assert.That(version.ToString()).IsEqualTo("2.1.0");
    }

    [Test]
    public async Task IncrementPatch()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Patch);
        await Assert.That(version.ToString()).IsEqualTo("2.0.1");
    }

    [Test]
    public async Task IncrementPatchWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Increment(StepType.Patch);
        await Assert.That(version.ToString()).IsEqualTo("2.1.1");
    }

    [Test]
    public async Task DecrementMajor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Decrement(StepType.Major);
        await Assert.That(version.ToString()).IsEqualTo("1.0.0");
    }

    [Test]
    public async Task DecrementMajorWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Decrement(StepType.Major);
        await Assert.That(version.ToString()).IsEqualTo("1.0.0");
    }

    [Test]
    public async Task DecrementMajorWithMinorAndPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Major);
        await Assert.That(version.ToString()).IsEqualTo("1.0.0");
    }

    [Test]
    public async Task DecrementMajorError()
    {
        SemanticVersion version1 = "0";
        await Assert.That(() => version1.Decrement(StepType.Major)).Throws<WeavingException>();
    }

    [Test]
    public async Task DecrementMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Decrement(StepType.Minor);
        await Assert.That(version.ToString()).IsEqualTo("2.0.0");
    }

    [Test]
    public async Task DecrementMinorWithPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Minor);
        await Assert.That(version.ToString()).IsEqualTo("2.0.0");
    }

    [Test]
    public async Task DecrementMinorError()
    {
        SemanticVersion version1 = "2.0";
        await Assert.That(() => version1.Decrement(StepType.Minor)).Throws<WeavingException>();
    }

    [Test]
    public async Task DecrementPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Patch);
        await Assert.That(version.ToString()).IsEqualTo("2.1.0");
    }

    [Test]
    public async Task DecrementPatchError()
    {
        SemanticVersion version1 = "2.1.0";
        await Assert.That(() => version1.Decrement(StepType.Patch)).Throws<WeavingException>();
    }
}