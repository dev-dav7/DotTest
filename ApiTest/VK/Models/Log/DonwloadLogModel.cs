using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotTest.VK
{
    /// <summary>
    /// Модель данных результата загрузки
    /// </summary>
    class DonwloadLogModel
    {
        public bool successComplite;
        public Exception exception;
        public RequestPostsModel requestBody;
        public int countLoadedPost;
        public DateTime utcDateLoad;
        
        public DonwloadLogModel(bool resultLoad, RequestPostsModel baseRequest, int _countLoadedPost, Exception e, DateTime time)
        {
            successComplite = resultLoad;
            exception = e;
            requestBody = baseRequest;
            countLoadedPost = _countLoadedPost;
            utcDateLoad = time;
        }
          
    }
}
