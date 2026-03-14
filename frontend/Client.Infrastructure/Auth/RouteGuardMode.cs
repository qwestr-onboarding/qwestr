namespace Client.Infrastructure.Auth;

public enum RouteGuardMode
{
    /// <summary>Доступно всем, на любом домене</summary>
    Public,
    
    /// <summary>Доступно всем, на главном домене</summary>
    PublicOnly,

    /// <summary>Только неавторизованным, на домене тенанта (логин, 2FA)</summary>
    GuestOnTenant,

    /// <summary>Только неавторизованным, на главном домене (регистрация)</summary>
    GuestOnMain,

    /// <summary>Только авторизованным, на домене тенанта (страницы приложения)</summary>
    TenantOnly
}
