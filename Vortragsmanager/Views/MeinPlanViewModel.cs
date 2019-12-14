using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

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

        public static int CurrentYear
        {
            get
            {
                return Core.DataContainer.DisplayedYear;
            }
        }

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

        public static void ChangeCurrentYear(int step)
        {
            Core.DataContainer.DisplayedYear += step;
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
            AnzahlAuswärtigeRedner = Core.DataContainer.ExternerPlan?.Count(x => x.Datum == tag) ?? 0;

            AnfrageLöschen = new DelegateCommand(EintragLöschen);
            BuchungLöschen = new DelegateCommand(EintragLöschen);
            RednerSuchen = new DelegateCommand(RednerFinden);
        }

        public DelegateCommand AnfrageLöschen { get; private set; }

        public DelegateCommand BuchungLöschen { get; private set; }

        public DelegateCommand RednerSuchen { get; private set; }

        public void EintragLöschen()
        {
            var w = new BuchungLöschenDialog
            {
                DataContext = new BuchungLöschenViewModel(Zuteilung)
            };
            w.ShowDialog();
            var data = (BuchungLöschenViewModel)w.DataContext;
            if (data.Gelöscht)
            {
                Core.DataContainer.MeinPlan.Remove(Zuteilung);
                Monat.GetWeeks(Jahr);
            }
        }

        public void RednerFinden()
        {
            //ToDo: Redner suchen aus "MeinPlan" heraus
            throw new NotImplementedException();
        }

        public Models.Invitation Zuteilung { get; set; }

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
                else if (Zuteilung.Status == Models.InvitationStatus.Anfrage)
                    color = Colors.Orange;
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
                if (Zuteilung.Status == Models.InvitationStatus.Anfrage)
                    return Zuteilung.AnfrageVersammlung.Name;
                if (Zuteilung.Status == Models.InvitationStatus.Zugesagt)
                    return Zuteilung.Ältester?.Name ?? "unbekannt";
                return "FEHLER";
            }
        }

        public bool IsAnfrage => Zuteilung?.Status == Models.InvitationStatus.Anfrage;

        public bool IsBuchung => Zuteilung?.Status == Models.InvitationStatus.Zugesagt;

        public bool IsOffen => (Zuteilung == null);

        public override string ToString()
        {
            return $"{Tag.Day:00}";
        }
    }
}