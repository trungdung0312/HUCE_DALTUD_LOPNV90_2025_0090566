using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BTCTColumnDesign
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

        private void DrawSection()
        {
            CanvasSection.Children.Clear();
            double W = CanvasSection.Width;
            double H = CanvasSection.Height;

            // Khung chữ nhật
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

            // Bố trí thép 4 góc
            for (int i = 0; i < 4; i++)
            {
                Ellipse steel = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Fill = Brushes.Red
                };
                double offsetX = (i % 2 == 0) ? 0 : rect.Width - steel.Width;
                double offsetY = (i / 2 == 0) ? 0 : rect.Height - steel.Height;
                Canvas.SetLeft(steel, (W - rect.Width) / 2 + offsetX);
                Canvas.SetTop(steel, (H - rect.Height) / 2 + offsetY);
                CanvasSection.Children.Add(steel);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Results.Add(new ColumnResult
            {
                TT = Results.Count + 1,
                B = TxtB.Text,
                H = TxtH.Text,
                Concrete = (CmbConcrete.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString(),
                Steel = (CmbSteel.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString(),
                As = "TODO",
                Md = "TODO",
                Nd = "TODO"
            });
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ResultGrid.SelectedItem is ColumnResult selected)
                Results.Remove(selected);
        }

        private void BtnCompute_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO: Thêm công thức tính toán BTCT.", "Thông báo");
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class ColumnResult
    {
        public int TT { get; set; }
        public string B { get; set; }
        public string H { get; set; }
        public string Concrete { get; set; }
        public string Steel { get; set; }
        public string As { get; set; }
        public string Md { get; set; }
        public string Nd { get; set; }
    }
}
