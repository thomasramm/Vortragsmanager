using System.Collections.Generic;

namespace Vortragsmanager.DataModels
{
    public class Template
    {
        public Templates.TemplateName Name { get; set; }

        //public string Titel => Name.ToString();

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