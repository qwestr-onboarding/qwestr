# Qwestr — Backend

SaaS-платформа геймифицированного онбординга сотрудников. Backend — REST API на ASP.NET Core 8, модульная архитектура, PostgreSQL.

## Быстрый старт

```bash
docker compose up
```

API будет доступно на `http://localhost:8080`.
Swagger UI: `http://localhost:8080/swagger`.

## Требования

- Docker + Docker Compose
- .NET 8 SDK (для локальной разработки без Docker)

## Локальная разработка без Docker

```bash
# Поднять только БД
docker compose up qwestr-db

# Запустить API
cd QwestrAPI
dotnet run
```

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

Каждый модуль разбит на 4 слоя: `Domain → Application → Infrastructure → Api`.

## Переменные окружения

| Переменная | Значение по умолчанию (docker) |
|---|---|
| `ConnectionStrings__DefaultConnection` | `Host=qwestr-db;Port=5432;Database=qwestr;...` |
| `ASPNETCORE_URLS` | `http://+:8080` |
| `Jwt__Issuer` | `QwestrAPI` |
| `Jwt__Audience` | `QwestrClient` |
| `Jwt__Secret` | задать через secrets в prod |
