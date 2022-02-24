using System;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Vortragsmanager.Helper;

namespace Vortragsmanager.PageModels
{
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
                if ((int)startDate.DayOfWeek == (int)DateCalcuation.Wochentag)
                {
                    var kw = DateCalcuation.CalculateWeek(startDate);
                    Wochen.Add(new WeekViewModel(jahr, this, kw));
                }
                startDate = startDate.AddDays(1);
            }
        }
    }
}