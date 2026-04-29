using Qwestr.Modules.Gamification.Application.Abstractions.Services;

namespace Qwestr.Modules.Gamification.Infrastructure.Services;

public class LevelCalculator : ILevelCalculator
{
    public int CalculateLevel(int totalXp)
    {
        var raw = (int)Math.Floor(Math.Sqrt(totalXp / 100.0));
        return Math.Max(1, raw);
    }

    public int XpRequiredForNextLevel(int totalXp)
    {
        var current = CalculateLevel(totalXp);
        var nextLevelThreshold = (current + 1) * (current + 1) * 100;
        return nextLevelThreshold - totalXp;
    }
}
