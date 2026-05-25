using FluentValidation;
using SignalR.BusinessLayer.Abstract;
using SignalR.BusinessLayer.Concrete;
using SignalR.BusinessLayer.Container;
using SignalR.BusinessLayer.ValidationRules.BookingValidations;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.concrete;
using SignalR.DataAccessLayer.EntityFreamwork;
using SignalRApi.Hubs;
using SignalRApi.Middleware;
using SignalRApi.Options;
using SignalRApi.Services.Recommendations;
using System.Reflection;
using System.Text.Json.Serialization;

var rootEnvPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
var localEnvPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
var envConfig = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

if (File.Exists(rootEnvPath))
{
    LoadEnvVariables(rootEnvPath, envConfig);
}
else if (File.Exists(localEnvPath))
{
    LoadEnvVariables(localEnvPath, envConfig);
}

var builder = WebApplication.CreateBuilder(args);
if (envConfig.Count > 0)
{
    builder.Configuration.AddInMemoryCollection(envConfig);
}

var allowedOriginsRaw = builder.Configuration["ApiSettings:AllowedOrigins"] ?? "";
var allowedOrigins = allowedOriginsRaw
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // Geliştirme ortamı için fallback — production'da AllowedOrigins ayarlanmalı
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .SetIsOriginAllowed(_ => true)
                  .AllowCredentials();
        }
    });
});
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<SignalRContext>();
builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

builder.Services.ContainerDependencies();

builder.Services.AddValidatorsFromAssemblyContaining<CreateBookingValidation>();

builder.Services.AddControllersWithViews().AddJsonOptions(options=> options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);

builder.Services.AddControllers();
builder.Services.Configure<LlmRecommendationOptions>(builder.Configuration.GetSection("LlmRecommendation"));
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddHttpClient<OpenAiLlmProvider>();
builder.Services.AddHttpClient<GeminiLlmProvider>();
builder.Services.AddScoped<ILlmProvider>(sp => sp.GetRequiredService<OpenAiLlmProvider>());
builder.Services.AddScoped<ILlmProvider>(sp => sp.GetRequiredService<GeminiLlmProvider>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();


app.MapControllers();
app.MapHub<SignalRHub>("/signalrhub");

app.Run();

static void LoadEnvVariables(string path, IDictionary<string, string?> configValues)
{
    foreach (var rawLine in File.ReadAllLines(path))
    {
        var line = rawLine.Trim();
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
        {
            continue;
        }

        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = line[..separatorIndex].Trim();
        var value = line[(separatorIndex + 1)..].Trim();
        Environment.SetEnvironmentVariable(key, value);
        configValues[key.Replace("__", ":", StringComparison.Ordinal)] = value;
    }
}
