using Microsoft.AspNetCore.Components.Web;

namespace JGUZDV.Blazor.Components.Cropper;

/// <summary>
/// Represents a vector.
/// </summary>
/// <param name="X">The x coordinate</param>
/// <param name="Y">The y coordinate</param>
public record Vector(double X, double Y)
{
    internal static Vector FromEvent(EventArgs e)
    {
        if (e is MouseEventArgs m)
            return new Vector(m.ClientX, m.ClientY);
        if (e is TouchEventArgs t)
            return new Vector(t.TargetTouches[0].ClientX, t.TargetTouches[0].ClientY);

        throw new InvalidOperationException($"e needs to be MouseEventArgs or TouchEventArgs");
    }

    internal static Vector FromDirection(DragDirections direction, int stepSize)
    {
        var x = direction switch
        {
            DragDirections.Left => -stepSize,
            DragDirections.Right => stepSize,
            _ => 0
        };
        var y = direction switch
        {
            DragDirections.Up => -stepSize,
            DragDirections.Down => stepSize,
            _ => 0
        };

        return new Vector(x, y);
    }
}
