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
        // Danh sách kết quả hiển thị trên bảng
        public ObservableCollection<RebarResultData> ResultsList { get; set; }

        // Dòng kết quả đang được chọn
        private RebarResultData _selectedResult;
        public RebarResultData SelectedResult
        {
            get => _selectedResult;
            set
            {
                _selectedResult = value;
                OnPropertyChanged();
            }
        }

        public ResultPreviewViewModel()
        {
            ResultsList = new ObservableCollection<RebarResultData>();
        }

        // Hàm cập nhật dữ liệu từ MainWindow gửi sang
        public void UpdateResults(List<RebarResultData> data)
        {
            ResultsList.Clear();
            if (data != null)
            {
                foreach (var item in data)
                {
                    ResultsList.Add(item);
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
