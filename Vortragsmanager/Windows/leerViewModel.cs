using DevExpress.Mvvm;
using System.Windows;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Views
{
    public class LeerViewModel : ViewModelBase
    {
        public LeerViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Close);
            SaveCommand = new DelegateCommand<ICloseable>(Save);
            CopyCommand = new DelegateCommand(Copy);
        }

        public LeerViewModel(string titel, bool CloseButton, bool SaveButton, bool CopyButton, string text, string headerText = "") : this()
        {
            Titel = titel;
            ShowCloseButton = CloseButton;
            ShowSaveButton = SaveButton;
            ShowCopyButton = CopyButton;
            Text = text;
            HeaderText = headerText;
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }
        public DelegateCommand<ICloseable> SaveCommand { get; private set; }

        public DelegateCommand CopyCommand { get; private set; }

        public void Close(ICloseable window)
        {
            Speichern = false;
            if (window != null)
                window.Close();
        }

        public void Save(ICloseable window)
        {
            Speichern = true;
            if (window != null)
                window.Close();
        }

        public void Copy()
        {
            Clipboard.SetText(Text);
        }

        public bool Speichern { get; set; }

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

        private bool _showCloseButton;

        public bool ShowCloseButton
        {
            get
            {
                return _showCloseButton;
            }
            set
            {
                _showCloseButton = value;
                RaisePropertyChanged();
            }
        }

        public string CloseButtonText => _showSaveButton ? "Abbrechen" : "Schließen";

        private bool _showSaveButton;

        public bool ShowSaveButton
        {
            get
            {
                return _showSaveButton;
            }
            set
            {
                _showSaveButton = value;
                RaisePropertyChanged(nameof(CloseButtonText));
            }
        }

        private bool _showCopyButton;

        public bool ShowCopyButton
        {
            get
            {
                return _showCopyButton;
            }
            set
            {
                _showCopyButton = value;
            }
        }

        private string _headerText;

        public string HeaderText
        {
            get { return _headerText; }
            set 
            { 
                _headerText = value;
                SetHeaderTextVisible(!string.IsNullOrWhiteSpace(_headerText));
            }
        }

        private void SetHeaderTextVisible(bool visible)
        {
            HeaderTextVisible = visible ? new GridLength(50) : new GridLength(0); ;
        }

        public GridLength HeaderTextVisible { get; private set; }


        private string _text;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }
    }
}