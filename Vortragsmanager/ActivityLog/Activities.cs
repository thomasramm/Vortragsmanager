using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.ActivityLog
{
    public class Activities : ViewModelBase
    {
        public Activities()
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

            foreach (var a in DataContainer.Aktivitäten.OrderByDescending(x => x.Datum))
            {
                var item = new Item(a);
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
                    || (_filterAktivität == "Meine Redner" && (a.Typ == Types.ExterneAnfrageAblehnen || a.Typ == Types.ExterneAnfrageBestätigen || a.Typ == Types.ExterneAnfrageListeSenden))
                    || (_filterAktivität == "Mein Plan" && (a.Typ == Types.Sonstige || a.Typ == Types.Sonstige))
                    || (_filterAktivität == "Sonstige" && (a.Typ == Types.Sonstige || a.Typ == Types.ExterneAnfrageListeSenden)))
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

        public ObservableCollection<Item> Alle { get; private set; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> Heute { get; private set; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> DieseWoche { get; private set; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> DieserMonat { get; private set; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> LetzterMonat { get; private set; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> DiesesJahr { get; private set; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> Älter { get; private set; } = new ObservableCollection<Item>();

        public string HeuteHeader => $"Heute ({Heute.Count(x => x.Aktiv)})";
        public string DieseWocheHeader => $"Diese Woche ({DieseWoche.Count(x => x.Aktiv)})";
        public string DieserMonatHeader => $"{DateTime.Today:MMMM} ({DieserMonat.Count(x => x.Aktiv)})";
        public string LetzterMonatHeader => $"Letzter Monat ({LetzterMonat.Count(x => x.Aktiv)})";
        public string DiesesJahrHeader => $"Dieses Jahr ({DiesesJahr.Count(x => x.Aktiv)})";
        public string ÄlterHeader => $"Älter ({Älter.Count(x => x.Aktiv)})";

        private void OnNewLog(Activity message)
        {
            DataContainer.Aktivitäten.Add(message);

            var item = new Item(message);
            Heute.Insert(0, item);
            Alle.Add(item);
            RaisePropertyChanged(nameof(HeuteHeader));
        }

        private static void AddActivity(Activity log)
        {
            Messenger.Default.Send(log, Messages.ActivityAdd);
        }

        public static void AddActivityOutside(Outside buchung, string mailtext1, string mailtext2, bool bestätigen)
        {
            var log = new Activity
            {
                Typ = bestätigen ? Types.ExterneAnfrageBestätigen : Types.ExterneAnfrageAblehnen,
                Versammlung = buchung?.Versammlung,
                Redner = buchung?.Ältester,
                Mails = mailtext1,
                Objekt = $"{buchung?.Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)} : {buchung?.Vortrag?.Vortrag.ToString()}",
            };
            if (!string.IsNullOrEmpty(mailtext2))
                log.Mails += _mailDelimiter + mailtext2;

            AddActivity(log);
        }

        public static void AddActivityOutsideSendList(Speaker redner, string mailtext)
        {
            var log = new Activity
            {
                Typ = Types.ExterneAnfrageListeSenden,
                Redner = redner,
                Versammlung = DataContainer.MeineVersammlung,
                Mails = mailtext
            };
            AddActivity(log);
        }

        public static void AddActivitySendMail(string mailtext, int? maxEntfernung)
        {
            var log = new Activity
            {
                Typ = Types.SendMail,
                Versammlung = DataContainer.MeineVersammlung,
                Mails = mailtext,
                Objekt = (maxEntfernung == null) ? "Mail an alle Koordinatoren im Kreis" : $"Mail an alle Koordinatoren im Umkreis von {maxEntfernung} km",
            };
            AddActivity(log);
        }

        public static void AddActivityRednerAnfrageAbgelehnt(Speaker redner, string vortrag, string wochen, string mailtext, bool anfrageGelöscht)
        {
            var log = new Activity
            {
                Typ = Types.RednerAnfrageAbgesagt,
                Versammlung = redner?.Versammlung,
                Redner = redner,
                Mails = mailtext,
                Objekt = $"Datum:   {wochen}{Environment.NewLine}" +
                         $"Vortrag: {vortrag}",
            };

            if (anfrageGelöscht)
                log.Objekt += Environment.NewLine + "Die komplette Anfrage wurde daraufhin gelöscht, da keine weiteres Datum oder weiterer Redner angefragt wurde";

            AddActivity(log);
        }

        public static void AddActivityRednerAnfrageZugesagt(Invitation einladung, string mailtext, bool anfrageGelöscht)
        {
            var log = new Activity
            {
                Typ = Types.RednerAnfrageBestätigt,
                Versammlung = einladung?.Ältester.Versammlung,
                Redner = einladung?.Ältester,
                Objekt = $"Datum:   {einladung?.Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}{Environment.NewLine}" +
                         $"Vortrag: {einladung?.Vortrag?.Vortrag.ToString()}",
                Mails = mailtext,
            };
            if (anfrageGelöscht)
                log.Objekt += Environment.NewLine + "Die komplette Anfrage wurde daraufhin gelöscht, da keine weiteres Datum oder weiterer Redner angefragt wurde";

            AddActivity(log);
        }

        public static void AddActivityRednerAnfragen(Inquiry anfrage)
        {
            if (anfrage == null)
                return;
            var objekt = "Datum: ";
            foreach (var d in anfrage.Wochen)
            {
                objekt += d.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) + ", ";
            }
            objekt += Environment.NewLine + "Redner: ";
            foreach (var r in anfrage.RednerVortrag)
            {
                objekt += $"{r.Key.Name} | {r.Value.NumberTopicShort}{Environment.NewLine}";
            }

            var redner = (anfrage.RednerVortrag.Keys.Distinct().Count() == 1) ? anfrage.RednerVortrag.Keys.ElementAt(0) : null;

            var log = new Activity
            {
                Typ = Types.RednerAnfragen,
                Versammlung = anfrage.Versammlung,
                Objekt = objekt,
                Mails = anfrage.Mailtext,
                Redner = redner,
            };

            AddActivity(log);
        }

        private const string _mailDelimiter = "\r\n=========================\r\n";
    }
}