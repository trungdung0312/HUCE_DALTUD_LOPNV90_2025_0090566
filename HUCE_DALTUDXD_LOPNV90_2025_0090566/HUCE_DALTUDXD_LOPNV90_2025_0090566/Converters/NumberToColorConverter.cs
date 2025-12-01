using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Converters
{
    // Converter này chuyển đổi giá trị số (double) sang màu sắc (Brush)
    public class NumberToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double number)
            {
                // Logic: Số âm (lỗi hoặc cảnh báo) -> Màu Đỏ
                if (number < 0) return Brushes.Red;
                // Số dương -> Màu Xanh Đậm (hoặc Đen tùy ý)
                return Brushes.DarkBlue;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
