using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ShitPosterBot2;
using ShitPosterBot2.Database;
using ShitPosterBot2.ExternValidators;
using ShitPosterBot2.MessageHandler;
using ShitPosterBot2.MessageHandler.UserHandlers;
using ShitPosterBot2.Repositories;
using ShitPosterBot2.Services;
using ShitPosterBot2.Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    // configure Logging with NLog
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
    loggingBuilder.AddNLog();
    loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
});

builder.Services.AddDbContext<BotContext>(options =>
    options.UseSqlite(
        $"Data Source={Path.Combine(Environment.CurrentDirectory, "DB", "botdata.db")}"),
    //$"Data Source=C:\\Projects\\ShitPosterBot2\\ShitPosterBot2\\bin\\Debug\\net7.0\\DB\\botdata.db"),
    ServiceLifetime.Singleton);

builder.Services.AddSingleton<TopSecretsRepository>();
builder.Services.AddSingleton<DomainsRepository>();
builder.Services.AddSingleton<IPostRepository, PostsRepository>();

builder.Services.AddTransient<IUserHandler, UserInlineQueryHandler>();
builder.Services.AddTransient<IUserHandler, UserMessageHandler>();

builder.Services.AddTransient<CommandsManager>();

builder.Services.AddTransient<IMessageHandler, TelegramMessageHandler>();

builder.Services.AddSingleton<IStatisticsService, StatisticsService>();
builder.Services.AddSingleton<IExternPostValidator, VkExternValidator>();
builder.Services.AddSingleton<ISplashService, SplashService>();

builder.Services.AddHostedService<BotHost>();

var host = builder.Build();

await host.RunAsync();
