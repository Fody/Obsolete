using System;
using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class AtributeDataFormatterTests
{
    [Test]
    public void Empty()
    {
        var assemblyVersion = new Version(1, 0, 0, 0);
        var formatterConfigReader = new FormatterConfigReader(null);
        var message = new DataFormatter(assemblyVersion, formatterConfigReader).ConvertToMessage(new AttributeData());
        Assert.AreEqual("", message);
    }
    [Test]
    public void CustomMessage()
    {
        var attributeData = new AttributeData {Message = "Custom Message."};
        var assemblyVersion = new Version(1, 0, 0, 0);
        var formatterConfigReader = new FormatterConfigReader(null);
        var message = new DataFormatter(assemblyVersion, formatterConfigReader).ConvertToMessage(attributeData);
        Assert.AreEqual("Custom Message.", message);
    }
    [Test]
    public void All()
    {
        var attributeData = new AttributeData
                                {
                                    Message = "Custom Message.",
                                    TreatAsErrorFromVersion = new Version(2,0,0,0),
                                    RemoveInVersion = new Version(3, 0, 0, 0),
                                    Replacement = "NewMember"
                                };
        var assemblyVersion = new Version(1, 0, 0, 0);
        var formatterConfigReader = new FormatterConfigReader(null);
        var dataFormatter = new DataFormatter(assemblyVersion, formatterConfigReader);
        var message = dataFormatter.ConvertToMessage(attributeData);
        Assert.AreEqual("Custom Message. Please use 'NewMember' instead. Will be treated as an error from version '2.0.0.0'. Will be removed in version '3.0.0.0'.", message);
    }

    [Test]
    public void ForSample()
    {
        var attributeData = new AttributeData
                                {
                                    Message = "Custom Message.",
                                    TreatAsErrorFromVersion = new Version(2,0,0,0),
                                    RemoveInVersion = new Version(4, 0, 0, 0),
                                    Replacement = "NewClass"
                                };
        var formatterConfigReader = new FormatterConfigReader(null);
        var dataFormatter1 = new DataFormatter(new Version(1, 0, 0, 0), formatterConfigReader);
        Debug.WriteLine(dataFormatter1.ConvertToMessage(attributeData));
        var dataFormatter2 = new DataFormatter(new Version(3, 0, 0, 0), formatterConfigReader);
        Debug.WriteLine(dataFormatter2.ConvertToMessage(attributeData));
    }

    
}