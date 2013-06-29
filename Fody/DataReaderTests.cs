using System;
using Mono.Cecil;
using NUnit.Framework;
using NUnit.Framework.Constraints;

[TestFixture]
public class DataReaderTests
{

    [Test]
    public void ConvertToVersionValid()
    {
        var version = DataReader.ConvertToVersion("1.0.0");
        Assert.AreEqual(new Version(1,0,0), version);
    }

    [Test]
    public void ConvertToVersionInValid()
    {
        var exception = Assert.Throws<WeavingException>(() => DataReader.ConvertToVersion("1.0.0.1"));

        Assert.AreEqual("Could not convert '1.0.0.1' to a Version. Obsolete Fody does not support Revision numbers.", exception.Message);
    }
}