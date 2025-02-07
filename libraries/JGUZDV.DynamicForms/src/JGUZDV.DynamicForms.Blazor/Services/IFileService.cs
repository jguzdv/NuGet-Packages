namespace JGUZDV.DynamicForms.Blazor.Services;

public interface IFileService
{
    /// <summary>
    /// Uploads a file to the server.
    /// </summary>
    /// <param name="stream">The stream containing the file data.</param>
    /// <param name="fileName">The name of the file to be uploaded.</param>
    /// <param name="identifier">A unique identifier for the files field.</param>
    /// <returns>A task that represents the asynchronous upload operation.</returns>
    public Task UploadFile(Stream stream, string fileName, string identifier);
}