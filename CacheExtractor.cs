using Microsoft.Extensions.Logging;

public interface ICacheExtractor
{
    Task Run();
}

public class CacheExtractor : ICacheExtractor
{
    private readonly ICacheReader _cacheReader;
    private readonly Settings _settings;
    private readonly ICachedFileProcessor _cachedFileProcessor;
    private readonly ILogger<CacheExtractor> _logger;

    public CacheExtractor(ICacheReader cacheReader, Settings settings, ICachedFileProcessor cachedFileProcessor, ILogger<CacheExtractor> logger)
    {
        _cacheReader = cacheReader;
        _settings = settings;
        _cachedFileProcessor = cachedFileProcessor;
        _logger = logger;
    }

    public async Task Run()
    {
        var cache = await _cacheReader.ReadCache(Path.Join(_settings.UtPath, "cache", "cache.ini"));
        foreach (var cachedFile in cache)
        {
            if (cachedFile.Value.FileType == CacheFileType.Unknown)
            {
                _logger.LogWarning($"Skipping unknown file type for file {cachedFile.Value.FileName}");
                continue;
            }

            _cachedFileProcessor.Process(cachedFile.Value, cachedFile.Key);
        }
    }
}