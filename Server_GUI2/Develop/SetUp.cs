using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    class SetUp
    {
        public static string StarterVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public static string DataPath { get { return @".\World_Data"; } }

        public static VersionFactory verFactory = new VersionFactory();
        public static UserSettings userSet = new UserSettings();


        public SetUp()
        {
            verFactory.LoadAllVersions();
            userSet.ReadFile();
        }

        public void ChangeSpecification()
        {
            // 仕様変更が必要な場合に使う
        }

        public void ShowProgressBar()
        {

        }

        /// <summary>
        /// 外部ファイルを読み込む
        /// 
        /// ※　全て事前に読み込んでおくことで、ViewModel内やその後の処理で、外部ファイルの情報を使いやすくする
        /// ※　すなわち、ここで読み込まなくても良い
        /// </summary>
        private void ReadOuterContents()
        {

        }
    }
}
