using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.ViewModel
{
    public class ResultPreviewViewModel : INotifyPropertyChanged
    {
        // Danh sách chứa kết quả tính toán để hiện lên bảng
        public ObservableCollection<RebarResultData> ResultsList { get; set; }

        // Biến lưu trữ dòng đang được chọn trong bảng
        private RebarResultData _selectedResult;
        public RebarResultData SelectedResult
        {
            get => _selectedResult;
            set { _selectedResult = value; OnPropertyChanged(); }
        }

        public ResultPreviewViewModel()
        {
            ResultsList = new ObservableCollection<RebarResultData>();
        }

        // Hàm nhận dữ liệu từ trang Nhập liệu gửi sang
        public void UpdateResults(List<RebarResultData> newResults)
        {
            ResultsList.Clear();
            foreach (var item in newResults)
            {
                ResultsList.Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
