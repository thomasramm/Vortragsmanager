using System;
using System.Windows.Media.Imaging;
using Vortragsmanager.Enums;

namespace Vortragsmanager.Helper
{
    public static class ActivityGetSymbols
    {
        private static readonly BitmapImage MeinPlanMailAbsage = new BitmapImage(new Uri("/Images/MeinPlanMailAbsage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailZusage = new BitmapImage(new Uri("/Images/MeinPlanMailZusage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanMailAnfrage = new BitmapImage(new Uri("/Images/MeinPlanMailAnfrage_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanBuchungLöschen = new BitmapImage(new Uri("/Images/MeinPlanBuchungLöschen_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanBuchungBearbeiten = new BitmapImage(new Uri("/Images/MeinPlanBuchungBearbeiten_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanBuchungNeu = new BitmapImage(new Uri("/Images/MeinPlanBuchungNeu_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeinPlanBuchungVerschieben = new BitmapImage(new Uri("/Images/MeinPlanBuchungVerschieben_32x32.png", UriKind.Relative));

        private static readonly BitmapImage MeineRednerBestätigen = new BitmapImage(new Uri("/Images/MeineRednerBestätigen1_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeineRednerAblehnen = new BitmapImage(new Uri("/Images/MeineRednerAbgelehnt1_32x32.png", UriKind.Relative));
        private static readonly BitmapImage MeineRednerMail = new BitmapImage(new Uri("/Images/MeineRednerMail_32x32.png", UriKind.Relative));

        private static readonly BitmapImage Sonstige = new BitmapImage(new Uri("/Images/Sonstige_64x64.png", UriKind.Relative));
        private static readonly BitmapImage Mail = new BitmapImage(new Uri("/Images/Mail_32x32.png", UriKind.Relative));

        public static BitmapImage GetImage(ActivityTypes typ)
        {
            switch (typ)
            {
                case ActivityTypes.ExterneAnfrageAblehnen:
                    return MeineRednerAblehnen;

                case ActivityTypes.ExterneAnfrageBestätigen:
                    return MeineRednerBestätigen;

                case ActivityTypes.ExterneAnfrageListeSenden:
                    return MeineRednerMail;

                case ActivityTypes.SendMail:
                    return Mail;

                case ActivityTypes.RednerAnfrageBestätigt:
                    return MeinPlanMailZusage;

                case ActivityTypes.RednerAnfrageAbgesagt:
                    return MeinPlanMailAbsage;

                case ActivityTypes.RednerAnfragen:
                case ActivityTypes.RednerErinnern:
                    return MeinPlanMailAnfrage;

                case ActivityTypes.EreignisLöschen:
                case ActivityTypes.BuchungLöschen:
                    return MeinPlanBuchungLöschen;

                case ActivityTypes.EreignisAnlegen:
                case ActivityTypes.RednerEintragen:
                    return MeinPlanBuchungNeu;

                case ActivityTypes.RednerBearbeiten:
                case ActivityTypes.EreignisBearbeiten:
                    return MeinPlanBuchungBearbeiten;

                case ActivityTypes.BuchungVerschieben:
                    return MeinPlanBuchungVerschieben;

                default:
                    return Sonstige;
            }
        }
    }
}