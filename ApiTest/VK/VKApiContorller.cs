using DotTest.Processing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace DotTest.VK
{
    class VKApiContorller
    {

        VkApi api = new VkApi();//Объект api для работы

        List<RequestPostsModel> queueDownloadRequest = new List<RequestPostsModel>();//Очередь на загрузку
        List<DonwloadLogModel> logDownloadingRequest = new List<DonwloadLogModel>();//Лог результатов загрузок

        List<WallPostParams> queueOutgoingPosts = new List<WallPostParams>();//Очередь постов на отправку на стену
        List<SendingPostLogModel> logSendingResult = new List<SendingPostLogModel>();//Лог результатов отправки постов на стену

        public List<DonwloadLogModel> DownloadLog
        {
            get
            {
                return logDownloadingRequest;
            }
        }

        public List<SendingPostLogModel> SendLog
        {
            get
            {
                return logSendingResult;
            }
        }

        bool loadTaskWork = false;//Флаг потока загрузки
        bool postTaskWork = false;//Флаг потока отправки постов

        /// <summary>
        /// Возвращает true,id если пользователь авторизирован
        /// Возвращает false,0 если пользователь не авторизирован
        /// </summary>
        public Tuple<bool, long> userID
        {
            get
            {
                if (api.UserId != null)
                    if (api.UserId != 0)
                        return Tuple.Create(true, (long)api.UserId);
                return Tuple.Create(false, (long)0);
            }
        }

        public VKApiContorller()
        {
            //Не учтено: обновление авторизации ВК, ВК раз в сутки вроде бы сбрасывает авторизацию
        }

        #region Авторизация
        /// <summary>
        /// Авторизирует по сервис токену
        /// </summary>
        /// <param name="ServiceToken"></param>
        /// <returns>Прошла ли авторизация</returns>
        public bool Authorize(string ServiceToken)
        {

            Console.WriteLine("Authorization attempt by service token");
            try
            {
                api.Authorize(new ApiAuthParams()
                {
                    AccessToken = ServiceToken
                });
                //Проверка доступности сервисов
                var x = api.Utils.ResolveScreenName("1");
                Console.WriteLine("Authorization success");
                Console.WriteLine();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Authorization failed");
                Console.WriteLine();
                return false;
            }

        }

        /// <summary>
        /// Авторизация пользователя в приложении
        /// Дает доступ 
        /// </summary>
        /// <param name="vkIdApp">ID приложения</param>
        /// <param name="login">E-mail или номер телефона</param>
        /// <param name="pass">Пароль</param>
        /// <param name="scope">Настрйоки доступа приложения (wall)</param>
        /// <returns></returns>
        public bool Authorize(long vkIdApp, string login, string pass, Settings scope)
        {
            //Авторизация пользователя
            Console.WriteLine("Start user authorization in app");
            try
            {
                api.Authorize(new ApiAuthParams
                {
                    ApplicationId = (ulong)vkIdApp,
                    Login = login,
                    Password = pass,
                    Settings = scope
                });
                Console.WriteLine("User authorization success complite");
                Console.WriteLine();
                return api.IsAuthorized;
            }
            catch (Exception e)
            {
                Console.WriteLine("Autorize user error: {0}", e.Message);
                Console.WriteLine();
                return api.IsAuthorized;
            }
        }

        #endregion


        /// <summary>
        /// Определяет User/Public ID зашифрованные в строке
        /// Не проверяет наличие или доступность аккаунта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Tuple<UserTypeForRequest, long> DefineId(string strId)
        {

            if (strId == null)
                return Tuple.Create(UserTypeForRequest.Vague, long.MinValue);

            strId.ToLower();

            //Проверка что строка является id который используется в vkApi
            try
            {
                var id = Convert.ToInt64(strId);
                if (id > 0)
                    return Tuple.Create(UserTypeForRequest.User, Math.Abs(id));
                if (id < 0)
                    return Tuple.Create(UserTypeForRequest.Public, Math.Abs(id));
                if (id == 0)
                    return Tuple.Create(UserTypeForRequest.Vague, long.MinValue);
            }
            catch
            { }

            //Поиск паттернов id/public/club/event + id
            try
            {
                //Проверка, что строка соотвествует типу id******
                if (checkNamePattern(strId,"id").Item1)
                    return Tuple.Create(UserTypeForRequest.User, checkNamePattern(strId,"id").Item2);
                //Проверка, что строка соотвествует типу public******
                if (checkNamePattern(strId, "public").Item1)
                    return Tuple.Create(UserTypeForRequest.Public, checkNamePattern(strId, "public").Item2);
                //Проверка, что строка соотвествует типу club******
                if (checkNamePattern(strId, "club").Item1)
                    return Tuple.Create(UserTypeForRequest.Public, checkNamePattern(strId, "club").Item2);
                //Проверка, что строка соотвествует типу event******
                if (checkNamePattern(strId, "event").Item1)
                    return Tuple.Create(UserTypeForRequest.Public, checkNamePattern(strId, "event").Item2);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Tuple.Create(UserTypeForRequest.Undefined, long.MaxValue);
            }

            //Проврека есть ли такое короткое имя
            try
            {
                var x = api.Utils.ResolveScreenName(strId);
                if (x != null)
                {
                    if (x.Type == VkNet.Enums.VkObjectType.User)
                        return Tuple.Create(UserTypeForRequest.User, Convert.ToInt64(x.Id));
                    if (x.Type == VkNet.Enums.VkObjectType.Group)
                        return Tuple.Create(UserTypeForRequest.Public, Convert.ToInt64(x.Id));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Не был определен ни один из типов Id
            return Tuple.Create(UserTypeForRequest.Undefined, long.MaxValue);
        }

        /// <summary>
        /// Проверяет является ли данная строка паттерном <nameType><id>
        /// </summary>
        /// <returns></returns>
        Tuple<bool,long> checkNamePattern(string str, string nameType)
        {
            string[] words = str.Split(new char[] { nameType[nameType.Length-1] });
            if (words.Count() == 2)
                if (words[0] == nameType.Substring(0,nameType.Length-1))
                    if (Utils.IsNumericString(words[1]).Item1)
                        return Tuple.Create(true,Convert.ToInt64(words[1]));
            var x = nameType.Substring(0, nameType.Length - 2);
            return Tuple.Create(false, (long)0);
        }


        #region Загрузка постов из ВК
        /// <summary>
        /// Добавляет запрос в очередь загрузок
        /// Запускает поток загрузки если он завершил работу ранее
        /// </summary>
        /// <param name="requestPost"></param>
        public void GetPosts(RequestPostsModel requestPost)
        {
            //Добавление в очередь загрузок
            if (requestPost.userType == UserTypeForRequest.Public || requestPost.userType == UserTypeForRequest.User)
                queueDownloadRequest.Add(requestPost);
            else
                Console.WriteLine("Does not support this userType request, support only: {0}, {1}", UserTypeForRequest.User.ToString(), UserTypeForRequest.Public.ToString());
            //Если поток загрузок был завершен ранее, запускает его
            if (!loadTaskWork)
                new Task(PostLoader).Start();
        }

        /// <summary>
        /// Метод для загрузки постов из VK
        /// Работает в отдельном потоке
        /// </summary>
        void PostLoader()
        {
            loadTaskWork = true;
            RequestPostsModel currentLoad;
            while (queueDownloadRequest.Count > 0)
            {
                //Обрабатывает первый запрос в очереди
                currentLoad = queueDownloadRequest.First();
                //Определение "внутреннего" id (>0 user, <0 public)
                long ownerId = currentLoad.userId;
                if (currentLoad.userType == UserTypeForRequest.Public) ownerId *= -1;

                //Загрузка постов
                try
                {
                    var wallPosts = api.Wall.Get(new WallGetParams { OwnerId = ownerId, Count = Convert.ToUInt64(currentLoad.countPost) });
                    //Запрос имени пользователя
                    string userName = "undefined";

                    if (currentLoad.userType == UserTypeForRequest.User)
                    {
                        var userInfo = api.Users.Get(new List<long> { ownerId });
                        if (userInfo != null)
                            userName = userInfo.First().FirstName + " " + userInfo.First().LastName;

                    }
                    if (currentLoad.userType == UserTypeForRequest.Public)
                    {
                        var publicInfo = api.Groups.GetById(null, currentLoad.userId.ToString(), null);
                        if (publicInfo != null)
                            userName = publicInfo.First().Name;
                    }

                    Console.WriteLine("Posts loading success: {0} id:{1}, count of loaded posts:{2}", currentLoad.userType, currentLoad.userId, wallPosts.WallPosts.Count());
                    //Отправка постов в обработку
                    currentLoad.textProcessor(new Models.DataToProcessingModel(wallPosts, currentLoad, userName, DateTime.UtcNow));
                    //Запись в лог результата загрузки
                    AddToLogRequest(new DonwloadLogModel(true, currentLoad, wallPosts.WallPosts.Count(), null, DateTime.UtcNow));
                    //Удаление отработатнной записи
                    queueDownloadRequest.Remove(queueDownloadRequest.First());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Posts loading failed: {0} id:{1}, error: {2}", currentLoad.userType, currentLoad.userId, e.Message);
                    //Запись в лог результата загрузки
                    AddToLogRequest(new DonwloadLogModel(false, currentLoad, 0, e, DateTime.UtcNow));
                    //Удаление отработатнной записи
                    queueDownloadRequest.Remove(queueDownloadRequest.First());
                }
            }
            loadTaskWork = false;
        }

        /// <summary>
        /// Добавление записи в лог загрузок
        /// </summary>
        /// <param name="logRecord"></param>
        void AddToLogRequest(DonwloadLogModel logRecord)
        {
            logDownloadingRequest.Add(logRecord);
        }
        #endregion

        #region Отправка постов в ВК
        /// <summary>
        /// Добавляет пост в очередь размещние на стенку
        /// Запускает поток отправкиесли он завершил работу ранее
        /// </summary>
        /// <param name="requestPost"></param>
        public void SendPostToWall(WallPostParams sendPost)
        {
            //Добавление в очередь загрузок
            queueOutgoingPosts.Add(sendPost);
            //Если поток загрузок был завершен, запускает его
            if (!postTaskWork)
                new Task(PostSender).Start();
        }

        /// <summary>
        /// Метод для публикования постов в VK
        /// Работает в отдельном потоке
        /// </summary>
        void PostSender()
        {
            postTaskWork = true;
            WallPostParams currentPost;
            while (queueOutgoingPosts.Count > 0)
            {
                currentPost = queueOutgoingPosts.First();
                //Отправка поста
                try
                {
                    var post = api.Wall.Post(currentPost);
                    //Запись в лог
                    AddToLogPost(new SendingPostLogModel(currentPost, DateTime.UtcNow, true, null));
                    Console.WriteLine("Posts send success: post to wall with ownerId:{0}", currentPost.OwnerId);
                    //Удаление отработанной записи
                    queueOutgoingPosts.Remove(queueOutgoingPosts.First());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Post send failed: ownerId: {0}, error: {1}", currentPost.OwnerId, e.Message);
                    //Запись в лог
                    AddToLogPost(new SendingPostLogModel(currentPost, DateTime.UtcNow, false, e));
                    //Удаление отработанной записи
                    queueOutgoingPosts.Remove(queueOutgoingPosts.First());
                }
            }
            postTaskWork = false;
        }

        /// <summary>
        /// Добавление записи в лог отпарвки
        /// </summary>
        /// <param name="logRecord"></param>
        void AddToLogPost(SendingPostLogModel logRecord)
        {
            logSendingResult.Add(logRecord);
        }
        #endregion

    }
}
