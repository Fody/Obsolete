using System;

namespace Before
{
    [ObsoleteEx(Message = "Custom message.", Replacement = "NewClass", TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0")]
    public class ClassToMark
    {
    }
}

namespace AfterVersion1
{
    [Obsolete("Custom Message. Please use 'NewClass' instead. Will be treated as an error from version '2.0.0.0'. Will be removed in version '4.0.0.0'.", false)]
    public class ClassToMark
    {
    }
}

namespace AfterVersion3
{
    [Obsolete("Custom Message. Please use 'NewClass' instead. Will be removed in version '4.0.0.0'.", false)]
    public class ClassToMark
    {
    }
}
