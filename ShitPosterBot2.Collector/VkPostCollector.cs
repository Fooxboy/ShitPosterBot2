using Microsoft.Extensions.Logging;
using ShitPosterBot2.Collector.VKontakte;
using ShitPosterBot2.Collector.VKontakte.Hadlers;
using ShitPosterBot2.Collector.VKontakte.Validators;
using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;
using VkNet.Model;
using Post = ShitPosterBot2.Shared.Models.Post;

namespace ShitPosterBot2.Collector;

public class VkPostCollector : IPostCollector
{
    public string Name => "ВК коллектор";
    
    private readonly ILogger<VkPostCollector> _logger;

    private VkCollectorSettings _settings;
    
    private readonly int _instance;

    private readonly IStatisticsService _statisticsService;

    private  PostManager _vkPostManager;

    private List<IAttachmentValidator> _attachmentValidators;
    private List<IAttachmentHandler> _attachmentHandlers;

    private bool _isRun;
    
    public VkPostCollector(ILogger<VkPostCollector> logger, IStatisticsService statisticsService, int instance = 1)
    {
        _statisticsService = statisticsService;
        _instance = instance;
        _logger = logger;
    }

    public event Action<Post>? NewPostParsed;
    
    public event Action<Exception>? PostCollectorCrashed;

    public async Task RunCollectorAsync(ICollectorSettings settings)
    {
        _isRun = true;
        
        _logger.LogInformation($"Запуск сборщика постов с именем '{GetName()}'");

        if (settings is not VkCollectorSettings collectorSettings)
        {
            var settingsErrorMessage = $"Невозможно запустить сбощик постов с именем '{GetName()}'," +
                                       $" потому что переданы не верные настройки";
            
            PostCollectorCrashed?.Invoke(new Exception(settingsErrorMessage));

            return;
        }

        _settings = collectorSettings;
        
        try
        {
            _vkPostManager = new PostManager(_settings.Token, _logger);

            _attachmentValidators = new List<IAttachmentValidator>()
            {
                //вот они слева направа
                new GifValidator(), new ImageValidator(), new VideoValidator(_vkPostManager)
            };

            _attachmentHandlers = new List<IAttachmentHandler>()
            {
                //вот они справа налево
                new GifHandler(), new ImageHandler(), new VideoHandler(_vkPostManager)
            };

            await RunCollectPosts();
        }
        catch (Exception ex)
        {
            PostCollectorCrashed?.Invoke(ex);
        }
    }

    public void StopCollector()
    {
        _logger.LogInformation("Бот остановится при следующем получении постов");
        _isRun = false;
    }

    private async Task RunCollectPosts()
    {
        while (_isRun)
        {
            var domains = new List<string>();
            domains.AddRange(_settings.Domains);
            var domainTryes = 0;

            //Пока есть хотя бы один паблик в списке ошибок, будем крутить.
            while (domains.Any() && domainTryes < _settings.DomainsTryes)
            {
                for (var i = 0; i < domains.Count; i++)
                {
                    var domain = domains[i];

                    try
                    {
                        await ProcessDomain(domain);
                        
                        domains.RemoveAt(i);

                    } catch (Exception ex)
                    {
                        _logger.LogInformation($"Произошла ошибка при получении постов с паблика {domain}. Оставляем паблик в очереди");
                        _logger.LogError(ex, ex.Message);
                    }
                }

                domainTryes++;
            }
            
            Thread.Sleep(_settings.TimeoutCollect);
        }
    }

    private async Task ProcessDomain(string domain)
    {
        _logger.LogInformation($"Получение постов с паблика {domain}");
        
        _logger.LogInformation($"Ждем таймаут перед получением постов {_settings.Timeout}");
        
        Thread.Sleep(_settings.Timeout);
        
        var vkPosts = await _vkPostManager.GetPostsAsync(domain, _settings.CountPosts);

        _logger.LogInformation($"Получено {vkPosts.Count} постов с паблика {domain}");

        foreach (var vkPost in vkPosts)
        {
            await ProcessPost(vkPost, domain);
        }
    }

    private async Task ProcessPost(VkNet.Model.Post vkPost, string domain)
    {
        _logger.LogInformation($"Ждем таймаут перед обработкой поста {_settings.TimeoutPost}");
        
        Thread.Sleep(_settings.TimeoutPost);

        try
        {
            var hasCopyright = await _vkPostManager.CheckCopyright($"{vkPost.OwnerId}_{vkPost.Id}");

            if (hasCopyright || vkPost.MarkedAsAds)
            {
                await _statisticsService.AddAdsPost(domain);
                
                _logger.LogInformation($"Пост {vkPost.Id} из паблика {domain} определен как рекламный. Пропускаем его");

                return;
            }

            _logger.LogInformation($"Начало обработки поста {vkPost.Id} из паблика {domain}");

            var post = new Post();

            post.Body = vkPost.Text;
            post.PlatformOwner = domain;
            post.PlatformId = vkPost.Id.ToString();
            post.CollectedAt = DateTime.UtcNow;
            post.Attachments = new List<PostAttachment>();

            _logger.LogInformation($"Начало обработки вложений поста {vkPost.Id} из паблика {domain}, количество: {post.Attachments.Count}");

            foreach (var vkPostAttachment in vkPost.Attachments)
            {
                var attachment = await ProcessAttachment(vkPostAttachment);

                if (attachment is not null)
                {
                    post.Attachments.Add(attachment);
                }
            }

            if (post.Attachments.Any())
            {
                NewPostParsed?.Invoke(post);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Произошла ошибка при обработки поста {vkPost.Id} из паблика {domain}. Пропускаем этот пост.");
            
            _logger.LogError(ex, ex.Message);
        }
    }

    private async Task<PostAttachment?> ProcessAttachment(Attachment attachment)
    {
        try
        {
            var validator = _attachmentValidators.FirstOrDefault(v => v.ValidationType == attachment.Type);
            
            if(validator is null)
            {
                _logger.LogInformation($"Не найден валидатор для вложения с типом {attachment.Type}");

                return null;
            }

            ValidationResult validationResult;
            try
            {
                validationResult = await validator.ValidateAsync(attachment.Instance);
            }catch(Exception ex)
            {
                _logger.LogInformation($"Ошибка при работе валидатора: {ex}");
                _logger.LogError(ex, ex.Message);

                return null;
            }

            if (!validationResult.IsSuccess)
            {
                _logger.LogInformation($"Вложение не прошло валидацию по причине: {validationResult.Message}");

                return null;
            }
            
            var handler = _attachmentHandlers.FirstOrDefault(h => h.HandlerType == attachment.Type);

            if(handler is null)
            {
                _logger.LogInformation($"Не найден хендлер для типа {attachment.Type}");

                return null;
            }

            var postAttachment = await handler.Handle(attachment.Instance);

            if (postAttachment is null)
            {
                _logger.LogInformation("Хендлер ничего не вернул лол");

                return null;
            }

            return postAttachment;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Произошла ошибка при обработке вложения  {ex}");
            _logger.LogError(ex, ex.Message);

            return null;
        }
    }

    private string GetName()
    {
        return $"{Name} #{_instance}";
    }
}