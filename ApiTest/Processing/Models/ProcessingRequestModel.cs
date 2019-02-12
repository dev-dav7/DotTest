using System;
using System.Collections.Generic;

namespace DotTest.Processing.Models
{
    /// <summary>
    /// Модель данных запроса на рассчет статистики 
    /// </summary>
    class ProcessingRequestModel
    {
        //Ссылка на метод в который отправляются данные после обработки
        public delegate void ResultDestination(ProcessingResultModel processingModel);

        public string type;//отображаемый тип (id/public/other)
        public long id;//отображаемый id 
        public List<string> texts;//тексты для анализа
        public DateTime date;//время когда были загруженны посты
        public int countPosts;//количество постов 
        public ResultDestination resultDestintaion;

        public ProcessingRequestModel(string _type, long _id, int _countPosts,List<string> _texts, DateTime _date, ResultDestination x)
        {
            type = _type;
            id = _id;
            countPosts = _countPosts;
            texts = _texts;
            date = _date;
            resultDestintaion = x;
        }
    }
}
