# Qwestr — Frontend

Blazor Server клиент для платформы геймифицированного онбординга сотрудников.

## Быстрый старт

```bash
docker compose up
```

## Требования

- .NET 8 SDK
- Docker + Docker Compose (опционально)

## Локальная разработка

```bash
cd QwestrWebClient
dotnet run
```

Приложение откроется на `https://localhost:5001` (или порт из `launchSettings.json`).

> Backend API должен быть запущен отдельно (см. `qwestr-backend`).

## Структура решения

```
qwestr-frontend/
├── QwestrWebClient/        # Blazor Server — страницы, layout, компоненты
│   ├── Layout/             # MainLayout, NavMenu (HxSidebar), TopBar
│   ├── Pages/              # Страницы по маршрутам
│   └── Shared/             # Переиспользуемые компоненты (PointsBadge и др.)
├── Client.Core/            # Абстракции, ViewModels, перечисления (без зависимостей)
└── Client.Infrastructure/  # HTTP-клиенты, сервисы для работы с API
```

## Стек

| | |
|---|---|
| Фреймворк | Blazor Server (.NET 8) |
| UI-библиотека | [HAVIT Blazor](https://havit.blazor.eu) (Bootstrap 5) |
