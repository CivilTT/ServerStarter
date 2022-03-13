using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server_GUI2.Windows.ProgressBar
{
    class ProgressBarDialogVM : GeneralVM
    {
        public string Title { get; private set; }
        public BindingValue<int> ProgressValue { get; private set; }
        private int Counter = 0;
        private readonly int MaxCount = 0;
        public string DisplayProgressValue => $"Finished {ProgressValue.Value}%";
        public int MaxValue { get; private set; }
        public int MinValue { get; private set; }
        public List<string> Comments { get; private set; }

        public ProgressBarDialogVM(string title, int maxCount, int maxV=100, int minV=0)
        {
            Title = title;
            MaxCount = maxCount;
            ProgressValue = new BindingValue<int>(0, () => OnPropertyChanged(new string[2] { "ProgressValue", "DisplayProgressValue" }));
            MaxValue = maxV;
            MinValue = minV;
            Comments = new List<string>();
        }

        /// <summary>
        /// Barの進行を進める
        /// </summary>
        public void AddCount()
        {
            Counter++;
            ProgressValue.Value = Counter * 100 / MaxCount;
        }

        /// <summary>
        /// AddCountが何回呼ばれたかを表示する
        /// デバッグ用のメソッド
        /// Close時のこの回数をmaxCountに入れると全体が100％になるように計算される
        /// </summary>
        public void ShowCounter()
        {
            MessageBox.Show($"Counter : {Counter} times");
        }
    }
}
