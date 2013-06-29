using System;
using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class VersionExtensionTests
{

    [Test]
    public void TrueHideObsoleteMembers()
    {
        var version = new Version(2, 0, 0, 0).Add(new Version(1, 0));
        Trace.WriteLine(version.ToString());
    }
}