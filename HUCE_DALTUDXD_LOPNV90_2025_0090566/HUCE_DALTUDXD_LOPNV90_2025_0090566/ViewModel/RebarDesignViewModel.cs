using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.ViewModel
{
    public class RebarDesignViewModel : INotifyPropertyChanged
    {
        // Danh sách đường kính để chọn (ComboBox)
        public List<int> DiameterOptions { get; } = new List<int> { 12, 14, 16, 18, 20, 22, 25, 28, 32 };

        // Cột đang được thiết kế
        private RebarResultData _designColumn;
        public RebarResultData DesignColumn
        {
            get => _designColumn;
            set { _designColumn = value; OnPropertyChanged(); Recalculate(); }
        }

        // --- CÁC BIẾN CHO PHÉP SỬA TRÊN GIAO DIỆN ---
        // Số thanh thép chọn
        public int EditBarQuantity
        {
            get => DesignColumn?.BarQuantity ?? 0;
            set
            {
                if (DesignColumn != null)
                {
                    DesignColumn.BarQuantity = value;
                    OnPropertyChanged();
                    Recalculate(); // Tính lại ngay khi sửa
                }
            }
        }

        // Đường kính thép chọn
        public int EditBarDiameter
        {
            get => DesignColumn?.BarDiameter ?? 0;
            set
            {
                if (DesignColumn != null)
                {
                    DesignColumn.BarDiameter = value;
                    OnPropertyChanged();
                    Recalculate();
                }
            }
        }

        // --- KẾT QUẢ KIỂM TRA ---
        private string _checkStatus;
        public string CheckStatus // "Thỏa mãn" hoặc "Thiếu thép"
        {
            get => _checkStatus;
            set { _checkStatus = value; OnPropertyChanged(); }
        }

        private Brush _statusColor;
        public Brush StatusColor // Màu chữ (Xanh/Đỏ)
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(); }
        }

        // Hàm tính toán lại As chọn và kiểm tra
        private void Recalculate()
        {
            if (DesignColumn == null) return;

            // 1. Tính diện tích thép chọn thực tế
            double as1 = (Math.PI * Math.Pow(DesignColumn.BarDiameter, 2)) / 4.0;
            DesignColumn.As_Provided = DesignColumn.BarQuantity * as1;

            // 2. Tính hàm lượng cốt thép
            DesignColumn.Mu_Percentage = (DesignColumn.As_Provided / (DesignColumn.B * DesignColumn.H)) * 100.0;

            // 3. Kiểm tra điều kiện chịu lực
            if (DesignColumn.As_Provided >= DesignColumn.As_Required)
            {
                CheckStatus = "✅ ĐÃ THỎA MÃN KHẢ NĂNG CHỊU LỰC";
                StatusColor = Brushes.Green;
                DesignColumn.Status = "Đạt";
            }
            else
            {
                double thieu = DesignColumn.As_Required - DesignColumn.As_Provided;
                CheckStatus = $"❌ KHÔNG ĐẠT (Thiếu {Math.Round(thieu, 0)} mm²)";
                StatusColor = Brushes.Red;
                DesignColumn.Status = "Không đạt";
            }

            // Cập nhật lại các thông số hiển thị
            OnPropertyChanged(nameof(DesignColumn));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
