using System;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Module;

namespace Vortragsmanager.Helper
{
    internal class DateCalcuation
    {
        public static int CurrentWeek { get; } = CalculateWeek(DateTime.Today);

        public static Wochentag Wochentag { get; set; } = Wochentag.Sonntag;

        public static DateTime GetConregationDay(DateTime date, Wochentag day)
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
            if (day == Wochentag.Sonntag)
                tag1 = tag1.AddDays(7);

            return tag1;
        }

        public static DateTime CalculateWeek(int week)
        {
            return CalculateWeek(week, DataContainer.MeineVersammlung);
        }

        public static int GetFirstWeekOfYear(int year)
        {
            var anfang = new DateTime(year, 1, 1);
            var woche = CalculateWeek(anfang);
            return woche;
        }

        /// <summary>
        /// Fügt einer übergebenen Kw eine bestimmte Anzahl Wochen hinzu
        /// </summary>
        /// <param name="woche">Die Startwoche im Format YYYYMM</param>
        /// <param name="add">Die Anzahl Wochen die hinzu addiert werden sollen</param>
        /// <returns>Die Zielwoche im Format YYYYMM</returns>      
        internal static int CalculateWeek(int woche, int add)
        {
            if (add > 0) return AddWeek(woche, add);
            if (add < 0) return SubstractWeek(woche, add*-1);
            return woche;
        }

        internal static int AddWeek(int woche, int add)
        {
            var baseJahr = woche / 100;
            var baseWoche = woche - baseJahr * 100;

            var addJahr = add / 53;
            var addWoche = add - addJahr * 53;

            var resultWoche = baseWoche + addWoche;
            var resultJahr = baseJahr + addJahr + resultWoche / 53;

            resultWoche -= (resultWoche / 53) * 53;

            if (resultWoche == 0)
            {
                resultWoche = 1;
            }

            return resultJahr * 100 + resultWoche;
        }

        internal static int SubstractWeek(int woche, int substract)
        {
            var baseJahr = woche / 100;
            var baseWoche = woche - baseJahr * 100;

            var addJahr = substract / 53;
            var addWoche = substract - addJahr * 53;

            var resultJahr = baseJahr - addJahr;
            var resultWoche = baseWoche - addWoche;

            if (baseWoche <= addWoche)
            {
                resultWoche = 53 + baseWoche - addWoche;
                resultJahr -= 1;
            }

            return resultJahr * 100 + resultWoche;
        }

        public static bool GetDayOfWeeks(ref string input, Wochentag day)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var test = day.ToString();

            if (input.Contains(test))
            {
                input = input.Replace(test, "");
                return true;
            }

            if (test.Length >= 3)
            {
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
            }
            if (test.Length >= 2)
            {
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
            }

            return false;
        }
    }
}