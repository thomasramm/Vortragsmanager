using System;
using System.Globalization;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaction logic for ActivityItem.xaml
    /// </summary>
    public partial class Item
    {
        private readonly ActivityItemViewModel _log;

        public Item()
        {
            InitializeComponent();
        }

        public Item(ActivityItemViewModel log)
        {
            InitializeComponent();
            _log = log;
            DataContext = _log;
            SetToolTip();
            Symbol.Source = ActivityGetSymbols.GetImage(Typ);
        }

        private void SetToolTip()
        {
            string nl = Environment.NewLine;
            string datum = (_log.KalenderKw == -1) ? "" : $"Datum: {DateCalcuation.CalculateWeek(_log.KalenderKw).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}{nl}";
            string vortrag = ((_log.Vortrag?.Nummer ?? 0) < 1) ? "" : $"Vortrag: {_log.Vortrag}{nl}";
            string redner = (_log.Redner == null) ? "" : $"Redner: {_log.Redner.Name}{nl}";
            string versammlung = (_log.Versammlung == null) ? "" : $"Versammlung {_log.Versammlung.NameMitKoordinator}{nl}";

            switch (_log.Typ)
            {
                case ActivityTypes.ExterneAnfrageBestätigen:
                case ActivityTypes.ExterneAnfrageAblehnen:
                    ToolTipHeader.Text = (_log.Typ == ActivityTypes.ExterneAnfrageAblehnen) ? "Abgelehnte Redneranfrage" : "Bestätigte Redneranfrage";
                    ToolTipBody.Text = $"Vortragsanfrage von:{nl}{versammlung}{nl}für: {nl}{redner}{datum}{vortrag}";
                    break;

                case ActivityTypes.ExterneAnfrageListeSenden:
                    ToolTipHeader.Text = "Einladungsliste für meine Redner";
                    ToolTipBody.Text = _log.Mails;
                    break;

                case ActivityTypes.SendMail:
                    ToolTipHeader.Text = _log.Objekt;
                    ToolTipBody.Text = _log.Mails;
                    break;

                case ActivityTypes.RednerAnfrageBestätigt:
                    ToolTipHeader.Text = "Bestätigung für Vortragseinladung am " + _log.Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    ToolTipBody.Text = $"Vortragseinladung an:{nl}{versammlung}{nl}für: {nl}{redner}{datum}{vortrag}";
                    break;

                case ActivityTypes.RednerAnfrageAbgesagt:
                    ToolTipHeader.Text = "Ablehnung einer Vortragseinladung";
                    ToolTipBody.Text = $"Vortragseinladung an:{nl}{versammlung}{nl}für: {nl}{redner}{vortrag}{_log.Objekt}";
                    break;

                case ActivityTypes.RednerAnfragen:
                    ToolTipHeader.Text = "Redneranfrage an Versammlung " + _log.Versammlung?.Name;
                    ToolTipBody.Text = $"{_log.Mails}";
                    break;

                case ActivityTypes.BuchungLöschen:
                    ToolTipHeader.Text = "Buchung gelöscht";
                    ToolTipBody.Text = $"{versammlung}{redner}{datum}{vortrag}";
                    break;

                case ActivityTypes.BuchungVerschieben:
                    ToolTipHeader.Text = _log.Kommentar;
                    ToolTipBody.Text = $"{_log.Objekt}{nl}{redner}{versammlung}";
                    break;

                case ActivityTypes.RednerEintragen:
                    ToolTipHeader.Text = "Redner direkt im Plan eingetragen";
                    ToolTipBody.Text = $"{datum}{versammlung}{redner}{vortrag}";
                    break;

                case ActivityTypes.RednerErinnern:
                    ToolTipHeader.Text = "Erinnerungsmail gesendet";
                    ToolTipBody.Text = $"{datum}{redner}{vortrag}{versammlung}";
                    break;

                case ActivityTypes.EreignisLöschen:
                    ToolTipHeader.Text = "Ereignis gelöscht";
                    ToolTipBody.Text = $"{datum}{vortrag}{_log.Objekt}";
                    break;

                case ActivityTypes.EreignisBearbeiten:
                    ToolTipHeader.Text = "Ereignis bearbeitet";
                    ToolTipBody.Text = $"{datum}{vortrag}{_log.Objekt}";
                    break;

                case ActivityTypes.EreignisAnlegen:
                    ToolTipHeader.Text = "Ereignis erstellt";
                    ToolTipBody.Text = $"{datum}{vortrag}{_log.Objekt}";
                    break;

                case ActivityTypes.RednerBearbeiten:
                    ToolTipHeader.Text = "Rednereinladung geändert";
                    ToolTipBody.Text = $"{versammlung}{datum}{redner}{vortrag}{_log.Objekt}";
                    break;

                case ActivityTypes.Sonstige:
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

        public ActivityTypes Typ => _log.Typ;

        public bool Aktiv { get; set; } = true;

        public ActivityTime Zeitraum { get; set; }
    }
}