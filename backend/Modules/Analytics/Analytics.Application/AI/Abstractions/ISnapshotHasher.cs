using Qwestr.Modules.Analytics.Application.AI.Models;

namespace Qwestr.Modules.Analytics.Application.AI.Abstractions;

public interface ISnapshotHasher
{
    string ComputeHash(AnalyticsSnapshot snapshot);
}
