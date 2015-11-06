using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SbTranslationHelper.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = false;
            Visibility falseVisibility = Visibility.Collapsed;
            if (value is bool) b = (bool)value;
            if (parameter != null)
            {
                String[] parts = parameter.ToString().ToLower().Split(',');
                foreach (var part in parts)
                {
                    if (part == "not")
                        b = !b;
                    else if (part == "hidden")
                        falseVisibility = Visibility.Hidden;
                }
            }
            return b ? Visibility.Visible : falseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
