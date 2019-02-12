using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model.RequestParams;

namespace DotTest.Processing.Models
{
    /// <summary>
    /// Модель данных для отправки постов на стену
    /// </summary>
    class WallPostResultModel
    {
        public WallPostParams post;//Пост который отправляли
        public DateTime date;//Время создания записи лога Utc
        public bool succesComplitel;//Успешность загрузки
        public Exception error;//Ошибка отправки если такая была

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_post"></param>
        /// <param name="_date">UTC Time</param>
        /// <param name="_sendResult"></param>
        /// <param name="_error"></param>
       public WallPostResultModel(WallPostParams _post, DateTime _date, bool _sendResult, Exception _error)
        {
            post = _post;
            date = _date;
            succesComplitel = _sendResult;
            error = _error;
        }  
    }
}
