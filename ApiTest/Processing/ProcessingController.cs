using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotTest.Processing.Models;

namespace DotTest.Processing
{
    class ProcessingController
    {

        //Очередь запрсов
        List<ProcessingRequestModel> processingQueue = new List<ProcessingRequestModel>();

        //Список результатов рассчетов
        List<ProcessingResultModel> resultsProcessing = new List<ProcessingResultModel>();

        //Поток обработки
        Task processingTask;
        bool processingTaskWork = false;

        public ProcessingController()
        {
            processingTask = new Task(TextFrequencyProcessor);
        }

        /// <summary>
        /// Добавляет запрос в очередь на расчет частотности букв в тексте
        /// </summary>
        public void getFrequency(ProcessingRequestModel request)
        {
            //Добавление в очередь загрузок
            processingQueue.Add(request);
            //Запуск потока рассчета если он был завршен
            if (!processingTaskWork)
                processingTask.Start();
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
            while (processingQueue.Count > 0)
            {
                //Обрабатыват первый запрос в очереди
                currentCalculaion = processingQueue.First();

                //Очистить тексты от символов переноса строки

                //Посчиать частотности букв

                //Сохранение результата расчета

                //Удаление из очереди рассчитанного текста
                processingQueue.Remove(processingQueue.First());


            }
            processingTaskWork = false;
        }

    }
}
