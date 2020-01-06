using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class SpeakerViewModel : ViewModelBase
    {
        public SpeakerViewModel(Speaker redner)
        {
            if (redner is null)
                throw new NullReferenceException();

            Redner = redner;
            NeuerVortragCommand = new DelegateCommand(NeuenVortragSpeichern);
            VortragLöschenCommand = new DelegateCommand(VortragLöschen);
            DeleteCommand = new DelegateCommand(RednerLöschen);
            _selectedVersammlung = Redner.Versammlung;
        }

        public DelegateCommand DeleteCommand { get; set; }

        public DelegateCommand NeuerVortragCommand { get; private set; }

        public DelegateCommand VortragLöschenCommand { get; private set; }

        public void RednerLöschen(bool silent)
        {
            //ToDo: Redner löschen: im Vortragsplan die Zuteilungen ersetzen gegen "unbekannt"
            if (silent || ThemedMessageBox.Show("Redner löschen",
                $"Wirklich Redner {Redner.Name} löschen?" + Environment.NewLine +
                "Alle vergangenen Einladungen werden durch 'unbekannt' ersetzt," + Environment.NewLine +
                "alle zukünftigen Einladungen werden gelöscht.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //Einladungen
                var einladungen = Core.DataContainer.MeinPlan
                    .Where(x => x.Status == EventStatus.Zugesagt)
                    .Cast<Invitation>()
                    .Where(x => x.Ältester == Redner)
                    .ToList();
                foreach (var einladung in einladungen)
                {
                    if (einladung.Datum < DateTime.Today)
                        einladung.Ältester = null;
                    else
                        Core.DataContainer.MeinPlan.Remove(einladung);
                }
                //Offene Anfragen
                var anfragen = Core.DataContainer.OffeneAnfragen
                    .Where(x => x.RednerVortrag.ContainsKey(Redner))
                    .ToList();
                foreach (var anfrage in anfragen)
                {
                    anfrage.RednerVortrag.Remove(Redner);
                    if (anfrage.RednerVortrag.Count == 0)
                        Core.DataContainer.OffeneAnfragen.Remove(anfrage);
                }

                //Externe Einladungen
                if (Redner.Versammlung == Core.DataContainer.MeineVersammlung)
                {
                    var externeE = Core.DataContainer.ExternerPlan.Where(x => x.Ältester == Redner).ToList();
                    foreach (var einladung in externeE)
                        Core.DataContainer.ExternerPlan.Remove(einladung);
                }

                Core.DataContainer.Redner.Remove(Redner);
                Sichtbar = Visibility.Collapsed;
                RaisePropertyChanged(nameof(Sichtbar));
            }
        }

        public void RednerLöschen()
        {
            RednerLöschen(false);
        }

        public Visibility Sichtbar { get; set; }

        public void VortragLöschen()
        {
            Redner.Vorträge.Remove(GewählterVortrag);
            RaisePropertyChanged(nameof(Vorträge));
        }

        public void NeuenVortragSpeichern()
        {
            if (string.IsNullOrWhiteSpace(NeueVorträgeListe))
                NeueVorträgeListe = NeuerVortrag.Nummer.ToString(Core.DataContainer.German);

            var nummern = NeueVorträgeListe.Split(new char[] { ',', ' ', ';' });
            int num;
            foreach (var nr in nummern)
            {
                bool isNum = int.TryParse(nr, out num);
                var neuerV = Core.DataContainer.FindTalk(num);
                if (neuerV == null)
                    continue;
                if (!Redner.Vorträge.Contains(neuerV))
                    Redner.Vorträge.Add(neuerV);
            }
            NeueVorträgeListe = string.Empty;
            RaisePropertyChanged(nameof(NeueVorträgeListe));
            RaisePropertyChanged(nameof(Vorträge));
        }

        public string NeueVorträgeListe
        {
            get; set;
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

        public ObservableCollection<Conregation> Versammlungen => new ObservableCollection<Conregation>(Core.DataContainer.Versammlungen);

        public bool VersammlungenPopUp { get; set; }

        private Conregation _selectedVersammlung;

        public Conregation SelectedVersammlung
        {
            get
            {
                return _selectedVersammlung;
            }
            set
            {
                if (!VersammlungenPopUp && value != null && value != Redner?.Versammlung)
                {
                    if (ThemedMessageBox.Show("Achtung", $"Soll der Redner {Redner.Name} wirklich in die Versammlung {value.Name} verschoben werden?") == MessageBoxResult.OK)
                    {
                        Redner.Versammlung = value;
                        _selectedVersammlung = value;
                        Sichtbar = Visibility.Collapsed;
                        RaisePropertyChanged(nameof(Sichtbar));
                        //ToDo: UI der übernehmenden Versammlung aktualisieren
                    }
                }
                RaisePropertyChanged();
            }
        }
    }
}