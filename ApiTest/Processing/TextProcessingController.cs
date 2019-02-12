﻿using System;
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

                //Посчитать частотности букв

                //Сохранение результата расчета

                //currentCalculaion.resultDestintaion();

                //Удаление из очереди рассчитанного текста
                queueToProcessing.Remove(queueToProcessing.First());


            }
            processingTaskWork = false;
        }

    }
}