using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.Views
{
    public class RednerEintragenView : ViewModelBase
    {
        public RednerEintragenView()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            SaveCommand = new DelegateCommand<ICloseable>(Save);
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }

        public DelegateCommand<ICloseable> SaveCommand { get; private set; }

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
            get
            {
                return _selectedVersammlung;
            }
            set
            {
                _selectedVersammlung = value;
                if (value != null)
                {
                    Redner = DataContainer.Redner.Where(x => x.Versammlung == _selectedVersammlung).OrderBy(x => x.Name).ToList();
                    SelectedRedner = null;
                }
            }
        }

        private List<Speaker> _redner;

        public List<Speaker> Redner
        {
            get
            {
                return _redner;
            }
            set
            {
                _redner = value;
                RaisePropertyChanged();
            }
        }

        private Speaker _selectedRedner;

        public Speaker SelectedRedner
        {
            get
            {
                return _selectedRedner;
            }
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
            get
            {
                return _selectedVortrag;
            }
            set
            {
                if (value == null)
                    _selectedVortrag = null;
                else
                    _selectedVortrag = Vortrag.FirstOrDefault(x => x.Vortrag.Nummer == value.Vortrag.Nummer);
                RaisePropertyChanged();
            }
        }
    }
}