using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Views.Pages;
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

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Views
{
    public partial class MainWindow : Window
    {
        // --- KHAI BÁO CÁC TRANG ---
        private ColumnInputPage _inputPage;
        private ResultPreviewPage _resultPage;
        private RebarDesignPage _rebarPage;
        private FinalReportPage _reportPage;

        // Biến lưu trữ dữ liệu hiện tại để dùng chung cho các trang
        private List<RebarResultData> _currentData;

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
            MainFrame.Navigate(_inputPage);
        }

        private void BtnResult_Click(object sender, RoutedEventArgs e)
        {
            // Nếu đã có dữ liệu, update lại trang kết quả trước khi vào (phòng khi sửa thép xong quay lại)
            if (_currentData != null) _resultPage.LoadData(_currentData);
            MainFrame.Navigate(_resultPage);
        }

        private void BtnRebar_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_rebarPage);
        }

        private void BtnFinal_Click(object sender, RoutedEventArgs e)
        {
            // Nạp dữ liệu mới nhất vào trang báo cáo trước khi chuyển trang
            
            if (_currentData != null)
            {
                _reportPage.LoadData(_currentData);
            }

            MainFrame.Navigate(_reportPage);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        // Hàm này được gọi từ ColumnInputPage sau khi bấm "TÍNH TOÁN"
        public void ShowResults(List<RebarResultData> results)
        {
            // Lưu dữ liệu vào biến toàn cục của MainWindow
            _currentData = results;

            // Nạp dữ liệu vào trang Xem trước
            _resultPage.LoadData(_currentData);

            // Nạp luôn vào trang Báo cáo để sẵn sàng
            _reportPage.LoadData(_currentData);

            // Chuyển người dùng đến trang Xem trước
            MainFrame.Navigate(_resultPage);
        }

        // Hàm này được gọi từ ResultPreviewPage khi bấm "CHỌN THÉP"
        public void GoToRebarDesign(RebarResultData selectedColumn)
        {
            // Nạp dữ liệu vào trang thiết kế
            _rebarPage.LoadColumnData(selectedColumn);

            // Chuyển trang
            MainFrame.Navigate(_rebarPage);
        }
    }
}
