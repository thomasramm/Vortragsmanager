using System;
using System.Collections.Generic;
using Vortragsmanager.Models;
using static Vortragsmanager.Core.Templates;

namespace Vortragsmanager.Core
{
    public static class Templates
    {
        public static Dictionary<TemplateName, Template> Vorlagen { get; } = new Dictionary<TemplateName, Template>();

        public static Template GetTemplate(TemplateName name)
        {
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
        }

        public static string GetMailTextEreignisTauschenAnKoordinator(Conregation conregation, DateTime startDatum, DateTime zielDatum, string name, string vortrag, string versammlung)
        {
            if (conregation is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mailAdresse = string.IsNullOrEmpty(conregation.KoordinatorJw) ? conregation.KoordinatorMail : conregation.KoordinatorJw;

            return GetMailTextEreignisTauschenAnRedner(name, vortrag, versammlung, mailAdresse, conregation.Koordinator, startDatum, zielDatum);
        }

        public static string GetMailTextEreignisTauschenAnRedner(Speaker redner, DateTime startDatum, DateTime zielDatum, string vortrag, string versammlung)
        {
            if (redner is null)
                return "Fehler beim verarbeiten der Vorlage";

            return GetMailTextEreignisTauschenAnRedner(redner.Name, vortrag, versammlung, redner.Mail, redner.Name, startDatum, zielDatum);
        }

        private static string GetMailTextEreignisTauschenAnRedner(string name, string vortrag, string versammlung, string mailEmpfänger, string nameEmpfänger, DateTime datumAlt, DateTime datumNeu)
        {
            var mt = GetTemplate(TemplateName.EreignisTauschenMailText).Inhalt;
            mt = mt
                .Replace("{Redner}", name)
                .Replace("{Vortrag}", vortrag)
                .Replace("{Versammlung}", versammlung)
                .Replace("{DatumAlt}", datumAlt.ToShortDateString())
                .Replace("{DatumNeu}", datumNeu.ToShortDateString())
                .Replace("{MailName}", nameEmpfänger)
                .Replace("{MailEmpfänger}", mailEmpfänger);
            return mt;
        }

        public static string GetMailTextAnnehmenKoordinator(Outside Buchung)
        {
            if (Buchung is null)
                return "Fehler beim verarbeiten der Vorlage";
            var mt = GetTemplate(TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString());
            return mt;
        }

        public static string GetMailTextAnnehmenRedner(Outside Buchung)
        {
            if (Buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Buchung.Versammlung);
            mt = mt
                .Replace("{Redner Name}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Redner Mail}", Buchung.Ältester?.Mail ?? "unbekannt")
                .Replace("{Redner Versammlung}", Buchung.Ältester?.Versammlung.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")

                .Replace("{Versammlung Zusammenkunftszeit}", Buchung.Versammlung.GetZusammenkunftszeit(Buchung.Datum));

            return mt;
        }

        public static string GetMailTextAblehnenKoordinator(Outside Buchung)
        {
            if (Buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString());

            return mt;
        }

        public static string GetMailTextAblehnenRedner(Outside Buchung)
        {
            if (Buchung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Buchung.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Redner Mail}", $"{Buchung.Ältester?.Mail ?? "unbekannt"}");

            return mt;
        }

        public static string GetMailTextAblehnenKoordinator(Invitation Zuteilung)
        {
            if (Zuteilung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            var vers = Zuteilung.Ältester?.Versammlung ?? DataContainer.FindConregation("Unbekannt");

            mt = ReplaceVersammlungsparameter(mt, vers);
            mt = mt
                .Replace("{Datum}", $"{Zuteilung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Zuteilung.Vortrag?.ToString() ?? "unbekannt");

            return mt;
        }

        public static string ReplaceVersammlungsparameter(string Mailtext, Conregation Versammlung)
        {
            if (string.IsNullOrEmpty(Mailtext))
                return string.Empty;

            if (Versammlung is null)
                Versammlung = DataContainer.FindConregation("Unbekannt");

            if (Versammlung is null)
                return "Fehler beim verarbeiten der Vorlage '" + Mailtext + "'";

            return Mailtext
                .Replace("{Versammlung}", Versammlung.Name)
                .Replace("{Koordinator Mail}", $"{Versammlung.KoordinatorJw}; {Versammlung.KoordinatorMail}")
                .Replace("{Koordinator Name}", Versammlung.Koordinator)
                .Replace("{Kreis}", Versammlung.Kreis.ToString(DataContainer.German))
                .Replace("{Versammlung Telefon}", Versammlung.Telefon)
                .Replace("{Versammlung Anreise}", Versammlung.Anreise)
                .Replace("{Versammlung Anschrift1}", Versammlung.Anschrift1)
                .Replace("{Versammlung Anschrift2}", Versammlung.Anschrift2);
        }

        public static string GetMailTextAblehnenRedner(Invitation Zuteilung)
        {
            if (Zuteilung is null)
                return "Fehler beim verarbeiten der Vorlage";

            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = ReplaceVersammlungsparameter(mt, Zuteilung.Ältester?.Versammlung);
            mt = mt
                .Replace("{Datum}", $"{Zuteilung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Zuteilung.Vortrag.ToString())
                .Replace("{Redner Mail}", $"{Zuteilung.Ältester.Mail ?? "unbekannt"}");

            return mt;
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