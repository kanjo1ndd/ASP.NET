namespace ASP_SPR311.Services.Storage
{
    public interface IStorageService
    {
        String SaveFile(IFormFile formFile);
        String GetRealPath(String name);
    }
}
