using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Collector;

public class VkCollectorSettings : ICollectorSettings
{
    /// <summary>
    /// Список доменов пабликов
    /// </summary>
    public List<string> Domains { get; set; }
    
    /// <summary>
    /// Токен вк
    /// </summary>
    public string Token { get; set; }
    
    /// <summary>
    /// Раз во сколько времени будет производиться сборка всех постов со всех пабликов
    /// </summary>
    public int TimeoutCollect { get; set; }
    
    /// <summary>
    /// Ожидание перед получением постов с паблика
    /// </summary>
    public int Timeout { get; set; }
    
    /// <summary>
    /// Ожидание перед обработки поста
    /// </summary>
    public int TimeoutPost { get; set; }
    
    /// <summary>
    /// Количество постов получаемых с паблика
    /// </summary>
    public ulong CountPosts { get; set; }
    
    /// <summary>
    /// Количество попыток получения постов с домена (если не успешно было)
    /// </summary>
    public int DomainsTryes { get; set; }
}