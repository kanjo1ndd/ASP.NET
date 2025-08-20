namespace ASP_SPR311.Services.AzureStorage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;

public class CloudFileStorageService : ICloudStorageService
{
    private const string directoryName = "B:\\Step\\Lessons\\C_sharp\\weak56\\";
    private const string connectionString =
        "";

    private const string containerName = "big-images";
    private const string queueName = "new-images";

    private BlobServiceClient blobService;
    private BlobContainerClient blobContainer;

    public void ConnectCloud()
    {
        blobService = new BlobServiceClient(connectionString);
        blobContainer = blobService.GetBlobContainerClient(containerName);
        blobContainer.CreateIfNotExists();
        blobContainer.SetAccessPolicy(PublicAccessType.BlobContainer);
    }

    public string GetPath(string name)
    {
        ConnectCloud();
        BlobClient blob = blobContainer.GetBlobClient(name);
        return blob.Uri.AbsoluteUri + name;
    }

    public string SaveFile(IFormFile formFile)
    {
        ConnectCloud();
        string savedName = formFile.FileName;
        string fullName = directoryName + savedName;
        BlobClient createdBlob = blobContainer.GetBlobClient(savedName);
        QueueServiceClient queueService = new QueueServiceClient(connectionString);
        QueueClient queueClient = queueService.GetQueueClient(queueName);
        queueClient.CreateIfNotExists();
        queueClient.SendMessage(savedName);
        using (FileStream fs = File.OpenRead(fullName))
        {
            createdBlob.Upload(fs);
        }

        return createdBlob.Uri.AbsoluteUri;
    }
}
