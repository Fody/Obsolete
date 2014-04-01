## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/Obsolete/master/Icons/package_icon.png)

Helps keep usages of [ObsoleteAttribute](http://msdn.microsoft.com/en-us/library/fwz0y5c2) consistent.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget

Nuget package http://nuget.org/packages/Obsolete.Fody 

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package Obsolete.Fody

### Your Code

    [ObsoleteEx(
    Message = "Custom message.", 
    TreatAsErrorFromVersion = "2.0", 
    RemoveInVersion = "4.0", 
    ReplacedWith = "NewClass")]
    public class ClassToMark {}

### Treat As Warning Mode

When the target assembly version is less than `RemoveInVersion` an `ObsoleteAttribute` with "treat as warning" will be injected. It will have the following format

    [Obsolete("MESSAGE. Please use 'REPLACED_WITH' instead. Will be treated as an error from version MARK_AS_VERSION_IN. Will be removed in version WILL_BE_REMOVED_IN_VERSION.")]

So given the above example when the assembly version is 1.0 the following will be injected

    [Obsolete("Custom Message. Please use 'NewClass' instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.")]
    public class ClassToMark{}

### Treat As Error Mode

When the target assembly version is greater than `RemoveInVersion` but less than `TreatAsErrorFromVersion` an `ObsoleteAttribute` with "treat as error" will be injected. It will have the following format

    [Obsolete("MESSAGE. Please use 'REPLACED_WITH' instead. Will be removed in version WILL_BE_REMOVED_IN_VERSION.", true)]

So given the above example when the assembly version is 3.0 the following will be injected

    [Obsolete("Custom Message. Please use 'NewClass' instead. Will be removed in version 4.0.0.", true)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public class ClassToMark{}

### Build Error Mode

When the target assembly version is greater  than `TreatAsErrorFromVersion` a build error will be generated. It will have the following format

     Cannot process 'MEMBER_NAME'. The assembly version ASSEMBLY_VERSION is higher than version specified in 'RemoveInVersion' WILL_BE_REMOVED_IN_VERSION. The member should be removed or 'RemoveInVersion' increased.
    
So given the above example when the assembly version is 5.0 a compile error will be thrown with the following text

     Cannot process 'ClassToMark'. The assembly version 5.0.0 is higher than version specified in RemoveInVersion 4.0.0. The member should be removed or RemoveInVersion increased.

## The Message property 

The message property should only be used for useful information. The fact that it is a obsolete member does not need to be reiterated in the message.

**DO NOT**  use and of the following redundant messages

 * "Do not call this method"
 * "This method will be removed"
 * "This method is obsolete"

# Configuration Options

All configuration options are access by modifying the `Obsolete` node in `FodyWeavers.xml`

## HideObsoleteMembers

When this is `true` obsolete members will also have `[EditorBrowsable(EditorBrowsableState.Advanced)]` added to them.

*Defaults to `true`*

    <Obsolete HideObsoleteMembers='false'/>

## TreatAsErrorFormat

The string used when informing the user what version the member will be treated as an error.

*Defaults to  `Will be treated as an error from version '{0}'. `*

    <Obsolete TreatAsErrorFormat="Will be treated as an error from version '{0}'. "/>

## RemoveInVersionFormat

The string used when informing the user what version the member will be removed it.

*Defaults to  `Will be removed in version '{0}'. `*

    <Obsolete RemoveInVersionFormat="Will be removed in version '{0}'. "/>

## ReplacementFormat

The string used when informing the user of an alternative member to use instead of the obsolete member.

*Defaults to `Please use '{0}' instead. `*

    <Obsolete ReplacementFormat="Please use '{0}' instead. "/>

## StepType

Used in two cases

 * To derive `TreatAsErrorFromVersion` if `RemoveInVersion` is not defined.
 * To derive `RemoveInVersion` if `TreatAsErrorFromVersion` is not defined.   

*Defaults to  `Major`. Other options are `Minor` and `Patch`*

    <Obsolete StepType="Minor"/>

## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)


