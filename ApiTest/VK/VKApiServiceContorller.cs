using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace DotTest.VK
{
    class VKApiServiceContorller
    {
        //Объект api для работы
        VkApi apiService = new VkApi();

        //Очередь на загрузку
        List<GetPostsModel> requestQueue = new List<GetPostsModel>();

        //Лог попыток загрузок
        List<ResultRequestLogModel> requestLog = new List<ResultRequestLogModel>();

        //Флаг потока загрузки
        bool loadTaskWork = false;

        public VKApiServiceContorller()
        {
        }

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
                apiService.Authorize(new ApiAuthParams()
                {
                    AccessToken = ServiceToken
                });
                //Проверка доступности сервисов
                var x = apiService.Utils.ResolveScreenName("1");
                Console.WriteLine("Authorization success");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Authorization failed");
                return false;
            }

        }

        /// <summary>
        /// Определяет User/Public ID зашифрованные в строке
        /// Не проверяет наличие или доступность аккаунта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Tuple<WallTypeForRequest, long> DefineId(string strId)
        {
            strId.ToLower();

            //Проверка что строка является id который используется в vkApi
            try
            {
                var id = Convert.ToInt64(strId);
                if (id > 0)
                    return Tuple.Create(WallTypeForRequest.User, Math.Abs(id));
                if (id < 0)
                    return Tuple.Create(WallTypeForRequest.Public, Math.Abs(id));
                if (id == 0)
                    return Tuple.Create(WallTypeForRequest.Vague, long.MinValue);
            }
            catch
            { }

            //Поиск id/public
            string[] words;

            try
            {
                //Проверка, что строка соотвествует типу id******
                words = strId.Split(new char[] { 'd' });
                if (words.Count() == 2)
                {
                    if (words[0][0] == 'i' && words[0].Count() == 1)
                        if (Utils.IsNumericString(words[1]).Item1)
                            return Tuple.Create(WallTypeForRequest.User, Convert.ToInt64(words[1]));
                }

                //Проверка, что строка соотвествует типу public******
                words = strId.Split(new char[] { 'c' });
                if (words.Count() == 2)
                {
                    if (words[0].Contains("publi") && words[0].Count() == 5)
                        if (Utils.IsNumericString(words[1]).Item1)
                            return Tuple.Create(WallTypeForRequest.Public, Convert.ToInt64(words[1]));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Tuple.Create(WallTypeForRequest.Undefined, long.MaxValue);
            }

            //Проврека есть ли такое короткое имя
            try
            {
                var x = apiService.Utils.ResolveScreenName(strId);
                if (x != null)
                {
                    if (x.Type == VkNet.Enums.VkObjectType.User)
                        return Tuple.Create(WallTypeForRequest.User, Convert.ToInt64(x.Id));
                    if (x.Type == VkNet.Enums.VkObjectType.Group)
                        return Tuple.Create(WallTypeForRequest.Public, Convert.ToInt64(x.Id));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Не был определен ни один из типов Id
            return Tuple.Create(WallTypeForRequest.Undefined, long.MaxValue);
        }

        /// <summary>
        /// Добавляет запрос в очередь загрузок
        /// Запускает поток загрузки если он завершил работу ранее
        /// </summary>
        /// <param name="requestPost"></param>
        public void GetPosts(GetPostsModel requestPost)
        {
            //Добавление в очередь загрузок
            if (requestPost.userType == WallTypeForRequest.Public || requestPost.userType == WallTypeForRequest.User)
                requestQueue.Add(requestPost);
            else
                Console.WriteLine("Does not support this userType request, support only: {0}, {1}", WallTypeForRequest.User.ToString(), WallTypeForRequest.Public.ToString());
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
            GetPostsModel currentLoad;
            while (requestQueue.Count > 0)
            {
                //Обрабатывает первый запрос в очереди
                currentLoad = requestQueue.First();
                //Определение "внутреннего" id (>0 user, <0 public)
                long ownerId = currentLoad.userId;
                if (currentLoad.userType == WallTypeForRequest.Public) ownerId *= -1;

                //Загрузка постов
                try
                {
                    var wallPosts = apiService.Wall.Get(new WallGetParams { OwnerId = ownerId, Count = Convert.ToUInt64(currentLoad.countPost) });
                    Console.WriteLine("Posts loading success: {0} id:{1}, count of loaded posts:{2}", currentLoad.userType, currentLoad.userId, wallPosts.WallPosts.Count());
                    //Отправка постов в обработку
                    currentLoad.textProcessor(new Models.FLoadTProcessingModel(wallPosts, currentLoad, DateTime.UtcNow));
                    //Запись в лог результата загрузки
                    AddToLogRequest(new ResultRequestLogModel(true, currentLoad, wallPosts.WallPosts.Count(), null, DateTime.UtcNow));
                    //Удаление отработатнной записи
                    requestQueue.Remove(requestQueue.First());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Posts loading failed: {0} id:{1}, error: {2}", currentLoad.userType, currentLoad.userId, e.Message);
                    //Запись в лог результата загрузки
                    AddToLogRequest(new ResultRequestLogModel(false, currentLoad, 0, e, DateTime.UtcNow));
                    //Удаление отработатнной записи
                    requestQueue.Remove(requestQueue.First());
                }
            }
            loadTaskWork = false;
        }

        /// <summary>
        /// Добавление записи в лог загрузок
        /// </summary>
        /// <param name="logRecord"></param>
        void AddToLogRequest(ResultRequestLogModel logRecord)
        {
            requestLog.Add(logRecord);
        }
    }
}
