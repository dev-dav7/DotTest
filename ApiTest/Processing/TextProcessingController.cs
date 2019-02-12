using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotTest.Processing.Models;

namespace DotTest.Processing
{
    class TextProcessingController
    {

        //Очередь запрсов
        List<ProcessingRequestModel> queueToProcessing = new List<ProcessingRequestModel>();

        //Список результатов рассчетов
        List<ProcessingResultModel> resultsProcessing = new List<ProcessingResultModel>();

        public List<ProcessingResultModel> ResultAll
        {
            get
            {
                return resultsProcessing;
            }
        }

        //Поток обработки
        bool processingTaskWork = false;

        public TextProcessingController()
        {
        }

        /// <summary>
        /// Добавляет запрос в очередь на расчет частотности букв в тексте
        /// </summary>
        public void getFrequency(ProcessingRequestModel request)
        {
            //Добавление в очередь загрузок
            queueToProcessing.Add(request);
            //Запуск потока рассчета если он был завршен
            if (!processingTaskWork)
                new Task(TextFrequencyProcessor).Start();
        }

        /// <summary>
        /// Метод считает частотность букв для текста
        /// Исходные данные берет из очереди запросов
        /// Работает в отдельном потоке
        /// </summary>
        void TextFrequencyProcessor()
        {
            processingTaskWork = true;

            ProcessingRequestModel currentCalculaion;
            while (queueToProcessing.Count > 0)
            {
                //Обрабатыват первый запрос в очереди
                currentCalculaion = queueToProcessing.First();

                //Очистить тексты от символов переноса строки
                List<string> clearText = new List<string>();
                foreach (var text in currentCalculaion.texts)
                    clearText.Add(ClearString(text));
                //Посчитать частотности букв
                var result = FrequencyCalculator.CalcFrequency(clearText);
                //Сохранение результата расчета
                ProcessingResultModel resultProcessing = new ProcessingResultModel(currentCalculaion, result.Item1, result.Item2);
                resultsProcessing.Add(resultProcessing);
                //Отправка результата в пункт назначения
                currentCalculaion.resultDestintaion(resultProcessing);
                //Удаление из очереди рассчитанного текста
                queueToProcessing.Remove(queueToProcessing.First());
            }
            processingTaskWork = false;
        }

        /// <summary>
        /// Очищает текст от символов переноса строки
        /// </summary>
        /// <returns></returns>
        string ClearString(string str)
        {
            if (str != null)
            {
                //Здесь также могут филтроваться другие конструкции
                string clearString = str.Replace("\n", "");
                clearString.ToLower();
                return clearString;
            }
            return "";
        }

    }
}
