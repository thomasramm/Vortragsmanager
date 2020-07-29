using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;

namespace Vortragsmanager.MeinPlan
{
    public class MeinPlanViewModel : ViewModelBase
    {
        public MeinPlanViewModel()
        {
            ChangeYear = new DelegateCommand<int>(ChangeCurrentYear);
            Monate = new ObservableCollection<MonthViewModel>();
            Monate.Add(new MonthViewModel(1, "Januar", Monate));
            Monate.Add(new MonthViewModel(2, "Februar", Monate));
            Monate.Add(new MonthViewModel(3, "März", Monate));
            Monate.Add(new MonthViewModel(4, "April", Monate));
            Monate.Add(new MonthViewModel(5, "Mai", Monate));
            Monate.Add(new MonthViewModel(6, "Juni", Monate));
            Monate.Add(new MonthViewModel(7, "Juli", Monate));
            Monate.Add(new MonthViewModel(8, "August", Monate));
            Monate.Add(new MonthViewModel(9, "September", Monate));
            Monate.Add(new MonthViewModel(10, "Oktober", Monate));
            Monate.Add(new MonthViewModel(11, "November", Monate));
            Monate.Add(new MonthViewModel(12, "Dezember", Monate));

            Messenger.Default.Register<int>(this, Messages.DisplayYearChanged, OnMessage);
            UpdateMonate();
        }

        public ObservableCollection<MonthViewModel> Monate { get; private set; }

        public DelegateCommand<int> ChangeYear { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822")]
        public int CurrentYear => Helper.DisplayedYear;

        public void UpdateMonate()
        {
            foreach (var m in Monate)
            {
                m.Wochen.Clear();
                m.GetWeeks(CurrentYear);
            }
        }

        private void OnMessage(int year)
        {
            RaisePropertyChanged(nameof(CurrentYear));
            UpdateMonate();
        }

        public void ChangeCurrentYear(int step)
        {
            Helper.DisplayedYear += step;
            RaisePropertyChanged(nameof(CurrentYear));
        }
    }

    public class MonthViewModel : ViewModelBase
    {
        public MonthViewModel(int nr, string name, ObservableCollection<MonthViewModel> monate)
        {
            Nr = nr;
            Name = name;

            Wochen = new ObservableCollection<WeekViewModel>();
            Monate = monate;
        }

        public int Nr { get; set; }

        public string Name { get; set; }

        public ObservableCollection<WeekViewModel> Wochen { get; private set; }

        public ObservableCollection<MonthViewModel> Monate { get; private set; }

        public void GetWeeks(int jahr)
        {
            Wochen.Clear();
            var startDate = new DateTime(jahr, Nr, 1);
            var endDate = startDate.AddMonths(1);
            while (startDate < endDate)
            {
                if (startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    Wochen.Add(new WeekViewModel(jahr, this, startDate));
                }
                startDate = startDate.AddDays(1);
            }
        }
    }

    //ToDo: Detailansicht bei Klick/Doppelklick zur Vortragsbuchung
    public class WeekViewModel : ViewModelBase
    {
        public WeekViewModel(int jahr, MonthViewModel monat, DateTime tag)
        {
            Jahr = jahr;
            Monat = monat;
            Tag = tag;
            Zuteilung = DataContainer.MeinPlan?.FirstOrDefault(x => x.Datum == tag) ?? null;
            if (Zuteilung == null)
                Zuteilung = DataContainer.OffeneAnfragen?.FirstOrDefault(x => x.Wochen.Contains(tag)) ?? null;

            AnzahlAuswärtigeRedner = DataContainer.ExternerPlan?.Count(x => x.Datum == tag) ?? 0;

            AnfrageLöschenCommand = new DelegateCommand(AnfrageLöschen);
            BuchungVerschiebenCommand = new DelegateCommand(BuchungVerschieben);
            BuchungLöschenCommand = new DelegateCommand(AnfrageLöschen);
            RednerSuchenCommand = new DelegateCommand(RednerSuchen);
            RednerEintragenCommand = new DelegateCommand(RednerEintragen);
            EreignisEintragenCommand = new DelegateCommand(EreignisEintragen);
            AnfrageBearbeitenCommand = new DelegateCommand(AnfrageBearbeiten);
            BuchungBearbeitenCommand = new DelegateCommand(BuchungBearbeiten);
            BuchungErinnernCommand = new DelegateCommand(BuchungErinnern);
            ClickCommand = new DelegateCommand(OnClick);
            ClosePopupCommand = new DelegateCommand(ClosePopup);
        }

        private void OnClick()
        {
            if (IsAnfrage)
                AnfrageBearbeiten();
            else if (IsOffen)
                RednerSuchen();
            else if (IsEreignis)
                EreignisEintragen();
            else if (IsBuchung)
            {
                DetailView = true;
                RaisePropertyChanged(nameof(DetailView));
            }
        }

        private void ClosePopup()
        {
            DetailView = false;
            RaisePropertyChanged(nameof(DetailView));
        }

        public DelegateCommand ClosePopupCommand { get; private set; }

        public DelegateCommand ClickCommand { get; private set; }

        public DelegateCommand AnfrageLöschenCommand { get; private set; }

        public DelegateCommand EreignisEintragenCommand { get; private set; }

        public DelegateCommand BuchungLöschenCommand { get; private set; }

        public DelegateCommand BuchungVerschiebenCommand { get; private set; }

        public DelegateCommand BuchungBearbeitenCommand { get; private set; }

        public DelegateCommand RednerSuchenCommand { get; private set; }

        public DelegateCommand RednerEintragenCommand { get; private set; }

        public DelegateCommand AnfrageBearbeitenCommand { get; private set; }

        public DelegateCommand BuchungErinnernCommand { get; private set; }

        private void EreignisEintragen()
        {
            var ev = Zuteilung as SpecialEvent;
            var neu = false;
            if (ev == null)
            {
                ev = new SpecialEvent() { Datum = Tag };
                neu = true;
            }
            var dialog = new EreignisEintragenCommandDialog();
            var data = (EreignisEintragenCommandDialogView)dialog.DataContext;
            data.Event = ev;

            dialog.ShowDialog();
            //var data = (AnfrageBestätigenViewModel)dialog.DataContext;
            if (data.Speichern)
            {
                if (neu)
                    DataContainer.MeinPlan.Add(ev);
                Zuteilung = ev;
                Monat.GetWeeks(Jahr);
            }
        }

        private void RednerEintragen()
        {
            var dialog = new RednerEintragenDialog();
            var data = (RednerEintragenView)dialog.DataContext;
            dialog.ShowDialog();
            if (!data.Speichern)
                return;

            var i = new Invitation
            {
                Datum = Tag,
                Status = EventStatus.Zugesagt,
                Ältester = data.SelectedRedner,
                Vortrag = data.SelectedVortrag
            };
            Zuteilung = i;
            DataContainer.MeinPlan.Add(i);
            Monat.GetWeeks(Jahr);
        }

        public void AnfrageLöschen()
        {
            if (Zuteilung.Status == EventStatus.Ereignis)
            {
                DataContainer.MeinPlan.Remove(Zuteilung);
                Monat.GetWeeks(Jahr);
                return;
            }

            var zuteilung = Zuteilung as Invitation;

            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            if (zuteilung.Ältester.Versammlung == DataContainer.MeineVersammlung)
                data.MailTextRedner = Templates.GetMailTextAblehnenRedner(zuteilung);
            else
                data.MailTextKoordinator = Templates.GetMailTextAblehnenKoordinator(zuteilung);

            w.ShowDialog();
            if (!data.Speichern)
                return;

            DataContainer.MeinPlan.Remove(Zuteilung);
            DataContainer.Absagen.Add(new Cancelation(zuteilung.Datum, zuteilung.Ältester, zuteilung.Status));
            Monat.GetWeeks(Jahr);
        }

        public void BuchungVerschieben()
        {
            var verschieben = new KalendereintragVerschieben();
            var data = (KalendereintragVerschiebenView)verschieben.DataContext;
            data.KalenderTyp = Kalenderart.Intern;
            data.LadeStartDatum(Zuteilung);
            verschieben.ShowDialog();

            if (!data.Speichern)
                return;

            //StartBuchung aktualisieren
            Monat.GetWeeks(Jahr);
            //ZielBuchung aktualisieren
            var zielMonatNr = data.ZielDatum.Month;
            if (zielMonatNr != Monat.Nr)
                Monat.Monate.Single(x => x.Nr == zielMonatNr).GetWeeks(Jahr);
        }

        public void BuchungBearbeiten()
        {
            if (Zuteilung.Status == EventStatus.Ereignis)
            {
                EreignisEintragen();
                return;
            }
            var dialog = new RednerEintragenDialog();
            var data = (RednerEintragenView)dialog.DataContext;
            data.SelectedVersammlung = Einladung.Ältester.Versammlung;
            data.SelectedRedner = Einladung.Ältester;
            data.SelectedVortrag = Einladung.Vortrag;
            dialog.ShowDialog();
            if (!data.Speichern)
                return;

            Einladung.Ältester = data.SelectedRedner;
            Einladung.Vortrag = data.SelectedVortrag;
            Monat.GetWeeks(Jahr);
        }

        public void AnfrageBearbeiten()
        {
            var dev = new AntwortEintragenDialog();
            var data = (AntwortEintragenViewModel)dev.Control.DataContext;
            data.LoadData(Zuteilung as Inquiry);
            dev.ShowDialog();
            Messenger.Default.Send(0, Messages.DisplayYearChanged);
        }

        public void RednerSuchen()
        {
            Navigation.NavigationView.Frame.Navigate("SearchSpeaker", Tag);
        }

        public void BuchungErinnern()
        {
            var mail = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)mail.DataContext;
            data.MailTextKoordinator = Templates.GetMailTextRednerErinnerung(Zuteilung as Invitation);
            data.DisableCancelButton();
            mail.ShowDialog();
        }

        public IEvent Zuteilung { get; set; }

        public Invitation Einladung => Zuteilung as Invitation;

        public int Jahr { get; }

        public MonthViewModel Monat { get; }

        public DateTime Tag { get; set; }

        public SolidColorBrush Background
        {
            get
            {
                var color = Color.FromRgb(51, 51, 51);
                if (Zuteilung == null)
                    color = Colors.Tomato;
                else if (Zuteilung.Status == EventStatus.Anfrage)
                    color = Colors.Orange;
                else if (Zuteilung.Status == EventStatus.Ereignis)
                    color = Colors.SlateGray;
                return new SolidColorBrush(color);
            }
        }

        public int AnzahlAuswärtigeRedner
        {
            get { return GetProperty(() => AnzahlAuswärtigeRedner); }
            set { SetProperty(() => AnzahlAuswärtigeRedner, value); }
        }

        public string Anzeigetext
        {
            get
            {
                if (Zuteilung == null)
                    return "offen";
                return Zuteilung.Anzeigetext;
            }
        }

        public bool IsAnfrage => Zuteilung?.Status == EventStatus.Anfrage;

        public bool IsBuchung => Zuteilung?.Status == EventStatus.Zugesagt || Zuteilung?.Status == EventStatus.Ereignis;

        public bool IsEinladung => Zuteilung?.Status == EventStatus.Zugesagt;

        public bool IsEreignis => Zuteilung?.Status == EventStatus.Ereignis;

        public bool IsOffen => Zuteilung == null;

        public bool DetailView { get; set; } = false;

        public override string ToString()
        {
            return $"{Tag.Day:00}";
        }
    }
}