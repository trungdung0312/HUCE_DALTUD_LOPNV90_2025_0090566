using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566
{
    public partial class FinalResultWindow : Window
    {
        public FinalResultWindow(bool ketQuaDat = true)
        {
            InitializeComponent();

            // Gán kết quả demo
            txtKQ1.Text = "As yêu cầu = 1200 mm² (demo)";
            txtKQ2.Text = "As bố trí = 1256 mm² (demo)";
            txtKQ3.Text = ketQuaDat ? "Kiểm tra điều kiện: ĐẠT (demo)"
                                    : "Kiểm tra điều kiện: KHÔNG ĐẠT (demo)";


            // Nếu không đạt yêu cầu → cảnh báo
            if (!ketQuaDat)
            {
                MessageBox.Show("Cốt thép không đạt yêu cầu! Vui lòng quay lại và chỉnh sửa dữ liệu.",
                                "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnBackInput_Click(object sender, RoutedEventArgs e)
        {
            ColumnInputWindow inputWindow = new ColumnInputWindow();
            inputWindow.Show();
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Redraw_Click(object sender, RoutedEventArgs e)
        {
            DrawSection();
        }

        private void DrawSection()
        {
            DrawArea.Children.Clear();

            double X = double.Parse(txtWidth.Text);
            double Y = double.Parse(txtHeight.Text);
            int bars = int.Parse(txtBars.Text);
            double dia = double.Parse(txtBarDia.Text);

            double scale = 1.0;
            double cover = 25;   // lớp bảo vệ mm → 25 mm

            double w = X * scale;
            double h = Y * scale;

            double left = 50;
            double top = 50;

            // 1) Mép ngoài cột
            Rectangle outer = new Rectangle
            {
                Width = w,
                Height = h,
                Stroke = Brushes.Black,
                StrokeThickness = 3
            };
            Canvas.SetLeft(outer, left);
            Canvas.SetTop(outer, top);
            DrawArea.Children.Add(outer);

            // 2) Lớp bê tông bảo vệ
            Rectangle concreteCover = new Rectangle
            {
                Width = w - 2 * cover,
                Height = h - 2 * cover,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 4, 2 } // gạch gạch cho dễ nhìn
            };
            Canvas.SetLeft(concreteCover, left + cover);
            Canvas.SetTop(concreteCover, top + cover);
            DrawArea.Children.Add(concreteCover);

            // 3) Khung cốt thép (inner frame)
            Rectangle inner = new Rectangle
            {
                Width = w - 2 * cover,
                Height = h - 2 * cover,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.SetLeft(inner, left + cover);
            Canvas.SetTop(inner, top + cover);
            DrawArea.Children.Add(inner);

            // 4) Thép
            DrawBars(left, top, w, h, bars, dia);
        }


        private void DrawBars(double left, double top, double w, double h, int barsPerSide, double dia)
        {
            double cover = 25;

            // toạ độ 4 góc
            double x1 = left + cover;
            double x2 = left + w - cover;
            double y1 = top + cover;
            double y2 = top + h - cover;

            double barRadius = dia / 2;

            // hàng trên
            for (int i = 0; i < barsPerSide; i++)
            {
                double x = x1 + i * (x2 - x1) / (barsPerSide - 1);
                DrawBar(x, y1, barRadius);
            }

            // hàng dưới
            for (int i = 0; i < barsPerSide; i++)
            {
                double x = x1 + i * (x2 - x1) / (barsPerSide - 1);
                DrawBar(x, y2, barRadius);
            }

            // hàng trái + phải (không vẽ lại góc)
            for (int i = 1; i < barsPerSide - 1; i++)
            {
                double y = y1 + i * (y2 - y1) / (barsPerSide - 1);
                DrawBar(x1, y, barRadius);
                DrawBar(x2, y, barRadius);
            }
        }

        private void DrawBar(double x, double y, double r)
        {
            Ellipse e = new Ellipse
            {
                Width = r * 2,
                Height = r * 2,
                Fill = Brushes.Black
            };
            Canvas.SetLeft(e, x - r);
            Canvas.SetTop(e, y - r);
            DrawArea.Children.Add(e);
        }

    }
}
