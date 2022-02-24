using DevExpress.Mvvm;
using System.Windows;
using Vortragsmanager.Core;
using Vortragsmanager.Helper;

namespace Vortragsmanager.Navigation
{
    public class DashboardViewModel : ViewModelBase
    {
        public int AktuelleWoche => DateCalcuation.CurrentWeek;

        public int Woche2 => DateCalcuation.AddWeek(DateCalcuation.CurrentWeek, 1);
        
        public int Woche3 => DateCalcuation.AddWeek(DateCalcuation.CurrentWeek, 2);

        public int Woche4 => DateCalcuation.AddWeek(DateCalcuation.CurrentWeek, 3);
    }
}