using System;

/// <summary>
/// Marks to Fody to skip program elements inspection for Obsolete replacement.
/// </summary>
[AttributeUsage(
    AttributeTargets.Delegate |
    AttributeTargets.Interface |
    AttributeTargets.Event |
    AttributeTargets.Field |
    AttributeTargets.Property |
    AttributeTargets.Method |
    AttributeTargets.Constructor |
    AttributeTargets.Enum |
    AttributeTargets.Struct |
    AttributeTargets.Class,
    Inherited = false)]
public sealed class DoNotWarnAboutObsoleteUsageAttribute : Attribute
{
}