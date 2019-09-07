using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

public class AttributeDataFormatterTests :
    XunitApprovalBase
{
    [Fact]
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
        Assert.Equal("Custom Message. Use `NewMember` instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.", message);
    }

    [Fact]
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
        Trace.WriteLine(dataFormatter1.ConvertToMessage(attributeData));
        var dataFormatter2 = new ModuleWeaver { assemblyVersion = "3"};
        Trace.WriteLine(dataFormatter2.ConvertToMessage(attributeData));
    }

    public AttributeDataFormatterTests(ITestOutputHelper output) : 
        base(output)
    {
    }
}