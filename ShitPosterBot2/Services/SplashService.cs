using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShitPosterBot2.Database;
using ShitPosterBot2.Shared;

namespace ShitPosterBot2.Services;

public class SplashService : ISplashService
{
    private readonly ILogger<SplashService> _logger;

    public SplashService(ILogger<SplashService> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetRandomText()
    {
        _logger.LogInformation("Получение рандомного сплеша");

        return GetSecret();
    }
    
     public static List<string> StealTextList = new List<string>();
     public static string GetSecret()
        {
            StealTextList.Add("Спизжено с");
            StealTextList.Add("Украдено у");
            StealTextList.Add("Своровано у");
            StealTextList.Add("Позаимствовано с");
            StealTextList.Add("Забрал у");
            StealTextList.Add("Отпиздил и спиздил с");
            StealTextList.Add("Нагло сваровал у");
            StealTextList.Add("Стянул у");
            StealTextList.Add("Стащил у");
            StealTextList.Add("Нагло стащил у");
            StealTextList.Add("Прикорманил с");
            StealTextList.Add("Угнал у");
            StealTextList.Add("Увел у");
            StealTextList.Add("Скоммуни́здил с");
            StealTextList.Add("Стырил у");
            StealTextList.Add("Слямзил у");
            StealTextList.Add("Стибрил у");
            StealTextList.Add("Спёр у");
            StealTextList.Add("Чухнул у");
            StealTextList.Add("Увёл из-под носа у");
            StealTextList.Add("Приделал ноги пикче из");
            StealTextList.Add("Сбондил с");
            StealTextList.Add("Слимонил у");
            StealTextList.Add("Умыкгул у");
            StealTextList.Add("Спионерил у");
            StealTextList.Add("Спизданул с");
            StealTextList.Add("Спиздохал у");
            StealTextList.Add("Вынес пикчу с");

            StealTextList.Add("Утащил без оглядки с");
            StealTextList.Add("Остаточно обездолил владельца");
            StealTextList.Add("Захватил на ходу с");
            StealTextList.Add("Позаимствовал для лучшего развития с");
            StealTextList.Add("Заполучил на свою сторону с");
            StealTextList.Add("Сбил со счета у");
            StealTextList.Add("Похвастался новым приобретением у");
            StealTextList.Add("Нажил у");
            StealTextList.Add("Привлек на сторону своей жадности с");
            StealTextList.Add("Снял с прилавка у");
            StealTextList.Add("Ворвался в дом владельца");
            StealTextList.Add("Вырвал из рук хозяина");
            StealTextList.Add("Отвлек глаза от владельца");
            StealTextList.Add("Набрал в кулак с");
            StealTextList.Add("Вытащил из-под носа");
            StealTextList.Add("Похитил на скорую руку с");
            StealTextList.Add("Откатал на свой счет у");
            StealTextList.Add("Покрыл тенью грабителя пикчу с");
            StealTextList.Add("Натренировал мастерство краж пикч с");
            StealTextList.Add("Смачно захапал с");
            StealTextList.Add("Оставил с носом хозяина");
            StealTextList.Add("Опустошил до дна трюм");
            StealTextList.Add("Похитил на сладость у");
            StealTextList.Add("Откупорил чужую бутылку у");
            StealTextList.Add("Завладел по нищенскому праву пикчей с");
            StealTextList.Add("Обогатился нечестным способом с");
            StealTextList.Add("Списал на свой счет с");
            StealTextList.Add("Взял на вооружение c");
            StealTextList.Add("Урвал по чужому у");
            StealTextList.Add("Вжился в роль вора из-за");
            StealTextList.Add("Втайне забрал с полки у");
            StealTextList.Add("Получил несанкционированный доступ к мемам из");
            StealTextList.Add("Срубил куш по-быстрому с");
            StealTextList.Add("Заработал на хлеб насущный у");
            StealTextList.Add("Выкрал на зло беднякам с");
            StealTextList.Add("Спрятал под крышу у");
            StealTextList.Add("Обездолил без права на возврат с");
            StealTextList.Add("Расхитил имущество врага народа у");
            StealTextList.Add("Воспользовался чужим опытом с");
            StealTextList.Add("Наварился на чужом горе с");
            StealTextList.Add("Сошел за шпиона у");
            StealTextList.Add("Скупил все подряд у");
            StealTextList.Add("Закрался незаметно у");
            StealTextList.Add("Навязал свои условия владельцу");
            StealTextList.Add("Приобрел на потоке с");
            StealTextList.Add("Поместил в свой карман из");
            StealTextList.Add("Смыл с полки у");
            StealTextList.Add("Выжал из кармана у");
            StealTextList.Add("Схитрил и забрал у");
            StealTextList.Add("Разобрал на цитаты у");
            StealTextList.Add("Надрал с куста у");
            StealTextList.Add("Подпер козырьком с");
            StealTextList.Add("Надергал на себя с");
            StealTextList.Add("Взял под охрану с");
            StealTextList.Add("Завладел на совесть с");
            StealTextList.Add("Гулял и нашел у");
            StealTextList.Add("Получил свою долю с");
            StealTextList.Add("Осуществил кражу безбоязненно с");
            StealTextList.Add("Поимел кое-что без разрешения у");
            StealTextList.Add("Внедрился и вынес с");
            StealTextList.Add("Похитил по старой памяти с");
            StealTextList.Add("Обеспечил себя сомнительными средствами с");
            StealTextList.Add("Заковыристо завладел с");
            StealTextList.Add("Надменно похитил с");
            StealTextList.Add("Сломя голову вытащил у");
            StealTextList.Add("Привез из Мексики мем от");
            StealTextList.Add("Сорвал урожай у фермера");
            StealTextList.Add("Устроил налет на банк");
            StealTextList.Add("Поставил на учет мем с");
            StealTextList.Add("Провел диверсию в личных целях на мем с");
            StealTextList.Add("Утащил на шару с");
            StealTextList.Add("Вырвал из пасти собаки");
            StealTextList.Add("Упер на свою банду с");
            StealTextList.Add("Вытащил из ниоткуда у");
            StealTextList.Add("Отхапал на халяву с");
            StealTextList.Add("Пригрелся к стороне тьмы у");
            StealTextList.Add("Укрепил свои запасы мемов за счет");
            StealTextList.Add("Попробовал на вкус мем из");
            StealTextList.Add("Присвоил нечто чужое из");
            StealTextList.Add("Принес на свой склад c");
            StealTextList.Add("Подслушал и вынес из");
            StealTextList.Add("Прибегнул к ловкости рук для");
            StealTextList.Add("Выступил в роли обыскателя для");
            StealTextList.Add("Подарил себе чужое из");





            /*
            StealTextList.Add("");
            StealTextList.Add("");
            StealTextList.Add("");
            StealTextList.Add("");
            StealTextList.Add("");
            StealTextList.Add("");
            StealTextList.Add("");
            StealTextList.Add("");
            StealTextList.Add("");
            */
            string[] strings = StealTextList.ToArray();
            Random random = new Random();
            string a;
            try
            {
                a = strings[random.Next(0, strings.Length)];
            }
            catch
            {
                a = "!" + strings[0];
            }
            
            return a;
        }
}