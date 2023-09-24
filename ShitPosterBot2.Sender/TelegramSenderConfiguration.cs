namespace ShitPosterBot2.Sender;

public class TelegramSenderConfiguration : ISenderConfiguration
{
    public string TelegramBotToken { get; set; }
    
    
    public int TimeoutPost { get; set; }
    
    public int TimeoutQueue { get; set; }
    
    public int MaxPostTryes { get; set; }
}