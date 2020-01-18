using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class MeinPlanViewModel : ViewModelBase
    {
        public MeinPlanViewModel()
        {
            ChangeYear = new DelegateCommand<int>(ChangeCurrentYear);
            Monate = new ObservableCollection<MonthViewModel>
            {
                new MonthViewModel(1, "Januar"),
                new MonthViewModel(2, "Februar"),
                new MonthViewModel(3, "März"),
                new MonthViewModel(4, "April"),
                new MonthViewModel(5, "Mai"),
                new MonthViewModel(6, "Juni"),
                new MonthViewModel(7, "Juli"),
                new MonthViewModel(8, "August"),
                new MonthViewModel(9, "September"),
                new MonthViewModel(10, "Oktober"),
                new MonthViewModel(11, "November"),
                new MonthViewModel(12, "Dezember"),
            };
            Messenger.Default.Register<Messages>(this, OnMessage);
            UpdateMonate();
        }

        public ObservableCollection<MonthViewModel> Monate { get; private set; }

        public DelegateCommand<int> ChangeYear { get; private set; }

        public int CurrentYear => Core.DataContainer.DisplayedYear;

        public void UpdateMonate()
        {
            foreach (var m in Monate)
            {
                m.Wochen.Clear();
                m.GetWeeks(CurrentYear);
            }
        }

        private void OnMessage(Messages message)
        {
            switch (message)
            {
                case Messages.DisplayYearChanged:
                    RaisePropertyChanged(nameof(CurrentYear));
                    UpdateMonate();
                    break;

                default:
                    break;
            }
        }

        public void ChangeCurrentYear(int step)
        {
            Core.DataContainer.DisplayedYear += step;
            RaisePropertyChanged(nameof(CurrentYear));
        }
    }

    public class MonthViewModel : ViewModelBase
    {
        public MonthViewModel(int nr, string name)
        {
            Nr = nr;
            Name = name;

            Wochen = new ObservableCollection<WeekViewModel>();
        }

        public int Nr { get; set; }

        public string Name { get; set; }

        public ObservableCollection<WeekViewModel> Wochen { get; private set; }

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
            Zuteilung = Core.DataContainer.MeinPlan?.FirstOrDefault(x => x.Datum == tag) ?? null;
            if (Zuteilung == null)
                Zuteilung = Core.DataContainer.OffeneAnfragen?.FirstOrDefault(x => x.Wochen.Contains(tag)) ?? null;

            AnzahlAuswärtigeRedner = Core.DataContainer.ExternerPlan?.Count(x => x.Datum == tag) ?? 0;

            AnfrageLöschenCommand = new DelegateCommand(AnfrageLöschen);
            BuchungVerschiebenCommand = new DelegateCommand(BuchungVerschieben);
            BuchungLöschenCommand = new DelegateCommand(AnfrageLöschen);
            RednerSuchenCommand = new DelegateCommand(RednerSuchen);
            EreignisEintragenCommand = new DelegateCommand(EreignisEintragen);
            AnfrageBearbeitenCommand = new DelegateCommand(AnfrageBearbeiten);
        }

        public DelegateCommand AnfrageLöschenCommand { get; private set; }

        public DelegateCommand EreignisEintragenCommand { get; private set; }

        public DelegateCommand BuchungLöschenCommand { get; private set; }

        public DelegateCommand BuchungVerschiebenCommand { get; private set; }

        public DelegateCommand RednerSuchenCommand { get; private set; }

        public DelegateCommand AnfrageBearbeitenCommand { get; private set; }

        private void EreignisEintragen()
        {
            var ev = (Zuteilung as SpecialEvent);
            var neu = false;
            if (ev == null)
            {
                ev = new SpecialEvent() { Datum = Tag };
                neu = true;
            }
            var dialog = new EreignisEintragenCommandDialog();
            var data = (EreignisEintragenCommandDialogView)(dialog.DataContext);
            data.Event = ev;

            dialog.ShowDialog();
            //var data = (AnfrageBestätigenViewModel)dialog.DataContext;
            if (data.Speichern)
            {
                if (neu)
                    Core.DataContainer.MeinPlan.Add(ev);
                Zuteilung = ev;
                Monat.GetWeeks(Jahr);
            }
        }

        public void AnfrageLöschen()
        {
            if (Zuteilung.Status == EventStatus.Ereignis)
            {
                Core.DataContainer.MeinPlan.Remove(Zuteilung);
                Monat.GetWeeks(Jahr);
                return;
            }

            var zuteilung = (Zuteilung as Invitation);

            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            if (zuteilung.Ältester.Versammlung == Core.DataContainer.MeineVersammlung)
                data.MailTextRedner = Core.Templates.GetMailTextAblehnenRedner(zuteilung);
            else
                data.MailTextKoordinator = Core.Templates.GetMailTextAblehnenKoordinator(zuteilung);

            w.ShowDialog();
            if (data.Speichern)
            {
                Core.DataContainer.MeinPlan.Remove(Zuteilung);
                Monat.GetWeeks(Jahr);
            }
        }

        public void BuchungVerschieben()
        {
            var verschieben = new KalendereintragVerschieben();
            var data = (KalendereintragVerschiebenView)verschieben.DataContext;
            data.KalenderTyp = Kalenderart.Intern;
            data.LadeStartDatum(Zuteilung);
            verschieben.ShowDialog();

            if (data.Speichern)
            {
                Monat.GetWeeks(Jahr);
            }
        }

        public void AnfrageBearbeiten()
        {
            var dev = new AntwortEintragenDialog();
            var data = (AntwortEintragenViewModel)dev.Control.DataContext;
            data.LoadData(Zuteilung as Inquiry);
            dev.ShowDialog();
            Messenger.Default.Send(Messages.DisplayYearChanged);
        }

        public void RednerSuchen()
        {
            //ToDo: Redner suchen aus "MeinPlan" heraus
            //throw new NotImplementedException();
            //var dev = new AntwortEintragenDialog();
            //dev.ShowDialog();

            Navigation.NavigationView.Frame.Navigate("SearchSpeaker", Tag);
        }

        public IEvent Zuteilung { get; set; }

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

        public bool IsEreignis => Zuteilung?.Status == EventStatus.Ereignis;

        public bool IsOffen => (Zuteilung == null);

        public override string ToString()
        {
            return $"{Tag.Day:00}";
        }
    }
}