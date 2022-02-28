using System;
using Vortragsmanager.Enums;

namespace Vortragsmanager.DataModels
{
    public class BackupItem
    {
        private DateTime _date;
        private string _fileName;

        public BackupItem(string filename)
        {
            FileName = filename;
        }

        public string Name => FileName.Replace(".sqlite3", "");

        public string FileName
        {
            get => _fileName; 
            set
            {
                _fileName = value;
                _date = DateTime.ParseExact(Name, "yyyy-MM-dd-HH-mm-ss", null);
                Age = GetAge();
            }
        }

        public string DisplayName
        {
            get
            {
                switch (Age)
                {
                    case BackupAge.Heute:
                        return _date.ToLongTimeString() + " Uhr";
                    case BackupAge.Diese_Woche:
                        return _date.ToString("dddd, dd.MM HH:mm:ss") + " Uhr";
                    case BackupAge.Dieser_Monat:
                        return _date.ToString("dd.MM HH:mm:ss") + " Uhr";
                    case BackupAge.Dieses_Jahr:
                        return _date.ToString("MMMM, dd.MM HH:mm:ss") + " Uhr";
                    case BackupAge.Älter:
                        return _date.ToString("yyyy, dd.MM.yyyy HH:mm:ss") + " Uhr";
                    default:
                        return _date.ToString("dd.MM.yyyy HH:mm:ss") + " Uhr";
                }
            }
        }

        public BackupAge Age { get; set; }

        public DateTime Date => _date;

        public string AgeIcon
        {
            get
            {
                switch (Age)
                {
                    case BackupAge.Heute:
                        return @"\Images\CalendarDay_32x32.png";
                    case BackupAge.Diese_Woche:
                        return @"\Images\CalendarWeek_32x32.png";
                    case BackupAge.Dieser_Monat:
                        return @"\Images\Calendar_32x32.png";
                    case BackupAge.Dieses_Jahr:
                        return @"\Images\CalendarYear_32x32.png";
                    case BackupAge.Älter:
                        return @"\Images\CalendarYear2_32x32.png";
                    default:
                        return @"\Images\Calendar_32x32.png";
                }
                
            }
        }

        public string Zeitabstand
        {
            get
            {
                var diff = DateTime.Now - _date;

                if (diff.TotalMinutes <= 1)
                    return $"vor {diff.Seconds} Sekunden.";
                if (diff.TotalMinutes < 60)
                    return $"vor {diff.Minutes} Minuten und {diff.Seconds} Sekunden";
                if (diff.TotalHours < 24)
                    return $"vor ~ {diff.Hours} Stunden und {diff.Minutes} Minuten.";
                if (diff.TotalDays < 31)
                    return $"vor {diff.Days} Tagen";

                return $"vor {Math.Round(diff.TotalDays / 31,1)} Monaten";
            }
        }

        private BackupAge GetAge()
        {
            //heute
            if (_date.Date == DateTime.Today)
                return BackupAge.Heute;

            //Diese Woche
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = _date.Date.AddDays(-1 * (int)cal.GetDayOfWeek(_date));
            var d2 = DateTime.Today.Date.AddDays(-1 * (int)cal.GetDayOfWeek(DateTime.Today));
            if (d1 == d2)
            {
                return BackupAge.Diese_Woche;
            }

            //dieses Jahr
            if (_date.Year == DateTime.Today.Year)
            {
                //dieser Monat
                if (_date.Month == DateTime.Today.Month)
                {
                    return BackupAge.Dieser_Monat;
                }

                return BackupAge.Dieses_Jahr;
            }

            //älter
            return BackupAge.Älter;
        }
    }
}