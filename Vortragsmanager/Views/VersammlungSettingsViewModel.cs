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
            NewPersonCommand = new DelegateCommand(NewPerson);
        }

        public DelegateCommand DeleteCommand { get; private set; }

        public DelegateCommand NewPersonCommand { get; private set; }

        public void Delete()
        {
            //ToDo: Versammlung löschen: Redner löschen, im Vortragsplan die Zuteilungen ersetzen gegen "unbekannt"
            Core.DataContainer.Versammlungen.Remove(Versammlung);
            Sichtbar = Visibility.Collapsed;
            RaisePropertyChanged(nameof(Sichtbar));
        }

        public void NewPerson()
        {
            var redner = Core.DataContainer.FindOrAddSpeaker("Neuer Redner", Versammlung);
            var rednerModel = new SpeakerViewModel(redner);
            RednerListe.Add(rednerModel);
            rednerModel.Select();
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

        public DevExpress.Xpf.LayoutControl.GroupBoxState IsSelected { get; set; }

        public void Select()
        {
            IsSelected = DevExpress.Xpf.LayoutControl.GroupBoxState.Maximized;
            RaisePropertyChanged(nameof(IsSelected));
        }
    }

    public class SpeakerViewModel : ViewModelBase
    {
        public SpeakerViewModel(Speaker redner)
        {
            Redner = redner;
            NeuerVortragCommand = new DelegateCommand(NeuenVortragSpeichern);
            VortragLöschenCommand = new DelegateCommand(VortragLöschen);
            DeleteCommand = new DelegateCommand(RednerLöschen);
        }

        public DelegateCommand DeleteCommand { get; set; }

        public DelegateCommand NeuerVortragCommand { get; private set; }

        public DelegateCommand VortragLöschenCommand { get; private set; }

        public void RednerLöschen()
        {
            //ToDo: Redner löschen: im Vortragsplan die Zuteilungen ersetzen gegen "unbekannt"
            Core.DataContainer.Redner.Remove(Redner);
            Sichtbar = Visibility.Collapsed;
            RaisePropertyChanged(nameof(Sichtbar));
        }

        public Visibility Sichtbar { get; set; }

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

        public SolidColorBrush AktivBrush => Redner.Aktiv ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

        public string AktivText => Redner.Aktiv ? "Aktiv" : "Inaktiv";

        public SolidColorBrush ÄltesterBrush => Redner.Ältester ? new SolidColorBrush(Colors.DodgerBlue) : new SolidColorBrush(Colors.Brown);

        public string ÄltesterText => Redner.Ältester ? "Ältester" : "DAG";

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

        public DevExpress.Xpf.LayoutControl.GroupBoxState IsSelected { get; set; }

        public void Select()
        {
            IsSelected = DevExpress.Xpf.LayoutControl.GroupBoxState.Maximized;
            RaisePropertyChanged(nameof(IsSelected));
        }

        public Speaker Redner { get; private set; }
    }

    public class SpeakersViewModelCollection : ObservableCollection<SpeakerViewModel>
    {
        public SpeakersViewModelCollection(Conregation versammlung)
        {
            var redner = Core.DataContainer.Redner.Where(x => x.Versammlung == versammlung).OrderBy(x => x.Name);
            foreach (Speaker r in redner)
                Add(new SpeakerViewModel(r));
        }
    }

    public class ConregationsViewModelCollection : ObservableCollection<ConregationViewModel>
    {
        public ConregationsViewModelCollection() : this(Core.DataContainer.Versammlungen)
        {
        }

        public ConregationsViewModelCollection(IEnumerable<Conregation> versammlungen)
        {
            if (versammlungen is null)
                throw new NullReferenceException();

            var v = versammlungen.OrderBy(x => x, new EigeneKreisNameComparer());

            foreach (Conregation versammlung in v)
                Add(new ConregationViewModel(versammlung));

            AddConregationCommand = new DelegateCommand(AddConregation);
        }

        public class EigeneKreisNameComparer : IComparer<Conregation>
        {
            public int Compare(Conregation x, Conregation y)
            {
                var eigene = Core.DataContainer.MeineVersammlung;
                var eigenerKreis = eigene.Kreis;
                string value1 = ((x.Kreis == eigenerKreis) ? "0" : "1") + ((x == eigene) ? "0" : "1") + x.Kreis + x.Name;
                string value2 = ((y.Kreis == eigenerKreis) ? "0" : "1") + ((y == eigene) ? "0" : "1") + y.Kreis + y.Name;
                return string.Compare(value1, value2, StringComparison.InvariantCulture);
            }
        }

        public DelegateCommand AddConregationCommand { get; private set; }

        public void AddConregation()
        {
            //ToDo: Versammlung hinzufügen, Oberfläche aktualisieren + Selektieren/Maximieren
            var vers = Core.DataContainer.FindOrAddConregation("Neue Versammlung");
            var model = new ConregationViewModel(vers);
            Add(model);
            model.Select();
        }
    }
}