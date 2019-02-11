using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotTest.VK
{
    /// <summary>
    /// Модель данных записи в лог результата запроса постов со стены в вк
    /// </summary>
    class ResultRequestLogModel
    {
        public bool successComplite;
        public Exception exception;
        public GetPostsModel requestBody;
        public int countLoadedPost;
        public DateTime utcDateLoad;
        
        public ResultRequestLogModel(bool resultLoad, GetPostsModel baseRequest, int _countLoadedPost, Exception e, DateTime time)
        {
            successComplite = resultLoad;
            exception = e;
            requestBody = baseRequest;
            countLoadedPost = _countLoadedPost;
            utcDateLoad = time;
        }
          
    }
}
