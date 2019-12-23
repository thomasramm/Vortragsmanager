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

        //ToDo: Speichern der Templates in einen Ordner
        public static void SaveTemplates()
        {
            throw new NotImplementedException();
        }

        public enum TemplateName
        {
            RednerAnfragenMailText = 1,
            RednerTermineMailText = 2,
            ExterneAnfrageAblehnenInfoAnKoordinatorMailText = 3,
            ExterneAnfrageAblehnenInfoAnRednerMailText = 4,
            ExterneAnfrageAnnehmenInfoAnKoordinatorMailText = 5,
            ExterneAnfrageAnnehmenInfoAnRednerMailText = 6,
        }

        public static string GetMailTextAnnehmenKoordinator(Outside Buchung)
        {
            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.ExterneAnfrageAnnehmenInfoAnKoordinatorMailText).Inhalt;
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Versammlung}", Buchung.Versammlung.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Koordinator Mail}", $"{Buchung.Versammlung.KoordinatorJw}; {Buchung.Versammlung.KoordinatorMail}")
                .Replace("{Koordinator Name}", Buchung.Versammlung.Koordinator);

            return mt;
        }

        public static string GetMailTextAnnehmenRedner(Outside Buchung)
        {
            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.ExterneAnfrageAnnehmenInfoAnRednerMailText).Inhalt;

            mt = mt
                .Replace("{Redner Name}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Redner Mail}", Buchung.Ältester?.Mail ?? "unbekannt")
                .Replace("{Redner Versammlung}", Buchung.Ältester?.Versammlung.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")

                .Replace("{Versammlung}", Buchung.Versammlung.Name)
                .Replace("{Versammlung Anschrift1}", Buchung.Versammlung.Anschrift1)
                .Replace("{Versammlung Anschrift2}", Buchung.Versammlung.Anschrift2)
                .Replace("{Versammlung Telefon}", Buchung.Versammlung.Telefon)
                .Replace("{Versammlung Zusammenkunftszeit}", Buchung.Versammlung.GetZusammenkunftszeit(Buchung.Datum));

            return mt;
        }

        public static string GetMailTextAblehnenKoordinator(Outside Buchung)
        {
            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Koordinator Mail}", $"{Buchung.Versammlung.KoordinatorJw}; {Buchung.Versammlung.KoordinatorMail}")
                .Replace("{Koordinator Name}", Buchung.Versammlung.Koordinator)
                .Replace("{Versammlung}", Buchung.Versammlung.Name);

            return mt;
        }

        public static string GetMailTextAblehnenRedner(Outside Buchung)
        {
            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = mt
                .Replace("{Datum}", $"{Buchung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", Buchung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", Buchung.Vortrag.ToString())
                .Replace("{Redner Mail}", $"{Buchung.Ältester?.Mail ?? "unbekannt"}")
                .Replace("{Versammlung}", $"{Buchung.Versammlung?.Name ?? "unbekannt"}");

            return mt;
        }

        public static string GetMailTextAblehnenKoordinator(Invitation zuteilung)
        {
            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnKoordinatorMailText).Inhalt;

            var vers = zuteilung.Ältester?.Versammlung ?? DataContainer.FindConregation("Unbekannt");

            mt = mt
                .Replace("{Datum}", $"{zuteilung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner Name}", zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Koordinator Mail}", $"{vers.KoordinatorJw}; {vers.KoordinatorMail}")
                .Replace("{Koordinator Name}", vers.Koordinator)
                .Replace("{Versammlung}", vers.Name);

            return mt;
        }

        public static string GetMailTextAblehnenRedner(Invitation zuteilung)
        {
            var mt = GetTemplate(TemplateName.ExterneAnfrageAblehnenInfoAnRednerMailText).Inhalt;
            mt = mt
                .Replace("{Datum}", $"{zuteilung.Datum:dd.MM.yyyy}, ")
                .Replace("{Redner}", zuteilung.Ältester?.Name ?? "unbekannt")
                .Replace("{Vortrag}", zuteilung.Vortrag.ToString())
                .Replace("{Redner Mail}", $"{zuteilung.Ältester.Mail ?? "unbekannt"}");

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