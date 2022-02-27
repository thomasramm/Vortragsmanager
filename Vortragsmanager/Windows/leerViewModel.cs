using System.Windows;
using DevExpress.Mvvm;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    public class LeerViewModel : ViewModelBase
    {
        public LeerViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Close);
            SaveCommand = new DelegateCommand<ICloseable>(Save);
            CopyCommand = new DelegateCommand(Copy);
        }

        public LeerViewModel(string titel, bool closeButton, bool saveButton, bool copyButton, string text, string headerText = "") : this()
        {
            Titel = titel;
            ShowCloseButton = closeButton;
            ShowSaveButton = saveButton;
            ShowCopyButton = copyButton;
            Text = text;
            HeaderText = headerText;
        }

        public DelegateCommand<ICloseable> CloseCommand { get; }
        public DelegateCommand<ICloseable> SaveCommand { get; }

        public DelegateCommand CopyCommand { get; }

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
            get => _titel;
            set
            {
                _titel = value;
                RaisePropertyChanged();
            }
        }

        private bool _showCloseButton;

        public bool ShowCloseButton
        {
            get => _showCloseButton;
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
            get => _showSaveButton;
            set
            {
                _showSaveButton = value;
                RaisePropertyChanged(nameof(CloseButtonText));
            }
        }

        public bool ShowCopyButton { get; set; }

        private string _headerText;

        public string HeaderText
        {
            get => _headerText;
            set 
            { 
                _headerText = value;
                SetHeaderTextVisible(!string.IsNullOrWhiteSpace(_headerText));
            }
        }

        private void SetHeaderTextVisible(bool visible)
        {
            HeaderTextVisible = visible ? new GridLength(50) : new GridLength(0);
        }

        public GridLength HeaderTextVisible { get; private set; }


        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }
    }
}