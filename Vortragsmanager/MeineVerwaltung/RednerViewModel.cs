using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.MeineVerwaltung
{
    public class RednerViewModel : ViewModelBase
    {
        public RednerViewModel()
        {
            AddSpeakerCommand = new DelegateCommand(SpeakerAdd);
            DeleteSpeakerCommand = new DelegateCommand(SpeakerDelete);
            AddTalkCommand = new DelegateCommand(TalkAdd);
            DeleteTalkCommand = new DelegateCommand<TalkSong>(TalkDelete);

            ListeAllerVersammlungen = new ObservableCollection<Conregation>(Datamodels.DataContainer.Versammlungen.OrderBy(x => x, new Helper.EigeneKreisNameComparer()));
            ListeFilteredVersammlungen = new ObservableCollection<Conregation>(ListeAllerVersammlungen);

            ListeAllerRedner = new ObservableCollection<Speaker>(Datamodels.DataContainer.Redner.OrderBy(x => x.Name));
            ListeFilteredRedner = new ObservableCollection<Speaker>(ListeAllerRedner);
        }

        public ObservableCollection<Conregation> ListeAllerVersammlungen { get; private set; }

        public ObservableCollection<Conregation> ListeFilteredVersammlungen { get; private set; }

        public ObservableCollection<Speaker> ListeAllerRedner { get; private set; }

        public ObservableCollection<Speaker> ListeFilteredRedner { get; private set; }

        public ObservableCollection<Talk> Vortragsliste => Datamodels.DataContainer.Vorträge;

        public ObservableCollection<TalkSong> Vorträge => new ObservableCollection<TalkSong>(Redner.Vorträge);

        public DelegateCommand DeleteSpeakerCommand { get; private set; }

        public DelegateCommand AddSpeakerCommand { get; private set; }

        public DelegateCommand<TalkSong> DeleteTalkCommand { get; private set; }

        public DelegateCommand AddTalkCommand { get; private set; }

        #region Versammlung

        private string _selectedConregationName;

        public string SelectedConregationName
        {
            get => _selectedConregationName;
            set
            {
                _selectedConregationName = value;
                SetRednerfilter(value);
                FindSpeaker();
            }
        }

        public void SetVersammlungfilter(string versammlungFilter = null)
        {
            ListeFilteredVersammlungen.Clear();
            var items = ListeAllerVersammlungen.Count;
            var newCount = 0;
            var maxCount = (versammlungFilter == null) ? items : 10;
            for (int i = 0; i < items; i++)
            {
                if (string.IsNullOrEmpty(versammlungFilter) || (Regex.IsMatch(ListeAllerVersammlungen[i].Name, Regex.Escape(versammlungFilter), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)))
                {
                    ListeFilteredVersammlungen.Add(ListeAllerVersammlungen[i]);
                    newCount++;
                    if (newCount == maxCount)
                        break;
                }
            }
        }

        #endregion Versammlung

        #region Filter Redner

        private string _selectedSpeakerName;

        public string SelectedSpeakerName
        {
            get => _selectedSpeakerName;
            set
            {
                _selectedSpeakerName = value;
                FindSpeaker();
            }
        }

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
            }
        }

        private void FindSpeaker()
        {
            if (string.IsNullOrWhiteSpace(_selectedSpeakerName))
            {
                Redner = null;
                return;
            }
            var redner = Datamodels.DataContainer.Redner.Where(x => x.Name == _selectedSpeakerName).ToList();
            if (redner.Count > 1 && !string.IsNullOrWhiteSpace(_selectedConregationName))
            {
                redner = redner.Where(x => x.Versammlung.Name == _selectedConregationName).ToList();
            }
            if (redner.Count > 1)
            {
                MessageBox.Show("Mehr als 1 Redner mit diesem Namen gefunden, bitte über den Versammlungsfilter eingrenzen.");
                Redner = null;
            }
            else if (redner.Count == 0)
                Redner = null;
            else if (redner.Count == 1)
                Redner = redner[0];
        }

        public bool RednerSelektiert => (Redner != null);

        public void SetRednerfilter(string rednerFilter = null)
        {
            ListeFilteredRedner.Clear();
            var items = ListeAllerRedner.Count;
            var newCount = 0;
            for (int i = 0; i < items; i++)
            {
                if (SelectedConregationName == null || ListeAllerRedner[i].Versammlung.Name == SelectedConregationName)
                {
                    if (string.IsNullOrEmpty(rednerFilter) || (Regex.IsMatch(ListeAllerRedner[i].Name, Regex.Escape(rednerFilter), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)))
                    {
                        ListeFilteredRedner.Add(ListeAllerRedner[i]);
                        newCount++;
                        if (newCount == 10)
                            break;
                    }
                }
            }
        }

        public void SpeakerDelete()
        {
            if (DialogResult.No == MessageBox.Show($"Soll der Redner '{Redner.Name}' wirklich gelöscht werden?", "Achtung!", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;

            Datamodels.DataContainer.SpeakerRemove(Redner);

            ListeAllerRedner.Remove(Redner);
            ListeFilteredRedner.Remove(Redner);
            Redner = null;
        }

        public void SpeakerAdd()
        {
            Conregation vers = null;
            if (!string.IsNullOrEmpty(SelectedConregationName))
                vers = Datamodels.DataContainer.ConregationFind(SelectedConregationName);

            if (vers is null)
            {
                ThemedMessageBox.Show("Im Filter ist aktuell keine Versammlung gewählt. Bitte für den neuen Redner unter Aktionen -> Versammlung verschieben die Versammlung zuweisen.");
                vers = Datamodels.DataContainer.ConregationFind("Unbekannt");
            }

            Speaker redner = new Speaker();
            var i = 0;
            while (redner != null)
            {
                i++;
                redner = Datamodels.DataContainer.SpeakerFind($"Neuer Redner #{i}", vers);
            }
            redner = Datamodels.DataContainer.SpeakerFindOrAdd($"Neuer Redner #{i}", vers);

            SelectedSpeakerName = redner.Name;
        }

        #endregion Filter Redner

        #region Vortrag

        public void TalkDelete(TalkSong talk)
        {
            Redner.Vorträge.Remove(talk);
            RaisePropertyChanged(nameof(Vorträge));
        }

        public void TalkAdd()
        {
            if (string.IsNullOrWhiteSpace(NeueVorträgeListe))
                NeueVorträgeListe = NeuerVortrag.Substring(1, NeuerVortrag.IndexOf(")", StringComparison.InvariantCulture) - 1);

            var nummern = NeueVorträgeListe.Split(new char[] { ',', ' ', ';' });
            foreach (var nr in nummern)
            {
                bool isNum = int.TryParse(nr, out int num);
                if (!isNum)
                    continue;
                var neuerV = Datamodels.DataContainer.TalkFind(num);
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

        public string NeuerVortrag { get; set; }

        #endregion Vortrag
    }
}