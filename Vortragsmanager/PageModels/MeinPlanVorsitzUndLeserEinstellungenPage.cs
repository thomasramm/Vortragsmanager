using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
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

        public DelegateCommand ButtonNewCommand { get; private set; }

        private void ButtonNew()
        {
            var az = DataContainer.AufgabenZuordnungAdd();
            Personen.Add(new SonntagEinstellungenItem(az));
        }

        public int MonateAnzahlAnzeige
        {
            get
            {
                return Properties.Settings.Default.SonntagAnzeigeMonate;
            }
            set
            {
                Properties.Settings.Default.SonntagAnzeigeMonate = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
    }
}