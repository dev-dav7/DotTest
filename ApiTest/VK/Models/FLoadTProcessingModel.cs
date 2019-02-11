using System;
using System.Collections.Generic;
using VkNet.Model;

namespace DotTest.VK.Models
{
    /// <summary>
    /// Модель данных результата загрузки
    /// </summary>
    class FLoadTProcessingModel
    {
        public List<string> postsText = new List<string>();
        public GetPostsModel requestBody;
        public DateTime utcDateLoad;

        public FLoadTProcessingModel(WallGetObject wallObject , GetPostsModel baseRequest, DateTime time)
        {
            requestBody = baseRequest;
            utcDateLoad = time;
            foreach (var post in wallObject.WallPosts)
                postsText.Add(post.Text);            
        }
    }
}
