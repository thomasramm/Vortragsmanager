using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.DataModels;

namespace Vortragsmanager.PageModels
{
    public class VerwaltungRednerPageModel : ViewModelBase
    {
        public VerwaltungRednerPageModel()
        {
            AddSpeakerCommand = new DelegateCommand(SpeakerAdd);
            AddTalkCommand = new DelegateCommand(TalkAdd);
            DeleteTalkCommand = new DelegateCommand<TalkSong>(TalkDelete);
            FotoRemoveCommand = new DelegateCommand(RednerRemoveFoto);

            RednerAktivitäten = new ObservableCollection<DateWithConregation>();

            //DropDown Redner in andere Versammlung verschieben füllen...
            ListeAllerVersammlungen = new ObservableCollection<Conregation>(DataContainer.Versammlungen.OrderBy(x => x, new Helper.EigeneKreisNameComparer()));
        }

        public bool VersammlungenPopUp { get; set; }

        private Conregation _newConregation;

        public Conregation NewConregation
        {
            get => _newConregation;
            set
            {
                if (!VersammlungenPopUp && value != null && value != Redner?.Versammlung)
                {
                    if (ThemedMessageBox.Show("Achtung", $"Soll der Redner {Redner?.Name} wirklich in die Versammlung {value.Name} verschoben werden?") == System.Windows.MessageBoxResult.OK)
                    {
                        if (Redner != null)
                        {
                            Redner.Versammlung = value;
                            _newConregation = value;
                            //ToDo: UI der übernehmenden Versammlung aktualisieren
                            RaisePropertyChanged(nameof(Redner));
                            RaisePropertyChanged(nameof(Redner.Versammlung));
                        }
                    }
                }
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TalkSong> Vorträge
        {
            get
            {
                if (Redner?.Vorträge != null)
                    return new ObservableCollection<TalkSong>(Redner?.Vorträge.OrderBy(x => x.Vortrag.Nummer));

                return null;
            }
        }

        public ObservableCollection<Conregation> ListeAllerVersammlungen { get; }

        public DelegateCommand AddSpeakerCommand { get; }

        public DelegateCommand<TalkSong> DeleteTalkCommand { get; }

        public DelegateCommand AddTalkCommand { get; }

        public DelegateCommand FotoRemoveCommand { get; }

        #region Filter Versammlung

        private Conregation _selectedConregation;

        public Conregation SelectedConregation
        {
            get => _selectedConregation;
            set
            {
                _selectedConregation = value;
                RaisePropertyChanged();
                //SetRednerfilter();
            }
        }

        #endregion Filter Versammlung

        #region Filter Redner

        private Speaker _redner;

        public Speaker Redner
        {
            get => _redner;
            set
            {
                _redner = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(RednerSelektiert));
                RaisePropertyChanged(nameof(Vorträge));
                RednerAktivitätenUpdate();
                RednerSetFoto();
            }
        }
        
        public bool RednerSelektiert => (Redner != null);

        public void SpeakerDelete()
        {
            if (DialogResult.No == MessageBox.Show($"Soll der Redner '{Redner.Name}' wirklich gelöscht werden?", "Achtung!", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;

            if (DataContainer.SpeakerRemove(Redner))
                Redner = null;
        }

        public void SpeakerAdd()
        {
            if (SelectedConregation is null)
            {
                ThemedMessageBox.Show("Im Filter ist aktuell keine Versammlung gewählt. Bitte für den neuen Redner unter Aktionen -> Versammlung verschieben die Versammlung zuweisen.");
                SelectedConregation = DataContainer.ConregationFind("Unbekannt");
            }

            Speaker redner = new Speaker();
            var i = 0;
            while (redner != null)
            {
                i++;
                redner = DataContainer.SpeakerFind($"Neuer Redner #{i}", SelectedConregation);
            }
            redner = DataContainer.SpeakerFindOrAdd($"Neuer Redner #{i}", SelectedConregation);

            Redner = redner;
        }

        #endregion Filter Redner

        private void RednerSetFoto()
        {
            Foto = Redner?.Foto;
        }

        private void RednerRemoveFoto()
        {
            Foto = null;
        }

        public BitmapSource Foto
        {
            get
            {
                if (Redner?.Foto != null)
                {
                    return Redner.Foto;
                }

                return Helper.Helper.StyleIsDark 
                    ? new BitmapImage(new Uri("/Images/Kamera_dunkel_64x64.png", UriKind.Relative)) 
                    : new BitmapImage(new Uri("/Images/Kamera_hell_64x64.png", UriKind.Relative));
            }
            set
            {
                if (Redner != null)
                    Redner.Foto = value;
                RaisePropertyChanged();
            }
        }

        #region Vortrag

        public void TalkDelete(TalkSong talk)
        {
            Redner.Vorträge.Remove(talk);
            RaisePropertyChanged(nameof(Vorträge));
        }

        public void TalkAdd()
        {
            if (string.IsNullOrWhiteSpace(NeueVorträgeListe))
                NeueVorträgeListe = NeuerVortrag?.Nummer.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrWhiteSpace(NeueVorträgeListe))
                return;

            var nummern = NeueVorträgeListe?.Split(',', ' ', ';');
            foreach (var nr in nummern)
            {
                bool isNum = int.TryParse(nr, out int num);
                if (!isNum)
                    continue;
                var neuerV = TalkList.Find(num);
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
            get => _neueVorträgeListe;
            set
            {
                _neueVorträgeListe = value;
                RaisePropertyChanged();
            }
        }

        public Talk NeuerVortrag { get; set; }

        #endregion Vortrag

        private void RednerAktivitätenUpdate()
        {
            RednerAktivitäten.Clear();

            if (Redner == null)
                return;

            var erg = DataContainer.SpeakerGetActivities(Redner).OrderByDescending(x => x.Kalenderwoche).Take(10);

            foreach (var item in erg)
            {
                RednerAktivitäten.Add(item);
            }
        }
        public ObservableCollection<DateWithConregation> RednerAktivitäten { get; }
    }
}