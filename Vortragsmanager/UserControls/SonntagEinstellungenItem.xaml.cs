using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaction logic for AufgabenItem.xaml
    /// </summary>
    public partial class SonntagEinstellungenItem : UserControl, INotifyPropertyChanged
    {

        public SonntagEinstellungenItem(AufgabenZuordnung az)
        {
            InitializeComponent();
            Person = az;
            RednerDropDown.SelectedName = Person?.VerknüpftePerson?.Name;
        }

        public AufgabenZuordnung Person { get; set; }

        public static Conregation Versammlung => DataContainer.MeineVersammlung;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DropDownRedner_OnSpeakerChanged(object sender, RoutedPropertyChangedEventArgs<Speaker> e)
        {
            var x = e.NewValue;
            Person.VerknüpftePerson = x;          
            if (string.IsNullOrWhiteSpace(x?.Name))
            {
                Person.PersonName = "Unbekannt";
                DisplayNameEdit.IsReadOnly = false;
            }
            else
            {
                Person.PersonName = x.Name;
                DisplayNameEdit.IsReadOnly = true;
            }
            RaisePropertyChanged(nameof(Person.PersonName));
            RaisePropertyChanged(nameof(Person));
            AnzahlVerknüpfungen();
        }

        public SolidColorBrush Hintergrund { get; set; } = new SolidColorBrush(Colors.Transparent);

        public string Hinweis { get; set; }

        private void AnzahlVerknüpfungen()
        {
            var anzahl = DataContainer.AufgabenPersonZuordnung.Where(x => x.VerknüpftePerson == Person.VerknüpftePerson).Count();
            if (anzahl > 1)
            {
                //Warnung
                Hintergrund.Color = Colors.Firebrick;
                Hinweis = "ACHTUNG! Diese Person wurde bereits 1x zugeordnet!";
            }
            else
            {
                Hintergrund.Color = Colors.Transparent;
                Hinweis = null;
            }
            RaisePropertyChanged(nameof(Hintergrund));
            RaisePropertyChanged(nameof(Hinweis));
        }

    }
}
