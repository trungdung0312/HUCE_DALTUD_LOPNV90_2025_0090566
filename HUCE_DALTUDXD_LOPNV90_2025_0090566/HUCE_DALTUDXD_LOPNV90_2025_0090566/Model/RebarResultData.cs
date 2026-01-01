using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Model
{
    public class RebarResultData : ColumnInputData
    {
        // --- CÁC THUỘC TÍNH TÍNH TOÁN (OUTPUT) ---
        public double L0 { get; set; }
        public double Lambda { get; set; }
        public double Eta { get; set; }
        public double E0 { get; set; }
        public double As_Required { get; set; }
        public double As_OneSide { get; set; }

        // --- INPUT CHỌN THÉP ---
        private int _q1_x; public int Quantity1_X { get => _q1_x; set { _q1_x = value; NotifyChange(); } }
        private int _d1_x; public int Diameter1_X { get => _d1_x; set { _d1_x = value; NotifyChange(); } }
        private int _q2_x; public int Quantity2_X { get => _q2_x; set { _q2_x = value; NotifyChange(); } }
        private int _d2_x; public int Diameter2_X { get => _d2_x; set { _d2_x = value; NotifyChange(); } }

        private int _q1_y; public int Quantity1_Y { get => _q1_y; set { _q1_y = value; NotifyChange(); } }
        private int _d1_y; public int Diameter1_Y { get => _d1_y; set { _d1_y = value; NotifyChange(); } }
        private int _q2_y; public int Quantity2_Y { get => _q2_y; set { _q2_y = value; NotifyChange(); } }
        private int _d2_y; public int Diameter2_Y { get => _d2_y; set { _d2_y = value; NotifyChange(); } }

        // --- KẾT QUẢ ĐÃ CHỌN ---
        private double _asProvided;
        public double As_Provided { get => _asProvided; set { _asProvided = value; OnPropertyChanged(); } }

        private double _muPercentage;
        public double Mu_Percentage { get => _muPercentage; set { _muPercentage = value; OnPropertyChanged(); } }

        private string _stirrupInfo; public string StirrupInfo { get => _stirrupInfo; set { _stirrupInfo = value; OnPropertyChanged(); } }
        private string _status; public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }
        private string _note; public string Note { get => _note; set { _note = value; OnPropertyChanged(); } }

        public int MaxDiameter => Math.Max(Math.Max(Diameter1_X, Diameter2_X), Math.Max(Diameter1_Y, Diameter2_Y));

        private void NotifyChange()
        {
            OnPropertyChanged(nameof(ResultString));
            OnPropertyChanged(nameof(SteelSummary)); // Cập nhật khi đổi thép
            OnPropertyChanged(nameof(MaxDiameter));
        }

        // --- LOGIC HIỂN THỊ ---
        public string ResultString
        {
            get
            {
                var summary = new Dictionary<int, int>();
                void Add(int dia, int qty)
                {
                    if (dia <= 0 || qty <= 0) return;
                    if (summary.ContainsKey(dia)) summary[dia] += qty; else summary.Add(dia, qty);
                }

                // 1. Bốn thanh góc (Luôn lấy D1 của X làm chuẩn)
                if (Quantity1_X >= 2 && Quantity1_Y >= 2) Add(Diameter1_X, 4);

                // 2. Bụng phương X (Trừ 2 góc)
                if (Quantity1_X > 2) Add(Diameter1_X, Quantity1_X - 2);
                Add(Diameter2_X, Quantity2_X);

                // 3. Bụng phương Y (Trừ 2 góc)
                if (Quantity1_Y > 2) Add(Diameter1_Y, Quantity1_Y - 2);
                Add(Diameter2_Y, Quantity2_Y);

                if (summary.Count == 0) return "Chưa chọn";
                return string.Join(" + ", summary.OrderByDescending(x => x.Key).Select(x => $"{x.Value}Φ{x.Key}"));
            }
        }

        // --- THÊM THUỘC TÍNH NÀY ĐỂ SỬA LỖI CS1061 VÀ DÙNG CHO BÁO CÁO ---
        // Thuộc tính này tách biệt X và Y (VD: "X: 4Φ20 | Y: 2Φ18") để báo cáo chi tiết hơn
        public string SteelSummary
        {
            get
            {
                // Tổng hợp chuỗi thép Phương X
                string sX = (Quantity2_X > 0)
                    ? $"{Quantity1_X}Φ{Diameter1_X} + {Quantity2_X}Φ{Diameter2_X}"
                    : $"{Quantity1_X}Φ{Diameter1_X}";

                // Tổng hợp chuỗi thép Phương Y
                string sY = "";
                // Chỉ hiện phương Y nếu có số lượng > 0 (và khác 0Φ0)
                if (Quantity1_Y > 0 && Diameter1_Y > 0)
                {
                    sY = (Quantity2_Y > 0)
                       ? $"{Quantity1_Y}Φ{Diameter1_Y} + {Quantity2_Y}Φ{Diameter2_Y}"
                       : $"{Quantity1_Y}Φ{Diameter1_Y}";
                }

                // Nếu không có thép Y hoặc thép Y = 0 (trường hợp thép cấu tạo chưa tính) -> Chỉ hiện X
                if (string.IsNullOrEmpty(sY) || sY == "0Φ0") return $"Total: {ResultString}"; // Fallback về ResultString nếu cần

                // Trả về định dạng chuẩn cho Báo cáo
                return $"X: {sX} | Y: {sY}";
            }
        }
    }
}
