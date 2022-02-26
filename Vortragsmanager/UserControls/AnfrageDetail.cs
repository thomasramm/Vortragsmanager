using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;

namespace Vortragsmanager.UserControls
{
    public class AnfrageDetail : ViewModelBase
    {
        private readonly Speaker _redner;

        private readonly Talk _vortrag;

        private readonly Anfrage _base;

        public AnfrageDetail(Anfrage basis, Speaker redner, Talk vortrag)
        {
            _base = basis;
            _redner = redner;
            _vortrag = vortrag;
            Wochen = new ObservableCollection<DateTime>(_base.Wochen);
            if (_base != null && Wochen.Count > 0)
                SelectedDatum = Wochen[0];

            SaveCommand = new DelegateCommand(Zusagen);
            CancelCommand = new DelegateCommand(Absagen);
            AlleDatenFreigeben = new DelegateCommand(LadeFreieTermine);
        }

        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand AlleDatenFreigeben { get; private set; }

        public string Name => _redner.Name;

        public string Vortrag => _vortrag.ToString();

        public ObservableCollection<DateTime> Wochen { get; set; }

        private void LadeFreieTermine()
        {
            Log.Info(nameof(LadeFreieTermine));
            _base.Wochen.Clear();

            var startDate = DateCalcuation.CurrentWeek;
            var endDate = DateCalcuation.AddWeek(DateCalcuation.CurrentWeek, 53);
            while (startDate < endDate)
            {
                if (!DataContainer.MeinPlan.Any(x => x.Kw == startDate))
                {
                    var d = DateCalcuation.CalculateWeek(startDate);
                    if (!Wochen.Any(x => x == d))
                        Wochen.Add(d);
                }
                startDate = DateCalcuation.AddWeek(startDate, 1);
            }
        }

        private bool _sichtbar = true;

        public bool Sichtbar
        {
            get
            {
                return _sichtbar;
            }
            set
            {
                _sichtbar = value;
                RaisePropertyChanged();
            }
        }

        private bool _aktiv = true;

        public bool Aktiv
        {
            get
            {
                return _aktiv;
            }
            set
            {
                _aktiv = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _selectedDatum;

        public DateTime SelectedDatum
        {
            get
            {
                return _selectedDatum;
            }
            set
            {
                _selectedDatum = value;
                _selectedKw = (value != null) ? DateCalcuation.CalculateWeek(_selectedDatum) : -1;
                RaisePropertyChanged();
            }
        }

        private int _selectedKw = -1;

        public void Zusagen()
        {
            Log.Info(nameof(Zusagen));
            Sichtbar = false;
            var vortrag = _redner.Vorträge.FirstOrDefault(x => x.Vortrag.Nummer == _vortrag.Nummer);
            if (vortrag == null)
            {
                vortrag = _redner.Vorträge.First();
                ThemedMessageBox.Show("Fehler",
                    $"Der gewählte Vortrag ist für den Redner nicht mehr verfügbar!" + Environment.NewLine +
                    $"Als gewählter Vortrag wurde statt dessen Vortrag #{vortrag.Vortrag.Nummer} ausgewählt." + Environment.NewLine +
                    "Bitte Prüfen und ggfs. korrigieren",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            var i = new Invitation
            {
                Kw = _selectedKw,
                LetzteAktion = DateTime.Today,
                Status = EventStatus.Zugesagt,
                Vortrag = vortrag,
                Ältester = _redner
            };
            DataContainer.MeinPlanAdd(i);

            _base.BaseAnfrage.RednerVortrag.Remove(_redner);
            _base.Wochen.Remove(SelectedDatum);
            bool anfrageGelöscht = false;
            if ((_base.BaseAnfrage.RednerVortrag.Count == 0) || _base.Wochen.Count == 0)
            {
                DataContainer.OffeneAnfragen.Remove(_base.BaseAnfrage);
                anfrageGelöscht = true;
            }

            ActivityLog.ActivityAddItem.RednerAnfrageZugesagt(i, _base.BaseAnfrage.Mailtext, anfrageGelöscht);
        }

        public void Absagen()
        {
            Log.Info(nameof(Absagen));
            Sichtbar = false;
            var vortrag = _base.BaseAnfrage.RednerVortrag[_redner];
            _base.BaseAnfrage.RednerVortrag.Remove(_redner);
            string wochen = string.Empty;
            foreach (var w in _base.BaseAnfrage.Kws)
            {
                DataContainer.Absagen.Add(new Cancelation(w, _redner, EventStatus.Anfrage));
                wochen += DateCalcuation.CalculateWeek(w).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) + ", ";
            }
            bool anfrageGelöscht = false;
            if (_base.BaseAnfrage.RednerVortrag.Count == 0)
            {
                DataContainer.OffeneAnfragen.Remove(_base.BaseAnfrage);
                anfrageGelöscht = true;
            }

            ActivityLog.ActivityAddItem.RednerAnfrageAbgelehnt(_redner, vortrag, wochen, _base.BaseAnfrage.Mailtext, anfrageGelöscht);
        }
    }
}