using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TinhtoancotBTCT.View;

namespace TinhtoancotBTCT
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Khởi động bằng Login.xaml
            var loginWindow = new View.Login();
            loginWindow.Show();
        }
    }
}

