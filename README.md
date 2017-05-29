[![Chat on Gitter Status](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/Obsolete.Fody.svg?style=flat)](https://www.nuget.org/packages/Obsolete.Fody/)

## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/Obsolete/master/Icons/package_icon.png)

Helps keep usages of [ObsoleteAttribute](http://msdn.microsoft.com/en-us/library/fwz0y5c2) consistent.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)


## The nuget package

https://nuget.org/packages/Obsolete.Fody/

    PM> Install-Package Obsolete.Fody


### Your Code

    [ObsoleteEx(
                Message = "Custom message.", 
                TreatAsErrorFromVersion = "2.0", 
                RemoveInVersion = "4.0", 
                ReplacementTypeOrMember = "NewClass")]
    public class ClassToMark {}


### Treat As Warning Mode

When the target assembly version is less than `RemoveInVersion` an `ObsoleteAttribute` with "treat as warning" will be injected. It will have the following format

    [Obsolete("MESSAGE. Use 'REPLACED_WITH' instead. Will be treated as an error from version MARK_AS_VERSION_IN. Will be removed in version WILL_BE_REMOVED_IN_VERSION.")]

So given the above example when the assembly version is 1.0 the following will be injected

    [Obsolete("Custom Message. Use 'NewClass' instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.")]
    public class ClassToMark{}


### Treat As Error Mode

When the target assembly version is greater than `RemoveInVersion` but less than `TreatAsErrorFromVersion` an `ObsoleteAttribute` with "treat as error" will be injected. It will have the following format

    [Obsolete("MESSAGE. Use 'REPLACED_WITH' instead. Will be removed in version WILL_BE_REMOVED_IN_VERSION.", true)]

So given the above example when the assembly version is 3.0 the following will be injected

    [Obsolete("Custom Message. Use 'NewClass' instead. Will be removed in version 4.0.0.", true)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public class ClassToMark{}


### Build Error Mode

When the target assembly version is greater  than `TreatAsErrorFromVersion` a build error will be generated. It will have the following format

     Cannot process 'MEMBER_NAME'. The assembly version ASSEMBLY_VERSION is higher than version specified in 'RemoveInVersion' WILL_BE_REMOVED_IN_VERSION. The member should be removed or 'RemoveInVersion' increased.

So given the above example when the assembly version is 5.0 a compile error will be thrown with the following text

     Cannot process 'ClassToMark'. The assembly version 5.0.0 is higher than version specified in RemoveInVersion 4.0.0. The member should be removed or RemoveInVersion increased.


## The Message property 

The message property should only be used for useful information. The fact that it is obsoleted does not need to be reiterated in the message.

**DO NOT**  use any of the following redundant messages

 * "Do not call this method"
 * "This method will be removed"
 * "This method is obsolete"
 * "The replacement method is"


# Configuration Options

All configuration options are access by modifying the `Obsolete` node in `FodyWeavers.xml`


## HideObsoleteMembers

When this is `true` obsolete members will also have `[EditorBrowsable(EditorBrowsableState.Advanced)]` added to them.

*Defaults to `true`*

    <Obsolete HideObsoleteMembers='false'/>


## TreatAsErrorFormat

The string used when informing the user what version the member will be treated as an error.

*Defaults to `Will be treated as an error from version {0}. `*

    <Obsolete TreatAsErrorFormat="Will be treated as an error from version {0}. "/>


## ThrowsNotImplementedText

The string used when informing the user when the member currently throws a [NotImplementedException](https://msdn.microsoft.com/en-us/library/system.notimplementedexception.aspx).

*Defaults to `The member currently throws a NotImplementedException. `*

    <Obsolete ThrowsNotImplementedText="The member currently throws a NotImplementedException. "/>


## RemoveInVersionFormat

The string used when informing the user what version the member will be removed it.

*Defaults to `Will be removed in version {0}. `*

    <Obsolete RemoveInVersionFormat="Will be removed in version {0}. "/>


## ReplacementFormat

The string used when informing the user of an alternative member to use instead of the obsolete member.

*Defaults to `Use {0} instead. `*

    <Obsolete ReplacementFormat="Use {0} instead. "/>


## StepType

Used in two cases

 * To derive `TreatAsErrorFromVersion` if `RemoveInVersion` is not defined.
 * To derive `RemoveInVersion` if `TreatAsErrorFromVersion` is not defined.   

*Defaults to  `Major`. Other options are `Minor` and `Patch`*

    <Obsolete StepType="Minor"/>


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)
