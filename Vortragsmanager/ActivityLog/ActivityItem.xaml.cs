using System;
using System.Globalization;
using System.Windows.Controls;

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
            string nl = Environment.NewLine;
            string datum = (_log.KalenderDatum == DateTime.MinValue) ? "" : $"Datum: {_log.KalenderDatum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}{nl}";
            string vortrag = ((_log.Vortrag?.Nummer ?? 0) < 1) ? "" : $"Vortrag: {_log.Vortrag}{nl}";
            string redner = (_log.Redner == null) ? "" : $"Redner: {_log.Redner.Name}{nl}";
            string versammlung = (_log.Versammlung == null) ? "" : $"Versammlung {_log.Versammlung.NameMitKoordinator}{nl}";

            switch (_log.Typ)
            {
                case Types.ExterneAnfrageBestätigen:
                case Types.ExterneAnfrageAblehnen:
                    ToolTipHeader.Text = (_log.Typ == Types.ExterneAnfrageAblehnen) ? "Abgelehnte Redneranfrage" : "Bestätigte Redneranfrage";
                    ToolTipBody.Text = $"Vortragsanfrage von:{nl}{versammlung}{nl}für: {nl}{redner}{datum}{vortrag}";
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
                    ToolTipBody.Text = $"Vortragseinladung an:{nl}{versammlung}{nl}für: {nl}{redner}{datum}{vortrag}";
                    break;

                case Types.RednerAnfrageAbgesagt:
                    ToolTipHeader.Text = "Ablehnung einer Vortragseinladung";
                    ToolTipBody.Text = $"Vortragseinladung an:{nl}{versammlung}{nl}für: {nl}{redner}{vortrag}{_log.Objekt}";
                    break;

                case Types.RednerAnfragen:
                    ToolTipHeader.Text = "Redneranfrage an Versammlung " + _log.Versammlung.Name;
                    ToolTipBody.Text = $"{_log.Mails}";
                    break;

                case Types.BuchungLöschen:
                    ToolTipHeader.Text = "Buchung gelöscht";
                    ToolTipBody.Text = $"{versammlung}{redner}{datum}{vortrag}";
                    break;

                case Types.BuchungVerschieben:
                    ToolTipHeader.Text = _log.Kommentar;
                    ToolTipBody.Text = $"{_log.Objekt}{nl}{redner}{versammlung}";
                    break;

                case Types.RednerEintragen:
                    ToolTipHeader.Text = "Redner direkt im Plan eingetragen";
                    ToolTipBody.Text = $"{datum}{versammlung}{redner}{vortrag}";
                    break;

                case Types.RednerErinnern:
                    ToolTipHeader.Text = "Erinnerungsmail gesendet";
                    ToolTipBody.Text = $"{datum}{redner}{vortrag}{versammlung}";
                    break;

                case Types.EreignisLöschen:
                    ToolTipHeader.Text = "Ereignis gelöscht";
                    ToolTipBody.Text = $"{datum}{vortrag}{_log.Objekt}";
                    break;

                case Types.EreignisBearbeiten:
                    ToolTipHeader.Text = "Ereignis bearbeitet";
                    ToolTipBody.Text = $"{datum}{vortrag}{_log.Objekt}";
                    break;

                case Types.EreignisAnlegen:
                    ToolTipHeader.Text = "Ereignis erstellt";
                    ToolTipBody.Text = $"{datum}{vortrag}{_log.Objekt}";
                    break;

                case Types.RednerBearbeiten:
                    ToolTipHeader.Text = "Rednereinladung geändert";
                    ToolTipBody.Text = $"{versammlung}{datum}{redner}{vortrag}{_log.Objekt}";
                    break;

                case Types.Sonstige:
                default:
                    ToolTipHeader.Text = "NOT IMPLEMENTED";
                    ToolTipBody.Text = string.Empty;
                    break;
            }
            if (!string.IsNullOrEmpty(_log.Kommentar))
                ToolTipBody.Text += $"{nl}{_log.Kommentar}";

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