using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.ViewModels
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        public MainWindowViewModel() { }

        public string PlayerName
        {
            get
            {
                return Data_list.Info[0];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
