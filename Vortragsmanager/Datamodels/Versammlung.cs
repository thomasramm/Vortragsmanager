using System.Collections.Generic;
using System.Linq;

namespace Vortragsmanager.DataModels
{
    public class Versammlung
    {
        public string Name { get; set; }

        public string Anschrift { get; set; }

        public string Kontaktinformationen { get; set; }

        public string GetZusammenkunftszeit(int jahr)
        {
            var letztesJahr = _zusammenkunftszeiten.Where(x => x.Key <= jahr).Max(y => y.Key);
            return letztesJahr == null ? "unbekannt" : _zusammenkunftszeiten[letztesJahr];
        }

        public void SetZusammenkunftszeit(int jahr, string zeit)
        {
            if (_zusammenkunftszeiten.ContainsKey(jahr))
                _zusammenkunftszeiten[jahr] = zeit;
            else
                _zusammenkunftszeiten.Add(jahr, zeit);
        }

        private readonly Dictionary<int, string> _zusammenkunftszeiten = new Dictionary<int, string>(1);
    }
}
