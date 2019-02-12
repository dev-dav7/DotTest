using DotTest.Processing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace DotTest.VK
{
    class VKApiUserController
    {
        //Объект api для работы
        VkApi apiUser = new VkApi();

        public Tuple<bool, long> userID
        {
            get
            {
                if  (apiUser.UserId != null)
                    return Tuple.Create(true,(long)apiUser.UserId);

                return Tuple.Create(false,(long)0);
            }
        }

        //Очередь постов на отправку на стену
        List<WallPostParams> queueToSend = new List<WallPostParams>();

        //Лог результатов отправки постов на стену
        List<WallPostResultModel> logSendResult = new List<WallPostResultModel>();

        //Поток отправки постов
        Task postTask;
        bool postTaskWork = false;

        public VKApiUserController()
        {
            postTask = new Task(PostSender);
        }

        /// <summary>
        /// Авторизация  пользователя в приложении
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
            apiUser.LogOut();
            try
            {
                apiUser.Authorize(new ApiAuthParams
                {
                    ApplicationId = (ulong)vkIdApp,
                    Login = login,
                    Password = pass,
                    Settings = scope
                });
                Console.WriteLine("User authorization success complite");
                return apiUser.IsAuthorized;
            }
            catch (Exception e)
            {
                Console.WriteLine("Autorize user error: {0}", e.Message);
                //isAutroized = false;
                return apiUser.IsAuthorized;
            }
        }

        /// <summary>
        /// Добавляет пост в очередь размещние на стенку
        /// Запускает поток отправкиесли он завершил работу ранее
        /// </summary>
        /// <param name="requestPost"></param>
        public void SendPostToWall(WallPostParams sendPost)
        {
            //Добавление в очередь загрузок
            queueToSend.Add(sendPost);
            //Если поток загрузок был завершен, запускает его
            if (!postTaskWork)
                postTask.Start();
        }


        /// <summary>
        /// Метод для публикования постов в VK
        /// Работает в отдельном потоке
        /// </summary>
        void PostSender()
        {
            postTaskWork = true;
            WallPostParams currentPost;
            while (queueToSend.Count > 0)
            {
                currentPost = queueToSend.First();
                //Отправка поста
                try
                {
                    var post = apiUser.Wall.Post(currentPost);
                    //Запись в лог
                    AddToLogPost(new WallPostResultModel(currentPost, DateTime.UtcNow, true, null));
                    Console.WriteLine("Posts send success: post to wall with ownerId:{0}", currentPost.OwnerId);
                    //Удаление отработанной записи
                    queueToSend.Remove(queueToSend.First());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Post send failed: ownerId: {0}, error: {1}", currentPost.OwnerId, e.Message);
                    //Запись в лог
                    AddToLogPost(new WallPostResultModel(currentPost, DateTime.UtcNow, false, e));
                    //Удаление отработанной записи
                    queueToSend.Remove(queueToSend.First());
                }
            }
            postTaskWork = false;
        }

        /// <summary>
        /// Добавление записи в лог отпарвки
        /// </summary>
        /// <param name="logRecord"></param>
        void AddToLogPost(WallPostResultModel logRecord)
        {
            logSendResult.Add(logRecord);
        }
    }
}
