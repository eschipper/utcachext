using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


using IHost host = Host.CreateDefaultBuilder(args)
    .UseConsoleLifetime()
    .ConfigureServices((hostContext, services) => 
    {
        services.AddSingleton<Settings>((sp) => hostContext.Configuration.GetRequiredSection("Settings").Get<Settings>());
        services.AddScoped<ICacheExtractor, CacheExtractor>();
        services.AddScoped<ICacheReader, CacheReader>();
        services.AddScoped<ICachedFileProcessor, CachedFileProcessor>();
    })
    .Build();

await host.StartAsync();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var cacheExtractor = host.Services.GetRequiredService<ICacheExtractor>();

await cacheExtractor.Run();

lifetime.StopApplication();
await host.WaitForShutdownAsync();