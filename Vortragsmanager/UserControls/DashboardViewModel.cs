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
            var prog = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == Helper.CurrentWeek);
            if (prog == null)
            {
                ShowMeinPlanDetails(false);
                return;
            }
            if (prog.Status == EventStatus.Ereignis)
            {
                MeinPlanProgramm = prog.Anzeigetext;
                Datum = Helper.CalculateWeek(prog.Kw).ToString("dd. MMM yyyy", Helper.German);
            }
            else if (prog.Status == EventStatus.Zugesagt)
            {
                var einladung = (Invitation)prog;
                MeinPlanProgramm = einladung.Ältester.Name + Environment.NewLine
                    + einladung.Ältester.Versammlung.Name + Environment.NewLine
                    + einladung.Vortrag.Vortrag.ToString() + Environment.NewLine
                    + einladung.Ältester.Mobil ?? einladung.Ältester.Telefon;
                Datum = Helper.CalculateWeek(prog.Kw).ToString("dd. MMM yyyy", Helper.German);
            }
            else
            {
                ShowMeinPlanDetails(false);
            }
            RaisePropertyChanged(nameof(MeinPlanProgramm));
            RaisePropertyChanged(nameof(Datum));
        }

        private void GetRednerProgram()
        {
            var nextRedner = DataContainer.ExternerPlan.Where(x => x.Kw >= Helper.CurrentWeek).OrderBy(x => x.Kw);
            if (!nextRedner.Any())
            {
                ShowRednerDetails(false);
            }
            else
            {
                var message = string.Empty;
                int kw = Helper.CurrentWeek;
                var nr = 1;
                foreach (var r in nextRedner)
                {
                    if (kw == Helper.CurrentWeek || kw == r.Kw)
                    {
                        kw = r.Kw;
                        if (nr > 2)
                        {
                            message += "...";
                            break;
                        }
                        var datum = Helper.CalculateWeek(r.Kw, r.Versammlung);
                        message += datum.ToShortDateString() + " | " + r.Zeit.ToString() + Environment.NewLine
                            + r.Ältester.Name + " in " + r.Versammlung.Name + ", Nr. " + r.Vortrag?.Vortrag.Nummer + Environment.NewLine;
                        nr++;
                    }
                }
                RednerProgramm = message.TrimEnd('\n');
            }

            RaisePropertyChanged(nameof(RednerProgramm));
        }

        public string Datum { get; private set; }

        public string MeinPlanProgramm { get; set; }

        public GridLength MeinPlanDetailHeight { get; set; } = new GridLength(1, GridUnitType.Star);

        public TileSize VersammlungsplanTileSize { get; set; } = TileSize.ExtraLarge;

        public TileSize RednerTileSize { get; set; } = TileSize.ExtraLarge;

        public GridLength RednerDetailHeight { get; set; } = new GridLength(1, GridUnitType.Star);

        public string RednerProgramm { get; set; }
    }
}