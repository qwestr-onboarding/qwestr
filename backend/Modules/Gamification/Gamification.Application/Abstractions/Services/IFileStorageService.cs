namespace Qwestr.Modules.Gamification.Application.Abstractions.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(string folder, string fileName, Stream stream, CancellationToken ct = default);
    Task DeleteAsync(string relativePath, CancellationToken ct = default);
}
