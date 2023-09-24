namespace ShitPosterBot2.MessageHandler;

public interface IMessageHandler
{
    public Task RunMesageHandler(IMessageHandlerConfiguration configuration);
}