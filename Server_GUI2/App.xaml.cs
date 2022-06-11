using log4net;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Server_GUI2
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<string, string> prop_dict = new Dictionary<string, string>();
        private string prop_dict_str = null;
        public const string end_str = "\n\nIf you want to go back writting mode, please type 'Enter' ...";

        public static string[] Args { get; set; }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // 手動でShutdown()が呼ばれるまでアプリを終了しない（MainWindowの前にWelcomeWindowを呼んだ際に、アプリが落ちてしまう現象を回避）
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            SetUnhandledDetecter();

            // システムの初期設定（処理）を行う
            SetUp.Initialize();

            // TODO: 実装の整理
            // base.OnStartup(e);
            //bool reset_data = false;
            //bool save_data = false;
            bool delete_data = false;

            if (e.Args.Length == 0)
            {
                // GUIを立ち上げる
                SetUp.InitProgressBar.AddMessage("Opening Main Window", moving:true);
                MainWindow main = new MainWindow();
                main.Show();
                main.Activate();
                return;
            }
            SetUp.InitProgressBar.Close();


            AttachConsole(-1);
            Console.WriteLine();
            Args = e.Args;
            if (e.Args[0] == "/?")
            {
                Console.Write(Server_GUI2.Properties.Resources.Guide);
                Finish();
            }
            else
            {
                bool op = false;
                foreach (string key in e.Args)
                {
                    Data_list.Argument.Add(key);

                    if (key.Substring(0,2) == "-s")
                    {
                        prop_dict_str = key.Substring(2);
                        prop_dict_str = prop_dict_str.Replace("{", "{\"");
                        prop_dict_str = prop_dict_str.Replace(",", "\",\"");
                        prop_dict_str = prop_dict_str.Replace(":", "\":\"");
                        prop_dict_str = prop_dict_str.Replace("}", "\"}");
                        continue;
                    }

                    switch (key)
                    {
                        case "-o":
                            op = true;
                            continue;
                        case "-a":
                            Server_GUI2.Properties.Settings.Default.Output_VW = true;
                            Server_GUI2.Properties.Settings.Default.Save();
                            continue;
                        case "/a":
                            Server_GUI2.Properties.Settings.Default.Output_VW = false;
                            Server_GUI2.Properties.Settings.Default.Save();
                            continue;
                        case "-r":
                            //reset_data = true;
                            continue;
                        case "/save":
                            //save_data = true;
                            continue;
                        case "/delete":
                            //delete_data = true;
                            continue;
                        default:
                            Console.WriteLine($"'{key}' is unknown paramater.");
                            Console.WriteLine(end_str);
                            Finish();
                            return;
                    }
                }

                //Data_delete(delete_data);
                
                //MainWindow main = new MainWindow();
                ////More_Settings m_settings = new More_Settings();
                ////main.Reset_world = reset_data;
                ////main.Save_world = save_data;

                //bool result = Check_valid(main);

                //if (result)
                //{
                //    //main.Get_op = op;
                //    Change_properties();
                //    //m_settings.Read_properties();
                //    //main.Start(false);
                //}

                //Console.Write(end_str);
                //Finish();
            }
        }

        private void SetUnhandledDetecter()
        {
            void ShowWindow(object exception)
            {
                var separator = new[] { Environment.NewLine };
                string error_message = exception.ToString();
                if (!(exception is ServerStarterException))
                {
                    var result = CustomMessageBox.Show(
                        $"{Server_GUI2.Properties.Resources.App_Unhandle}\n{error_message.Split(separator, StringSplitOptions.None)[0]}",
                        new string[2] { Server_GUI2.Properties.Resources.LogFolder, Server_GUI2.Properties.Resources.Close },
                        Image.Error,
                        new LinkMessage(Server_GUI2.Properties.Resources.Manage_Vup2, "https://github.com/CivilTT/ServerStarter/issues/new?assignees=&labels=bug&template=bug_report.md&title=%5BBUG%5D")
                        );

                    if (result == 0)
                        Process.Start(Path.GetFullPath(@".\log\"));
                    logger.Error(error_message);
                }
            }
            // 想定外のエラーを処理する
            DispatcherUnhandledException += (sender, eventargs) => ShowWindow(eventargs.Exception);
            TaskScheduler.UnobservedTaskException += (sender, eventargs) => ShowWindow(eventargs.Exception.InnerException);
            AppDomain.CurrentDomain.UnhandledException += (sender, eventargs) => ShowWindow(eventargs.ExceptionObject);
        }

        private void AnalizeArgs(string[] args)
        {

        }

        private bool Check_valid(MainWindow main)
        {
            if(Data_list.Argument.Count < 2 || Data_list.Argument.Count > 3)
            {
                Console.Write($"This command is invalid");
                return false;
            }

            // 入力の確認を行う場合はここで確認する
            Check_Ver(0, main);
            Check_Ver(1, main);
            Check_Wor();
            Check_Prop();
            return true;
        }

        private void Check_Ver(int index, MainWindow main)
        {
            // Copy_Versionの記載が新規ワールドを立てる際などにより省略された場合、WorldにArgument[1]を登録する。
            if (Data_list.Argument.Count == 2 && index == 1)
            {
                Data_list.Copy_version = "";
                return;
            }

            string type = "release";
            if(!Regex.IsMatch(Data_list.Argument[index], @"1\.[0-9]+"))
            {
                type = "snapshot";
            }

            if (!main.All_versions.Contains($"{type} {Data_list.Argument[index]}"))
            {
                Console.WriteLine($"The Version {Data_list.Argument[index]} is not available{end_str}");
                Finish();
            }

            switch (index)
            {
                case 0:
                    Data_list.Version = Data_list.Argument[0];
                    break;
                case 1:
                    Data_list.Copy_version = Data_list.Argument[1];
                    break;
            }
        }

        private void Check_Wor()
        {
            Data_list.World = Data_list.Argument[Data_list.Argument.Count - 1];
            
            if (!Regex.IsMatch(Data_list.World, @"^[0-9a-zA-Z-_]+$"))
            {
                Console.Write($"\n{Data_list.World} is not match World name. You can use only capital and small letters{end_str}");
                Finish();
            }
            else if(Data_list.World == "logs")
            {
                Console.Write($"You can not name the World for 'logs' or '(empty)'{end_str}");
                Finish();
            }

        }

        private void Check_Prop()
        {
            if(prop_dict_str == null)
            {
                // 設定が入っていない場合は何もしない
                return;
            }

            try
            {
                prop_dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(prop_dict_str);
            }
            catch(Exception ex)
            {
                throw new ArgumentException($"The Settings of properties is invalid\n(Error Message : {ex.Message})");
            }
        }

        private void Change_properties()
        {
            foreach(KeyValuePair<string, string> kvp in prop_dict)
            {
                if (!Data_list.Server_Properties.ContainsKey(kvp.Key))
                {
                    logger.Warn($"The key ('{kvp.Key}') is not valid in the server.properties.");
                    continue;
                }
                Data_list.Server_Properties[kvp.Key] = kvp.Value;
            }
        }

        private void Data_delete(bool delete_data)
        {
            if (!delete_data)
            {
                return;
            }

            Data_list.Version = Data_list.Argument[0];
            switch (Data_list.Argument.Count)
            {
                case 1:
                    Data_list.World = "";
                    break;
                case 2:
                    Data_list.World = Data_list.Argument[1];
                    break;
            }

            try
            {
                Directory.Delete($@"{Server_GUI2.MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}", true);
                Console.Write($"{Data_list.Version} {Data_list.World + " "}is successfully deleted.{end_str}");
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to delete World data (Error Message : {ex.Message})");
            }

            Finish();
        }

        private void Finish()
        {
            Shutdown();
            Environment.Exit(0);
        }
    }
}
