using DevExpress.Mvvm;
using System.Windows;

namespace Vortragsmanager.Views
{
    public class BuchungLöschenViewModel : ViewModelBase
    {
        private readonly Models.Invitation _zuteilung;

        public BuchungLöschenViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            DeleteCommand = new DelegateCommand<ICloseable>(LöschenSchließen);
            CopyCommand = new DelegateCommand(Kopieren);
        }

        public BuchungLöschenViewModel(Models.Invitation Zuteilung) : this()
        {
            _zuteilung = Zuteilung;
            GetMailText();  
        }

        public DelegateCommand CopyCommand { get; private set; }
        public DelegateCommand<ICloseable> CloseCommand { get; private set; }
        public DelegateCommand<ICloseable> DeleteCommand { get; private set; }

        public void Schließen(ICloseable window)
        {
            Gelöscht = false;
            if (window != null)
                window.Close();
        }

        public void LöschenSchließen(ICloseable window)
        {
            Gelöscht = true;
            if (window != null)
                window.Close();
        }

        public void Kopieren()
        {
            Clipboard.SetText(MailText);
        }

        private void GetMailText()
        {
            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            
            var vers = _zuteilung.Ältester?.Versammlung  ?? Core.DataContainer.FindConregation("Unbekannt");

            mt = mt
                .Replace("{Datum}", $"{_zuteilung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner Name}", _zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Koordinator Mail}", $"{vers.KoordinatorJw}; {vers.KoordinatorMail}")
                .Replace("{Koordinator Name}", vers.Koordinator)
                .Replace("{Versammlung}", vers.Name);

            MailText = mt;
            Clipboard.SetText(mt);
        }

        public bool Gelöscht { get; set; }

        public string MailText
        {
            get { return GetProperty(() => MailText); }
            set { SetProperty(() => MailText, value); }
        }
    }
}

