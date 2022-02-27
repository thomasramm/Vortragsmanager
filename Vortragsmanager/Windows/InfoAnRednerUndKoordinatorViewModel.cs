using System.Windows;
using DevExpress.Mvvm;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    public class InfoAnRednerUndKoordinatorViewModel : ViewModelBase
    {
        public InfoAnRednerUndKoordinatorViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            SaveCommand = new DelegateCommand<ICloseable>(SpeichernSchließen);
            CopyCommand = new DelegateCommand<int>(CopyToClipboard);
        }

        public DelegateCommand<ICloseable> CloseCommand { get; }

        public DelegateCommand<ICloseable> SaveCommand { get; }

        public DelegateCommand<int> CopyCommand { get; }

        private string _titel;

        public string Titel
        {
            get => _titel;
            set
            {
                _titel = value;
                RaisePropertyChanged();
            }
        }

        public void CopyToClipboard(int fenster)
        {
            Clipboard.SetText(fenster == 1 ? MailTextRedner : MailTextKoordinator);
        }

        public void Schließen(ICloseable window)
        {
            Speichern = false;
            window?.Close();
        }

        public void SpeichernSchließen(ICloseable window)
        {
            Speichern = true;
            window?.Close();
        }

        public bool Speichern { get; set; }

        public GridLength ShowRednerInfoWidth => string.IsNullOrEmpty(MailTextRedner) ? new GridLength(0) : new GridLength(1, GridUnitType.Star);

        public GridLength ShowKoordinatorInfoWidth => string.IsNullOrEmpty(MailTextKoordinator) ? new GridLength(0) : new GridLength(1, GridUnitType.Star);

        private string _mailTextKoordinator;

        public string MailTextKoordinator
        {
            get => _mailTextKoordinator;
            set
            {
                _mailTextKoordinator = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ShowKoordinatorInfoWidth));
                if (value != null)
                    CopyToClipboard(2);
            }
        }

        private string _mailTextRedner;

        public string MailTextRedner
        {
            get => _mailTextRedner;
            set
            {
                _mailTextRedner = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ShowRednerInfoWidth));
                if (value != null)
                    CopyToClipboard(1);
            }
        }

        private string _infoAnRedner = "Info an Redner";

        public string InfoAnRednerTitel
        {
            get => _infoAnRedner;
            set
            {
                _infoAnRedner = value;
                RaisePropertyChanged();
            }
        }

        private string _infoAnKoordinator = "Info an Koordinator";

        public string InfoAnKoordinatorTitel
        {
            get => _infoAnKoordinator;
            set
            {
                _infoAnKoordinator = value;
                RaisePropertyChanged();
            }
        }

        public Visibility ShowSaveCancelButton { get; set; } = Visibility.Visible;

        public Visibility ShowCloseButton { get; set; } = Visibility.Collapsed;

        public void DisableCancelButton()
        {
            ShowCloseButton = Visibility.Visible;
            ShowSaveCancelButton = Visibility.Collapsed;
            RaisePropertiesChanged(nameof(ShowSaveCancelButton));
            RaisePropertiesChanged(nameof(ShowCloseButton));
        }
    }
}