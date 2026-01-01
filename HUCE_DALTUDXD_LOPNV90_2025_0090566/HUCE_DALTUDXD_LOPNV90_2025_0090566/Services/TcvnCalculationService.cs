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
            result.ConcreteCover = input.ConcreteCover;
            result.StirrupDiameter = input.StirrupDiameter;

            try
            {
                // =======================================================================
                // XỬ LÝ QUY ĐỔI NỘI LỰC (LỆCH TÂM XIÊN -> PHẲNG)
                // =======================================================================
                double calc_B = input.B;
                double calc_H = input.H;
                double calc_M = Math.Abs(input.Mx);
                string calc_Note = "";

                double absMx = Math.Abs(input.Mx);
                double absMy = Math.Abs(input.My);

                // Case 1: Chỉ có Mx
                if (absMx > 0.001 && absMy <= 0.001)
                {
                    calc_M = absMx;
                }
                // Case 2: Chỉ có My -> Đảo trục
                else if (absMx <= 0.001 && absMy > 0.001)
                {
                    calc_B = input.H; 
                    calc_H = input.B;
                    calc_M = absMy;
                }
                // Case 3: Xiên -> Quy đổi
                else if (absMx > 0.001 && absMy > 0.001)
                {
                    double ratioX = absMx / input.H;
                    double ratioY = absMy / input.B;
                    double k = 0.5; // Hệ số an toàn

                    if (ratioX >= ratioY) // Ưu tiên phương X
                    {
                        calc_M = Math.Round(absMx + k * (input.H / input.B) * absMy, 3);
                        calc_Note = " (Xiên->X)";
                    }
                    else // Ưu tiên phương Y -> Đảo trục
                    {
                        calc_M = Math.Round(absMy + k * (input.B / input.H) * absMx, 3);
                        calc_B = input.H; 
                        calc_H = input.B;
                        calc_Note = " (Xiên->Y)";
                    }
                }

                // =======================================================================
                // TÍNH TOÁN CƠ BẢN 
                // =======================================================================
                double Rb_Standard = GetStandardRb(input.ConcreteGrade);
                double Eb = GetEb(input.ConcreteGrade);

                double a = input.ConcreteCover;
                double h0 = calc_H - a; // Dùng calc_H
                double Za = h0 - a;
                
                // e1
                double e1 = (input.N > 0) ? (calc_M * 1000.0) / input.N : 0; 
                e1 = Math.Round(e1, 3);

                // ea (Tính theo calc_H)
                double ea = Math.Max(input.L / 600.0, Math.Max(calc_H / 30.0, 10.0));
                ea = Math.Round(ea, 3);

                // e0
                double e0 = (input.StructureType == StructureType.StaticallyIndeterminate) ? Math.Max(e1, ea) : e1 + ea;
                e0 = Math.Round(e0, 3);
                result.E0 = e0;

                // Lambda (Tính theo calc_H)
                double l0 = input.L * input.Psi;
                l0 = Math.Round(l0, 3);
                result.L0 = l0;

                double lambda = l0 / calc_H; 
                lambda = Math.Round(lambda, 3);
                result.Lambda = lambda;

                // Omega & Xi_R
                double omega = 0.8 - 0.008 * Rb_Standard;
                if (omega < 0.5) omega = 0.5;
                omega = Math.Round(omega, 3);

                double xi_R = omega / (1 + (input.Rs / 500) * (1 - omega / 1.1));
                xi_R = Math.Round(xi_R, 3);

                // 2. TÍNH TOÁN LẶP
                double As_OneSide_Calc = 0;
                double mu_total_assumed = 0.01;
                double eta = 1.0;
                int iteration = 0;

                do
                {
                    iteration++;
                    if (lambda <= 8) eta = 1.0;
                    else
                    {
                        double As_est = mu_total_assumed * calc_B * calc_H;
                        As_est = Math.Round(As_est, 3);

                        double Is = As_est * Math.Pow(0.5 * calc_H - a, 2);
                        Is = Math.Round(Is, 3);

                        double Ib = (calc_B * Math.Pow(calc_H, 3)) / 12.0;
                        Ib = Math.Round(Ib, 3);

                        double phi_L = 1.1;
                        double moment_total = calc_M * 1000 + input.N * 1000 * (0.5 * calc_H);
                        moment_total = Math.Round(moment_total, 3);

                        // Xử lý Momen dài hạn cho bài toán xiên
                        double M_long_val = 0;
                        if (calc_M == absMx) M_long_val = Math.Abs(input.Mx_LongTerm);
                        else if (calc_M == absMy) M_long_val = Math.Abs(input.My_LongTerm);
                        else M_long_val = Math.Abs(input.Mx_LongTerm) + Math.Abs(input.My_LongTerm);

                        if (M_long_val != 0 || input.N_LongTerm != 0)
                        {
                            double moment_long = M_long_val * 1000 + input.N_LongTerm * 1000 * (0.5 * calc_H);
                            moment_long = Math.Round(moment_long, 3);
                            phi_L = 1 + moment_long / moment_total;
                            phi_L = Math.Round(phi_L, 3);
                        }
                        if (phi_L > 2.0) phi_L = 2.0;

                        double delta_e = Math.Max(e0 / calc_H, 0.15);
                        delta_e = Math.Round(delta_e, 3);

                        double kb = 0.15 / (phi_L * (0.3 + delta_e));
                        kb = Math.Round(kb, 3);

                        double D = kb * Eb * Ib + 0.7 * 200000 * Is;
                        D = Math.Round(D, 3);

                        double Ncr = (Math.Pow(Math.PI, 2) * D) / Math.Pow(l0, 2) / 1000.0;
                        Ncr = Math.Round(Ncr, 3);

                        if (input.N >= Ncr) { result.Status = "Mất ổn định"; return result; }
                        eta = 1.0 / (1.0 - input.N / Ncr);
                    }
                    eta = Math.Round(eta, 3);
                    result.Eta = eta;

                    double e0_prime = e0 * eta;
                    e0_prime = Math.Round(e0_prime, 3);

                    double alpha_n = (input.N * 1000.0) / (input.Rb * calc_B * h0);
                    alpha_n = Math.Round(alpha_n, 3);

                    double delta_val = a / h0;
                    delta_val = Math.Round(delta_val, 3);

                    double M_a = (input.N * 1000.0) * (e0_prime + 0.5 * calc_H - a);
                    M_a = Math.Round(M_a, 3);

                    if (alpha_n > xi_R)
                    {
                        result.Note = $"LT Bé (ξ>{xi_R})";
                        double xi_1 = Math.Min((alpha_n + xi_R) / 2.0, 1.0);
                        xi_1 = Math.Round(xi_1, 3);
                        double alpha_m1 = M_a / (input.Rb * calc_B * h0 * h0);
                        alpha_m1 = Math.Round(alpha_m1, 3);
                        double alpha_s1 = (alpha_m1 - xi_1 * (1 - 0.5 * xi_1)) / (1 - delta_val);
                        alpha_s1 = Math.Round(alpha_s1, 3);
                        double xi = (alpha_n * (1 - xi_R) + 2 * alpha_s1 * xi_R) / (1 - xi_R + 2 * alpha_s1);
                        xi = Math.Round(xi, 3);
                        double factor = (input.Rb * calc_B * h0) / input.Rs;
                        factor = Math.Round(factor, 3);
                        As_OneSide_Calc = factor * (alpha_m1 - xi * (1 - 0.5 * xi)) / (1 - delta_val);
                    }
                    else
                    {
                        if (alpha_n <= 2 * delta_val)
                        {
                            result.Note = "LT Lớn (ĐB)";
                            double tu_so = (input.N * 1000.0) * (2 * e0_prime - Za);
                            double mau_so = 2 * input.Rs * Za;
                            As_OneSide_Calc = tu_so / mau_so;
                        }
                        else
                        {
                            result.Note = "LT Lớn";
                            double alpha_m1 = M_a / (input.Rb * calc_B * h0 * h0);
                            alpha_m1 = Math.Round(alpha_m1, 3);
                            double factor = (input.Rb * calc_B * h0) / input.Rs;
                            factor = Math.Round(factor, 3);
                            double bracket = alpha_m1 - alpha_n * (1 - 0.5 * alpha_n);
                            bracket = Math.Round(bracket, 3);
                            As_OneSide_Calc = factor * (bracket / (1 - delta_val));
                        }
                    }
                    As_OneSide_Calc = Math.Round(As_OneSide_Calc, 3);

                    double mu_total_new = (As_OneSide_Calc * 2) / (calc_B * calc_H);
                    mu_total_new = Math.Round(mu_total_new, 3);

                    if (mu_total_new < 0) mu_total_new = 0;
                    if (Math.Abs(mu_total_new - mu_total_assumed) < EPSILON) break;

                    mu_total_assumed = (mu_total_assumed + mu_total_new) / 2.0;
                    mu_total_assumed = Math.Round(mu_total_assumed, 3);

                } while (iteration < MAX_ITERATIONS);

                result.Note += calc_Note;

                double As_min_Total = 0.001 * calc_B * calc_H; 
                As_min_Total = Math.Round(As_min_Total, 3);
                double As_min_OneSide = As_min_Total / 2;
                As_min_OneSide = Math.Round(As_min_OneSide, 3);

                result.As_OneSide = Math.Max(As_OneSide_Calc, As_min_OneSide);
                result.As_Required = result.As_OneSide * 2;
                result.As_Required = Math.Round(result.As_Required, 3);

                // --- GỌI HÀM CHỌN THÉP ---
                // Truyền calc_B, calc_H để hàm biết kích thước thực tế đang tính
                SelectBestRebarNew(result, result.As_Required, calc_B, calc_H);

                CalculateStirrups(result, input);

                if (string.IsNullOrEmpty(result.Status)) result.Status = "Đạt";
            }
            catch (Exception ex)
            {
                result.Status = "Lỗi";
                result.Note = ex.Message;
            }

            return result;
        }

        // --- HÀM CHỌN THÉP TỐI ƯU ---
        private static void SelectBestRebarNew(RebarResultData result, double As_Total_Req, double b, double h)
        {
            result.Quantity1_X = 0; result.Diameter1_X = 0; result.Quantity2_X = 0; result.Diameter2_X = 0;
            result.Quantity1_Y = 0; result.Diameter1_Y = 0; result.Quantity2_Y = 0; result.Diameter2_Y = 0;

            double minSide = Math.Min(b, h);
            List<int> diameters = new List<int> { 16, 18, 20, 22, 25, 28, 32 };

            if (minSide >= 350) diameters.Remove(16);

            double minExcess = double.MaxValue;

            int bestD1 = 0, bestD2 = 0;
            int bestQ1_X = 0, bestQ2_X = 0;
            int bestQ1_Y = 0, bestQ2_Y = 0;

            foreach (int d1 in diameters)
            {
                foreach (int d2 in diameters)
                {
                    if (d2 > d1) continue;

                    double a1 = Math.PI * d1 * d1 / 4.0; a1 = Math.Round(a1, 3);
                    double a2 = Math.PI * d2 * d2 / 4.0; a2 = Math.Round(a2, 3);

                    for (int nx = 0; nx <= 6; nx += 2)
                    {
                        for (int ny = 0; ny <= 6; ny += 2)
                        {
                            int q1_x = 2; int q1_y = 2;
                            int q2_x = 0; int q2_y = 0;

                            if (d1 == d2) { q1_x += nx; q1_y += ny; }
                            else { q2_x = nx; q2_y = ny; }

                            double currentTotalAs = (q1_x * a1 + q2_x * a2) + (q1_y * a1 + q2_y * a2);
                            currentTotalAs = Math.Round(currentTotalAs, 3);

                            double excess = currentTotalAs - As_Total_Req;
                            excess = Math.Round(excess, 3);

                            if (excess >= -0.5 && excess < minExcess)
                            {
                                // --- SỬA CHỮA LOGIC ---
                                // Đã xóa bỏ đoạn code: if (h > 1.2 * b && ...) isRatioGood = false;
                                // Lý do: Khi tính toán chịu uốn quanh trục nào (ở đây là trục đi qua H/2),
                                // ta luôn ưu tiên thép đặt xa trục trung hòa nhất (tức là thép Phương X - Top/Bottom).
                                // Vì vậy, chỉ cần kiểm tra: Thép X (chịu lực) không được ít hơn Thép Y (cấu tạo) quá nhiều.
                                
                                bool isRatioGood = true;

                                // Bổ sung điều kiện mới: Ưu tiên thép chịu lực (X) luôn >= Thép cấu tạo (Y)
                                // Vì 'b' và 'h' ở đây là kích thước tính toán (đã đảo nếu cần),
                                // nên Phương X (cạnh b) luôn là phương chịu lực chính.
                                int totalBarX = (d1 == d2) ? q1_x : (q1_x + q2_x);
                                int totalBarY = (d1 == d2) ? q1_y : (q1_y + q2_y);

                                if (totalBarX < totalBarY) isRatioGood = false;

                                if (isRatioGood)
                                {
                                    minExcess = excess;
                                    bestD1 = d1; bestD2 = d2;
                                    bestQ1_X = q1_x; bestQ2_X = q2_x;
                                    bestQ1_Y = q1_y; bestQ2_Y = q2_y;
                                }
                            }
                        }
                    }
                }
            }

            result.Diameter1_X = bestD1; result.Quantity1_X = bestQ1_X;
            result.Diameter2_X = bestD2; result.Quantity2_X = bestQ2_X;
            result.Diameter1_Y = bestD1; result.Quantity1_Y = bestQ1_Y;
            result.Diameter2_Y = bestD2; result.Quantity2_Y = bestQ2_Y;

            double a1_fin = Math.PI * Math.Pow(result.Diameter1_X, 2) / 4.0; a1_fin = Math.Round(a1_fin, 3);
            double a2x_fin = (result.Diameter2_X > 0) ? Math.PI * Math.Pow(result.Diameter2_X, 2) / 4.0 : 0; a2x_fin = Math.Round(a2x_fin, 3);
            double a2y_fin = (result.Diameter2_Y > 0) ? Math.PI * Math.Pow(result.Diameter2_Y, 2) / 4.0 : 0; a2y_fin = Math.Round(a2y_fin, 3);

            double areaX = result.Quantity1_X * a1_fin + result.Quantity2_X * a2x_fin; areaX = Math.Round(areaX, 3);
            double areaY = result.Quantity1_Y * a1_fin + result.Quantity2_Y * a2y_fin; areaY = Math.Round(areaY, 3);

            result.As_Provided = Math.Round(areaX + areaY, 3);
            result.Mu_Percentage = Math.Round((result.As_Provided / (result.B * result.H)) * 100.0, 3);

            if (result.Mu_Percentage > 4.0) result.Note += " | μ>4%";
        }

        private static void CalculateStirrups(RebarResultData result, ColumnInputData input)
        {
            double maxD = result.MaxDiameter > 0 ? result.MaxDiameter : 20;
            double s_max = Math.Min(15 * maxD, 500);
            s_max = Math.Min(s_max, input.B);
            int s = (int)(s_max / 10) * 10;
            result.StirrupInfo = $"Φ{input.StirrupDiameter}a{s}";
        }

        private static double GetStandardRb(string grade)
        {
            switch (grade) { case "B15": return 8.5; case "B20": return 11.5; case "B25": return 14.5; case "B30": return 17.0; case "B35": return 19.5; case "B40": return 22.0; default: return 11.5; }
        }
        private static double GetEb(string grade)
        {
            switch (grade) { case "B15": return 23000; case "B20": return 27000; case "B25": return 30000; case "B30": return 32500; case "B35": return 34500; default: return 30000; }
        }
    }
}
