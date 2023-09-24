using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ShitPosterBot2;
using ShitPosterBot2.Database;
using ShitPosterBot2.ExternValidators;
using ShitPosterBot2.Repositories;
using ShitPosterBot2.Services;
using ShitPosterBot2.Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    // configure Logging with NLog
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
    loggingBuilder.AddNLog();
});

builder.Services.AddDbContext<BotContext>(options =>
    options.UseSqlite(
        //$"Data Source={ Path.Combine(Environment.CurrentDirectory, "DB", "botdata.db")}"),
    $"Data Source=C:\\Projects\\ShitPosterBot2\\ShitPosterBot2\\bin\\Debug\\net7.0\\DB\\botdata.db"),
    ServiceLifetime.Singleton);

builder.Services.AddTransient<TopSecretsRepository>();
builder.Services.AddTransient<DomainsRepository>();
builder.Services.AddTransient<IStatisticsService, StatisticsService>();
builder.Services.AddTransient<IExternPostValidator, VkExternValidator>();

builder.Services.AddHostedService<BotHost>();

var host = builder.Build();

await host.RunAsync();
