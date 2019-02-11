using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotTest.VK
{
    /// <summary>
    /// Модель данных запроса постов с стены вк
    /// </summary>
    class GetPostsModel
    {
        //Ссылка на метод в который отправляются данные после загрузки
        public delegate void TextProcessor(DotTest.VK.Models.FLoadTProcessingModel processingModel);

        public WallTypeForRequest userType;
        public long userId;
        public int countPost;
        public TextProcessor textProcessor;

        public GetPostsModel(WallTypeForRequest _userType, long _userId, int _countPost, TextProcessor x) 
        {
            userType = _userType;
            userId = _userId;
            countPost = _countPost;
            textProcessor = x;
        }
    }
}
