using System;
using System.Collections.Generic;
using System.Linq;
using Vortragsmanager.Models;
using static Vortragsmanager.Core.Templates;

namespace Vortragsmanager.Core
{
    public static class Templates
    {
        public static Dictionary<TemplateName, Template> Vorlagen { get; } = new Dictionary<TemplateName, Template>();

        public static Template GetTemplate(TemplateName name)
        {
            Log.Info(nameof(GetTemplate), name.ToString());
            return Vorlagen[name];
        }

        public enum TemplateName
        {
            RednerAnfragenMailText = 1,
            RednerTermineMailText = 2,
            ExterneAnfrageAblehnenInfoAnKoordinatorMailText = 3,
            ExterneAnfrageAblehnenInfoAnRednerMailText = 4,
            ExterneAnfrageAnnehmenInfoAnKoordinatorMailText = 5,
            ExterneAnfrageAnnehmenInfoAnRednerMailText = 6,
            EreignisTauschenMailText = 7,
            RednerErinnerungMailText = 8,
            Signatur = 100,
        }

        public static string GetMailTextEreignisTauschenAnKoordinator(Conregation conregation, DateTime startDatum, DateTime zielDatum, string name, string vortrag, string versammlung)
        {
            Log.Info(nameof(GetMailTextEreignisTauschenAnKoordinator));
            if (conregation is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mailAdresse = GetMailadresseKoordinator(conregation);
            return GetMailTextEreignisTauschenAnRedner(name, vortrag, versammlung, mailAdresse, conregation.Koordinator, startDatum, zielDatum);
        }

        public static string GetMailTextEreignisTauschenAnRedner(Speaker redner, DateTime startDatum, DateTime zielDatum, string vortrag, string versammlung)
        {
            Log.Info(nameof(GetMailTextEreignisTauschenAnRedner));
            if (redner is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mailAdresse = GetMailadresseRedner(redner);
            return GetMailTextEreignisTauschenAnRedner(redner.Name, vortrag, versammlung, mailAdresse, redner.Name, startDatum, zielDatum);
        }

        private static string GetMailTextEreignisTauschenAnRedner(string name, string vortrag, string versammlung, string mailEmpfänger, string nameEmpfänger, DateTime datumAlt, DateTime datumNeu)
        {
            Log.Info(nameof(GetMailTextEreignisTauschenAnRedner));
            var mt = GetTemplate(TemplateName.EreignisTauschenMailText).Inhalt;
            mt = mt
                .Replace("{Redner}", name)
                .Replace("{Vortrag}", vortrag)
                .Replace("{Versammlung}", versammlung)
                .Replace("{DatumAlt}", datumAlt.ToShortDateString())
                .Replace("{DatumNeu}", datumNeu.ToShortDateString())
                .Replace("{MailName}", nameEmpfänger)
                .Replace("{MailEmpfänger}", mailEmpfänger)
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);
            return mt;
        }

        public static string GetMailTextAnnehmenKoordinator(Outside Buchung)
        {
            Log.Info(nameof(GetMailTextAnnehmenKoordinator));
            if (Buchung is null)
                return "Fehler beim verarbeiten der Vorlage";
            var mt = GetTemplate(TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText).Inhalt;
            var vortrag = Buchung.Ältester?.Vorträge.FirstOrDefault(x => x.Vortrag.Nummer == Buchung.Vortrag.Vortrag.Nummer);
            if (vortrag is null)
                vortrag = Buchung.Vortrag;
            mt = ReplaceVersammlungsparameter(mt, Buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", vortrag.VortragMitNummerUndLied)
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);
            return mt;
        }

        public static string GetMailTextAnnehmenRedner(Outside Buchung)
        {
            Log.Info(nameof(GetMailTextAnnehmenRedner));
            if (Buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Buchung.Versammlung);
            mt = mt
                .Replace("{Redner Name}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Redner Mail}", GetMailadresseRedner(Buchung.Ältester))
                .Replace("{Redner Versammlung}", Buchung.Ältester?.Versammlung.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.Vortrag.ToString())
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt)
                .Replace("{Versammlung Zusammenkunftszeit}", Buchung.Versammlung.GetZusammenkunftszeit(Buchung.Datum))
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenKoordinator(Outside Buchung)
        {
            Log.Info(nameof(GetMailTextAblehnenKoordinator));
            if (Buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.Vortrag.ToString())
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenRedner(Outside Buchung)
        {
            Log.Info(nameof(GetMailTextAblehnenRedner));
            if (Buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.Vortrag.ToString())
                .Replace("{Redner Mail}", GetMailadresseRedner(Buchung.Ältester))
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenKoordinator(Invitation Zuteilung)
        {
            Log.Info(nameof(GetMailTextAblehnenKoordinator));
            if (Zuteilung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            var vers = Zuteilung.Ältester?.Versammlung ?? DataContainer.FindConregation("Unbekannt");

            mt = ReplaceVersammlungsparameter(mt, vers);
            mt = mt
                .Replace("{Datum}", $"{Zuteilung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Zuteilung.Vortrag?.Vortrag?.ToString() ?? "unbekannt")
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextRednerErinnerung(Invitation Zuteilung)
        {
            Log.Info(nameof(GetMailTextRednerErinnerung));
            if (Zuteilung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var EmpfängerName = Zuteilung.Ältester.Name;
            var EmpfängerMail = GetMailadresseRedner(Zuteilung.Ältester);

            if (string.IsNullOrWhiteSpace(EmpfängerMail))
            {
                EmpfängerName = Zuteilung.Ältester.Versammlung.Koordinator;
                EmpfängerMail = GetMailadresseKoordinator(Zuteilung.Ältester.Versammlung);
            }

            var mt = GetTemplate(TemplateName.RednerErinnerungMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Zuteilung.Ältester?.Versammlung);
            mt = mt
                .Replace("{MailEmpfänger}", EmpfängerMail)
                .Replace("{MailName}", EmpfängerName)
                .Replace("{Datum}", $"{Zuteilung.Datum:dd.MM.yyyy}")
                .Replace("{Redner}", $"{Zuteilung.Ältester.Name ?? "unbekannt"}")
                .Replace("{Vortrag}", Zuteilung.Vortrag.Vortrag.ToString())
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenRedner(Invitation Zuteilung)
        {
            Log.Info(nameof(GetMailTextAblehnenRedner));
            if (Zuteilung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Zuteilung.Ältester?.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{Zuteilung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Zuteilung.Vortrag.Vortrag.ToString())
                .Replace("{Redner Mail}", GetMailadresseRedner(Zuteilung.Ältester))
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string ReplaceVersammlungsparameter(string Mailtext, Conregation Versammlung)
        {
            Log.Info(nameof(ReplaceVersammlungsparameter));
            if (string.IsNullOrEmpty(Mailtext))
                return string.Empty;

            if (Versammlung is null)
                Versammlung = DataContainer.FindConregation("Unbekannt");

            if (Versammlung is null)
                return "Fehler beim verarbeiten der Vorlage '" + Mailtext + "'";

            return Mailtext
                .Replace("{Versammlung}", Versammlung.Name)
                .Replace("{Koordinator Mail}", GetMailadresseKoordinator(Versammlung))
                .Replace("{Koordinator Name}", Versammlung.Koordinator)
                .Replace("{Kreis}", Versammlung.Kreis.ToString(DataContainer.German))
                .Replace("{Versammlung Telefon}", Versammlung.Telefon)
                .Replace("{Versammlung Anreise}", Versammlung.Anreise)
                .Replace("{Versammlung Zoom}", Versammlung.Zoom)
                .Replace("{Versammlung Anschrift1}", Versammlung.Anschrift1)
                .Replace("{Versammlung Anschrift2}", Versammlung.Anschrift2);
        }

        private static string GetMailadresseKoordinator(Conregation versammlung)
        {
            var ergebnis = string.IsNullOrEmpty(versammlung?.KoordinatorJw) ? versammlung?.KoordinatorMail : versammlung?.KoordinatorJw;
            return string.IsNullOrWhiteSpace(ergebnis) ? "unbekannt" : ergebnis;
        }

        private static string GetMailadresseRedner(Speaker redner)
        {
            var ergebnis = string.IsNullOrWhiteSpace(redner?.JwMail) ? redner?.Mail : redner?.JwMail;
            return string.IsNullOrWhiteSpace(ergebnis) ? "unbekannt" : ergebnis;
        }

        #region Mailvorlagen

        public static void Load()
        {
            Log.Info(nameof(Load));
            Templates.Vorlagen.Clear();
            var t = LoadSignatur();
            Templates.Vorlagen.Add(t.Name, t);

            t = LoadRednerAnfragenMailText();
            Templates.Vorlagen.Add(t.Name, t);

            t = LoadRednerTermineMailText();
            Templates.Vorlagen.Add(t.Name, t);

            t = LoadExterneAnfrageAblehnenTextRedner();
            Templates.Vorlagen.Add(t.Name, t);

            t = LoadExterneAnfrageAblehnenTextKoordinator();
            Templates.Vorlagen.Add(t.Name, t);

            t = LoadExterneAnfrageAnnehmenTextRedner();
            Templates.Vorlagen.Add(t.Name, t);

            t = LoadExterneAnfrageAnnehmenTextKoordinator();
            Templates.Vorlagen.Add(t.Name, t);

            t = LoadEreignisTauschenMailText();
            Templates.Vorlagen.Add(t.Name, t);

            t = LoadRednerErinnerungMailText();
            Templates.Vorlagen.Add(t.Name, t);
        }

        public static string LoadInhalt(TemplateName name)
        {
            switch (name)
            {
                case TemplateName.RednerAnfragenMailText:
                    return Templates.LoadRednerAnfragenMailText().Inhalt;

                case TemplateName.RednerTermineMailText:
                    return Templates.LoadRednerTermineMailText().Inhalt;

                case TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText:
                    return Templates.LoadExterneAnfrageAblehnenTextKoordinator().Inhalt;

                case TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText:
                    return Templates.LoadExterneAnfrageAblehnenTextRedner().Inhalt;

                case TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText:
                    return Templates.LoadExterneAnfrageAnnehmenTextKoordinator().Inhalt;

                case TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText:
                    return Templates.LoadExterneAnfrageAnnehmenTextRedner().Inhalt;

                case TemplateName.EreignisTauschenMailText:
                    return Templates.LoadEreignisTauschenMailText().Inhalt;

                case TemplateName.RednerErinnerungMailText:
                    return Templates.LoadRednerErinnerungMailText().Inhalt;

                case TemplateName.Signatur:
                    return Templates.LoadSignatur().Inhalt;

                default:
                    return string.Empty;
            }
        }

        private static Template LoadRednerAnfragenMailText()
        {
            Log.Info(nameof(LoadRednerAnfragenMailText));
            var x = new Template
            {
                Name = Templates.TemplateName.RednerAnfragenMailText,
                Beschreibung = "Diese Vorlage wird verwendet wenn Redner aus einer anderen Versammlung eingeladen werden",
                Inhalt =
@"
Empfänger = {Koordinator Mail}
------------------------------
An Versammlung {Versammlung}

Hallo {Koordinator Name}.
Ich würde gerne folgende Vortragsredner mit den angegebenen Vorträgen bei uns in die Versammlung einladen.
{Liste Redner}

Folgende Termine sind in meiner Vortragsplannung aktuell noch frei:
{Freie Termine}.

Ich würde mich freuen von dir zu hören.

{Signatur}"
            };
            x.Parameter.Add("{Koordinator Mail}", "Mailadressen des Koordinator an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Koordinator Name}", "Name des Koordinator an den die Absage geschickt werden soll, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Liste Redner}", "Liste der angefragten Redner mit deren Vortrag");
            x.Parameter.Add("{Freie Termine}", "Liste der Angefragten Termine");
            x.Parameter.Add("{Signatur}", "Deine Signatur");
            return x;
        }

        private static Template LoadRednerTermineMailText()
        {
            Log.Info(nameof(LoadRednerTermineMailText));
            var x = new Template
            {
                Name = Templates.TemplateName.RednerTermineMailText,
                Inhalt =
@"
Empfänger = {Redner Mail}
------------------------------
Hallo,
anbei die Liste deiner/eurer Vortragseinladungen:

{Redner Termine}

{Signatur}"
            };
            x.Parameter.Add("{Redner Mail}", "Mailadressen der Redner die in der Liste aufgeführt sind, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Redner Termine}", "Die gewählten Vortragseinladungen");
            x.Parameter.Add("{Signatur}", "Deine Signatur");
            return x;
        }

        private static Template LoadExterneAnfrageAnnehmenTextKoordinator()
        {
            Log.Info(nameof(LoadExterneAnfrageAnnehmenTextKoordinator));
            var x = new Template
            {
                Name = Templates.TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText,
                Inhalt = @"
Empfänger = {Koordinator Mail}
------------------------------
An Versammlung {Versammlung}

Hallo {Koordinator Name},

folgende Vortragsanfrage bestätige ich dir hiermit:
Datum: {Datum}
Redner: {Redner}
Vortrag: {Vortrag}

{Signatur}"
            };
            x.Parameter.Add("{Koordinator Mail}", "Mailadressen des Koordinator an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Koordinator Name}", "Name des Koordinator an den die Absage geschickt werden soll, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Datum", "Datum der Anfrage, z.B. '05.11.2019'");
            x.Parameter.Add("{Redner}", "Name des angefragten Redner, z.B. 'Max Vortragsredner'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            x.Parameter.Add("{Signatur}", "Deine Signatur");

            return x;
        }

        private static Template LoadExterneAnfrageAnnehmenTextRedner()
        {
            Log.Info(nameof(LoadExterneAnfrageAnnehmenTextRedner));
            var x = new Template
            {
                Name = Templates.TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText,
                Inhalt = @"
Empfänger = {Redner Mail}
------------------------------

Hallo {Redner Name},

folgende Vortragseinladung habe ich für dich:
Datum: {Datum}
Vortrag: {Vortrag}
Versammlung: {Versammlung}

Anschrift des Königreichssaals:
{Versammlung Anschrift1}
{Versammlung Anschrift2}

Zoom:
{Versammlung Zoom}

{Versammlung Telefon}
Zusammenkunftszeit: {Versammlung Zusammenkunftszeit}

{Signatur}"
            };
            x.Parameter.Add("{Redner Mail}", "Mailadressen des Koordinator an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Redner Name}", "Name des Koordinator an den die Absage geschickt werden soll, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Redner Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Datum}", "Datum der Anfrage, z.B. '05.11.2019'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            x.Parameter.Add("{Versammlung}", "Name der einladenden Versammlung, z.B. 'Grebenstein'");
            x.Parameter.Add("{Versammlung Anschrift1}", "Anschrift1 (Straße) der einladenden Versammlung, z.B. 'Grüner Weg 1'");
            x.Parameter.Add("{Versammlung Anschrift2}", "Anschrift2 (Ort) der einladenden Versammlung, z.B. '34371 Grebenstein'");
            x.Parameter.Add("{Versammlung Telefon}", "Telefonnummer der einladenden Versammlung, z.B. '0123-456789'");
            x.Parameter.Add("{Versammlung Zoom}", "Zugangsdaten zu Zoom, z.B. 'ID 12345, Passwd. XXXXX'");
            x.Parameter.Add("{Versammlung Zusammenkunftszeit}", "Zusammenkunftszeit der einladenden Versammlung, z.B. 'Sonntag 10 Uhr'");
            x.Parameter.Add("{Signatur}", "Deine Signatur");

            return x;
        }

        private static Template LoadExterneAnfrageAblehnenTextKoordinator()
        {
            Log.Info(nameof(LoadExterneAnfrageAblehnenTextKoordinator));
            var x = new Template
            {
                Name = Templates.TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText,
                Inhalt = @"
Empfänger = {Koordinator Mail}
------------------------------
An Versammlung {Versammlung}

Hallo {Koordinator Name},

folgende Vortragsanfrage muss ich leider ablehnen:
Datum: {Datum}
Redner: {Redner}
Vortrag: {Vortrag}

{Signatur}"
            };
            x.Parameter.Add("{Koordinator Mail}", "Mailadressen des Koordinator an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Koordinator Name}", "Name des Koordinator an den die Absage geschickt werden soll, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Datum", "Datum der Anfrage, z.B. '05.11.2019'");
            x.Parameter.Add("{Redner}", "Name des angefragten Redner, z.B. 'Max Vortragsredner'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            x.Parameter.Add("{Signatur}", "Deine Signatur");

            return x;
        }

        private static Template LoadExterneAnfrageAblehnenTextRedner()
        {
            Log.Info(nameof(LoadExterneAnfrageAblehnenTextRedner));

            var x = new Template
            {
                Name = Templates.TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText,
                Inhalt = @"
Empfänger = {Redner Mail}
------------------------------
Hallo {Redner},

folgende Vortragsanfrage wurde gelöscht:
Datum: {Datum}
Redner: {Redner}
Vortrag: {Vortrag}
Versammlung: {Versammlung}

{Signatur}"
            };
            x.Parameter.Add("{Redner Mail}", "Mailadresse des Redners an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Datum", "Datum der Anfrage, z.B. '05.11.2019'");
            x.Parameter.Add("{Redner}", "Name des angefragten Redner, z.B. 'Max Vortragsredner'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            x.Parameter.Add("{Signatur}", "Deine Signatur");

            return x;
        }

        private static Template LoadEreignisTauschenMailText()
        {
            Log.Info(nameof(LoadEreignisTauschenMailText));
            var x = new Template
            {
                Name = Templates.TemplateName.EreignisTauschenMailText,
                Inhalt = @"
Empfänger = {MailEmpfänger}
Betreff = Vortrag tauschen
------------------------------

Hallo {MailName},

folgenden Vortrag habe ich in meiner Planung getauscht:
Datum ALT: {DatumAlt}
Datum NEU: {DatumNeu}
Redner: {Redner}
Vortrag: {Vortrag}
Versammlung: {Versammlung}

{Signatur}"
            };

            x.Parameter.Add("{MailEmpfänger}", "Mailadresse des Empfänger an den die Info geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{MailName}", "Name des Empfängers, dies kann entweder der Koordinator oder der Redner sein, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{DatumAlt}", "Bisheriges Datum der Buchung, z.B. '05.11.2019'");
            x.Parameter.Add("{DatumNeu}", "Neues Datum der Buchung, z.B. '05.12.2019'");
            x.Parameter.Add("{Redner}", "Name des angefragten Redner, z.B. 'Max Vortragsredner'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            x.Parameter.Add("{Signatur}", "Deine Signatur");

            return x;
        }

        private static Template LoadRednerErinnerungMailText()
        {
            Log.Info(nameof(LoadRednerErinnerungMailText));
            var x = new Template
            {
                Name = Templates.TemplateName.RednerErinnerungMailText,
                Inhalt = @"
Empfänger = {MailEmpfänger}
Betreff = Vortrag in Versammlung Hofgeismar
------------------------------

Hallo {MailName},

ich möchte hiermit an folgenden Vortrag erinnern und einige Hinweise geben:
Datum: {Datum}
Redner: {Redner}
Vortrag: {Vortrag}

Wir benutzen für unsere Zusammenkünfte aktuell Zoom.

Zugangsdaten
Meeting-ID: XXX-XXX-XXXX
Passwort: GEHEIM

Hinweise zu Zoom
Das Meeting starten wir 30 min vor Programmbeginn, also ab 9:30 Uhr.
Das Melden beim Wachtturm-Studium machen wir ausschließlich per 'Hand - Heben' Funktion von Zoom.
Für deinen Vortrag, das Schlußgebet und Kommentare beim WT Studium würdest du bitte deine Stumm-Schaltung selber aufheben.
Wenn du unseren Meeting Raum testen möchtest, oder andere Fragen hast, kannst du dich gerne an mich wenden.

{Signatur}"
            };

            x.Parameter.Add("{MailEmpfänger}", "Mailadresse des Empfänger an den die Info geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{MailName}", "Name des Empfängers, dies kann entweder der Koordinator oder der Redner sein, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Datum}", "Datum der Buchung, z.B. '05.11.2019'");
            x.Parameter.Add("{Redner}", "Name des angefragten Redner, z.B. 'Max Vortragsredner'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            x.Parameter.Add("{Signatur}", "Deine Signatur");

            return x;
        }

        private static Template LoadSignatur()
        {
            Log.Info(nameof(LoadRednerErinnerungMailText));
            var x = new Template
            {
                Name = TemplateName.Signatur,
                BenutzerdefinierterInhalt = true,
                Beschreibung = "Individuelle Signatur die an jedem Template benutzt werden kann.",
                Inhalt = @"Liebe Grüße
DEIN NAME

Versammlung DEINE VERSAMMLUNG
Versammlungszeit: DEINE VERSAMMLUNGSZEIT
Versammlungsort:  DEINE ANSCHRIFT"
            };

            return x;
        }

        #endregion Mailvorlagen
    }

    public class Template
    {
        public TemplateName Name { get; set; }

        public string Titel => Name.ToString();

        public string Inhalt { get; set; }

        public bool BenutzerdefinierterInhalt { get; set; }

        public string Beschreibung { get; set; }

        public Dictionary<string, string> Parameter { get; } = new Dictionary<string, string>();

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}