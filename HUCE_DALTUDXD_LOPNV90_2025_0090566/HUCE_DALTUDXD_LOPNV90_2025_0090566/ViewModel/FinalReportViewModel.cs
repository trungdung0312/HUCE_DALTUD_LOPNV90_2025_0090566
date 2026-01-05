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
        // Danh sách hiển thị lên bảng 
        public ObservableCollection<RebarResultData> ReportList { get; set; }

        public ICommand ExportCommand { get; set; }

        public FinalReportViewModel()
        {
            ReportList = new ObservableCollection<RebarResultData>();
            ExportCommand = new RelayCommand(ExecuteExport);
        }

        // Hàm nhận dữ liệu
        public void LoadReportData(List<RebarResultData> data)
        {
            ReportList.Clear();
            if (data != null)
            {
                foreach (var item in data)
                {
                    // Chỉ thêm cột đã tính xong
                    if (!string.IsNullOrEmpty(item.Status))
                    {
                        ReportList.Add(item);
                    }
                }
            }
        }

        private void ExecuteExport(object obj)
        {
            if (ReportList.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để xuất báo cáo!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            // Lưu file đuôi .xls nhưng nội dung là HTML -> Excel vẫn mở tốt và giữ định dạng màu sắc đẹp hơn CSV
            saveDialog.Filter = "Excel Web File (*.xls)|*.xls|HTML Report (*.html)|*.html";
            saveDialog.FileName = $"ThuyetMinh_Cot_{DateTime.Now:ddMMyy}.xls";

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    var dataToList = new List<RebarResultData>(ReportList);

                    // Gọi Service đã nâng cấp
                    Services.ExportService.ExportToHtml(dataToList, saveDialog.FileName);

                    var res = MessageBox.Show("Xuất file thành công!\nBạn có muốn mở file ngay không?", "Thành công", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (res == MessageBoxResult.Yes)
                    {
                        // Mở file bằng phần mềm mặc định (Excel hoặc Trình duyệt)
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
