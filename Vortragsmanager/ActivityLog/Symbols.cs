using System;
using System.Windows.Media.Imaging;

namespace Vortragsmanager.ActivityLog
{
    public static class Symbols
    {
        private static readonly BitmapImage MeinPlanMailAbsage = new BitmapImage(new Uri("/Images/MeinPlanMailAbsage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailZusage = new BitmapImage(new Uri("/Images/MeinPlanMailZusage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailAnfrage = new BitmapImage(new Uri("/Images/MeinPlanMailAnfrage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanBuchungLöschen = new BitmapImage(new Uri("/Images/MeinPlanBuchungLöschen_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanBuchungBearbeiten = new BitmapImage(new Uri("/Images/MeinPlanBuchungBearbeiten_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanBuchungNeu = new BitmapImage(new Uri("/Images/MeinPlanBuchungNeu_32x32.png", UriKind.Relative));

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

                case Types.EreignisLöschen:
                case Types.BuchungLöschen:
                    return MeinPlanBuchungLöschen;

                case Types.EreignisAnlegen:
                    return MeinPlanBuchungNeu;

                case Types.EinladungBearbeiten:
                case Types.EreignisBearbeiten:
                    return MeinPlanBuchungBearbeiten;

                default:
                    return Sonstige;
            }
        }
    }
}