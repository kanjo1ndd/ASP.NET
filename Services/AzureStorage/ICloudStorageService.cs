namespace ASP_SPR311.Services.AzureStorage
{
    public interface ICloudStorageService
    {
        string SaveFile(IFormFile formFile);
        string GetPath(string name);
    }
}
