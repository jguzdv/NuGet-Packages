using Microsoft.AspNetCore.Components;

namespace JGUZDV.DynamicForms.Blazor.Services;


public class DefaultFileService : IFileService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly NavigationManager _navigationManager;

    /// <summary>
    /// Gets or sets the format of the URL path for uploading files. <br />
    /// {0} is identifier of corresponding field <br />
    /// {1} is file name
    /// </summary>
    public static string FileServiceFormatUrlPath { get; set; } = "api/upload?identifier={0}&fileName={1}";

    public DefaultFileService(
        IHttpClientFactory clientFactory, 
        NavigationManager navigationManager)
    {
        _clientFactory = clientFactory;
        _navigationManager = navigationManager;
    }

    public async Task UploadFile(Stream stream, string fileName, string fileIdentifier)
    {
        var client = _clientFactory.CreateClient();

        var streamContent = new StreamContent(stream);
        var response = await client.PostAsync($"{_navigationManager.BaseUri}{string.Format(FileServiceFormatUrlPath, fileIdentifier, fileName)}", streamContent);
    }
}
