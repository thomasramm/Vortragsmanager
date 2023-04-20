using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using System.Windows.Media;

namespace Vortragsmanager.Converter
{
    [ValueConversion(typeof(int), typeof(Brush))]
    public sealed class NumberToBadgeConverter : IValueConverter
    {
        public Brush DefaultStyle { get; set; }
        public Brush ErrorStyle { get; set; } 

        public NumberToBadgeConverter()
        {
            // set defaults
            DefaultStyle = new SolidColorBrush( Colors.Pink);
            ErrorStyle = new SolidColorBrush(Color.FromRgb(207, 57, 45));
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return null;
            return (int)value == 0 ? ErrorStyle : DefaultStyle;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
