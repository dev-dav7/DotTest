using System;
using DotTest.VK;
using DotTest.Processing;
using VkNet.Enums.Filters;
using System.Collections.Generic;
using System.Globalization;

namespace DotTest
{
    class Program
    {
        //Создание объекта, выполняющего обработку текста
        static TextProcessingController TextProcessingController = new TextProcessingController();

        //Создание объекта для работы с VK API 
        static VKApiContorller VKController = new VKApiContorller();

        static void Main(string[] args)
        {
            //Авторизация в API по сервис-ключу
            VKController.Authorize(Config.VKAppServiceToken);

            //Вывод консольного меню
            MainMenu();
        }

        /// <summary>
        /// В этот метод приходят загруженные из вк посты
        /// </summary>
        /// <param name="processingModel"></param>
        public static void ResultRequest(DotTest.VK.Models.DataToProcessingModel processingModel)
        {
            //Пост был загружен из ВК и передан сюда
            //Отпралвяем пост на обработку
            TextProcessingController.getFrequency(new Processing.Models.ProcessingRequestModel(
                processingModel.requestBody.userType.ToString(),
                processingModel.requestBody.userId,
                processingModel.name,
                processingModel.requestBody.countPost,
                processingModel.postsText,
                processingModel.utcDateLoad,
                ResultProcessing
                ));
        }

        /// <summary>
        /// В этот метод приходят рассчитанные данные
        /// </summary>
        /// <param name="result"></param>
        public static void ResultProcessing(DotTest.Processing.Models.ProcessingResultModel result)
        {
            //Вывод в консоль
            if (Config.outToConsole)
            {
                Console.WriteLine();
                Console.WriteLine("User name: {0}", result.request.name);
                Console.WriteLine("User type: {0}", result.request.type);
                Console.WriteLine("User id: {0}", result.request.id);
                Console.WriteLine("Count posts: {0}", result.request.countPosts);
                Console.WriteLine("Frequency for english alphabet:");
                foreach (var x in result.en)
                    x.ConsoleView(Config.roundTo);
                Console.WriteLine();
                Console.WriteLine("Frequency for russian alphabet:");
                foreach (var x in result.ru)
                    x.ConsoleView(Config.roundTo);
                Console.WriteLine();
                Console.WriteLine();
            }

            //Отправка поста в ВК
            if (Config.outToConsole)
                if (VKController.userID.Item1)
                {
                    string message = result.request.name + ", статистика для последних " + result.request.countPosts + " постов: ";
                    string en = JSONView(result.en);
                    string ru = JSONView(result.ru);
                    message = message + en + ' ' + ru;
                    VKController.SendPostToWall(new VkNet.Model.RequestParams.WallPostParams { OwnerId = VKController.userID.Item2, Message = message });
                }
        }

        /// <summary>
        /// Приводит лист FrequencyResult к виду JSON-строки
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        static string JSONView(List<Processing.Models.FrequencyResult> records)
        {
            string result = "{";
            foreach (var rec in records)
                result += rec.JSONView(Config.roundTo, CultureInfo.GetCultureInfo("en-US")) + ", ";
            result = result.Substring(0, result.Length - 2) + '}';
            return result;
        }

        #region Консольное меню
        static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("*Main menu*");
                Console.WriteLine("Select section:");
                Console.WriteLine("1 - Get post frequency by wall");
                if (VKController.userID.Item1)
                    Console.WriteLine("2 - User authorize: id{0}", VKController.userID.Item2.ToString());
                else
                    Console.WriteLine("2 - User authorize: none authorize");
                Console.WriteLine("3 - Set parameters");
                Console.WriteLine("4 - Log and result");
                Console.WriteLine("5 - Exit");
                Console.WriteLine("Enter code section:");

                string str = Console.ReadLine();
                Console.Clear();
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
                    if (str == "5")
                    {
                        Console.WriteLine("Close application");
                        System.Threading.Thread.Sleep(1000);
                        Environment.Exit(0);
                    }
                    if (str == "1")
                    {
                        GetPostsMenu();
                    }
                    if (str == "2")
                    {
                        UserAuthorization(VKController);
                    }
                    if (str == "3")
                    {
                        ParametrsMenu();
                    }
                    if (str == "4")
                    {
                        LogMenu();
                    }
                }
            }
        }

        /// <summary>
        /// Меню для запроса постов со стены вк
        /// </summary>
        static void GetPostsMenu()
        {
            Console.WriteLine("*Request frequency char from wallpost*");
            Console.WriteLine("Enter id or short name and press enter");
            Console.WriteLine("To exit enter '.' ");
            string newId;
            while (true)
            {
                newId = Console.ReadLine();
                Console.Clear();
                if (newId == null || newId == "")
                    Console.WriteLine("Please, enter not null request");
                else
                if (newId == ".")
                    return;
                else
                if (!Utils.IsNormalString(newId).Item1)
                    ConsoleInputError(Utils.IsNormalString(newId), newId);
                else
                {
                    //Определение ID введенной записи
                    var idInfo = VKController.DefineId(newId);
                    //Такого ID не сущесвтует
                    if (idInfo.Item1 == UserTypeForRequest.Undefined)
                        Console.WriteLine("Such '{0}' id does not exist", newId);
                    //ID 
                    if (idInfo.Item1 == UserTypeForRequest.Vague)
                        Console.WriteLine("ID '{0}' is not defined. (ID '0' not use.)", newId);
                    if (idInfo.Item1 == UserTypeForRequest.Public || idInfo.Item1 == UserTypeForRequest.User)
                    {
                        Console.WriteLine("User type: {0}, user id: {1}, posts requested: {2}", idInfo.Item1, idInfo.Item2, Config.postGetCount);
                        //Зарпос в вк
                        VKController.GetPosts(new RequestPostsModel(idInfo.Item1, idInfo.Item2, Config.postGetCount, ResultRequest));
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Авторизация пользователя в приложении
        /// </summary>
        /// <param name="userApi"></param>
        static void UserAuthorization(VKApiContorller userApi)
        {
            Console.WriteLine("*User Authorization*");
            Console.WriteLine("Enter '.' instead of login to cancel");
            Console.WriteLine("Enter your login or email from VK:");
            string login = Console.ReadLine();
            if (login == ".")
            {
                Console.WriteLine("Authorization was aborted");
                return;
            }
            Console.WriteLine("Enter your password from VK:");
            string password = Utils.ReadPassword();
            //Доступ к стене
            Settings scope = Settings.Wall;
            //Авторизация
            userApi.Authorize(Config.VKAppID, login, password, scope);

        }

        /// <summary>
        /// Консольное меню для вывода и изменения параметров
        /// </summary>
        static void ParametrsMenu()
        {
            while (true)
            {
                Console.WriteLine("*Current parametrs*");
                Console.WriteLine("Current AppID: {0}", Config.VKAppID);
                Console.WriteLine("Current ServiceKey: {0}", Config.VKAppServiceToken);
                Console.WriteLine();
                Console.WriteLine("1 - Requested number of posts: {0}", Config.postGetCount);
                Console.WriteLine("2 - Place frequency to wall: {0}", Config.outToWall);
                Console.WriteLine("3 - Print frequency to console: {0}", Config.outToConsole);
                Console.WriteLine();
                Console.WriteLine("To set parametr 1, 2, 3 enter to console: <number param><.><new value>");
                Console.WriteLine("Parametr type: 1 - int: min 1, max 100; 2, 3  boolen: true or false");
                Console.WriteLine("Enter '4' to go to main menu");
                Console.WriteLine("Enter request:");
                string str = Console.ReadLine();
                Console.Clear();
                var cr = Utils.IsNormalString(str);
                if (cr.Item1 == false)
                    ConsoleInputError(cr, str);
                else
                if (str == "4")
                    MainMenu();
                else
                if (str.Length < 3)
                {
                    Console.WriteLine("Incorrect entered");
                    Console.WriteLine();
                }
                else
                {
                    string[] words = str.Split(new char[] { '.' });
                    if (words[0].Length > 1)
                    {
                        Console.WriteLine("Incorrect section entered");
                        Console.WriteLine();
                    }
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
                                Config.postGetCount = newValue;
                            }
                        }
                        if (words[0] == "2")
                        {
                            if (words[1] == "true")
                                Config.outToWall = true;
                            else
                                if (words[1] == "false")
                                Config.outToWall = false;
                            else
                            {
                                Console.WriteLine("Incorrect value");
                                Console.WriteLine();
                            }
                        }
                        if (words[0] == "3")
                        {
                            if (words[1] == "true")
                                Config.outToConsole = true;
                            else
                               if (words[1] == "false")
                                Config.outToConsole = false;
                            else
                            {
                                Console.WriteLine("Incorrect value");
                                Console.WriteLine();
                            }
                        }
                        //Сохранить изменения
                        Config.SaveConfig();
                    }
                }
            }
        }

        /// <summary>
        /// Консольное меню для вывода логов
        /// </summary>
        static void LogMenu()
        {
            string keyLog = "";
            while (true)
            {
                Console.WriteLine("Select a list of logs to display");
                Console.WriteLine("1 - result download request log");
                Console.WriteLine("2 - result sending posts log");
                Console.WriteLine("3 - result calculation");
                Console.WriteLine("4 - to main menu");
                Console.WriteLine("Enter log section number:");
                keyLog = Console.ReadLine();
                Console.Clear();
                if (keyLog == "1")
                {
                    var donwloadLog = VKController.DownloadLog;
                    Console.WriteLine("Download log:");
                    foreach (var log in donwloadLog)
                        log.ConsoleView();
                    Console.WriteLine();
                }
                else
                if (keyLog == "2")
                {
                    var sendLog = VKController.SendLog;
                    Console.WriteLine("Sending log:");
                    foreach (var log in sendLog)
                        log.ConsoleView();
                    Console.WriteLine();

                }
                else
                if (keyLog == "3")
                {
                    Console.WriteLine("Result calcultaions:");

                    var results = TextProcessingController.ResultAll;
                    foreach (var result in results)
                    {
                        Console.WriteLine();
                        Console.WriteLine("User type: {0}", result.request.type);
                        Console.WriteLine("User id: {0}", result.request.id);
                        Console.WriteLine("User name: {0}", result.request.name);
                        Console.WriteLine("Frequency for english alphabet:");
                        foreach (var x in result.en)
                            x.ConsoleView(Config.roundTo);
                        Console.WriteLine();
                        Console.WriteLine("Frequency for russian alphabet:");
                        foreach (var x in result.ru)
                            x.ConsoleView(Config.roundTo);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                else
                if (keyLog == "4")
                {
                    MainMenu();
                }
                else
                {
                    Console.WriteLine("Incorrect section entered");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Вывод информации об обнаружении некорректнго символа в строке
        /// </summary>
        /// <param name="t"></param>
        /// <param name="s"></param>
        static void ConsoleInputError(Tuple<bool, int> t, string s)
        {
            if (s != null)
                Console.WriteLine("Invalid character entered. First invalid character: '{0}'", s[t.Item2]);
            else
                Console.WriteLine("Invalid character entered. Null string.");
            Console.WriteLine();
        }
        #endregion
    }
}
