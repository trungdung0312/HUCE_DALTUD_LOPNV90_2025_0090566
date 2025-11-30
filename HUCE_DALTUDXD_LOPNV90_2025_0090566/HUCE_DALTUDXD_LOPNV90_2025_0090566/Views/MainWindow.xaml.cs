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
using HUCE_DALTUDXD_LOPNV90_2025_0090566.Views.Pages;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new ColumnInputPage());
        }
        private void BtnInput_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new ColumnInputPage());

        private void BtnResult_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new ResultPreviewPage());

        private void BtnRebar_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new RebarDesignPage());

        private void BtnFinal_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new FinalReportPage());

        private void BtnExit_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();
    }
}
