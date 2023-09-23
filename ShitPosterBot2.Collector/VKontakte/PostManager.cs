using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShitPosterBot2.Collector.VKontakte.Models;
using VkNet;
using VkNet.Model;
using VkNet.Utils;

namespace ShitPosterBot2.Collector.VKontakte
{
    public class PostManager
    {
        private readonly VkApi _vkApi;
        private readonly ILogger<VkPostCollector> _logger;
    
        public PostManager(string token, ILogger<VkPostCollector> logger)
        {
            _logger =  logger;

            _vkApi = new VkApi();
            _vkApi.Authorize(new VkNet.Model.ApiAuthParams() { AccessToken = token });
        }
    
        public async Task<ReadOnlyCollection<Post>> GetPostsAsync(string domain, ulong count = 10)
        {
            var posts = await _vkApi.Wall.GetAsync(new WallGetParams() { Count = count, Domain = domain });
                
            return posts.WallPosts;
        }


        public async Task<bool> CheckCopyright(string postId)
        {
            var parameters = new VkParameters
            {
                {"posts", postId},
                {"access_token", _vkApi.Token },
                {"v", "5.131" }
            };
            
            var json = await _vkApi.InvokeAsync("wall.getById", parameters);

            var model = JsonConvert.DeserializeObject<PostResponse>(json);

            if (model is null)
            {
                _logger.LogError($"ВКонтакте не вернул пост для проверки копирайта с postId = {postId}");

                return true;
            }
            
            var post = model.Response.SingleOrDefault();

            if (post is null)
            {
                _logger.LogError($"ВКонтакте не вернул пост для проверки копирайта с postId = {postId}");

                return true;
            }

            return post.Copyright != null;
        }
        
        public async Task<Video> GetVideo(Video vid)
        {
            var video = await _vkApi.Video.GetAsync(new VideoGetParams(){Videos = new List<Video>() {vid}});

            return video.FirstOrDefault();
        }
    }
}
