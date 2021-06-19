using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
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

        private ObservableCollection<DateTime> _wochen = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> Wochen
        {
            get
            {
                _wochen.Clear();
                foreach(var w in BaseAnfrage.Kws)
                {
                    _wochen.Add(Helper.CalculateWeek(w));
                }

                return _wochen;
            }
        }

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

            var startDate = Helper.CurrentWeek;
            var endDate = Helper.AddWeek(Helper.CurrentWeek, 53);
            while (startDate < endDate)
            {
                if (!DataContainer.MeinPlan.Any(x => x.Kw == startDate))
                {
                    var d = Helper.CalculateWeek(startDate);
                    if (!Wochen.Any(x => x == d))
                        Wochen.Add(d);
                }
                startDate = Helper.AddWeek(startDate, 1);
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
                _selectedKw = (value != null) ? Helper.CalculateWeek(_selectedDatum) : -1;
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

            ActivityLog.AddActivity.RednerAnfrageZugesagt(i, _base.BaseAnfrage.Mailtext, anfrageGelöscht);
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
                wochen += Helper.CalculateWeek(w).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) + ", ";
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