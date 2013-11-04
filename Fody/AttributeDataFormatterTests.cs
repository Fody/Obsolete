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
                                    TreatAsErrorFromVersion = "2",
                                    RemoveInVersion = "4",
                                    Replacement = "NewMember"
                                };
        SemanticVersion assemblyVersion = "1";
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
                                    TreatAsErrorFromVersion = "2",
                                    RemoveInVersion = "4",
                                    Replacement = "NewClass"
                                };
        var dataFormatter1 = new ModuleWeaver { assemblyVersion = "1"};
        Debug.WriteLine(dataFormatter1.ConvertToMessage(attributeData));
        var dataFormatter2 = new ModuleWeaver { assemblyVersion = "3"};
        Debug.WriteLine(dataFormatter2.ConvertToMessage(attributeData));
    }

    
}