using DevExpress.Mvvm;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Linq;
using System.Windows;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.Navigation
{
    public class DashboardViewModel : ViewModelBase
    {
        public DashboardViewModel()
        {
            if (Properties.Settings.Default.DashboardShowDetails)
            {
                RaisePropertyChanged(nameof(Datum));
                return;
            }
        }


        public string Datum { get; private set; }

        public string MeinPlanProgramm { get; set; }

        public int AktuelleWoche => Helper.CurrentWeek;

        public int Woche2 => Helper.AddWeek(Helper.CurrentWeek, 1);
        
        public int Woche3 => Helper.AddWeek(Helper.CurrentWeek, 2);

        public int Woche4 => Helper.AddWeek(Helper.CurrentWeek, 3);

        public GridLength MeinPlanDetailHeight { get; set; } = new GridLength(1, GridUnitType.Star);

        public TileSize VersammlungsplanTileSize { get; set; } = TileSize.ExtraLarge;

        public TileSize RednerTileSize { get; set; } = TileSize.ExtraLarge;

        public GridLength RednerDetailHeight { get; set; } = new GridLength(1, GridUnitType.Star);

        public string RednerProgramm { get; set; }
    }
}