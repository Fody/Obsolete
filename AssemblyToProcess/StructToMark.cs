using System;

[ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", Replacement = "NewThing")]
public struct StructToMark
{
    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", Replacement = "NewThing")]
    public string PropertyToMark { get; set; }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", Replacement = "NewThing")] 
    public string FieldToMark;

   [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", Replacement = "NewThing")]
    public void MethodToMark() { }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", Replacement = "NewThing")]
    public event EventHandler EventToMark;
}