using System;

[ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
public class ClassToMark
{
    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public string PropertyToMark { get; set; }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public void MethodToMark (){}

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public event EventHandler EventToMark;

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public string FieldToMark;
}