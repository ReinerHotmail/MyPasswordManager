using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace MyPasswordManager
{
    
    public partial class MainWindow
    {
        DispatcherTimer TimerStart = new DispatcherTimer();
        DispatcherTimer TimerLoginShort = new DispatcherTimer();
        string PmPath = "";
        List<CPwDat> ListPw = new List<CPwDat>();
        int ShortLogin = 0;
        string LoginImage = "";



    }
}
