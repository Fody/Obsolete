# <img src="/package_icon.png" height="30px"> Obsolete.Fody

[![NuGet Status](https://img.shields.io/nuget/v/Obsolete.Fody.svg)](https://www.nuget.org/packages/Obsolete.Fody/)

Helps keep usages of [ObsoleteAttribute](http://msdn.microsoft.com/en-us/library/fwz0y5c2) consistent.

**See [Milestones](../../milestones?state=closed) for release notes.**


### This is an add-in for [Fody](https://github.com/Fody/Home/)

**It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059). [See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.**


## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).


### NuGet installation

Install the [Obsolete.Fody NuGet package](https://nuget.org/packages/Obsolete.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package Obsolete.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<Obsolete/>` to [FodyWeavers.xml](https://github.com/Fody/Home/blob/master/pages/usage.md#add-fodyweaversxml)

```xml
<Weavers>
  <Obsolete/>
</Weavers>
```


### Your Code

```csharp
[ObsoleteEx(
    Message = "Custom message.", 
    TreatAsErrorFromVersion = "2.0", 
    RemoveInVersion = "4.0", 
    ReplacementTypeOrMember = "NewClass")]
public class ClassToMark {}
```


### Treat As Warning Mode

When the target assembly version is less than `RemoveInVersion` an `ObsoleteAttribute` with "treat as warning" will be injected. It will have the following format

```csharp
[Obsolete("MESSAGE. Use 'REPLACED_WITH' instead. Will be treated as an error from version MARK_AS_VERSION_IN. Will be removed in version WILL_BE_REMOVED_IN_VERSION.")]
```

So given the above example when the assembly version is 1.0 the following will be injected

```csharp
[Obsolete("Custom Message. Use 'NewClass' instead. Will be treated as an error from version 2.0.0. Will be removed in version 4.0.0.")]
    public class ClassToMark{}
```


### Treat As Error Mode

When the target assembly version is greater than `RemoveInVersion` but less than `TreatAsErrorFromVersion` an `ObsoleteAttribute` with "treat as error" will be injected. It will have the following format

```csharp
[Obsolete("MESSAGE. Use 'REPLACED_WITH' instead. Will be removed in version WILL_BE_REMOVED_IN_VERSION.", true)]
```

So given the above example when the assembly version is 3.0 the following will be injected

```csharp
[Obsolete("Custom Message. Use 'NewClass' instead. Will be removed in version 4.0.0.", true)]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public class ClassToMark{}
```


### Build Error Mode

When the target assembly version is greater  than `TreatAsErrorFromVersion` a build error will be generated. It will have the following format

> Cannot process 'MEMBER_NAME'. The assembly version ASSEMBLY_VERSION is higher than version specified in 'RemoveInVersion' WILL_BE_REMOVED_IN_VERSION. The member should be removed or 'RemoveInVersion' increased.


So given the above example when the assembly version is 5.0 a compile error will be thrown with the following text

> Cannot process 'ClassToMark'. The assembly version 5.0.0 is higher than version specified in RemoveInVersion 4.0.0. The member should be removed or RemoveInVersion increased.


## Version convention

By default the prefix of [AssemblyInformationalVersionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assemblyinformationalversionattribute) will be used, if it can be parsed to a version. Otherwise the [assembly version](https://docs.microsoft.com/en-us/dotnet/standard/assembly/versioning) is used.


## The Message property

The message property should only be used for useful information. The fact that it is obsoleted does not need to be reiterated in the message.

**DO NOT** use any of the following redundant messages

 * "Do not call this method"
 * "This method will be removed"
 * "This method is obsolete"
 * "The replacement method is"


## Configuration Options

All configuration options are access by modifying the `Obsolete` node in `FodyWeavers.xml`


### HideObsoleteMembers

When this is `advanced` or `true` obsolete members will also have `[EditorBrowsable(EditorBrowsableState.Advanced)]` added to them.

*Defaults to `advanced`*

When this is `never` obsolete members will also have `[EditorBrowsable(EditorBrowsableState.Never)]` added to them.

When this is `off` or `false` obsolete members will not have `[EditorBrowsable]` added to them.

```xml
<Obsolete HideObsoleteMembers='false'/>
```


### TreatAsErrorFormat

The string used when informing the user what version the member will be treated as an error.

*Defaults to `Will be treated as an error from version {0}. `*

```xml
<Obsolete TreatAsErrorFormat="Will be treated as an error from version {0}. "/>
```


### ThrowsNotImplementedText

The string used when informing the user when the member currently throws a [NotImplementedException](https://msdn.microsoft.com/en-us/library/system.notimplementedexception.aspx).

*Defaults to `The member currently throws a NotImplementedException. `*

```xml
<Obsolete ThrowsNotImplementedText="The member currently throws a NotImplementedException. "/>
```


### RemoveInVersionFormat

The string used when informing the user what version the member will be removed it.

*Defaults to `Will be removed in version {0}. `*

```xml
<Obsolete RemoveInVersionFormat="Will be removed in version {0}. "/>
```


### ReplacementFormat

The string used when informing the user of an alternative member to use instead of the obsolete member.

*Defaults to `Use {0} instead. `*

```xml
<Obsolete ReplacementFormat="Use {0} instead. "/>
```


### StepType

Used in two cases

 * To derive `TreatAsErrorFromVersion` if `RemoveInVersion` is not defined.
 * To derive `RemoveInVersion` if `TreatAsErrorFromVersion` is not defined.

*Defaults to  `Major`. Other options are `Minor` and `Patch`*

```xml
<Obsolete StepType="Minor"/>
```


### Mute warnings about Obsolete usage

For `ObsoleteAttribute` is used and should not be replaced, use `DoNotWarnAboutObsoleteUsageAttribute` to mute Fody warnings during build time.

```csharp
[Obsolete]
[DoNotWarnAboutObsoleteUsage]
public class LegacyCode {}
```


## Icon

Icon courtesy of [The Noun Project](https://thenounproject.com)