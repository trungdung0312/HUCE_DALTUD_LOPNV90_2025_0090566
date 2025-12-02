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
    // Class hỗ trợ hiển thị Psi trong ComboBox (VD: Hiện "Ngàm - Khớp" nhưng giá trị là 0.7)
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
        public List<PsiOption> PsiOptions { get; } = new List<PsiOption>
        {
            new PsiOption { Name = "Ngàm - Ngàm (0.5)", Value = 0.5 },
            new PsiOption { Name = "Ngàm - Khớp (0.7)", Value = 0.7 },
            new PsiOption { Name = "Khớp - Khớp (1.0)", Value = 1.0 },
            new PsiOption { Name = "Ngàm - Tự do (2.0)", Value = 2.0 }
        };

        // Danh sách cột hiển thị trên DataGrid (ObservableCollection giúp giao diện tự cập nhật khi thêm/xóa)
        public ObservableCollection<ColumnInputData> ColumnsList { get; set; }

        // --- 2. CÁC BIẾN TRẠNG THÁI ---

        // Cột đang nhập liệu trên Form
        private ColumnInputData _currentColumn;
        public ColumnInputData CurrentColumn
        {
            get => _currentColumn;
            set { _currentColumn = value; OnPropertyChanged(); }
        }

        // Cột đang được chọn bên DataGrid (để sửa hoặc xóa)
        private ColumnInputData _selectedColumnInGrid;
        public ColumnInputData SelectedColumnInGrid
        {
            get => _selectedColumnInGrid;
            set
            {
                _selectedColumnInGrid = value;
                OnPropertyChanged();
                // Khi chọn dòng bên bảng -> Đổ dữ liệu ngược lại Form để sửa
                if (_selectedColumnInGrid != null)
                {
                    // Copy dữ liệu sang CurrentColumn (Clone) để tránh sửa trực tiếp trên bảng khi chưa bấm Lưu
                    // Nhưng để đơn giản cho bạn lúc này, mình gán trực tiếp.
                    // (Sửa trên Form sẽ nhảy số trên bảng luôn - Tính năng Live Edit)
                    CurrentColumn = _selectedColumnInGrid;
                }
            }
        }

        // --- 3. CÁC LỆNH (COMMANDS) ---
        public ICommand AddColumnCommand { get; set; }
        public ICommand UpdateColumnCommand { get; set; }
        public ICommand DeleteColumnCommand { get; set; }
        public ICommand ClearFormCommand { get; set; }
        public ICommand CalculateCommand { get; set; }

        // --- 4. HÀM KHỞI TẠO (CONSTRUCTOR) - CHẠY ĐẦU TIÊN ---
        public ColumnInputViewModel()
        {
            // Khởi tạo danh sách rỗng
            ColumnsList = new ObservableCollection<ColumnInputData>();

            // Tạo một cột mới tinh để nhập liệu
            CurrentColumn = CreateNewColumn();

            // Gán chức năng cho các nút bấm
            AddColumnCommand = new RelayCommand(ExecuteAddColumn);
            UpdateColumnCommand = new RelayCommand(ExecuteUpdateColumn);
            DeleteColumnCommand = new RelayCommand(ExecuteDeleteColumn);
            ClearFormCommand = new RelayCommand(ExecuteClearForm);
            CalculateCommand = new RelayCommand(ExecuteCalculate);
        }

        // --- 5. LOGIC XỬ LÝ ---

        private ColumnInputData CreateNewColumn()
        {
            // Tạo cột mặc định
            return new ColumnInputData
            {
                ColumnName = "C" + (ColumnsList.Count + 1),
                ConcreteGrade = "B20",
                SteelGrade = "CB400-V",
                B = 300,
                H = 400,
                L = 3600,
                Psi = 0.7,
                ConcreteCover = 25
            };
        }

        private void ExecuteAddColumn(object obj)
        {
            // Logic thêm cột
            if (CurrentColumn == null) return;

            // Kiểm tra trùng tên (nếu cần)

            // Thêm vào danh sách (Sẽ tự hiện lên DataGrid)
            // Lưu ý: Phải tạo đối tượng mới để ngắt kết nối với Form
            var newCol = new ColumnInputData
            {
                ColumnName = CurrentColumn.ColumnName,
                ConcreteGrade = CurrentColumn.ConcreteGrade,
                SteelGrade = CurrentColumn.SteelGrade,
                B = CurrentColumn.B,
                H = CurrentColumn.H,
                L = CurrentColumn.L,
                N = CurrentColumn.N,
                Mx = CurrentColumn.Mx,
                My = CurrentColumn.My,
                Psi = CurrentColumn.Psi,
                ConcreteCover = CurrentColumn.ConcreteCover,
                Rb = CurrentColumn.Rb,
                Rs = CurrentColumn.Rs
            };

            ColumnsList.Add(newCol);

            // Sau khi thêm, tạo form mới để nhập tiếp
            CurrentColumn = CreateNewColumn();
        }

        private void ExecuteUpdateColumn(object obj)
        {
            // Vì mình đang Binding trực tiếp, nên sửa trên Form là sửa trên List rồi.
            // Nút này có thể dùng để thông báo "Đã lưu" hoặc Validate lại.
            MessageBox.Show("Đã cập nhật thông tin cột!", "Thông báo");
        }

        private void ExecuteDeleteColumn(object obj)
        {
            // Xóa các cột ĐANG ĐƯỢC TÍCH CHỌN (CheckBox)
            var itemsToRemove = ColumnsList.Where(c => c.IsSelected).ToList();

            if (itemsToRemove.Count == 0 && SelectedColumnInGrid != null)
            {
                // Nếu không tích chọn cái nào thì xóa cái đang click chuột vào
                itemsToRemove.Add(SelectedColumnInGrid);
            }

            if (itemsToRemove.Count > 0)
            {
                var confirm = MessageBox.Show($"Bạn có chắc muốn xóa {itemsToRemove.Count} cột?", "Xác nhận", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    foreach (var item in itemsToRemove)
                    {
                        ColumnsList.Remove(item);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn cột để xóa!", "Nhắc nhở");
            }
        }

        private void ExecuteClearForm(object obj)
        {
            CurrentColumn = CreateNewColumn();
            // Bỏ chọn bên bảng
            SelectedColumnInGrid = null;
        }

        private void ExecuteCalculate(object obj)
        {
            // 1. Lấy các cột được tích chọn
            var selectedColumns = ColumnsList.Where(c => c.IsSelected).ToList();

            if (selectedColumns.Count == 0)
            {
                MessageBox.Show("Bạn chưa chọn cột nào trong bảng!", "Thông báo");
                return;
            }

            // 2. Tính toán từng cột
            string thongBao = "KẾT QUẢ TÍNH TOÁN SƠ BỘ:\n\n";

            foreach (var col in selectedColumns)
            {
                // Gọi Service tính toán
                var ketQua = Services.TcvnCalculationService.CalculateColumn(col);

                thongBao += $"{ketQua.ColumnName}: {ketQua.ResultString} (As={Math.Round(ketQua.As_Required, 0)} mm2)\n";
            }

            // 3. Hiển thị tạm thời (Sau này sẽ chuyển trang)
            MessageBox.Show(thongBao, "Kết quả");
        }

        // Boilerplate MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
