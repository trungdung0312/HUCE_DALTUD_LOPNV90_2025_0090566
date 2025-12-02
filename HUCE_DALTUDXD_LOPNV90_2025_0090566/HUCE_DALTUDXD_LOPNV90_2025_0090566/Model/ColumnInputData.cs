using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Model
{
    // Thêm kế thừa INotifyPropertyChanged để khi đổi Bê tông -> Rb tự nhảy
    public class ColumnInputData : IDataErrorInfo, INotifyPropertyChanged
    {
        // 0. Checkbox chọn để tính toán
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        // 1. Định danh
        private string _columnName;
        public string ColumnName
        {
            get => _columnName;
            set { _columnName = value; OnPropertyChanged(); }
        }

        // 2. Vật liệu (Chọn ComboBox)
        private string _concreteGrade;
        public string ConcreteGrade // Ví dụ: B20, B25
        {
            get => _concreteGrade;
            set
            {
                _concreteGrade = value;
                UpdateRb(); // Tự động cập nhật Rb khi chọn cấp độ bền
                OnPropertyChanged();
            }
        }

        private string _steelGrade;
        public string SteelGrade // Ví dụ: CB300-V, CB400-V
        {
            get => _steelGrade;
            set
            {
                _steelGrade = value;
                UpdateRs(); // Tự động cập nhật Rs
                OnPropertyChanged();
            }
        }

        // Các giá trị tính toán (Read-only trên giao diện hoặc tự nhảy)
        private double _rb;
        public double Rb
        {
            get => _rb;
            set { _rb = value; OnPropertyChanged(); }
        }

        private double _rs;
        public double Rs
        {
            get => _rs;
            set { _rs = value; OnPropertyChanged(); }
        }

        // 3. Hình học & Cấu tạo
        public double B { get; set; }
        public double H { get; set; }
        public double L { get; set; }
        public double ConcreteCover { get; set; } // Lớp bảo vệ (mm)

        // 4. Nội lực
        public double N { get; set; }
        public double Mx { get; set; }
        public double My { get; set; }
        public double Psi { get; set; } = 1.0; // Mặc định là 1

        // --- Logic tự động điền Rb, Rs (Theo TCVN 5574:2018) ---
        private void UpdateRb()
        {
            switch (ConcreteGrade)
            {
                case "B15": Rb = 8.5; break;
                case "B20": Rb = 11.5; break;
                case "B25": Rb = 14.5; break;
                case "B30": Rb = 17.0; break;
                case "B35": Rb = 19.5; break;
                default: Rb = 0; break;
            }
        }

        private void UpdateRs()
        {
            switch (SteelGrade)
            {
                case "CB240-T": Rs = 210; break;
                case "CB300-V": Rs = 260; break;
                case "CB400-V": Rs = 350; break;
                case "CB500-V": Rs = 435; break;
                default: Rs = 0; break;
            }
        }

        // --- VALIDATION (Kiểm tra lỗi) ---
        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case nameof(ColumnName):
                        if (string.IsNullOrWhiteSpace(ColumnName)) result = "Cần nhập tên cột";
                        break;
                    case nameof(B):
                        if (B < 200) result = "B >= 200mm";
                        break;
                    case nameof(ConcreteCover):
                        if (ConcreteCover < 20) result = "Lớp bảo vệ >= 20mm";
                        break;
                }
                return result;
            }
        }

        // Boilerplate code cho MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
