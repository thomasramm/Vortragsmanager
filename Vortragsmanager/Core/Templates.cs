using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Vortragsmanager.Core.Templates;

namespace Vortragsmanager.Core
{
    public static class Templates
    {
        public static Dictionary<TemplateName,Template> Vorlagen { get; } = new Dictionary<TemplateName, Template>();

        public static Template GetTemplate(TemplateName name)
        {
            return Vorlagen[name];
        }

        //ToDo: Einlesen von Templates aus Ordner oder Datei
        public static void LoadTemplates()
        {
            LoadRednerAnfragenMailText();
            LoadRednerTermineMailText();
            LoadExterneAnfrageAblehnenText();
            LoadExterneAnfrageAnnehmenText();
        }

        private static void LoadRednerAnfragenMailText()
        {
            var x = new Template
            {
                Name = TemplateName.RednerAnfragenMailText,
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

Liebe Grüße
Thomas Ramm

Versammlung Hofgeismar
Versammlungszeit: Sonntag 10:00Uhr
Versammlungsort:  34393 Grebenstein, Über der Bahn
"
            };
            x.Parameter.Add("{Koordinator Mail}", "Mailadressen des Koordinator an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Koordinator Name}", "Name des Koordinator an den die Absage geschickt werden soll, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Liste Redner}", "Liste der angefragten Redner mit deren Vortrag");
            x.Parameter.Add("{Freie Termine}", "Liste der Angefragten Termine");
            Vorlagen.Add(x.Name, x);
        }    

        private static void LoadRednerTermineMailText()
        {
            var x = new Template
            {
                Name = TemplateName.RednerTermineMailText,
                Inhalt =
@"
Empfänger = {Redner Mail}
------------------------------
Hallo,
anbei die Liste deiner/eurer Vortragseinladungen:

{Redner Termine}

Liebe Grüße
Thomas Ramm

Versammlung Hofgeismar
Versammlungszeit: Sonntag 10:00Uhr
Versammlungsort:  34393 Grebenstein, Über der Bahn
"
            };
            x.Parameter.Add("{Redner Mail}", "Mailadressen der Redner die in der Liste aufgeführt sind, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Redner Termine}", "Die gewählten Vortragseinladungen");
            Vorlagen.Add(x.Name, x);
        }

        private static void LoadExterneAnfrageAnnehmenText()
        {
            var x = new Template
            {
                Name = TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText,
                Inhalt = @"
Empfänger = {Koordinator Mail}
------------------------------
An Versammlung {Versammlung}

Hallo {Koordinator Name},

folgende Vortragsanfrage bestätige ich dir hiermit:
Datum: {Datum}
Redner: {Redner}
Vortrag: {Vortrag}

Liebe Grüße
Thomas Ramm

Versammlung Hofgeismar
Versammlungszeit: Sonntag 10:00Uhr
Versammlungsort:  34393 Grebenstein, Über der Bahn"
            };
            x.Parameter.Add("{Koordinator Mail}", "Mailadressen des Koordinator an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Koordinator Name}", "Name des Koordinator an den die Absage geschickt werden soll, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Datum", "Datum der Anfrage, z.B. '05.11.2019'");
            x.Parameter.Add("{Redner}", "Name des angefragten Redner, z.B. 'Max Vortragsredner'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            Vorlagen.Add(x.Name, x);

            x = new Template
            {
                Name = TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText,
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

{Versammlung Telefon}
Zusammenkunftszeit: {Versammlung Zusammenkunftszeit}

Liebe Grüße
Thomas Ramm

Versammlung Hofgeismar
"
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
            x.Parameter.Add("{Versammlung Zusammenkunftszeit}", "Zusammenkunftszeit der einladenden Versammlung, z.B. 'Sonntag 10 Uhr'");

            Vorlagen.Add(x.Name, x);
        }

        private static void LoadExterneAnfrageAblehnenText()
        {
            var x = new Template
            {
                Name = TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText,
                Inhalt = @"
Empfänger = {Koordinator Mail}
------------------------------
An Versammlung {Versammlung}

Hallo {Koordinator Name},

folgende Vortragsanfrage muss ich leider ablehnen:
Datum: {Datum}
Redner: {Redner}
Vortrag: {Vortrag}

Liebe Grüße
Thomas Ramm

Versammlung Hofgeismar
Versammlungszeit: Sonntag 10:00Uhr
Versammlungsort:  34393 Grebenstein, Über der Bahn"
            };
            x.Parameter.Add("{Koordinator Mail}", "Mailadressen des Koordinator an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Koordinator Name}", "Name des Koordinator an den die Absage geschickt werden soll, z.B. 'Gustav Koordinator'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Datum", "Datum der Anfrage, z.B. '05.11.2019'");
            x.Parameter.Add("{Redner}", "Name des angefragten Redner, z.B. 'Max Vortragsredner'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            Vorlagen.Add(x.Name, x);

            x = new Template
            {
                Name = TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText,
                Inhalt = @"
Empfänger = {Redner Mail}
------------------------------
Hallo {Redner},

folgende Vortragsanfrage wurde gelöscht:
Datum: {Datum}
Redner: {Redner}
Vortrag: {Vortrag}

Liebe Grüße
Thomas Ramm

Versammlung Hofgeismar
Versammlungszeit: Sonntag 10:00Uhr
Versammlungsort:  34393 Grebenstein, Über der Bahn"
            };
            x.Parameter.Add("{Redner Mail}", "Mailadresse des Redners an den die Absage geschickt werden soll, z.B. 'mail@jwpub.org; mail@webdienst.de'");
            x.Parameter.Add("{Versammlung}", "Name der Versammlung des eingeladenen Redners, z.B. 'Grebenstein'");
            x.Parameter.Add("{Datum", "Datum der Anfrage, z.B. '05.11.2019'");
            x.Parameter.Add("{Redner}", "Name des angefragten Redner, z.B. 'Max Vortragsredner'");
            x.Parameter.Add("{Vortrag}", "Angefragter Vortrag, z.B. '123 Vortragsthema'");
            Vorlagen.Add(x.Name, x);
        }

        //ToDo: Speichern der Templates in einen Ordner
        public static void SaveTemplates()
        {
            throw new NotImplementedException();
        }

        public enum TemplateName
        {
            RednerAnfragenMailText,
            RednerTermineMailText,
            ExterneAnfrageAblehnenInfoAnKoordinatorMailText,
            ExterneAnfrageAblehnenInfoAnRednerMailText,
            ExterneAnfrageAnnehmenInfoAnKoordinatorMailText,
            ExterneAnfrageAnnehmenInfoAnRednerMailText,
        }
    }

    public class Template
    {
        public TemplateName Name { get; set; }

        public string Titel => Name.ToString();

        public string Inhalt { get; set; }

        public string Beschreibung { get; set; }

        public Dictionary<string, string> Parameter { get; } = new Dictionary<string, string>();

        public override string ToString()
        {
            return Name.ToString();
        }
    }


}
