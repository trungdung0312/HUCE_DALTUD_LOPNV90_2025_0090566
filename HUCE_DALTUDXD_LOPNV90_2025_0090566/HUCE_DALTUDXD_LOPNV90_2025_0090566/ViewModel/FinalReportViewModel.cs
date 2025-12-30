using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32; // Thư viện hộp thoại lưu file
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.ViewModel
{
    public class FinalReportViewModel : INotifyPropertyChanged
    {
        // Danh sách để hiển thị lên bảng báo cáo
        public ObservableCollection<RebarResultData> ReportList { get; set; }

        // Lệnh xuất file
        public ICommand ExportCommand { get; set; }

        public FinalReportViewModel()
        {
            ReportList = new ObservableCollection<RebarResultData>();
            ExportCommand = new RelayCommand(ExecuteExport);
        }

        // Hàm nhận dữ liệu từ các trang trước
        public void LoadReportData(List<RebarResultData> data)
        {
            ReportList.Clear();
            foreach (var item in data)
            {
                ReportList.Add(item);
            }
        }

        private void ExecuteExport(object obj)
        {
            if (ReportList.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để xuất báo cáo!", "Thông báo");
                return;
            }

            // Mở hộp thoại chọn nơi lưu file
            SaveFileDialog saveDialog = new SaveFileDialog();
            // Cho phép lưu thành file .xls (Excel tự mở được HTML)
            saveDialog.Filter = "Excel File (*.xls)|*.xls|HTML File (*.html)|*.html";
            saveDialog.FileName = "ThuyetMinh_Cot_BTCT.xls";

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    // Chuyển ObservableCollection sang List để gửi cho Service
                    var dataToList = new List<RebarResultData>(ReportList);

                    // Gọi Service xuất file
                    Services.ExportService.ExportToHtml(dataToList, saveDialog.FileName);

                    var result = MessageBox.Show("Xuất file thành công!\nBạn có muốn mở file ngay không?", "Thành công", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Tự động mở file vừa xuất
                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi khi xuất file: " + ex.Message, "Lỗi");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
