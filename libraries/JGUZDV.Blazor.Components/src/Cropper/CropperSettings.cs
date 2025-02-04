namespace JGUZDV.Blazor.Components.Cropper;

/// <summary>
/// Represents the settings for the cropper.
/// </summary>
/// <param name="ResizeTo">Target size</param>
/// <param name="OutputFormat">Target image format (default: jpeg)</param>
/// <param name="ImageQuality">The quality of the image (default: 0.92)</param>
/// <param name="EnsureAspectRatio">Will ensure the aspect ratio of the crop (defaults to true)</param>
/// <param name="BackgroundColor">Sets the background color for cropping (defaults to "inherit")</param>
/// <param name="HideResizeSquares">Will disable the resize handles (defaults to false)</param>
public record CropperSettings(
        Size ResizeTo,
        string OutputFormat = "jpeg",
        double ImageQuality = 0.92,

        bool EnsureAspectRatio = true,

        string BackgroundColor = "Inherit",
        bool HideResizeSquares = false
    )
{
    /// <summary>
    /// The aspect ratio of the image. This is calculated from the ResizeTo property.
    /// </summary>
    public double AspectRatio { get; } = ResizeTo.Width / ResizeTo.Height;

    /// <summary>
    /// The quality of the image. Must be between 0 and 1. This parameter might be ignored by the browser.
    /// </summary>
    public double ImageQuality { get; init; } = ImageQuality > 1
        ? Math.Clamp(ImageQuality / 100.0, 0.0, 1.0)
        : Math.Clamp(ImageQuality, 0, 1);
}
