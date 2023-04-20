using System;
using System.Collections.Generic;
using System.Linq;
using Vortragsmanager.Helper;
using Vortragsmanager.Module;

namespace Vortragsmanager.DataModels
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

        public static string GetMailTextEreignisTauschenAnRednerUndKoordinator(Conregation conregation, Speaker redner, DateTime startDatum, DateTime zielDatum, string vortrag, string versammlung)
        {
            Log.Info(nameof(GetMailTextEreignisTauschenAnRedner));
            if (redner is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mailAdresse = GetMailadresseRedner(redner);
            var mailAdresse2 = GetMailadresseKoordinator(conregation);
            return GetMailTextEreignisTauschenAnRedner(redner.Name, vortrag, versammlung, mailAdresse, redner.Name, startDatum, zielDatum, mailAdresse2, conregation.Koordinator);
        }
        
        private static string GetMailTextEreignisTauschenAnRedner(string name, string vortrag, string versammlung, string mailEmpfänger, string nameEmpfänger, DateTime datumAlt, DateTime datumNeu, string mailEmpfänger2 = null, string nameEmpfänger2 = null)
        {
            if (nameEmpfänger2 != null)
                nameEmpfänger += $", {nameEmpfänger2}";

            if (mailEmpfänger2 != null)
                mailEmpfänger += $", {mailEmpfänger2}";

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

        public static string GetMailTextAnnehmenKoordinator(Outside buchung)
        {
            Log.Info(nameof(GetMailTextAnnehmenKoordinator));
            if (buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText).Inhalt;
            var vortrag = buchung.Ältester?.Vorträge.FirstOrDefault(x => x.Vortrag.Nummer == buchung.Vortrag.Vortrag.Nummer) 
                          ?? buchung.Vortrag;

            mt = ReplaceVersammlungsparameter(mt, buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{DateCalcuation.CalculateWeek(buchung.Kw, buchung.Versammlung):dd.MM.yyyy}, ")
                .Replace("{Redner}", buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", vortrag.VortragMitNummerUndLied)
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);
            return mt;
        }

        public static string GetMailTextAnnehmenRedner(Outside buchung)
        {
            Log.Info(nameof(GetMailTextAnnehmenRedner));
            if (buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, buchung.Versammlung);
            mt = mt
                .Replace("{Redner Name}", buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Redner Mail}", GetMailadresseRedner(buchung.Ältester))
                .Replace("{Redner Versammlung}", buchung.Ältester?.Versammlung.Name ?? "unbekannt")
                .Replace("{Vortrag}", buchung.Vortrag.Vortrag.ToString())
                .Replace("{Datum}", $"{buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt)
                .Replace("{Versammlung Zusammenkunftszeit}", buchung.Zeit.ToString())
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenKoordinator(Outside buchung)
        {
            Log.Info(nameof(GetMailTextAblehnenKoordinator));
            if (buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", buchung.Vortrag?.Vortrag.ToString() ?? "unbekannt")
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenRedner(Outside buchung)
        {
            Log.Info(nameof(GetMailTextAblehnenRedner));
            if (buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", buchung.Vortrag?.Vortrag.ToString() ?? "unbekannt")
                .Replace("{Redner Mail}", GetMailadresseRedner(buchung.Ältester))
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenRednerUndKoordinator(Outside buchung)
        {
            Log.Info(nameof(GetMailTextAblehnenRedner));
            if (buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mailEmpfänger = GetMailadresseRedner(buchung.Ältester) + ", " + GetMailadresseKoordinator(buchung.Versammlung);

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", buchung.Vortrag.Vortrag.ToString())
                .Replace("{Redner Mail}", mailEmpfänger)
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenKoordinator(Invitation zuteilung)
        {
            Log.Info(nameof(GetMailTextAblehnenKoordinator));
            if (zuteilung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            var vers = zuteilung.Ältester?.Versammlung ?? DataContainer.ConregationFind("Unbekannt");

            mt = ReplaceVersammlungsparameter(mt, vers);
            mt = mt
                .Replace("{Datum}", $"{DateCalcuation.CalculateWeek(zuteilung.Kw):dd.MM.yyyy}, ")
                .Replace("{Redner}", zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", zuteilung.Vortrag?.Vortrag?.ToString() ?? "unbekannt")
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextRednerErinnerung(Invitation zuteilung)
        {
            Log.Info(nameof(GetMailTextRednerErinnerung));
            if (zuteilung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var empfängerName = zuteilung.Ältester.Name;
            var empfängerMail = GetMailadresseRedner(zuteilung.Ältester);

            if (empfängerMail == "unbekannt")
            {
                empfängerName = zuteilung.Ältester.Versammlung.Koordinator;
                empfängerMail = GetMailadresseKoordinator(zuteilung.Ältester.Versammlung);
            }

            var mt = GetTemplate(TemplateName.RednerErinnerungMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, zuteilung.Ältester?.Versammlung);
            mt = mt
                .Replace("{MailEmpfänger}", empfängerMail)
                .Replace("{MailName}", empfängerName)
                .Replace("{Datum}", $"{DateCalcuation.CalculateWeek(zuteilung.Kw):dd.MM.yyyy}")
                .Replace("{Redner}", $"{zuteilung.Ältester.Name ?? "unbekannt"}")
                .Replace("{Vortrag}", zuteilung.Vortrag.Vortrag.ToString())
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextAblehnenRedner(Invitation zuteilung)
        {
            Log.Info(nameof(GetMailTextAblehnenRedner));
            if (zuteilung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, zuteilung.Ältester?.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{DateCalcuation.CalculateWeek(zuteilung.Kw):dd.MM.yyyy}, ")
                .Replace("{Redner}", zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", zuteilung.Vortrag?.Vortrag.ToString() ?? "unbekannt")
                .Replace("{Redner Mail}", GetMailadresseRedner(zuteilung.Ältester))
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetMailTextRednerAnfragen(Conregation versammlung, string rednerListe, string terminListe)
        {
            Log.Info(nameof(GetMailTextRednerAnfragen));

            if (versammlung == null)
                return string.Empty;

            var mt = Templates.GetTemplate(TemplateName.RednerAnfragenMailText).Inhalt;

            mt = mt
                .Replace("{Freie Termine}", terminListe)
                .Replace("{Liste Redner}", rednerListe)
                .Replace("{Koordinator Mail}", $"{versammlung.KoordinatorJw}; {versammlung.KoordinatorMail}")
                .Replace("{Koordinator Name}", versammlung.Koordinator)
                .Replace("{Versammlung}", versammlung.Name)
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string GetRednerlisteMailText(List<Speaker> listeRedner, IEnumerable<Outside> talks)
        {
            if (listeRedner == null || talks == null)
                return null;

            var einladungs = talks.ToList();
            var mt = GetTemplate(TemplateName.RednerTermineMailText).Inhalt;

            var mails = "";
            var termine = "";
            var jwpubs = "";

            foreach (var ä in listeRedner)
            {
                if (!string.IsNullOrEmpty(ä.JwMail))
                    jwpubs += $"{ä.JwMail}; ";
                else if (!string.IsNullOrEmpty(ä.Mail))
                    mails += $"{ä.Mail}; ";
                else
                    mails += $"{ä.Name} (Keine Adresse vorhanden); ";
                termine += "-----------------------------------------------------" + Environment.NewLine;
                termine += ä.Name + Environment.NewLine;

                foreach (var einladung in einladungs)
                {
                    if (einladung.Ältester != ä)
                        continue;

                    var zoom = einladung.Versammlung.Zoom;
                    
                    termine += $"\tDatum:      \t{einladung.Datum:dd.MM.yyyy}" + Environment.NewLine;
                    
                    termine += $"\tVortrag:    \t{einladung.Vortrag.Vortrag}" + Environment.NewLine;
                    
                    termine += $"\tVersammlung:\t{einladung.Versammlung.Name}, {einladung.Versammlung.Anschrift1}, {einladung.Versammlung.Anschrift2}".TrimEnd(' ', ',');
                    termine += $" | {einladung.Zeit}";
                    if (!string.IsNullOrWhiteSpace(zoom))
                        termine += " | " + zoom;
                    if (!string.IsNullOrWhiteSpace(einladung.Versammlung.Telefon))
                        termine += $" | {einladung.Versammlung.Telefon}";
                    if (!string.IsNullOrWhiteSpace(einladung.Versammlung.Anreise))
                        termine += $" | {einladung.Versammlung.Anreise}";
                    termine += Environment.NewLine;
                    
                    termine += $"\tKoordinator:\t{einladung.Versammlung.Koordinator}";
                    if (!string.IsNullOrWhiteSpace(einladung.Versammlung.KoordinatorJw + einladung.Versammlung.KoordinatorMail))
                        termine += $" | {einladung.Versammlung.KoordinatorJw}, { einladung.Versammlung.KoordinatorMail}".TrimEnd(' ', ',');
                    if (!string.IsNullOrWhiteSpace(einladung.Versammlung.KoordinatorTelefon+einladung.Versammlung.KoordinatorMobil))
                        termine += $" | {einladung.Versammlung.KoordinatorTelefon}, {einladung.Versammlung.KoordinatorMobil}".TrimEnd(' ', ',');
                    termine += Environment.NewLine + Environment.NewLine;
                }
                termine += Environment.NewLine;
            }
            if (jwpubs.Length >= 2)
                jwpubs = jwpubs.Substring(0, jwpubs.Length - 2);
            if (mails.Length >= 2)
            {
                jwpubs += Environment.NewLine + "Redner ohne JwPub-Adresse:" + mails.Substring(0, mails.Length - 2);
            }

            mt = mt
                .Replace("{Redner Mail}", jwpubs)
                .Replace("{Redner Termine}", termine.Replace(" ,", ""))
                .Replace("{Signatur}", GetTemplate(TemplateName.Signatur).Inhalt);

            return mt;
        }

        public static string ReplaceVersammlungsparameter(string mailtext, Conregation versammlung)
        {
            Log.Info(nameof(ReplaceVersammlungsparameter));
            if (string.IsNullOrEmpty(mailtext))
                return string.Empty;

            if (versammlung is null)
                versammlung = DataContainer.ConregationFind("Unbekannt");

            if (versammlung is null)
                return "Fehler beim verarbeiten der Vorlage '" + mailtext + "'";

            return mailtext
                .Replace("{Versammlung}", versammlung.Name)
                .Replace("{Koordinator Mail}", GetMailadresseKoordinator(versammlung))
                .Replace("{Koordinator Name}", versammlung.Koordinator)
                .Replace("{Kreis}", versammlung.Kreis.ToString(Helper.Helper.German))
                .Replace("{Versammlung Telefon}", versammlung.Telefon)
                .Replace("{Versammlung Anreise}", versammlung.Anreise)
                .Replace("{Versammlung Zoom}", versammlung.Zoom)
                .Replace("{Versammlung Anschrift1}", versammlung.Anschrift1)
                .Replace("{Versammlung Anschrift2}", versammlung.Anschrift2);
        }

        private static string GetMailadresseKoordinator(Conregation versammlung)
        {
            var ergebnis = string.IsNullOrEmpty(versammlung?.KoordinatorJw) ? versammlung?.KoordinatorMail : versammlung.KoordinatorJw;
            return string.IsNullOrWhiteSpace(ergebnis) ? "unbekannt" : ergebnis;
        }

        private static string GetMailadresseRedner(Speaker redner)
        {
            var ergebnis = string.IsNullOrWhiteSpace(redner?.JwMail) ? redner?.Mail : redner.JwMail;
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
                Name = TemplateName.RednerAnfragenMailText,
                Beschreibung = "Diese Vorlage wird verwendet wenn Redner aus einer anderen Versammlung eingeladen werden",
                Inhalt =
@"
Empfänger = {Koordinator Mail}
Betreff = Redner Anfrage
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
                Name = TemplateName.RednerTermineMailText,
                Inhalt =
@"
Empfänger = {Redner Mail}
Betreff = Deine/Eure Vortragseinladungen
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
                Name = TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText,
                Inhalt = @"
Empfänger = {Koordinator Mail}
Betreff = Bestätigung deiner Vortragsanfrage
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
                Name = TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText,
                Inhalt = @"
Empfänger = {Redner Mail}
Betreff = Vortragseinladung
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
                Name = TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText,
                Inhalt = @"
Empfänger = {Koordinator Mail}
Betreff = Abgelehnte Vortragsanfrage
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
                Name = TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText,
                Inhalt = @"
Empfänger = {Redner Mail}
Betreff = Vortragseinladung wurde gelöscht
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
                Name = TemplateName.EreignisTauschenMailText,
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
                Name = TemplateName.RednerErinnerungMailText,
                Inhalt = @"
Empfänger = {MailEmpfänger}
Betreff = Erinnerung an den Vortrag
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
}