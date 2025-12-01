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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.ViewModel;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Views.Pages
{
    public partial class ColumnInputPage : Page
    {
        public ColumnInputPage()
        {
            InitializeComponent();

            // --- QUÁ TRÌNH KHỞI TẠO (INITIALIZATION) ---
            // 1. Tạo một "bộ não" mới (ViewModel)
            var viewModel = new ColumnInputViewModel();

            // 2. Gán "bộ não" đó vào DataContext của trang này
            // DataContext là nơi Giao diện (XAML) tìm kiếm các biến như "CurrentColumn", "ConcreteGrades"...
            this.DataContext = viewModel;
        }
    }
}
