using DevExpress.Mvvm;
using System.Windows;

namespace Vortragsmanager.Views
{
    public class InfoAnRednerUndKoordinatorViewModel : ViewModelBase
    {
        public InfoAnRednerUndKoordinatorViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            SaveCommand = new DelegateCommand<ICloseable>(SpeichernSchließen);
            CopyCommand = new DelegateCommand<int>(CopyToClipboard);
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }

        public DelegateCommand<ICloseable> SaveCommand { get; private set; }

        public DelegateCommand<int> CopyCommand { get; private set; }

        private string _titel;

        public string Titel
        {
            get
            {
                return _titel;
            }
            set
            {
                _titel = value;
                RaisePropertyChanged();
            }
        }

        public void CopyToClipboard(int fenster)
        {
            if (fenster == 1)
                Clipboard.SetText(MailTextRedner);
            else
                Clipboard.SetText(MailTextKoordinator);
        }

        public void Schließen(ICloseable window)
        {
            Speichern = false;
            if (window != null)
                window.Close();
        }

        public void SpeichernSchließen(ICloseable window)
        {
            Speichern = true;
            if (window != null)
                window.Close();
        }

        public bool Speichern { get; set; }

        public GridLength ShowRednerInfoWidth => string.IsNullOrEmpty(MailTextRedner) ? new GridLength(0) : new GridLength(1, GridUnitType.Star);

        public GridLength ShowKoordinatorInfoWidth => string.IsNullOrEmpty(MailTextKoordinator) ? new GridLength(0) : new GridLength(1, GridUnitType.Star);

        private string _mailTextKoordinator;

        public string MailTextKoordinator
        {
            get
            {
                return _mailTextKoordinator;
            }
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
            get { return _mailTextRedner; }
            set
            {
                _mailTextRedner = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ShowRednerInfoWidth));
                if (value != null)
                    CopyToClipboard(1);
            }
        }

        public Visibility ShowSaveCancelButton { get; set; } = Visibility.Visible;

        public Visibility ShowCloseButton { get; set; } = Visibility.Hidden;

        public void DisableCancelButton()
        {
            ShowCloseButton = Visibility.Visible;
            ShowSaveCancelButton = Visibility.Hidden;
            RaisePropertiesChanged(nameof(ShowSaveCancelButton));
            RaisePropertiesChanged(nameof(ShowCloseButton));
        }
    }
}