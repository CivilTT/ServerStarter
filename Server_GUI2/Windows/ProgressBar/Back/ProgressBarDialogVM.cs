using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Server_GUI2.Windows.ProgressBar.Back
{
    class ProgressBarDialogVM : GeneralVM
    {
        public string Title { get; private set; }
        public BindingValue<int> ProgressValue { get; private set; }
        public string DisplayProgressValue => $"Finished {ProgressValue.Value}%";
        public int MaxValue { get; private set; }
        public int MinValue { get; private set; }
        public BindingValue<string> Messages { get; private set; }

        public ProgressBarDialogVM(string title, int maxV, int minV)
        {
            Title = title;
            ProgressValue = new BindingValue<int>(0, () => OnPropertyChanged(new string[2] { "ProgressValue", "DisplayProgressValue" }));
            MaxValue = maxV;
            MinValue = minV;
            Messages = new BindingValue<string>("", () => OnPropertyChanged("Messages"));
        }
    }
}
