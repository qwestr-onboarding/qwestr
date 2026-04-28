namespace Qwestr.Shared.Kernel.Enums.Gamification;

public enum CoinTransactionSource
{
    Earned = 0,           // получение монет (от Step или Award)
    Purchase = 1,         // трата на ShopItem
    ManualAdjustment = 2  // ручная корректировка менеджером
}