using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public class StartCommand : IUserCommand
{
    public string Name => "start";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        await botClient.SendPhotoAsync(msg.From.Id, "https://sun9-23.userapi.com/impg/llCnzMwTpgI0PVsG0CUb8aJ9IphwAmrfMECCeg/ipj2Vn5x06k.jpg?size=1024x1024&quality=95&sign=b06a8e1c6216ed5f31151182a7f9e30a&type=album"
            ,$"СТАРТУЕМ! {Environment.NewLine} {Environment.NewLine}" +
                                                          $" Отправь мне ID поста и я верну тебе оригинал картинки из ВК\\n" +
                                                          $"ID поста это цифры под постом, вот пример:");
    }
}