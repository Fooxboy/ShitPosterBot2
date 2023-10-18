// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using ShitPosterBot.Database;
using ShitPosterBot2.Shared.Models;
using ShitPosterBotConsole.Database;

Console.WriteLine("Hello, World!");



using (var oldConnection = new BotContext())
{
    Console.WriteLine("Получение всей инфы из старой бд...");
    var allAttachments = oldConnection.Attachments.ToList();
    
    /*var allPosts = oldConnection.Posts.ToList();
    var allDomains = oldConnection.Domains.ToList();*/
    
    Console.WriteLine("Получена инфа из старой бд");


    using (var newConnection = new ShitPosterBot2.Database.BotContext())
    {
        /*
        Console.WriteLine("Начало миграции доменов");
        foreach (var oldDomain in allDomains)
        {
            var newDomain = new ShitPosterBot2.Shared.Models.Domain();

            newDomain.Id = oldDomain.Id;
            newDomain.Emoji = oldDomain.Emoji ?? string.Empty;
            newDomain.Name = oldDomain.Name ?? "ОШИБКА";
            newDomain.Target = "-1001435506317";
            newDomain.ShowOriginalText = oldDomain.ShowOriginalText ?? false;

            newConnection.Domains.Add(newDomain);

            await newConnection.SaveChangesAsync();
        }
        
        Console.WriteLine("Домены смигрированы");
        
        */

        /*
        Console.WriteLine("Миграция постов");
        
        var newPosts = new List<ShitPosterBot2.Shared.Models.Post>();
        
        foreach (var oldPost in allPosts)
        {
            var newPost = new ShitPosterBot2.Shared.Models.Post();
            newPost.Id = oldPost.Id;
            newPost.Body = oldPost.OriginalText;
            newPost.Tryes = 0;
            newPost.CollectedAt = oldPost.CreatedTime;
            newPost.PlatformId = oldPost.VkId.ToString() ?? string.Empty;
            newPost.PlatformOwner = oldPost.Domain ?? string.Empty;
            newPost.PublishAt = oldPost.CreatedTime;
            newPost.DomainInfo = newConnection.Domains.FirstOrDefault(x => x.Name == oldPost.Domain) ?? newConnection.Domains.FirstOrDefault(x=> x.Name == "li_is17");

            newPosts.Add(newPost);
        }
        
        await newConnection.Posts.AddRangeAsync(newPosts);
            
        await newConnection.SaveChangesAsync(); 
        
        Console.WriteLine("Посты смигрированы");
        
        */

        Console.WriteLine("Загружаем новые посты");
        var newPosts = newConnection.Posts.ToList();
        
        Console.WriteLine("Новые посты загружены");

        var newAttachments = new ConcurrentBag<ShitPosterBot2.Shared.Models.PostAttachment>();
        
        Console.WriteLine("Миграция вложений");
        
        Parallel.ForEach(allAttachments, oldAttachment =>
        {
            var newAttachment = new ShitPosterBot2.Shared.Models.PostAttachment();

            newAttachment.Id = oldAttachment.Id;
            newAttachment.AttachmentType =  (AttachmentType)(int)oldAttachment.ContentType;
            newAttachment.Post = newPosts.FirstOrDefault(p => p.Id == oldAttachment.ParentPostId);
            newAttachment.Url = oldAttachment.Uri;

            newAttachments.Add(newAttachment);

            Console.WriteLine($"Создано вложение с id {newAttachment.Id}");
        });
        

        await newConnection.Attachments.AddRangeAsync(newAttachments);

        try
        {
            await newConnection.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            
        }
        
        Console.WriteLine("Вложения смигрированы");

    }
    
    Console.WriteLine("Миграция завершена");

}



