using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.PageModels
{
    public class HistorieAktivitätenPageModel : ViewModelBase
    {
        private static List<string> _listeAllerVersammlungen;
        private static int _nextId;

        public HistorieAktivitätenPageModel()
        {
            Initialize();
            Messenger.Default.Register<ActivityItemViewModel>(this, Messages.ActivityAdd, OnNewLog);
            Messenger.Default.Register<bool>(this, Messages.NewDatabaseOpened, OnOpenDatabase);
        }

        private void SetFilter()
        {
            foreach (var a in _alleUnfiltered)
            {
                if (_filterAktivität == "Alle"
                    || (_filterAktivität == "Meine Redner"
                        && (a.Typ == ActivityTypes.ExterneAnfrageAblehnen
                            || a.Typ == ActivityTypes.ExterneAnfrageBestätigen
                            || a.Typ == ActivityTypes.ExterneAnfrageListeSenden
                        ))
                    || (_filterAktivität == "Mein Plan"
                        && (a.Typ == ActivityTypes.RednerAnfragen
                            || a.Typ == ActivityTypes.RednerAnfrageBestätigt
                            || a.Typ == ActivityTypes.RednerAnfrageAbgesagt
                            || a.Typ == ActivityTypes.BuchungLöschen
                            || a.Typ == ActivityTypes.BuchungVerschieben
                            || a.Typ == ActivityTypes.EreignisAnlegen
                            || a.Typ == ActivityTypes.EreignisBearbeiten
                            || a.Typ == ActivityTypes.EreignisLöschen
                            || a.Typ == ActivityTypes.RednerBearbeiten
                            || a.Typ == ActivityTypes.RednerEintragen
                            || a.Typ == ActivityTypes.RednerErinnern
                        ))
                    || (_filterAktivität == "Sonstige"
                        && (a.Typ == ActivityTypes.Sonstige
                            || a.Typ == ActivityTypes.SendMail
                            || a.Typ == ActivityTypes.RednerErinnern
                            || a.Typ == ActivityTypes.ExterneAnfrageListeSenden
                        ))
                   )
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

            Heute.Clear();
            DieseWoche.Clear();
            DieserMonat.Clear();
            LetzterMonat.Clear();
            DiesesJahr.Clear();
            Älter.Clear();
            foreach (var item in _alleUnfiltered.Where(x => x.Aktiv))
            {
                switch (item.Zeitraum)
                {
                    case ActivityTime.Heute:
                        Heute.Add(item);
                        break;
                    case ActivityTime.DieseWoche:
                        DieseWoche.Add(item);
                        break;
                    case ActivityTime.DieserMonat:
                        DieserMonat.Add(item);
                        break;
                    case ActivityTime.LetzterMonat:
                        LetzterMonat.Add(item);
                        break;
                    case ActivityTime.DiesesJahr:
                        DiesesJahr.Add(item);
                        break;
                    case ActivityTime.Älter:
                        Älter.Add(item);
                        break;
                }
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
            var items = _listeAllerVersammlungen.Count;
            var newCount = 0;
            var maxCount = (versammlungFilter == null) ? items : 10;
            for (int i = 0; i < items; i++)
            {
                if (string.IsNullOrEmpty(versammlungFilter) || (Regex.IsMatch(_listeAllerVersammlungen[i], Regex.Escape(versammlungFilter), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)))
                {
                    ListeFilteredVersammlungen.Add(_listeAllerVersammlungen[i]);
                    newCount++;
                    if (newCount == maxCount)
                        break;
                }
            }
        }

        public void Initialize()
        {
            _alleUnfiltered.Clear();

            _nextId = DataContainer.Aktivitäten.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;

            _listeAllerVersammlungen = DataContainer.Versammlungen.OrderBy(x => x, new Helper.EigeneKreisNameComparer()).Select(x => x.NameMitKoordinator).ToList();
            _listeAllerVersammlungen.Insert(0, "Alle");

            ListeFilteredVersammlungen.Clear();
            foreach (var vers in _listeAllerVersammlungen)
            {
                ListeFilteredVersammlungen.Add(vers);
            }

            var week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            foreach (var a in DataContainer.Aktivitäten.OrderByDescending(x => x.Datum))
            {
                var item = new Item(a);
                _alleUnfiltered.Add(item);

                //Zeitraum festlegen
                if (a.Datum.Date == DateTime.Today)
                    item.Zeitraum = ActivityTime.Heute;
                else if (a.Datum.Year == DateTime.Today.Year)
                {
                    if (week == CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(a.Datum, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                        item.Zeitraum = ActivityTime.DieseWoche;
                    else if (a.Datum.Month == DateTime.Today.Month)
                        item.Zeitraum = ActivityTime.DieserMonat;
                    else if (a.Datum.Month == ((DateTime.Today.Month == 1) ? 12 : DateTime.Today.Month - 1))
                        item.Zeitraum = ActivityTime.LetzterMonat;
                    else
                        item.Zeitraum = ActivityTime.DiesesJahr;
                }
                else
                    item.Zeitraum = ActivityTime.Älter;
            }

            FilterVersammlung = string.Empty;
        }

        public ObservableCollection<string> ListeFilteredVersammlungen { get; } = new ObservableCollection<string>();

        private readonly List<Item> _alleUnfiltered = new List<Item>();

        public ObservableCollection<Item> Heute { get; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> DieseWoche { get; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> DieserMonat { get; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> LetzterMonat { get; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> DiesesJahr { get; } = new ObservableCollection<Item>();

        public ObservableCollection<Item> Älter { get; } = new ObservableCollection<Item>();

        public string HeuteHeader => $"Heute ({Heute.Count(x => x.Aktiv)})";
        public string DieseWocheHeader => $"Diese Woche ({DieseWoche.Count(x => x.Aktiv)})";
        public string DieserMonatHeader => $"{DateTime.Today:MMMM} ({DieserMonat.Count(x => x.Aktiv)})";
        public string LetzterMonatHeader => $"Letzter Monat ({LetzterMonat.Count(x => x.Aktiv)})";
        public string DiesesJahrHeader => $"Dieses Jahr ({DiesesJahr.Count(x => x.Aktiv)})";
        public string ÄlterHeader => $"Älter ({Älter.Count(x => x.Aktiv)})";

        private void OnNewLog(ActivityItemViewModel message)
        {
            message.Id = _nextId++;
            DataContainer.Aktivitäten.Add(message);

            var item = new Item(message)
            {
                Zeitraum = ActivityTime.Heute
            };
            Heute.Insert(0, item);
            _alleUnfiltered.Add(item);
            RaisePropertyChanged(nameof(HeuteHeader));
        }

        private void OnOpenDatabase(bool opened)
        {
            Initialize();
        }
    }
}