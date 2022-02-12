using Microsoft.Extensions.Logging;

public interface ICacheReader
{
    Task<IDictionary<string, CacheEntry>> ReadCache(string path);
}

public class CacheReader : ICacheReader
{
    private ILogger<CacheReader> _logger;

    public CacheReader(ILogger<CacheReader> logger)
    {
        _logger = logger;
    }

    public async Task<IDictionary<string, CacheEntry>> ReadCache(string path)
    {
        var dictionary = new Dictionary<string, CacheEntry>();

        _logger.LogInformation($"Reading file {path}");
        var lines = await File.ReadAllLinesAsync(path);
        _logger.LogDebug($"{lines.Length} lines read.");

        _logger.LogInformation("Parsing lines");

        foreach (var line in lines.Skip(1))
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                (string key, CacheEntry value) = ParseLine(line);
                dictionary.Add(key, value);
            }
        }

        _logger.LogDebug($"{dictionary.Count} lines sucessfully parsed");
        return dictionary;
    }

    private (string checksum, CacheEntry cacheEntry) ParseLine(string line)
    {
        _logger.LogDebug($"Parsing line: {line}");
        string checksum = line.Substring(0, 32);
        string fileName = line.Substring(33);

        var cacheType = DetermineFileType(Path.GetExtension(fileName));

        return (checksum, new CacheEntry { FileName = fileName, FileType = cacheType });
    }

    private CacheFileType DetermineFileType(string extension)
    {
        string upper = extension.ToUpper();
        switch (upper)
        {
            case ".UMX":
                return CacheFileType.Music;
            case ".UAX":
                return CacheFileType.Sound;
            case ".UNR":
                return CacheFileType.Map;
            case ".UTX":
                return CacheFileType.Texture;
            case ".U":
                return CacheFileType.System;
            default:
                return CacheFileType.Unknown;
        }
    }
}