using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaction logic for SonntagItem.xaml
    /// </summary>
    public partial class SonntagItem : UserControl
    {
        public SonntagItem()
        {
            InitializeComponent();
        }

        public SonntagItem(DateTime datum)
        {
            InitializeComponent();

            Datum = datum;
            LoadData();
        }

        public DateTime Datum { get; set; }

        public AufgabenZuordnung SelectedLeser
        {
            get { return zuteilung.Leser; }
            set
            {
                FilterOtherList(Vorsitz, zuteilung.Leser, value);
                zuteilung.Leser = value;
            }
        }

        public AufgabenZuordnung SelectedVorsitz
        {
            get { return zuteilung.Vorsitz; }
            set
            {
                FilterOtherList(Leser, zuteilung.Vorsitz, value);
                zuteilung.Vorsitz = value;
            }
        }

        public string Vortragsredner { get; set; }

        public string AuswärtigeRedner { get; set; }

        private void LoadData()
        {
            //Vortragsredner
            var redner = DataContainer.MeinPlan.FirstOrDefault(x => x.Datum == Datum);
            switch (redner.Status)
            {
                case EventStatus.Zugesagt:
                    var zu = (redner as Invitation);
                    Vortragsredner = zu.Ältester.Name;
                    break;
                case EventStatus.Ereignis:
                    var er = (redner as SpecialEvent);
                    Vortragsredner = er.Anzeigetext;
                    break;
                default:
                    break;
            }

            //Auswärtige Redner
            var auswärts = DataContainer.ExternerPlan.Where(x => x.Datum == Datum);
            AuswärtigeRedner = string.Empty;
            foreach (var item in auswärts)
            {
                AuswärtigeRedner += item.Ältester + ", ";
            }
            if (AuswärtigeRedner.Length > 2)
                AuswärtigeRedner = AuswärtigeRedner.Substring(0, AuswärtigeRedner.Length - 2);

            //zuteilungen
            zuteilung = DataContainer.AufgabenPersonKalenderFindOrAdd(Datum);

            //DropDown Vorsitz + Leser
            foreach (var az in DataContainer.AufgabenPersonZuordnung)
            {
                if (az.VerknüpftePerson != null)
                {
                    if (redner.Status == EventStatus.Zugesagt)
                    {
                        var ereignis = (redner as Invitation);
                        if (ereignis != null && (ereignis.Ältester == az.VerknüpftePerson))
                        {
                            continue;
                        }

                    }

                    if (auswärts.Any(x => x.Ältester == az.VerknüpftePerson))
                    {
                        continue;
                    }
                }

                if (az.IsLeser && az != zuteilung.Vorsitz)
                {
                    Leser.Add(az);
                }

                if (az.IsVorsitz && az != zuteilung.Leser)
                {
                    Vorsitz.Add(az);
                }
            }

            

            FilterOtherList(Leser, null, SelectedVorsitz);
            FilterOtherList(Vorsitz, null, SelectedLeser);
        }

        private void FilterOtherList(ObservableCollection<AufgabenZuordnung> liste, AufgabenZuordnung oldValue, AufgabenZuordnung newValue)
        {
            if (oldValue == newValue)
                return;

            //entfernen der geählten Person aus der Liste
            if (liste.Contains(newValue))
                liste.Remove(newValue);
            //hinzufügen der frei gewordenen Person
            if ((oldValue != null) && !liste.Contains(oldValue) && ((newValue.IsLeser && liste == Leser) || (newValue.IsVorsitz && liste == Vorsitz)))
                liste.Add(oldValue);
        }

        public ObservableCollection<AufgabenZuordnung> Leser { get; } = new ObservableCollection<AufgabenZuordnung>();

        public ObservableCollection<AufgabenZuordnung> Vorsitz { get; } = new ObservableCollection<AufgabenZuordnung>();

        private AufgabenKalender zuteilung;
    }
}
