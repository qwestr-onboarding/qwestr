using Analytics.Api;
using Gamification.Api;
using Identity.Api;
using Integration.Api;
using Notifications.Api;
using Onboarding.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Подключение модулей ---
builder.Services.AddAnalyticsModule(builder.Configuration);
builder.Services.AddGamificationModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddIntegrationModule(builder.Configuration);
builder.Services.AddNotificationsModule(builder.Configuration);
builder.Services.AddOnboardingModule(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- Endpoint'ы модулей
app.MapAnalyticsModule();
app.MapGamificationModule();
app.MapIdentityModule();
app.MapIntegrationModule();
app.MapNotificationsModule();
app.MapOnboardingModule();


app.UseHttpsRedirection();

app.Run();