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
            if (column == null) return;
            _viewModel.DesignColumn = column;
            DrawSection();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RebarDesignViewModel.DesignColumn)) DrawSection();
        }

        private void DetailCanvas_SizeChanged(object sender, SizeChangedEventArgs e) => DrawSection();
        private void BtnBack_Click(object sender, RoutedEventArgs e) { if (NavigationService.CanGoBack) NavigationService.GoBack(); }

        private void DrawSection()
        {
            DetailCanvas.Children.Clear();
            var col = _viewModel.DesignColumn;
            if (col == null || col.B <= 0 || col.H <= 0) return;

            double canvasW = DetailCanvas.ActualWidth;
            double canvasH = DetailCanvas.ActualHeight;
            if (canvasW <= 0 || canvasH <= 0) return;

            // 1. TÍNH TỈ LỆ VẼ (SCALE)
            double padding = 60;
            double scaleX = (canvasW - padding * 2) / col.B;
            double scaleY = (canvasH - padding * 2) / col.H;
            double scale = Math.Min(scaleX, scaleY);

            double drawB = col.B * scale;
            double drawH = col.H * scale;
            double startX = (canvasW - drawB) / 2;
            double startY = (canvasH - drawH) / 2;

            double coverPx = (col.ConcreteCover > 0 ? col.ConcreteCover : 25) * scale;
            double stirrupStroke = 1.5; // Nét đai mảnh (1.5px)

            // Đường kính góc (Mặc định lấy D1 của phương X)
            double dCorner = col.Diameter1_X > 0 ? col.Diameter1_X : 20;
            double sizeCorner = Math.Max(dCorner * scale, 6.0);

            // 2. VẼ BÊ TÔNG
            Rectangle rectConcrete = new Rectangle
            {
                Width = drawB,
                Height = drawH,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(rectConcrete, startX); Canvas.SetTop(rectConcrete, startY);
            DetailCanvas.Children.Add(rectConcrete);

            // 3. VẼ CỐT ĐAI
            double cornerRadius = (sizeCorner / 2) + stirrupStroke;
            Rectangle rectStirrup = new Rectangle
            {
                Width = Math.Max(0, drawB - 2 * coverPx),
                Height = Math.Max(0, drawH - 2 * coverPx),
                Stroke = Brushes.Red,
                StrokeThickness = stirrupStroke,
                RadiusX = cornerRadius,
                RadiusY = cornerRadius
            };
            Canvas.SetLeft(rectStirrup, startX + coverPx); Canvas.SetTop(rectStirrup, startY + coverPx);
            DetailCanvas.Children.Add(rectStirrup);

            // Tọa độ tâm các thanh thép (trừ đi bán kính để vẽ hình tròn)
            double centerOffset = coverPx + (stirrupStroke / 2) + (sizeCorner / 2);
            double dotOffset = centerOffset - (sizeCorner / 2); // Tọa độ Top-Left của Ellipse

            // 4. VẼ 4 THANH GÓC (CỐ ĐỊNH) - Luôn dùng Diameter1
            DrawRebarDot(startX + centerOffset, startY + centerOffset, sizeCorner); // Góc TL
            DrawRebarDot(startX + drawB - centerOffset, startY + centerOffset, sizeCorner); // Góc TR
            DrawRebarDot(startX + drawB - centerOffset, startY + drawH - centerOffset, sizeCorner); // Góc BR
            DrawRebarDot(startX + centerOffset, startY + drawH - centerOffset, sizeCorner); // Góc BL

            // 5. VẼ THANH BỤNG PHƯƠNG X (TOP & BOTTOM)
            int totalX = col.Quantity1_X + col.Quantity2_X;

            // Phải có tối thiểu 2 thanh (2 góc) mới xét tiếp
            if (totalX > 2)
            {
                int midsPerFaceX = (totalX - 2) / 2;

                if (midsPerFaceX > 0)
                {
                    double dMidX = (col.Quantity2_X > 0)
                        ? col.Diameter2_X
                        : col.Diameter1_X;

                    double sizeMidX = Math.Max(dMidX * scale, 6.0);
                    double gapX = (drawB - 2 * centerOffset) / (midsPerFaceX + 1);

                    for (int i = 1; i <= midsPerFaceX; i++)
                    {
                        double x = startX + centerOffset + i * gapX;

                        // Cạnh trên
                        DrawRebarDot(x, startY + centerOffset, sizeMidX);
                        // Cạnh dưới
                        DrawRebarDot(x, startY + drawH - centerOffset, sizeMidX);
                    }
                }
            }

            // 6. VẼ THANH BỤNG PHƯƠNG Y (LEFT & RIGHT)
            int totalY = col.Quantity1_Y + col.Quantity2_Y;

            if (totalY > 2)
            {
                int midsPerFaceY = (totalY - 2) / 2;

                if (midsPerFaceY > 0)
                {
                    double dMidY = (col.Quantity2_Y > 0)
                        ? col.Diameter2_Y
                        : col.Diameter1_Y;

                    double sizeMidY = Math.Max(dMidY * scale, 6.0);
                    double gapY = (drawH - 2 * centerOffset) / (midsPerFaceY + 1);

                    for (int j = 1; j <= midsPerFaceY; j++)
                    {
                        double y = startY + centerOffset + j * gapY;

                        // Cạnh trái
                        DrawRebarDot(startX + centerOffset, y, sizeMidY);
                        // Cạnh phải
                        DrawRebarDot(startX + drawB - centerOffset, y, sizeMidY);
                    }
                }
            }
        }

        // Hàm vẽ chấm tròn (Input x,y là TÂM của chấm)
        private void DrawRebarDot(double xCenter, double yCenter, double size)
        {
            Ellipse dot = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = Brushes.Red,
                Stroke = Brushes.DarkRed,
                StrokeThickness = 1
            };
            Canvas.SetLeft(dot, xCenter - size / 2);
            Canvas.SetTop(dot, yCenter - size / 2);
            DetailCanvas.Children.Add(dot);
        }
    }
}
