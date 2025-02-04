namespace JGUZDV.Blazor.Components.Cropper;

/// <summary>
/// Represents a rectangle with two points.
/// </summary>
/// <param name="X1">The x coordinate of the upper left</param>
/// <param name="Y1">The y coordinate of the upper left</param>
/// <param name="X2">The x coordinate of the lower right</param>
/// <param name="Y2">The y coordinate of the lower right</param>
public record Rect(double X1, double Y1, double X2, double Y2)
{
    /// <summary>
    /// The height of the rectangle.
    /// </summary>
    public double H => Y2 - Y1;

    /// <summary>
    /// The width of the rectangle.
    /// </summary>
    public double W => X2 - X1;
}
