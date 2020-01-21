using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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