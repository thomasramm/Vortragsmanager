using DevExpress.Mvvm;
using System.Windows;
using Vortragsmanager.Core;

namespace Vortragsmanager.Navigation
{
    public class DashboardViewModel : ViewModelBase
    {
        public int AktuelleWoche => Helper.CurrentWeek;

        public int Woche2 => Helper.AddWeek(Helper.CurrentWeek, 1);
        
        public int Woche3 => Helper.AddWeek(Helper.CurrentWeek, 2);

        public int Woche4 => Helper.AddWeek(Helper.CurrentWeek, 3);
    }
}