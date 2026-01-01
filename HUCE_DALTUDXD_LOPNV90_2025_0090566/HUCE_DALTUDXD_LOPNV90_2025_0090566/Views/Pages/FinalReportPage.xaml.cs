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
    public partial class FinalReportPage : Page
    {
        private FinalReportViewModel _viewModel;

        public FinalReportPage()
        {
            InitializeComponent();
            _viewModel = new FinalReportViewModel();
            this.DataContext = _viewModel;
        }

        public void LoadData(List<RebarResultData> data)
        {
            // Truyền dữ liệu vào ViewModel để hiển thị và chuẩn bị xuất file
            _viewModel.LoadReportData(data);
        }
    }
}
