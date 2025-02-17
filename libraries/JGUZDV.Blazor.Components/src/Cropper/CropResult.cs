namespace JGUZDV.Blazor.Components.Cropper;

/// <summary>
/// Represents the result of a crop operation.
/// </summary>
/// <param name="CroppedSize">The actual size of the cropped image</param>
/// <param name="ContentType">The content type, e.g. image/jpg</param>
/// <param name="Base64Content">The cropped image as base64.</param>
public record CropResult(
        Size CroppedSize,
        string ContentType,
        string Base64Content
    );
