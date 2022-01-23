using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    public class ServerProperty
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string LevelName;

        public Dictionary<string, string> StringOption { get; }
        public Dictionary<string, bool> BoolOption { get; }


        public ServerProperty()
        {
            StringOption = new Dictionary<string, string>();
            BoolOption = new Dictionary<string, bool>();

            ReadFile();
        }

        private void ReadFile()
        {
            logger.Info("set_value method");

            // GUIでの変更を保存するための仕掛け
            // 何も考えずにtmp_properties = Data_list.Server_Propertiesとすると「参照渡し」になってしまい、tmp_propertiesの変更がData_list.Server_Propertiesにも反映されてしまう。
            // 「参照渡し」ではなく通常の代入のようなものは「値渡し」という
            tmp_properites = new SortedDictionary<string, string>(Data_list.Server_Properties);

            string[] difficulty_list = new string[] { "easy", "normal", "hard", "peaceful" };
            string[] gamemode_list = new string[] { "adventure", "creative", "spectator", "survival" };
            string[] true_false_list = new string[] { "true", "false" };
            //other_settingsのリストを作る
            List<string> other_settings_TF_1 = new List<string>();
            List<string> other_settings_last_1 = new List<string>();

            foreach (string key in Data_list.Server_Properties.Keys)
            {
                if (key == "level-name" || key == "difficulty" || key == "gamemode" || key == "hardcore" || key == "force-gamemode" || key == "white-list" || key == "enforce-whitelist" || key.Contains("#"))
                {
                    continue;
                }
                if (Data_list.Server_Properties[key] == "true" || Data_list.Server_Properties[key] == "false")
                {
                    other_settings_TF_1.Add(key);
                }
                else
                {
                    other_settings_last_1.Add(key);
                }
            }
            //型を合わせるための処理
            string[] other_settings_TF = new string[other_settings_TF_1.Count];
            string[] other_settings_last = new string[other_settings_last_1.Count];
            for (int i = 0; i < other_settings_TF_1.Count; i++)
            {
                other_settings_TF[i] = other_settings_TF_1[i];
            }
            for (int i = 0; i < other_settings_last_1.Count; i++)
            {
                other_settings_last[i] = other_settings_last_1[i];
            }

            //MAIN Settings
            Register_combo(difficulty, difficulty_list, Data_list.Server_Properties["difficulty"]);
            Register_combo(hardcore, true_false_list, Data_list.Server_Properties["hardcore"]);
            Register_combo(gamemode, gamemode_list, Data_list.Server_Properties["gamemode"]);
            Register_combo(force_gamemode, true_false_list, Data_list.Server_Properties["force-gamemode"]);
            Register_combo(white_list, true_false_list, Data_list.Server_Properties["white-list"]);
            Register_combo(enforce_white_list, true_false_list, Data_list.Server_Properties["enforce-whitelist"]);

            //OTHER Settings
            Register_combo(true_false, other_settings_TF, other_settings_TF[0]);
            Register_combo(true_false_combo, true_false_list, Data_list.Server_Properties[other_settings_TF[0]]);
            Register_combo(input_text, other_settings_last, other_settings_last[0]);
            input_text_txt.Text = Data_list.Server_Properties[other_settings_last[0]];
        }

        public void SetProperty(string indexName, string strContent)
        {
            StringOption[indexName] = strContent;
        }

        public void SetProperty(string indexName, bool boolContent)
        {
            BoolOption[indexName] = boolContent;
        }

        public void WriteFile()
        {

        }
    }
}
