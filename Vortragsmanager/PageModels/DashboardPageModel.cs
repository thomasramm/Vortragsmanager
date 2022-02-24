using DevExpress.Mvvm;
using Vortragsmanager.Helper;

namespace Vortragsmanager.PageModels
{
    internal class DashboardPageModel : ViewModelBase
    {
        public int AktuelleWoche => DateCalcuation.CurrentWeek;

        public int Woche2 => DateCalcuation.AddWeek(DateCalcuation.CurrentWeek, 1);

        public int Woche3 => DateCalcuation.AddWeek(DateCalcuation.CurrentWeek, 2);

        public int Woche4 => DateCalcuation.AddWeek(DateCalcuation.CurrentWeek, 3);
    }
}
