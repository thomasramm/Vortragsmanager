using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    public class RednerEintragenView : ViewModelBase
    {
        public RednerEintragenView()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            SaveCommand = new DelegateCommand<ICloseable>(Save);
        }

        public DelegateCommand<ICloseable> CloseCommand { get; }

        public DelegateCommand<ICloseable> SaveCommand { get; }

        public static void Schließen(ICloseable window)
        {
            if (window != null)
                window.Close();
        }

        public bool Speichern { get; private set; }

        public void Save(ICloseable window)
        {
            //prüfen ob alle Felder gefüllt sind
            Speichern = true;
            if (window != null)
                window.Close();
        }

        private Conregation _selectedVersammlung;

        public Conregation SelectedVersammlung
        {
            get => _selectedVersammlung;
            set
            {
                _selectedVersammlung = value;
                if (value != null)
                {
                    Redner = DataContainer.Redner.Where(x => x.Versammlung == _selectedVersammlung).OrderBy(x => x.Name).ToList();
                    SelectedRedner = null;
                }
                RaisePropertyChanged();
            }
        }

        private List<Speaker> _redner;

        public List<Speaker> Redner
        {
            get => _redner;
            set
            {
                _redner = value;
                RaisePropertyChanged();
            }
        }

        private Speaker _selectedRedner;

        public Speaker SelectedRedner
        {
            get => _selectedRedner;
            set
            {
                _selectedRedner = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Vortrag));
                SelectedVortrag = null;
            }
        }

        public List<TalkSong> Vortrag => _selectedRedner?.Vorträge;

        private TalkSong _selectedVortrag;

        public TalkSong SelectedVortrag
        {
            get => _selectedVortrag;
            set
            {
                _selectedVortrag = value == null ? null : Vortrag.FirstOrDefault(x => x.Vortrag.Nummer == value.Vortrag.Nummer);
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanSave));
            }
        }

        public bool CanSave => SelectedVortrag != null;
    }
}