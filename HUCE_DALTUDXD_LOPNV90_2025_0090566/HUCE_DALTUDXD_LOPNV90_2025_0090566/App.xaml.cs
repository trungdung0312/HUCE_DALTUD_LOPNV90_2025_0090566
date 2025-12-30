using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Ép buộc sử dụng định dạng số kiểu Mỹ (dấu chấm cho thập phân)
            var cultureInfo = new CultureInfo("en-US");

            // Cấu hình cho toàn bộ luồng xử lý của ứng dụng
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            // Đảm bảo các khung nhập liệu (WPF Framework) cũng dùng định dạng này
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(cultureInfo.IetfLanguageTag)));

            base.OnStartup(e);
        }
    }
}
