[ObsoleteEx(
    TreatAsErrorFromVersion = "2.0", 
    RemoveInVersion = "4.0", 
    Message = "Custom message.", 
    Replacement = "NewThing")]
enum EnumToMark
{
    [ObsoleteEx(
        TreatAsErrorFromVersion = "2.0", 
        RemoveInVersion = "4.0", 
        Message = "Custom message.", 
        Replacement = "NewThing")]
    Foo,
}