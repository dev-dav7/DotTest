using System;
using System.Collections.Generic;
using VkNet.Model;

namespace DotTest.VK.Models
{
    /// <summary>
    /// Модель загруженных данных , для отправки в обработку
    /// </summary>
    class DataToProcessingModel
    {
        public List<string> postsText = new List<string>();
        public RequestPostsModel requestBody;
        public DateTime utcDateLoad;

        public DataToProcessingModel(WallGetObject wallObject , RequestPostsModel baseRequest, DateTime time)
        {
            requestBody = baseRequest;
            utcDateLoad = time;
            foreach (var post in wallObject.WallPosts)
                postsText.Add(post.Text);            
        }
    }
}
