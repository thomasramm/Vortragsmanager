using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class ConregationViewModel : ViewModelBase
    {
        public ConregationViewModel(Conregation versammlung)
        {
            Versammlung = versammlung;
            RednerListe = new SpeakersViewModelCollection(versammlung);
            DeleteCommand = new DelegateCommand(Delete);
        }

        
        public DelegateCommand DeleteCommand { get; private set; }

        public void Delete()
        {
            //ToDO: Versammlung löschen: Redner löschen, im Vortragsplan die Zuteilungen ersetzen gegen "unbekannt"
            Core.DataContainer.Versammlungen.Remove(Versammlung);
            Sichtbar = Visibility.Collapsed;
            RaisePropertyChanged(nameof(Sichtbar));
        }

        public Visibility Sichtbar { get; set; } = Visibility.Visible;

        public int Jahr1 { get; } = DateTime.Today.Year;

        public int Jahr2 => Jahr1 + 1;
        
        public int Jahr3 => Jahr1 + 2;

        public string ZusammenkunftszeitJahr1
        {
            get
            {
                return Versammlung.GetZusammenkunftszeit(Jahr1);
            }
            set
            {
                if (value != ZusammenkunftszeitJahr1) 
                    Versammlung.SetZusammenkunftszeit(Jahr1, value);
                RaisePropertyChanged(ZusammenkunftszeitJahr1);
            }
        }

        public string ZusammenkunftszeitJahr2
        {
            get
            {
                return Versammlung.GetZusammenkunftszeit(Jahr2);
            }
            set
            {
                if ((value != ZusammenkunftszeitJahr1) && (value != Versammlung.GetZusammenkunftszeit(Jahr2)))
                    Versammlung.SetZusammenkunftszeit(Jahr1, value);
                RaisePropertyChanged(ZusammenkunftszeitJahr2);
            }
        }

        public string ZusammenkunftszeitJahr3
        {
            get
            {
                return Versammlung.GetZusammenkunftszeit(Jahr3);
            }
            set
            {
                if ((value != ZusammenkunftszeitJahr2) && (value != Versammlung.GetZusammenkunftszeit(Jahr3)))
                    Versammlung.SetZusammenkunftszeit(Jahr3, value);
                RaisePropertyChanged(ZusammenkunftszeitJahr3);
            }
        }

        public bool EigeneVersammlung
        {
            get
            {
                return (Core.DataContainer.MeineVersammlung == Versammlung);
            }
            set
            {
                if ((value == true) && (ThemedMessageBox.Show(
                    "Willst du diese Versammlung wirklich als deine eigene Versammlung setzen?", 
                    "Achtung!", 
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes))
                    Core.DataContainer.MeineVersammlung = Versammlung;
            }
        }

        public Brush EigeneVersammlungTextInOrange
        {
            get
            {
                return (EigeneVersammlung) ? Brushes.Orange : Brushes.White;
            }
        }

        public Conregation Versammlung { get; private set; }

        public SpeakersViewModelCollection RednerListe { get; private set; }

        public bool IsMaximized { get; set; }

    }

    public class SpeakerViewModel : ViewModelBase
    {
        public SpeakerViewModel(Speaker redner)
        {
            Redner = redner;
            NeuerVortragCommand = new DelegateCommand(NeuenVortragSpeichern);
            VortragLöschenCommand = new DelegateCommand(VortragLöschen);
        }

        public DelegateCommand NeuerVortragCommand { get; private set; }

        public DelegateCommand VortragLöschenCommand { get; private set; }

        public void VortragLöschen()
        {
            Redner.Vorträge.Remove(GewählterVortrag);
            RaisePropertyChanged(nameof(Vorträge));
        }

        public void NeuenVortragSpeichern()
        {
            if (!Redner.Vorträge.Contains(NeuerVortrag))
                Redner.Vorträge.Add(NeuerVortrag);
            RaisePropertyChanged(nameof(Vorträge));
        }

        public SolidColorBrush AktivBrush => Redner.Aktiv? new SolidColorBrush(Colors.Green): new SolidColorBrush(Colors.Red);

        public string AktivText => Redner.Aktiv ? "Aktiv" : "Inaktiv";

        public SolidColorBrush ÄltesterBrush => Redner.Ältester ? new SolidColorBrush(Colors.DodgerBlue) : new SolidColorBrush(Colors.Brown);

        public string ÄltesterText => Redner.Ältester? "Ältester" : "DAG";


        public string Overview
        {
            get
            {
                string o = "Vorträge: ";
                foreach (var v in Vorträge)
                    o += v.Nummer + ", ";

                return o.TrimEnd(' ').TrimEnd(',');
            }
        }

        public Talk GewählterVortrag { get; set; }

        public Talk NeuerVortrag { get; set; }

        public ObservableCollection<Talk> Vorträge
        {
            get => new ObservableCollection<Talk>(Redner.Vorträge.OrderBy(x => x.Nummer));
        }

        public static ObservableCollection<Talk> Vortragsliste => Core.DataContainer.Vorträge;

        public bool Ältester
        {
            get => Redner.Ältester;
            set
            {
                Redner.Ältester = value;
                RaisePropertiesChanged(new[] { "Ältester", "Typ" });
            }
        }

        public Speaker Redner { get; private set; }
    }

    public class SpeakersViewModelCollection : List<SpeakerViewModel>
    {
        public SpeakersViewModelCollection(Conregation versammlung)
        {
            var redner = Core.DataContainer.Redner.Where(x => x.Versammlung == versammlung);
            foreach (Speaker r in redner)
                Add(new SpeakerViewModel(r));

            Sort(CompareByName);
        }

        private int CompareByName(SpeakerViewModel x, SpeakerViewModel y)
        {

            string value1 = x.Redner.Name;
            string value2 = y.Redner.Name;
            return string.Compare(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class ConregationsViewModelCollection : List<ConregationViewModel>
    {
        public ConregationsViewModelCollection() : this(Core.DataContainer.Versammlungen) { }
        public ConregationsViewModelCollection(IEnumerable<Conregation> versammlungen)
        {
            if (versammlungen is null)
                throw new NullReferenceException();

            foreach (Conregation versammlung in versammlungen)
                Add(new ConregationViewModel(versammlung));

            Sort(CompareByEigeneKreisName);

            AddConregationCommand = new DelegateCommand(AddConregation);
        }

        private int CompareByEigeneKreisName(ConregationViewModel x, ConregationViewModel y)
        {
            var eigene = Core.DataContainer.MeineVersammlung;
            var eigenerKreis = eigene.Kreis;
            string value1 = ((x.Versammlung.Kreis == eigenerKreis) ? "0" : "1") + ((x.Versammlung == eigene) ? "0" : "1") + x.Versammlung.Kreis + x.Versammlung.Name;
            string value2 = ((y.Versammlung.Kreis == eigenerKreis) ? "0" : "1") + ((y.Versammlung == eigene) ? "0" : "1") + y.Versammlung.Kreis + y.Versammlung.Name;
            return string.Compare(value1, value2, StringComparison.InvariantCulture);
        }

        public DelegateCommand AddConregationCommand { get; private set; }

        public void AddConregation()
        {
            //ToDo: Versammlung hinzufügen, Oberfläche aktualisieren + Selektieren/Maximieren
            var vers = new Conregation() { Name = "Neue Versammlung" };
            Core.DataContainer.Versammlungen.Add(vers);
            var model = new ConregationViewModel(vers);
            Add(model);
            SelectedConregation = model;
        }

        public ConregationViewModel SelectedConregation { get; set; }
    }
}
