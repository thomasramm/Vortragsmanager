using System;
using System.Collections.Generic;
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