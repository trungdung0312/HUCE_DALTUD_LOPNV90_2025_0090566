using HUCE_DALTUDXD_LOPNV90_2025_0090566.Model;
using HUCE_DALTUDXD_LOPNV90_2025_0090566.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Views.Pages
{
    public partial class RebarDesignPage : Page
    {
        private RebarDesignViewModel _viewModel;

        public RebarDesignPage()
        {
            InitializeComponent();
            _viewModel = new RebarDesignViewModel();
            this.DataContext = _viewModel;

            // Đăng ký sự kiện: Khi ViewModel thay đổi dữ liệu -> Vẽ lại hình
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        // Hàm nhận dữ liệu cột từ trang trước chuyển sang
        public void LoadColumnData(RebarResultData column)
        {
            _viewModel.DesignColumn = column;
            // Vẽ lần đầu tiên
            DrawSection();
        }

        // Khi số liệu thay đổi -> Vẽ lại
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RebarDesignViewModel.DesignColumn))
            {
                DrawSection();
            }
        }

        // --- HÀM VẼ HÌNH TRÊN CANVAS (Giống phần trước nhưng chi tiết hơn) ---
        private void DrawSection()
        {
            DetailCanvas.Children.Clear();
            var col = _viewModel.DesignColumn;
            if (col == null) return;

            // 1. Tính toán tỷ lệ vẽ
            double w = DetailCanvas.ActualWidth;
            double h = DetailCanvas.ActualHeight;
            if (w == 0) w = 500; if (h == 0) h = 600;

            double scaleX = (w - 100) / col.B;
            double scaleY = (h - 100) / col.H;
            double scale = Math.Min(scaleX, scaleY);

            double drawB = col.B * scale;
            double drawH = col.H * scale;
            double startX = (w - drawB) / 2;
            double startY = (h - drawH) / 2;

            // 2. Vẽ Bê tông
            Rectangle rect = new Rectangle
            {
                Width = drawB,
                Height = drawH,
                Stroke = Brushes.DarkSlateGray,
                StrokeThickness = 3,
                Fill = new SolidColorBrush(Color.FromRgb(240, 240, 240))
            };
            Canvas.SetLeft(rect, startX);
            Canvas.SetTop(rect, startY);
            DetailCanvas.Children.Add(rect);

            // 3. Vẽ Cốt đai (Giả định cách mép bằng lớp bảo vệ)
            double coverPx = col.ConcreteCover * scale;
            Rectangle stirrup = new Rectangle
            {
                Width = Math.Max(0, drawB - 2 * coverPx),
                Height = Math.Max(0, drawH - 2 * coverPx),
                Stroke = Brushes.Blue,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 4, 2 } // Nét đứt
            };
            Canvas.SetLeft(stirrup, startX + coverPx);
            Canvas.SetTop(stirrup, startY + coverPx);
            DetailCanvas.Children.Add(stirrup);

            // 4. Vẽ Thép chủ
            if (col.BarQuantity < 4) return;

            double barSize = Math.Max(col.BarDiameter * scale * 1.5, 12); // Phóng to 1.5 lần
            double coreX = startX + coverPx;
            double coreY = startY + coverPx;
            double coreW = drawB - 2 * coverPx;
            double coreH = drawH - 2 * coverPx;

            // Vẽ 4 góc
            DrawBar(coreX, coreY, barSize);
            DrawBar(coreX + coreW, coreY, barSize);
            DrawBar(coreX + coreW, coreY + coreH, barSize);
            DrawBar(coreX, coreY + coreH, barSize);

            // Vẽ các thanh giữa
            int remain = col.BarQuantity - 4;
            if (remain > 0)
            {
                // Chia thép cho các cạnh. Ưu tiên cạnh dài H.
                // Logic đơn giản: Chia đều (remain / 2) cho mỗi bên trái/phải
                int barsPerSide = (int)Math.Ceiling(remain / 2.0);
                double gap = coreH / (barsPerSide + 1);

                for (int i = 1; i <= barsPerSide; i++)
                {
                    if (remain <= 0) break;
                    double y = coreY + (gap * i);

                    DrawBar(coreX, y, barSize); // Bên trái
                    remain--;

                    if (remain > 0)
                    {
                        DrawBar(coreX + coreW, y, barSize); // Bên phải
                        remain--;
                    }
                }
            }
        }

        private void DrawBar(double x, double y, double size)
        {
            Ellipse bar = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(bar, x - size / 2);
            Canvas.SetTop(bar, y - size / 2);
            DetailCanvas.Children.Add(bar);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại trang trước
            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }

        // Vẽ lại khi thay đổi kích thước cửa sổ
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawSection();
        }
    }
}
