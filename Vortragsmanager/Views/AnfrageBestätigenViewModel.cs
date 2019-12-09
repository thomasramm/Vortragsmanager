using DevExpress.Mvvm;
using System.Windows;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class AnfrageBestätigenViewModel : ViewModelBase
    {
        private readonly bool _annehmen;
        private readonly bool _infoAnRedner;

        public AnfrageBestätigenViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            SaveCommand = new DelegateCommand<ICloseable>(SpeichernSchließen);
            CopyCommand = new DelegateCommand<int>(CopyToClipboard);
            _annehmen = true;
        }

        public AnfrageBestätigenViewModel(Outside anfrage, bool annehmen, bool infoAnRedner) : this()
        {
            Buchung = anfrage;
            _annehmen = annehmen;
            _infoAnRedner = infoAnRedner;
            if (annehmen)
            {
                GetMailTextAnnehmen();
            }
            else
            {
                GetMailTextAblehnen();
            }
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }
        
        public DelegateCommand<ICloseable> SaveCommand { get; private set; }

        public DelegateCommand<int> CopyCommand { get; private set; }

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
            if (_annehmen)
                Speichern = true;
            if (window != null)
                window.Close();
        }

        private void GetMailTextAnnehmen()
        {
            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText).Inhalt;
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Versammlung}", Buchung.Versammlung.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Koordinator Mail}", $"{Buchung.Versammlung.KoordinatorJw}; {Buchung.Versammlung.KoordinatorMail}")
                .Replace("{Koordinator Name}", Buchung.Versammlung.Koordinator);
        
            MailTextKoordinator = mt;
            Clipboard.SetText(mt);

            mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText).Inhalt;

            mt = mt
                .Replace("{Redner Name}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Redner Mail}", Buchung.Ältester?.Mail ?? "unbekannt")
                .Replace("{Redner Versammlung}", Buchung.Ältester?.Versammlung.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")

                .Replace("{Versammlung}", Buchung.Versammlung.Name)
                .Replace("{Versammlung Anschrift1}", Buchung.Versammlung.Anschrift1)
                .Replace("{Versammlung Anschrift2}", Buchung.Versammlung.Anschrift2)
                .Replace("{Versammlung Telefon}", Buchung.Versammlung.Telefon)
                .Replace("{Versammlung Zusammenkunftszeit}", Buchung.Versammlung.GetZusammenkunftszeit(Buchung.Datum));

            MailTextRedner = mt;
            Clipboard.SetText(mt);
        }

        private void GetMailTextAblehnen()
        {
            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Redner Mail}", $"{Buchung.Ältester.Mail ?? "unbekannt"}")
                .Replace("{Koordinator Name}", Buchung.Versammlung.Koordinator)
                .Replace("{Versammlung}", Buchung.Versammlung.Name);

            MailTextRedner = mt;

            mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Koordinator Mail}", $"{Buchung.Versammlung.KoordinatorJw}; {Buchung.Versammlung.KoordinatorMail}")
                .Replace("{Koordinator Name}", Buchung.Versammlung.Koordinator)
                .Replace("{Versammlung}", Buchung.Versammlung.Name);

            MailTextKoordinator = mt;
            Clipboard.SetText(mt);
        }

        public bool Speichern { get; set; }

        public Outside Buchung { get; set; }

        public GridLength ShowRednerInfoWidth => _infoAnRedner ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

        public string MailTextKoordinator
        {
            get { return GetProperty(() => MailTextKoordinator); }
            set { SetProperty(() => MailTextKoordinator, value); }
        }
        public string MailTextRedner
        {
            get { return GetProperty(() => MailTextRedner); }
            set { SetProperty(() => MailTextRedner, value); }
        }
    }
}

