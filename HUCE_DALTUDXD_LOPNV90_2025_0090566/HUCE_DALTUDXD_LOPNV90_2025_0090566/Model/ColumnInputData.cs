using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Model
{
    // 1. Định nghĩa loại sơ đồ kết cấu (Dùng cho công thức tính e0)
    public enum StructureType
    {
        StaticallyIndeterminate, // Siêu tĩnh
        StaticallyDeterminate    // Tĩnh định
    }

    public class ColumnInputData : IDataErrorInfo, INotifyPropertyChanged
    {
        // --- CÁC BIẾN CƠ BẢN ---

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        private string _columnName;
        public string ColumnName
        {
            get => _columnName;
            set { _columnName = value; OnPropertyChanged(); }
        }

        // --- NHÓM VẬT LIỆU BÊ TÔNG ---
        private string _concreteGrade;
        public string ConcreteGrade
        {
            get => _concreteGrade;
            set
            {
                _concreteGrade = value;
                UpdateRb(); // Tự động tính lại Rb
                OnPropertyChanged();
            }
        }

        private double _rb;
        public double Rb
        {
            get => _rb;
            set { _rb = value; OnPropertyChanged(); }
        }

        // Điều kiện thi công : Đổ cao > 1.5m
        private bool _isVerticalPouring = true;
        public bool IsVerticalPouring
        {
            get => _isVerticalPouring;
            set
            {
                _isVerticalPouring = value;
                UpdateRb(); // Tính lại Rb ngay khi check
                OnPropertyChanged();
            }
        }

        // --- NHÓM VẬT LIỆU THÉP CHỦ ---
        private string _steelGrade;
        public string SteelGrade
        {
            get => _steelGrade;
            set
            {
                _steelGrade = value;
                UpdateRs();
                OnPropertyChanged();
            }
        }

        private double _rs;
        public double Rs
        {
            get => _rs;
            set { _rs = value; OnPropertyChanged(); }
        }

        // --- NHÓM VẬT LIỆU CỐT ĐAI ---
        private string _stirrupGrade;
        public string StirrupGrade // VD: CB240-T
        {
            get => _stirrupGrade;
            set
            {
                _stirrupGrade = value;
                UpdateRsw(); // Tự động cập nhật Rsw
                OnPropertyChanged();
            }
        }

        private int _stirrupDiameter;
        public int StirrupDiameter // VD: 6, 8, 10
        {
            get => _stirrupDiameter;
            set { _stirrupDiameter = value; OnPropertyChanged(); }
        }

        private double _rsw;
        public double Rsw
        {
            get => _rsw;
            set { _rsw = value; OnPropertyChanged(); }
        }

        // --- HÌNH HỌC ---
        public double B { get; set; }
        public double H { get; set; }
        public double L { get; set; }
        public double ConcreteCover { get; set; }

        public StructureType StructureType { get; set; } = StructureType.StaticallyIndeterminate;

        // --- NỘI LỰC TÍNH TOÁN ---
        public double N { get; set; }
        public double Mx { get; set; }
        public double My { get; set; }
        public double Psi { get; set; } = 1.0;

        // --- NỘI LỰC DÀI HẠN (ĐỂ TÍNH UỐN DỌC) ---
        public double N_LongTerm { get; set; }
        public double Mx_LongTerm { get; set; }
        public double My_LongTerm { get; set; }


        // --- LOGIC CẬP NHẬT TỰ ĐỘNG (UPDATED) ---

        private void UpdateRb()
        {
            // 1. Lấy cường độ gốc (Tra bảng TCVN)
            double baseRb = 0;
            switch (ConcreteGrade)
            {
                case "B15": baseRb = 8.5; break;
                case "B20": baseRb = 11.5; break;
                case "B25": baseRb = 14.5; break;
                case "B30": baseRb = 17.0; break;
                case "B35": baseRb = 19.5; break;
                case "B40": baseRb = 22.0; break;
                default: baseRb = 0; break;
            }

            // 2. Xét hệ số gamma_b3 = 0.85 (Trang 108)
            if (IsVerticalPouring)
            {
                baseRb = baseRb * 0.85;
            }

            Rb = Math.Round(baseRb, 3);
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

        private void UpdateRsw()
        {
            switch (StirrupGrade)
            {
                case "CB240-T": Rsw = 170; break;
                case "CB300-V": Rsw = 210; break;
                case "CB400-V": Rsw = 280; break;
                default: Rsw = 170; break;
            }
        }

        // --- VALIDATION ---
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
                    // Validate thêm cho tải dài hạn
                    case nameof(N_LongTerm):
                        if (Math.Abs(N_LongTerm) > Math.Abs(N)) result = "N.dh phải <= N.tổng";
                        break;
                }
                return result;
            }
        }

        // --- BOILERPLATE MVVM ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
