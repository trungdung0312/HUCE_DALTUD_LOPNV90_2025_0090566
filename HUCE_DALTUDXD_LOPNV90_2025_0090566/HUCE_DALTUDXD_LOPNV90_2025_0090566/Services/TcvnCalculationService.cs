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
                // BƯỚC 1: THÔNG SỐ CƠ BẢN

                // [QUAN TRỌNG] Lấy Rb chuẩn (chưa giảm) để tính Xi_R
                double Rb_Standard = GetStandardRb(input.ConcreteGrade);

                // Lấy Eb chuẩn
                double Eb = GetEb(input.ConcreteGrade);

                // 1.2 Hình học
                double a = input.ConcreteCover;
                double h0 = input.H - a;
                double Za = h0 - a;

                // 1.3 Nội lực & e0
                double M_calc = Math.Abs(input.Mx);
                double e1 = (input.N > 0) ? (M_calc * 1000.0) / input.N : 0;
                double ea = Math.Max(input.L / 600.0, Math.Max(input.H / 30.0, 10.0));

                // Lực tới hạn Euler (N_cr) - Đổi đơn vị lực ra kN (chia 1000)
                double Ncr = (Math.Pow(Math.PI, 2) * D) / Math.Pow(result.L0, 2) / 1000.0;

                // 1.4 Độ mảnh
                double l0 = input.L * input.Psi;
                result.L0 = l0;
                double lambda = l0 / input.H;
                result.Lambda = lambda;

                // 1.5 Tính Xi_R (Dựa trên Rb CHUẨN - Giống sách trang 113)
                double omega = 0.8 - 0.008 * Rb_Standard;
                if (omega < 0.5) omega = 0.5;

                // Tính xi_R
                double xi_R = omega / (1 + (input.Rs / 500) * (1 - omega / 1.1));

                // BƯỚC 2: TÍNH TOÁN LẶP
                double As_OneSide_Calc = 0;
                double mu_total_assumed = 0.01;
                double eta = 1.0;
                int iteration = 0;

                do
                {
                    result.Status = "Mất ổn định";
                    result.Note = "Lực dọc quá lớn (> Ncr)";
                    return result;
                }

                    // --- 2.1 Tính Eta ---
                    if (lambda <= 8) eta = 1.0;
                    else
                    {
                        double As_est = mu_total_assumed * input.B * input.H;
                        double Is = As_est * Math.Pow(0.5 * input.H - a, 2);
                        double Ib = (input.B * Math.Pow(input.H, 3)) / 12.0;

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
                        double Ncr = (Math.Pow(Math.PI, 2) * D) / Math.Pow(l0, 2) / 1000.0;

                result.E0 = Math.Max(e1, ea);

                    // --- 2.2 Tính Thép (1 bên) ---
                    // [QUAN TRỌNG] Ở đây dùng input.Rb (9.775) để tính lực

                    double e0_prime = e0 * eta;
                    double alpha_n = (input.N * 1000.0) / (input.Rb * input.B * h0);
                    double delta_val = a / h0;
                    double M_a = (input.N * 1000.0) * (e0_prime + 0.5 * input.H - a);

                    if (alpha_n > xi_R)
                    {
                        // LỆCH TÂM BÉ
                        result.Note = $"Lệch tâm bé (ξ>{Math.Round(xi_R, 3)})";
                        double xi_1 = Math.Min((alpha_n + xi_R) / 2.0, 1.0);
                        double alpha_m1 = M_a / (input.Rb * input.B * h0 * h0);

                        double alpha_s1 = (alpha_m1 - xi_1 * (1 - 0.5 * xi_1)) / (1 - delta_val);
                        double xi = (alpha_n * (1 - xi_R) + 2 * alpha_s1 * xi_R) / (1 - xi_R + 2 * alpha_s1);

                        double factor = (input.Rb * input.B * h0) / input.Rs;
                        As_OneSide_Calc = factor * (alpha_m1 - xi * (1 - 0.5 * xi)) / (1 - delta_val);
                    }
                    else
                    {
                        // LỆCH TÂM LỚN
                        if (alpha_n <= 2 * delta_val)
                        {
                            result.Note = "LT Lớn (Đặc biệt 4-19)";
                            double tu_so = (input.N * 1000.0) * (2 * e0_prime - Za);
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
                    double mu_total_new = (As_OneSide_Calc * 2) / (input.B * input.H);
                    if (mu_total_new < 0) mu_total_new = 0;
                    if (Math.Abs(mu_total_new - mu_total_assumed) < EPSILON) break;
                    mu_total_assumed = (mu_total_assumed + mu_total_new) / 2.0;

                } while (iteration < MAX_ITERATIONS);

                // BƯỚC 3: OUTPUT
                double As_Total_Calc = As_OneSide_Calc * 2;
                double As_min_Total = 0.001 * input.B * input.H;
                double As_min_OneSide = As_min_Total / 2;

                // Gán hiển thị
                result.As_OneSide = Math.Max(As_OneSide_Calc, As_min_OneSide);
                result.As_Required = result.As_OneSide * 2;

                SelectBestRebar(result, input.B, input.H, result.As_Required);
                CalculateStirrups(result, input);

                if (string.IsNullOrEmpty(result.Status)) result.Status = "Đạt";
            }

            return result;
        }

        // --- CÁC HÀM TRA BẢNG CHUẨN ---

        private static double GetStandardRb(string grade)
        {
            // Trả về Rb tiêu chuẩn (Bảng 4 TCVN 5574:2018) - KHÔNG nhân hệ số điều kiện
            switch (grade)
            {
                case "B15": return 8.5;
                case "B20": return 11.5;
                case "B25": return 14.5;
                case "B30": return 17.0;
                case "B35": return 19.5;
                case "B40": return 22.0;
                default: return 11.5;
            }
        }

        private static double GetEb(string grade)
        {
            switch (grade)
            {
                case "B15": return 23000;
                case "B20": return 27000;
                case "B25": return 30000;
                case "B30": return 32500;
                case "B35": return 34500;
                default: return 30000;
            }
        }

        private static void SelectBestRebar(RebarResultData result, double b, double h, double As_Total_Req)
        {
            int[] diameters = { 16, 18, 20, 22, 25, 28, 32 };
            double minDiff = double.MaxValue;

            foreach (int d in diameters)
            {
                double area1 = (Math.PI * d * d) / 4.0;
                int n = (int)Math.Ceiling(As_Total_Req / area1);
                if (n < 4) n = 4;
                if (n % 2 != 0) n++;

                double As_prov = n * area1;
                double diff = As_prov - As_Total_Req;
                if (diff >= 0 && diff < minDiff)
                {
                    minDiff = diff;
                    result.BarDiameter = d;
                    result.BarQuantity = n;
                    result.As_Provided = As_prov;
                }
            }

        private static void CalculateStirrups(RebarResultData result, ColumnInputData input)
        {
            double minD = Math.Max(0.25 * result.BarDiameter, 6);
            double s_max = Math.Min(15 * result.BarDiameter, 500);
            s_max = Math.Min(s_max, input.B);
            int s = (int)(s_max / 10) * 10;
            result.StirrupInfo = $"Φ{input.StirrupDiameter}a{s}";
        }
    }
}
