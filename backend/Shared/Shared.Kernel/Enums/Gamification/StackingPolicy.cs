namespace Qwestr.Shared.Kernel.Enums.Gamification;

public enum StackingPolicy
{
    Unique = 0,    // один экземпляр на игрока, повторная покупка запрещена
    Stackable = 1  // накапливается Quantity
}