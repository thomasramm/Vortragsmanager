using DevExpress.Mvvm;
using System;
using Vortragsmanager.Helper;

namespace Vortragsmanager.PageModels
{
    internal class DashboardPageModel : ViewModelBase
    {
        public int AktuelleWoche => DateCalcuation.CurrentWeek;

        public static int Woche2 => DateCalcuation.AddWeek(DateTime.Today, 1);

        public static int Woche3 => DateCalcuation.AddWeek(DateTime.Today, 2);

        public static int Woche4 => DateCalcuation.AddWeek(DateTime.Today, 3);
    }
}
