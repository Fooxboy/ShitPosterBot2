namespace ShitPosterBot2.MessageHandler;

public class TelegramMessageHandlerConfiguration : IMessageHandlerConfiguration
{
    public string TelegramToken { get; set; }
    
    public int SendMessageUserTimeout { get; set; }
}