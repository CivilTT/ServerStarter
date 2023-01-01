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
        public Properties.Resources Resources => new Properties.Resources();

        public string Title { get; private set; }
        public BindingValue<int> ProgressValue { get; private set; }
        public BindingValue<string> SubMessage { get; private set; }
        public string DisplayProgressValue => $"{Properties.Resources.ProgressBra_Count1} {ProgressValue.Value}% {Properties.Resources.ProgressBra_Count2}";
        public int MaxValue { get; private set; }
        public int MinValue { get; private set; }
        public BindingValue<bool> Moving { get; private set; }
        public BindingValue<string> Messages { get; private set; }

        public ProgressBarDialogVM(string title, int maxV, int minV)
        {
            Title = title;
            ProgressValue = new BindingValue<int>(0, () => OnPropertyChanged(new string[2] { "ProgressValue", "DisplayProgressValue" }));
            SubMessage = new BindingValue<string>("", () => OnPropertyChanged("SubMessage"));
            MaxValue = maxV;
            MinValue = minV;
            Moving = new BindingValue<bool>(false, () => OnPropertyChanged("Moving"));
            Messages = new BindingValue<string>("", () => OnPropertyChanged("Messages"));
        }
    }
}
