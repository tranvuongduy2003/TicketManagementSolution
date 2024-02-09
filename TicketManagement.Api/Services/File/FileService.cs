using Azure.Storage;
using Azure.Storage.Blobs;
using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Services;

public class FileService
{
    private readonly IConfiguration _configuration;
    private readonly string _storageAccount;
    private readonly string _key;
    private readonly string _containerName;
    private readonly string _connectionString;
    private readonly BlobContainerClient _filesContainer;

    public FileService(IConfiguration configuration)
    {
        _configuration = configuration;
        _storageAccount = _configuration.GetValue<string>("AzureBlobStorage:StorageAccount");
        _key = _configuration.GetValue<string>("AzureBlobStorage:Key");
        _containerName = _configuration.GetValue<string>("AzureBlobStorage:ContainerName");
        _connectionString = _configuration.GetValue<string>("AzureBlobStorage:ConnectionString");

        var credential = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        _filesContainer = blobServiceClient.GetBlobContainerClient(_containerName);
    }

    public async Task<List<BlobDto>> ListAsync()
    {
        List<BlobDto> files = new List<BlobDto>();

        await foreach (var file in _filesContainer.GetBlobsAsync())
        {
            string uri = _filesContainer.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            files.Add(new BlobDto
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType
            });
        }

        return files;
    }

    public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
    {
        BlobResponseDto response = new();
        BlobClient client = _filesContainer.GetBlobClient(blob.FileName);

        if (await client.ExistsAsync())
        {
            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;
        }
        else
        {
            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;
        }

        return response;
    }

    public async Task<BlobDto?> DownloadAsync(string blobFilename)
    {
        BlobClient file = _filesContainer.GetBlobClient(blobFilename);

        if (await file.ExistsAsync())
        {
            var data = await file.OpenReadAsync();
            Stream blobContent = data;

            var content = await file.DownloadContentAsync();

            string name = blobFilename;
            string contentType = content.Value.Details.ContentType;

            return new BlobDto { Content = blobContent, Name = name, ContentType = contentType };
        }

        return null;
    }

    public async Task<BlobResponseDto> DeleteAsync(string blobFilename)
    {
        BlobClient file = _filesContainer.GetBlobClient(blobFilename);

        await file.DeleteAsync();

        return new BlobResponseDto { Error = false, Status = $"File: {blobFilename} has been successfully deleted" };
    }
}