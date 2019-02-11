using System;
using DotTest.VK;
using DotTest.Processing;

namespace DotTest
{
    class Program
    {
        static Random r = new Random();

        //Создание объекта, выполняющего обработку текста
        static ProcessingController TextProcessingController = new ProcessingController();

        //Создание объекта для работы с VK API
        static VKApiContorller VKController = new VKApiContorller();

        static void Main(string[] args)
        {
            //Авторизация в API по сервис-ключу
            VKController.Authorize(Config.VKAppServiceToken);

            var s = Console.ReadLine();

            var j = VKController.DefineId(s);
            Console.WriteLine(j.Item1 + " " + j.Item2);
            VKController.GetPosts(new GetPostsModel(j.Item1, j.Item2, 5, ResultRequest));


            Console.ReadLine();
        }

        /// <summary>
        /// В этот метод приходят загруженные из вк посты
        /// </summary>
        /// <param name="processingModel"></param>
        public static void ResultRequest(DotTest.VK.Models.FLoadTProcessingModel processingModel)
        {
            //Пост был загружен из ВК и передан сюда
            //Отпралвяем пост на обработку
            TextProcessingController.getFrequency(new Processing.Models.ProcessingRequestModel(
                processingModel.requestBody.userType.ToString(),
                processingModel.requestBody.userId,
                processingModel.requestBody.countPost,
                processingModel.postsText,
                processingModel.utcDateLoad
                ));
        }

        /// <summary>
        /// В жтот метод приходят рассчитанные данные
        /// </summary>
        /// <param name="result"></param>
        public static void ResulProcessing(DotTest.Processing.Models.ProcessingResultModel result)
        {

        }


    }
}
