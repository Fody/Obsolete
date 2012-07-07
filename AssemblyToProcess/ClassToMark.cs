using System;

[ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
public class ClassToMark
{
    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
    public string PropertyToMark { get; set; }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
    public void MethodToMark (){}

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
    public event EventHandler EventToMark;

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
    public string FieldToMark;
}