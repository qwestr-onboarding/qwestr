# Gamification — план реализации Application / Infrastructure / Api

Документ для Claude Code. Реализуй модуль `Gamification` по аналогии с уже готовым модулем `Onboarding`. Доменный слой написан вручную и менять его в рамках этого плана не нужно — только если найдёшь явное расхождение, тогда сначала уточни.

## Контекст

- Доменный слой `Qwestr.Modules.Gamification.Domain` уже реализован
- Эталон структуры и стиля — модуль `Onboarding` (его и копируй по форме)
- Все правила из корневого `CLAUDE.md` обязательны: Result вместо exceptions, var, фигурные скобки в if/else, snake_case в БД, schema-per-module, фильтр по TenantId везде, никаких прямых ссылок на другие модули
- Расширений NuGet-пакетов без явного запроса не добавлять

## Доменные сущности (для контекста)

**Агрегаты:** `PlayerProfile` (содержит VO `Equipment`), `LevelTier`, `Award` (содержит VO `AwardRule`), `ShopItem`.

**Прочие сущности:** `PointsTransaction`, `CoinTransaction`, `AchievementProgress`, `EarnedAward`, `InventoryItem`.

**Доменные события:** `PlayerLevelUpEvent`, `AwardEarnedEvent`, `ItemPurchasedEvent`, `ItemEquippedEvent`, `CoinsAwardedEvent`.

**Ключевые инварианты:**
- Идемпотентность XP-начислений: уникальный индекс `PointsTransaction(ProfileId, SourceType, SourceId)`
- Один Award на игрока: уникальный индекс `EarnedAward(ProfileId, AwardId)`
- Один прогресс на пару игрок-награда: уникальный индекс `AchievementProgress(ProfileId, AwardId)`
- Один InventoryItem на пару игрок-предмет: уникальный индекс `InventoryItem(ProfileId, ShopItemId)`
- Manual-Award не имеет Rule, Automatic — обязан иметь
- Расходники (`InventorySlot.Consumable`) не экипируются и всегда `Stackable`

---

# Слой Application

Проект: `Qwestr.Modules.Gamification.Application`. Зависимости: `Domain` + `Shared.Contracts` + `Shared.Kernel`. **Никаких ссылок на Infrastructure.**

## Структура папок

```
Application/
├── Abstractions/                — интерфейсы для Infrastructure
│   ├── Persistence/
│   │   ├── IGamificationDbContext.cs
│   │   ├── IPlayerProfileRepository.cs
│   │   ├── IAwardRepository.cs
│   │   ├── IShopItemRepository.cs
│   │   ├── ILevelTierRepository.cs
│   │   └── ... (по одному репозиторию на агрегат)
│   └── Services/
│       ├── ILevelCalculator.cs
│       └── ITenantConfigurationProvider.cs   — курс XP→Coins per-tenant
│
├── Behaviors/                   — pipeline behaviors MediatR (валидация и т.п.)
│
├── Common/
│   ├── Mappings/                — Mapster-конфигурации
│   └── Extensions/
│
├── Profiles/                    — операции над PlayerProfile
│   ├── Queries/
│   │   ├── GetMyProfile/
│   │   └── GetProfileByUserId/
│   └── Commands/
│       └── (создание профиля — внутреннее, через handler UserRegisteredEvent)
│
├── Levels/
│   ├── Queries/
│   │   └── GetLevelTiers/
│   └── Commands/
│       ├── CreateLevelTier/
│       ├── UpdateLevelTier/
│       └── DeleteLevelTier/
│
├── Awards/
│   ├── Queries/
│   │   ├── GetAwards/
│   │   ├── GetAwardById/
│   │   └── GetEarnedAwardsByProfile/
│   └── Commands/
│       ├── CreateAutomaticAward/
│       ├── CreateManualAward/
│       ├── UpdateAward/
│       ├── DeactivateAward/
│       └── IssueManualAward/         — менеджер выдаёт бейдж вручную
│
├── Shop/
│   ├── Queries/
│   │   ├── GetShopItems/             — для игрока, с признаками "доступно"/"уже куплено"
│   │   └── GetShopItemsForManagement/ — для менеджера, без фильтрации
│   └── Commands/
│       ├── CreateShopItem/
│       ├── UpdateShopItem/
│       ├── DeactivateShopItem/
│       └── PurchaseShopItem/
│
├── Inventory/
│   ├── Queries/
│   │   └── GetMyInventory/
│   └── Commands/
│       ├── EquipItem/
│       └── UnequipItem/
│
├── Leaderboard/
│   └── Queries/
│       └── GetLeaderboard/           — параметры: period (AllTime/Month/Week), top N
│
├── Points/                       — внутренние сервисы, не endpoint'ы
│   ├── PointsService.cs          — реализация IPointsService
│   ├── IPointsService.cs
│   └── Models/
│       └── AwardPointsRequest.cs
│
├── Awarding/                     — движок проверки и выдачи Award
│   ├── AwardEvaluator.cs         — диспетчер
│   ├── IAwardEvaluator.cs
│   ├── Evaluators/               — реализации IRuleEvaluator
│   │   ├── IRuleEvaluator.cs
│   │   ├── StepCompletedCountEvaluator.cs
│   │   ├── ScenarioCompletedCountEvaluator.cs
│   │   ├── TestPassedWithFilterEvaluator.cs
│   │   ├── LevelThresholdEvaluator.cs
│   │   ├── PointsAccumulatedEvaluator.cs
│   │   └── DailyStreakEvaluator.cs
│   └── Models/
│       ├── EvaluationContext.cs  — что произошло (триггер + payload)
│       └── EvaluationResult.cs   — delta для прогресса
│
├── EventHandlers/                — обработчики междумодульных событий
│   ├── UserRegisteredEventHandler.cs              — создаёт PlayerProfile
│   ├── OnboardingStepCompletedEventHandler.cs     — главный обработчик: баллы + Award
│   ├── ScenarioCompletedEventHandler.cs           — если Onboarding такое событие шлёт
│   └── PlayerLevelUpEventHandler.cs               — внутренний: проверяет LevelReached-награды
│
├── Validators/                   — FluentValidation валидаторы команд
│
└── DependencyInjection.cs        — AddGamificationApplication()
```

## Соглашения CQRS

Все команды и запросы — `IRequest<Result<TResponse>>` или `IRequest<Result>`. Регистрируем MediatR через `AddMediatR(typeof(DependencyInjection).Assembly)`.

Для каждой команды/запроса один файл с:
- record-команда `XxxCommand : IRequest<Result<XxxResponse>>`
- record-response `XxxResponse(...)`
- класс `XxxCommandHandler : IRequestHandler<XxxCommand, Result<XxxResponse>>`
- класс `XxxCommandValidator : AbstractValidator<XxxCommand>` (если нужна валидация)

Везде, где нужен текущий пользователь и тенант — через `ICurrentUserService` из `Shared.Infrastructure` (в Application — через интерфейс `IUserProvider` из `Shared.Contracts`).

## Ключевые механики Application

### 1. PointsService

Интерфейс:
```
Task<Result> AwardAsync(Guid profileId, int xp, int coins, PointsSourceType sourceType, Guid sourceId, string reason, CancellationToken ct);
```

Логика:
1. Проверить идемпотентность: попытка найти `PointsTransaction(ProfileId, SourceType, SourceId)`. Если есть — `Result.Success()` (молча).
2. Загрузить `PlayerProfile`. Если нет — Failure (профиль создаётся отдельным handler'ом).
3. Вызвать `profile.AddXp(xp)` и `profile.AddCoins(coins)`.
4. Создать `PointsTransaction` (с XP+Coins для аудита) и `CoinTransaction` (Earned, для отчётности по монетам).
5. Пересчитать уровень через `ILevelCalculator.CalculateLevel(profile.TotalXp)`. Если изменился — `profile.RecalculateLevel(newLevel)`, опубликовать `PlayerLevelUpEvent`.
6. Сохранить.

**Курс XP→Coins** берётся из `ITenantConfigurationProvider.GetCoinsPerXpRate(tenantId)`. Дефолт 1.0 (1 XP = 1 Coin), если конфигурации нет. Реализация провайдера в Infrastructure — через таблицу настроек или просто константу на MVP.

### 2. AwardEvaluator

Главный диспетчер выдачи автоматических наград.

Интерфейс:
```
Task<IReadOnlyList<EarnedAward>> EvaluateAsync(EvaluationContext context, CancellationToken ct);
```

`EvaluationContext` содержит: `TenantId`, `ProfileId`, `RuleTrigger Trigger`, `Dictionary<string, object> Payload` (например, `{ "stepType": "Test", "score": 95 }`).

Логика:
1. Загрузить все активные `Award` тенанта (включая системные с `TenantId == null`) с `IssuanceType == Automatic` и `Rule.Trigger == context.Trigger`.
2. Для каждого такого Award:
   - Найти подходящий `IRuleEvaluator` по ключу `(Rule.Trigger, Rule.Condition)` через DI.
   - Вызвать `evaluator.EvaluateAsync(award, context)` → возвращает `EvaluationResult { delta: int, setValue: int? }`.
   - Найти или создать `AchievementProgress(profileId, awardId)` с `TargetValue = award.Rule.TargetValue`.
   - Применить delta (`Increment`) или setValue (`SetValue` для Threshold-условий).
   - Если прогресс перешёл в `IsCompleted` — создать `EarnedAward.CreateAutomatic(...)`, вызвать `PointsService.AwardAsync` для бонусов (XpBonus + CoinsBonus, source = AwardBonus, sourceId = awardId), опубликовать `AwardEarnedEvent`.
3. Вернуть список выданных наград.

### 3. IRuleEvaluator — стратегии

Каждый evaluator знает, как обработать одно сочетание `(Trigger, Condition)`. Регистрируется в DI под ключом или через Marker-интерфейс с метаданными.

```csharp
public interface IRuleEvaluator
{
    RuleTrigger Trigger { get; }
    RuleCondition Condition { get; }
    Task<EvaluationResult> EvaluateAsync(Award award, EvaluationContext ctx, CancellationToken ct);
}
```

Регистрация: `services.AddScoped<IRuleEvaluator, StepCompletedCountEvaluator>();` для всех. `AwardEvaluator` инжектит `IEnumerable<IRuleEvaluator>` и при поиске фильтрует по `(Trigger, Condition)`.

**Реализации (минимум для MVP):**

| Класс | Trigger | Condition | Что делает |
|---|---|---|---|
| `StepCompletedCountEvaluator` | StepCompleted | Count | Возвращает delta=1 |
| `ScenarioCompletedCountEvaluator` | ScenarioCompleted | Count | delta=1 |
| `TestPassedWithFilterEvaluator` | TestPassed | CountWithFilter | delta=1 если Payload удовлетворяет FilterParams (например, `score >= 90`) |
| `LevelThresholdEvaluator` | LevelReached | Threshold | setValue = текущий уровень из Payload |
| `PointsAccumulatedEvaluator` | PointsAccumulated | Threshold | setValue = текущий TotalXp из Payload |
| `DailyStreakEvaluator` | DailyActivity | Streak | delta=1 если активность подряд, иначе сброс прогресса в 0 (через `AchievementProgress.Reset` — добавь метод в домен, если решишь поддерживать; иначе пока удаляй и пересоздавай) |

**Важно:** `DailyStreakEvaluator` требует от домена возможности сбросить прогресс. Если такого метода в `AchievementProgress` нет — на MVP пропусти эту реализацию, оставь TODO. Не модифицируй домен сам, спроси.

### 4. OnboardingStepCompletedEventHandler — главный обработчик

Подписывается на событие из `Shared.Contracts.OnboardingStepCompletedEvent` (которое опубликовал модуль Onboarding).

Логика, в одной транзакции:
1. Получить или создать `PlayerProfile(tenantId, userId)`.
2. Вызвать `PointsService.AwardAsync` с XP=`event.Points`, Coins=`event.Points * coinsRate`.
3. Вызвать `AwardEvaluator.EvaluateAsync` с триггером `StepCompleted` и payload (stepId, scenarioId, stepType).
4. Если шаг был типа Test — дополнительно вызвать `EvaluateAsync` с триггером `TestPassed` (с информацией о результате).
5. Если в результате evaluation выданы новые Award с XpBonus/CoinsBonus — может потребоваться повторная evaluation (потому что бонус мог поднять `PointsAccumulated`-награду или `LevelReached`-награду). Реализуй цикл: повторять evaluation, пока `evaluator` возвращает непустой список новых наград, **максимум 3 итерации** (защита от рекурсии).

**Транзакционность:** используй `IUnitOfWork` или просто `IGamificationDbContext.SaveChangesAsync` — один коммит на всё. Доменные события публикуй **после** успешного коммита (стандартный паттерн через outbox или просто пост-коммит-хук в DbContext).

### 5. UserRegisteredEventHandler

Подписывается на `UserRegisteredEvent` из `Shared.Contracts`. Создаёт пустой `PlayerProfile(tenantId, userId)`. Идемпотентность — если профиль уже есть, выходит.

### 6. PurchaseShopItem command

Логика:
1. Загрузить `PlayerProfile` по `UserId` текущего пользователя.
2. Загрузить `ShopItem` по Id, проверить `TenantId == profile.TenantId`.
3. Вызвать `shopItem.CanBePurchasedBy(profile.CurrentLevel, profile.AvailableCoins)` — если Failure, вернуть.
4. Найти `InventoryItem(profileId, shopItemId)`:
   - Есть и `StackingPolicy.Unique` → Failure "AlreadyOwned"
   - Есть и `Stackable` → `IncreaseQuantity(1)`
   - Нет → создать новый `InventoryItem.Create(...)`
5. `profile.SpendCoins(price)` — Result проверить.
6. Создать `CoinTransaction.CreateSpent(...)`.
7. Сохранить, опубликовать `ItemPurchasedEvent`.

### 7. EquipItem / UnequipItem

EquipItem:
1. Загрузить `PlayerProfile` и `InventoryItem(profileId, shopItemId)`.
2. Если предмета в инвентаре нет → Failure.
3. Загрузить `ShopItem`, получить `Slot`.
4. `profile.Equip(slot, shopItemId)` — Result проверить (Consumable откажется).
5. Сохранить, опубликовать `ItemEquippedEvent`.

UnequipItem: принимает `Slot`, вызывает `profile.Unequip(slot)`.

### 8. GetLeaderboard query

Параметры: `LeaderboardPeriod` (AllTime / Month / Week), `int topN` (по умолчанию 10).

Логика:
- AllTime: `ORDER BY p.TotalXp DESC LIMIT N` по `PlayerProfile` тенанта.
- Month/Week: `SUM(XpAmount)` из `PointsTransaction` за период, GROUP BY ProfileId, ORDER BY DESC, LIMIT N. JOIN с `PlayerProfile` для метаданных.

Возвращай DTO с: позиция, UserId, отображаемое имя (получить через `IUserProvider` из Shared.Contracts), уровень, звание (по `LevelTier`), XP/период-XP.

Дополнительно: позиция текущего пользователя, если он не в топе. Реализуй отдельным запросом — RANK() в SQL или подсчёт COUNT(*) с большим XP.

## DependencyInjection.cs

```csharp
public static IServiceCollection AddGamificationApplication(this IServiceCollection services)
{
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
    services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
    services.AddScoped<IPointsService, PointsService>();
    services.AddScoped<IAwardEvaluator, AwardEvaluator>();
    // регистрация всех IRuleEvaluator
    services.AddScoped<IRuleEvaluator, StepCompletedCountEvaluator>();
    services.AddScoped<IRuleEvaluator, ScenarioCompletedCountEvaluator>();
    // ...
    return services;
}
```

---

# Слой Infrastructure

Проект: `Qwestr.Modules.Gamification.Infrastructure`. Зависимости: `Application` + `Domain` + `Shared.Infrastructure`.

## Структура папок

```
Infrastructure/
├── Persistence/
│   ├── GamificationDbContext.cs
│   ├── DesignTimeDbContextFactory.cs
│   ├── Configurations/
│   │   ├── PlayerProfileConfiguration.cs
│   │   ├── PointsTransactionConfiguration.cs
│   │   ├── CoinTransactionConfiguration.cs
│   │   ├── LevelTierConfiguration.cs
│   │   ├── AwardConfiguration.cs
│   │   ├── AchievementProgressConfiguration.cs
│   │   ├── EarnedAwardConfiguration.cs
│   │   ├── ShopItemConfiguration.cs
│   │   └── InventoryItemConfiguration.cs
│   ├── Repositories/
│   │   └── (один файл на репозиторий из Application/Abstractions)
│   ├── Migrations/
│   └── Seeders/
│       ├── SystemAwardsSeeder.cs       — системные Award (TenantId=null)
│       ├── DefaultLevelTiersSeeder.cs  — на регистрацию тенанта (вызывается из event handler'а)
│       └── DemoShopItemsSeeder.cs      — на регистрацию тенанта
│
├── Services/
│   ├── LevelCalculator.cs              — реализация ILevelCalculator
│   └── TenantConfigurationProvider.cs
│
└── DependencyInjection.cs              — AddGamificationInfrastructure()
```

## Конфигурации EF

Стандарт из проекта Onboarding: PostgreSQL, schema `gamification`, snake_case в БД, имена таблиц через `ToTable("player_profiles", "gamification")`.

**Особое внимание:**

1. **`PlayerProfile.Equipment` (VO)** — настроить как Owned Type или сериализовать в jsonb-колонку. Я бы предпочёл jsonb для простоты:
   ```
   builder.Property(p => p.Equipment)
       .HasConversion(
           v => JsonSerializer.Serialize(v, JsonOptions),
           v => JsonSerializer.Deserialize<Equipment>(v, JsonOptions) ?? Equipment.Empty())
       .HasColumnType("jsonb")
       .HasColumnName("equipment");
   ```
   Учти, что `Equipment` имеет приватный конструктор и static factory — сериализация/десериализация должна работать (если нет — добавь `[JsonConstructor]` атрибут на ctor или конвертер).

2. **`Award.Rule` (VO)** — аналогично jsonb. `FilterParams` (Dictionary) сериализуется естественно.

3. **`ShopItem.AssetData`** — уже строка с jsonb-разметкой, просто `.HasColumnType("jsonb")`.

4. **Уникальные индексы:**
   ```
   modelBuilder.Entity<PointsTransaction>()
       .HasIndex(t => new { t.ProfileId, t.SourceType, t.SourceId })
       .IsUnique()
       .HasDatabaseName("ix_points_transactions_idempotency");

   modelBuilder.Entity<EarnedAward>()
       .HasIndex(e => new { e.ProfileId, e.AwardId })
       .IsUnique();

   modelBuilder.Entity<AchievementProgress>()
       .HasIndex(p => new { p.ProfileId, p.AwardId })
       .IsUnique();

   modelBuilder.Entity<InventoryItem>()
       .HasIndex(i => new { i.ProfileId, i.ShopItemId })
       .IsUnique();

   modelBuilder.Entity<PlayerProfile>()
       .HasIndex(p => new { p.TenantId, p.UserId })
       .IsUnique();
   ```

5. **Индексы для производительности:**
   - `LevelTier(TenantId, Order)` — для отображения списка тиров
   - `PlayerProfile(TenantId, TotalXp DESC)` — для лидерборда
   - `PointsTransaction(TenantId, ProfileId, OccurredAt DESC)` — для лидерборда за период

6. **Enum'ы** — конвертируй в int (`builder.Property(x => x.SourceType).HasConversion<int>()`), не в строку. Это эталон из Onboarding.

7. **Filter Query по TenantId** — глобальный фильтр на каждой сущности с `TenantId`, через `HasQueryFilter(e => e.TenantId == _currentUser.TenantId)`. Кроме `Award`, где `TenantId` nullable — там фильтр сложнее: `e.TenantId == null || e.TenantId == _currentUser.TenantId`.

## Migrations

Создай первую миграцию `Initial_Gamification_Schema`. Команда:

```
dotnet ef migrations add Initial_Gamification_Schema \
  -p src/Modules/Gamification/Qwestr.Modules.Gamification.Infrastructure \
  -s src/Qwestr.Api
```

В `Up()` миграции должна быть инструкция создания schema `gamification` (обычно автоматически из конфига).

## Сидеры

**SystemAwardsSeeder** — вызывается на старте приложения, создаёт системные Award (`TenantId == null`):

| Code | Title | Trigger | Condition | Target | Bonus |
|---|---|---|---|---|---|
| `first_step` | Первый шаг | StepCompleted | Count | 1 | 10 XP, 5 Coins |
| `first_scenario` | Первый сценарий | ScenarioCompleted | Count | 1 | 50 XP, 25 Coins |
| `level_5` | Пятый уровень | LevelReached | Threshold | 5 | 0, 50 Coins |
| `points_500` | Полтыщи опыта | PointsAccumulated | Threshold | 500 | 0, 100 Coins |
| `perfect_test` | Без ошибок | TestPassed | CountWithFilter | 1 (filter: minScore=100) | 30 XP, 20 Coins |

**DefaultLevelTiersSeeder** — НЕ при старте приложения, а через handler `TenantCreatedEvent` (если такого нет — на регистрацию первого пользователя через `UserRegisteredEvent`). Создаёт 5 тиров для тенанта:

| Order | Title | From | To | Color |
|---|---|---|---|---|
| 1 | Новичок | 1 | 10 | #9E9E9E |
| 2 | Начинающий | 11 | 20 | #4CAF50 |
| 3 | Специалист | 21 | 30 | #2196F3 |
| 4 | Эксперт | 31 | 40 | #9C27B0 |
| 5 | Мастер | 41 | 50 | #FFC107 |

**DemoShopItemsSeeder** — пара демо-предметов для нового тенанта:
- ProfileColor "Синий" — 50 Coins, Unique
- AvatarFrame "Простая рамка" — 100 Coins, Unique
- Pet "Котёнок" — 200 Coins, Unique, RequiredLevel=3

## LevelCalculator

Чистая функция, без БД. Формула: `RequiredXpForLevel(N) = 100 * N * N` (квадратичная).

```csharp
public int CalculateLevel(int totalXp)
{
    if (totalXp <= 0) { return 1; }
    var level = (int)Math.Floor(Math.Sqrt(totalXp / 100.0));
    return Math.Max(1, level);
}

public int XpRequiredForNextLevel(int totalXp)
{
    var current = CalculateLevel(totalXp);
    return 100 * (current + 1) * (current + 1) - totalXp;
}
```

Если позже захочешь сделать формулу настраиваемой per-tenant — выделишь интерфейс с конфигом. На MVP — константа в Infrastructure.

## DependencyInjection.cs

```csharp
public static IServiceCollection AddGamificationInfrastructure(this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<GamificationDbContext>(opts =>
        opts.UseNpgsql(config.GetConnectionString("Default"),
            npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "gamification")));
    services.AddScoped<IGamificationDbContext>(sp => sp.GetRequiredService<GamificationDbContext>());
    services.AddScoped<IPlayerProfileRepository, PlayerProfileRepository>();
    // ... остальные репозитории
    services.AddScoped<ILevelCalculator, LevelCalculator>();
    services.AddScoped<ITenantConfigurationProvider, TenantConfigurationProvider>();
    return services;
}
```

---

# Слой Api

Проект: `Qwestr.Modules.Gamification.Api`. Зависимости: **только** `Application`. Не ссылайся на Infrastructure.

## Структура папок

```
Api/
├── Endpoints/
│   ├── PlayerProfileEndpoints.cs
│   ├── LevelTiersEndpoints.cs
│   ├── AwardsEndpoints.cs
│   ├── ShopEndpoints.cs
│   ├── InventoryEndpoints.cs
│   └── LeaderboardEndpoints.cs
├── Contracts/                  — request/response DTO для API (если отличаются от Application)
└── GamificationModule.cs       — AddGamificationModule + MapGamificationModule
```

## Endpoint-карта

База: `/api/gamification`

| Метод | Путь | Роль | Назначение |
|---|---|---|---|
| GET | `/profiles/me` | любая | Свой профиль с экипировкой и счётчиками |
| GET | `/profiles/{userId}` | любая | Чужой профиль (для просмотра карточки сотрудника) |
| GET | `/profiles/me/transactions?from=&to=&limit=` | любая | История XP/Coins-транзакций |
| GET | `/level-tiers` | любая | Список тиров тенанта |
| POST | `/level-tiers` | Manager+ | Создать тир |
| PUT | `/level-tiers/{id}` | Manager+ | Обновить |
| DELETE | `/level-tiers/{id}` | Manager+ | Удалить |
| GET | `/awards` | любая | Список наград (для коллекции) |
| GET | `/awards/management` | Manager+ | Список для управления |
| POST | `/awards/automatic` | Manager+ | Создать достижение с правилом |
| POST | `/awards/manual` | Manager+ | Создать бейдж (визуал) |
| PUT | `/awards/{id}` | Manager+ | Обновить |
| POST | `/awards/{id}/deactivate` | Manager+ | Деактивировать |
| POST | `/awards/{id}/issue` | Manager+ | Выдать Manual-Award сотруднику (тело: profileId) |
| GET | `/profiles/me/earned-awards` | любая | Полученные награды |
| GET | `/profiles/me/progress` | любая | Прогресс по достижениям (только Automatic) |
| GET | `/shop` | любая | Магазин с признаками "доступно/куплено" |
| GET | `/shop/management` | Manager+ | Управление |
| POST | `/shop` | Manager+ | Создать предмет |
| PUT | `/shop/{id}` | Manager+ | Обновить |
| POST | `/shop/{id}/deactivate` | Manager+ | Деактивировать |
| POST | `/shop/{id}/purchase` | любая | Купить (от лица текущего пользователя) |
| GET | `/inventory/me` | любая | Мой инвентарь |
| POST | `/inventory/equip` | любая | Тело: `{ shopItemId }`. Слот определяется автоматически |
| POST | `/inventory/unequip` | любая | Тело: `{ slot }` |
| GET | `/leaderboard?period=&top=` | любая | Лидерборд |
| GET | `/leaderboard/me?period=` | любая | Моя позиция |

## GamificationModule.cs

По шаблону `OnboardingModule`:

```csharp
public static class GamificationModule
{
    public static IServiceCollection AddGamificationModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddGamificationApplication();
        services.AddGamificationInfrastructure(config);
        return services;
    }

    public static IEndpointRouteBuilder MapGamificationModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/gamification").WithTags("Gamification");
        group.MapPlayerProfileEndpoints();
        group.MapLevelTiersEndpoints();
        group.MapAwardsEndpoints();
        group.MapShopEndpoints();
        group.MapInventoryEndpoints();
        group.MapLeaderboardEndpoints();
        return app;
    }
}
```

## Обработка Result в endpoints

Используй тот же экстеншн, что и в Onboarding (например, `result.ToHttpResponse()` или хелпер, превращающий `Result<T>` в `Results.Ok(value)` / `Results.BadRequest(error)`). Не писать ad-hoc.

## Авторизация

Те же политики, что и в Onboarding: `RequireAuthorization("Manager")` для управляющих endpoint'ов, `RequireAuthorization()` для остальных. Конкретные имена политик возьми из существующих модулей.

---

# Регистрация в QwestrAPI хосте

В `Program.cs` добавить:

```csharp
builder.Services.AddGamificationModule(builder.Configuration);
// ...
app.MapGamificationModule();
```

В `appsettings.json` — ничего нового, ConnectionString общий.

---

# Контракты в Shared.Contracts

Добавь в `Shared.Contracts`:

```csharp
namespace Shared.Contracts.Gamification;

public sealed record AwardEarnedIntegrationEvent(
    Guid TenantId, Guid UserId, Guid AwardId, string AwardCode, DateTime EarnedAt) : INotification;

public sealed record PlayerLevelUpIntegrationEvent(
    Guid TenantId, Guid UserId, int OldLevel, int NewLevel, DateTime OccurredAt) : INotification;

public interface IGamificationStatsProvider
{
    Task<GamificationTenantStats> GetTenantStatsAsync(Guid tenantId, CancellationToken ct);
}

public sealed record GamificationTenantStats(
    int ActivePlayers, double AverageLevel, IReadOnlyList<TopAwardStat> TopAwards);

public sealed record TopAwardStat(Guid AwardId, string Code, int EarnedCount);
```

`IGamificationStatsProvider` понадобится Analytics в фазе 4. Реализация — в Infrastructure модуля Gamification.

Внутренние доменные события (`PlayerLevelUpEvent` и др.) на эти integration-события мапятся в обработчиках уровня Application — преобразуй и публикуй через MediatR во время post-commit hook.

---

# Порядок реализации

Не пытайся делать всё сразу. Идём вертикальными срезами:

1. **Срез 1 — Profile + Points (фундамент).** PlayerProfile + PointsTransaction + CoinTransaction. UserRegisteredEventHandler. PointsService. OnboardingStepCompletedEventHandler без Award (только баллы). LevelCalculator. Endpoints `/profiles/me` и `/profiles/me/transactions`. Миграция, прогон. После этого: сотрудник проходит шаг → видит баллы и уровень. **Это уже демонстрируемая фича.**

2. **Срез 2 — LevelTiers.** CRUD тиров, отображение звания в `/profiles/me`. DefaultLevelTiersSeeder.

3. **Срез 3 — Awards (сначала только Automatic + базовые evaluators).** Award + AchievementProgress + EarnedAward. AwardEvaluator с двумя evaluator'ами (StepCompletedCount, ScenarioCompletedCount — самые простые). Подключение к OnboardingStepCompletedEventHandler. Endpoints на чтение. SystemAwardsSeeder с двумя-тремя наградами. Видимый эффект: сотрудник прошёл первый шаг → получил бейдж.

4. **Срез 4 — Manual Awards + остальные evaluators.** CreateManualAward, IssueManualAward. LevelThresholdEvaluator, PointsAccumulatedEvaluator, TestPassedWithFilterEvaluator.

5. **Срез 5 — Shop + Inventory.** ShopItem CRUD, PurchaseShopItem, GetMyInventory, Equip/Unequip. DemoShopItemsSeeder.

6. **Срез 6 — Leaderboard.** GetLeaderboard, GetLeaderboardMyPosition.

7. **Срез 7 — IGamificationStatsProvider** (минимально, для Analytics фазы 4).

После каждого среза — компилируется, миграция накатывается, endpoint возвращает данные. Не переходи к следующему срезу с поломанным предыдущим.

---

# Что НЕ делать в этом плане

- Blazor UI — отдельная задача, делается после backend
- DailyStreakEvaluator — пропустить, если требует модификации домена
- Outbox pattern для доменных событий — на MVP достаточно publish-after-savechanges в DbContext
- Уведомления, дашборды — это фазы 4-5, не трогать
- Кэш лидерборда — без него на MVP, добавлять только если будут видимые проблемы производительности

---

# Чек перед PR

- [ ] Все команды/запросы возвращают `Result<T>` или `Result`
- [ ] Все DB-запросы фильтруются по `TenantId` (через global filter или явно)
- [ ] Все уникальные индексы из доменного раздела созданы в миграции
- [ ] OnboardingStepCompletedEventHandler идемпотентен (проверка через `PointsTransaction(ProfileId, SourceType, SourceId)`)
- [ ] Доменные события публикуются после `SaveChangesAsync`, не до
- [ ] Нет ссылок Api → Infrastructure
- [ ] Нет прямых ссылок между модулями (Onboarding → Gamification и наоборот) — только через `Shared.Contracts`
- [ ] Все if/else имеют фигурные скобки
- [ ] Используется `var` где возможно
- [ ] Snake_case в БД, PascalCase в C#
- [ ] Swagger показывает все endpoint'ы с корректной авторизацией
