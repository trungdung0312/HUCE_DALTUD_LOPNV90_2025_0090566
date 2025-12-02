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

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public void LoadColumnData(RebarResultData column)
        {
            _viewModel.DesignColumn = column;
            DrawSection();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RebarDesignViewModel.DesignColumn))
            {
                DrawSection();
            }
        }

        private void DetailCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawSection();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }

        // =================================================================
        // HÀM VẼ HÌNH CHÍNH (ĐÃ CHỈNH SỬA TỌA ĐỘ CHUẨN KỸ THUẬT)
        // =================================================================
        private void DrawSection()
        {
            DetailCanvas.Children.Clear();
            var col = _viewModel.DesignColumn;
            if (col == null) return;

            // --- 0. THÔNG SỐ GIẢ ĐỊNH ĐỂ VẼ ---
            double realCover = col.ConcreteCover > 0 ? col.ConcreteCover : 25.0;
            double stirrupDiameter = 8.0; // Giả định cốt đai phi 8 để tính khoảng hở

            // 1. Lấy kích thước khung vẽ
            double w = DetailCanvas.ActualWidth > 0 ? DetailCanvas.ActualWidth : 500;
            double h = DetailCanvas.ActualHeight > 0 ? DetailCanvas.ActualHeight : 600;

            // 2. Tính tỷ lệ Scale
            double scaleX = (w - 120) / col.B;
            double scaleY = (h - 120) / col.H;
            double scale = Math.Min(scaleX, scaleY);

            // Kích thước Pixel
            double drawB = col.B * scale;
            double drawH = col.H * scale;
            double coverPx = realCover * scale;        // Lớp bảo vệ (px)
            double stirrupPx = stirrupDiameter * scale; // Đường kính đai (px) - Dùng để đẩy thép chủ vào trong

            // Tọa độ gốc (Góc trên trái của bê tông)
            double startX = (w - drawB) / 2;
            double startY = (h - drawH) / 2;

            // ---------------------------------------------------------
            // BƯỚC 1: VẼ KHỐI BÊ TÔNG (MÀU XÁM)
            // ---------------------------------------------------------
            Rectangle concreteBlock = new Rectangle
            {
                Width = drawB,
                Height = drawH,
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Fill = new SolidColorBrush(Color.FromRgb(211, 211, 211)) // LightGray
            };
            Canvas.SetLeft(concreteBlock, startX);
            Canvas.SetTop(concreteBlock, startY);
            DetailCanvas.Children.Add(concreteBlock);

            // ---------------------------------------------------------
            // BƯỚC 2: VẼ CỐT ĐAI (NẰM TRONG LỚP BẢO VỆ)
            // ---------------------------------------------------------
            // Vị trí cốt đai: Cách mép ngoài bê tông một đoạn = Cover
            Rectangle stirrup = new Rectangle
            {
                Width = Math.Max(0, drawB - 2 * coverPx),
                Height = Math.Max(0, drawH - 2 * coverPx),
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 4, 2 }, // Nét đứt
                Fill = Brushes.Transparent // Để lộ màu bê tông nền
            };

            Canvas.SetLeft(stirrup, startX + coverPx);
            Canvas.SetTop(stirrup, startY + coverPx);
            DetailCanvas.Children.Add(stirrup);

            // ---------------------------------------------------------
            // BƯỚC 3: VẼ THÉP CHỦ (NẰM TRONG CỐT ĐAI)
            // ---------------------------------------------------------
            if (col.BarQuantity < 4) return;

            double barSize = Math.Max(col.BarDiameter * scale * 1.5, 14);

            // TỌA ĐỘ ĐẶT THÉP (CỰC KỲ QUAN TRỌNG)
            // Thép chủ phải nằm lọt trong cốt đai.
            // Tọa độ mép ngoài thép chủ = Tọa độ mép trong cốt đai.
            // => Tọa độ Tâm thép = (Start + Cover + Đường kính đai) + (Bán kính thép chủ)
            // Tuy nhiên hàm DrawBar của mình nhận tọa độ Tâm.
            // Để đơn giản: Ta xác định khung chữ nhật "Lõi thép" đi qua TÂM các thanh thép.

            // Khoảng cách từ mép bê tông đến TÂM thanh thép góc:
            // Offset = Cover + d_dai + (d_chu / 2)
            // Nhưng để vẽ cho đẹp và thoáng, ta chỉ cần đảm bảo nó nằm gọn trong đai.

            // Offset tính từ mép bê tông vào đến tâm thanh thép:
            double offsetToCenter = coverPx + stirrupPx + (barSize / 2);

            // Chiều rộng/cao của hình chữ nhật nối tâm các thanh thép
            double coreW = drawB - 2 * offsetToCenter;
            double coreH = drawH - 2 * offsetToCenter;

            // Tọa độ bắt đầu (Tâm thanh góc trái trên)
            double coreX = startX + offsetToCenter;
            double coreY = startY + offsetToCenter;

            // Vẽ 4 thanh góc
            DrawBar(coreX, coreY, barSize);
            DrawBar(coreX + coreW, coreY, barSize);
            DrawBar(coreX + coreW, coreY + coreH, barSize);
            DrawBar(coreX, coreY + coreH, barSize);

            // Vẽ các thanh giữa
            int remain = col.BarQuantity - 4;
            if (remain > 0)
            {
                int barsPerSide = (int)Math.Ceiling(remain / 2.0);
                double gap = coreH / (barsPerSide + 1);

                for (int i = 1; i <= barsPerSide; i++)
                {
                    if (remain <= 0) break;
                    double y = coreY + (gap * i);

                    DrawBar(coreX, y, barSize); // Trái
                    remain--;

                    if (remain > 0)
                    {
                        DrawBar(coreX + coreW, y, barSize); // Phải
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
            // Hàm này nhận x, y là toạ độ TÂM của chấm tròn
            Canvas.SetLeft(bar, x - size / 2);
            Canvas.SetTop(bar, y - size / 2);
            DetailCanvas.Children.Add(bar);
        }
    }
}
