using System;
using System.Collections.Generic;
using Vortragsmanager.Models;

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

        public static int CurrentVersion => 7;

        public class EigeneKreisNameComparer : IComparer<Conregation>
        {
            public int Compare(Conregation x, Conregation y)
            {
                var eigene = DataContainer.MeineVersammlung;
                var eigenerKreis = eigene.Kreis;
                string value1 = ((x.Kreis == eigenerKreis) ? "0" : "1") + ((x == eigene) ? "0" : "1") + x.Kreis + x.Name;
                string value2 = ((y.Kreis == eigenerKreis) ? "0" : "1") + ((y == eigene) ? "0" : "1") + y.Kreis + y.Name;
                return string.Compare(value1, value2, StringComparison.InvariantCulture);
            }
        }
    }
}