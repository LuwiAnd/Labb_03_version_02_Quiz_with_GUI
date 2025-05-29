using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Labb_03_version_02_Quiz_with_GUI.Enums;


namespace Labb_03_version_02_Quiz_with_GUI.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return Enum.Parse(targetType, value.ToString() ?? "", true);
        }
    }
}
