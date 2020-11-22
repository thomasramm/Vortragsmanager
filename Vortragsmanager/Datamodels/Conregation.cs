using System;
using System.Collections.Generic;
using System.Linq;
using Vortragsmanager.Core;

namespace Vortragsmanager.Datamodels
{
    public class Conregation
    {
        public int Id { get; set; }

        public int Kreis { get; set; }

        public string Name { get; set; }

        public string Anschrift1 { get; set; }

        public string Anschrift2 { get; set; }

        public string Anreise { get; set; }

        public int Entfernung { get; set; }

        public string Zoom { get; set; }

        public string Telefon { get; set; }

        public string Koordinator { get; set; }

        public string KoordinatorTelefon { get; set; }

        public string KoordinatorMobil { get; set; }

        public string KoordinatorMail { get; set; }

        public string KoordinatorJw { get; set; }

        public string GetZusammenkunftszeit(int Jahr)
        {
            Log.Info(nameof(GetZusammenkunftszeit), Jahr);
            if (Zusammenkunftszeiten.Count == 0)
                return "unbekannt";
            if (Zusammenkunftszeiten.ContainsKey(Jahr))
                return Zusammenkunftszeiten[Jahr];
            var letztesJahr = Zusammenkunftszeiten.Where(x => x.Key <= Jahr).Max(y => y.Key);
            return Zusammenkunftszeiten[letztesJahr];
        }

        public string GetZusammenkunftszeit(DateTime Datum)
        {
            Log.Info(nameof(GetZusammenkunftszeit), Datum);
            return GetZusammenkunftszeit(Datum.Year);
        }

        public void SetZusammenkunftszeit(int Jahr, string Zeit)
        {
            Log.Info(nameof(SetZusammenkunftszeit), $"jahr={Jahr}, Zeit={Zeit}");
            if (Zusammenkunftszeiten.ContainsKey(Jahr))
                Zusammenkunftszeiten[Jahr] = Zeit;
            else
                Zusammenkunftszeiten.Add(Jahr, Zeit);
        }

        public string NameMitKoordinator => $"{Name} ({Koordinator})";

        public override string ToString() => $"Versammlung {Name}";

        public Dictionary<int, string> Zusammenkunftszeiten { get; } = new Dictionary<int, string>(1);
    }
}