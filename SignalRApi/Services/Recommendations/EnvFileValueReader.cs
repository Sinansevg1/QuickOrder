namespace SignalRApi.Services.Recommendations
{
    public static class EnvFileValueReader
    {
        private static Dictionary<string, string>? _cache;
        public static string? LastLoadedPath { get; private set; }
        private static readonly object Sync = new();

        public static string? Get(string key)
        {
            EnsureLoaded();
            return _cache!.TryGetValue(key, out var value) ? value : null;
        }

        private static void EnsureLoaded()
        {
            if (_cache is not null)
            {
                return;
            }

            lock (Sync)
            {
                if (_cache is not null)
                {
                    return;
                }

                _cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var candidates = new[]
                {
                    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env")),
                    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env")),
                    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                    Path.Combine(Directory.GetCurrentDirectory(), "..", ".env")
                };

                var envPath = candidates.FirstOrDefault(File.Exists);
                if (string.IsNullOrWhiteSpace(envPath))
                {
                    return;
                }
                LastLoadedPath = envPath;

                foreach (var rawLine in File.ReadAllLines(envPath))
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    var idx = line.IndexOf('=');
                    if (idx <= 0)
                    {
                        continue;
                    }

                    var key = line[..idx].Trim();
                    var value = line[(idx + 1)..].Trim();
                    _cache[key] = value;
                }
            }
        }
    }
}
