using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public class HelpCommand : IUserCommand
{
    public string Name => "help";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        await botClient.SendPhotoAsync(msg.From.Id, photo: "https://sun9-23.userapi.com/impg/llCnzMwTpgI0PVsG0CUb8aJ9IphwAmrfMECCeg/ipj2Vn5x06k.jpg?size=1024x1024&quality=95&sign=b06a8e1c6216ed5f31151182a7f9e30a&type=album",
            $"Чтобы получить оригинал картинки с ВК отправь ID поста из основного канала\\nID поста это цифры под постом, как на картинке выше {Environment.NewLine} {Environment.NewLine} Какие-то вопросы? Обращайся! \\n@Li_is");
    }
}