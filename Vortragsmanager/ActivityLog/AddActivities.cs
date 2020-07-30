using System;
using System.Globalization;
using System.Linq;
using Vortragsmanager.Datamodels;
using DevExpress.Mvvm;

namespace Vortragsmanager.ActivityLog
{
    internal static class AddActivity
    {
        private static void Add(Activity log)
        {
            Messenger.Default.Send(log, Messages.ActivityAdd);
        }

        public static void Outside(Outside buchung, string mailtext1, string mailtext2, bool bestätigen)
        {
            var log = new Activity
            {
                Typ = bestätigen ? Types.ExterneAnfrageBestätigen : Types.ExterneAnfrageAblehnen,
                Versammlung = buchung?.Versammlung,
                Redner = buchung?.Ältester,
                Mails = mailtext1,
                Objekt = $"{buchung?.Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)} : {buchung?.Vortrag?.Vortrag.ToString()}",
            };
            if (!string.IsNullOrEmpty(mailtext2))
                log.Mails += _mailDelimiter + mailtext2;

            Add(log);
        }

        public static void OutsideSendList(Speaker redner, string mailtext)
        {
            var log = new Activity
            {
                Typ = Types.ExterneAnfrageListeSenden,
                Redner = redner,
                Versammlung = DataContainer.MeineVersammlung,
                Mails = mailtext
            };
            Add(log);
        }

        public static void SendMail(string mailtext, int? maxEntfernung)
        {
            var log = new Activity
            {
                Typ = Types.SendMail,
                Versammlung = DataContainer.MeineVersammlung,
                Mails = mailtext,
                Objekt = (maxEntfernung == null) ? "Mail an alle Koordinatoren im Kreis" : $"Mail an alle Koordinatoren im Umkreis von {maxEntfernung} km",
            };
            Add(log);
        }

        public static void RednerAnfrageAbgelehnt(Speaker redner, string vortrag, string wochen, string mailtext, bool anfrageGelöscht)
        {
            var log = new Activity
            {
                Typ = Types.RednerAnfrageAbgesagt,
                Versammlung = redner?.Versammlung,
                Redner = redner,
                Mails = mailtext,
                Objekt = $"Datum:   {wochen}{Environment.NewLine}" +
                         $"Vortrag: {vortrag}",
            };

            if (anfrageGelöscht)
                log.Objekt += Environment.NewLine + "Die komplette Anfrage wurde daraufhin gelöscht, da keine weiteres Datum oder weiterer Redner angefragt wurde";

            Add(log);
        }

        public static void RednerAnfrageZugesagt(Invitation einladung, string mailtext, bool anfrageGelöscht)
        {
            var log = new Activity
            {
                Typ = Types.RednerAnfrageBestätigt,
                Versammlung = einladung?.Ältester.Versammlung,
                Redner = einladung?.Ältester,
                Objekt = $"Datum:   {einladung?.Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}{Environment.NewLine}" +
                         $"Vortrag: {einladung?.Vortrag?.Vortrag.ToString()}",
                Mails = mailtext,
            };
            if (anfrageGelöscht)
                log.Objekt += Environment.NewLine + "Die komplette Anfrage wurde daraufhin gelöscht, da keine weiteres Datum oder weiterer Redner angefragt wurde";

            Add(log);
        }

        public static void RednerAnfragen(Inquiry anfrage)
        {
            if (anfrage == null)
                return;
            var objekt = "Datum: ";
            foreach (var d in anfrage.Wochen)
            {
                objekt += d.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) + ", ";
            }
            objekt += Environment.NewLine + "Redner: ";
            foreach (var r in anfrage.RednerVortrag)
            {
                objekt += $"{r.Key.Name} | {r.Value.NumberTopicShort}{Environment.NewLine}";
            }

            var redner = (anfrage.RednerVortrag.Keys.Distinct().Count() == 1) ? anfrage.RednerVortrag.Keys.ElementAt(0) : null;

            var log = new Activity
            {
                Typ = Types.RednerAnfragen,
                Versammlung = anfrage.Versammlung,
                Objekt = objekt,
                Mails = anfrage.Mailtext,
                Redner = redner,
            };

            Add(log);
        }

        private const string _mailDelimiter = "\r\n=========================\r\n";
    }
}