using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Vortragsmanager.Datamodels;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.MeinPlan
{
    public class SonntagEinstellungenViewModel : ViewModelBase
    {
        public SonntagEinstellungenViewModel()
        {
            foreach(var az in DataContainer.AufgabenPersonZuordnung.OrderBy(x => x.PersonName))
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
