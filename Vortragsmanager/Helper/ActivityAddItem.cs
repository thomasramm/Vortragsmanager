using System;
using System.Globalization;
using System.Linq;
using Vortragsmanager.Datamodels;
using DevExpress.Mvvm;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;

namespace Vortragsmanager.ActivityLog
{
    internal static class ActivityAddItem
    {
        private static void Add(ActivityItemViewModel log)
        {
            Messenger.Default.Send(log, Messages.ActivityAdd);
        }

        public static void Outside(Outside buchung, string mailtext1, string mailtext2, bool bestätigen)
        {
            var log = new ActivityItemViewModel
            {
                Typ = bestätigen ? ActivityTypes.ExterneAnfrageBestätigen : ActivityTypes.ExterneAnfrageAblehnen,
                Versammlung = buchung?.Versammlung,
                Redner = buchung?.Ältester,
                Mails = mailtext1,
                KalenderKw = buchung?.Kw ?? -1,
                Vortrag = buchung?.Vortrag?.Vortrag,
            };
            if (!string.IsNullOrEmpty(mailtext2))
                log.Mails += _mailDelimiter + mailtext2;

            Add(log);
        }

        public static void OutsideSendList(Speaker redner, string mailtext)
        {
            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.ExterneAnfrageListeSenden,
                Redner = redner,
                Versammlung = DataContainer.MeineVersammlung,
                Mails = mailtext
            };
            Add(log);
        }

        public static void SendMail(string mailtext, int? maxEntfernung)
        {
            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.SendMail,
                Versammlung = DataContainer.MeineVersammlung,
                Mails = mailtext,
                Objekt = (maxEntfernung == null) ? "Mail an alle Koordinatoren im Kreis" : $"Mail an alle Koordinatoren im Umkreis von {maxEntfernung} km",
            };
            Add(log);
        }

        public static void RednerAnfrageAbgelehnt(Speaker redner, Talk vortrag, string wochen, string mailtext, bool anfrageGelöscht)
        {
            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.RednerAnfrageAbgesagt,
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
            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.RednerAnfrageBestätigt,
                Versammlung = einladung?.Ältester.Versammlung,
                Redner = einladung?.Ältester,
                KalenderKw = einladung?.Kw ?? -1,
                Vortrag = einladung?.Vortrag?.Vortrag,
                Mails = mailtext,
            };

            if (anfrageGelöscht)
                log.Kommentar = Environment.NewLine + "Die komplette Anfrage wurde daraufhin gelöscht, da keine weiteres Datum oder weiterer Redner angefragt wurde";

            Add(log);
        }

        public static void BuchungLöschen(Invitation buchung, string mailtext)
        {
            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.BuchungLöschen,
                Versammlung = buchung.Ältester.Versammlung,
                Redner = buchung.Ältester,
                Mails = mailtext,
                KalenderKw = buchung?.Kw ?? -1,
                Vortrag = buchung?.Vortrag?.Vortrag,
            };

            Add(log);
        }

        public static void EreignisBearbeiten(SpecialEvent ereignis, ActivityTypes typ)
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

            var log = new ActivityItemViewModel
            {
                Typ = typ,
                Versammlung = DataContainer.MeineVersammlung,
                KalenderKw = ereignis?.Kw ?? -1,
                Vortrag = ereignis.Vortrag?.Vortrag,
                Objekt = objekt
            };

            Add(log);
        }

        public static void EinladungBearbeiten(Invitation buchung, Speaker rednerNeu, TalkSong vortragNeu)
        {
            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.RednerBearbeiten,
                Versammlung = rednerNeu.Versammlung,
                Redner = rednerNeu,
                KalenderKw = buchung.Kw,
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
            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.RednerEintragen,
                Versammlung = buchung.Ältester.Versammlung,
                Redner = buchung.Ältester,
                KalenderKw = buchung.Kw,
                Vortrag = buchung.Vortrag.Vortrag,
            };
            Add(log);
        }

        public static void RednerAnfragen(Inquiry anfrage)
        {
            if (anfrage == null)
                return;

            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.RednerAnfragen,
                Versammlung = anfrage.Versammlung,
                Objekt = string.Empty,
                Mails = anfrage.Mailtext,
            };

            if (anfrage.Kws.Count == 1)
                log.KalenderKw = anfrage.Kws.First();
            else
            {
                log.Objekt = "Datum: ";
                foreach (var d in anfrage.Kws)
                {
                    log.Objekt += DateCalcuation.CalculateWeek(d).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) + ", ";
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
            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.RednerErinnern,
                Versammlung = buchung.Ältester.Versammlung,
                Redner = buchung.Ältester,
                Mails = mailtext,
                KalenderKw = buchung.Kw,
                Vortrag = buchung.Vortrag?.Vortrag,
            };

            Add(log);
        }

        public static void BuchungVerschieben(IEvent buchung, string mailtext, DateTime datumAlt, string zielBuchung, string header)
        {
            Conregation versammlung = null;
            Speaker redner = null;
            Talk vortrag = null;
            int kw = buchung.Kw;

            var objekt = $"{zielBuchung}{Environment.NewLine}" +
                         $"Datum: " + datumAlt.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
                         + " → " + DateCalcuation.CalculateWeek(buchung.Kw).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

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

            var log = new ActivityItemViewModel
            {
                Typ = ActivityTypes.BuchungVerschieben,
                Versammlung = versammlung,
                Redner = redner,
                Mails = mailtext,
                KalenderKw = kw,
                Vortrag = vortrag,
                Objekt = objekt,
                Kommentar = header
            };

            Add(log);
        }

        private const string _mailDelimiter = "\r\n=========================\r\n";
    }
}