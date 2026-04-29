namespace Qwestr.Modules.Gamification.Application.Abstractions.Services;

public interface ILevelCalculator
{
    int CalculateLevel(int totalXp);
    int XpRequiredForNextLevel(int totalXp);
}
