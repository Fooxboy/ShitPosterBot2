namespace ShitPosterBot2.MessageHandler;

public interface IMessageHandler
{
    public Task RunMesageHandler(IMessageHandlerConfiguration configuration);

    public Task SendMessage(string message, string target);
}