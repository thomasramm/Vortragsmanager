using System;
using System.Windows.Media.Imaging;

namespace Vortragsmanager.ActivityLog
{
    public static class Symbols
    {
        private static readonly BitmapImage MeinPlanMailAbsage = new BitmapImage(new Uri("/Images/MeinPlanMailAbsage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailZusage = new BitmapImage(new Uri("/Images/MeinPlanMailZusage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailAnfrage = new BitmapImage(new Uri("/Images/MeinPlanMailAnfrage_32x32.png", UriKind.Relative));

        private static readonly BitmapImage MeineRednerBestätigen = new BitmapImage(new Uri("/Images/MeineRednerBestätigen1_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeineRednerAblehnen = new BitmapImage(new Uri("/Images/MeineRednerAbgelehnt1_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeineRednerMail = new BitmapImage(new Uri("/Images/MeineRednerMail_32x32.png", UriKind.Relative));

        private static readonly BitmapImage Sonstige = new BitmapImage(new Uri("/Images/Sonstige_64x64.png", UriKind.Relative));
        private static readonly BitmapImage Mail = new BitmapImage(new Uri("/Images/Mail_32x32.png", UriKind.Relative));

        public static BitmapImage GetImage(Types typ)
        {
            switch (typ)
            {
                case Types.ExterneAnfrageAblehnen:
                    return MeineRednerAblehnen;

                case Types.ExterneAnfrageBestätigen:
                    return MeineRednerBestätigen;

                case Types.ExterneAnfrageListeSenden:
                    return MeineRednerMail;

                case Types.SendMail:
                    return Mail;

                case Types.RednerAnfrageBestätigt:
                    return MeinPlanMailZusage;

                case Types.RednerAnfrageAbgesagt:
                    return MeinPlanMailAbsage;

                case Types.RednerAnfragen:
                    return MeinPlanMailAnfrage;

                default:
                    return Sonstige;
            }
        }
    }
}