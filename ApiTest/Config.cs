
using System.IO;

namespace DotTest
{
    public static class Config
    {

        public static string configFileName = "config.txt";

        //Default value
        public static long VKAppID = 6850670;
        public static string VKAppServiceToken = "378265ec378265ec378265ec5c37eaed8233782378265ec6bde774e37850945263fda62";
        public static bool PostResult = false;
        public static bool OutToConsole = true;
        public static int PostGetCount = 5;

        public static void LoadConfig()
        {

        }

        public static void SaveConfig()
        {

        }
    }
}
