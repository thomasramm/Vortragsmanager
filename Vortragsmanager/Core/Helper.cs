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
        public static DateTime GetConregationDay(DateTime date)
        {
            Log.Info(nameof(GetConregationDay), $"date={date}");
            if (date.DayOfWeek != Wochentag)
            {
                //berechnen des nächsten Sonntag
                date = date.AddDays(7 - (int)date.DayOfWeek);

                //Abziehen von Tagen, falls anderer Wochentag als Sonntag
                if (Wochentag != DayOfWeek.Sunday)
                {
                    date = date.AddDays((int)Wochentag - 7);
                }
            }
            return date;
        }

        private static DateTime GetFirstMondayOfYear(int year)
        {
            DateTime tag1 = new DateTime(year, 1, 1);
            while (tag1.DayOfWeek != DayOfWeek.Monday)
            {
                tag1 = tag1.AddDays(1);
            }
            return tag1;
        }

        public static int CalculateWeek(DateTime date)
        {
            //Montag der aktuellen Woche
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
            }
            var year = date.Year;

            //1. Montag im Jahr
            var firstMonday = GetFirstMondayOfYear(year);

            //differenz in Wochen
            var week = (int)(((date - firstMonday).TotalDays / 7) + 1);

            if (date.Month == 1 & week > 51)
                year--;
            else if (date.Month == 12 & week == 1)
                year++;

            return year * 100 + week;
        }

        public static DateTime CalculateWeek(int week)
        {
            if (week < 200000)
            {
                var x = "error";
            }
            //am ersten Montag im Jahr beginnt kw1
            var jahr = week / 100;
            var kw = week - jahr*100 - 1;
            DateTime tag1 = GetFirstMondayOfYear(jahr);

            //Tag 1 + KW Wochen = Montag in der KW + Wochentag der Vers.
            tag1 = tag1.AddDays((kw) * 7).AddDays((int)Wochentag - 1);
            if (Wochentag == DayOfWeek.Sunday)
                tag1 = tag1.AddDays(7);

            return tag1;
        }

        private static int _currentWeek = CalculateWeek(DateTime.Today);
        public static int CurrentWeek
        {
            get
            {
                return _currentWeek;
            }
        }

        public static int GetFirstWeekOfYear(int year)
        {
            var anfang = new DateTime(year, 1, 1);
            var woche = CalculateWeek(anfang);
            return woche;
        }

        public static int CurrentVersion => 12;

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

        public static DayOfWeek Wochentag { get; set; } = DayOfWeek.Sunday;

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
        public DateWithConregation(int kw, string versammlung, int? vortrag)
        {
            Kalenderwoche = kw;
            Versammlung = versammlung;
            Vortrag = vortrag?.ToString(Helper.German) ?? "";
        }

        public int Kalenderwoche { get; set; }

        public string Versammlung { get; set; }

        public string Vortrag { get; set; }

        public override string ToString()
        {
            return $"{Helper.CalculateWeek(Kalenderwoche):dd.MM.yyyy} {Versammlung} | {Vortrag}";
        }
    }
}