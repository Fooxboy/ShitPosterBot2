using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShitPosterBot2.Collector;
using ShitPosterBot2.Database;
using ShitPosterBot2.MessageHandler;
using ShitPosterBot2.Repositories;
using ShitPosterBot2.Sender;
using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2;

public class BotHost : IHostedService
{
    private readonly List<IPostCollector> _collectors;
    
    private readonly List<IPostSender> _senders;

    private readonly ILogger<BotHost> _hostLogger;

    private readonly TopSecretsRepository _topSecretsRepository;

    private readonly IConfiguration _configuration;

    private readonly DomainsRepository _domainsRepository;
    
    private readonly ILogger<VkPostCollector> _collectorLogger;

    private readonly ILogger<TelegramPostSender> _senderLogger;

    private readonly IStatisticsService _statisticsService;

    private readonly IExternPostValidator _externPostValidator;

    private readonly IPostRepository _postsRepository;

    private readonly ISplashService _splashService;

    private readonly IMessageHandler _messageHandler;

    public BotHost(ILogger<BotHost> collectorsLogger, TopSecretsRepository topSecretsRepository, 
        IConfiguration configuration, 
        ILogger<VkPostCollector> collectorLogger, 
        IStatisticsService statisticsService, DomainsRepository domainsRepository, IExternPostValidator externPostValidator,
        IPostRepository postsRepository, ILogger<TelegramPostSender> senderLogger, ISplashService splashService, 
        IMessageHandler messageHandler)
    {
        _collectors = new List<IPostCollector>();
        _senders = new List<IPostSender>();
        _hostLogger = collectorsLogger;
        _topSecretsRepository = topSecretsRepository;
        _configuration = configuration;
        _collectorLogger = collectorLogger;
        _statisticsService = statisticsService;
        _domainsRepository = domainsRepository;
        _externPostValidator = externPostValidator;
        _postsRepository = postsRepository;
        _senderLogger = senderLogger;
        _splashService = splashService;
        _messageHandler = messageHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _hostLogger.LogInformation("Запуск хоста бота...");

        try
        {
            var collector = GetVkPostCollector(1);
            
            _collectors.Add(collector);

            var sender = GetTgPostSender(1);
            
            _senders.Add(sender);
            
        }
        catch (Exception ex)
        {
            _hostLogger.LogInformation("Ошибка при создании ВК коллектора или ТГ сендера...");
            _hostLogger.LogError(ex, ex.Message);
        }
        
        _hostLogger.LogInformation("Подписка на события коллекторов");

        foreach (var postCollector in _collectors)
        {
            postCollector.NewPostParsed += PostCollectorOnNewPostParsed;
            postCollector.PostCollectorCrashed += PostCollectorOnPostCollectorCrashed;
        }

        _hostLogger.LogInformation("Подписка на события сендеров");

        foreach (var postSender in _senders)
        {
            postSender.PostSended += PostSenderOnPostSended;
            postSender.SenderCrashed += PostSenderOnSenderCrashed;
        }

        try
        {
            RunCollectors();
        }
        catch (Exception ex)
        {
            _hostLogger.LogError("Ошибка при запуске  коллекторов");
            _hostLogger.LogError(ex, ex.Message);
        }

        try
        {
            RunSenders();
        }
        catch (Exception ex)
        {
            _hostLogger.LogError("Ошибка при запуске  сендеров");
            _hostLogger.LogError(ex, ex.Message);
        }
        
        _hostLogger.LogInformation("Запуск месседж хендлера");

        var handlerConfiguration = new TelegramMessageHandlerConfiguration()
        {
            TelegramToken = _topSecretsRepository.GetSecret(TopSecretsKeys.TokenTelegramBot)
        };


        new Thread(async () =>
        {
            await _messageHandler.RunMesageHandler(handlerConfiguration);

        }).Start();
    }

    private void PostSenderOnSenderCrashed(Exception ex, IPostSender sender)
    {
        _hostLogger.LogInformation($"Крашнулся сендер '{sender.Name}'. Причина: {ex.Message}");
        _hostLogger.LogError(ex, ex.Message);
    }

    private async Task PostSenderOnPostSended(Post post)
    {
        _hostLogger.LogInformation($"Отправлен пост {post.PlatformId} паблика {post.DomainInfo.Name} в канал {post.DomainInfo.Target}");
        
        _hostLogger.LogInformation("Попытка изменить дату оптравления...");

        try
        {
            await _postsRepository.UpdateSentTime(post, DateTime.UtcNow);
            
            _hostLogger.LogInformation("Дата публикации успешно изменена.");

        }
        catch (Exception ex)
        {
            _hostLogger.LogError($"Ошибка при изменении даты публикации: {ex.Message}");
            _hostLogger.LogError(ex, ex.Message);
        }
        
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _hostLogger.LogInformation("Попытка остановки коллекторов...");

        foreach (var postCollector in _collectors)
        {
            postCollector.StopCollector();
        }
        
        _hostLogger.LogInformation("Попытка остановки сендеров...");

        foreach (var postCollector in _senders)
        {
            await postCollector.StopSender();
        }
    }
    
    private void PostCollectorOnPostCollectorCrashed(Exception ex, IPostCollector collector)
    {
        _hostLogger.LogInformation($"Крашнулся коллектор '{collector.Name}'. Причина: {ex.Message}");
        _hostLogger.LogError(ex, ex.Message);
    }

    private async Task PostCollectorOnNewPostParsed(Post post)
    {
        _hostLogger.LogInformation($"Спарсился новый пост {post.PlatformId}");
        
        _hostLogger.LogInformation("Добавление поста в БД");

        try
        {
            var dbPost = await _postsRepository.AddPostAsync(post);

            if (dbPost is null)
            {
                _hostLogger.LogError($"Мы не нашли в базе данных пост {post.PlatformId}");
            }
            
            _hostLogger.LogInformation("Добавление поста в очередь на отправку");

            var sender = _senders.MaxBy(x => x.CountPostsInQueue);

            await sender.AddToQueue(dbPost);
        }
        catch (Exception ex)
        {
            _hostLogger.LogInformation($"Ошибка при добавлении поста в БД или при добавлении поста в очередь: {ex.Message} ");
            _hostLogger.LogError(ex, ex.Message);
        }
    }

    private void RunCollectors()
    {
        _hostLogger.LogInformation("Запуск коллекторов...");
        
        _hostLogger.LogInformation("Инициализация настроек для настроек для ВК коллектора");

        var vkCollectorSettings = new VkCollectorSettings();

        vkCollectorSettings.TimeoutCollect = _configuration.GetValue<int>("TimeoutCollect");
        vkCollectorSettings.CountPosts = _configuration.GetValue<uint>("CountPosts");
        vkCollectorSettings.Timeout = _configuration.GetValue<int>("Timeout");
        vkCollectorSettings.TimeoutPost = _configuration.GetValue<int>("TimeoutPost");
        vkCollectorSettings.DomainsTryes = _configuration.GetValue<int>("DomainsTryes");
        
        var tokenVk = _topSecretsRepository.GetSecret(TopSecretsKeys.TokenVk);

        if (tokenVk is null)
        {
            _hostLogger.LogError("Не получен токен из топ сикретов. Остановка запуска коллектора");

            return;
        }

        vkCollectorSettings.Token = tokenVk;

        var domains =
            vkCollectorSettings.Domains = _domainsRepository.GetAllDomains();

        vkCollectorSettings.Domains = domains;

        foreach (var postCollector in _collectors)
        {
            new Thread(async () =>
            {
                _hostLogger.LogInformation($"Запуск коллектора {postCollector.Name}");

                await postCollector.RunCollectorAsync(vkCollectorSettings);
                
                _hostLogger.LogInformation($"Запущен коллектор {postCollector.Name}");

            }).Start();
        }
    }

    private void RunSenders()
    {
        _hostLogger.LogInformation("Запуск сендеров...");

        var tgSenderSettings = new TelegramSenderConfiguration();
        tgSenderSettings.TimeoutPost = _configuration.GetValue<int>("SendPostTimeout");
        tgSenderSettings.TimeoutQueue = _configuration.GetValue<int>("SendQueueTimeout");
        tgSenderSettings.MaxPostTryes = _configuration.GetValue<int>("MaxPostTryes");
        
        var telegramToken =  _topSecretsRepository.GetSecret(TopSecretsKeys.TokenTelegramBot);
        
        if (telegramToken is null)
        {
            _hostLogger.LogError("Не получен токен телеграма из топ сикретов. Остановка запуска сендеров");

            return;
        }

        tgSenderSettings.TelegramBotToken = telegramToken;

        foreach (var postSender in _senders)
        {
            new Thread(async () =>
            {
                _hostLogger.LogInformation($"Запуск сендера {postSender.Name}");

                await postSender.RunSender(tgSenderSettings);
                
                _hostLogger.LogInformation($"Запущен сендер {postSender.Name}");
            }).Start();
        }
    }

    private IPostCollector GetVkPostCollector(int instance)
    {
        _hostLogger.LogInformation($"Создание ВК коллектора #P{instance}");

        var vkCollector = new VkPostCollector(_collectorLogger, _statisticsService, _externPostValidator, instance);

        return vkCollector;
    }

    private IPostSender GetTgPostSender(int instance)
    {
        _hostLogger.LogInformation($"Создание tg сендера");

        var tgSender = new TelegramPostSender(_senderLogger, _splashService);

        return tgSender;
    }
}