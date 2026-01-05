using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.ViewModel
{
    // Class con quản lý thông tin thép 1 phương
    public class SteelSelectionInfo : INotifyPropertyChanged
    {
        public string DirectionName { get; set; }
        public ObservableCollection<int> DiameterList { get; } = new ObservableCollection<int> { 10, 12, 14, 16, 18, 20, 22, 25, 28, 32 };
        public RebarDesignViewModel Parent { get; set; }

        private int _q1;
        public int Q1 { get => _q1; set { if (_q1 != value) { _q1 = value; OnPropertyChanged(); Parent?.RecalculateTotal(); } } }

        private int _d1;
        public int D1 { get => _d1; set { if (_d1 != value) { _d1 = value; OnPropertyChanged(); Parent?.RecalculateTotal(); } } }

        private int _q2;
        public int Q2 { get => _q2; set { if (_q2 != value) { _q2 = value; OnPropertyChanged(); Parent?.RecalculateTotal(); } } }

        private int _d2;
        public int D2 { get => _d2; set { if (_d2 != value) { _d2 = value; OnPropertyChanged(); Parent?.RecalculateTotal(); } } }

        // Tính diện tích thô (để cộng dồn)
        public double RawArea => (Q1 * Math.PI * Math.Pow(D1, 2) / 4.0) + (Q2 * Math.PI * Math.Pow(D2, 2) / 4.0);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // ViewModel chính
    public class RebarDesignViewModel : INotifyPropertyChanged
    {
        public SteelSelectionInfo SteelX { get; set; }
        public SteelSelectionInfo SteelY { get; set; }

        private bool _isLoading = false;

        private RebarResultData _designColumn;
        public RebarResultData DesignColumn
        {
            get => _designColumn;
            set
            {
                _designColumn = value;
                OnPropertyChanged();
                // Khi nhận cột mới -> Load dữ liệu vào các ô nhập
                if (_designColumn != null) LoadDataFromModel();
            }
        }

        public string CheckStatus { get; set; } 
        public Brush StatusColor { get; set; }
        public ICommand SaveCommand { get; set; }

        public RebarDesignViewModel()
        {
            SteelX = new SteelSelectionInfo { DirectionName = "Phương X (Cạnh B)", Parent = this };
            SteelY = new SteelSelectionInfo { DirectionName = "Phương Y (Cạnh H)", Parent = this };
            SaveCommand = new RelayCommand(ExecuteSave);
        }

        private void LoadDataFromModel()
        {
            if (DesignColumn == null) return;

            _isLoading = true; // Chặn sự kiện tính toán khi đang load

            try 
            {
                // KIỂM TRA LOGIC ĐẢO TRỤC (QUAN TRỌNG CHO LỆCH TÂM XIÊN)
                // Nếu Note có chữ "Xiên->Y", nghĩa là Service đã tính trên tiết diện bị xoay 90 độ.
                // Lúc này: Kết quả thép X của Service thực chất là thép cho cạnh H (Phương Y).
                //          Kết quả thép Y của Service thực chất là thép cho cạnh B (Phương X).
                // -> TA PHẢI ĐỔI CHỖ LẠI ĐỂ HIỂN THỊ ĐÚNG TRÊN HÌNH GỐC.
                
                bool isSwapped = !string.IsNullOrEmpty(DesignColumn.Note) && DesignColumn.Note.Contains("Xiên->Y");

                if (isSwapped)
                {
                    // Nạp Thép X (lấy từ Y của Model)
                    SteelX.Q1 = DesignColumn.Quantity1_Y;
                    SteelX.D1 = DesignColumn.Diameter1_Y > 0 ? DesignColumn.Diameter1_Y : 20;
                    SteelX.Q2 = DesignColumn.Quantity2_Y;
                    SteelX.D2 = DesignColumn.Diameter2_Y > 0 ? DesignColumn.Diameter2_Y : 20;

                    // Nạp Thép Y (lấy từ X của Model)
                    SteelY.Q1 = DesignColumn.Quantity1_X;
                    SteelY.D1 = DesignColumn.Diameter1_X > 0 ? DesignColumn.Diameter1_X : 20;
                    SteelY.Q2 = DesignColumn.Quantity2_X;
                    SteelY.D2 = DesignColumn.Diameter2_X > 0 ? DesignColumn.Diameter2_X : 20;
                }
                else
                {
                    // Nạp bình thường (X vào X, Y vào Y)
                    SteelX.Q1 = DesignColumn.Quantity1_X;
                    SteelX.D1 = DesignColumn.Diameter1_X > 0 ? DesignColumn.Diameter1_X : 20;
                    SteelX.Q2 = DesignColumn.Quantity2_X;
                    SteelX.D2 = DesignColumn.Diameter2_X > 0 ? DesignColumn.Diameter2_X : 20;

                    SteelY.Q1 = DesignColumn.Quantity1_Y;
                    SteelY.D1 = DesignColumn.Diameter1_Y > 0 ? DesignColumn.Diameter1_Y : 20;
                    SteelY.Q2 = DesignColumn.Quantity2_Y;
                    SteelY.D2 = DesignColumn.Diameter2_Y > 0 ? DesignColumn.Diameter2_Y : 20;
                }
            }
            finally
            {
                _isLoading = false;
                RecalculateTotal(); // Tính toán lại trạng thái Đạt/Không đạt
            }
        }

        public void RecalculateTotal()
        {
            if (_isLoading || DesignColumn == null) return;

            // Cập nhật ngược lại vào Model (Lưu ý: Không cần đảo lại khi lưu, cứ lưu thẳng cái người dùng nhìn thấy)
            // Hoặc để chuẩn xác với logic tính toán, ta có thể đảo lại, nhưng để đơn giản cho hiển thị, ta gán thẳng.
            DesignColumn.Quantity1_X = SteelX.Q1; DesignColumn.Diameter1_X = SteelX.D1;
            DesignColumn.Quantity2_X = SteelX.Q2; DesignColumn.Diameter2_X = SteelX.D2;
            DesignColumn.Quantity1_Y = SteelY.Q1; DesignColumn.Diameter1_Y = SteelY.D1;
            DesignColumn.Quantity2_Y = SteelY.Q2; DesignColumn.Diameter2_Y = SteelY.D2;

            // Tính tổng (Cộng dồn)
            double totalAs = SteelX.RawArea + SteelY.RawArea; 
            
            DesignColumn.As_Provided = Math.Round(totalAs, 3);
            DesignColumn.Mu_Percentage = Math.Round((totalAs / (DesignColumn.B * DesignColumn.H)) * 100.0, 2);

            // Kiểm tra bền (Cho phép sai số -1.0mm2)
            if (totalAs >= DesignColumn.As_Required - 1.0)
            {
                double du = totalAs - DesignColumn.As_Required;
                CheckStatus = $"✅ ĐẠT (Dư {du:N0} mm²)";
                StatusColor = Brushes.Green;
                DesignColumn.Status = "Đạt";
            }
            else
            {
                double thieu = DesignColumn.As_Required - totalAs;
                CheckStatus = $"❌ KHÔNG ĐẠT (Thiếu {thieu:N0} mm²)";
                StatusColor = Brushes.Red;
                DesignColumn.Status = "Không đạt";
            }

            if (DesignColumn.Mu_Percentage > 4.0)
            {
                CheckStatus = "⚠️ HÀM LƯỢNG LỚN (>4%)";
                StatusColor = Brushes.OrangeRed;
            }

            // Báo cho View cập nhật
            OnPropertyChanged(nameof(DesignColumn));
            OnPropertyChanged(nameof(CheckStatus));
            OnPropertyChanged(nameof(StatusColor));
        }

        private void ExecuteSave(object obj)
        {
            RecalculateTotal();
            // Gọi cập nhật chuỗi hiển thị kết quả (nếu Model có property ResultString)
            DesignColumn.GetType().GetProperty("ResultString")?.GetValue(DesignColumn);
            MessageBox.Show("Đã lưu phương án thiết kế!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
