// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Data;
// using System.Windows.Documents;
// using System.Windows.Input;
// using System.Windows.Media;
// using System.Windows.Media.Imaging;
// using System.Windows.Shapes;

// using System.Threading;

// namespace Server_GUI2
// {
//     /// <summary>
//     /// Progress_Bar.xaml の相互作用ロジック
//     /// </summary>
//     public partial class Progress_Bar : Window
//     {
//         //フォームが表示されるまで待機するための待機ハンドル
//         private System.Threading.ManualResetEvent startEvent;

//         //別処理をするためのスレッド
//         private System.Threading.Thread thread;

//         public Progress_Bar()
//         {
//             InitializeComponent();
//         }

//         private void InitBar()
//         {
//             Set_value(0);
//             ShowDialog();
//         }

//         [Obsolete]
//         public void Show_progress_dialog()
//         {
//             startEvent = new ManualResetEvent(false);

//             //スレッドを作成
//             thread = new Thread(new ThreadStart(InitBar));
//             thread.IsBackground = true;
//             this.thread.ApartmentState = ApartmentState.STA;
//             thread.Start();

//             //フォームが表示されるまで待機する
//             startEvent.WaitOne();
//         }

//         public void Set_progress(int ratio, string text)
//         {
//             Set_value(ratio);
//             Set_log(text);
//         }

//         public void Set_value(int ratio)
//         {
//             bar.Value = ratio;
//             bar_display.Text = $"Finish {ratio}%";
//             if(ratio == 100)
//             {
//                 Close();
//             }
//         }

//         public void Set_log(string text)
//         {
//             log.Text += $"{text}\n";
//         }

//         public void Set_Title(string title)
//         {
//             title_box.Text = $"Ready to {title} ...";
//         }
//     }

//     public class ProgressDialog
//     {
//         //フォームが表示されるまで待機するための待機ハンドル
//         private ManualResetEvent startEvent;
//         //ダイアログフォーム
//         private volatile Progress_Bar form;

//         //別処理をするためのスレッド
//         private Thread thread;

//         private static string _title = "Ready to XXXX ...";
//         private static string _logs = "";
//         private static int _value = 0;

//         private void Run()
//         {
//             form = new Progress_Bar();

//             form.Show();
//         }

//         [Obsolete]
//         public void Show()
//         {
//             // startEvent = new ManualResetEvent(false);

//             //スレッドを作成
//             thread = new Thread(new ThreadStart(Run));
//             thread.IsBackground = true;
//             this.thread.ApartmentState = ApartmentState.STA;
//             thread.Start();

//             //フォームが表示されるまで待機する
//             // startEvent.WaitOne();
//         }

//         public string Title
//         {
//             set
//             {
//                 _title = value;
//                 form.Set_Title(_title);
//             }
//             get
//             {
//                 return _title;
//             }
//         }

//         public int Value
//         {
//             set
//             {
//                 _value = value;
//                 form.Set_value(_value);
//             }
//             get
//             {
//                 return _value;
//             }
//         }

//         public string Log
//         {
//             set
//             {
//                 _logs = value;
//                 form.Set_log(_logs);
//             }
//             get
//             {
//                 return _logs;
//             }
//         }

//     }

// }
