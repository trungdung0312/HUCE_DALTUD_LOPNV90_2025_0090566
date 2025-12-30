using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.ViewModel
{
    // Class hỗ trợ hiển thị ComboBox Psi
    public class PsiOption
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class ColumnInputViewModel : INotifyPropertyChanged
    {
        // --- 1. CÁC DANH SÁCH DỮ LIỆU (SOURCE) ---
        public List<string> ConcreteGrades { get; } = new List<string> { "B15", "B20", "B25", "B30", "B35", "B40" };
        public List<string> SteelGrades { get; } = new List<string> { "CB240-T", "CB300-V", "CB400-V", "CB500-V" };

        // Danh sách cho Cốt đai
        public List<string> StirrupGrades { get; } = new List<string> { "CB240-T", "CB300-V" };
        public List<int> StirrupDiameters { get; } = new List<int> { 6, 8, 10, 12, 14 };

        public List<PsiOption> PsiOptions { get; } = new List<PsiOption>
        {
            new PsiOption { Name = "Ngàm - Ngàm (0.5)", Value = 0.5 },
            new PsiOption { Name = "Ngàm - Khớp (0.7)", Value = 0.7 },
            new PsiOption { Name = "Khớp - Khớp (1.0)", Value = 1.0 },
            new PsiOption { Name = "Ngàm - Tự do (2.0)", Value = 2.0 }
        };

        // Danh sách cột hiển thị trên DataGrid
        public ObservableCollection<ColumnInputData> ColumnsList { get; set; }

        // --- 2. CÁC BIẾN TRẠNG THÁI ---
        private ColumnInputData _currentColumn;
        public ColumnInputData CurrentColumn
        {
            get => _currentColumn;
            set { _currentColumn = value; OnPropertyChanged(); }
        }

        private ColumnInputData _selectedColumnInGrid;
        public ColumnInputData SelectedColumnInGrid
        {
            get => _selectedColumnInGrid;
            set
            {
                _selectedColumnInGrid = value;
                OnPropertyChanged();
                // Live Edit: Khi chọn dòng, đổ dữ liệu lên Form 
                if (_selectedColumnInGrid != null)
                {
                    CurrentColumn = _selectedColumnInGrid;
                }
            }
        }

        // --- 3. COMMANDS ---
        public ICommand AddColumnCommand { get; set; }
        public ICommand UpdateColumnCommand { get; set; }
        public ICommand DeleteColumnCommand { get; set; }
        public ICommand ClearFormCommand { get; set; }
        public ICommand CalculateCommand { get; set; }

        // --- 4. CONSTRUCTOR ---
        public ColumnInputViewModel()
        {
            ColumnsList = new ObservableCollection<ColumnInputData>();
            CurrentColumn = CreateNewColumn();

            AddColumnCommand = new RelayCommand(ExecuteAddColumn);
            UpdateColumnCommand = new RelayCommand(ExecuteUpdateColumn);
            DeleteColumnCommand = new RelayCommand(ExecuteDeleteColumn);
            ClearFormCommand = new RelayCommand(ExecuteClearForm);
            CalculateCommand = new RelayCommand(ExecuteCalculate);
        }

        // --- 5. LOGIC XỬ LÝ ---

        private ColumnInputData CreateNewColumn()
        {
            // Tạo cột mặc định với các thông số phổ biến
            return new ColumnInputData
            {
                ColumnName = "C" + (ColumnsList.Count + 1),
                ConcreteGrade = "B20",
                SteelGrade = "CB400-V",

                // Mặc định Cốt đai
                StirrupGrade = "CB240-T",
                StirrupDiameter = 6,

                IsVerticalPouring = true, // Mặc định đổ bê tông > 1.5m (an toàn)
                StructureType = StructureType.StaticallyIndeterminate, // Mặc định siêu tĩnh

                B = 300,
                H = 400,
                L = 3600,
                Psi = 0.7,
                ConcreteCover = 25,

                // Khởi tạo nội lực = 0
                N = 0,
                Mx = 0,
                My = 0,
                N_LongTerm = 0,
                Mx_LongTerm = 0,
                My_LongTerm = 0
            };
        }

        private void ExecuteAddColumn(object obj)
        {
            if (CurrentColumn == null) return;

            // COPY toàn bộ dữ liệu từ Form vào đối tượng mới để thêm vào list
            var newCol = new ColumnInputData
            {
                ColumnName = CurrentColumn.ColumnName,

                // Vật liệu & Thi công
                ConcreteGrade = CurrentColumn.ConcreteGrade,
                Rb = CurrentColumn.Rb,
                IsVerticalPouring = CurrentColumn.IsVerticalPouring,

                SteelGrade = CurrentColumn.SteelGrade,
                Rs = CurrentColumn.Rs,

                StirrupGrade = CurrentColumn.StirrupGrade,
                StirrupDiameter = CurrentColumn.StirrupDiameter,
                Rsw = CurrentColumn.Rsw,

                // Hình học
                B = CurrentColumn.B,
                H = CurrentColumn.H,
                L = CurrentColumn.L,
                ConcreteCover = CurrentColumn.ConcreteCover,
                StructureType = CurrentColumn.StructureType,
                Psi = CurrentColumn.Psi,

                // Nội lực
                N = CurrentColumn.N,
                Mx = CurrentColumn.Mx,
                My = CurrentColumn.My,
                N_LongTerm = CurrentColumn.N_LongTerm,
                Mx_LongTerm = CurrentColumn.Mx_LongTerm,
                My_LongTerm = CurrentColumn.My_LongTerm
            };

            ColumnsList.Add(newCol);

            // Reset Form để nhập cái tiếp theo
            CurrentColumn = CreateNewColumn();
        }

        private void ExecuteUpdateColumn(object obj)
        {
            // Vì Binding Mode=TwoWay, dữ liệu đã được cập nhật.
            // Chỉ cần báo thành công.
            MessageBox.Show("Đã lưu thông tin!", "Thông báo");
        }

        private void ExecuteDeleteColumn(object obj)
        {
            // Xóa các cột ĐANG ĐƯỢC TÍCH CHỌN
            var itemsToRemove = ColumnsList.Where(c => c.IsSelected).ToList();

            if (itemsToRemove.Count == 0 && SelectedColumnInGrid != null)
            {
                itemsToRemove.Add(SelectedColumnInGrid);
            }

            if (itemsToRemove.Count > 0)
            {
                var confirm = MessageBox.Show($"Xóa {itemsToRemove.Count} cột đã chọn?", "Xác nhận", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    foreach (var item in itemsToRemove) ColumnsList.Remove(item);
                    ExecuteClearForm(null); // Reset form nếu xóa cái đang chọn
                }
            }
            else
            {
                MessageBox.Show("Chọn cột để xóa!", "Nhắc nhở");
            }
        }

        private void ExecuteClearForm(object obj)
        {
            CurrentColumn = CreateNewColumn();
            SelectedColumnInGrid = null; // Bỏ chọn trên bảng
        }

        private void ExecuteCalculate(object obj)
        {
            // 1. Lọc các cột được tích chọn
            var selectedColumns = ColumnsList.Where(c => c.IsSelected).ToList();

            if (selectedColumns.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn cột trong bảng để tính!", "Chưa chọn");
                return;
            }

            // 2. Gọi Service tính toán 
            var results = new List<RebarResultData>();
            foreach (var col in selectedColumns)
            {
                try
                {
                    var res = Services.TcvnCalculationService.CalculateColumn(col);
                    results.Add(res);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tính cột {col.ColumnName}: {ex.Message}", "Lỗi");
                }
            }

            // 3. Chuyển trang hiển thị kết quả
            var mainWindow = Application.Current.MainWindow as Views.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ShowResults(results);
            }
        }

        // Boilerplate MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}