using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.PageModels
{
    public class MeinPlanVorsitzUndLeserEinstellungenPage : ViewModelBase
    {
        public MeinPlanVorsitzUndLeserEinstellungenPage()
        {
            foreach (var az in DataContainer.AufgabenPersonZuordnung.OrderBy(x => x.PersonName))
            {
                Personen.Add(new SonntagEinstellungenItem(az));
            }

            ButtonNewCommand = new DelegateCommand(ButtonNew);
        }

        public ObservableCollection<SonntagEinstellungenItem> Personen { get; } = new ObservableCollection<SonntagEinstellungenItem>();

        public static Conregation Versammlung => DataContainer.MeineVersammlung;

        public DelegateCommand ButtonNewCommand { get; }

        private void ButtonNew()
        {
            var az = DataContainer.AufgabenZuordnungAdd();
            Personen.Add(new SonntagEinstellungenItem(az));
        }

        public int MonateAnzahlAnzeige
        {
            get => Helper.Helper.GlobalSettings.SonntagAnzeigeMonate;
            set
            {
                Helper.Helper.GlobalSettings.SonntagAnzeigeMonate = value;
                Helper.Helper.GlobalSettings.Save();
                RaisePropertyChanged();
            }
        }
    }
}