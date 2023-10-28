using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
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
            ButtonDeleteCommand = new DelegateCommand(ButtonDelete);
        }

        public ObservableCollection<SonntagEinstellungenItem> Personen { get; } = new ObservableCollection<SonntagEinstellungenItem>();

        public SonntagEinstellungenItem SelectedPerson { get; set; }

        public static Conregation Versammlung => DataContainer.MeineVersammlung;

        public DelegateCommand ButtonNewCommand { get; }

        public DelegateCommand ButtonDeleteCommand { get; }

        private void ButtonNew()
        {
            var az = DataContainer.AufgabenZuordnungAdd();
            Personen.Add(new SonntagEinstellungenItem(az));
        }

        private void ButtonDelete()
        {
            if (SelectedPerson == null)
            {
                ThemedMessageBox.Show("Information", "Bitte die zu löschende Person auswählen", System.Windows.MessageBoxButton.OK);
                return;
            }

            //Alle Zuordnungen der Person entfernen, dann die Person selber entfernen
            foreach (var p in DataContainer.AufgabenPersonKalender.Where(x=> x.Vorsitz == SelectedPerson.Person))
            {
                p.Vorsitz = null;
            }
            foreach (var p in DataContainer.AufgabenPersonKalender.Where(x => x.Leser == SelectedPerson.Person))
            {
                p.Leser = null;
            }

            var apz = DataContainer.AufgabenPersonZuordnung.First(x => x == SelectedPerson.Person);
            DataContainer.AufgabenPersonZuordnung.Remove(apz);
            Personen.Remove(SelectedPerson);
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