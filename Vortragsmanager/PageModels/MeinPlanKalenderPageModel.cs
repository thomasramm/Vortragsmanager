using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;

namespace Vortragsmanager.PageModels
{
    public class MeinPlanKalenderPageModel : ViewModelBase
    {
        public MeinPlanKalenderPageModel(INavigation parentModel)
        {
            ChangeYear = new DelegateCommand<int>(ChangeCurrentYear);
            Monate = new ObservableCollection<MonthViewModel>();
            Monate.Add(new MonthViewModel(parentModel, 1, "Januar", Monate));
            Monate.Add(new MonthViewModel(parentModel, 2, "Februar", Monate));
            Monate.Add(new MonthViewModel(parentModel, 3, "März", Monate));
            Monate.Add(new MonthViewModel(parentModel, 4, "April", Monate));
            Monate.Add(new MonthViewModel(parentModel, 5, "Mai", Monate));
            Monate.Add(new MonthViewModel(parentModel, 6, "Juni", Monate));
            Monate.Add(new MonthViewModel(parentModel, 7, "Juli", Monate));
            Monate.Add(new MonthViewModel(parentModel, 8, "August", Monate));
            Monate.Add(new MonthViewModel(parentModel, 9, "September", Monate));
            Monate.Add(new MonthViewModel(parentModel, 10, "Oktober", Monate));
            Monate.Add(new MonthViewModel(parentModel, 11, "November", Monate));
            Monate.Add(new MonthViewModel(parentModel, 12, "Dezember", Monate));

            Messenger.Default.Register<int>(this, Messages.DisplayYearChanged, OnMessage);
            UpdateMonate();
        }

        public ObservableCollection<MonthViewModel> Monate { get; }

        public DelegateCommand<int> ChangeYear { get; }

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