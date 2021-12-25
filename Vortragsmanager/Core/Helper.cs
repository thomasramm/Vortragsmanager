using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Vortragsmanager.Datamodels;
using System.Linq;
using System.Windows.Markup;
using System.Windows;

namespace Vortragsmanager.Core
{
    internal class Helper
    {
        public static DateTime GetConregationDay(DateTime date, DayOfWeeks day)
        {
            Log.Info(nameof(GetConregationDay), $"date={date}");
            if ((int)date.DayOfWeek != (int)day)
            {
                //berechnen des Versammlungstages in der gewählten Woche.
                //Da die Woche in C# am Sonntag beginnt, möchte ich den Sonntag der nächsten Woche haben
                var clickDay = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                var conDay = Wochentag == 0 ? 7 : (int)Wochentag;
                date = date.AddDays(conDay - clickDay);
            }
            return date;
        }

        public static DateTime GetConregationDay(DateTime date)
        {
            return GetConregationDay(date, Wochentag);
        }

        private static DateTime GetFirstMondayOfYear(int year)
        {
            if (year == 0)
            {
                year = DateTime.Today.Year;
            }

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

        public static DateTime CalculateWeek(int week, Conregation versammlung)
        {
            //am ersten Montag im Jahr beginnt kw1
            var jahr = week / 100;
            var kw = week - jahr*100 - 1;
            DateTime tag1 = GetFirstMondayOfYear(jahr);
            var day = versammlung.Zeit.Get(jahr).Tag;

            //Tag 1 + KW Wochen = Montag in der KW + Wochentag der Vers.
            tag1 = tag1.AddDays((kw) * 7).AddDays((int)day - 1);
            if (day == DayOfWeeks.Sonntag)
                tag1 = tag1.AddDays(7);

            return tag1;
        }

        public static DateTime CalculateWeek(int week)
        {
            return CalculateWeek(week, DataContainer.MeineVersammlung);
        }

        private static readonly int _currentWeek = CalculateWeek(DateTime.Today);
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

        public static DayOfWeeks Wochentag { get; set; } = DayOfWeeks.Sonntag;

        public static CultureInfo German { get; } = new CultureInfo("de-DE");

        internal static int AddWeek(int woche, int add)
        {
            var baseJahr = woche / 100;
            var baseWoche = woche - baseJahr * 100;

            var addJahr = add / 53;
            var addWoche = add - addJahr * 53;

            var resultWoche = baseWoche + addWoche;
            var resultJahr = baseJahr + addJahr + resultWoche / 53;

            resultWoche -= (resultWoche / 53) * 53;

            return resultJahr * 100 + resultWoche;
        }

        public static string TemplateFolder => AppDomain.CurrentDomain.BaseDirectory + @"Templates\";

        public static string ConvertToString(object input)
        {
            if (input == null)
                return string.Empty;
            return input.ToString().Trim();
        }

        public static Version ConvertToVersion(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input == "0")
                return new Version();
            else
                return new Version(input);
        }

        public static bool CheckNegativListe(string input, string[] liste)
        {
            var u = input.ToUpperInvariant();
            return !liste.Any(x => x == u);
        }

        public static bool GetDayOfWeeks(ref string input, DayOfWeeks day)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var test = day.ToString();

            if (input.Contains(test))
            {
                input = input.Replace(test, "");
                return true;
            }

            test = test.Substring(0, 3) + " ";
            if (input.Contains(test))
            {
                input = input.Replace(test, "");
                return true;
            }

            test = test.Substring(0, 3) + ".";
            if (input.Contains(test))
            {
                input = input.Replace(test, "");
                return true;
            }

            test = test.Substring(0, 3) + ",";
            if (input.Contains(test))
            {
                input = input.Replace(test, "");
                return true;
            }

            test = test.Substring(0, 2) + " ";
            if (input.Contains(test))
            {
                input = input.Replace(test, "");
                return true;
            }

            test = test.Substring(0, 2) + ".";
            if (input.Contains(test))
            {
                input = input.Replace(test, "");
                return true;
            }

            test = test.Substring(0, 2) + ",";
            if (input.Contains(test))
            {
                input = input.Replace(test, "");
                return true;
            }

            return false;
        }
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

    public class BooleanToVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = values.OfType<bool>().Any(b => !b) ? Visibility.Collapsed : Visibility.Visible;
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;
        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }

                    this._enumType = value;
                }
            }
        }

        public EnumBindingSourceExtension() { }

        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._enumType)
                throw new InvalidOperationException("The EnumType must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == this._enumType)
                return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
        
    }

    public enum DayOfWeeks
    {
        Montag = 1,
        Dienstag = 2,
        Mittwoch = 3,
        Donnerstag = 4,
        Freitag = 5,
        Samstag = 6,
        Sonntag = 0,
    }

    public static class DialogCloser
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogCloser),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null)
                window.DialogResult = e.NewValue as bool?;
        }
        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }
    }
}

namespace Vortragsmanager.Core.DataHelper
{ 
    /// <summary>
    /// Wird genutzt für Aktivitäten in meiner Versammlung (Redner)
    /// </summary>
    public class DateWithConregation
    {
        public DateWithConregation(DateTime datum, string versammlung, int? vortrag)
        {
            Datum = datum;
            Kalenderwoche = Helper.CalculateWeek(datum);
            Versammlung = versammlung;
            Vortrag = vortrag?.ToString(Helper.German) ?? "";
        }

        public int Kalenderwoche { get; set; }

        public DateTime Datum { get; set; }

        public string Versammlung { get; set; }

        public string Vortrag { get; set; }

        public override string ToString()
        {
            return $"{Datum :dd.MM.yyyy} {Versammlung} | {Vortrag}";
        }
    }

    public class Zusammenkunftszeit
    {
        public Zusammenkunftszeit(int jahr)
        {
            Jahr = jahr;
            Tag = DayOfWeeks.Sonntag;
            Zeit = "10:00 Uhr";
        }

        public Zusammenkunftszeit(int jahr, DayOfWeeks tag, string zeit)
        {
            Jahr = jahr;
            Tag = tag;
            Zeit = zeit;
        }

        public int Jahr { get; set; }

        public DayOfWeeks Tag { get; set; }

        public string Zeit { get; set; }

        public override string ToString()
        {
            return $"{Tag} {Zeit}";
        }
    }

    public class Zusammenkunftszeiten
    {
        public List<Zusammenkunftszeit> Items { get; } = new List<Zusammenkunftszeit>();

        public Zusammenkunftszeit Get(int Jahr)
        {
            Log.Info(nameof(Get), Jahr);
            
            //Das aktuelle Jahr abfragen
            var myItem = Items.FirstOrDefault(x => x.Jahr == Jahr);
            if (myItem != null)
                return myItem;

            //den letzen Eintrag vor meinem Jahr abfragen
            myItem = Items.Where(x => x.Jahr <= Jahr).OrderByDescending(y => y.Jahr).FirstOrDefault();
            if (myItem != null)
                return myItem;

            //das jüngste Jahr nach meinem Eintrag abfragen
            myItem = Items.OrderBy(y => y.Jahr).FirstOrDefault();
            if (myItem != null)
                return myItem;

            //Es gibt kein Jahr, neues Jahr erstellen
            myItem = new Zusammenkunftszeit(Jahr);
            Items.Add(myItem);
            return myItem;
        }

        public Zusammenkunftszeit Set(int Jahr, DayOfWeeks Tag, string Zeit)
        {
            Log.Info(nameof(Set), $"jahr={Jahr}, Tag={Tag}, Zeit={Zeit}");
            var myItem = Items.FirstOrDefault(x => x.Jahr == Jahr);
            if (myItem == null) 
            {
                myItem = new Zusammenkunftszeit(Jahr, Tag, Zeit);
                Items.Add(myItem);
            }
            else
            {
                myItem.Tag = Tag;
                myItem.Zeit = Zeit;
            }
            return myItem;
        }

        public Zusammenkunftszeit Add(int Jahr, DayOfWeeks Tag, string Zeit)
        {
            return Set(Jahr, Tag, Zeit);
        }

        public Zusammenkunftszeit Add(int Jahr, int Tag, string Zeit)
        {
            return Set(Jahr, (DayOfWeeks)Tag, Zeit);
        }

        public void Remove(Zusammenkunftszeit zeit)
        {
            Items.Remove(zeit);
        }

        public Zusammenkunftszeit GetLastItem()
        {
            return Items.OrderByDescending(x => x.Jahr).FirstOrDefault();
        }
    }
}