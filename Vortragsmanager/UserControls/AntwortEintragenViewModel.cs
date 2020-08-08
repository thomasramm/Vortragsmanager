using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.Views
{
    public class AntwortEintragenViewModel : ViewModelBase
    {
        public void LoadData()
        {
            Log.Info(nameof(LoadData));
            inquiryList = DataContainer.OffeneAnfragen;
            LoadInquiryUI();
        }

        public void LoadData(Inquiry Anfrage)
        {
            Log.Info(nameof(LoadData), "Anfrage=" + Anfrage?.Id);
            inquiryList.Clear();
            inquiryList.Add(Anfrage);
            LoadInquiryUI();
        }

        private void LoadInquiryUI()
        {
            Log.Info(nameof(LoadInquiryUI));
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

            MailtextAnzeigenCommand = new DelegateCommand(MailtextAnzeigen);

            foreach (var x in BaseAnfrage.RednerVortrag)
            {
                Redner.Add(new AnfrageDetail(this, x.Key, x.Value));
            }
        }

        public DelegateCommand MailtextAnzeigenCommand { get; private set; }

        public void MailtextAnzeigen()
        {
            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            data.Titel = $"Original Mailtext vom {AnfrageDatum}";
            data.MailTextKoordinator = string.IsNullOrWhiteSpace(BaseAnfrage.Mailtext) ? "kein Mailtext vorhanden!" + Environment.NewLine + BaseAnfrage.Kommentar : BaseAnfrage.Mailtext;
            data.DisableCancelButton();

            w.ShowDialog();
        }

        public ObservableCollection<DateTime> Wochen => BaseAnfrage.Wochen;

        public string Versammlung => BaseAnfrage.Versammlung.Name;

        public string AnfrageDatum => BaseAnfrage.AnfrageDatum.ToString("dd.MM.yyyy", Core.Helper.German);

        public ObservableCollection<AnfrageDetail> Redner { get; private set; } = new ObservableCollection<AnfrageDetail>();
    }

    public class AnfrageDetail : ViewModelBase
    {
        private readonly Speaker _redner;

        private readonly Talk _vortrag;

        private readonly Anfrage _base;

        public AnfrageDetail(Anfrage Base, Speaker Redner, Talk Vortrag)
        {
            _base = Base;
            _redner = Redner;
            _vortrag = Vortrag;
            if (_base != null && _base.Wochen.Count > 0)
                SelectedDatum = _base.Wochen[0];

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
            Log.Info(nameof(LadeFreieTermine));
            _base.Wochen.Clear();
            var startDate = Helper.GetSunday(DateTime.Today);
            var endDate = startDate.AddYears(1);
            while (startDate < endDate)
            {
                if (!DataContainer.MeinPlan.Any(x => x.Datum == startDate))
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
            Log.Info(nameof(Zusagen));
            Sichtbar = false;
            var i = new Invitation
            {
                Datum = SelectedDatum,
                LetzteAktion = DateTime.Today,
                Status = EventStatus.Zugesagt,
                Vortrag = _redner.Vorträge.First(x => x.Vortrag.Nummer == _vortrag.Nummer),
                Ältester = _redner
            };
            DataContainer.MeinPlan.Add(i);
            _base.BaseAnfrage.RednerVortrag.Remove(_redner);
            _base.Wochen.Remove(SelectedDatum);
            bool anfrageGelöscht = false;
            if ((_base.BaseAnfrage.RednerVortrag.Count == 0) || _base.Wochen.Count == 0)
            {
                DataContainer.OffeneAnfragen.Remove(_base.BaseAnfrage);
                anfrageGelöscht = true;
            }

            ActivityLog.AddActivity.RednerAnfrageZugesagt(i, _base.BaseAnfrage.Mailtext, anfrageGelöscht);
        }

        public void Absagen()
        {
            Log.Info(nameof(Absagen));
            Sichtbar = false;
            var vortrag = _base.BaseAnfrage.RednerVortrag[_redner];
            _base.BaseAnfrage.RednerVortrag.Remove(_redner);
            string wochen = string.Empty;
            foreach (var w in _base.BaseAnfrage.Wochen)
            {
                DataContainer.Absagen.Add(new Cancelation(w, _redner, EventStatus.Anfrage));
                wochen += w.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) + ", ";
            }
            bool anfrageGelöscht = false;
            if (_base.BaseAnfrage.RednerVortrag.Count == 0)
            {
                DataContainer.OffeneAnfragen.Remove(_base.BaseAnfrage);
                anfrageGelöscht = true;
            }

            ActivityLog.AddActivity.RednerAnfrageAbgelehnt(_redner, vortrag, wochen, _base.BaseAnfrage.Mailtext, anfrageGelöscht);
        }
    }
}