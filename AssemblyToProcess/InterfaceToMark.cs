using System;

[ObsoleteEx(
    TreatAsErrorFromVersion = "2.0",
    RemoveInVersion = "4.0",
    Message = "Custom message.",
    ReplacementTypeOrMember = "NewThing")]
public interface InterfaceToMark
{
    [ObsoleteEx(
        TreatAsErrorFromVersion = "2.0",
        RemoveInVersion = "4.0",
        Message = "Custom message.",
        ReplacementTypeOrMember = "NewThing")]
    string PropertyToMark { get; set; }

    [ObsoleteEx(
        TreatAsErrorFromVersion = "2.0",
        RemoveInVersion = "4.0",
        Message = "Custom message.",
        ReplacementTypeOrMember = "NewThing")]
    void MethodToMark();

    [ObsoleteEx(
        TreatAsErrorFromVersion = "2.0",
        RemoveInVersion = "4.0",
        Message = "Custom message.",
        ReplacementTypeOrMember = "NewThing")]
    event EventHandler EventToMark;
}