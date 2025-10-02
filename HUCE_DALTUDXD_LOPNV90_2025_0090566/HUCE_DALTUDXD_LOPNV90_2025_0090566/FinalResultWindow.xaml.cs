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
    public partial class FinalResultWindow : Window
    {
        public FinalResultWindow(bool ketQuaDat = true)
        {
            InitializeComponent();

            // Gán kết quả demo
            txtKQ1.Text = "As yêu cầu = 1200 mm² (demo)";
            txtKQ2.Text = "As bố trí = 1256 mm² (demo)";
            txtKQ3.Text = ketQuaDat ? "Kiểm tra điều kiện: ĐẠT (demo)"
                                    : "Kiểm tra điều kiện: KHÔNG ĐẠT (demo)";

            // Hiển thị ảnh minh hoạ
            string imgPath = "Assets/Images/bo tri cot.png";
            try
            {
                imgTietDien.Source = new BitmapImage(new System.Uri(imgPath, System.UriKind.Relative));
            }
            catch
            {

            }

            // Nếu không đạt yêu cầu → cảnh báo
            if (!ketQuaDat)
            {
                MessageBox.Show("Cốt thép không đạt yêu cầu! Vui lòng quay lại và chỉnh sửa dữ liệu.",
                                "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnBackInput_Click(object sender, RoutedEventArgs e)
        {
            ColumnInputWindow inputWindow = new ColumnInputWindow();
            inputWindow.Show();
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
