using System;

[ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
public interface InterfaceToMark
{
    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
    string PropertyToMark { get; set; }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
    void MethodToMark();

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0", Message = "Custom message.", Replacement = "NewThing")]
    event EventHandler EventToMark;
}