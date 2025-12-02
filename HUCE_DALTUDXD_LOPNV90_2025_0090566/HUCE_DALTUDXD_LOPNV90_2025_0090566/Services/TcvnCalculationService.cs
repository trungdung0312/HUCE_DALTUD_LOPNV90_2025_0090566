using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Services
{
    public static class TcvnCalculationService
    {
        public static RebarResultData CalculateColumn(ColumnInputData input)
        {
            var result = new RebarResultData();

            // 1. Sao chép dữ liệu đầu vào sang kết quả
            result.ColumnName = input.ColumnName;
            result.ConcreteGrade = input.ConcreteGrade; result.Rb = input.Rb;
            result.SteelGrade = input.SteelGrade; result.Rs = input.Rs;
            result.B = input.B; result.H = input.H; result.L = input.L;
            result.N = input.N; result.Mx = input.Mx; result.My = input.My;

            try
            {
                // --- A. TÍNH TOÁN HÌNH HỌC ---
                result.L0 = input.L * input.Psi; // Chiều dài tính toán

                // Tính toán theo phương cạnh H (chịu uốn Mx)
                double Ix = (input.B * Math.Pow(input.H, 3)) / 12.0; // Mô men quán tính
                double Area = input.B * input.H;
                double ix = Math.Sqrt(Ix / Area); // Bán kính quán tính
                result.Lambda = (ix > 0) ? result.L0 / ix : 0; // Độ mảnh

                // --- B. TÍNH HỆ SỐ UỐN DỌC ETA (η) ---
                // Giả định Eb = 30000 MPa (Trung bình) để tính độ cứng D đơn giản
                double Eb = 30000;
                double D = 0.7 * Eb * Ix;

                // Lực tới hạn Euler (N_cr) - Đổi đơn vị lực ra kN (chia 1000)
                double Ncr = (Math.Pow(Math.PI, 2) * D) / Math.Pow(result.L0, 2) / 1000.0;

                if (input.N >= Ncr)
                {
                    result.Status = "Mất ổn định";
                    result.Note = "Lực dọc quá lớn (> Ncr)";
                    return result;
                }

                result.Eta = 1.0 / (1.0 - (input.N / Ncr));
                if (result.Eta < 1.0) result.Eta = 1.0;

                // --- C. TÍNH ĐỘ LỆCH TÂM E0 ---
                // Quy đổi lệch tâm xiên gần đúng: M_td = Mx + 0.5 * My * (h/b)
                double M_qui_doi = Math.Abs(input.Mx * result.Eta) + 0.5 * Math.Abs(input.My * result.Eta) * (input.H / input.B);

                double e1 = (input.N > 0) ? (M_qui_doi * 1000) / input.N : 0; // e = M/N (mm)
                double ea = Math.Max(input.L / 600.0, Math.Max(input.H / 30.0, 10.0)); // Ngẫu nhiên

                result.E0 = Math.Max(e1, ea);

                // --- D. TÍNH DIỆN TÍCH THÉP As ---
                // Công thức thực nghiệm sơ bộ (để chọn thép nhanh):
                // As = [N*e - Rb*b*h*0.4] / (0.8 * Rs * (h-a))
                double h0 = input.H - input.ConcreteCover;
                double za = h0 - input.ConcreteCover;

                // Tính ước lượng (đây là công thức giả định để ra số liệu test)
                double term1 = (input.N * 1000 * result.E0);
                double term2 = (0.4 * input.Rb * input.B * Math.Pow(h0, 2)); // Phần bê tông chịu

                // Nếu bê tông chịu đủ thì ra âm -> Lấy theo cấu tạo
                double As_calc = (term1 - term2) / (0.8 * input.Rs * za);

                // Hàm lượng tối thiểu 0.4%
                double As_min = 0.004 * input.B * input.H;
                result.As_Required = Math.Max(As_calc, As_min);

                // --- E. TỰ ĐỘNG CHỌN THÉP ---
                SelectBestRebar(result, input.B, input.H);

                if (result.Status == null) result.Status = "Đạt";
            }
            catch (Exception ex)
            {
                result.Status = "Lỗi";
                result.Note = ex.Message;
            }

            return result;
        }

        private static void SelectBestRebar(RebarResultData result, double b, double h)
        {
            int[] diameters = { 16, 18, 20, 22, 25 }; // Các loại đường kính
            double minDiff = double.MaxValue;

            foreach (int d in diameters)
            {
                double as1 = (Math.PI * d * d) / 4.0;
                // Tính số thanh: As_req / as1 -> Làm tròn lên
                int n = (int)Math.Ceiling(result.As_Required / as1);

                if (n % 2 != 0) n++; // Phải chẵn
                if (n < 4) n = 4;    // Tối thiểu 4 thanh

                double as_total = n * as1;
                double diff = as_total - result.As_Required;

                // Chọn phương án dư ít nhất
                if (diff >= 0 && diff < minDiff)
                {
                    minDiff = diff;
                    result.BarDiameter = d;
                    result.BarQuantity = n;
                    result.As_Provided = as_total;
                }
            }

            // Tính hàm lượng Mu (%)
            result.Mu_Percentage = (result.As_Provided / (b * h)) * 100.0;
            if (result.Mu_Percentage > 4.0) result.Note = "Hàm lượng thép lớn > 4%";
        }
    }
}
