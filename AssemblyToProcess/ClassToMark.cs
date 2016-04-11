using System;

[ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
public class ClassToMark
{
    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public string PropertyToMark { get; set; }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public string PropertyWithSetExceptionToMark
    {
        get { return propertyWithSetExceptionToMark; }
        set
        {
            throw new NotImplementedException();
        }
    }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public string PropertyWithGetExceptionToMark
    {
        get
        {
            throw new NotImplementedException();
        }
        set { propertyWithGetExceptionToMark = value; }
    }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public void MethodToMark() { }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public void MethodWithExceptionToMark()
    {
        throw new NotImplementedException();
    }

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public event EventHandler EventToMark;

    [ObsoleteEx(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "4.0", Message = "Custom message.", ReplacementTypeOrMember = "NewThing")]
    public string FieldToMark;

    string propertyWithGetExceptionToMark;
    string propertyWithSetExceptionToMark;
}