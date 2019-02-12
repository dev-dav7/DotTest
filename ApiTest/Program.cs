using System;
using DotTest.VK;
using DotTest.Processing;
using VkNet.Enums.Filters;

namespace DotTest
{
    class Program
    {
        static Random r = new Random();

        //Создание объекта, выполняющего обработку текста
        static ProcessingController TextProcessingController = new ProcessingController();

        //Создание объекта для работы с VK API со стороны приложения
        static VKApiServiceContorller VKServiceController = new VKApiServiceContorller();

        //Создание объекта для работы с VK API со стороны пользователя
        static VKApiUserController VKUserController = new VKApiUserController();

        static void Main(string[] args)
        {
            //Загрузка конфигурации
            Console.WriteLine("Load config");
            Config.LoadConfig();
            Console.WriteLine();
            //Авторизация в API по сервис-ключу
            VKServiceController.Authorize(Config.VKAppServiceToken);

            MainMenu();

            //Определение ID введенной записи
            var s = Console.ReadLine();
            var j = VKServiceController.DefineId(s);
            Console.WriteLine(j.Item1 + " " + j.Item2);
            VKServiceController.GetPosts(new GetPostsModel(j.Item1, j.Item2, Config.PostGetCount, ResultRequest));


            Console.ReadLine();
        }

        /// <summary>
        /// Авторизация пользователя в приложении
        /// </summary>
        /// <param name="userApi"></param>
        static void UserAuthorization(VKApiUserController userApi)
        {
            Console.WriteLine("Enter your login or email from VK:");
            string login = Console.ReadLine();
            Console.WriteLine("Enter your password from VK:");
            string password = Utils.ReadPassword();
            //Доступ к стене
            Settings scope = Settings.Wall;
            //Авторизация
            userApi.Authorize(Config.VKAppID, login, password, scope);

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
        /// В этот метод приходят рассчитанные данные
        /// </summary>
        /// <param name="result"></param>
        public static void ResultProcessing(DotTest.Processing.Models.ProcessingResultModel result)
        {

        }

        static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Select section:");
                Console.WriteLine("1 - Get post frequency by wall");
                Console.WriteLine("2 - User");
                Console.WriteLine("3 - Set parameters");
                Console.WriteLine("4 - Exit");
                Console.WriteLine("Enter code section:");

                string str = Console.ReadLine();
                //Тут допустимы только цифры
                var cr = Utils.IsNumericString(str);
                if (cr.Item1 == false)
                    ConsoleInputError(cr, str);
                else
                if (str.Length > 1)
                {
                    Console.WriteLine("Incorrect section entered");
                }
                else
                {
                    if (str == "4")
                    {
                        Console.WriteLine("Close application");
                        System.Threading.Thread.Sleep(1000);
                        Environment.Exit(0);
                    }
                    if (str == "1")
                    {
                        //
                    }
                    if (str == "2")
                    {
                        //
                    }
                    if (str == "3")
                    {
                        ParametrsMenu();
                    }
                }
            }
        }

        static void ParametrsMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Current parametrs:");
                Console.WriteLine("Current AppID: {0}", Config.VKAppID);
                Console.WriteLine("Current ServiceKey: {0}", Config.VKAppServiceToken);
                Console.WriteLine();
                Console.WriteLine("1 - Requested number of posts: {0}", Config.PostGetCount);
                Console.WriteLine("2 - Place frequency to wall: {0}", Config.PostResult);
                Console.WriteLine("3 - Print frequency to console: {0}", Config.OutToConsole);
                Console.WriteLine("4 - Go to main menu");
                Console.WriteLine();
                Console.WriteLine("To set param 1, 2, 3 enter to console: <number param><.><new value>.");
                Console.WriteLine("Param type: 1 - int: min 1, max 100; 2, 3  boolen: true or false.");
                string str = Console.ReadLine();
                var cr = Utils.IsNormalString(str);
                if (cr.Item1 == false)
                    ConsoleInputError(cr, str);
                else
                if (str == "4")
                    MainMenu();
                else
                if (str.Length < 3)
                    Console.WriteLine("Incorrect entered");
                else
                {
                    string[] words = str.Split(new char[] { '.' });
                    if (words[0].Length > 1)
                        Console.WriteLine("Incorrect section entered");
                    else
                    {
                        if (words[0] == "1")
                        {
                            var ar = Utils.IsNumericString(words[1]);
                            if (!ar.Item1)
                                ConsoleInputError(ar, words[1]);
                            else
                            {
                                int newValue = Convert.ToInt32(words[1]);
                                if (newValue < 1)
                                    newValue = 1;
                                if (newValue > 100)
                                    newValue = 100;
                                Config.PostGetCount = newValue;
                            }
                        }
                        if (words[0] == "2")
                        {
                            if (words[1] == "true")
                                Config.PostResult = true;
                            else
                                if (words[1] == "false")
                                Config.PostResult = false;
                            else
                                Console.WriteLine("Incorrect value");
                        }
                        if (words[0] == "3")
                        {
                            if (words[1] == "true")
                                Config.OutToConsole = true;
                            else
                               if (words[1] == "false")
                                Config.OutToConsole = false;
                            else
                                Console.WriteLine("Incorrect value");
                        }
                        Config.SaveConfig();
                    }
                }
            }
        }

        static void ConsoleInputError(Tuple<bool, int> t, string s)
        {
            if (s!=null)
            Console.WriteLine("Invalid character entered. First invalid character: {0}.", s[t.Item2]);
            else
                Console.WriteLine("Invalid character entered. Null string.");

        }
    }
}
