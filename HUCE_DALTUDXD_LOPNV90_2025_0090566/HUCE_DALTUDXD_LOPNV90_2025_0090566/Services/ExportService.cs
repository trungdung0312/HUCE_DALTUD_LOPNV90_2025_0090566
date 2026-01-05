using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Services
{
    public static class ExportService
    {
        public static void ExportToHtml(List<RebarResultData> data, string filePath)
        {
            var sb = new StringBuilder();

            // 1. HEADER & CSS
            sb.AppendLine("<html><head>");
            sb.AppendLine("<meta charset='utf-8'>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: 'Times New Roman', serif; margin: 20px; }");
            sb.AppendLine("h1, h2, h3 { text-align: center; margin: 5px; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; font-size: 12px; }"); // Giảm font xuống 12px 
            sb.AppendLine("th, td { border: 1px solid #000; padding: 5px; text-align: center; vertical-align: middle; }");
            sb.AppendLine("th { background-color: #D6EAF8; font-weight: bold; height: 40px; }");
            sb.AppendLine(".fail { color: red; font-weight: bold; }");
            sb.AppendLine(".pass { color: green; font-weight: bold; }");
            sb.AppendLine(".note { font-style: italic; color: #555; font-size: 11px; }");
            sb.AppendLine(".orange { color: #E67E22; font-weight: bold; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head><body>");

            // 2. TIÊU ĐỀ
            sb.AppendLine("<h1>THUYẾT MINH TÍNH TOÁN CỘT BÊ TÔNG CỐT THÉP</h1>");
            sb.AppendLine("<h3>(Xuất từ phần mềm tính toán TCVN 5574:2018)</h3>");
            sb.AppendLine($"<p style='text-align:right; font-style:italic; margin-right:20px;'>Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine("<hr/>");

            // 3. BẢNG DỮ LIỆU 
            sb.AppendLine("<table>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            // Thêm cột Vật liệu
            sb.AppendLine("<th rowspan='2'>STT</th> <th rowspan='2'>Tên Cột</th> <th rowspan='2'>Tiết diện<br/>(mm)</th> <th rowspan='2'>Vật liệu<br/>(BT | Thép)</th> <th colspan='3'>Nội lực Tính toán</th> <th colspan='4'>Thông số Cột mảnh</th> <th colspan='5'>Kết quả Cốt thép</th> <th rowspan='2'>Trạng thái</th>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<th>N (kN)</th> <th>Mx (kNm)</th> <th>My (kNm)</th>");
            sb.AppendLine("<th>L0 (mm)</th> <th>λ</th> <th>η</th> <th>e0 (mm)</th>");
            // Thêm cột As 1 bên
            sb.AppendLine("<th>As(1 bên)<br/>(mm2)</th> <th>As.yc<br/>(mm2)</th> <th>Bố trí<br/>(X | Y)</th> <th>As.chọn<br/>(mm2)</th> <th>μ (%)</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead><tbody>");

            // 4. DUYỆT DATA
            int stt = 1;
            foreach (var item in data)
            {
                if (string.IsNullOrEmpty(item.Status)) continue;

                string statusClass = (item.Status == "Không đạt" || item.Status == "Mất ổn định") ? "class='fail'" : "class='pass'";
                string steelDisplay = item.SteelSummary.Replace("\n", "<br/>").Replace("|", "<br/>");

                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{stt++}</td>");
                sb.AppendLine($"<td><b>{item.ColumnName}</b></td>");
                sb.AppendLine($"<td>{item.B}x{item.H}</td>");

                // Cột Vật liệu
                sb.AppendLine($"<td>{item.ConcreteGrade}<br/>{item.SteelGrade}</td>");

                // Nội lực
                sb.AppendLine($"<td>{item.N}</td>");
                sb.AppendLine($"<td>{item.Mx}</td>");
                sb.AppendLine($"<td class='orange'>{item.My}</td>");

                // Thông số mảnh
                sb.AppendLine($"<td>{item.L0}</td>");
                sb.AppendLine($"<td>{item.Lambda}</td>");
                sb.AppendLine($"<td class='orange'>{item.Eta}</td>");
                sb.AppendLine($"<td>{item.E0}</td>");

                // Kết quả thép (Thêm As 1 bên)
                sb.AppendLine($"<td style='color:#555;'>{Math.Round(item.As_OneSide, 0)}</td>");
                sb.AppendLine($"<td style='color:red; font-weight:bold;'>{Math.Round(item.As_Required, 0)}</td>");
                sb.AppendLine($"<td style='text-align:left; padding-left:5px; font-size:11px;'>{steelDisplay}<br/><i>({item.StirrupInfo})</i></td>");
                sb.AppendLine($"<td><b>{item.As_Provided}</b></td>");
                sb.AppendLine($"<td>{Math.Round(item.Mu_Percentage, 2)}%</td>");

                // Trạng thái
                sb.AppendLine($"<td><span {statusClass}>{item.Status}</span><br/><span class='note'>{item.Note}</span></td>");

                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table>");

            // Footer
            sb.AppendLine("<br/><br/><div style='float:right; text-align:center; margin-right:50px; width:200px;'>");
            sb.AppendLine("<p><b>Người lập bảng</b></p>");
            sb.AppendLine("<br/><br/><br/>");
            sb.AppendLine("<p>(Ký, ghi rõ họ tên)</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("</body></html>");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
