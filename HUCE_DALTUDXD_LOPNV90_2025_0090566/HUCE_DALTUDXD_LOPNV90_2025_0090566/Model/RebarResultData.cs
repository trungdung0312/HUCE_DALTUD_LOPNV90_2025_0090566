using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Model
{
    // Kế thừa ColumnInputData để giữ lại toàn bộ thông tin đầu vào (b, h, N, M...)
    // Không cần khai báo lại N, M, Rb, Rs... vì đã có sẵn từ cha.
    public class RebarResultData : ColumnInputData
    {
        // --- 1. KẾT QUẢ TRUNG GIAN ---
        public double L0 { get; set; }           // Chiều dài tính toán (mm)
        public double Lambda { get; set; }       // Độ mảnh lớn nhất (λ)
        public double Eta { get; set; }          // Hệ số uốn dọc (η)
        public double E0 { get; set; }           // Độ lệch tâm tính toán e0 (mm)

        // --- 2. DIỆN TÍCH THÉP YÊU CẦU ---
        public double As_OneSide { get; set; }
        public double As_Required { get; set; }  // As tính toán ra (mm2)

        // --- 3. KẾT QUẢ CHỌN THÉP CHỦ (DỌC) ---
        public int BarDiameter { get; set; }     // Đường kính (phi)
        public int BarQuantity { get; set; }     // Số lượng thanh
        public double As_Provided { get; set; }  // Diện tích thực tế chọn (mm2)
        public double Mu_Percentage { get; set; } // Hàm lượng cốt thép (%)

        // --- [MỚI] 4. KẾT QUẢ CỐT ĐAI (STIRRUPS) ---
        // Biến này để hiển thị thông tin đai tính được (VD: "Φ6a150")
        public string StirrupInfo { get; set; }

        // --- 5. TRẠNG THÁI & GHI CHÚ ---
        public string Status { get; set; }       // "Đạt", "Không đạt", "Mất ổn định"
        public string Note { get; set; }         // Ghi chú chi tiết (VD: "Lệch tâm bé")

        // --- 6. HELPER HIỂN THỊ ---
        // Chuỗi hiển thị kết quả thép dọc rút gọn (VD: 4Φ20)
        public string ResultString => (BarQuantity > 0) ? $"{BarQuantity}Φ{BarDiameter}" : "---";
    }
}
