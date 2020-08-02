using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
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
        private List<string> _listeAllerVersammlungen;

        public Activities()
        {
            Messenger.Default.Register<Activity>(this, Messages.ActivityAdd, OnNewLog);
            _listeAllerVersammlungen = DataContainer.Versammlungen.OrderBy(x => x, new Helper.EigeneKreisNameComparer()).Select(x => x.NameMitKoordinator).ToList();
            _listeAllerVersammlungen.Insert(0, "Alle");
            ListeFilteredVersammlungen = new ObservableCollection<string>(_listeAllerVersammlungen);

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

        private void SetFilter()
        {
            foreach (var a in Alle)
            {
                if (_filterAktivität == "Alle"
                    || (_filterAktivität == "Meine Redner"
                        && (a.Typ == Types.ExterneAnfrageAblehnen
                        || a.Typ == Types.ExterneAnfrageBestätigen
                        || a.Typ == Types.ExterneAnfrageListeSenden))
                    || (_filterAktivität == "Mein Plan"
                        && (a.Typ == Types.RednerAnfragen
                        || a.Typ == Types.RednerAnfrageBestätigt
                        || a.Typ == Types.RednerAnfrageAbgesagt
                        || a.Typ == Types.BuchungLöschen))
                    || (_filterAktivität == "Sonstige"
                        && (a.Typ == Types.Sonstige
                        || a.Typ == Types.SendMail
                        || a.Typ == Types.ExterneAnfrageListeSenden)))
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
    }
}