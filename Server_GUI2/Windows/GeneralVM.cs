using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.ViewModels
{
    class GeneralVM
    {
    }

    interface IOperateWindows
    {
        Action Show { get; set; }
        Action Hide { get; set; }
        Action Close { get; set; }
    }

}
