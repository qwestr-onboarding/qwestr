using Microsoft.AspNetCore.Hosting;
using Qwestr.Modules.Identity.Application.Abstractions.Services;

namespace Qwestr.Modules.Identity.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _webRootPath;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _webRootPath = env.WebRootPath
            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }

    public async Task<string> UploadAsync(string folder, string fileName, Stream stream, CancellationToken ct = default)
    {
        var directory = Path.Combine(_webRootPath, "uploads", folder);
        Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, fileName);
        await using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream, ct);

        return $"/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(string relativePath, CancellationToken ct = default)
    {
        var normalized = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webRootPath, normalized);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}