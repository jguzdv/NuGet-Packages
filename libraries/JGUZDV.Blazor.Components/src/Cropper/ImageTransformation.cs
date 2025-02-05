namespace JGUZDV.Blazor.Components.Cropper;

/// <summary>
/// Represents the transformation of an image.
/// </summary>
public class ImageTransformation
{
    /// <summary>
    /// The scale of the image. Default is 1.
    /// </summary>
    public int Scale { get; set; } = 1;

    /// <summary>
    /// The rotation of the image in degree. Default is 0.
    /// </summary>
    public int Rotate { get; set; } = 0;

    /// <summary>
    /// The horizontal flip of the image. Default is false.
    /// </summary>
    public bool FlipHorizontal { get; set; }

    /// <summary>
    /// The vertical flip of the image. Default is false.
    /// </summary>
    public bool FlipVertical { get; set; }
}