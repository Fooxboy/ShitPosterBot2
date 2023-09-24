using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShitPosterBot2.Collector;
using ShitPosterBot2.Database;
using ShitPosterBot2.Repositories;
using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2;

public class BotHost : IHostedService
{
    private readonly List<IPostCollector> _collectors;

    private readonly ILogger<BotHost> _hostLogger;

    private readonly TopSecretsRepository _topSecretsRepository;

    private readonly IConfiguration _configuration;

    private readonly DomainsRepository _domainsRepository;
    
    private readonly ILogger<VkPostCollector> _collectorLogger;

    private readonly IStatisticsService _statisticsService;

    private readonly IExternPostValidator _externPostValidator;

    public BotHost(ILogger<BotHost> collectorsLogger, TopSecretsRepository topSecretsRepository, 
        IConfiguration configuration, 
        ILogger<VkPostCollector> collectorLogger, 
        IStatisticsService statisticsService, DomainsRepository domainsRepository, IExternPostValidator externPostValidator)
    {
        _collectors = new List<IPostCollector>();
        _hostLogger = collectorsLogger;
        _topSecretsRepository = topSecretsRepository;
        _configuration = configuration;
        _collectorLogger = collectorLogger;
        _statisticsService = statisticsService;
        _domainsRepository = domainsRepository;
        _externPostValidator = externPostValidator;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _hostLogger.LogInformation("Запуск хоста бота...");

        try
        {
            var collector = GetVkPostCollector(1);
            
            _collectors.Add(collector);
        }
        catch (Exception ex)
        {
            _hostLogger.LogInformation("Ошибка при создании ВК коллектора...");
            _hostLogger.LogError(ex, ex.Message);
        }
        
        _hostLogger.LogInformation("Подписка на события коллекторов");

        foreach (var postCollector in _collectors)
        {
            postCollector.NewPostParsed += PostCollectorOnNewPostParsed;
            postCollector.PostCollectorCrashed += PostCollectorOnPostCollectorCrashed;
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
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _hostLogger.LogInformation("Попытка остановки коллекторов...");

        foreach (var postCollector in _collectors)
        {
            postCollector.StopCollector();
        }
    }
    
    private void PostCollectorOnPostCollectorCrashed(Exception ex, IPostCollector collector)
    {
        _hostLogger.LogInformation($"Крашнулся коллектор '{collector.Name}'. Причина: {ex.Message}");
        _hostLogger.LogError(ex, ex.Message);
    }

    private void PostCollectorOnNewPostParsed(Post post)
    {
        _hostLogger.LogInformation("ебать новый пост спарсился");
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

    private IPostCollector GetVkPostCollector(int instance)
    {
        
        _hostLogger.LogInformation($"Создание ВК коллектора #P{instance}");

        var vkCollector = new VkPostCollector(_collectorLogger, _statisticsService, _externPostValidator, instance);

        return vkCollector;
    }
}