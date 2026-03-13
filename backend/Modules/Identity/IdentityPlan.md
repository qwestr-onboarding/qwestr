# Identity Module — Полный план реализации

## Обзор

Модуль Identity отвечает за: регистрацию компаний, аутентификацию, управление пользователями,
приглашения, сброс пароля, 2FA. Два агрегата: **Tenant** и **User**.

---

## 1. Domain Layer

### Entities & Aggregates

| Класс | Тип | Описание |
|-------|-----|----------|
| Tenant | AggregateRoot\<Guid\> | Компания-арендатор |
| TenantSettings | Entity\<Guid\> | Настройки компании (1:1 с Tenant) |
| User | AggregateRoot\<Guid\> | Пользователь внутри тенанта |
| RefreshToken | Entity\<Guid\> | JWT refresh-токен |
| Invitation | Entity\<Guid\> | Приглашение в компанию |
| PasswordResetToken | Entity\<Guid\> | Токен сброса пароля |
| UserBackupCode | Entity\<Guid\> | Резервный код 2FA |

### Enums

| Enum | Значения |
|------|----------|
| TenantPlan | Free = 0, Basic = 1, Pro = 2 |
| UserRole | Employee = 0, Manager = 1, Admin = 2, Owner = 3 |
| InvitationStatus | Pending = 0, Accepted = 1, Expired = 2, Cancelled = 3 |

### Domain Events (MediatR INotification)

| Событие | Когда публикуется | Кто слушает |
|---------|-------------------|-------------|
| TenantRegisteredEvent | После создания компании | Notifications (welcome email) |
| UserRegisteredEvent | После регистрации пользователя | Onboarding, Gamification (создание проекций) |
| InvitationCreatedEvent | После создания приглашения | Notifications (отправка email) |
| PasswordResetRequestedEvent | После запроса сброса пароля | Notifications (отправка email) |
| TwoFactorEnabledEvent | После подтверждения 2FA | Notifications (уведомление безопасности) |

### Бизнес-логика в сущностях

**Tenant:**
- `Tenant.Create(name, subdomain)` → статический фабричный метод
- `Deactivate()` → устанавливает IsActive = false
- `UpdateSettings(timezone, language, logoUrl)` → обновляет TenantSettings

**User:**
- `User.Create(tenantId, email, passwordHash, firstName, lastName, username, role)` → фабричный метод
- `VerifyPassword(hash, password)` → проверка пароля (делегирует IPasswordHasher)
- `UpdateProfile(firstName, lastName, username)` → смена данных
- `ChangePassword(newPasswordHash)` → смена пароля
- `EnableTwoFactor(secret)` → начало настройки 2FA
- `ConfirmTwoFactor()` → подтверждение 2FA после ввода кода
- `DisableTwoFactor()` → отключение 2FA
- `GenerateBackupCodes(hashes)` → создание резервных кодов
- `UseBackupCode(codeHash)` → использование резервного кода
- `Deactivate()` / `Activate()` → управление активностью
- `RecordLogin()` → обновление LastLoginAt

**RefreshToken:**
- `RefreshToken.Create(userId, token, expiresAt)` → фабричный метод
- `Revoke()` → отзыв токена
- `IsExpired` → computed property
- `IsActive` → !IsExpired && RevokedAt == null

**Invitation:**
- `Invitation.Create(tenantId, email, role, invitedByUserId, token, expiresAt)` → фабричный метод
- `Accept()` → смена статуса
- `Cancel()` → отмена
- `IsExpired` → computed property

**PasswordResetToken:**
- `PasswordResetToken.Create(userId, token, expiresAt)` → фабричный метод
- `MarkUsed()` → помечает использованным
- `IsValid` → !IsUsed && !IsExpired

---

## 2. Application Layer

### Абстракции (интерфейсы)

#### Репозитории

```
ITenantRepository
├── GetByIdAsync(Guid id, CancellationToken ct)              → Tenant?
├── GetBySubdomainAsync(string subdomain, CancellationToken ct) → Tenant?
├── ExistsBySubdomainAsync(string subdomain, CancellationToken ct) → bool
├── AddAsync(Tenant tenant, CancellationToken ct)             → void
└── SaveChangesAsync(CancellationToken ct)                    → Task

IUserRepository
├── GetByIdAsync(Guid id, CancellationToken ct)               → User?
├── GetByEmailAsync(Guid tenantId, string email, CancellationToken ct) → User?
├── GetByUsernameAsync(Guid tenantId, string username, CancellationToken ct) → User?
├── ExistsByEmailAsync(Guid tenantId, string email, CancellationToken ct) → bool
├── ExistsByUsernameAsync(Guid tenantId, string username, CancellationToken ct) → bool
├── GetActiveByTenantAsync(Guid tenantId, int page, int pageSize, CancellationToken ct) → List<User>
├── CountByTenantAsync(Guid tenantId, CancellationToken ct)   → int
├── AddAsync(User user, CancellationToken ct)                  → void
└── SaveChangesAsync(CancellationToken ct)                     → Task

IRefreshTokenRepository
├── GetByTokenAsync(string token, CancellationToken ct)        → RefreshToken?
├── GetActiveByUserIdAsync(Guid userId, CancellationToken ct)  → List<RefreshToken>
├── AddAsync(RefreshToken token, CancellationToken ct)         → void
├── RevokeAllByUserIdAsync(Guid userId, CancellationToken ct)  → Task
└── SaveChangesAsync(CancellationToken ct)                     → Task

IInvitationRepository
├── GetByTokenAsync(string token, CancellationToken ct)        → Invitation?
├── GetByTenantAsync(Guid tenantId, int page, int pageSize, CancellationToken ct) → List<Invitation>
├── ExistsPendingAsync(Guid tenantId, string email, CancellationToken ct) → bool
├── AddAsync(Invitation invitation, CancellationToken ct)      → void
└── SaveChangesAsync(CancellationToken ct)                     → Task

IPasswordResetTokenRepository
├── GetByTokenAsync(string token, CancellationToken ct)        → PasswordResetToken?
├── InvalidateAllByUserIdAsync(Guid userId, CancellationToken ct) → Task
├── AddAsync(PasswordResetToken token, CancellationToken ct)   → void
└── SaveChangesAsync(CancellationToken ct)                     → Task
```

#### Сервисы (интерфейсы в Application, реализация в Infrastructure)

```
ITokenService
├── GenerateAccessToken(User user)          → string
├── GenerateRefreshToken()                  → string
└── GetPrincipalFromExpiredToken(string token) → ClaimsPrincipal?

IPasswordHasher
├── Hash(string password)                   → string
└── Verify(string password, string hash)    → bool

ITotpService
├── GenerateSecret()                        → string
├── GenerateQrCodeUri(string email, string secret) → string
└── VerifyCode(string secret, string code)  → bool
```

### Commands (записи, мутации)

Каждая команда — отдельная папка с Command.cs + Handler.cs + Validator.cs.

#### Регистрация и вход

| Команда | Вход | Выход | Что делает |
|---------|------|-------|------------|
| RegisterTenantCommand | Name, Subdomain, OwnerEmail, OwnerPassword, OwnerFirstName, OwnerLastName | Result\<TenantRegisteredResponse\> | Создаёт Tenant + TenantSettings + User(Owner). Публикует TenantRegisteredEvent, UserRegisteredEvent |
| LoginCommand | Email, Password, Subdomain (или TenantId) | Result\<TokenResponse\> | Находит юзера, проверяет пароль, если 2FA — возвращает RequiresTwoFactor. Иначе — генерирует access+refresh |
| VerifyTwoFactorCommand | UserId, Code, IsBackupCode | Result\<TokenResponse\> | Проверяет TOTP/backup код, генерирует токены |
| RefreshTokenCommand | RefreshToken | Result\<TokenResponse\> | Валидирует refresh, ротация: старый → revoked, новый → выдан |
| LogoutCommand | RefreshToken | Result | Отзывает refresh token |

#### Управление пользователями

| Команда | Вход | Выход | Что делает |
|---------|------|-------|------------|
| CreateInvitationCommand | Email, Role | Result\<Guid\> | Создаёт приглашение, публикует InvitationCreatedEvent |
| AcceptInvitationCommand | Token, Password, FirstName, LastName, Username | Result\<TokenResponse\> | Принимает приглашение, создаёт User, логинит |
| CancelInvitationCommand | InvitationId | Result | Отменяет приглашение (Owner/Admin) |
| DeactivateUserCommand | UserId | Result | Деактивирует пользователя (Owner/Admin) |
| ActivateUserCommand | UserId | Result | Активирует пользователя |
| ChangeUserRoleCommand | UserId, NewRole | Result | Смена роли (Owner only, нельзя менять себе) |

#### Профиль и безопасность

| Команда | Вход | Выход | Что делает |
|---------|------|-------|------------|
| UpdateProfileCommand | FirstName, LastName, Username | Result | Обновляет профиль текущего юзера |
| ChangePasswordCommand | CurrentPassword, NewPassword | Result | Смена пароля (проверяет текущий) |
| RequestPasswordResetCommand | Email, Subdomain | Result | Создаёт PasswordResetToken, публикует PasswordResetRequestedEvent. Всегда Result.Success (не раскрываем существование email) |
| ResetPasswordCommand | Token, NewPassword | Result | Сбрасывает пароль по токену |
| SetupTwoFactorCommand | (текущий юзер) | Result\<TwoFactorSetupResponse\> | Генерирует секрет, возвращает QR URI |
| ConfirmTwoFactorCommand | Code | Result\<BackupCodesResponse\> | Подтверждает 2FA, генерирует backup-коды |
| DisableTwoFactorCommand | Password | Result | Отключает 2FA (требует пароль) |

#### Настройки тенанта

| Команда | Вход | Выход | Что делает |
|---------|------|-------|------------|
| UpdateTenantSettingsCommand | Timezone, Language, LogoUrl | Result | Обновляет настройки (Owner/Admin) |

### Queries (чтение)

| Запрос | Вход | Выход | Кто может |
|--------|------|-------|-----------|
| GetCurrentUserQuery | (из JWT) | UserProfileResponse | Любой авторизованный |
| GetUserByIdQuery | UserId | UserProfileResponse | Manager+ |
| GetUsersListQuery | Page, PageSize, Search? | PagedList\<UserListItemResponse\> | Manager+ |
| GetInvitationsListQuery | Page, PageSize, Status? | PagedList\<InvitationResponse\> | Admin+ |
| GetTenantSettingsQuery | (из JWT) | TenantSettingsResponse | Любой авторизованный |

### DTOs (Response models)

```
TokenResponse
├── AccessToken: string
├── RefreshToken: string
├── ExpiresAt: DateTime
└── RequiresTwoFactor: bool   (если true — токены пустые, нужен VerifyTwoFactor)

TenantRegisteredResponse
├── TenantId: Guid
├── AccessToken: string
├── RefreshToken: string
└── ExpiresAt: DateTime

UserProfileResponse
├── Id: Guid
├── Email: string
├── FirstName: string
├── LastName: string
├── Username: string?
├── Role: UserRole
├── TwoFactorEnabled: bool
├── LastLoginAt: DateTime?
├── CreatedAt: DateTime

UserListItemResponse
├── Id: Guid
├── Email: string
├── FullName: string
├── Username: string?
├── Role: UserRole
├── IsActive: bool
├── LastLoginAt: DateTime?

InvitationResponse
├── Id: Guid
├── Email: string
├── Role: UserRole
├── Status: InvitationStatus
├── InvitedByName: string
├── CreatedAt: DateTime
├── ExpiresAt: DateTime

TenantSettingsResponse
├── CompanyName: string
├── CompanyLogoUrl: string?
├── Timezone: string
├── Language: string

TwoFactorSetupResponse
├── Secret: string
├── QrCodeUri: string

BackupCodesResponse
├── Codes: List<string>    (показываются один раз, потом хранятся как хэши)

PagedList<T>
├── Items: List<T>
├── Page: int
├── PageSize: int
├── TotalCount: int
├── TotalPages: int
```

### Validators (FluentValidation)

Каждая команда получает свой валидатор. Ключевые правила:

- **RegisterTenantCommand:** Email формат, пароль >= 8 символов, Name непустое, Subdomain — только a-z0-9-
- **LoginCommand:** Email и Password непустые
- **CreateInvitationCommand:** Email формат, Role != Owner (нельзя приглашать владельца)
- **AcceptInvitationCommand:** Token непустой, пароль >= 8, Username — a-z0-9_ длина 3-32
- **ChangePasswordCommand:** NewPassword >= 8, не совпадает с CurrentPassword
- **UpdateProfileCommand:** Username — a-z0-9_ длина 3-32, FirstName/LastName непустые
- **UpdateTenantSettingsCommand:** Timezone — валидная IANA таймзона

### MediatR Pipeline Behaviors

```
ValidationBehavior<TRequest, TResponse>     — запускает FluentValidation перед хендлером
TenantAccessBehavior<TRequest, TResponse>   — проверяет что TenantId из JWT активен
```

---

## 3. Infrastructure Layer

### Persistence

#### IdentityDbContext
- DbSet\<Tenant\>, DbSet\<TenantSettings\>
- DbSet\<User\>, DbSet\<RefreshToken\>, DbSet\<UserBackupCode\>
- DbSet\<Invitation\>, DbSet\<PasswordResetToken\>
- Schema: `identity`
- Global query filter: `.HasQueryFilter(x => x.TenantId == _currentUser.TenantId)` на tenant-scoped entities

#### EF Configurations (по файлу на сущность)
- TenantConfiguration.cs
- TenantSettingsConfiguration.cs
- UserConfiguration.cs — уникальный индекс (TenantId, Email), (TenantId, Username)
- RefreshTokenConfiguration.cs — уникальный индекс (Token)
- InvitationConfiguration.cs — уникальный индекс (Token), индекс (TenantId, Email)
- PasswordResetTokenConfiguration.cs — уникальный индекс (Token)
- UserBackupCodeConfiguration.cs — индекс (UserId)

#### Repositories (реализации интерфейсов из Application)
- TenantRepository.cs
- UserRepository.cs
- RefreshTokenRepository.cs
- InvitationRepository.cs
- PasswordResetTokenRepository.cs

### Services (реализации интерфейсов из Application)

| Класс | Реализует | NuGet-зависимость |
|-------|-----------|-------------------|
| JwtTokenService | ITokenService | Microsoft.AspNetCore.Authentication.JwtBearer |
| BcryptPasswordHasher | IPasswordHasher | BCrypt.Net-Next |
| TotpService | ITotpService | Otp.NET (Kel.OtpNet) |

### DI Registration

```csharp
public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration config)
{
    // DbContext
    services.AddDbContext<IdentityDbContext>(opts =>
        opts.UseNpgsql(config.GetConnectionString("Database"),
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));

    // Repositories
    services.AddScoped<ITenantRepository, TenantRepository>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    services.AddScoped<IInvitationRepository, InvitationRepository>();
    services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

    // Services
    services.AddScoped<ITokenService, JwtTokenService>();
    services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
    services.AddSingleton<ITotpService, TotpService>();

    // MediatR (handlers + validators + behaviors)
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));
    services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);

    return services;
}
```

---

## 4. Api Layer (Minimal API Endpoints)

### Группы эндпоинтов

#### /api/identity/auth
| Метод | Маршрут | Команда/Запрос | Авторизация |
|-------|---------|----------------|-------------|
| POST | /register | RegisterTenantCommand | Anonymous |
| POST | /login | LoginCommand | Anonymous |
| POST | /login/2fa | VerifyTwoFactorCommand | Anonymous (с temp token) |
| POST | /refresh | RefreshTokenCommand | Anonymous |
| POST | /logout | LogoutCommand | Authorized |
| POST | /password/forgot | RequestPasswordResetCommand | Anonymous |
| POST | /password/reset | ResetPasswordCommand | Anonymous |

#### /api/identity/users
| Метод | Маршрут | Команда/Запрос | Авторизация |
|-------|---------|----------------|-------------|
| GET | /me | GetCurrentUserQuery | Authorized |
| PUT | /me | UpdateProfileCommand | Authorized |
| PUT | /me/password | ChangePasswordCommand | Authorized |
| POST | /me/2fa/setup | SetupTwoFactorCommand | Authorized |
| POST | /me/2fa/confirm | ConfirmTwoFactorCommand | Authorized |
| DELETE | /me/2fa | DisableTwoFactorCommand | Authorized |
| GET | / | GetUsersListQuery | Manager+ |
| GET | /{id} | GetUserByIdQuery | Manager+ |
| PUT | /{id}/deactivate | DeactivateUserCommand | Admin+ |
| PUT | /{id}/activate | ActivateUserCommand | Admin+ |
| PUT | /{id}/role | ChangeUserRoleCommand | Owner |

#### /api/identity/invitations
| Метод | Маршрут | Команда/Запрос | Авторизация |
|-------|---------|----------------|-------------|
| POST | / | CreateInvitationCommand | Admin+ |
| GET | / | GetInvitationsListQuery | Admin+ |
| POST | /accept | AcceptInvitationCommand | Anonymous |
| DELETE | /{id} | CancelInvitationCommand | Admin+ |

#### /api/identity/tenant/settings
| Метод | Маршрут | Команда/Запрос | Авторизация |
|-------|---------|----------------|-------------|
| GET | / | GetTenantSettingsQuery | Authorized |
| PUT | / | UpdateTenantSettingsCommand | Admin+ |

---

## 5. Shared.Contracts (то, что видят другие модули)

### Интерфейсы

```csharp
public interface IUserProvider
{
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken ct);
    Task<List<UserDto>> GetByTenantAsync(Guid tenantId, CancellationToken ct);
}

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid TenantId { get; }
    UserRole Role { get; }
    bool IsAuthenticated { get; }
}
```

### Integration Events (MediatR INotification)

```csharp
public record UserRegisteredIntegrationEvent(Guid UserId, Guid TenantId, string Email, string FullName) : INotification;
public record UserDeactivatedIntegrationEvent(Guid UserId, Guid TenantId) : INotification;
```

### Shared DTOs

```csharp
public record UserDto(Guid Id, Guid TenantId, string Email, string FullName, string? Username, UserRole Role);
```

---

## 6. Порядок реализации (рекомендуемый)

### Шаг 1 — Domain (день 1)
Все 7 сущностей, 3 enum, фабричные методы, domain events.

### Шаг 2 — Infrastructure: DbContext + Configurations + Миграции (день 1-2)
IdentityDbContext, все 7 EF-конфигураций, первая миграция, проверить что schema `identity` создаётся.

### Шаг 3 — Infrastructure: Repositories + Services (день 2)
Все 5 репозиториев, JwtTokenService, BcryptPasswordHasher. Без TotpService пока.

### Шаг 4 — Application: Auth flow (день 3-4)
RegisterTenantCommand → LoginCommand → RefreshTokenCommand → LogoutCommand.
Это вертикальный срез: после него можно зарегистрироваться и залогиниться.

Шаг 4 выполнен. Итог:                                                                                                                                                                                                               
Domain Events (Identity.Domain/Events/):                                                                                                                                                                                              - TenantRegisteredEvent, UserRegisteredEvent                                                                                                                                                                                        
DTOs (Identity.Application/DTOs/):
- TokenResponse — AccessToken, RefreshToken, ExpiresAt, RequiresTwoFactor                                                                                                                                                             - TenantRegisteredResponse — TenantId + токены

ValidationBehavior — перехватывает ошибки FluentValidation, бросает ValidationException, которое GlobalExceptionHandler возвращает как 400

Команды:                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
┌───────────────────────┬────────────────────────────────────────────────────────────────────────────────────────────────────┐                                                                                                        │        Команда        │                                               Логика                                               │                                                                                                    
├───────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────┤                                                                                                      
│ RegisterTenantCommand │ Создаёт Tenant + TenantSettings + User(Owner), генерирует токены, публикует события                │                                                                                                      
├───────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ LoginCommand          │ Находит тенант по subdomain, проверяет пароль, возвращает RequiresTwoFactor=true если включена 2FA │                                                                                                      
├───────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────┤                                                                                                        │ RefreshTokenCommand   │ Ротация: старый токен отзывается (с детекцией повторного использования), выдаётся новый            │                                                                                                      
├───────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────┤                                                                                                        │ LogoutCommand         │ Отзывает refresh token                                                                             │                                                                                                    
└───────────────────────┴────────────────────────────────────────────────────────────────────────────────────────────────────┘

DI в IdentityStartup — зарегистрированы MediatR, валидаторы и ValidationBehavior<,>.

### Шаг 5 — Api: Auth endpoints + Swagger (день 4)
Эндпоинты /api/auth/*, проверить весь цикл через Swagger.

Выполнен.

### Шаг 6 — Application: Users + Invitations (день 5-6)
Все команды управления пользователями, приглашения, смена пароля.

Шаг 6 выполнен.                                                                                                                                                                      
---                                                                                                                                                                                                                                   Вот что реализовано:

Domain — новые методы в User:                                                                                                                                                                                                         - UpdateProfile, ChangePassword, Deactivate, Activate, ChangeRole                                                                                                                                                                     - Новое событие InvitationCreatedEvent                                                                                                                                                                                                                                                                                                                                                                                                                                    
Application DTOs — 5 новых:
UserProfileResponse, UserListItemResponse, InvitationResponse, TenantSettingsResponse, PagedList<T>

Commands (8 штук):

┌─────────────────────────┬─────────────────────────────────────────────────────────────────────────┐
│         Команда         │                                 Логика                                  │
├─────────────────────────┼─────────────────────────────────────────────────────────────────────────┤
│ CreateInvitationCommand │ Проверяет дубли, генерирует токен, публикует InvitationCreatedEvent     │
├─────────────────────────┼─────────────────────────────────────────────────────────────────────────┤
│ AcceptInvitationCommand │ Валидирует инвайт, создаёт User, логинит, публикует UserRegisteredEvent │
├─────────────────────────┼─────────────────────────────────────────────────────────────────────────┤
│ CancelInvitationCommand │ Отменяет только Pending инвайты, проверяет TenantId                     │
├─────────────────────────┼─────────────────────────────────────────────────────────────────────────┤
│ DeactivateUserCommand   │ Идемпотентно, проверяет TenantId                                        │
├─────────────────────────┼─────────────────────────────────────────────────────────────────────────┤
│ ActivateUserCommand     │ Идемпотентно, проверяет TenantId                                        │
├─────────────────────────┼─────────────────────────────────────────────────────────────────────────┤
│ ChangeUserRoleCommand   │ Запрещает менять себе / Owner → нельзя назначить/переназначить          │
├─────────────────────────┼─────────────────────────────────────────────────────────────────────────┤
│ UpdateProfileCommand    │ FirstName + LastName                                                    │
├─────────────────────────┼─────────────────────────────────────────────────────────────────────────┤
│ ChangePasswordCommand   │ Проверяет текущий пароль, запрещает совпадение с новым                  │
└─────────────────────────┴─────────────────────────────────────────────────────────────────────────┘

Queries (5 штук):
GetCurrentUserQuery, GetUserByIdQuery, GetUsersListQuery, GetInvitationsListQuery, GetTenantSettingsQuery

Infrastructure — обновлены репозитории:
- InvitationRepository: добавлены GetByIdAsync, CountByTenantAsync; обновлён GetByTenantAsync с фильтром по статусу и Include(InvitedByUser)
- TenantRepository: добавлен GetWithSettingsAsync с Include(Settings)


### Шаг 7 — Application: 2FA (день 6)
2FA & Password Reset: SetupTwoFactor, ConfirmTwoFactor, DisableTwoFactor, VerifyTwoFactor, RequestPasswordReset, ResetPassword, TotpService

2FA: Включение
SetupTwoFactorCommand (вход: —, из JWT текущий юзер)
Генерирует TOTP-секрет → сохраняет в User.TwoFactorSecret → TwoFactorEnabled остаётся false → возвращает TwoFactorSetupResponse(Secret, QrCodeUri). Пользователь сканирует QR в Google Authenticator / Authy.
ConfirmTwoFactorCommand (вход: Code)
Проверяет код через ITotpService.VerifyCode(secret, code). Если верный → User.ConfirmTwoFactor() ставит TwoFactorEnabled = true, TwoFactorConfirmedAt = UtcNow → генерирует 8 резервных кодов, хэширует, сохраняет как UserBackupCode → возвращает коды в открытом виде однократно → публикует TwoFactorEnabledEvent(UserId, Email). Если код неверный → ошибка, секрет сохраняется, можно повторить.
2FA: Логин
LoginCommand (уже реализован, нужна доработка)
После успешной проверки пароля проверяет User.TwoFactorEnabled. Если true → токены не выдаются → генерируется короткоживущий JWT (5 минут, claim purpose: 2fa_verification, claim UserId) → возвращается TokenResponse(RequiresTwoFactor: true, AccessToken: temporaryJwt).
VerifyTwoFactorCommand (вход: Code, IsBackupCode)
Валидирует временный JWT → достаёт UserId из claims → если IsBackupCode = false → проверяет TOTP-код через ITotpService.VerifyCode() → если IsBackupCode = true → ищет среди UserBackupCode где IsUsed = false, сверяет хэши → если совпал, помечает IsUsed = true. При успехе → генерирует полноценные access + refresh токены → User.RecordLogin(). При неудаче → ошибка.
2FA: Отключение
DisableTwoFactorCommand (вход: Code)
Проверяет код через ITotpService.VerifyCode(). Если неверный → пробует как резервный код. Если подтверждён → User.DisableTwoFactor() обнуляет TwoFactorSecret, ставит TwoFactorEnabled = false, TwoFactorConfirmedAt = null → удаляет все UserBackupCode пользователя.
Password Reset
RequestPasswordResetCommand (вход: Email, Subdomain)
Ищет юзера по (TenantId, Email). Всегда возвращает Result.Success() — не раскрываем существование аккаунта. Если юзер найден → InvalidateAllByUserIdAsync (старые токены) → создаёт PasswordResetToken (срок жизни 2 часа) → публикует PasswordResetRequestedEvent(UserId, Email, Token, ExpiresAt) → модуль Notifications обработает и отправит email.
ResetPasswordCommand (вход: Token, NewPassword)
Ищет PasswordResetToken по значению → проверяет IsValid (не использован, не истёк). Если валиден → хэширует новый пароль → User.ChangePassword(newHash) → PasswordResetToken.MarkUsed() → RevokeAllByUserIdAsync (разлогинивает все сессии). Если невалиден → ошибка.
Infrastructure-сервис
TotpService (реализует ITotpService, NuGet: Otp.NET)

GenerateSecret() → 20-байтный ключ → Base32
GenerateQrCodeUri(email, secret) → otpauth://totp/Qwestr:{email}?secret={secret}&issuer=Qwestr
VerifyCode(secret, code) → new Totp(key).VerifyTotp(code, out _) с окном ±1 шаг (30 сек допуск)

Шаг 7 выполнен.

---                                                                                                                                                                                                                                   Вот что реализовано:

Domain:
- User: добавлены RecordLogin(), ConfirmTwoFactor(), DisableTwoFactor()
- PasswordResetToken: добавлено IsValid (computed: !IsUsed && !IsExpired)
- События: TwoFactorEnabledEvent, PasswordResetRequestedEvent

Application:
- DTOs: TwoFactorSetupResponse, BackupCodesResponse
- ITotpService: GenerateSecret, GenerateQrCodeUri, VerifyCode
- ITokenService: добавлены GenerateTwoFactorToken(Guid), GetUserIdFromTwoFactorToken(string)
- IUserRepository: добавлены методы для backup-кодов
- 6 новых команд: SetupTwoFactor, ConfirmTwoFactor, DisableTwoFactor, VerifyTwoFactor, RequestPasswordReset, ResetPassword
- LoginCommandHandler: теперь возвращает короткоживущий JWT (5 мин, purpose=2fa_verification) вместо пустых токенов при 2FA

Infrastructure:
- TotpService (Otp.NET): Base32 секрет, QR URI в формате otpauth://totp/, верификация с окном ±1 шаг
- JwtTokenService: GenerateTwoFactorToken (без role/tenant claims) + GetUserIdFromTwoFactorToken (валидирует lifetime + проверяет purpose claim)
- UserRepository: методы GetActiveBackupCodesAsync, AddBackupCodesAsync, DeleteAllBackupCodesAsync
- IdentityStartup: зарегистрирован ITotpService


### Шаг 8 — Application: Password Reset (день 6-7)
RequestPasswordReset, ResetPassword.

### Шаг 9 — Api: все оставшиеся endpoints (день 7)
/api/identity/users/*, /api/identity/invitations/*, /api/identity/tenant/settings.

Шаг 9 выполнен.                                                                                                                                                                                                           
---                                                                                                                                                                                                                                   Создано 4 partial-файла по образцу IdentityEndpoints.Auth.cs:                                                                                                                                                                       
IdentityEndpoints.Auth.cs (дополнен):
- POST /auth/login/2fa → VerifyTwoFactorHandler — устанавливает refresh cookie при успехе                                                                                                                                             - POST /auth/password/forgot → ForgotPasswordHandler — всегда 204 (не раскрывает существование email)                                                                                                                                 - POST /auth/password/reset → ResetPasswordHandler

IdentityEndpoints.Users.cs:
- GET/PUT /me, PUT /me/password, POST/DELETE /me/2fa/* — любой авторизованный
- GET /, GET /{id} — Manager+
- PUT /{id}/deactivate, PUT /{id}/activate — Admin+
- PUT /{id}/role — Owner only

IdentityEndpoints.Invitations.cs:
- POST / (create), GET / (list) — Admin+
- POST /accept — Anonymous (устанавливает refresh cookie)
- DELETE /{id} — Admin+

IdentityEndpoints.TenantSettings.cs:
- GET / — любой авторизованный
- PUT / — Admin+

UpdateTenantSettingsCommand — обновляет Timezone/Language/CompanyLogoUrl частично (null = не обновлять), через метод Tenant.UpdateSettings

### Шаг 10 — Blazor UI (день 8-9)
Страницы: логин, регистрация компании, профиль, список пользователей, приглашения, настройки 2FA.

### Шаг 11 — Shared.Contracts (день 9)
IUserProvider, ICurrentUserService, UserRegisteredIntegrationEvent, UserRegisteredDomainEventHandler — то, что нужно другим модулям.