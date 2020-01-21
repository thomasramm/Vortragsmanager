using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using Vortragsmanager.Models;
using System.Linq;

namespace Vortragsmanager.Views
{
    public class AntwortEintragenViewModel : ViewModelBase
    {
        public void LoadData()
        {
            inquiryList = Core.DataContainer.OffeneAnfragen;
            LoadInquiryUI();
        }

        public void LoadData(Inquiry Anfrage)
        {
            inquiryList.Clear();
            inquiryList.Add(Anfrage);
            LoadInquiryUI();
        }

        private void LoadInquiryUI()
        {
            Anfragen.Clear();
            foreach (var a in inquiryList)
            {
                Anfragen.Add(new Anfrage(a));
            }
        }

        private ObservableCollection<Inquiry> inquiryList = new ObservableCollection<Inquiry>();

        public ObservableCollection<Anfrage> Anfragen { get; } = new ObservableCollection<Anfrage>();
    }

    public class Anfrage
    {
        public Inquiry BaseAnfrage { get; set; }

        public Anfrage(Inquiry inquiry)
        {
            BaseAnfrage = inquiry;
            if (inquiry is null)
                return;

            foreach (var x in BaseAnfrage.RednerVortrag)
            {
                Redner.Add(new AnfrageDetail(this, x.Key, x.Value));
            }
        }

        public ObservableCollection<DateTime> Wochen => BaseAnfrage.Wochen;

        public string Versammlung => BaseAnfrage.Versammlung.Name;

        public ObservableCollection<AnfrageDetail> Redner { get; private set; } = new ObservableCollection<AnfrageDetail>();
    }

    public class AnfrageDetail : ViewModelBase
    {
        private Speaker _redner;

        private Talk _vortrag;

        private Anfrage _base;

        public AnfrageDetail(Anfrage Base, Speaker Redner, Talk Vortrag)
        {
            _base = Base;
            _redner = Redner;
            _vortrag = Vortrag;

            SaveCommand = new DelegateCommand(Zusagen);
            CancelCommand = new DelegateCommand(Absagen);
            AlleDatenFreigeben = new DelegateCommand(LadeFreieTermine);
        }

        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand AlleDatenFreigeben { get; private set; }

        public string Name => _redner.Name;

        public string Vortrag => _vortrag.ToString();

        public ObservableCollection<DateTime> Wochen => _base.Wochen;

        private void LadeFreieTermine()
        {
            _base.Wochen.Clear();
            var startDate = Core.Helper.GetSunday(DateTime.Today);
            var endDate = startDate.AddYears(1);
            while (startDate < endDate)
            {
                if (!Core.DataContainer.MeinPlan.Any(x => x.Datum == startDate))
                    _base.Wochen.Add(startDate);
                startDate = startDate.AddDays(7);
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
                RaisePropertyChanged();
            }
        }

        public void Zusagen()
        {
            Sichtbar = false;
            var i = new Invitation();
            i.Datum = SelectedDatum;
            i.LetzteAktion = DateTime.Today;
            i.Status = EventStatus.Zugesagt;
            i.Vortrag = _vortrag;
            i.Ältester = _redner;
            Core.DataContainer.MeinPlan.Add(i);
            _base.BaseAnfrage.RednerVortrag.Remove(_redner);
            _base.Wochen.Remove(SelectedDatum);
            if ((_base.BaseAnfrage.RednerVortrag.Count == 0) || _base.Wochen.Count == 0)
                Core.DataContainer.OffeneAnfragen.Remove(_base.BaseAnfrage);
        }

        public void Absagen()
        {
            Sichtbar = false;
            _base.BaseAnfrage.RednerVortrag.Remove(_redner);
            foreach (var w in _base.BaseAnfrage.Wochen)
            {
                Core.DataContainer.Absagen.Add(new Cancelation(w, _redner, EventStatus.Anfrage));
            }
            if (_base.BaseAnfrage.RednerVortrag.Count == 0)
                Core.DataContainer.OffeneAnfragen.Remove(_base.BaseAnfrage);
        }
    }
}