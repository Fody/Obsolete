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

# Configuration Options

All config options are access by modifying the `Obsolete` node in FodyWeavers.xml

## HideObsoleteMembers

When this is `true` obsolete members will also have `[EditorBrowsable(EditorBrowsableState.Advanced)]` added to them.

*Defaults to `false`*

    <Obsolete HideObsoleteMembers='true'/>

## TreatAsErrorFormat

The string used when informing the user what version the member will be treated as an error.

*Defaults to  `Will be treated as an error from version '{0}'. `

    <Obsolete TreatAsErrorFormat='Will be treated as an error from version '{0}'. '/>

## RemoveInVersionFormat

The string used when informing the user what version the member will be removed it.

*Defaults to  `Will be removed in version '{0}'. `

    <Obsolete RemoveInVersionFormat='Will be removed in version '{0}'. '/>

## ReplacementFormat

The string used when informing the user of an alternative member to use instead of the obsolete member.

*Defaults to `Please use '{0}' instead. `

    <Obsolete ReplacementFormat='Please use '{0}' instead. '/>

    

