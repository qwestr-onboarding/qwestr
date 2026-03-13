using Client.Infrastructure;
using MudBlazor.Services;
using QwestrWebClient;
using QwestrWebClient.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- MudBlazor ---
builder.Services.AddMudServices();

// --- Qwestr API ---
var apiBaseUrl = builder.Configuration["QwestrApi:BaseUrl"]
    ?? throw new InvalidOperationException("QwestrApi:BaseUrl is not configured.");
builder.Services.AddQwestrApi(apiBaseUrl);

// --- BFF backend client (no AuthHeaderHandler, raw proxy) ---
builder.Services.AddHttpClient("Backend", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapBffAuth();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();