using System;
using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class AttributeDataFormatterTests
{

    [Test]
    public void All()
    {
        var attributeData = new AttributeData
                                {
                                    Message = "Custom Message.",
                                    TreatAsErrorFromVersion = new Version(2,0,0),
                                    RemoveInVersion = new Version(4, 0, 0),
                                    Replacement = "NewMember"
                                };
        var assemblyVersion = new Version(1, 0, 0, 0);
        var dataFormatter = new ModuleWeaver {assemblyVersion = assemblyVersion};
        var message = dataFormatter.ConvertToMessage(attributeData);
        Assert.AreEqual("Custom Message. Please use `NewMember` instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.", message);
    }
  

    [Test]
    public void ForSample()
    {
        var attributeData = new AttributeData
                                {
                                    Message = "Custom Message.",
                                    TreatAsErrorFromVersion = new Version(2,0,0),
                                    RemoveInVersion = new Version(4, 0, 0),
                                    Replacement = "NewClass"
                                };
        var dataFormatter1 = new ModuleWeaver { assemblyVersion = new Version(1, 0, 0) };
        Debug.WriteLine(dataFormatter1.ConvertToMessage(attributeData));
        var dataFormatter2 = new ModuleWeaver { assemblyVersion = new Version(3, 0, 0) };
        Debug.WriteLine(dataFormatter2.ConvertToMessage(attributeData));
    }

    
}