using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TinhtoancotBTCT;

namespace TinhtoancotBTCT.View
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        // Kéo cửa sổ khi nhấn giữ chuột vào Border
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        // Nút đóng
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Xử lý nút đăng nhập
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Password.Trim();

            if (user == "admin" && pass == "123")
            {
                MainWindow mw = new MainWindow(); // mở cửa sổ chính
                mw.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }
    }
}