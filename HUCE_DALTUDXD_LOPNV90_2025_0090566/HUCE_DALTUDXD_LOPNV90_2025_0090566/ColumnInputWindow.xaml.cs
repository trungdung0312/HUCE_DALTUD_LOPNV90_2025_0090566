using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
using System.Xml.Serialization;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566
{
    public partial class ColumnInputWindow : Window
    {
        public ObservableCollection<ColumnData> Columns { get; set; }
        private ColumnData selectedRow;

        public ColumnInputWindow()
        {
            InitializeComponent();
            Columns = new ObservableCollection<ColumnData>();
            dataGrid.ItemsSource = Columns;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (TryReadForm(out ColumnData data))
            {
                Columns.Add(data);
                ClearForm();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRow != null && TryReadForm(out ColumnData data))
            {
                int index = Columns.IndexOf(selectedRow);
                Columns[index] = data;
                dataGrid.Items.Refresh();
                ClearForm();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRow != null)
            {
                Columns.Remove(selectedRow);
                ClearForm();
            }
        }

        private void btnTinhToan_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRow == null)
            {
                MessageBox.Show("Hãy chọn một dòng trong bảng để tính toán!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedRow.N <= 0 || selectedRow.B <= 0 || selectedRow.H <= 0 || selectedRow.L <= 0)
            {
                MessageBox.Show("Dữ liệu không hợp lệ! Giá trị phải > 0.", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (selectedRow.Spacing <= selectedRow.Cover)
            {
                MessageBox.Show("Khoảng cách cốt thép phải lớn hơn lớp bảo vệ!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Mở cửa sổ kết quả demo
                ResultWindow resultWindow = new ResultWindow();
                resultWindow.Show();

                // Đóng cửa sổ nhập liệu
                this.Close();
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is ColumnData row)
            {
                selectedRow = row;
                PopulateForm(row);
            }
        }

        private bool TryReadForm(out ColumnData data)
        {
            data = null;
            try
            {
                data = new ColumnData
                {
                    N = double.Parse(txtN.Text),
                    Mx = double.Parse(txtMx.Text),
                    My = double.Parse(txtMy.Text),
                    B = double.Parse(txtB.Text),
                    H = double.Parse(txtH.Text),
                    L = double.Parse(txtL.Text),
                    Psi = (cbPsi.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Rb = (cbRb.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Rs = (cbRs.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Phi = double.Parse(txtPhi.Text),
                    Cover = double.Parse(txtCover.Text),
                    Spacing = double.Parse(txtSpacing.Text)
                };
                return true;
            }
            catch
            {
                MessageBox.Show("Lỗi nhập liệu. Hãy kiểm tra lại!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void PopulateForm(ColumnData row)
        {
            txtN.Text = row.N.ToString();
            txtMx.Text = row.Mx.ToString();
            txtMy.Text = row.My.ToString();
            txtB.Text = row.B.ToString();
            txtH.Text = row.H.ToString();
            txtL.Text = row.L.ToString();
            txtPhi.Text = row.Phi.ToString();
            txtCover.Text = row.Cover.ToString();
            txtSpacing.Text = row.Spacing.ToString();
        }

        private void ClearForm()
        {
            txtN.Clear(); txtMx.Clear(); txtMy.Clear(); txtB.Clear(); txtH.Clear();
            txtL.Clear(); txtPhi.Clear(); txtCover.Clear(); txtSpacing.Clear();
            cbPsi.SelectedIndex = -1; cbRb.SelectedIndex = -1; cbRs.SelectedIndex = -1;
            selectedRow = null;
        }
    }

    public class ColumnData
    {
        public double N { get; set; }
        public double Mx { get; set; }
        public double My { get; set; }
        public double B { get; set; }
        public double H { get; set; }
        public double L { get; set; }
        public string Psi { get; set; }
        public string Rb { get; set; }
        public string Rs { get; set; }
        public double Phi { get; set; }
        public double Cover { get; set; }
        public double Spacing { get; set; }
    }
}



