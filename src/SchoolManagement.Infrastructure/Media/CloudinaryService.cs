using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Configuration;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Media;

/// <summary>
/// Stores profile pictures and student documents in Cloudinary. Images are
/// uploaded as image resources; everything else (PDFs, etc.) as raw/auto.
/// </summary>
public sealed class CloudinaryService : IMediaStorage
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinaryOptions> options)
    {
        var o = options.Value;
        var account = new Account(o.CloudName, o.ApiKey, o.ApiSecret);
        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }

    public async Task<MediaUploadResponse> UploadAsync(Stream content, string fileName, string folder, CancellationToken ct = default)
    {
        var isImage = IsImage(fileName);

        if (isImage)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, content),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };
            var result = await _cloudinary.UploadAsync(uploadParams, ct);
            EnsureSuccess(result);
            return new MediaUploadResponse { Url = result.SecureUrl.ToString(), PublicId = result.PublicId };
        }
        else
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, content),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };
            var result = await _cloudinary.UploadAsync(uploadParams, "auto", ct);
            EnsureSuccess(result);
            return new MediaUploadResponse { Url = result.SecureUrl.ToString(), PublicId = result.PublicId };
        }
    }

    private static void EnsureSuccess(RawUploadResult result)
    {
        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");
    }

    private static bool IsImage(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp";
    }
}
