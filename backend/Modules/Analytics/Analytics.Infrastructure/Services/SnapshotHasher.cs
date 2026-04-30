using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.AI.Models;

namespace Qwestr.Modules.Analytics.Infrastructure.Services;

public class SnapshotHasher : ISnapshotHasher
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = false };

    public string ComputeHash(AnalyticsSnapshot snapshot)
    {
        var json = JsonSerializer.Serialize(snapshot, Options);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
