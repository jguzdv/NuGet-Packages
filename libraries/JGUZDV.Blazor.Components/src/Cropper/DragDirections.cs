namespace JGUZDV.Blazor.Components.Cropper;

/// <summary>
/// Represents the directions in which an element can be dragged.
/// </summary>
[Flags]
public enum DragDirections
{
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0,

    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,

    UpLeft = Up | Left,
    UpRight = Up | Right,
    DownLeft = Down | Left,
    DownRight = Down | Right
    #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
