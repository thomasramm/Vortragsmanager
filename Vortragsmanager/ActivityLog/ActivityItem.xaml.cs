using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.ActivityLog
{
    /// <summary>
    /// Interaction logic for ActivityItem.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        private readonly Activity _log;

        public Item()
        {
            InitializeComponent();
        }

        public Item(Activity log)
        {
            InitializeComponent();
            _log = log;
            DataContext = _log;
            SetToolTip();
            Symbol.Source = Symbols.GetImage(Typ);
        }

        private void SetToolTip()
        {
            switch (_log.Typ)
            {
                case Types.ExterneAnfrageAblehnen:
                    ToolTipHeader.Text = "Abgelehnte Redneranfrage";
                    ToolTipBody.Text = $"Vortragsanfrage von:{Environment.NewLine}" +
                        $"{_log.Versammlung.NameMitKoordinator}{Environment.NewLine}{Environment.NewLine}" +
                        $"für: {Environment.NewLine}" +
                        $"{_log.Redner.Name}{Environment.NewLine}" +
                        $"{_log.Objekt}";
                    break;

                case Types.ExterneAnfrageBestätigen:
                    ToolTipHeader.Text = "Bestätigte Redneranfrage";
                    ToolTipBody.Text = $"Vortragsanfrage von:{Environment.NewLine}" +
                        $"{_log.Versammlung.NameMitKoordinator}{Environment.NewLine}{Environment.NewLine}" +
                        $"für: {Environment.NewLine}" +
                        $"{_log.Redner.Name}{Environment.NewLine}" +
                        $"{_log.Objekt}";
                    break;

                case Types.ExterneAnfrageListeSenden:
                    ToolTipHeader.Text = "Einladungsliste für meine Redner";
                    ToolTipBody.Text = _log.Mails;
                    break;

                case Types.SendMail:
                    ToolTipHeader.Text = _log.Objekt;
                    ToolTipBody.Text = _log.Mails;
                    break;

                case Types.RednerAnfrageBestätigt:
                    ToolTipHeader.Text = "Bestätigung für Vortragseinladung am " + _log.Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    ToolTipBody.Text = $"Vortragseinladung an:{Environment.NewLine}" +
                        $"{_log.Versammlung.NameMitKoordinator}{Environment.NewLine}{Environment.NewLine}" +
                        $"für: {Environment.NewLine}" +
                        $"{_log.Redner.Name}{Environment.NewLine}" +
                        $"{_log.Objekt}";
                    break;

                case Types.RednerAnfrageAbgesagt:
                    ToolTipHeader.Text = "Ablehnung einer Vortragseinladung";
                    ToolTipBody.Text = $"Vortragseinladung an:{Environment.NewLine}" +
                        $"{_log.Versammlung.NameMitKoordinator}{Environment.NewLine}{Environment.NewLine}" +
                        $"für: {Environment.NewLine}" +
                        $"{_log.Redner.Name}{Environment.NewLine}" +
                        $"{_log.Objekt}";
                    break;

                case Types.RednerAnfragen:
                    ToolTipHeader.Text = "Redneranfrage an Versammlung " + _log.Versammlung.Name;
                    ToolTipBody.Text = _log.Mails;
                    break;

                case Types.BuchungLöschen:
                    ToolTipHeader.Text = "Buchung gelöscht";
                    ToolTipBody.Text = $"Redner: {_log.Redner.Name}{Environment.NewLine}" +
                        $"{_log.Objekt}" +
                        $"Versammlung: {_log.Versammlung.NameMitKoordinator}";
                    break;

                case Types.RednerEintragen:
                    ToolTipHeader.Text = "Redner direkt im Plan eingetragen";
                    ToolTipBody.Text = $"Redner: {_log.Redner.Name}{Environment.NewLine}" +
                        $"{_log.Objekt}" +
                        $"Versammlung: {_log.Versammlung.NameMitKoordinator}";
                    break;

                case Types.RednerErinnern:
                    ToolTipHeader.Text = "Erinnerungsmail gesendet";
                    ToolTipBody.Text = $"Redner: {_log.Redner.Name}{Environment.NewLine}" +
                        $"{_log.Objekt}{Environment.NewLine}" +
                        $"Versammlung: {_log.Versammlung.NameMitKoordinator}";
                    break;

                case Types.EreignisLöschen:
                    ToolTipHeader.Text = "Ereignis gelöscht";
                    ToolTipBody.Text = _log.Objekt;
                    break;

                case Types.EreignisBearbeiten:
                    ToolTipHeader.Text = "Ereignis bearbeitet";
                    ToolTipBody.Text = _log.Objekt;
                    break;

                case Types.EreignisAnlegen:
                    ToolTipHeader.Text = "Ereignis erstellt";
                    ToolTipBody.Text = _log.Objekt;
                    break;

                case Types.EinladungBearbeiten:
                    ToolTipHeader.Text = "Rednereinladung geändert";
                    ToolTipBody.Text = _log.Objekt;
                    break;

                case Types.Sonstige:
                default:
                    ToolTipHeader.Text = "NOT IMPLEMENTED";
                    ToolTipBody.Text = string.Empty;
                    break;
            }

            Symbol.ToolTip = ToolTipHeader.Text;
        }

        public string VersammlungName => _log.Versammlung.NameMitKoordinator;

        public Types Typ => _log.Typ;

        public bool Aktiv
        {
            get
            {
                return myUserControlItem.Visibility == System.Windows.Visibility.Visible;
            }
            set
            {
                myUserControlItem.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
    }
}