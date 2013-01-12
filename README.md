## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Helps keep usages of [ObsoleteAttribute](http://msdn.microsoft.com/en-us/library/fwz0y5c2) consistent.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget package http://nuget.org/packages/Obsolete.Fody 

### Your Code

    [ObsoleteEx(
    Message = "Custom message.", 
    MarkAsErrorInVersion = "2.0", 
    WillBeRemovedInVersion = "4.0", 
    ReplacedWith = "NewClass")]
    public class ClassToMark {}



### What gets compiled when your assembly version is 1.0

    [Obsolete("Custom Message. Please use 'NewClass' instead. Will be treated as an error from version '2.0.0.0'. Will be removed in version '4.0.0.0'.")]
    public class ClassToMark{}



### What gets compiled when your assembly version is 3.0

    [Obsolete("Custom Message. Please use 'NewClass' instead. Will be removed in version '4.0.0.0'.", true)]
    public class ClassToMark{}


### What happens when your assembly version is 5.0

A compile error will be thrown since the class should have already been removed.

## The Message property 

The message property should only be used for useful information. The fact that it is a obsolete member does not need to be reiterated in the message.

**DO NOT**  use and of the following redundant messages

 * "Do not call this method"
 * "This method will be removed"
 * "This method is obsolete"
