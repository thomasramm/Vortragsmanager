using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Vortragsmanager.Converter
{
    public class ListToStringConverter : IValueConverter
    {
        public int MaxItems { get; set; }
        public string Einheit { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IList values = value as IList;
            if (values != null)
            {
                if (values.Count >= MaxItems)
                    return $"{values.Count} {Einheit}";

                StringBuilder builder = new StringBuilder();
                bool isFirst = true;
                foreach (object obj in values)
                {
                    if (isFirst)
                    {
                        builder.Append(obj);
                        isFirst = false;
                        continue;
                    }
                    builder.AppendFormat(", {0}", obj);
                }
                return builder.ToString();
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
