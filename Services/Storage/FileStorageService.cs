namespace ASP_SPR311.Services.Storage
{
    public class FileStorageService : IStorageService
    {
        private const String storagePath = "B:\\Step\\Lessons\\C_sharp\\weak56\\ASP311";
        public string GetRealPath(string name)
        {
            return storagePath + name;
        }
        public string SaveFile(IFormFile formFile)
        {
            var ext = Path.GetExtension(formFile.FileName);
            String savedName;
            String fullName;
            do
            {
                savedName = Guid.NewGuid() + ext;
                fullName = storagePath + savedName;
            } while (File.Exists(fullName));

            formFile.CopyTo(new FileStream(fullName, FileMode.CreateNew));

            return savedName;
        }
    }
}
