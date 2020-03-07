using System;

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
    }
}