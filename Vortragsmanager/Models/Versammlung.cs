using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Models
{
    class Versammlung
    {
        public string Name { get; set; }

        public string Anschrift { get; set; }

        public string Kontaktinformationen { get; set; }

        public string GetZusammenkunftszeit(int Jahr)
        {
            var letztesJahr = zusammenkunftszeiten.Where(x => x.Key <= Jahr).Max(y => y.Key);
            return letztesJahr == null ? "unbekannt" : zusammenkunftszeiten[letztesJahr];
        }

        public void SetZusammenkunftszeit(int Jahr, string Zeit)
        {
            if (zusammenkunftszeiten.ContainsKey(Jahr))
                zusammenkunftszeiten[Jahr] = Zeit;
            else
                zusammenkunftszeiten.Add(Jahr, Zeit);
        }

        private readonly Dictionary<int, string> zusammenkunftszeiten = new Dictionary<int, string>(1);
    }
}
