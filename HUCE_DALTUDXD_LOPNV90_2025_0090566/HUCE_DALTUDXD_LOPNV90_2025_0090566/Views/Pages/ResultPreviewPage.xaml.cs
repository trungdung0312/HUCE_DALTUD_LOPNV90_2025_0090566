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
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.ViewModel;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Views.Pages
{
    public partial class ResultPreviewPage : Page
    {
        // Giữ tham chiếu đến ViewModel để gọi hàm cập nhật
        private ResultPreviewViewModel _viewModel;

        public ResultPreviewPage()
        {
            InitializeComponent();
            _viewModel = new ResultPreviewViewModel();
            this.DataContext = _viewModel;
        }

        // Hàm này sẽ được MainWindow gọi để đẩy dữ liệu vào
        public void LoadData(List<RebarResultData> data)
        {
            _viewModel.UpdateResults(data);
        }
    }
}
