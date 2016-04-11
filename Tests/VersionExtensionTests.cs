using NUnit.Framework;

[TestFixture]
public class VersionExtensionTests
{

    [Test]
    public void IncrementMajor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Major);
        Assert.AreEqual("3.0.0",(string)version);
    }

    [Test]
    public void IncrementMajorWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Increment(StepType.Major);
        Assert.AreEqual("3.0.0",(string)version);
    }

    [Test]
    public void IncrementMajorWithMinorAndPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Increment(StepType.Major);
        Assert.AreEqual("3.0.0",(string)version);
    }

    [Test]
    public void IncrementMinor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Minor);
        Assert.AreEqual("2.1.0",(string)version);
    }

    [Test]
    public void IncrementMinorWithPatch()
    {
        SemanticVersion version1 = "2.0.1";
        var version = version1.Increment(StepType.Minor);
        Assert.AreEqual("2.1.0",(string)version);
    }

    [Test]
    public void IncrementPatch()
    {
        SemanticVersion version1 = "2";
        var version = version1.Increment(StepType.Patch);
        Assert.AreEqual("2.0.1",(string)version);
    }

    [Test]
    public void IncrementPatchWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Increment(StepType.Patch);
        Assert.AreEqual("2.1.1",(string)version);
    }

    [Test]
    public void DecrementMajor()
    {
        SemanticVersion version1 = "2";
        var version = version1.Decrement(StepType.Major);
        Assert.AreEqual("1.0.0", (string)version);
    }

    [Test]
    public void DecrementMajorWithMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Decrement(StepType.Major);
        Assert.AreEqual("1.0.0", (string)version);
    }

    [Test]
    public void DecrementMajorWithMinorAndPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Major);
        Assert.AreEqual("1.0.0", (string)version);
    }

    [Test]
    public void DecrementMajorError()
    {
        SemanticVersion version1 = "0";
        Assert.Throws<WeavingException>(() => version1.Decrement(StepType.Major));
    }

    [Test]
    public void DecrementMinor()
    {
        SemanticVersion version1 = "2.1";
        var version = version1.Decrement(StepType.Minor);
        Assert.AreEqual("2.0.0", (string)version);
    }

    [Test]
    public void DecrementMinorWithPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Minor);
        Assert.AreEqual("2.0.0", (string)version);
    }

    [Test]
    public void DecrementMinorError()
    {
        SemanticVersion version1 = "2.0";
        Assert.Throws<WeavingException>(() => version1.Decrement(StepType.Minor));
    }

    [Test]
    public void DecrementPatch()
    {
        SemanticVersion version1 = "2.1.1";
        var version = version1.Decrement(StepType.Patch);
        Assert.AreEqual("2.1.0", (string)version);
    }

    [Test]
    public void DecrementPatchError()
    {
        SemanticVersion version1 = "2.1.0";
        Assert.Throws<WeavingException>(() => version1.Decrement(StepType.Patch));
    }
}