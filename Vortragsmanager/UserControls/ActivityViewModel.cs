using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    public class ActivityViewModel : ViewModelBase
    {
        public ActivityViewModel()
        {
            Messenger.Default.Register<Activity>(this, Messages.ActivityAdd, OnNewLog);
            var versNamen = DataContainer.Versammlungen.OrderBy(x => x, new Helper.EigeneKreisNameComparer()).Select(x => x.NameMitKoordinator).ToList();
            versNamen.Insert(0, "Alle");
            ListeAllerVersammlungen = new ObservableCollection<string>(versNamen);
            ListeFilteredVersammlungen = new ObservableCollection<string>(ListeAllerVersammlungen);

            Heute.Clear();
            DieseWoche.Clear();
            DieserMonat.Clear();
            DiesesJahr.Clear();
            Älter.Clear();
            Alle.Clear();

            var week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            foreach (var a in DataContainer.Aktivitäten)
            {
                var item = new ActivityItem(a);
                Alle.Add(item);

                if (a.Datum.Date == DateTime.Today)
                    Heute.Add(item);
                else if (a.Datum.Year == DateTime.Today.Year)
                {
                    if (week == CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(a.Datum, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                        DieseWoche.Add(item);
                    else if (a.Datum.Month == DateTime.Today.Month)
                        DieserMonat.Add(item);
                    else if (a.Datum.Month == ((DateTime.Today.Month == 1) ? 12 : DateTime.Today.Month - 1))
                        LetzterMonat.Add(item);
                    else
                        DiesesJahr.Add(item);
                }
                else
                    Älter.Add(item);
            }
            SetFilter();
        }

        public void SetFilter()
        {
            foreach (var a in Alle)
            {
                if (_filterAktivität == "Alle"
                    || (_filterAktivität == "Meine Redner" && (a.Typ == ActivityType.ExterneAnfrageAblehnen || a.Typ == ActivityType.ExterneAnfrageBestätigen))
                    || (_filterAktivität == "Mein Plan" && (a.Typ == ActivityType.VortragAnfragen || a.Typ == ActivityType.VortragLöschen || a.Typ == ActivityType.VortragsanfrageBestätigen || a.Typ == ActivityType.VortragTauschen))
                    || (_filterAktivität == "Sonstige" && (a.Typ == ActivityType.MailSenden)))
                {
                    if (FilterVersammlung == "Alle"
                        || string.IsNullOrEmpty(FilterVersammlung)
                        || FilterVersammlung == a.VersammlungName)
                        a.Aktiv = true;
                    else
                        a.Aktiv = false;
                }
                else
                    a.Aktiv = false;
            }
            RaisePropertyChanged(nameof(HeuteHeader));
            RaisePropertyChanged(nameof(DieseWocheHeader));
            RaisePropertyChanged(nameof(DieserMonatHeader));
            RaisePropertyChanged(nameof(LetzterMonatHeader));
            RaisePropertyChanged(nameof(DiesesJahrHeader));
            RaisePropertyChanged(nameof(ÄlterHeader));
        }

        private string _filterAktivität = "Alle";

        public string FilterAktivität
        {
            get => _filterAktivität;
            set
            {
                _filterAktivität = value;
                SetFilter();
            }
        }

        private string _filterVersammlung;

        public string FilterVersammlung
        {
            get => _filterVersammlung;
            set
            {
                _filterVersammlung = value;
                SetFilter();
            }
        }

        public void SetVersammlungfilter(string versammlungFilter = null)
        {
            ListeFilteredVersammlungen.Clear();
            var items = ListeAllerVersammlungen.Count;
            var newCount = 0;
            var maxCount = (versammlungFilter == null) ? items : 10;
            for (int i = 0; i < items; i++)
            {
                if (string.IsNullOrEmpty(versammlungFilter) || (Regex.IsMatch(ListeAllerVersammlungen[i], Regex.Escape(versammlungFilter), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)))
                {
                    ListeFilteredVersammlungen.Add(ListeAllerVersammlungen[i]);
                    newCount++;
                    if (newCount == maxCount)
                        break;
                }
            }
        }

        public ObservableCollection<string> ListeAllerVersammlungen { get; private set; }

        public ObservableCollection<string> ListeFilteredVersammlungen { get; private set; }

        public ObservableCollection<ActivityItem> Alle { get; private set; } = new ObservableCollection<ActivityItem>();

        public ObservableCollection<ActivityItem> Heute { get; private set; } = new ObservableCollection<ActivityItem>();

        public ObservableCollection<ActivityItem> DieseWoche { get; private set; } = new ObservableCollection<ActivityItem>();

        public ObservableCollection<ActivityItem> DieserMonat { get; private set; } = new ObservableCollection<ActivityItem>();

        public ObservableCollection<ActivityItem> LetzterMonat { get; private set; } = new ObservableCollection<ActivityItem>();

        public ObservableCollection<ActivityItem> DiesesJahr { get; private set; } = new ObservableCollection<ActivityItem>();

        public ObservableCollection<ActivityItem> Älter { get; private set; } = new ObservableCollection<ActivityItem>();

        public string HeuteHeader => $"Heute ({Heute.Count(x => x.Aktiv)})";
        public string DieseWocheHeader => $"Diese Woche ({DieseWoche.Count(x => x.Aktiv)})";
        public string DieserMonatHeader => $"{DateTime.Today:MMMM} ({DieserMonat.Count(x => x.Aktiv)})";
        public string LetzterMonatHeader => $"Letzter Monat ({LetzterMonat.Count(x => x.Aktiv)})";
        public string DiesesJahrHeader => $"Dieses Jahr ({DiesesJahr.Count(x => x.Aktiv)})";
        public string ÄlterHeader => $"Älter ({Älter.Count(x => x.Aktiv)})";

        private void OnNewLog(Activity message)
        {
            DataContainer.Aktivitäten.Add(message);

            var item = new ActivityItem(message);
            Heute.Add(item);
            Alle.Add(item);
            RaisePropertyChanged(nameof(HeuteHeader));
        }

        public static Activity CreateDummy()
        {
            return new Activity() { Id = 1, Typ = ActivityType.MailSenden, Datum = DateTime.Today, Kommentar = "Eintrag 1", Objekt = "Details", Versammlung = DataContainer.MeineVersammlung };
        }

        public static void AddActivity(Activity log)
        {
            Messenger.Default.Send(log, Messages.ActivityAdd);
        }

        public static void AddActivityOutside(Outside buchung, string mailtext1, string mailtext2, bool bestätigen)
        {
            var log = new Activity
            {
                Typ = bestätigen ? ActivityType.ExterneAnfrageBestätigen : ActivityType.ExterneAnfrageAblehnen,
                Versammlung = buchung?.Versammlung,
                Redner = buchung?.Ältester,
                Mails = mailtext1,
                Objekt = $"{buchung?.Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)} : {buchung?.Vortrag?.Vortrag.ToString()}",
            };
            if (!string.IsNullOrEmpty(mailtext2))
                log.Mails += _mailDelimiter + mailtext2;

            AddActivity(log);
        }

        private const string _mailDelimiter = "\r\n=========================\r\n";
    }
}