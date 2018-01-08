using System;

/// <summary>
/// Marks the program elements that are no longer in use.
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
public sealed class ObsoleteExAttribute : Attribute
{
    /// <summary>
    /// The text string that describes alternative workarounds.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// An information Version telling the user when the Member will be removed.
    /// </summary>
    /// <remarks>
    /// When the assembly version is equal to or higher than this value the <see cref="ObsoleteAttribute.IsError"/> will be marked to true.
    /// Must be convertible to a <see cref="Version"/>.
    /// </remarks>
    public string TreatAsErrorFromVersion { get; set; }

    /// <summary>
    /// An information Version telling the user when the Member will be removed.
    /// </summary>
    /// <remarks>
    /// If the assembly version is equal to or higher than this value then a compile error will thrown since it should not exist in the assembly anymore.
    /// Must be convertible to a <see cref="Version"/>.
    /// </remarks>
    public string RemoveInVersion { get; set; }

    /// <summary>
    /// A value pointing to the name of the replacement member if available.
    /// </summary>
    public string ReplacementTypeOrMember { get; set; }
}