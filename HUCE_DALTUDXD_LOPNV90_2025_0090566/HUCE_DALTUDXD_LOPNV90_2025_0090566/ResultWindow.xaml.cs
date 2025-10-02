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

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566
{
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
        }

        // Nút trở lại → đóng cửa sổ kết quả, mở lại giao diện nhập
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ColumnInputWindow inputWindow = new ColumnInputWindow();
            inputWindow.Show();
            this.Close();
        }

        // Nút tiếp tục → đóng cửa sổ kết quả, mở cửa sổ bố trí cốt thép (RebarWindow)
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            RebarWindow rebarWindow = new RebarWindow();
            rebarWindow.Show();
            this.Close();
        }
    }
}
