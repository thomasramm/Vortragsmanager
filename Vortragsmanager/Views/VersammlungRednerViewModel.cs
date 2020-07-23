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

        public void RednerLöschen()
        {
            RednerLöschen(false);
        }

        public void RednerLöschen(bool silent)
        {
            if (silent || ThemedMessageBox.Show("Redner löschen",
                $"Wirklich Redner {Redner.Name} löschen?" + Environment.NewLine +
                "Alle vergangenen Einladungen werden durch 'unbekannt' ersetzt," + Environment.NewLine +
                "alle zukünftigen Einladungen werden gelöscht.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Core.DataContainer.SpeakerRemove(Redner);
                Sichtbar = Visibility.Collapsed;
                RaisePropertyChanged(nameof(Sichtbar));
            }
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
                NeueVorträgeListe = NeuerVortrag.Vortrag.Nummer.ToString(Core.DataContainer.German);

            var nummern = NeueVorträgeListe.Split(new char[] { ',', ' ', ';' });
            foreach (var nr in nummern)
            {
                bool isNum = int.TryParse(nr, out int num);
                if (!isNum)
                    continue;
                var neuerV = Core.DataContainer.TalkFind(num);
                if (neuerV == null)
                    continue;
                if (!Redner.Vorträge.Select(x => x.Vortrag).Contains(neuerV))
                    Redner.Vorträge.Add(new TalkSong(neuerV));
            }
            NeueVorträgeListe = string.Empty;
            RaisePropertyChanged(nameof(Vorträge));
        }

        private string _neueVorträgeListe;

        public string NeueVorträgeListe
        {
            get
            {
                return _neueVorträgeListe;
            }
            set
            {
                _neueVorträgeListe = value;
                RaisePropertyChanged();
            }
        }

        public SolidColorBrush AktivBrush => Redner.Aktiv && Redner.Einladen ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

        public string AktivText => Redner.Aktiv && Redner.Einladen ? "Aktiv" : "Inaktiv";

        public SolidColorBrush ÄltesterBrush => Redner.Ältester ? new SolidColorBrush(Colors.DodgerBlue) : new SolidColorBrush(Colors.Brown);

        public string ÄltesterText => Redner.Ältester ? "Ältester" : "DAG";

        public string Overview
        {
            get
            {
                string o = "Vorträge: ";
                foreach (var v in Vorträge)
                    o += v.Vortrag.Nummer + ", ";

                return o.TrimEnd(' ').TrimEnd(',');
            }
        }

        public TalkSong GewählterVortrag { get; set; }

        public TalkSong NeuerVortrag { get; set; }

        public ObservableCollection<TalkSong> Vorträge
        {
            get => new ObservableCollection<TalkSong>(Redner.Vorträge.OrderBy(x => x.Vortrag.Nummer));
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822")]
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