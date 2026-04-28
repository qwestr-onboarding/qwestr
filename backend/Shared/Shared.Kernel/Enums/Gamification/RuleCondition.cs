namespace Qwestr.Shared.Kernel.Enums.Gamification;

public enum RuleCondition
{
    Count = 0,           // N раз произошло
    CountWithFilter = 1, // N раз с фильтром (FilterParams)
    Threshold = 2,       // достичь значения (уровень >= 5)
    Streak = 3           // N раз подряд
}