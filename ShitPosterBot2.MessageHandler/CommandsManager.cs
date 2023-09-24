using System.Reflection;
using Microsoft.Extensions.Logging;
using ShitPosterBot2.MessageHandler.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler;

public class CommandsManager
{
    private readonly ILogger<CommandsManager> _logger;
    
    public List<IUserCommand> Commands = new();
    
    public CommandsManager(ILogger<CommandsManager> logger)
    {
        _logger = logger;
        
        //Магия, которая за нас находит все комманды, которые реализуют интерфейс IUserCommand, чтобы ручками не пришлось их вписывать при каждом добавлении
        var types = Assembly.GetExecutingAssembly().GetTypes();

        var userCommandTypes = types.Where(t => typeof(IUserCommand).IsAssignableFrom(t) && !t.IsInterface);

        Commands = userCommandTypes.Select(t=> (IUserCommand) Activator.CreateInstance(t)).ToList();
    }
    
    public async Task ProccessCommand(string command, Message msg, ITelegramBotClient botClient)
    {
        var classCommand = Commands.FirstOrDefault(c => c.Name.ToLower() == command.ToLower());

        if (classCommand is null)
        {
            _logger.LogError($"Команда {command} не найдена епта");
            return;
        }

        await classCommand.Invoke(msg, botClient);
    }

}