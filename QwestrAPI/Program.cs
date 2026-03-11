using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Qwestr.Modules.Analytics.Api;
using Qwestr.Modules.Gamification.Api;
using Qwestr.Modules.Identity.Api;
using Qwestr.Modules.Integration.Api;
using Qwestr.Modules.Notifications.Api;
using Qwestr.Modules.Onboarding.Api;
using Qwestr.Shared.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- Serilog ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ConfigureSwaggerWithJwtBearer();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Qwestr API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// --- JWT Authentication ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

// --- Подключение shared-библиотек
builder.Services.AddSharedInfrastructure();

// --- Подключение модулей ---
builder.Services.AddAnalyticsModule(builder.Configuration);
builder.Services.AddGamificationModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddIntegrationModule(builder.Configuration);
builder.Services.AddNotificationsModule(builder.Configuration);
builder.Services.AddOnboardingModule(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

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

try
{
    Log.Information("Starting QwestrAPI application");
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}