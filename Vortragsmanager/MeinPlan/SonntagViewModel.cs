using DevExpress.Mvvm;
using DevExpress.RichEdit.Export;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.MeinPlan
{
    class SonntagViewModel : ViewModelBase
    {
        private DateTime _month;
        private DateTime _currentMonth;

        public SonntagViewModel()
        {
            _currentMonth = DateTime.Today;
            _currentMonth = _currentMonth.AddDays((_currentMonth.Day - 1) * -1);
            Monat = _currentMonth;
            ChangeMonth = new DelegateCommand<int>(ChangeTheMonth);
            Einstellungen = new DelegateCommand(OpenEinstellungen);
            Hauptseite = new DelegateCommand(OpenHauptseite);

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

        private void WochenLoad()
        {
            Wochen.Clear();
            var tag = Core.Helper.GetSunday(Monat);
            var nMonat = Monat.AddMonths(Properties.Settings.Default.SonntagAnzeigeMonate);
            while (tag < nMonat)
            {
                Wochen.Add(new SonntagItem(tag));
                tag = tag.AddDays(7);
            }
        }

        public DelegateCommand Einstellungen { get; private set; }

        public DelegateCommand Hauptseite { get; private set; }
    }
}
