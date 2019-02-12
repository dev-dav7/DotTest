
using System.IO;

namespace DotTest
{
    public static class Config
    {

        public static string configFileName = "config.txt";

        //Default value
        public static long VKAppID = 6850670;
        public static string VKAppServiceToken = "378265ec378265ec378265ec5c37eaed8233782378265ec6bde774e37850945263fda62";
        public static bool PostResult = false;//Печатать ли результат на стену
        public static bool OutToConsole = true;//Выводить ли в консоль
        public static int PostGetCount = 5;//Количество запрашиваемых постов
        public static int roundTo = 6;//Округление при выводе

        public static void LoadConfig()
        {
        }

        public static void SaveConfig()
        {
        }
    }
}
