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
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Views.Pages;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Views
{
    public partial class MainWindow : Window
    {
        // --- KHAI BÁO CÁC TRANG (Singleton-like) ---
        // Khai báo ở đây để giữ dữ liệu không bị mất khi chuyển trang
        private ColumnInputPage _inputPage;
        private ResultPreviewPage _resultPage;
        private RebarDesignPage _rebarPage;
        private FinalReportPage _reportPage;

        public MainWindow()
        {
            InitializeComponent();

            // Khởi tạo các trang 1 lần duy nhất
            _inputPage = new ColumnInputPage();
            _resultPage = new ResultPreviewPage();
            _rebarPage = new RebarDesignPage();
            _reportPage = new FinalReportPage();

            // Mặc định hiển thị trang nhập liệu đầu tiên
            MainFrame.Navigate(_inputPage);
        }

        // --- SỰ KIỆN CHUYỂN TRANG ---

        private void BtnInput_Click(object sender, RoutedEventArgs e)
        {
            // Điều hướng đến trang đã khởi tạo sẵn
            MainFrame.Navigate(_inputPage);
        }

        private void BtnResult_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_resultPage);
        }

        private void BtnRebar_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_rebarPage);
        }

        private void BtnFinal_Click(object sender, RoutedEventArgs e)
        {
            // Chuyển sang trang báo cáo
            MainFrame.Navigate(_reportPage);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            // Hỏi người dùng trước khi thoát
            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        public void ShowResults(System.Collections.Generic.List<HUCE_DALTUDXD_LOPNV90_2025_0090566.Model.RebarResultData> results)
        {
            // Nạp dữ liệu vào trang Xem trước (Page 2)
            _resultPage.LoadData(results);

            // QUAN TRỌNG: Nạp LUÔN dữ liệu vào trang Báo cáo (Page 4) để dành
            _reportPage.LoadData(results);

            // Chuyển người dùng đến trang Xem trước
            MainFrame.Navigate(_resultPage);
        }
        public void GoToRebarDesign(Model.RebarResultData selectedColumn)
        {
            // Nạp dữ liệu vào trang thiết kế
            _rebarPage.LoadColumnData(selectedColumn);

            // Chuyển trang
            MainFrame.Navigate(_rebarPage);
        }
    }

}
