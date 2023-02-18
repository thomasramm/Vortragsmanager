using DevExpress.Mvvm;
using Vortragsmanager.Helper;

namespace Vortragsmanager.PageModels
{
    internal class DashboardPageModel : ViewModelBase
    {
        public int AktuelleWoche => DateCalcuation.CurrentWeek;

        public static int Woche2 => DateCalcuation.CalculateWeek(DateCalcuation.CurrentWeek, 1);

        public static int Woche3 => DateCalcuation.CalculateWeek(DateCalcuation.CurrentWeek, 2);

        public static int Woche4 => DateCalcuation.CalculateWeek(DateCalcuation.CurrentWeek, 3);
    }
}
