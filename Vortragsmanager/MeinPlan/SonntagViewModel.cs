using DevExpress.Mvvm;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.MeinPlan
{
    class SonntagViewModel : ViewModelBase
    {
        private DateTime _month;
        private readonly DateTime _currentMonth;

        public SonntagViewModel()
        {
            _currentMonth = DateTime.Today;
            _currentMonth = _currentMonth.AddDays((_currentMonth.Day - 1) * -1);
            Monat = _currentMonth;
            ChangeMonth = new DelegateCommand<int>(ChangeTheMonth);
            Einstellungen = new DelegateCommand(OpenEinstellungen);
            Hauptseite = new DelegateCommand(OpenHauptseite);
            PlanAusgeben = new DelegateCommand(PlanErstellen);
            SonntagCalculateCommand = new DelegateCommand(SonntagCalculate);

            OpenHauptseite();
        }

        public GridLength HauptseiteWidth { get; set; }

        public GridLength EinstellungenWidth { get; set; }

        private void OpenEinstellungen() 
        {
            EinstellungenWidth = new GridLength(1, GridUnitType.Star);
            HauptseiteWidth = new GridLength(0);
            RaisePropertiesChanged(nameof(HauptseiteWidth));
            RaisePropertiesChanged(nameof(EinstellungenWidth));
        }

        private void OpenHauptseite()
        {
            HauptseiteWidth = new GridLength(1, GridUnitType.Star);
            EinstellungenWidth = new GridLength(0);
            RaisePropertiesChanged(nameof(HauptseiteWidth));
            RaisePropertiesChanged(nameof(EinstellungenWidth));
            RefreshLastActivity();
            WochenLoad();
        }

        public DateTime Monat
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
                RaisePropertyChanged();
                WochenLoad();
            }
        }

        public DelegateCommand<int> ChangeMonth { get; private set; }

        private void ChangeTheMonth(int direction)
        {
            if (direction != 0)
            {
                Monat = Monat.AddMonths(direction);
            }
            else
            {
                Monat = _currentMonth;
            }
        }

        public ObservableCollection<SonntagItem> Wochen { get; private set; } = new ObservableCollection<SonntagItem>();

        private static void RefreshLastActivity()
        {
            foreach (var person in DataContainer.AufgabenPersonZuordnung)
            {
                int kwLetzterEinsatz;

                var a1 = DataContainer.AufgabenPersonKalender.Where(x => x.Leser == person || x.Vorsitz == person);
                if (a1.Any())
                {
                    var a2 = a1?.Select(x => x.Kw);
                    kwLetzterEinsatz = a2.Max();
                }
                else
                    kwLetzterEinsatz = -1;

                var wochen = (Helper.CurrentWeek - kwLetzterEinsatz);
                person.LetzterEinsatz = wochen * person.Häufigkeit;
            }
        }

        private void WochenLoad()
        {
            Wochen.Clear();
            var tag = Core.Helper.GetConregationDay(Monat);
            var nMonat = Monat.AddMonths(Properties.Settings.Default.SonntagAnzeigeMonate);
            while (tag < nMonat)
            {
                Wochen.Add(new SonntagItem(tag));
                tag = tag.AddDays(7);
            }
        }

        private void PlanErstellen()
        {
            Core.IoExcel.CreateReportAushang(true);
        }

        private void SonntagCalculate()
        {
            if (!DataContainer.AufgabenPersonZuordnung.Any())
            {
                System.Windows.Forms.MessageBox.Show("Bitte zuerst Personen für Vorsitz + Lesen anlegen");
                return;
            }
            if (!DataContainer.AufgabenPersonZuordnung.Any(x => x.IsVorsitz))
            {
                System.Windows.Forms.MessageBox.Show("Bitte zuerst Personen für Funktion Vorsitz anlegen");
                return;
            }
            if (!DataContainer.AufgabenPersonZuordnung.Any(x => x.IsLeser))
            {
                System.Windows.Forms.MessageBox.Show("Bitte zuerst Personen Funktion Lesen anlegen");
                return;
            }

            foreach(var woche in Wochen.OrderBy(x => x.Datum))
            {
                //Vorsitz
                if (woche.SelectedVorsitz == null && woche.IsVorsitz)
                {
                    var nextPerson = woche.Vorsitz.OrderByDescending(x => x.LetzterEinsatz).First();
                    woche.SelectedVorsitz = nextPerson;
                }

                if (woche.SelectedLeser == null && woche.IsLeser)
                {
                    var person = woche.Leser.OrderByDescending(x => x.LetzterEinsatz).First();
                    woche.SelectedLeser = person;
                }
            }
        }

        public DelegateCommand Einstellungen { get; private set; }

        public DelegateCommand Hauptseite { get; private set; }

        public DelegateCommand PlanAusgeben { get; private set; }

        public DelegateCommand SonntagCalculateCommand { get; private set; }
    }
}
