using Qwestr.Modules.Gamification.Application.Abstractions.Services;

namespace Qwestr.Modules.Gamification.Infrastructure.Services;

public class LevelCalculator : ILevelCalculator
{
    // Необходимое кол-во 
    public int CalculateLevel(int totalXp)
    {
        var level = 1;
        var accumulated = 0;
        var threshold = level * (level + 1) / 2 * 100;
    
        while (accumulated + threshold <= totalXp)
        {
            accumulated += threshold;
            level++;
            threshold = level * (level + 1) / 2 * 100;
        }
    
        return level;
    }
    
    public int XpRequiredForNextLevel(int totalXp)
    {
        var level = CalculateLevel(totalXp);
        var accumulated = Enumerable.Range(1, level - 1)
            .Sum(n => n * (n + 1) / 2 * 100);
        var nextThreshold = level * (level + 1) / 2 * 100;
    
        return accumulated + nextThreshold - totalXp;
    }
}
