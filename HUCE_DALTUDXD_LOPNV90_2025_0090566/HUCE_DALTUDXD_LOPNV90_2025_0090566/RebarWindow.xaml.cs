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
    public partial class RebarWindow : Window
    {
        public RebarWindow()
        {
            InitializeComponent();
        }
        // Nút kiểm tra (demo kết quả giả lập)
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            string soThanh = (cbSoThanh.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(soThanh))
            {
                MessageBox.Show("Hãy chọn số thanh bố trí!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Demo: gắn kết quả kiểm tra cứng
            txtKiemTraAs.Text = "As yêu cầu: 1200 mm² (demo)";
            txtChonThep.Text = $"Đã chọn: {soThanh}Ø20 (demo)";
            txtCheckCover.Text = "Kiểm tra lớp bảo vệ: Đạt (demo)";
            txtCheckSpacing.Text = "Kiểm tra khoảng cách: Đạt (demo)";
        }

        // Nút trở lại (quay lại ResultWindow)
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ResultWindow resultWindow = new ResultWindow();
            resultWindow.Show();
            this.Close();
        }

        // Nút tiếp tục ( mở sang cửa sổ minh họa sơ đồ cốt thép)
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            // Ví dụ: cho kết quả "đạt" (true). Sau này tính toán xong sẽ truyền true/false thực tế.
            FinalResultWindow finalWindow = new FinalResultWindow(true);
            finalWindow.Show();
            this.Close();
        }
    }
}
