[ObsoleteEx(
    TreatAsErrorFromVersion = "2.0",
    RemoveInVersion = "4.0",
    Message = "Custom message.",
    ReplacementTypeOrMember = "NewThing")]
enum EnumToMark
{
    [ObsoleteEx(
        TreatAsErrorFromVersion = "2.0",
        RemoveInVersion = "4.0",
        Message = "Custom message.",
        ReplacementTypeOrMember = "NewThing")]
    Foo,
}