using System.Security.AccessControl;
using BuildingBlocks.Core.Response;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Common.FileStorage;

public class CloudinaryStorageService : IStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _bucketName;

    public CloudinaryStorageService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _cloudinary = new Cloudinary(new Account
        {
            ApiKey = configuration["CloudinarySettings:ApiKey"],
            ApiSecret = configuration["CloudinarySettings:ApiSecret"],
            Cloud = configuration["CloudinarySettings:Cloud"]
        });

        _bucketName = configuration["CloudinarySettings:BucketName"] ?? webHostEnvironment.EnvironmentName;
    }

    public async Task<ApiResponse> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var publicId = ExtractPublicIdFromUrl(fileName);
        var resourceType = DetectResourceType(fileName);
        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = resourceType
        };

        var destroyResult = await _cloudinary.DestroyAsync(deletionParams);

        if (destroyResult.Result == "ok")
        {
            return ApiResponse.Success();
        }

        var errorMessage = destroyResult.Error != null
            ? destroyResult.Error.Message
            : "Unknown error occurred while deleting the file.";

        return ApiResponse.Error(errorMessage);
    }
    public async Task<ApiResponse> DeleteFileAsync(List<string> fileNames, CancellationToken cancellationToken = default)
    {
        var filesWithType = fileNames
       .Select(url => new
       {
           PublicId = ExtractPublicIdFromUrl(url),
           ResourceType = DetectResourceType(url)
       })
       .Where(x => !string.IsNullOrWhiteSpace(x.PublicId))
       .GroupBy(x => x.ResourceType);

        if (!filesWithType.Any())
        {
            return ApiResponse.Error("No valid public IDs found.");
        }

        var failedItems = new List<string>();

        foreach (var group in filesWithType)
        {
            var deletionParams = new DelResParams
            {
                PublicIds = group.Select(x => x.PublicId).ToList(),
                ResourceType = group.Key
            };

            var destroyResult = await _cloudinary.DeleteResourcesAsync(deletionParams);

            var groupFailures = destroyResult.Deleted
                .Where(kvp => kvp.Value != "deleted" && kvp.Value != "not_found")
                .Select(kvp => $"{kvp.Key} ({group.Key}): {kvp.Value}");

            failedItems.AddRange(groupFailures);
        }

        return failedItems.Count == 0
            ? ApiResponse.Success()
            : ApiResponse.Error("Some files failed to delete: " + string.Join(", ", failedItems));
    }
    public string GetFileUrl(string fileName)
    {
        return _cloudinary.Api.UrlImgUp
            .Secure(true)
            .BuildUrl(string.Concat(_bucketName, "/", fileName));
    }

    public async Task<string> SaveFileAsync(Stream mediaBinaryStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, mediaBinaryStream),
            PublicId = fileName,
            Folder = _bucketName
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        return result.SecureUrl.ToString();
    }

    public async Task<string> SaveFileAsync(IFormFile file, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, file.OpenReadStream()),
            PublicId = fileName,
            Folder = _bucketName
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        return result.SecureUrl.ToString();
    }

    public async Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var fileName = Guid.NewGuid().ToString("N");
        return await SaveFileAsync(file, $"{fileName}", cancellationToken);
    }

    public async Task<string> SaveFileAsync(string base64String, CancellationToken cancellationToken = default)
    {
        var imageBytes = Convert.FromBase64String(base64String);
        var stream = new MemoryStream(imageBytes);
        var fileName = Guid.NewGuid();
        return await SaveFileAsync(stream, $"{fileName}", cancellationToken);
    }

    public async Task<List<string>> SaveFilesAsync(List<string> base64String, CancellationToken cancellationToken = default)
    {
        var tasks = base64String.Select(e => SaveFileAsync(e, cancellationToken));
        await Task.WhenAll(tasks);
        return tasks.Select(e => e.Result).ToList();
    }

    public async Task<List<string>> SaveFilesAsync(List<IFormFile> files, CancellationToken cancellationToken = default)
    {
        var tasks = files.Select(e => SaveFileAsync(e, cancellationToken));
        await Task.WhenAll(tasks);
        return tasks.Select(e => e.Result).ToList();
    }
    public static string ExtractPublicIdFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return url;

        try
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');

            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex == -1 || uploadIndex + 1 >= segments.Length)
                return string.Empty;

            var publicIdSegments = segments.Skip(uploadIndex + 1).ToList();

            if (publicIdSegments[0].StartsWith("v") && long.TryParse(publicIdSegments[0].Substring(1), out _))
            {
                publicIdSegments.RemoveAt(0);
            }

            var filename = string.Join("/", publicIdSegments);
            var lastDot = filename.LastIndexOf('.');
            if (lastDot >= 0)
            {
                filename = filename.Substring(0, lastDot);
            }

            return filename;
        }
        catch
        {
            return string.Empty;
        }
    }
    private static CloudinaryDotNet.Actions.ResourceType DetectResourceType(string urlOrFilename)
    {
        if (urlOrFilename.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
            urlOrFilename.EndsWith(".mov", StringComparison.OrdinalIgnoreCase) ||
            urlOrFilename.Contains("/video/upload"))
        {
            return CloudinaryDotNet.Actions.ResourceType.Video;
        }

        if (urlOrFilename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
            urlOrFilename.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
        {
            return CloudinaryDotNet.Actions.ResourceType.Raw;
        }

        return CloudinaryDotNet.Actions.ResourceType.Image; // default
    }
}
