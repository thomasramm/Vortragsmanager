using System;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Vortragsmanager.Helper;
using Vortragsmanager.Interface;

namespace Vortragsmanager.PageModels
{
    public class MonthViewModel : ViewModelBase
    {
        private readonly INavigation _parentModel;

        public MonthViewModel(INavigation parentModel, int nr, string name, ObservableCollection<MonthViewModel> monate)
        {
            _parentModel = parentModel;

            Nr = nr;
            Name = name;

            Wochen = new ObservableCollection<WeekViewModel>();
            Monate = monate;
        }

        public int Nr { get; set; }

        public string Name { get; set; }

        public ObservableCollection<WeekViewModel> Wochen { get; }

        public ObservableCollection<MonthViewModel> Monate { get; }

        public void GetWeeks(int jahr)
        {
            Wochen.Clear();
            var startDate = new DateTime(jahr, Nr, 1);
            var endDate = startDate.AddMonths(1);
            while (startDate < endDate)
            {
                if ((int)startDate.DayOfWeek == (int)DateCalcuation.Wochentag)
                {
                    var kw = DateCalcuation.CalculateWeek(startDate);
                    Wochen.Add(new WeekViewModel(_parentModel, jahr, this, kw));
                }
                startDate = startDate.AddDays(1);
            }
        }
    }
}