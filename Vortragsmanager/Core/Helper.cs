using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.Core
{
    internal class Helper
    {
        public static DateTime GetSunday(DateTime date)
        {
            Log.Info(nameof(GetSunday), $"date={date}");
            if (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(7 - (int)date.DayOfWeek);
            }
            return date;
        }

        public static int CurrentVersion => 11;

        public class EigeneKreisNameComparer : IComparer<Conregation>
        {
            public int Compare(Conregation x, Conregation y)
            {
                var eigene = DataContainer.MeineVersammlung;
                var eigenerKreis = eigene.Kreis;
                string value1 = (x.Kreis == eigenerKreis ? "0" : "1") + (x == eigene ? "0" : "1") + x.Kreis + x.Name;
                string value2 = (y.Kreis == eigenerKreis ? "0" : "1") + (y == eigene ? "0" : "1") + y.Kreis + y.Name;
                return string.Compare(value1, value2, StringComparison.InvariantCulture);
            }
        }

        public static MyGloabalSettings GlobalSettings { get; set; }

        private static int _displayedYear = DateTime.Now.Year;

        public static int DisplayedYear
        {
            get => _displayedYear;
            set
            {
                _displayedYear = value;
                Messenger.Default.Send(_displayedYear, Messages.DisplayYearChanged);
            }
        }

        public static CultureInfo German { get; } = new CultureInfo("de-DE");

        public static string TemplateFolder => AppDomain.CurrentDomain.BaseDirectory + @"Templates\";

    }

    public class DoubleToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value, culture);
        }
    }
}

namespace Vortragsmanager.Core.DataHelper
{ 
    public class DateWithConregation
    {
        public DateWithConregation(DateTime datum, string versammlung, int? vortrag)
        {
            Datum = datum;
            Versammlung = versammlung;
            Vortrag = vortrag?.ToString(Helper.German) ?? "";
        }

        public DateTime Datum { get; set; }

        public string Versammlung { get; set; }

        public string Vortrag { get; set; }

        public override string ToString()
        {
            return $"{Datum:dd.MM.yyyy} {Versammlung} | {Vortrag}";
        }
    }
}