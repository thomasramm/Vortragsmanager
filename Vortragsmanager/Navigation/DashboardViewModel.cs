using DevExpress.Mvvm;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Linq;
using System.Windows;
using Vortragsmanager.Models;

namespace Vortragsmanager.Navigation
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly DateTime _datum;

        public DashboardViewModel()
        {
            if (Properties.Settings.Default.DashboardShowDetails)
            {
                _datum = Core.Helper.GetSunday(DateTime.Today);
                RaisePropertyChanged(nameof(Datum));
                GetProgramOfWeek();
                GetRednerProgram();
                return;
            }

            ShowMeinPlanDetails(false);
            ShowRednerDetails(false);
        }

        private void ShowMeinPlanDetails(bool show)
        {
            if (show)
            {
                MeinPlanDetailHeight = new GridLength(1, GridUnitType.Star);
                VersammlungsplanTileSize = TileSize.ExtraLarge;
                RaisePropertyChanged(nameof(VersammlungsplanTileSize));
            }
            else
            {
                MeinPlanDetailHeight = new GridLength(0, GridUnitType.Pixel);
                VersammlungsplanTileSize = TileSize.Large;
            }
            RaisePropertyChanged(nameof(VersammlungsplanTileSize));
        }

        private void ShowRednerDetails(bool show)
        {
            if (show)
            {
                RednerDetailHeight = new GridLength(1, GridUnitType.Star);
                RednerTileSize = TileSize.ExtraLarge;
            }
            else
            {
                RednerDetailHeight = new GridLength(0, GridUnitType.Pixel);
                RednerTileSize = TileSize.Large;
            }
            RaisePropertyChanged(nameof(RednerTileSize));
        }

        private void GetProgramOfWeek()
        {
            var prog = Core.DataContainer.MeinPlan.FirstOrDefault(x => x.Datum == _datum);
            if (prog == null)
            {
                ShowMeinPlanDetails(false);
                return;
            }
            if (prog.Status == EventStatus.Ereignis)
            {
                MeinPlanProgramm = prog.Anzeigetext;
            }
            else if (prog.Status == EventStatus.Zugesagt)
            {
                var einladung = (Invitation)prog;
                MeinPlanProgramm = einladung.Ältester.Name + Environment.NewLine
                    + einladung.Ältester.Versammlung.Name + Environment.NewLine
                    + einladung.Vortrag.Vortrag.ToString() + Environment.NewLine
                    + einladung.Ältester.Mobil ?? einladung.Ältester.Telefon;
            }
            else
            {
                ShowMeinPlanDetails(false);
            }
            RaisePropertyChanged(nameof(MeinPlanProgramm));
        }

        private void GetRednerProgram()
        {
            var nextRedner = Core.DataContainer.ExternerPlan.Where(x => x.Datum > DateTime.Today).OrderBy(x => x.Datum);
            if (!nextRedner.Any())
            {
                ShowRednerDetails(false);
            }
            else
            {
                var message = string.Empty;
                DateTime datum = DateTime.Today;
                var nr = 1;
                foreach (var r in nextRedner)
                {
                    if (datum == DateTime.Today || datum == r.Datum)
                    {
                        datum = r.Datum;
                        if (nr > 2)
                        {
                            message += "...";
                            break;
                        }
                        message += r.Datum.ToShortDateString() + " | " + r.Versammlung.GetZusammenkunftszeit(r.Datum) + Environment.NewLine
                            + r.Ältester.Name + " in " + r.Versammlung.Name + ", Nr. " + r.Vortrag?.Vortrag.Nummer + Environment.NewLine;
                        nr++;
                    }
                }
                RednerProgramm = message.TrimEnd('\n');
            }

            RaisePropertyChanged(nameof(RednerProgramm));
        }

        public string Datum => _datum.ToString("dd. MMM yyyy", Core.DataContainer.German);

        public string MeinPlanProgramm { get; set; }

        public GridLength MeinPlanDetailHeight { get; set; } = new GridLength(1, GridUnitType.Star);

        public TileSize VersammlungsplanTileSize { get; set; } = TileSize.ExtraLarge;

        public TileSize RednerTileSize { get; set; } = TileSize.ExtraLarge;

        public GridLength RednerDetailHeight { get; set; } = new GridLength(1, GridUnitType.Star);

        public string RednerProgramm { get; set; }
    }
}