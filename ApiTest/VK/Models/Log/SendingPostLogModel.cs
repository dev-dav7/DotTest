using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model.RequestParams;

namespace DotTest.Processing.Models
{
    /// <summary>
    /// Модель данных для отправки поста на стену
    /// </summary>
    class SendingPostLogModel
    {
        public WallPostParams post;//Пост который отправляли
        public DateTime date;//Время создания записи лога Utc
        public bool successComplite;//Успешность загрузки
        public Exception exception;//Ошибка отправки если такая была

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_post"></param>
        /// <param name="_date">UTC Time</param>
        /// <param name="_sendResult"></param>
        /// <param name="_error"></param>
       public SendingPostLogModel(WallPostParams _post, DateTime _date, bool _sendResult, Exception _error)
        {
            post = _post;
            date = _date;
            successComplite = _sendResult;
            exception = _error;
        }  

        /// <summary>
        /// Вывод в консоль
        /// </summary>
        public void ConsoleView()
        {
            if (successComplite)
                Console.WriteLine("Post to ID{0} succes in UTC{1}", post.OwnerId, date);
            else
                Console.WriteLine("Post to ID{0} failed in UTC{1}, error:{2}", post.OwnerId, date, exception.Message);
        }
    }
}
