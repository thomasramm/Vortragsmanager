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
                KalenderDatum = buchung?.Datum ?? DateTime.MinValue,
                Vortrag = buchung?.Vortrag?.Vortrag,
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

        public static void RednerAnfrageAbgelehnt(Speaker redner, Talk vortrag, string wochen, string mailtext, bool anfrageGelöscht)
        {
            var log = new Activity
            {
                Typ = Types.RednerAnfrageAbgesagt,
                Versammlung = redner?.Versammlung,
                Redner = redner,
                Mails = mailtext,
                Vortrag = vortrag,
                Objekt = $"Datum: {wochen}{Environment.NewLine}"
            };

            if (anfrageGelöscht)
                log.Kommentar = "Die komplette Anfrage wurde daraufhin gelöscht, da keine weiteres Datum oder weiterer Redner angefragt wurde";

            Add(log);
        }

        public static void RednerAnfrageZugesagt(Invitation einladung, string mailtext, bool anfrageGelöscht)
        {
            var log = new Activity
            {
                Typ = Types.RednerAnfrageBestätigt,
                Versammlung = einladung?.Ältester.Versammlung,
                Redner = einladung?.Ältester,
                KalenderDatum = einladung?.Datum ?? DateTime.MinValue,
                Vortrag = einladung?.Vortrag?.Vortrag,
                Mails = mailtext,
            };

            if (anfrageGelöscht)
                log.Kommentar = Environment.NewLine + "Die komplette Anfrage wurde daraufhin gelöscht, da keine weiteres Datum oder weiterer Redner angefragt wurde";

            Add(log);
        }

        public static void BuchungLöschen(Invitation buchung, string mailtext)
        {
            var log = new Activity
            {
                Typ = Types.BuchungLöschen,
                Versammlung = buchung.Ältester.Versammlung,
                Redner = buchung.Ältester,
                Mails = mailtext,
                KalenderDatum = buchung?.Datum ?? DateTime.MinValue,
                Vortrag = buchung?.Vortrag?.Vortrag,
            };

            Add(log);
        }

        public static void EreignisBearbeiten(SpecialEvent ereignis, Types typ)
        {
            if (ereignis == null)
                return;

            var objekt = (!string.IsNullOrEmpty(ereignis.Name))
                ? $"Ereignis: {ereignis.Name}{Environment.NewLine}" //EventName
                : $"Ereignis: { ereignis.Typ}{Environment.NewLine}"; //EventName
            if (!string.IsNullOrEmpty(ereignis.Vortragender))
                objekt += $"Redner: {ereignis.Vortragender}{Environment.NewLine}"; //Redner
            if (ereignis.Vortrag == null && !string.IsNullOrEmpty(ereignis.Thema))
                objekt += $"Thema: {ereignis.Thema}";

            var log = new Activity
            {
                Typ = typ,
                Versammlung = DataContainer.MeineVersammlung,
                KalenderDatum = ereignis?.Datum ?? DateTime.MinValue,
                Vortrag = ereignis.Vortrag?.Vortrag,
                Objekt = objekt
            };

            Add(log);
        }

        public static void EinladungBearbeiten(Invitation buchung, Speaker rednerNeu, TalkSong vortragNeu)
        {
            var log = new Activity
            {
                Typ = Types.RednerBearbeiten,
                Versammlung = rednerNeu.Versammlung,
                Redner = rednerNeu,
                KalenderDatum = buchung.Datum,
                Vortrag = buchung.Vortrag.Vortrag,
            };

            log.Objekt = string.Empty;
            if (rednerNeu.Versammlung != buchung.Ältester.Versammlung)
                log.Objekt += $"{Environment.NewLine} | Versammlung: {buchung.Ältester.Versammlung.Name}";

            if (vortragNeu.Vortrag.Nummer != buchung.Vortrag.Vortrag.Nummer)
                log.Objekt += $"{Environment.NewLine} | Vortrag: {buchung?.Vortrag?.Vortrag?.ToString()}";

            if (rednerNeu != buchung.Ältester)
                log.Objekt += $"{Environment.NewLine} | Redner: {buchung.Ältester.Name}";
            if (!string.IsNullOrEmpty(log.Objekt))
                log.Objekt = $"Bisherige Daten:{Environment.NewLine}" + log.Objekt;

            Add(log);
        }

        public static void RednerEintragen(Invitation buchung)
        {
            var log = new Activity
            {
                Typ = Types.RednerEintragen,
                Versammlung = buchung.Ältester.Versammlung,
                Redner = buchung.Ältester,
                KalenderDatum = buchung.Datum,
                Vortrag = buchung.Vortrag.Vortrag,
            };
            Add(log);
        }

        public static void RednerAnfragen(Inquiry anfrage)
        {
            if (anfrage == null)
                return;

            var log = new Activity
            {
                Typ = Types.RednerAnfragen,
                Versammlung = anfrage.Versammlung,
                Objekt = string.Empty,
                Mails = anfrage.Mailtext,
            };

            if (anfrage.Wochen.Count() == 1)
                log.KalenderDatum = anfrage.Wochen.First();
            else
            {
                log.Objekt = "Datum: ";
                foreach (var d in anfrage.Wochen)
                {
                    log.Objekt += d.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) + ", ";
                }
            }
            if (anfrage.RednerVortrag.Keys.Distinct().Count() == 1)
            {
                log.Redner = anfrage.RednerVortrag.Keys.ElementAt(0);
                log.Vortrag = anfrage.RednerVortrag.Values.ElementAt(0);
            }
            else
            {
                log.Objekt += Environment.NewLine + "Redner: ";
                foreach (var r in anfrage.RednerVortrag)
                {
                    log.Objekt += $"{r.Key.Name} | {r.Value.NumberTopicShort}{Environment.NewLine}";
                }
            }

            Add(log);
        }

        public static void RednerErinnern(Invitation buchung, string mailtext)
        {
            var log = new Activity
            {
                Typ = Types.RednerErinnern,
                Versammlung = buchung.Ältester.Versammlung,
                Redner = buchung.Ältester,
                Mails = mailtext,
                KalenderDatum = buchung.Datum,
                Vortrag = buchung.Vortrag?.Vortrag,
            };

            Add(log);
        }

        public static void BuchungVerschieben(IEvent buchung, string mailtext, DateTime datumAlt, string zielBuchung, string header)
        {
            Conregation versammlung = null;
            Speaker redner = null;
            Talk vortrag = null;
            DateTime datum = buchung.Datum;

            var objekt = $"{zielBuchung}{Environment.NewLine}" +
                         $"Datum: " + datumAlt.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
                         + " → " + buchung.Datum.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

            var rednereinladung = (buchung as Invitation);
            if (rednereinladung != null)
            {
                redner = rednereinladung.Ältester;
                versammlung = redner.Versammlung;
                vortrag = rednereinladung.Vortrag.Vortrag;
                objekt += $"{Environment.NewLine}Vortrag: {rednereinladung.Vortrag.Vortrag.NumberTopicShort}";
            }

            var ereignis = (buchung as SpecialEvent);
            if (ereignis != null)
            {
                versammlung = DataContainer.MeineVersammlung;
                if (!string.IsNullOrEmpty(ereignis.Name))
                    objekt += $"{Environment.NewLine}Ereignis: {ereignis.Name}{Environment.NewLine}"; //EventName
                else
                    objekt += $"{Environment.NewLine}Ereignis: {ereignis.Typ}{Environment.NewLine}"; //EventName
                if (!string.IsNullOrEmpty(ereignis.Vortragender))
                    objekt += $"Redner: {ereignis.Vortragender}{Environment.NewLine}"; //Redner
                if (ereignis.Vortrag != null)
                {
                    vortrag = ereignis.Vortrag.Vortrag;
                    objekt += $"Thema: {ereignis.Vortrag.Vortrag.NumberTopicShort}";
                }
                else if (!string.IsNullOrEmpty(ereignis.Thema))
                    objekt += $"Thema: {ereignis.Thema}";
            }

            var anfrage = (buchung as Inquiry);
            if (anfrage != null)
            {
                versammlung = anfrage.Versammlung;
            }

            var log = new Activity
            {
                Typ = Types.BuchungVerschieben,
                Versammlung = versammlung,
                Redner = redner,
                Mails = mailtext,
                KalenderDatum = datum,
                Vortrag = vortrag,
                Objekt = objekt,
                Kommentar = header
            };

            Add(log);
        }

        private const string _mailDelimiter = "\r\n=========================\r\n";
    }
}