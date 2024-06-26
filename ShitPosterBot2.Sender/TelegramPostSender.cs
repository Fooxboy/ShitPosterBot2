using Microsoft.Extensions.Logging;
using ShitPosterBot2.Sender.Telegram;
using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Sender;

public class TelegramPostSender : IPostSender
{
    private readonly ILogger<TelegramPostSender> _logger;

    private TelegramSenderService _telegramSenderService;
    
    private readonly ISplashService _splashService;
    
    private TelegramSenderConfiguration _configuration;
    
    public int CountPostsInQueue => _postsQueue.Count;

    private bool _isRun;

    public string Name => "Telegram";
    
    public event Action<Exception, IPostSender>? SenderCrashed;
    
    public event Func<Post, Task>? PostSended;

    private List<Post> _postsQueue = new();
    private List<Post> _postsBuffer = new();

    public TelegramPostSender(ILogger<TelegramPostSender> logger, ISplashService splashService)
    {
        _logger = logger;
        _splashService = splashService;
    }

    public async Task AddToQueue(Post post)
    {
        _postsBuffer.Add(post);
    }

    public async Task RunSender(ISenderConfiguration configuration)
    {
        _isRun = true;
        _logger.LogInformation($"Запуск сендера '{Name}'");

        if (configuration is not TelegramSenderConfiguration tgConfiguration)
        {
            _logger.LogError("Переданы неверные параметры. Работа сендера будет остановлена");
            
            SenderCrashed?.Invoke(new Exception("Не указаны параметры для telegram sender"), this);

            return;
        }

        _configuration = tgConfiguration;

        _telegramSenderService = new TelegramSenderService(_configuration.TelegramBotToken);
        
        while (_isRun)
        {
            var removePosts = new List<Post>();

            foreach (var post in _postsQueue)
            {
                //генерация текста
                _logger.LogInformation($"Начало отправки поста {post.PlatformOwner} - {post.PlatformId} для отправки в {post.DomainInfo.Target}");
                var vkName = $"[паблика](https://vk.com/{post.DomainInfo.Name}?w=wall{post.PlatformId}) {post.DomainInfo.Emoji}";
                
                var text = string.Join(" ", await _splashService.GetRandomText(), vkName, post.DomainInfo.ShowOriginalText ? Environment.NewLine + post.Body : string.Empty /*,  Environment.NewLine, Environment.NewLine, $"ID: `{post.Id}`" */); //убрать закоментированнойть для вывода id поста  

                try
                {
                    _logger.LogInformation($"Ожидание таймаута перед постом: {_configuration.TimeoutPost}");
                    Thread.Sleep(_configuration.TimeoutPost);
                    
                    _logger.LogInformation("Отправка поста в телеграм");
                    await _telegramSenderService.SendPostAsync(post, text);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Произошла ошибка при отправке поста в телегу: {ex.Message}");
                    
                    _logger.LogError(ex, ex.Message);

                    post.Tryes++;
                    
                    if (post.Tryes >= _configuration.MaxPostTryes)
                    {
                        removePosts.Add(post);
                    }
                    
                    continue;
                }
               
                removePosts.Add(post);
                PostSended?.Invoke(post);
            }
                
            //Удаляем из очереди успешно отправленные посты ебать.
            foreach (var removeIndex in removePosts)
            {
                var index = _postsQueue.IndexOf(removeIndex);
                
                _postsQueue.RemoveAt(index);
            }
            
            lock (_postsBuffer)
            {
                _postsQueue.AddRange(_postsBuffer);
                _postsBuffer.Clear();
            }
            
            _logger.LogInformation($"Ждем таймаут полной переборки очереди ({_configuration.TimeoutQueue} ms)");
            Thread.Sleep(_configuration.TimeoutQueue);
        }
    }

    public async Task StopSender()
    {
        _isRun = false;
    }
}