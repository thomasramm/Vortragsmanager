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
                var old = zuteilung.Leser;
                zuteilung.Leser = value;
                if (zuteilung.Leser != old)
                    FilterWt();
            }
        }

        public AufgabenZuordnung SelectedVorsitz
        {
            get { return zuteilung.Vorsitz; }
            set
            {
                var old = zuteilung.Vorsitz;
                zuteilung.Vorsitz = value;
                if (zuteilung.Vorsitz != old)
                    FilterLeser();
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

                if (az.IsLeser)
                {
                    listeAlleLeser.Add(az);
                }

                if (az.IsVorsitz)
                {
                    listeAlleLeiter.Add(az);
                }
            }

            //zuteilungen
            zuteilung = DataContainer.AufgabenPersonKalenderFindOrAdd(Datum);

            FilterLeser();
            FilterWt();
        }

        private void FilterLeser()
        {
            var a = SelectedLeser;
            Leser.Clear();
            foreach (var item in listeAlleLeser.Where(x => x != zuteilung?.Vorsitz))
                Leser.Add(item);
            SelectedLeser = a;
        }

        private void FilterWt()
        {
            var a = SelectedVorsitz;
            Vorsitz.Clear();
            foreach (var item in listeAlleLeiter.Where(x => x != zuteilung?.Leser))
                Vorsitz.Add(item);
            SelectedVorsitz = a;
        }

        private readonly List<AufgabenZuordnung> listeAlleLeser = new List<AufgabenZuordnung>();

        public ObservableCollection<AufgabenZuordnung> Leser { get; } = new ObservableCollection<AufgabenZuordnung>();

        private readonly List<AufgabenZuordnung> listeAlleLeiter = new List<AufgabenZuordnung>();

        public ObservableCollection<AufgabenZuordnung> Vorsitz { get; } = new ObservableCollection<AufgabenZuordnung>();

        private AufgabenKalender zuteilung;
    }
}
