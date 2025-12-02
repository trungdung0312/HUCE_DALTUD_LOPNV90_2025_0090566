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

            // 1. Tạo phần đầu HTML (Header & CSS cho bảng đẹp)
            sb.AppendLine("<html><head>");
            sb.AppendLine("<meta charset='utf-8'>"); // Hỗ trợ tiếng Việt không lỗi font
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: 'Times New Roman', serif; margin: 20px; }");
            sb.AppendLine("h1, h2, h3 { text-align: center; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            sb.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: center; font-size: 14px; }");
            sb.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
            sb.AppendLine(".fail { color: red; font-weight: bold; }");
            sb.AppendLine(".pass { color: green; font-weight: bold; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head><body>");

            // 2. Tiêu đề báo cáo
            sb.AppendLine("<h1>THUYẾT MINH TÍNH TOÁN CỘT BÊ TÔNG CỐT THÉP</h1>");
            sb.AppendLine("<h3>ĐỒ ÁN MÔN HỌC / TỐT NGHIỆP</h3>");
            sb.AppendLine($"<p style='text-align:center'><i>Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}</i></p>");
            sb.AppendLine("<hr/>");

            // 3. Tạo bảng dữ liệu
            sb.AppendLine("<table>");
            sb.AppendLine("<thead><tr>");
            sb.AppendLine("<th>STT</th> <th>Tên Cột</th> <th>Tiết diện (mm)</th> <th>Vật liệu</th> <th>Nội lực N (kN)</th> <th>As Yêu cầu (mm2)</th> <th>Chọn Thép</th> <th>Hàm lượng %</th> <th>Trạng thái</th>");
            sb.AppendLine("</tr></thead><tbody>");

            // 4. Duyệt danh sách và đổ dữ liệu vào dòng
            int stt = 1;
            foreach (var item in data)
            {
                string statusClass = (item.Status == "Không đạt") ? "class='fail'" : "class='pass'";

                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{stt++}</td>");
                sb.AppendLine($"<td><b>{item.ColumnName}</b></td>");
                sb.AppendLine($"<td>{item.B}x{item.H}</td>");
                sb.AppendLine($"<td>{item.ConcreteGrade}<br/>{item.SteelGrade}</td>");
                sb.AppendLine($"<td>{item.N}</td>");
                sb.AppendLine($"<td>{Math.Round(item.As_Required, 0)}</td>");
                sb.AppendLine($"<td><b>{item.ResultString}</b><br/>(As={item.As_Provided})</td>");
                sb.AppendLine($"<td>{Math.Round(item.Mu_Percentage, 2)}%</td>");
                sb.AppendLine($"<td {statusClass}>{item.Status}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table>");

            // Chân trang
            sb.AppendLine("<br/><br/><div style='float:right; text-align:center; margin-right:50px;'>");
            sb.AppendLine("<p><b>Người lập bảng</b></p>");
            sb.AppendLine("<br/><br/><br/>");
            sb.AppendLine("<p>Sinh viên thực hiện</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("</body></html>");

            // 5. Ghi nội dung ra file
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
