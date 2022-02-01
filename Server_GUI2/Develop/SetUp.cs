using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    class SetUp
    {
        public static string StarterVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public static string CurrentDirectory { get { return Environment.GetEnvironmentVariable("SERVER_STERTER_TEST"); } }
        public static string DataPath { get { return Path.Combine(CurrentDirectory, "World_Data"); } }

        public static UserSettings userSet = new UserSettings();


        public void ChangeSpecification()
        {
            // 仕様変更が必要な場合に使う
        }

        public void ShowProgressBar()
        {

        }
    }
}
