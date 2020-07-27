using DevExpress.Mvvm;
using System;
using System.Windows.Media.Imaging;

namespace Vortragsmanager.Datamodels
{
    public class Activity : ViewModelBase
    {
        private bool _sichtbar = true;
        private DateTime _datum;
        private int _id;
        private Conregation _versammlung;
        private ActivityType _typ;
        private string _objekt;
        private string _kommentar;

        public Activity()
        {
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
            get => _versammlung; set
            {
                _versammlung = value;
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
            get => _kommentar; set
            {
                _kommentar = value;
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
    }

    public enum ActivityType
    {
        VortragAnfragen,
        VortragsanfrageBestätigen,
        VortragLöschen,
        VortragTauschen,
        MailSenden,
        ExterneAnfrageAblehnen,
        ExterneAnfrageBestätigen,
    }

    public static class ActivityTypeSymbols
    {
        private static readonly BitmapImage MeinPlan = new BitmapImage(new Uri("/Images/Kalender_64x64.png", UriKind.Relative));
        private static readonly BitmapImage MeineRedner = new BitmapImage(new Uri("/Images/Person_64x64.png", UriKind.Relative));
        private static readonly BitmapImage MailAntwort = new BitmapImage(new Uri("/Images/MailAntwort_64x64.png", UriKind.Relative));
        private static readonly BitmapImage Sonstige = new BitmapImage(new Uri("/Images/Sonstige_64x64.png", UriKind.Relative));

        public static BitmapImage GetImage(ActivityType typ)
        {
            switch (typ)
            {
                case ActivityType.VortragAnfragen:
                    return MeinPlan;

                case ActivityType.VortragsanfrageBestätigen:
                    return MeinPlan;

                case ActivityType.VortragLöschen:
                    return MeinPlan;

                case ActivityType.VortragTauschen:
                    return MeinPlan;

                case ActivityType.MailSenden:
                    return MailAntwort;

                case ActivityType.ExterneAnfrageAblehnen:
                    return MeineRedner;

                case ActivityType.ExterneAnfrageBestätigen:
                    return MeineRedner;

                default:
                    return Sonstige;
            }
        }
    }
}