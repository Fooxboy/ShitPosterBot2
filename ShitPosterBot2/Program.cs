using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ShitPosterBot2;
using ShitPosterBot2.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    // configure Logging with NLog
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    loggingBuilder.AddNLog();
});

builder.Services.AddDbContext<BotContext>(options =>
    options.UseSqlite(
        "Data Source=C:\\Users\\daske\\source\\repos\\ShitPosterBot\\ShitPosterBotConsole\\DB\\botdata.db"));


builder.Services.AddHostedService<BotHost>();

var host = builder.Build();

await host.RunAsync();
