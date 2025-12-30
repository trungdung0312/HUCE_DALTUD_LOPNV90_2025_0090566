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
        private const double EPSILON = 0.001;
        private const int MAX_ITERATIONS = 50;

        public static RebarResultData CalculateColumn(ColumnInputData input)
        {
            var result = new RebarResultData();
            // Map dữ liệu
            result.ColumnName = input.ColumnName;
            result.ConcreteGrade = input.ConcreteGrade; result.Rb = input.Rb;
            result.SteelGrade = input.SteelGrade; result.Rs = input.Rs;
            result.B = input.B; result.H = input.H; result.L = input.L;
            result.N = input.N; result.Mx = input.Mx; result.My = input.My;

            try
            {
                // BƯỚC 1: THÔNG SỐ CƠ BẢN
                // Tra bảng Eb (B20 = 27000)
                double Eb = (input.ConcreteGrade == "B20") ? 27000 : 30000;
                if (input.ConcreteGrade == "B15") Eb = 23000;

                double a = input.ConcreteCover;
                double h0 = input.H - a;
                double Za = h0 - a; // Khoảng cách 2 trọng tâm cốt thép

                double M_calc = Math.Abs(input.Mx);
                double e1 = (input.N > 0) ? (M_calc * 1000.0) / input.N : 0;
                double ea = Math.Max(input.L / 600.0, Math.Max(input.H / 30.0, 10.0));

                double e0 = (input.StructureType == StructureType.StaticallyIndeterminate)
                            ? Math.Max(e1, ea) : e1 + ea;
                result.E0 = e0;

                double l0 = input.L * input.Psi;
                result.L0 = l0;
                double lambda = l0 / input.H;
                result.Lambda = lambda;

                // Xi_R
                double omega = 0.8 - 0.008 * input.Rb;
                if (omega < 0.5) omega = 0.5;
                double xi_R = omega / (1 + (input.Rs / 500) * (1 - omega / 1.1));

                // BƯỚC 2: TÍNH TOÁN LẶP
                double As_OneSide_Calc = 0;
                double mu_assumed = 0.005; // Giả thiết 0.5%
                double eta = 1.0;
                int iteration = 0;

                do
                {
                    iteration++;

                    // 2.1 Tính Eta (uốn dọc)
                    if (lambda <= 8) eta = 1.0;
                    else
                    {
                        double As_est = mu_assumed * input.B * input.H;
                        double Is = As_est * Math.Pow(0.5 * input.H - a, 2);
                        double Ib = (input.B * Math.Pow(input.H, 3)) / 12.0;

                        // Phi_L: Nếu không nhập tải dài hạn thì mặc định lấy = 1.1 cho an toàn (như sách)
                        // Hoặc tính đúng nếu có input
                        double phi_L = 1.1;
                        double moment_total = M_calc * 1000 + input.N * 1000 * (0.5 * input.H);
                        if (input.Mx_LongTerm != 0 || input.N_LongTerm != 0)
                        {
                            double moment_long = Math.Abs(input.Mx_LongTerm) * 1000 + input.N_LongTerm * 1000 * (0.5 * input.H);
                            phi_L = 1 + moment_long / moment_total;
                        }
                        if (phi_L > 2.0) phi_L = 2.0;

                        double delta_e = Math.Max(e0 / input.H, 0.15);
                        double kb = 0.15 / (phi_L * (0.3 + delta_e));
                        double D = kb * Eb * Ib + 0.7 * 200000 * Is;
                        double Ncr = (Math.Pow(Math.PI, 2) * D) / Math.Pow(l0, 2) / 1000.0; // kN

                        if (input.N >= Ncr) { result.Status = "Mất ổn định"; return result; }
                        eta = 1.0 / (1.0 - input.N / Ncr);
                    }
                    result.Eta = Math.Round(eta, 3);

                    // 2.2 Tính Thép
                    double e0_prime = e0 * eta;
                    double alpha_n = (input.N * 1000.0) / (input.Rb * input.B * h0);
                    double delta_val = a / h0;

                    // Momen với trọng tâm thép chịu kéo
                    double M_a = (input.N * 1000.0) * (e0_prime + 0.5 * input.H - a);

                    if (alpha_n > xi_R)
                    {
                        // === NHÁNH 1: LỆCH TÂM BÉ ===
                        result.Note = $"Lệch tâm bé (ξ>{Math.Round(xi_R, 2)})";
                        double xi_1 = Math.Min((alpha_n + xi_R) / 2.0, 1.0);
                        double alpha_m1 = M_a / (input.Rb * input.B * h0 * h0);
                        double alpha_s1 = (alpha_m1 - xi_1 * (1 - 0.5 * xi_1)) / (1 - delta_val);
                        double xi = (alpha_n * (1 - xi_R) + 2 * alpha_s1 * xi_R) / (1 - xi_R + 2 * alpha_s1);

                        double factor = (input.Rb * input.B * h0) / input.Rs;
                        As_OneSide_Calc = factor * (alpha_m1 - xi * (1 - 0.5 * xi)) / (1 - delta_val);
                    }
                    else
                    {
                        // === NHÁNH 2 & 3: LỆCH TÂM LỚN ===

                        // [CHECK ĐIỀU KIỆN 4-19]
                        if (alpha_n <= 2 * delta_val)
                        {
                            result.Note = "LT Lớn (Đặc biệt 4-19)";

                            // --- SỬA THEO ẢNH SÁCH GIÁO TRÌNH ---
                            // Công thức: As = N * (2*eta*e0 - Za) / (2*Rs*Za)

                            // 1. Tính tử số: N * (2*e0_prime - Za)
                            // Lưu ý: e0_prime chính là (eta * e0)
                            double tu_so = (input.N * 1000.0) * (2 * e0_prime - Za);

                            // 2. Tính mẫu số: 2 * Rs * Za
                            double mau_so = 2 * input.Rs * Za;

                            As_OneSide_Calc = tu_so / mau_so;
                        }
                        else
                        {
                            result.Note = "LT Lớn (Thường 4-17)";
                            double alpha_m1 = M_a / (input.Rb * input.B * h0 * h0);
                            double factor = (input.Rb * input.B * h0) / input.Rs;
                            double bracket = alpha_m1 - alpha_n * (1 - 0.5 * alpha_n);
                            As_OneSide_Calc = factor * (bracket / (1 - delta_val));
                        }
                    }

                    // Hội tụ
                    double mu_new = (As_OneSide_Calc * 2) / (input.B * input.H);
                    if (mu_new < 0) mu_new = 0;
                    if (Math.Abs(mu_new - mu_assumed) < EPSILON) break;
                    mu_assumed = (mu_assumed + mu_new) / 2.0;

                } while (iteration < MAX_ITERATIONS);

                // BƯỚC 3: OUTPUT
                // 1. Kiểm tra hàm lượng tối thiểu cho 1 BÊN
                // Mu_min toàn bộ = 0.1% => Mu_min 1 bên = 0.05%
                double As_min_OneSide = 0.0005 * input.B * input.H;

                // 2. Lưu kết quả 1 bên (Lấy Max với cấu tạo) 
                result.As_OneSide = Math.Max(As_OneSide_Calc, As_min_OneSide);

                // 3. Tính tổng diện tích yêu cầu (2 bên) 
                result.As_Required = result.As_OneSide * 2;

                // 4. Chọn thép (Dựa trên TỔNG AS)
                SelectBestRebar(result, input.B, input.H, result.As_Required);

                // 5. Tính đai
                CalculateStirrups(result, input);

                if (string.IsNullOrEmpty(result.Status)) result.Status = "Đạt";
            }
            catch (Exception ex) { result.Status = "Lỗi"; result.Note = ex.Message; }

            return result;
        }

        private static void SelectBestRebar(RebarResultData result, double b, double h, double As_OneSide)
        {
            double As_Total_Req = As_OneSide * 2;
            int[] diameters = { 16, 18, 20, 22, 25, 28 };
            double minDiff = double.MaxValue;

            foreach (int d in diameters)
            {
                double area1 = (Math.PI * d * d) / 4.0;
                int n_side = (int)Math.Ceiling(As_OneSide / area1);
                if (n_side < 2) n_side = 2; // Tối thiểu 2 thanh 1 bên

                int n_total = n_side * 2;
                double As_prov = n_total * area1;
                double diff = As_prov - As_Total_Req;
                if (diff >= 0 && diff < minDiff)
                {
                    minDiff = diff;
                    result.BarDiameter = d;
                    result.BarQuantity = n_total;
                    result.As_Provided = As_prov;
                }
            }
            result.Mu_Percentage = (result.As_Provided / (b * h)) * 100.0;
            if (result.Mu_Percentage > 4.0) result.Note += " | μ>4%";
        }

        private static void CalculateStirrups(RebarResultData result, ColumnInputData input)
        {
            double s_max = Math.Min(15 * result.BarDiameter, 500);
            s_max = Math.Min(s_max, input.B);
            int s = (int)(s_max / 10) * 10;
            result.StirrupInfo = $"Φ{input.StirrupDiameter}a{s}";
        }
    }
}
