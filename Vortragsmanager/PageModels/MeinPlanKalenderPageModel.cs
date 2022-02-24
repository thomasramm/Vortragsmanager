using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Vortragsmanager.Enums;

namespace Vortragsmanager.PageModels
{
    public class MeinPlanKalenderPageModel : ViewModelBase
    {
        public MeinPlanKalenderPageModel()
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
        public int CurrentYear => Helper.Helper.DisplayedYear;

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
            Helper.Helper.DisplayedYear += step;
            RaisePropertyChanged(nameof(CurrentYear));
        }
    }

    //ToDo: Detailansicht bei Klick/Doppelklick zur Vortragsbuchung
}