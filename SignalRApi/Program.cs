using FluentValidation;
using SignalR.BusinessLayer.Abstract;
using SignalR.BusinessLayer.Concrete;
using SignalR.BusinessLayer.Container;
using SignalR.BusinessLayer.ValidationRules.BookingValidations;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.concrete;
using SignalR.DataAccessLayer.EntityFreamwork;
using SignalRApi.Hubs;
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

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", builder =>
    {
       builder.AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed((Host) => true) 
        .AllowCredentials();
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
