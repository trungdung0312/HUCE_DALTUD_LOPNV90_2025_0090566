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
                // BƯỚC 1: THÔNG SỐ CƠ BẢN

                result.Lambda = (ix > 0) ? result.L0 / ix : 0; // Độ mảnh

                // --- B. TÍNH HỆ SỐ UỐN DỌC ETA (η) ---
                // Giả định Eb = 30000 MPa (Trung bình) để tính độ cứng D đơn giản
                double Eb = 30000;
                double D = 0.7 * Eb * Ix;
                double ea = Math.Max(input.L / 600.0, Math.Max(input.H / 30.0, 10.0));

                // Lực tới hạn Euler (N_cr) - Đổi đơn vị lực ra kN (chia 1000)
                double Ncr = (Math.Pow(Math.PI, 2) * D) / Math.Pow(result.L0, 2) / 1000.0;

                {
                    result.Status = "Mất ổn định";
                    result.Note = "Lực dọc quá lớn (> Ncr)";
                    return result;
                }

                    // --- 2.1 Tính Eta ---

                result.E0 = Math.Max(e1, ea);

                double h0 = input.H - input.ConcreteCover;
                double As_calc = (term1 - term2) / (0.8 * input.Rs * za);

            return result;


                if (result.Status == null) result.Status = "Đạt";
            }
            catch (Exception ex)
            {
                result.Status = "Lỗi";
                result.Note = ex.Message;
            }

            return result;
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
