namespace SignalRApi.Middleware
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            var configuredKey = configuration["ApiSettings:ApiKey"];

            // API key konfigüre edilmemişse veya boşsa, middleware'i atla (geliştirme ortamı)
            if (string.IsNullOrEmpty(configuredKey))
            {
                await _next(context);
                return;
            }

            // Yalnızca /api/* yollarını koru; SignalR hub ve Swagger için kontrol yapma
            var path = context.Request.Path.Value ?? "";
            if (!path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedKey)
                || !configuredKey.Equals(extractedKey, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Geçersiz veya eksik API anahtarı.");
                return;
            }

            await _next(context);
        }
    }
}
