using Microsoft.Extensions.Logging;

public interface ICachedFileProcessor
{
    void Process(CacheEntry cachedEntry, string checksum);
}

public class CachedFileProcessor : ICachedFileProcessor
{
    private readonly Settings _settings;
    private readonly ILogger<CachedFileProcessor> _logger;

    public CachedFileProcessor(Settings settings, ILogger<CachedFileProcessor> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public void Process(CacheEntry cachedEntry, string checksum)
    {
        _logger.LogDebug($"Processing {cachedEntry.FileName}");

        string sourcePath = Path.Join(_settings.UtPath, "cache", checksum + ".uxx");

        if (!File.Exists(sourcePath))
        {
            _logger.LogError($"File not found: {sourcePath}");
            return;
        }

        string targetMap = DetermineTargetMap(cachedEntry.FileType);
        string targetPath = Path.Join(_settings.UtPath, targetMap, cachedEntry.FileName);

        if (File.Exists(targetPath))
        {
            _logger.LogDebug($"skipping existing file {targetPath}");
            return;
        }

        _logger.LogInformation($"Adding new file from cache: {targetPath}");

        try
        {
            File.Copy(sourcePath, targetPath);
        }
        catch(IOException ioEx)
        {
            _logger.LogError(ioEx.Message);
        }
    }

    private string DetermineTargetMap(CacheFileType fileType)
    {
        switch (fileType)
        {
            case CacheFileType.Map:
                return "Maps";
            case CacheFileType.Music:
                return "Music";
            case CacheFileType.Sound:
                return "Sounds";
            case CacheFileType.System:
                return "System";
            case CacheFileType.Texture:
                return "Textures";
            default:
                throw new Exception("Unknown FileType");
        }
    }
}