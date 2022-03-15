using log4net;
using Server_GUI2.Windows.ProgressBar.Back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Server_GUI2.Windows.ProgressBar
{
    public class ProgressBar
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ProgressBarDialogVM ProgressVM;

        private readonly string Title;
        private readonly int MaxV;
        private readonly int MinV;

        private int Percent = 0;
        private int Counter = 0;
        private readonly int AllCount = 0;

        private string oldMessage = "";

        public ProgressBar(string title, int allCount, int maxV = 100, int minV = 0)
        {
            Title = title;
            AllCount = allCount;
            MaxV = maxV;
            MinV = minV;

            // １つのProgressBarインスタンスに対してWindowが１つ立ち上がるようにする
            Show();
        }

        private void Show()
        {
            var progressBar = new ShowNewWindow<ProgressBarDialog, ProgressBarDialogVM>();
            ProgressVM = new ProgressBarDialogVM(Title, MaxV, MinV);
            Thread thread = new Thread(new ParameterizedThreadStart(_Show))
            {
                IsBackground = true,

            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(ProgressVM);

            logger.Info("Shown Progress Bar");
        }

        private void _Show(object vm)
        {
            var progressBar = new ShowNewWindow<ProgressBarDialog, ProgressBarDialogVM>();
            progressBar.ShowDialog((ProgressBarDialogVM)vm);
        }

        /// <summary>
        /// Barの進行を進める
        /// </summary>
        public void AddCount()
        {
            Counter++;
            Percent = Counter * 100 / AllCount;
            ProgressVM.ProgressValue.Value = Percent;
        }

        /// <summary>
        /// 進行状況のメッセージを追加する
        /// 進行状況の数字も進む
        /// </summary>
        /// <param name="addCount">falseの時、カウントを増やさず、メッセージのみ追記する</param>
        public void AddMessage(string message, bool moving=false, bool addCount=true)
        {
            if (message == null)
                return;

            Match header = Regex.Match(message, @"\w+:");
            bool headerBool = header.Success;
            string headerStr = header.Value;

            Match percent = Regex.Match(message, @"\d+%");
            bool percentBool = percent.Success;
            string percentStr = percent.Value;

            if (moving)
            {
                ProgressVM.Moving.Value = true;
                ProgressVM.SubMessage.Value = message;
            }

            if (headerBool && percentBool)
            {
                ProgressVM.Messages.Value = $"{oldMessage}{message}\n";
                if (message.Contains(" done"))
                    oldMessage = ProgressVM.Messages.Value;
            }
            else
            {
                oldMessage = ProgressVM.Messages.Value;
                ProgressVM.Messages.Value += $"{message}\n";
            }

            if (addCount)
                AddCount();

            logger.Info($"[ProgressBar] Message : '{message}', Value : {Percent}%");
        }

        public void ReShow()
        {
            ProgressVM.Show();
        }

        public void Hide()
        {
            ProgressVM.Hide();
        }

        public void Close()
        {
            ProgressVM.Close();
        }

        /// <summary>
        /// AddCountが何回呼ばれたかを表示する
        /// デバッグ用のメソッド
        /// Close時のこの回数をmaxCountに入れると全体が100％になるように計算される
        /// </summary>
        public void ShowCount()
        {
            MessageBox.Show($"Counter : {Counter} times");
        }
    }
}
