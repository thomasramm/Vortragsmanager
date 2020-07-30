using DevExpress.Mvvm;
using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Vortragsmanager.Datamodels
{
    public class Activity : ViewModelBase
    {
        private bool _sichtbar = true;
        private DateTime _datum;
        private int _id;
        private Conregation _versammlung;
        private Speaker _redner;
        private ActivityType _typ;
        private string _objekt;
        private string _kommentar;
        private string _mails;

        public Activity()
        {
            Datum = DateTime.Now;
        }

        public Activity(DateTime datum)
        {
            Datum = datum;
        }

        public Activity(int id, DateTime datum, Conregation versammlung, ActivityType typ, string objekt, string kommentar, bool sichtbar)
        {
            Id = id;
            Datum = datum;
            Versammlung = versammlung;
            Typ = typ;
            Objekt = objekt;
            Kommentar = kommentar;
            Sichtbar = sichtbar;
        }

        public int Id
        {
            get => _id; set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        public DateTime Datum
        {
            get => _datum; set
            {
                _datum = value;
                RaisePropertyChanged();
            }
        }

        public Conregation Versammlung
        {
            get => _versammlung;
            set
            {
                _versammlung = value;
                RaisePropertyChanged();
            }
        }

        public Speaker Redner
        {
            get => _redner;
            set
            {
                _redner = value;
                RaisePropertyChanged();
            }
        }

        public ActivityType Typ
        {
            get => _typ; set
            {
                _typ = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Symbol));
            }
        }

        public string Objekt
        {
            get => _objekt; set
            {
                _objekt = value;
                RaisePropertyChanged();
            }
        }

        public string Kommentar
        {
            get => _kommentar;
            set
            {
                _kommentar = value;
                RaisePropertyChanged();
            }
        }

        public string Mails
        {
            get => _mails;
            set
            {
                _mails = value;
                RaisePropertyChanged();
            }
        }

        public bool Sichtbar
        {
            get => _sichtbar;
            set
            {
                _sichtbar = value;
                RaisePropertyChanged();
            }
        }

        public BitmapImage Symbol => ActivityTypeSymbols.GetImage(Typ);

        public string ToolTipHeader
        {
            get
            {
                switch (Typ)
                {
                    case ActivityType.ExterneAnfrageAblehnen:
                        return "Abgelehnte Redneranfrage";

                    case ActivityType.ExterneAnfrageBestätigen:
                        return "Bestätigte Redneranfrage";

                    case ActivityType.ExterneAnfrageListeSenden:
                        return "Einladungsliste für meine Redner";

                    case ActivityType.SendMail:
                        return Objekt;

                    case ActivityType.RednerAnfrageBestätigt:
                        return "Bestätigung für Vortragseinladung am " + Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

                    case ActivityType.RednerAnfrageAbgesagt:
                        return "Ablehnung einer Vortragseinladung";

                    case ActivityType.RednerAnfragen:
                        return "Redneranfrage an Versammlung " + Versammlung.Name;

                    case ActivityType.Sonstige:
                    default:
                        return "NOT IMPLEMENTED";
                }
            }
        }

        public string ToolTip
        {
            get
            {
                switch (Typ)
                {
                    case ActivityType.ExterneAnfrageBestätigen:
                    case ActivityType.ExterneAnfrageAblehnen:
                        return $"Vortragsanfrage von:{Environment.NewLine}" +
                            $"{Versammlung.NameMitKoordinator}{Environment.NewLine}{Environment.NewLine}" +
                            $"für: {Environment.NewLine}" +
                            $"{Redner.Name}{Environment.NewLine}" +
                            $"{Objekt}";

                    case ActivityType.ExterneAnfrageListeSenden:
                    case ActivityType.SendMail:
                    case ActivityType.RednerAnfragen:
                        return Mails;

                    case ActivityType.RednerAnfrageBestätigt:
                    case ActivityType.RednerAnfrageAbgesagt:
                        return $"Vortragseinladung an:{Environment.NewLine}" +
                            $"{Versammlung.NameMitKoordinator}{Environment.NewLine}{Environment.NewLine}" +
                            $"für: {Environment.NewLine}" +
                            $"{Redner.Name}{Environment.NewLine}" +
                            $"{Objekt}";

                    case ActivityType.Sonstige:
                    default:
                        return "NOT IMPLEMENTED";
                }
            }
        }

        public string ToolTipMailtext => Mails;
    }

    public enum ActivityType
    {
        Sonstige,
        ExterneAnfrageAblehnen,
        ExterneAnfrageBestätigen,
        ExterneAnfrageListeSenden,
        SendMail,
        RednerAnfrageBestätigt,
        RednerAnfrageAbgesagt,
        RednerAnfragen,
    }

    public static class ActivityTypeSymbols
    {
        private static readonly BitmapImage MeinPlan = new BitmapImage(new Uri("/Images/Kalender_64x64.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailAbsage = new BitmapImage(new Uri("/Images/MeinPlanMailAbsage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailZusage = new BitmapImage(new Uri("/Images/MeinPlanMailZusage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailAnfrage = new BitmapImage(new Uri("/Images/MeinPlanMailAnfrage_32x32.png", UriKind.Relative));

        private static readonly BitmapImage MeineRedner = new BitmapImage(new Uri("/Images/Person_64x64.png", UriKind.Relative));
        private static readonly BitmapImage MailAntwort = new BitmapImage(new Uri("/Images/MailAntwort_64x64.png", UriKind.Relative));
        private static readonly BitmapImage Sonstige = new BitmapImage(new Uri("/Images/Sonstige_64x64.png", UriKind.Relative));
        private static readonly BitmapImage MeineRednerBestätigen = new BitmapImage(new Uri("/Images/MeineRednerBestätigen1_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeineRednerAblehnen = new BitmapImage(new Uri("/Images/MeineRednerAbgelehnt1_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeineRednerMail = new BitmapImage(new Uri("/Images/MeineRednerMail_32x32.png", UriKind.Relative));
        private static readonly BitmapImage Mail = new BitmapImage(new Uri("/Images/Mail_32x32.png", UriKind.Relative));

        public static BitmapImage GetImage(ActivityType typ)
        {
            switch (typ)
            {
                case ActivityType.ExterneAnfrageAblehnen:
                    return MeineRednerAblehnen;

                case ActivityType.ExterneAnfrageBestätigen:
                    return MeineRednerBestätigen;

                case ActivityType.ExterneAnfrageListeSenden:
                    return MeineRednerMail;

                case ActivityType.SendMail:
                    return Mail;

                case ActivityType.RednerAnfrageBestätigt:
                    return MeinPlanMailZusage;

                case ActivityType.RednerAnfrageAbgesagt:
                    return MeinPlanMailAbsage;

                case ActivityType.RednerAnfragen:
                    return MeinPlanMailAnfrage;

                default:
                    return Sonstige;
            }
        }
    }
}