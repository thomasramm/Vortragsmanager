using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.Helper;
using Vortragsmanager.Module;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.PageModels
{
    class MeinPlanVorsitzUndLeserPageModel : ViewModelBase
    {
        private DateTime _month;
        private readonly DateTime _currentMonth;

        public MeinPlanVorsitzUndLeserPageModel()
        {
            _currentMonth = DateTime.Today;
            _currentMonth = _currentMonth.AddDays((_currentMonth.Day - 1) * -1);
            Monat = _currentMonth;
            ChangeMonth = new DelegateCommand<int>(ChangeTheMonth);
            PlanAusgeben = new DelegateCommand(PlanErstellen);
            SonntagCalculateCommand = new DelegateCommand(SonntagCalculate);

            EinstellungenButtonIsChecked = false;
        }

        public GridLength HauptseiteWidth { get; set; }

        public GridLength EinstellungenWidth { get; set; }

        private bool _einstellungenButtonIsChecked;
        public bool EinstellungenButtonIsChecked
        {
            get => _einstellungenButtonIsChecked;
            set
            {
                _einstellungenButtonIsChecked = value;
                if (value)
                {
                    EinstellungenWidth = new GridLength(1, GridUnitType.Star);
                    HauptseiteWidth = new GridLength(0);
                }
                else
                {
                    HauptseiteWidth = new GridLength(1, GridUnitType.Star);
                    EinstellungenWidth = new GridLength(0);
                    RefreshLastActivity();
                    WochenLoad();
                }

                RaisePropertyChanged();
                RaisePropertiesChanged(nameof(HauptseiteWidth));
                RaisePropertiesChanged(nameof(EinstellungenWidth));
                RaisePropertyChanged(nameof(MenuHeaderTitel));
            }
        }

        public string MenuHeaderTitel => _einstellungenButtonIsChecked ? "Mein Plan > Vorsitz & WT Leser > Einstellungen" : "Mein Plan > Vorsitz & WT Leser";

        public DateTime Monat
        {
            get => _month;
            set
            {
                _month = value;
                RaisePropertyChanged();
                WochenLoad();
            }
        }

        public DelegateCommand<int> ChangeMonth { get; }

        private void ChangeTheMonth(int direction)
        {
            Monat = direction != 0 ? Monat.AddMonths(direction) : _currentMonth;
        }

        public ObservableCollection<SonntagItem> Wochen { get; } = new ObservableCollection<SonntagItem>();

        private static void RefreshLastActivity()
        {
            foreach (var person in DataContainer.AufgabenPersonZuordnung)
            {
                int kwLetzterEinsatz;

                var a1 = DataContainer.AufgabenPersonKalender.Where(x => x.Leser == person || x.Vorsitz == person).ToList();
                if (a1.Any())
                {
                    var a2 = a1.Select(x => x.Kw);
                    kwLetzterEinsatz = a2.Max();
                }
                else
                    kwLetzterEinsatz = -1;

                var wochen = (kwLetzterEinsatz);
                person.LetzterEinsatz = wochen + person.Häufigkeit;
            }
        }

        private void WochenLoad()
        {
            Wochen.Clear();
            var tag = DateCalcuation.GetConregationDay(Monat);
            var nMonat = Monat.AddMonths(Helper.Helper.GlobalSettings.SonntagAnzeigeMonate);
            while (tag < nMonat)
            {
                Wochen.Add(new SonntagItem(tag));
                tag = tag.AddDays(7);
            }
        }

        private void PlanErstellen()
        {
            IoExcel.Export.Aushang(true, DateTime.Today);
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

            foreach (var woche in Wochen.OrderBy(x => x.Datum))
            {
                //Vorsitz
                if (woche.SelectedVorsitz == null && woche.IsVorsitz)
                {
                    var nextPerson = woche.Vorsitz.Where(x => x.Häufigkeit > 1).OrderBy(x => x.LetzterEinsatz).First();
                    woche.SelectedVorsitz = nextPerson;
                }

                if (woche.SelectedLeser == null && woche.IsLeser)
                {
                    var person = woche.Leser.Where(x => x.Häufigkeit > 1).OrderBy(x => x.LetzterEinsatz).First();
                    woche.SelectedLeser = person;
                }
            }
        }

        public DelegateCommand PlanAusgeben { get; }

        public DelegateCommand SonntagCalculateCommand { get; }
    }
}
