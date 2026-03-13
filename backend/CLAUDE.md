# Qwestr

SaaS-платформа для онбординга и мотивации сотрудников с геймификацией. Модульный монолит на ASP.NET Core + Blazor Server. Дипломный проект УрФУ.

## Tech Stack

- .NET 9, C# 13, ASP.NET Core Minimal API
- Blazor Server (отдельный проект, MudBlazor UI)
- PostgreSQL, EF Core, schema-per-module
- MediatR (CQRS), FluentValidation, Mapster
- JWT auth (access + refresh tokens)
- Docker Compose

## Структура решения

```
qwestr-backend/
├── QwestrAPI/              # Хост — точка входа, Swagger, JWT, DI всех модулей
├── Modules/
│   ├── Identity/           # Тенанты, пользователи, роли, аутентификация
│   ├── Onboarding/         # Сценарии адаптации, этапы, задания, прогресс
│   ├── Gamification/       # Баллы, уровни, бейджи, лидерборд
│   ├── Analytics/          # Метрики вовлечённости, дашборд HR
│   ├── Notifications/      # Уведомления, SignalR-хаб
│   └── Integration/        # Импорт сотрудников из внешних систем
├── Shared/
│   ├── Shared.Kernel/      # Entity, AggregateRoot, Result, доменные примитивы
│   ├── Shared.Infrastructure/ # ICurrentUserService, EF-конвенции, exception handler
│   └── Shared.Contracts/   # Межмодульные события и интерфейсы (IUserProvider и др.)
└── docker-compose.yml
```

Модули: Identity, Onboarding, Gamification, Analytics, Notifications, Integration.

## Dependency Rules — CRITICAL

- Domain → ничего (только Shared.Kernel)
- Application → Domain + Shared.Contracts
- Infrastructure → Application + Domain
- Api → Application (НИКОГДА не ссылается на Infrastructure напрямую)
- Модули НЕ ссылаются друг на друга — только через Shared.Contracts
- Общих entity между модулями нет — каждый хранит свои проекции

## Coding Conventions

- Все entity наследуют от `Entity<Guid>` или `AggregateRoot<Guid>` из Shared.Kernel
- Результаты операций через `Result<T>` — не exceptions для бизнес-ошибок
- При объявлении новых переменных в методе используй ключевое слово var по возможности
- Все блоки if {} else {} обязательно должны иметь фигурные скобки
- CQRS: команды и запросы через MediatR `IRequest<Result<T>>`
- Валидация через FluentValidation + MediatR pipeline behavior
- Маппинг через Mapster (не AutoMapper)
- Каждый модуль: свой DbContext, своя PostgreSQL schema
- Naming: PascalCase для всего C#, snake_case для таблиц/колонок в PostgreSQL
- Файлы EF-конфигураций в Infrastructure/{ModuleName}/Persistence/Configurations/

## Module Registration Pattern

```csharp
// В каждом модуле Api:
public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration config) { ... }
    public static IEndpointRouteBuilder MapIdentityModule(this IEndpointRouteBuilder app) { ... }
}

// В хосте QwestrAPI:
builder.Services.AddIdentityModule(builder.Configuration);
app.MapIdentityModule();
```

## Commands

- `dotnet build QwestrAPI.sln` — сборка всего
- `dotnet test` — запуск тестов
- `docker compose up -d` — поднять PostgreSQL + API
- `dotnet ef migrations add {Name} -p src/Modules/{Module}/Qwestr.Modules.{Module}.Infrastructure -s src/Qwestr.Api`

## Multi-tenancy

- TenantId передаётся в JWT claims
- ICurrentUserService извлекает TenantId из ClaimsPrincipal
- Все запросы к БД ОБЯЗАТЕЛЬНО фильтруются по TenantId
- НИКОГДА не создавай endpoint без проверки TenantId

## Important Rules

- НЕ создавай God-классы и сервисы — один handler = одна операция
- НЕ используй исключения для бизнес-логики — только Result<T>
- НЕ делай прямых вызовов между модулями — только Shared.Contracts
- НЕ добавляй пакеты без явного запроса
- Перед созданием нового файла проверь, нет ли аналогичного
- При изменении контрактов в Shared проверь все модули-потребители
