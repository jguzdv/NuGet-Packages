namespace JGUZDV.Blazor.Components.Cropper;

/// <summary>
/// Represents the information of a drag operation.
/// </summary>
/// <param name="Origin"></param>
/// <param name="Cropper"></param>
/// <param name="MoveType"></param>
/// <param name="Direction"></param>
public record DragInfo(
        Vector Origin,
        Rect Cropper,
        MoveType MoveType,
        DragDirections Direction = default);
