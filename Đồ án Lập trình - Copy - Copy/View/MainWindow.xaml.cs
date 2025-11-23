using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TinhtoancotBTCT
{ 
    public partial class MainWindow : Window
    {
        public ObservableCollection<ColumnResult> Results { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Results = new ObservableCollection<ColumnResult>();
            ResultGrid.ItemsSource = Results;
            DrawSection();
        }

        // Vẽ tiết diện cột
        private void DrawSection()
        {
            CanvasSection.Children.Clear();
            double W = CanvasSection.Width;
            double H = CanvasSection.Height;

            // Lấy số liệu
            if (!int.TryParse(TxtNumBars.Text, out int numBars)) numBars = 4;
            if (!double.TryParse(TxtBarDia.Text, out double barDia)) barDia = 16;

            // Vẽ khung cột
            Rectangle rect = new Rectangle
            {
                Width = 120,
                Height = 160,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(rect, (W - rect.Width) / 2);
            Canvas.SetTop(rect, (H - rect.Height) / 2);
            CanvasSection.Children.Add(rect);

            double left = (W - rect.Width) / 2;
            double top = (H - rect.Height) / 2;

            // Vẽ thép dọc
            double barSize = 10;
            int barsPerSide = Math.Max(1, numBars / 4);

            // 4 góc
            Ellipse corner1 = MakeBar(barSize, Brushes.Red);
            Canvas.SetLeft(corner1, left - barSize / 2);
            Canvas.SetTop(corner1, top - barSize / 2);
            CanvasSection.Children.Add(corner1);

            Ellipse corner2 = MakeBar(barSize, Brushes.Red);
            Canvas.SetLeft(corner2, left + rect.Width - barSize / 2);
            Canvas.SetTop(corner2, top - barSize / 2);
            CanvasSection.Children.Add(corner2);

            Ellipse corner3 = MakeBar(barSize, Brushes.Red);
            Canvas.SetLeft(corner3, left - barSize / 2);
            Canvas.SetTop(corner3, top + rect.Height - barSize / 2);
            CanvasSection.Children.Add(corner3);

            Ellipse corner4 = MakeBar(barSize, Brushes.Red);
            Canvas.SetLeft(corner4, left + rect.Width - barSize / 2);
            Canvas.SetTop(corner4, top + rect.Height - barSize / 2);
            CanvasSection.Children.Add(corner4);

            // Phân bố các thanh còn lại trên cạnh
            int remaining = numBars - 4;
            if (remaining > 0)
            {
                int perEdge = remaining / 4;
                for (int i = 0; i < perEdge; i++)
                {
                    double offset = (i + 1) * rect.Width / (perEdge + 1);
                    // cạnh trên
                    CanvasSection.Children.Add(MakeBarAt(left + offset - barSize / 2, top - barSize / 2, barSize));
                    // cạnh dưới
                    CanvasSection.Children.Add(MakeBarAt(left + offset - barSize / 2, top + rect.Height - barSize / 2, barSize));
                }
                for (int i = 0; i < perEdge; i++)
                {
                    double offset = (i + 1) * rect.Height / (perEdge + 1);
                    // cạnh trái
                    CanvasSection.Children.Add(MakeBarAt(left - barSize / 2, top + offset - barSize / 2, barSize));
                    // cạnh phải
                    CanvasSection.Children.Add(MakeBarAt(left + rect.Width - barSize / 2, top + offset - barSize / 2, barSize));
                }
            }
        }

        private Ellipse MakeBar(double size, Brush color)
        {
            return new Ellipse { Width = size, Height = size, Fill = color };
        }

        private Ellipse MakeBarAt(double x, double y, double size)
        {
            Ellipse e = MakeBar(size, Brushes.Red);
            Canvas.SetLeft(e, x);
            Canvas.SetTop(e, y);
            return e;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtNumBars.Text, out int numBars)) numBars = 4;
            if (!double.TryParse(TxtBarDia.Text, out double barDia)) barDia = 16;

            double As = numBars * Math.PI * Math.Pow(barDia, 2) / 4;

            Results.Add(new ColumnResult
            {
                TT = Results.Count + 1,
                B = TxtB.Text,
                H = TxtH.Text,
                Concrete = (CmbConcrete.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Steel = (CmbSteel.SelectedItem as ComboBoxItem)?.Content.ToString(),
                NumBars = numBars,
                BarDia = barDia,
                As = As.ToString("0.##")
            });

            DrawSection();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ResultGrid.SelectedItem is ColumnResult selected)
                Results.Remove(selected);
        }

        private void BtnCompute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ResultGrid.SelectedItem is ColumnResult selected)
                {
                    // Lấy dữ liệu
                    double b = double.Parse(selected.B);
                    double h = double.Parse(selected.H);
                    double As = double.Parse(selected.As);
                    double a_s = 30; // lớp bảo vệ giả định 30mm

                    // Chuyển chọn mác bê tông sang Rb (MPa)
                    double Rb = 14; // B20 mặc định
                    if (selected.Concrete.Contains("B20")) Rb = 11.5;
                    else if (selected.Concrete.Contains("B25")) Rb = 14.5;
                    else if (selected.Concrete.Contains("B30")) Rb = 17;

                    // Chuyển chọn thép sang Rs (MPa)
                    double Rs = 280;
                    if (selected.Steel.Contains("CB300")) Rs = 280;
                    else if (selected.Steel.Contains("CB400")) Rs = 360;
                    else if (selected.Steel.Contains("CB500")) Rs = 435;

                    // Diện tích bê tông chịu nén
                    double Ac = b * h - As;

                    // 1. Nén đúng tâm
                    double N_rd = Rb * Ac + Rs * As;

                    // 2. Nén lệch tâm
                    double h0 = h - a_s;
                    double x = (Rs * As) / (Rb * b);
                    if (x > h0) x = h0;

                    double N_lech = Rb * b * x + Rs * As;
                    double M_lech = Rb * b * x * (h0 - 0.5 * x) + Rs * As * (h0 - a_s);

                    // Xuất kết quả
                    string msg = $"--- KẾT QUẢ ---\n" +
                                 $"Sức chịu tải Nén đúng tâm: Nrd = {N_rd / 1000:F2} kN\n" +
                                 $"Sức chịu tải Nén lệch tâm: N = {N_lech / 1000:F2} kN ; M = {M_lech / 1e6:F2} kNm";

                    MessageBox.Show(msg, "Kết quả tính toán", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Hãy chọn một hàng trong bảng để tính toán!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính toán: " + ex.Message);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class ColumnResult
    {
        public int TT { get; set; }
        public double B { get; set; }
        public double H { get; set; }
        public string Concrete { get; set; }
        public string Steel { get; set; }
        public int NumBars { get; set; }

        // Đặt tên rõ ràng thay vì A
        public double HamLuong { get; set; }

        public int SoNhanhDai { get; set; }
        public double DuongKinhDai { get; set; }
        public string KieuBoTriThep { get; set; }
        public string As { get; set; }
    }
}
