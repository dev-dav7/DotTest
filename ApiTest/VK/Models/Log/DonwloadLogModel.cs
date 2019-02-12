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

        /// <summary>
        /// Вывод в консоль
        /// </summary>
        public void ConsoleView()
        {
            if (successComplite)
                Console.WriteLine("Download data from {4} ID{0} succes in UTC {1}, count loaded/request posts:{2}/{3}", requestBody.userId, utcDateLoad, countLoadedPost, requestBody.countPost, requestBody.userType);
            else
                Console.WriteLine("Download data from {3} ID{0} failed in UTC {1}, error:{2}", requestBody.userId, utcDateLoad, exception.Message, requestBody.userType);
        }
    }
}
